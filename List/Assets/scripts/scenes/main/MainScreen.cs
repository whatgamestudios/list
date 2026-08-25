// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class MenuScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Main screen");
        }


        public void OnButtonClickSettings()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickContacts()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ContractsScene", LoadSceneMode.Single);
        }
    }
}
