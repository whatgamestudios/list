# server

A small FastAPI RPC server, deployed to Vercel, backed by a Neon (serverless Postgres) database. See [API.md](./API.md) for the endpoint reference.

## Stack

- [FastAPI](https://fastapi.tiangolo.com/) (Python) as the ASGI app, deployed as a Vercel serverless function
- [Neon](https://neon.tech/) for Postgres storage
- [psycopg](https://www.psycopg.org/psycopg3/) as the database driver

## Project layout

```
server/
  api/
    index.py       # FastAPI app and routes - Vercel's serverless entrypoint
  schema.sql        # run once against your Neon database
  requirements.txt
  .env.example
```

## Setup

### 1. Create the Neon database

1. Create a project at [neon.tech](https://neon.tech).
2. Copy the **pooled** connection string (its hostname contains `-pooler`) - use this one rather than the direct connection string, since each request to this API may open its own database connection.
3. Run `schema.sql` against it, either by pasting it into the Neon SQL editor or with `psql`:
   ```bash
   psql "$DATABASE_URL" -f schema.sql
   ```

### 2. Configure environment variables

Copy `.env.example` to `.env` and fill in `DATABASE_URL` with the connection string from step 1.

```bash
cp .env.example .env
```

### 3. Run locally

```bash
cd server
python -m venv .venv && source .venv/bin/activate
pip install -r requirements.txt
export $(cat .env | xargs)   # or use a tool like direnv / python-dotenv
uvicorn api.index:app --reload
```

The server listens on `http://127.0.0.1:8000`. Try it:

```bash
curl -X POST http://127.0.0.1:8000/register \
  -H "Content-Type: application/json" \
  -d '{"name": "alice", "publicKey": "0xabc123", "image": 3}'

curl -X POST http://127.0.0.1:8000/getUser \
  -H "Content-Type: application/json" \
  -d '{"name": "alice"}'
```

### 4. Deploy to Vercel

```bash
cd server
vercel
```

Set `DATABASE_URL` as an environment variable in the Vercel project (Project → Settings → Environment Variables) - it is not picked up from your local `.env` file.

Vercel's zero-config Python/FastAPI detection mounts the app at the plain root - verified live against a real deployment, so `https://your-project.vercel.app/getUser` etc., same shape as running locally. (An earlier version of this project shipped a `vercel.json` rewrite trying to force routing to a fixed destination path; that actively broke things once Vercel started honoring rewrite destination paths for backend-framework projects, so it's been removed rather than fought. A later guess that the zero-config mount used a `/fastapi` prefix was also wrong and has been corrected.)

### Testing /deleteUser locally

`deleteUser` needs a real P-256 keypair for the admin - here's how to generate one and sign with it using OpenSSL (verified working against this server's verification code):

```bash
# Generate a P-256 private key
openssl ecparam -name prime256v1 -genkey -noout -out admin.pem

# Extract the raw uncompressed public key point, base64-encoded - this is what
# goes in `publicKey` when you register the admin (the DER SubjectPublicKeyInfo
# for a P-256 key ends with exactly these 65 bytes)
openssl ec -in admin.pem -pubout -outform DER | tail -c 65 | base64

# Build the message and sign it - this is `proofOfPossession`. adminName is
# "alice", name (the user to delete) is "bob"; timestamp must be the same
# value you send as the `timestamp` field, and must be within 5 minutes of
# the server's clock when the request arrives.
TIMESTAMP=$(date +%s)
printf 'ADMINNAME:alice:TODELETENAME:bob%s' "$TIMESTAMP" \
  | openssl dgst -sha256 -sign admin.pem | base64
echo "timestamp: $TIMESTAMP"
```

Register the admin with that public key (`POST /register`), mark them as an admin (there's no API for this yet - see Notes below), then call `/deleteUser` with `adminName: "alice"`, `name: "bob"`, that `timestamp`, and the signature above as `proofOfPossession`.

## Notes

- `register` rejects duplicates: registering a `name` that's already taken returns HTTP 409 rather than overwriting the existing entry.
- `getUser` for an unregistered `name` returns `publicKey: ""` and `image: 0`, not an error - check for either on the client to detect "not found".
- There's no API to grant `is_admin` - every user is created with it `false` (see `schema.sql`). Promote a user to admin directly in Neon:
  ```sql
  UPDATE users SET is_admin = true WHERE name = 'alice';
  ```
