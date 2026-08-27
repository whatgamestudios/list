#if UNITY_ANDROID && !UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;

namespace Lists {

    // Unity's default player Activity is not a FragmentActivity, which
    // androidx.biometric.BiometricPrompt requires, so authentication is delegated
    // to a small transparent FragmentActivity - see
    // Assets/Plugins/Android/BiometricActivity.java - started from here and which
    // reports back via UnitySendMessage(gameObject.name, "OnBiometricResult", ...).
    public class AndroidBiometricAuth : MonoBehaviour, IBiometricAuth {
        private const string BiometricActivityClass = "com.whatgamestudios.lists.biometric.BiometricActivity";

        private Action<bool> callback;

        public bool IsAvailable()
        {
            using (AndroidJavaObject activity = GetActivity())
            using (AndroidJavaClass biometricManagerClass = new AndroidJavaClass("androidx.biometric.BiometricManager"))
            using (AndroidJavaObject biometricManager = biometricManagerClass.CallStatic<AndroidJavaObject>("from", activity))
            using (AndroidJavaClass authenticatorsClass = new AndroidJavaClass("androidx.biometric.BiometricManager$Authenticators")) {
                int strong = authenticatorsClass.GetStatic<int>("BIOMETRIC_STRONG");
                int canAuthenticate = biometricManager.Call<int>("canAuthenticate", strong);
                return canAuthenticate == 0; // BiometricManager.BIOMETRIC_SUCCESS
            }
        }

        public void Authenticate(string reason, Action<bool> onResult)
        {
            callback = onResult;

            using (AndroidJavaObject activity = GetActivity())
            using (AndroidJavaClass activityClass = new AndroidJavaClass(BiometricActivityClass))
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", activity, activityClass)) {
                intent.Call<AndroidJavaObject>("putExtra", "reason", reason);
                intent.Call<AndroidJavaObject>("putExtra", "callbackObject", gameObject.name);
                activity.Call("startActivity", intent);
            }
        }

        // Invoked by BiometricActivity.java via UnityPlayer.UnitySendMessage.
        private void OnBiometricResult(string result)
        {
            Action<bool> cb = callback;
            callback = null;
            cb?.Invoke(result == "1");
        }

        private static AndroidJavaObject GetActivity()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }
    }
}
#endif
