// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class SceneStack
    {
        // Build index of MainScene. WelcomeScene (0), RegistrationScene (1) and
        // AuthScene (2) come before it - keep this in sync with
        // ProjectSettings/EditorBuildSettings.asset if the scene order changes again.
        public const int MENU_SCENE = 3;
        private Stack<int> sceneStack = new Stack<int>();

        private static SceneStack instance;

        public static SceneStack Instance() {
            if (instance == null)
            {
                instance = new SceneStack();
            }
            return instance;
        }

        public void PushScene() {
            int sceneId = SceneManager.GetActiveScene().buildIndex;
            if (sceneStack.Count > 0)
            {
                int topOfStack = sceneStack.Peek();
                if (topOfStack == sceneId)
                {
                    AuditLog.Log($"ERROR: pushing scene {sceneId} twice");
                    return;                    
                }
            }
            //AuditLog.Log($"Push: {sceneId}");
            sceneStack.Push(sceneId);
        }

        public void Reset()
        {
            //AuditLog.Log($"Resetting scene stack");
            while (sceneStack.Count > 0)
            {
                sceneStack.Pop();
            }
        }

        public int PopScene() {
            if (sceneStack.Count > 0)
            {
                return sceneStack.Pop();
            }
            else
            {
                return MENU_SCENE;
            }
        }
    }
}