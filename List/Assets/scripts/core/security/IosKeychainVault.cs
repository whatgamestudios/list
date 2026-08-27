#if UNITY_IOS && !UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System.Runtime.InteropServices;

namespace Lists {

    // Bridges to Assets/Plugins/iOS/KeychainBridge.mm + KeychainBridge.swift, which
    // store values in the iOS Keychain (kSecClassGenericPassword), keyed by account.
    public class IosKeychainVault : ISecretVault {
        [DllImport("__Internal")]
        private static extern int _keychainSetSecret(string account, byte[] bytes, int length);

        [DllImport("__Internal")]
        private static extern int _keychainGetSecret(string account, byte[] outBuffer, int bufferLength);

        [DllImport("__Internal")]
        private static extern int _keychainDeleteSecret(string account);

        // Keychain items don't self-report their length ahead of time through this
        // bridge, so probe with a buffer comfortably larger than anything we store.
        private const int MaxValueLength = 4096;

        public bool TryLoad(string key, out byte[] value)
        {
            byte[] buffer = new byte[MaxValueLength];
            int length = _keychainGetSecret(key, buffer, buffer.Length);
            if (length < 0) {
                value = null;
                return false;
            }
            value = new byte[length];
            System.Array.Copy(buffer, value, length);
            return true;
        }

        public void Save(string key, byte[] value)
        {
            _keychainSetSecret(key, value, value.Length);
        }

        public void Delete(string key)
        {
            _keychainDeleteSecret(key);
        }
    }
}
#endif
