#if UNITY_IOS && !UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Lists {

    // Bridges to Assets/Plugins/iOS/BiometricBridge.mm + BiometricBridge.swift
    // (LocalAuthentication / Face ID). The native side reports back
    // asynchronously via UnitySendMessage(gameObject.name, "OnBiometricResult", ...),
    // so this must live on an active GameObject.
    public class IosBiometricAuth : MonoBehaviour, IBiometricAuth {
        [DllImport("__Internal")]
        private static extern int _biometricIsAvailable();

        [DllImport("__Internal")]
        private static extern void _biometricAuthenticate(string reason, string callbackGameObject);

        private Action<bool> callback;

        public bool IsAvailable() => _biometricIsAvailable() == 1;

        public void Authenticate(string reason, Action<bool> onResult)
        {
            callback = onResult;
            _biometricAuthenticate(reason, gameObject.name);
        }

        // Invoked by KeychainBridge/BiometricBridge.mm via UnitySendMessage.
        private void OnBiometricResult(string result)
        {
            Action<bool> cb = callback;
            callback = null;
            cb?.Invoke(result == "1");
        }
    }
}
#endif
