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

        public void OnButtonClickContacts()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ContractsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickCredits()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickSecurity()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("SecurityScene", LoadSceneMode.Single);
        }

        public void OnButtonClickWallpaper()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
        }
    }
}
