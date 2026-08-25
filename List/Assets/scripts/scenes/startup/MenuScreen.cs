// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class MenuScreen : MonoBehaviour {

        public void Start() {
            AuditLog.Log("Menu screen");
        }


        public void OnButtonClick(string buttonText)
        {
            if (buttonText == "Play")
            {
                MessagePass.SetMsg(null);
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("GamePlayScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Stats")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("StatsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Solutions")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SolutionsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Backgrounds")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("BackgroundsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Lore")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("LoreScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Rules")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("RulesScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Strategy")
            {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("StrategyScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Socials") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SocialsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Settings") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "Credits") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Single);
            }
            else if (buttonText == "WordList") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("WordListScene", LoadSceneMode.Single);
            }
            else if (buttonText == "HowToPlay") {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("HowToPlayScene", LoadSceneMode.Single);
            }
            else
            {
                AuditLog.Log($"Menu: Unknown button {buttonText}");
                return;
            }
        }
    }
}
