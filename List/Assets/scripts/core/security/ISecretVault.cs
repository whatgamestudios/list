// Copyright (c) Whatgame Studios 2024 - 2026
namespace Lists {

    // A place that can durably hold secrets outside of PlayerPrefs - the iOS
    // Keychain or the Android Keystore, depending on platform. Keyed so it can
    // hold more than one secret (the device secret, the PIN hash+salt, ...)
    // under distinct names.
    public interface ISecretVault {
        bool TryLoad(string key, out byte[] value);
        void Save(string key, byte[] value);
        void Delete(string key);
    }
}
