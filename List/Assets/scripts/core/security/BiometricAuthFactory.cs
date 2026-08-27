// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;

namespace Lists {

    public static class BiometricAuthFactory {
        // Adds the platform-appropriate IBiometricAuth component to host and
        // returns it. host must stay active for the lifetime of any in-flight
        // Authenticate() call, since the native side reports back by name.
        public static IBiometricAuth Attach(GameObject host)
        {
#if UNITY_IOS && !UNITY_EDITOR
            return host.AddComponent<IosBiometricAuth>();
#elif UNITY_ANDROID && !UNITY_EDITOR
            return host.AddComponent<AndroidBiometricAuth>();
#elif UNITY_EDITOR
            return host.AddComponent<EditorBiometricAuth>();
#else
            return null;
#endif
        }
    }
}
