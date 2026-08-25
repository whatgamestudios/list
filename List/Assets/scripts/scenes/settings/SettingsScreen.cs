// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class SettingsScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Settings screen");
        }


        public void OnButtonClickWallpaper()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
        }
    }
}
