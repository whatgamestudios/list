#if UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;

namespace Lists {

    // Always reports unavailable, so Editor testing exercises the PIN path -
    // there is no Face ID / BiometricPrompt to call into from the Editor.
    public class EditorBiometricAuth : MonoBehaviour, IBiometricAuth {
        public bool IsAvailable() => false;

        public void Authenticate(string reason, Action<bool> onResult)
        {
            onResult?.Invoke(false);
        }
    }
}
#endif
