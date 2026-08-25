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
        private InputAction backAction;
        private Coroutine enableCoroutine;

        private void Awake()
        {
            backAction = new InputAction("Back", binding: "<Keyboard>/escape");
            backAction.performed += OnBackPerformed;
            
            // Start the delayed enable coroutine
            enableCoroutine = StartCoroutine(DelayedEnable());
        }

        private IEnumerator DelayedEnable()
        {
            yield return new WaitForSeconds(0.5f); // 500ms delay
            backAction.Enable();
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            // AuditLog.Log($"Android Back button enabled in scene {currentScene} after delay");
        }

        private void OnDisable()
        {
            if (enableCoroutine != null)
            {
                StopCoroutine(enableCoroutine);
                enableCoroutine = null;
            }
            backAction.Disable();
            var currentScene = SceneManager.GetActiveScene().buildIndex;
            // AuditLog.Log($"Android Back button disabled in scene {currentScene}");
        }

        private void OnBackPerformed(InputAction.CallbackContext context)
        {
            GoBack();
        }

        public void OnButtonClickBack()
        {
            GoBack();
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