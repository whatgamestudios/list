// Copyright (c) Whatgame Studios 2024 - 2026
using System.Collections.Generic;

namespace Lists {

    public class ListEntry {
        public string Title = "";
        public List<string> Items = new List<string>();
    }

    // In-memory store of the lists and their items for the current session.
    public static class ListsStore {
        public static List<ListEntry> Lists = new List<ListEntry>();
        public static int CurrentListIndex = -1;
    }
}
