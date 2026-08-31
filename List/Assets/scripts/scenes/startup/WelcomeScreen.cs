// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace Lists {
    public class WelcomeScreen : MonoBehaviour
    {
        public void Start()
        {
            AuditLog.Log("Welcome screen");

            string nextScene = ProfileStore.HasProfileName() ? "AuthScene" : "RegistrationScene";
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
}
