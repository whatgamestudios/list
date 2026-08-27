#if UNITY_ANDROID && !UNITY_EDITOR
// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using UnityEngine;

namespace Lists {

    // Stores values by encrypting them with an AES-256/GCM key generated inside the
    // Android Keystore (the key material never leaves secure hardware) and saving
    // the resulting ciphertext blob via PlayerPrefs. PlayerPrefs only ever holds
    // ciphertext here - it is useless without the matching Keystore-resident key.
    // Each key gets its own Keystore alias and PlayerPrefs entry.
    public class AndroidKeystoreVault : ISecretVault {
        private const string AliasPrefix = "ListSecretWrapKey_";
        private const string BlobPrefsPrefix = "ANDROID_KEYSTORE_BLOB_";
        private const int GcmIvLength = 12;
        private const int GcmTagLengthBits = 128;

        // javax.crypto.Cipher opmode constants (stable across JDK versions).
        private const int CipherEncryptMode = 1;
        private const int CipherDecryptMode = 2;

        // android.security.keystore.KeyProperties.PURPOSE_ENCRYPT | PURPOSE_DECRYPT.
        private const int KeyPurposesEncryptDecrypt = 1 | 2;

        public bool TryLoad(string key, out byte[] value)
        {
            value = null;
            string base64 = PlayerPrefs.GetString(BlobPrefsPrefix + key, "");
            if (string.IsNullOrEmpty(base64)) {
                return false;
            }

            byte[] blob = Convert.FromBase64String(base64);
            if (blob.Length <= GcmIvLength) {
                return false;
            }

            byte[] iv = new byte[GcmIvLength];
            byte[] ciphertext = new byte[blob.Length - GcmIvLength];
            Buffer.BlockCopy(blob, 0, iv, 0, GcmIvLength);
            Buffer.BlockCopy(blob, GcmIvLength, ciphertext, 0, ciphertext.Length);

            using (AndroidJavaObject secretKey = GetOrCreateSecretKey(key))
            using (AndroidJavaObject cipher = GetCipher())
            using (AndroidJavaObject gcmSpec = new AndroidJavaObject("javax.crypto.spec.GCMParameterSpec", GcmTagLengthBits, iv)) {
                cipher.Call("init", CipherDecryptMode, secretKey, gcmSpec);
                value = cipher.Call<byte[]>("doFinal", ciphertext);
                return true;
            }
        }

        public void Save(string key, byte[] value)
        {
            using (AndroidJavaObject secretKey = GetOrCreateSecretKey(key))
            using (AndroidJavaObject cipher = GetCipher()) {
                cipher.Call("init", CipherEncryptMode, secretKey);
                byte[] iv = cipher.Call<byte[]>("getIV");
                byte[] ciphertext = cipher.Call<byte[]>("doFinal", value);

                byte[] blob = new byte[iv.Length + ciphertext.Length];
                Buffer.BlockCopy(iv, 0, blob, 0, iv.Length);
                Buffer.BlockCopy(ciphertext, 0, blob, iv.Length, ciphertext.Length);

                PlayerPrefs.SetString(BlobPrefsPrefix + key, Convert.ToBase64String(blob));
                PlayerPrefs.Save();
            }
        }

        public void Delete(string key)
        {
            PlayerPrefs.DeleteKey(BlobPrefsPrefix + key);
            PlayerPrefs.Save();

            string alias = AliasPrefix + key;
            using (AndroidJavaClass keyStoreClass = new AndroidJavaClass("java.security.KeyStore"))
            using (AndroidJavaObject keyStore = keyStoreClass.CallStatic<AndroidJavaObject>("getInstance", "AndroidKeyStore")) {
                keyStore.Call("load", (object) null);
                if (keyStore.Call<bool>("containsAlias", alias)) {
                    keyStore.Call("deleteEntry", alias);
                }
            }
        }

        private static AndroidJavaObject GetCipher()
        {
            using (AndroidJavaClass cipherClass = new AndroidJavaClass("javax.crypto.Cipher")) {
                return cipherClass.CallStatic<AndroidJavaObject>("getInstance", "AES/GCM/NoPadding");
            }
        }

        private AndroidJavaObject GetOrCreateSecretKey(string key)
        {
            string alias = AliasPrefix + key;

            using (AndroidJavaClass keyStoreClass = new AndroidJavaClass("java.security.KeyStore")) {
                AndroidJavaObject keyStore = keyStoreClass.CallStatic<AndroidJavaObject>("getInstance", "AndroidKeyStore");
                keyStore.Call("load", (object) null);

                if (!keyStore.Call<bool>("containsAlias", alias)) {
                    GenerateSecretKey(alias);
                }

                return keyStore.Call<AndroidJavaObject>("getKey", alias, null);
            }
        }

        private void GenerateSecretKey(string alias)
        {
            using (AndroidJavaObject builder = new AndroidJavaObject(
                       "android.security.keystore.KeyGenParameterSpec$Builder", alias, KeyPurposesEncryptDecrypt)) {
                builder.Call<AndroidJavaObject>("setBlockModes", new string[] { "GCM" });
                builder.Call<AndroidJavaObject>("setEncryptionPaddings", new string[] { "NoPadding" });
                builder.Call<AndroidJavaObject>("setKeySize", 256);
                AndroidJavaObject spec = builder.Call<AndroidJavaObject>("build");

                using (AndroidJavaClass keyGeneratorClass = new AndroidJavaClass("javax.crypto.KeyGenerator"))
                using (AndroidJavaObject keyGenerator = keyGeneratorClass.CallStatic<AndroidJavaObject>("getInstance", "AES", "AndroidKeyStore")) {
                    keyGenerator.Call("init", spec);
                    keyGenerator.Call<AndroidJavaObject>("generateKey");
                }
            }
        }
    }
}
#endif
