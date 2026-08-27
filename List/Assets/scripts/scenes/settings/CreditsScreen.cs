// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


namespace Lists {

    public class CreditsScreen : MonoBehaviour {

        public TextMeshProUGUI VersionText;

        public void Start()
        {
            AuditLog.Log("Credits screen");
            VersionText.text = Application.version;
        }

        public void OnButtonClickWebsite() {
            Application.OpenURL("https://whatgamestudios.com/lotsalists");
        }

        public void OnButtonClickPrivacy() {
            Application.OpenURL("https://whatgamestudios.com/lotsalists/privacy-policy/");
        }
    }
}