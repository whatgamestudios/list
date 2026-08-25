// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;


namespace Lists {

    /**
    * Lock orientation to portrait only.
    */
    public class ScreenOrientationLock : MonoBehaviour {

        void Start() {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = true;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.AutoRotation;
        }
    }
}