// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;

namespace Lists {

    /**
    * Manage data storage format upgrades.
    * This scipt must be included in the first scene at the top so it runs first. 
    * All other scripts assume the latest data storage format.
    */
    public class UpgradeStorageWrapper : MonoBehaviour {
        public void Start() {
            //UpgradeStorage.UpgradeStorageIfNecessary();
       }
    }
}