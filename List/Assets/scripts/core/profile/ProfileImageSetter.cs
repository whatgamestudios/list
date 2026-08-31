// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Lists {

    public class ProfileImageSetter {
        /**
         * Pass in a panel and set the profile image.
         */
        public static void SetProfileImage(GameObject panel, int type) {
            Image img = panel.GetComponent<Image>();
            if (img == null) {
                AuditLog.Log("ERROR: ProfileImageSetter: No raw image");
                return;
            }

            string imageLink = ProfileMetadata.GetPhotoResource(type);

            // Set the background image.
            Texture2D tex = Resources.Load<Texture2D>(imageLink);
            if (tex == null) {
                AuditLog.Log("ERROR: Resource not found: " + imageLink);
                return;
            }
            Rect size = new Rect(0.0f, 0.0f, tex.width, tex.height);
            Vector2 pivot = new Vector2(0.0f, 0.0f);
            Sprite s = Sprite.Create(tex, size, pivot);
            img.sprite = s;
        }


        /**
        * Pass in a panel and set the profile image.
        */
        public static void SetMyProfileImage(GameObject panel) {
            int typeId = ProfileStore.GetProfileImageType();
            SetProfileImage(panel, typeId);
        }
    }
}