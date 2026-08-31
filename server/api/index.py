import base64
import os
import time

import psycopg
from cryptography.exceptions import InvalidSignature
from cryptography.hazmat.primitives import hashes
from cryptography.hazmat.primitives.asymmetric import ec
from fastapi import FastAPI, HTTPException
from fastapi.responses import HTMLResponse
from psycopg import errors
from psycopg.rows import dict_row
from pydantic import BaseModel

app = FastAPI(title="list-server")

DATABASE_URL = os.environ["DATABASE_URL"]


def get_connection():
    return psycopg.connect(DATABASE_URL, row_factory=dict_row)


@app.get("/", response_class=HTMLResponse)
def root() -> str:
    return "<!doctype html><html><head><title>Lotsalists App Server</title></head><body><h1>Lotsalists App Server</h1></body></html>"


class GetUserRequest(BaseModel):
    name: str


class GetUserResponse(BaseModel):
    publicKey: str
    image: int


class RegisterRequest(BaseModel):
    name: str
    publicKey: str
    image: int


class RegisterResponse(BaseModel):
    success: bool


class DeleteUserRequest(BaseModel):
    name: str
    adminName: str
    timestamp: str
    proofOfPossession: str


class DeleteUserResponse(BaseModel):
    success: bool


# proofOfPossession must be stale-checked against the request timestamp: not
# more than this many seconds old...
MAX_TIMESTAMP_AGE_SECONDS = 300
# ...and not from (more than a small clock-skew allowance into) the future -
# otherwise a caller could pick a far-future timestamp and get a signature
# that's "not yet 5 minutes old" forever, defeating the freshness check.
MAX_TIMESTAMP_SKEW_SECONDS = 5


def verify_admin(admin_name: str, name_to_delete: str, timestamp: str, proof_of_possession: str, cur) -> None:
    """Raises HTTPException if admin_name isn't a verified admin.

    publicKey is expected to be base64-encoded raw uncompressed P-256 point
    bytes (0x04 || X || Y, 65 bytes - the standard iOS SecKeyCopyExternalRepresentation
    format). proofOfPossession is expected to be a base64-encoded DER ECDSA
    signature (SHA-256) over:

        "ADMINNAME:" + admin_name + ":TODELETENAME:" + name_to_delete + timestamp

    where timestamp is the same ASCII decimal Unix-seconds string passed as
    the timestamp parameter.
    """
    cur.execute(
        "SELECT public_key, is_admin FROM users WHERE name = %s",
        (admin_name,),
    )
    row = cur.fetchone()
    if row is None:
        raise HTTPException(status_code=401, detail="unknown admin user")

    try:
        timestamp_seconds = int(timestamp)
    except ValueError:
        raise HTTPException(status_code=401, detail="invalid timestamp")

    age = time.time() - timestamp_seconds
    if age > MAX_TIMESTAMP_AGE_SECONDS or age < -MAX_TIMESTAMP_SKEW_SECONDS:
        raise HTTPException(status_code=401, detail="timestamp is expired or not yet valid")

    message = f"ADMINNAME:{admin_name}:TODELETENAME:{name_to_delete}{timestamp}".encode("utf-8")

    try:
        public_key_bytes = base64.b64decode(row["public_key"], validate=True)
        public_key = ec.EllipticCurvePublicKey.from_encoded_point(ec.SECP256R1(), public_key_bytes)
        signature = base64.b64decode(proof_of_possession, validate=True)
        public_key.verify(signature, message, ec.ECDSA(hashes.SHA256()))
    except (InvalidSignature, ValueError):
        raise HTTPException(status_code=401, detail="invalid proof of possession")

    if not row["is_admin"]:
        raise HTTPException(status_code=403, detail=f"'{admin_name}' is not an admin")


@app.post("/getUser", response_model=GetUserResponse)
def get_user(req: GetUserRequest) -> GetUserResponse:
    with get_connection() as conn, conn.cursor() as cur:
        cur.execute(
            "SELECT public_key, image FROM users WHERE name = %s",
            (req.name,),
        )
        row = cur.fetchone()

    if row is None:
        return GetUserResponse(publicKey="", image=0)
    return GetUserResponse(publicKey=row["public_key"], image=row["image"])


@app.post("/register", response_model=RegisterResponse)
def register(req: RegisterRequest) -> RegisterResponse:
    conflict = False
    with get_connection() as conn:
        try:
            with conn.cursor() as cur:
                cur.execute(
                    "INSERT INTO users (name, public_key, image) VALUES (%s, %s, %s)",
                    (req.name, req.publicKey, req.image),
                )
        except errors.UniqueViolation:
            conn.rollback()
            conflict = True
        else:
            conn.commit()

    if conflict:
        raise HTTPException(status_code=409, detail=f"'{req.name}' is already registered")

    return RegisterResponse(success=True)


@app.post("/deleteUser", response_model=DeleteUserResponse)
def delete_user(req: DeleteUserRequest) -> DeleteUserResponse:
    with get_connection() as conn:
        with conn.cursor() as cur:
            verify_admin(req.adminName, req.name, req.timestamp, req.proofOfPossession, cur)

            cur.execute("DELETE FROM users WHERE name = %s", (req.name,))
            deleted = cur.rowcount > 0
        conn.commit()

    return DeleteUserResponse(success=deleted)
