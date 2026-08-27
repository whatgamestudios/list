// Copyright (c) Whatgame Studios 2024 - 2026
namespace Lists {

    public static class SecretVaultFactory {
        public static ISecretVault Get()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return new IosKeychainVault();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return new AndroidKeystoreVault();
#elif UNITY_EDITOR
            return new EditorDevVault();
#else
            throw new System.PlatformNotSupportedException("No secure secret storage implemented for this platform");
#endif
        }
    }
}
