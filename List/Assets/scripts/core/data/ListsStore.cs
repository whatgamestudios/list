// Copyright (c) Whatgame Studios 2024 - 2026
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lists {

    [Serializable]
    public class ListEntry {
        public string Title = "";
        public List<string> Items = new List<string>();

        // Per-item swipe/marking state (ListScreen), kept in lockstep with Items
        // by index. Carried along automatically when a list moves between Lists
        // and ArchivedLists, since it's just a field of the same entry - see
        // ArchivedListScreen, which renders it read-only.
        public List<ItemSwipeState> ItemStates = new List<ItemSwipeState>();

        // Pads/trims ItemStates to match Items.Count. Defensive against legacy
        // saved data from before ItemStates existed, and cheap enough to call
        // before every read/write of either list.
        public void EnsureItemStatesLength()
        {
            if (ItemStates == null) {
                ItemStates = new List<ItemSwipeState>();
            }
            while (ItemStates.Count < Items.Count) {
                ItemStates.Add(ItemSwipeState.Default);
            }
            if (ItemStates.Count > Items.Count) {
                ItemStates.RemoveRange(Items.Count, ItemStates.Count - Items.Count);
            }
        }
    }

    // Store of the lists and their items, persisted to PlayerPrefs.
    public static class ListsStore {
        private const string StoreKey = "LISTS_STORE";

        public static List<ListEntry> Lists;
        public static List<ListEntry> ArchivedLists;
        public static int CurrentListIndex = -1;
        public static int CurrentArchivedListIndex = -1;

        static ListsStore() {
            Load();
        }

        // Moves the list at index from Lists into ArchivedLists and persists the change.
        public static void ArchiveList(int index)
        {
            ListEntry entry = Lists[index];
            Lists.RemoveAt(index);
            ArchivedLists.Add(entry);
            if (CurrentListIndex == index) {
                CurrentListIndex = -1;
            }
            Save();
        }

        // Moves the list at index from ArchivedLists back into Lists and persists the change.
        public static void RestoreList(int index)
        {
            ListEntry entry = ArchivedLists[index];
            ArchivedLists.RemoveAt(index);
            Lists.Add(entry);
            if (CurrentArchivedListIndex == index) {
                CurrentArchivedListIndex = -1;
            }
            Save();
        }

        // Permanently removes the list at index from ArchivedLists. Cannot be undone.
        public static void DeleteArchivedList(int index)
        {
            ArchivedLists.RemoveAt(index);
            if (CurrentArchivedListIndex == index) {
                CurrentArchivedListIndex = -1;
            }
            Save();
        }

        public static void Save() {
            SaveData data = new SaveData { Lists = Lists, ArchivedLists = ArchivedLists, CurrentListIndex = CurrentListIndex };
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(StoreKey, json);
            PlayerPrefs.Save();
        }

        private static void Load() {
            string json = PlayerPrefs.GetString(StoreKey, "");
            if (string.IsNullOrEmpty(json)) {
                Lists = new List<ListEntry>();
                ArchivedLists = new List<ListEntry>();
                return;
            }
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Lists = data.Lists ?? new List<ListEntry>();
            ArchivedLists = data.ArchivedLists ?? new List<ListEntry>();
            CurrentListIndex = data.CurrentListIndex;

            foreach (ListEntry entry in Lists) {
                entry.EnsureItemStatesLength();
            }
            foreach (ListEntry entry in ArchivedLists) {
                entry.EnsureItemStatesLength();
            }
        }

        [Serializable]
        private class SaveData {
            public List<ListEntry> Lists = new List<ListEntry>();
            public List<ListEntry> ArchivedLists = new List<ListEntry>();
            public int CurrentListIndex = -1;
        }
    }
}
