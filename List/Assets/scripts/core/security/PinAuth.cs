// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Security.Cryptography;

namespace Lists {

    // Salts and hashes the app-unlock PIN (PBKDF2) so the raw PIN never has to be
    // stored anywhere, on top of the salt/hash themselves living in the platform
    // secure storage (ISecretVault) rather than plain PlayerPrefs.
    public static class PinAuth {
        public const string SaltVaultKey = "pin_salt";
        public const string HashVaultKey = "pin_hash";

        public const int PinLength = 6;

        private const int SaltLength = 16;
        private const int HashLength = 32;
        private const int Iterations = 100_000;

        public static bool IsValidFormat(string pin)
        {
            if (pin == null || pin.Length != PinLength) {
                return false;
            }
            foreach (char c in pin) {
                if (c < '0' || c > '9') {
                    return false;
                }
            }
            return true;
        }

        public static void ComputeSaltAndHash(string pin, out byte[] salt, out byte[] hash)
        {
            salt = new byte[SaltLength];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(salt);
            }
            hash = Hash(pin, salt);
        }

        public static bool Verify(string pin, byte[] salt, byte[] expectedHash)
        {
            byte[] actualHash = Hash(pin, salt);
            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        private static byte[] Hash(string pin, byte[] salt)
        {
            using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(pin, salt, Iterations, HashAlgorithmName.SHA256)) {
                return pbkdf2.GetBytes(HashLength);
            }
        }
    }
}
