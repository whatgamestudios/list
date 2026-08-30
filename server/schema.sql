CREATE TABLE IF NOT EXISTS users (
    name       TEXT PRIMARY KEY,
    public_key TEXT NOT NULL,
    image      INTEGER NOT NULL,
    is_admin   BOOLEAN NOT NULL DEFAULT FALSE
);
