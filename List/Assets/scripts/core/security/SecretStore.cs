// Copyright (c) Whatgame Studios 2024 - 2026
using System;

namespace Lists {

    // Holds the device secret in memory for the lifetime of the app run so it can be
    // used across scenes for key generation. Populated once, right after PIN/Face ID
    // unlock, by loading it from (or creating and storing it into) the platform
    // secure storage - see ISecretVault / SecretVaultFactory.
    public static class SecretStore {
        public const int SecretLengthBytes = 32; // 256 bits
        public const string VaultKey = "device_secret";

        public static byte[] Secret { get; private set; }

        public static bool HasSecret => Secret != null;

        public static void SetSecret(byte[] secret)
        {
            if (secret == null || secret.Length != SecretLengthBytes) {
                throw new ArgumentException($"Secret must be {SecretLengthBytes} bytes long");
            }
            Secret = secret;
        }

        public static void Clear()
        {
            if (Secret != null) {
                Array.Clear(Secret, 0, Secret.Length);
            }
            Secret = null;
        }
    }
}
