// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lists {

    // Watches for the app losing/regaining OS focus (backgrounded, screen locked,
    // app-switched away from, ...) and forces the user back to AuthScene for
    // re-authentication if it was away longer than ReauthTimeoutSeconds. Bootstraps
    // itself into a DontDestroyOnLoad object before the first scene loads, so it
    // keeps watching across every scene for the whole app run.
    public class AppLockMonitor : MonoBehaviour {
        public const float DefaultReauthTimeoutSeconds = 30f;

        // Sentinel ReauthTimeoutSeconds value meaning "never force re-authentication".
        public const float NeverTimeoutValue = -1f;

        private const string ReauthTimeoutPrefsKey = "REAUTH_TIMEOUT_SECONDS";

        private static AppLockMonitor instance;
        private double? focusLostAtRealtime;

        public static float ReauthTimeoutSeconds
        {
            get => PlayerPrefs.GetFloat(ReauthTimeoutPrefsKey, DefaultReauthTimeoutSeconds);
            set
            {
                PlayerPrefs.SetFloat(ReauthTimeoutPrefsKey, value);
                PlayerPrefs.Save();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            if (instance != null) {
                return;
            }
            GameObject monitorObj = new GameObject("AppLockMonitor");
            instance = monitorObj.AddComponent<AppLockMonitor>();
            DontDestroyOnLoad(monitorObj);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            HandleFocusChange(hasFocus: !pauseStatus);
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            HandleFocusChange(hasFocus);
        }

        private void HandleFocusChange(bool hasFocus)
        {
            if (!hasFocus) {
                // OnApplicationFocus(false) and OnApplicationPause(true) can both
                // fire for the same backgrounding event - only record the first.
                if (focusLostAtRealtime == null) {
                    focusLostAtRealtime = Time.realtimeSinceStartupAsDouble;
                }
                return;
            }

            if (focusLostAtRealtime == null) {
                return;
            }

            double awaySeconds = Time.realtimeSinceStartupAsDouble - focusLostAtRealtime.Value;
            focusLostAtRealtime = null;

            float timeout = ReauthTimeoutSeconds;
            if (timeout >= 0 && awaySeconds >= timeout) {
                ForceReauth(awaySeconds);
            }
        }

        private void ForceReauth(double awaySeconds)
        {
            if (SceneManager.GetActiveScene().name == "AuthScene") {
                return;
            }

            AuditLog.Log($"App was backgrounded for {awaySeconds:F0}s (>= {ReauthTimeoutSeconds:F0}s timeout) - forcing re-authentication");

            SecretStore.Clear();
            SceneStack.Instance().Reset();
            SceneManager.LoadScene("AuthScene", LoadSceneMode.Single);
        }
    }
}
