// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

namespace Lists {

    public class ProfileScreen : MonoBehaviour {
        public GameObject panelType0;
        public GameObject panelType1;
        public GameObject panelType2;
        public GameObject panelType3;
        public GameObject panelType4;
        public GameObject panelType5;
        public GameObject panelType6;
        public GameObject panelType7;
        public GameObject panelType8;
        public GameObject panelType9;
        public GameObject panelType10;
        public GameObject panelType11;
        public GameObject panelType12;
        public GameObject panelType13;
        public GameObject panelType14;


        public void Start()
        {
            AuditLog.Log("Profile screen");

            int selected = ProfileStore.GetProfileImageType();
            setSelected(0, selected);
        }

        public void OnButtonClick(string buttonText) {
            // One of the image buttons has been pressed.
            int option = ProfileMetadata.ButtonTextToType(buttonText);
            int alreadySelectedOption = ProfileStore.GetProfileImageType();
            ProfileStore.SetProfileImageType(option);
            setSelected(alreadySelectedOption, option);
        }

        private void setSelected(int previouslySelected, int selected) {
            AuditLog.Log($"Background Selector: prev: {previouslySelected}, selected: {selected}");

            switch (previouslySelected) {
                case 0:
                    setCol(panelType0, false);
                    break;
                case 1:
                    setCol(panelType1, false);
                    break;
                case 2:
                    setCol(panelType2, false);
                    break;
                case 3:
                    setCol(panelType3, false);
                    break;
                case 4:
                    setCol(panelType4, false);
                    break;
                case 5:
                    setCol(panelType5, false);
                    break;
                case 6:
                    setCol(panelType6, false);
                    break;
                case 7:
                    setCol(panelType7, false);
                    break;
                case 8:
                    setCol(panelType8, false);
                    break;
                case 9:
                    setCol(panelType9, false);
                    break;
                case 10:
                    setCol(panelType10, false);
                    break;
                case 11:
                    setCol(panelType11, false);
                    break;
                case 12:
                    setCol(panelType12, false);
                    break;
                case 13:
                    setCol(panelType13, false);
                    break;
                case 14:
                    setCol(panelType14, false);
                    break;
            }

            switch (selected) {
                case 0:
                    setCol(panelType0, true);
                    break;
                case 1:
                    setCol(panelType1, true);
                    break;
                case 2:
                    setCol(panelType2, true);
                    break;
                case 3:
                    setCol(panelType3, true);
                    break;
                case 4:
                    setCol(panelType4, true);
                    break;
                case 5:
                    setCol(panelType5, true);
                    break;
                case 6:
                    setCol(panelType6, true);
                    break;
                case 7:
                    setCol(panelType7, true);
                    break;
                case 8:
                    setCol(panelType8, true);
                    break;
                case 9:
                    setCol(panelType9, true);
                    break;
                case 10:
                    setCol(panelType10, true);
                    break;
                case 11:
                    setCol(panelType11, true);
                    break;
                case 12:
                    setCol(panelType12, true);
                    break;
                case 13:
                    setCol(panelType13, true);
                    break;
                case 14:
                    setCol(panelType14, true);
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