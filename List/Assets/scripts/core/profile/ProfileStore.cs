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

        public const int PROFILE_PHOTO_DEFAULT = 0;

        /**
        * Set the profile image.
        */
        public static void SetProfileImageType(int option) {
            PlayerPrefs.SetInt(PROFILE_PHOTO, option);
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
    }
}