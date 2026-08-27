// Copyright (c) Whatgame Studios 2024 - 2026
using System;

namespace Lists {

    // Face ID (iOS) / BiometricPrompt (Android) gate. Implementations are
    // MonoBehaviours because the underlying platform calls are asynchronous and
    // report back through UnitySendMessage, which needs a live GameObject to
    // target - see BiometricAuthFactory.Attach.
    public interface IBiometricAuth {
        bool IsAvailable();
        void Authenticate(string reason, Action<bool> onResult);
    }
}
