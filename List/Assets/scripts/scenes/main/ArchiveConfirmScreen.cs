// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lists {

    public class ArchiveConfirmScreen : MonoBehaviour {

        public void OnButtonClickYes()
        {
            if (ListsStore.CurrentListIndex >= 0) {
                ListsStore.ArchiveList(ListsStore.CurrentListIndex);
            }
            AuditLog.Log("Archived list");

            SceneStack.Instance().Reset();
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        public void OnButtonClickNo()
        {
            SceneManager.UnloadSceneAsync("ArchiveAreYouSure");
        }
    }
}
