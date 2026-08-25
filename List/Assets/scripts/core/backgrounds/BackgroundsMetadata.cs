// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Lists {
    public class BackgroundsMetadata {

        public static int OptionToGeneration(int option) {
            return option / 100;
        }
        public static int OptionToType(int option) {
            return option % 100;
        }

        public static SceneInfo GetInfo(int option) {
            int generation = OptionToGeneration(option);
            int type = OptionToType(option);

            switch (generation) {
                case 0:
                    return getFreeInfo(type);
                case 1:
                    return getGen1Info(type);
                case 2:
                    return getGen2Info(type);
                case 3:
                    return getGen3Info(type);
                default:
                    AuditLog.Log($"Unknown generation: {generation}");
                    return getDefaultInfo();
            }
        }


        private static SceneInfo getDefaultInfo() {
            SceneInfo info = new SceneInfo("scenes/free/free-type1-coffee", UnityEngine.Color.white, UnityEngine.Color.black);
            info.SetMetadata("Coffee Beans", "Free", "Common", 0, "Frendy Adew");
            return info;
        }


        public static SceneInfo getFreeInfo(int type) {
            SceneInfo info;
            switch (type) {
                case 1:
                    info = getDefaultInfo();
                    break;
                case 2:
                    info = new SceneInfo("scenes/free/free-type2-dogs", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Happy Dogs", "Free", "Common", 0, "Anna Allakuz");
                    break;
                case 3:
                    info = new SceneInfo("scenes/free/free-type3-koi", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Koi", "Free", "Common", 0, "Olga Graholskaya");
                    break;
                default:
                    AuditLog.Log($"BackgroundMetadata: Unknown free type: {type}");
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static SceneInfo getGen1Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen1/gen1-type0-goldenfans", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Golden Fans", "Gen1", "Mythical", 10, "Natata");
                    break;
                case 1:
                    info = new SceneInfo("scenes/gen1/gen1-type1-brightcats", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Bright Cats", "Gen1", "Legendary", 25, "Burbura");
                    break;
                case 2:
                    info = new SceneInfo("scenes/gen1/gen1-type2-mixedflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Fantasy Flowers", "Gen1", "Epic", 100, "Elen Lane");
                    break;
                case 3:
                    info = new SceneInfo("scenes/gen1/gen1-type3-yellowflowers", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Yellow Flowers", "Gen1", "Common", 1000, "Unknown");
                    break;
                default:
                    AuditLog.Log($"Unknown gen1 type: {type}");
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static SceneInfo getGen2Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen2/gen2-type0-clocks", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Clocks", "Gen2", "Mythical", 20, "Usova Olga");
                    break;
                case 1:
                    info = new SceneInfo("scenes/gen2/gen2-type1-cats", UnityEngine.Color.white, UnityEngine.Color.white);
                    info.SetMetadata("Cats", "Gen2", "Legendary", 50, "Natalia Zagory");
                    break;
                case 2:
                    info = new SceneInfo("scenes/gen2/gen2-type2-tea", UnityEngine.Color.white, UnityEngine.Color.white);
                    info.SetMetadata("Tea", "Gen2", "Epic", 100, "Olka Kostenko");
                    break;
                case 3:
                    info = new SceneInfo("scenes/gen2/gen2-type3-circuit", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Circuit", "Gen2", "Epic", 100, "Amgun");
                    break;
                case 4:
                    info = new SceneInfo("scenes/gen2/gen2-type4-wild-tea", UnityEngine.Color.white, UnityEngine.Color.white);
                    info.SetMetadata("Wild Tea", "Gen2", "Rare", 400, "Olka Kostenko");
                    break;
                case 5:
                    info = new SceneInfo("scenes/gen2/gen2-type5-space", UnityEngine.Color.white, UnityEngine.Color.white);
                    info.SetMetadata("Space", "Gen2", "Common", 1000, "Hatcha");
                    break;
                case 6:
                    info = new SceneInfo("scenes/gen2/gen2-type6-garden", UnityEngine.Color.black, UnityEngine.Color.black);
                    info.SetMetadata("Garden", "Gen2", "Common", 1000, "Burbura");
                    break;
                default:
                    AuditLog.Log($"Unknown gen2 type: {type}");
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static SceneInfo getGen3Info(int type) {
            SceneInfo info;
            switch (type) {
                case 0:
                    info = new SceneInfo("scenes/gen3/gen3-type0-honeycomb", UnityEngine.Color.white, UnityEngine.Color.black);
                    info.SetMetadata("Honeycomb", "Gen3", "Mythical", 20, "Bun Mihail");
                    break;
                default:
                    AuditLog.Log($"Unknown gen3 type: {type}");
                    info = getDefaultInfo();
                    break;
            }
            return info;
        }

        public static int ButtonTextToOption(string buttonText) {
            if (buttonText == "free1") {
                return 1;
            }
            else if (buttonText == "free2") {
                return 2;
            }
            else if (buttonText == "free3") {
                return 3;
            }
            else if (buttonText == "gen1_0") {
                return 100;
            }
            else if (buttonText == "gen1_1") {
                return 101;
            }
            else if (buttonText == "gen1_2") {
                return 102;
            }
            else if (buttonText == "gen1_3") {
                return 103;
            }
            else if (buttonText == "gen2_0") {
                return 200;
            }
            else if (buttonText == "gen2_1") {
                return 201;
            }
            else if (buttonText == "gen2_2") {
                return 202;
            }
            else if (buttonText == "gen2_3") {
                return 203;
            }
            else if (buttonText == "gen2_4") {
                return 204;
            }
            else if (buttonText == "gen2_5") {
                return 205;
            }
            else if (buttonText == "gen2_6") {
                return 206;
            }
            else if (buttonText == "gen3_0") {
                return 300;
            }
            else {
                // Default
                AuditLog.Log("Background Meta: Unknown button: " + buttonText);
                return 1;
            }
        }
    }
}