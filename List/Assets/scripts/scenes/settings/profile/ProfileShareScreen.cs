// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Lists {

    public class ProfileShareScreen : MonoBehaviour {

        public TextMeshProUGUI nameText;
        public GameObject profileImageObject;

        public void Start()
        {
            AuditLog.Log("Profile screen");
            nameText.text = ProfileStore.GetProfileName();
            ProfileImageSetter.SetMyProfileImage(profileImageObject);
        }

        public void OnButtonClickShare()
        {
            string msg = "Lotsalists<" + ProfileStore.GetProfileName() + ">";
            SunShineNativeShare.instance.ShareText(msg, msg);
        }
    }
}
