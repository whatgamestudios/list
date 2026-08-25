// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {
    /**
     * Screen shown when play presses on a scene in the scene selector screen, when the 
     * scene is already selected.
     */
    public class SceneDetailScreen : MonoBehaviour {

        public TextMeshProUGUI sceneName;
        public TextMeshProUGUI sceneMetadata;


        public void Start() {
            int tokenId = SceneStore.GetBackground();
            AuditLog.Log("Scene detail screen: " + tokenId.ToString());
            SceneInfo sceneInfo = BackgroundsMetadata.GetInfo(tokenId);
            sceneName.text = sceneInfo.name;
            sceneMetadata.text = sceneInfo.artist;
        }
    }
}