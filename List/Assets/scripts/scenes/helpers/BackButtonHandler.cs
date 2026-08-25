// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class BackButtonHandler : MonoBehaviour
    {

        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Back")
            {
                GoBack();
            }
            else
            {
                AuditLog.Log($"BackButton: Unknown button: {buttonText}");
            }
        }

        public static void GoBack() {
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            // If menu scene, then quit the app
            if (currentScene == SceneStack.MENU_SCENE) {
                Application.Quit();
            }

            var previousScene = SceneStack.Instance().PopScene();
            AuditLog.Log($"Back: switching from scene {currentScene} to scene {previousScene}");
            SceneManager.LoadScene(previousScene, LoadSceneMode.Single);
        }
    }
}