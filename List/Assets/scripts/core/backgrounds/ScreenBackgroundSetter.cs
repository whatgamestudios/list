// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Lists {
    public class ScreenBackgroundSetter {
        /**
        * Pass in the scene's main panel and set the background image.
        */
        public static void SetPanelBackground(GameObject panel) {
            Image img = panel.GetComponent<Image>();
            if (img == null) {
                AuditLog.Log("ERROR: No raw image");
                return;
            }

            // Get the current selected background. If due to asset sync, that background is no longer
            // owned, switch to the default background.
            int tokenId = SceneStore.GetBackground();
            SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(tokenId);

            // Set the background image.
            Texture2D tex = Resources.Load<Texture2D>(sceneInfo.resource);
            if (tex == null) {
                AuditLog.Log("ERROR: Resource not found: " + sceneInfo.resource);
            }
            Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
            Vector2 pivot = new Vector2(0.0f, 0.0f);
            Sprite s = Sprite.Create(tex, size, pivot);
            img.sprite = s;

            // Find all tagged objects and set their colour to the contract colour.
            GameObject[] textMeshes = GameObject.FindGameObjectsWithTag("ColMe");
            foreach (GameObject textMeshObj in textMeshes) {
                TextMeshProUGUI textMeshProGUI = textMeshObj.GetComponent<TextMeshProUGUI>();
                textMeshProGUI.color = sceneInfo.faceColour;    
                textMeshProGUI.outlineColor = sceneInfo.outlineColour;
            }
        }
    }
}