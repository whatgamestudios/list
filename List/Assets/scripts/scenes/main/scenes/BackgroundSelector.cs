// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace Lists {

    public class BackgroundSelector : MonoBehaviour {
        public GameObject panelScreenBackground;

        public GameObject panelFreeType1;
        public GameObject panelFreeType2;
        public GameObject panelFreeType3;
        public GameObject panelGen1Type0;
        public GameObject panelGen1Type1;
        public GameObject panelGen1Type2;
        public GameObject panelGen1Type3;
        public GameObject panelGen2Type0;
        public GameObject panelGen2Type1;
        public GameObject panelGen2Type2;
        public GameObject panelGen2Type3;
        public GameObject panelGen2Type4;
        public GameObject panelGen2Type5;
        public GameObject panelGen2Type6;
        public GameObject panelGen3Type0;


        public void Start()
        {
            AuditLog.Log("Scene screen");

            int selected = SceneStore.GetBackground();
            setSelected(0, selected);
        }

        public void OnButtonClick(string buttonText) {
            // One of the image buttons has been pressed.
            int option = BackgroundsMetadata.ButtonTextToOption(buttonText);
            int alreadySelectedOption = SceneStore.GetBackground();
            if (option == alreadySelectedOption) {
                SceneStack.Instance().PushScene();
                SceneManager.LoadScene("SceneDetailScene", LoadSceneMode.Additive);
            }
            else {
                SceneStore.SetBackground(option);
                setSelected(alreadySelectedOption, option);
                ScreenBackgroundSetter.SetPanelBackground(panelScreenBackground);
            }
        }

        private void setSelected(int previouslySelected, int selected) {
            AuditLog.Log($"Background Selector: prev: {previouslySelected}, selected: {selected}");

            switch (previouslySelected) {
                case 1:
                    setCol(panelFreeType1, false);
                    break;
                case 2:
                    setCol(panelFreeType2, false);
                    break;
                case 3:
                    setCol(panelFreeType3, false);
                    break;
                case 100:
                    setCol(panelGen1Type0, false);
                    break;
                case 101:
                    setCol(panelGen1Type1, false);
                    break;
                case 102:
                    setCol(panelGen1Type2, false);
                    break;
                case 103:
                    setCol(panelGen1Type3, false);
                    break;
                case 200:
                    setCol(panelGen2Type0, false);
                    break;
                case 201:
                    setCol(panelGen2Type1, false);
                    break;
                case 202:
                    setCol(panelGen2Type2, false);
                    break;
                case 203:
                    setCol(panelGen2Type3, false);
                    break;
                case 204:
                    setCol(panelGen2Type4, false);
                    break;
                case 205:
                    setCol(panelGen2Type5, false);
                    break;
                case 206:
                    setCol(panelGen2Type6, false);
                    break;
                case 300:
                    setCol(panelGen3Type0, false);
                    break;
            }

            switch (selected) {
                case 1:
                    setCol(panelFreeType1, true);
                    break;
                case 2:
                    setCol(panelFreeType2, true);
                    break;
                case 3:
                    setCol(panelFreeType3, true);
                    break;
                case 100:
                    setCol(panelGen1Type0, true);
                    break;
                case 101:
                    setCol(panelGen1Type1, true);
                    break;
                case 102:
                    setCol(panelGen1Type2, true);
                    break;
                case 103:
                    setCol(panelGen1Type3, true);
                    break;
                case 200:
                    setCol(panelGen2Type0, true);
                    break;
                case 201:
                    setCol(panelGen2Type1, true);
                    break;
                case 202:
                    setCol(panelGen2Type2, true);
                    break;
                case 203:
                    setCol(panelGen2Type3, true);
                    break;
                case 204:
                    setCol(panelGen2Type4, true);
                    break;
                case 205:
                    setCol(panelGen2Type5, true);
                    break;
                case 206:
                    setCol(panelGen2Type6, true);
                    break;
                case 300:
                    setCol(panelGen3Type0, true);
                    break;
            }
        }

        private void setCol(GameObject border, bool isSelected)
        {
            Image img = border.GetComponent<Image>();
            if (isSelected)
            {
                img.color = UnityEngine.Color.red;                      
            }
            else
            {
                img.color = UnityEngine.Color.black;          
            }
        }
    }
}