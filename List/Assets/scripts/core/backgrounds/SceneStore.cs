// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Lists {
    /**
     * Storage for holding scene related information.
    **/
    public class SceneStore {
        public const string BG_OPTION = "OPTION_BACKGROUND";

        public const int BG_DEFAULT = 1;

        /**
        * Set the background used by all scenes.
        */
        public static void SetBackground(int option) {
            PlayerPrefs.SetInt(BG_OPTION, option);
            PlayerPrefs.Save();
        }

        /**
        * Get the background option to be used.
        *
        * @return the background option number to use.
        */
        public static int GetBackground() {
            return PlayerPrefs.GetInt(BG_OPTION, BG_DEFAULT);
        }
    }
}