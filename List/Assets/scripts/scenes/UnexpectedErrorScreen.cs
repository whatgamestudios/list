// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;


namespace Lists {

    public class UnexpectedErrorScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Unexpected Error screen");
        }

        public void OnButtonClick(string buttonText) {
            if (buttonText == "Share") {
                string msg = AuditLog.GetLogs();
                SunShineNativeShare.instance.ShareText(msg, msg);
            }
            else if (buttonText == "GoToGame") {
                SceneManager.LoadScene("MenuScene", LoadSceneMode.Single);
            }
            else {
                AuditLog.Log($"Unexpected E Screen: Unknwon Button: {buttonText}");
            }
        }
    }
}