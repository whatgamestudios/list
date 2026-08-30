// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;

namespace Lists {

    public enum ItemFontSize { Small, Medium, Large, XLarge }

    // Persisted (PlayerPrefs) font size used for item text - ListScreen and
    // ArchivedListScreen. Default and current-baseline value is Medium (45pt,
    // matching what item text was hardcoded to before this setting existed).
    public static class ItemFontSizeSettings {
        private const string PrefsKey = "ITEM_FONT_SIZE";
        public const ItemFontSize Default = ItemFontSize.Medium;

        public static ItemFontSize Current {
            get => (ItemFontSize) PlayerPrefs.GetInt(PrefsKey, (int) Default);
            set {
                PlayerPrefs.SetInt(PrefsKey, (int) value);
                PlayerPrefs.Save();
            }
        }

        public static float CurrentPointSize => GetPointSize(Current);

        public static float GetPointSize(ItemFontSize size)
        {
            switch (size) {
                case ItemFontSize.Small: return 35f;
                case ItemFontSize.Large: return 55f;
                case ItemFontSize.XLarge: return 65f;
                default: return 45f;
            }
        }

        // Row height for item rows (ListScreen, ArchivedListScreen): the text
        // itself (treated as one font-size tall) plus half a font-size of
        // whitespace above and half below, i.e. double the point size.
        public static float CurrentRowHeight => GetPointSize(Current) * 2f;
    }
}
