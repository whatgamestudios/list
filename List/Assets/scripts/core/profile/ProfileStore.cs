// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Lists {
    /**
     * Storage for holding profile related information.
    **/
    public class ProfileStore {
        public const string PROFILE_PHOTO = "PROFILE_PHOTO";
        public const string PROFILE_PHOTO_SET = "PROFILE_PHOTO_SET";
        public const string PROFILE_NAME = "PROFILE_NAME";
        public const string PROFILE_NAME_DRAFT = "PROFILE_NAME_DRAFT";

        public const int PROFILE_PHOTO_DEFAULT = 0;

        /**
        * Set the profile image. Also marks it as explicitly set, so
        * RegistrationScreen knows not to overwrite it with a random pick.
        */
        public static void SetProfileImageType(int option) {
            PlayerPrefs.SetInt(PROFILE_PHOTO, option);
            PlayerPrefs.SetInt(PROFILE_PHOTO_SET, 1);
            PlayerPrefs.Save();
        }

        /**
        * Get the profile image.
        *
        * @return the profile image.
        */
        public static int GetProfileImageType() {
            return PlayerPrefs.GetInt(PROFILE_PHOTO, PROFILE_PHOTO_DEFAULT);
        }

        /**
        * Whether a profile image has ever been set (randomly during
        * registration, or explicitly via ProfileScreen).
        */
        public static bool HasProfileImageBeenSet() {
            return PlayerPrefs.GetInt(PROFILE_PHOTO_SET, 0) == 1;
        }

        /**
        * Set the registered profile (user) name.
        */
        public static void SetProfileName(string name) {
            PlayerPrefs.SetString(PROFILE_NAME, name);
            PlayerPrefs.Save();
        }

        /**
        * Get the registered profile (user) name. Empty string if not
        * registered yet.
        */
        public static string GetProfileName() {
            return PlayerPrefs.GetString(PROFILE_NAME, "");
        }

        /**
        * Whether the device has completed registration.
        */
        public static bool HasProfileName() {
            return !string.IsNullOrEmpty(GetProfileName());
        }

        /**
        * Set the tentative (not yet registered) name the user is typing in
        * RegistrationScreen, so it survives a trip to ProfileImageScene and back.
        * Distinct from PROFILE_NAME, which is only set once registration with
        * the server actually succeeds.
        */
        public static void SetDraftProfileName(string name) {
            PlayerPrefs.SetString(PROFILE_NAME_DRAFT, name);
            PlayerPrefs.Save();
        }

        /**
        * Get the tentative profile name. Empty string if none is set.
        */
        public static string GetDraftProfileName() {
            return PlayerPrefs.GetString(PROFILE_NAME_DRAFT, "");
        }

        /**
        * Clear the tentative profile name (once registration succeeds).
        */
        public static void ClearDraftProfileName() {
            PlayerPrefs.DeleteKey(PROFILE_NAME_DRAFT);
            PlayerPrefs.Save();
        }
    }
}