// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Lists {
    public class ProfileMetadata {

        public static string GetPhotoResource(int type) {
            switch (type) {
                case 0:
                    return "contact-images/type0";
                case 1:
                    return "contact-images/type1";
                default:
                    AuditLog.Log($"ProfileMetadata: Unknown type: {type}");
                    return "contact-images/type0";
            }
        }

        public static int ButtonTextToType(string buttonText) {
            if (buttonText == "type0") {
                return 0;
            }
            else if (buttonText == "type1") {
                return 1;
            }
            else if (buttonText == "type2") {
                return 2;
            }
            else if (buttonText == "type3") {
                return 3;
            }
            else if (buttonText == "type4") {
                return 4;
            }
            else if (buttonText == "type5") {
                return 5;
            }
            else if (buttonText == "type6") {
                return 6;
            }
            else if (buttonText == "type7") {
                return 7;
            }
            else if (buttonText == "type8") {
                return 8;
            }
            else if (buttonText == "type9") {
                return 9;
            }
            else if (buttonText == "type10") {
                return 10;
            }
            else if (buttonText == "type11") {
                return 11;
            }
            else if (buttonText == "type12") {
                return 12;
            }
            else if (buttonText == "type13") {
                return 13;
            }
            else if (buttonText == "type14") {
                return 14;
            }
            else {
                // Default
                AuditLog.Log("Profile Meta: Unknown button: " + buttonText);
                return 0;
            }
        }
    }
}