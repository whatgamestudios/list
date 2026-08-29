// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;

namespace Lists {

    // Persisted per-item swipe/marking state, set via ListScreen's swipe
    // gesture (SwipeableListItem) and carried along with the item so it's still
    // visible after a list is archived (ArchivedListScreen renders it
    // read-only, using the same colors via ItemSwipeColors.Get).
    public enum ItemSwipeState { Default, DarkRed, Red, DarkGreen, Green }

    public static class ItemSwipeColors {
        public static readonly Color Default = new Color(1f, 1f, 1f, 0.06f);
        public static readonly Color DarkRed = new Color(0.5f, 0.0f, 0.0f, 1.0f);
        public static readonly Color Red = new Color(0.85f, 0.3f, 0.3f, 0.5f);
        public static readonly Color DarkGreen = new Color(0.0f, 0.39f, 0.15f, 1.0f);
        public static readonly Color Green = new Color(0.3f, 0.75f, 0.4f, 0.5f);

        public static Color Get(ItemSwipeState state)
        {
            switch (state) {
                case ItemSwipeState.DarkRed: return DarkRed;
                case ItemSwipeState.Red: return Red;
                case ItemSwipeState.DarkGreen: return DarkGreen;
                case ItemSwipeState.Green: return Green;
                default: return Default;
            }
        }
    }
}
