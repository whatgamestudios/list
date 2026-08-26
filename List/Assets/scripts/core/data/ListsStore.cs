// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lists {

    [Serializable]
    public class ListEntry {
        public string Title = "";
        public List<string> Items = new List<string>();
    }

    // Store of the lists and their items, persisted to PlayerPrefs.
    public static class ListsStore {
        private const string StoreKey = "LISTS_STORE";

        public static List<ListEntry> Lists;
        public static int CurrentListIndex = -1;

        static ListsStore() {
            Load();
        }

        public static void Save() {
            SaveData data = new SaveData { Lists = Lists, CurrentListIndex = CurrentListIndex };
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(StoreKey, json);
            PlayerPrefs.Save();
        }

        private static void Load() {
            string json = PlayerPrefs.GetString(StoreKey, "");
            if (string.IsNullOrEmpty(json)) {
                Lists = new List<ListEntry>();
                return;
            }
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Lists = data.Lists ?? new List<ListEntry>();
            CurrentListIndex = data.CurrentListIndex;
        }

        [Serializable]
        private class SaveData {
            public List<ListEntry> Lists = new List<ListEntry>();
            public int CurrentListIndex = -1;
        }
    }
}
