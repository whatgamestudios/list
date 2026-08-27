#if UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;

namespace Lists {

    // NOT SECURE. PlayerPrefs-backed stand-in for the Keychain/Keystore so the
    // sign-in flow is testable in the Editor, where neither is available.
    // Real devices always use IosKeychainVault or AndroidKeystoreVault.
    public class EditorDevVault : ISecretVault {
        private const string PrefsPrefix = "EDITOR_DEV_VAULT_";

        public bool TryLoad(string key, out byte[] value)
        {
            string base64 = PlayerPrefs.GetString(PrefsPrefix + key, "");
            if (string.IsNullOrEmpty(base64)) {
                value = null;
                return false;
            }
            value = Convert.FromBase64String(base64);
            return true;
        }

        public void Save(string key, byte[] value)
        {
            PlayerPrefs.SetString(PrefsPrefix + key, Convert.ToBase64String(value));
            PlayerPrefs.Save();
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(PrefsPrefix + key);
            PlayerPrefs.Save();
        }
    }
}
#endif
