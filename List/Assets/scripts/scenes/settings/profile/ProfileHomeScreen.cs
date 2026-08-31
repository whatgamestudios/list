// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Lists {

    // Shows the registered name and profile image. Tapping the image goes to
    // ProfileImageScene to change it. Share/Delete are not implemented yet.
    public class ProfileHomeScreen : MonoBehaviour {

        public TextMeshProUGUI nameText;
        public GameObject profileImageObject;

        public void Start()
        {
            AuditLog.Log("Profile screen");
            nameText.text = ProfileStore.GetProfileName();
            ProfileImageSetter.SetMyProfileImage(profileImageObject);
        }

        public void OnButtonClickImage()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileImageScene", LoadSceneMode.Single);
        }

        public void OnButtonClickShare()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileShareScene", LoadSceneMode.Single);
        }

        public void OnButtonClickDangerous()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileDangerousScene", LoadSceneMode.Single);
        }
    }
}
