// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;


// Pop up an error screen for three seconds
namespace Lists {

    public class ErrorScreen : MonoBehaviour {
        public TextMeshProUGUI errorText;
        public TextMeshProUGUI countDownText;

        private const int WAIT_TIME_MS = 3000;
        private DateTime start;

        public void Start() {
            string msg = MessagePass.GetErrorMsg();
            errorText.text = msg;
            countDownText.text = (WAIT_TIME_MS / 1000 + 1).ToString();
            start = DateTime.Now;
            AuditLog.Log("Info Popup: " + msg);
        }


        public async void Update() {
            DateTime now = DateTime.Now;
            double timeElapsedMs = (now - start).TotalMilliseconds;
            if ((int)timeElapsedMs > WAIT_TIME_MS) {
                await SceneManager.UnloadSceneAsync("ErrorScene");
            }
            else {
                int timeToWait = (WAIT_TIME_MS - (int) timeElapsedMs) / 1000 + 1;
                countDownText.text = (timeToWait).ToString();
            }
        }
    }
}