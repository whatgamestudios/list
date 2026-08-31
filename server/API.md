# API

Base URL: your Vercel deployment URL (e.g. `https://your-project.vercel.app`), or `http://127.0.0.1:8000` when running locally. No path prefix - endpoints are at the plain root in both cases (there's no `vercel.json` here; Vercel's zero-config Python/FastAPI detection mounts the app directly).

All endpoints are `POST`, take a JSON request body, and return a JSON response body.

## POST /getUser

Look up a registered user by name.

### Request body

| field | type   | required |
|-------|--------|----------|
| name  | string | yes      |

### Response body

| field     | type    |
|-----------|---------|
| publicKey | string  |
| image     | integer |

If `name` isn't registered, `publicKey` is returned as an empty string (`""`) and `image` as `0` — this is not an error.

### Example

Request:

```json
{ "name": "alice" }
```

Response:

```json
{ "publicKey": "0xabc123...", "image": 3 }
```

## POST /register

Register a user's public key and image. `name` must not already be registered.

### Request body

| field     | type    | required |
|-----------|---------|----------|
| name      | string  | yes      |
| publicKey | string  | yes      |
| image     | integer | yes      |

`publicKey` must be a P-256 (secp256r1) public key, as the raw uncompressed point (`0x04 \|\| X \|\| Y`, 65 bytes), base64-encoded. This is the format iOS's `SecKeyCopyExternalRepresentation` produces for EC keys. It's the key that will later be checked against `proofOfPossession` for [`/deleteUser`](#post-deleteuser).

### Response body

| field   | type    |
|---------|---------|
| success | boolean |

### Example

Request:

```json
{ "name": "alice", "publicKey": "0xabc123...", "image": 3 }
```

Response:

```json
{ "success": true }
```

If `name` is already registered, the request is rejected with HTTP 409:

```json
{ "detail": "'alice' is already registered" }
```

## POST /deleteUser

Deletes a user by name. Restricted to admins (`is_admin = true`) - the caller proves their identity with `proofOfPossession`, a signature over a message built from `adminName`, `name`, and `timestamp`.

### Request body

| field              | type   | required |
|--------------------|--------|----------|
| name               | string | yes      |
| adminName          | string | yes      |
| timestamp          | string | yes      |
| proofOfPossession  | string | yes      |

- `name` is the user to delete.
- `adminName` is the calling admin's own registered name.
- `timestamp` is the current Unix time (seconds), as an ASCII decimal string (e.g. `"1735689600"`). The server rejects the request if this is more than 5 minutes old, or more than 5 seconds in the future (to allow for clock skew without letting a caller pick a far-future timestamp to dodge the freshness check).
- `proofOfPossession` is a base64-encoded ECDSA signature (SHA-256, P-256/secp256r1 curve), produced with the private key matching `adminName`'s stored `publicKey`, over the UTF-8 bytes of:

  ```
  "ADMINNAME:" + adminName + ":TODELETENAME:" + name + timestamp
  ```

  (the exact same `timestamp` string sent in the request body - the signature is only valid for that specific timestamp). The signature must be DER-encoded (the default ECDSA signature encoding, and what iOS's `SecKeyCreateSignature`/CryptoKit produce for P-256).

### Response body

| field   | type    |
|---------|---------|
| success | boolean |

`success` is `true` if `name` was found and deleted, `false` if `name` was not found. This is not an error - a false result still means the request itself was valid and the caller was a verified admin.

### Example

For `adminName: "alice"`, `name: "bob"`, `timestamp: "1735689600"`, the signed message is the UTF-8 bytes of `ADMINNAME:alice:TODELETENAME:bob1735689600`.

Request:

```json
{
  "name": "bob",
  "adminName": "alice",
  "timestamp": "1735689600",
  "proofOfPossession": "MEUCIQDotQ1kVJyPT0r0gczdQ0Rh7jgsEOMUPBwqcIJTrB4p8QIgbx3NPZMl9ms71XAzDgv+fkGVtSd1WoUiEr9DFsfWZDc="
}
```

Response:

```json
{ "success": true }
```

### Errors specific to this endpoint

| status | when |
|--------|------|
| 401    | `adminName` isn't a registered user; `timestamp` isn't a valid integer; `timestamp` is too old or too far in the future; or `proofOfPossession` doesn't verify against `adminName`'s stored `publicKey` |
| 403    | `adminName` is a registered user, but `is_admin` is `false` |

## Errors

A request missing a required field, or with a field of the wrong type, gets back HTTP 422 with FastAPI's standard validation error body. Registering a `name` that's already taken gets back HTTP 409, as shown above. See `/deleteUser` above for its specific 401/403 responses.
