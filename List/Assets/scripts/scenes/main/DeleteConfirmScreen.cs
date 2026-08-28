// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lists {

    // Popup opened additively (on top of ArchivedListScene) by
    // ArchivedListScreen.OnButtonClickDeletePermanently.
    public class DeleteConfirmScreen : MonoBehaviour {

        public void OnButtonClickYes()
        {
            AuditLog.Log("Permanently deleted list");
            ListsStore.DeleteArchivedList(ListsStore.CurrentArchivedListIndex);

            SceneStack.Instance().Reset();
            SceneManager.LoadScene("ArchivedListsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickNo()
        {
            SceneManager.UnloadSceneAsync("DeleteAreYouSure");
        }
    }
}
