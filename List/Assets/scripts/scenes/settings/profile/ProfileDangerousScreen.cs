// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Lists {

    public class ProfileDangerousScreen : MonoBehaviour {

        public TextMeshProUGUI nameText;
        public GameObject profileImageObject;

        public void Start()
        {
            AuditLog.Log("Profile Dangerous screen");
            nameText.text = ProfileStore.GetProfileName();
            ProfileImageSetter.SetMyProfileImage(profileImageObject);
        }

        public void OnButtonClickImport()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileImportScene", LoadSceneMode.Single);
        }

        public void OnButtonClickExport()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileExportScene", LoadSceneMode.Single);
        }

        public void OnButtonClickDelete()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileDeleteScene", LoadSceneMode.Single);
        }
    }
}
