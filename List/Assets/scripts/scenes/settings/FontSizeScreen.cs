// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Lists {

    public class FontSizeScreen : MonoBehaviour {

        public GameObject panel;

        private static readonly (string Label, ItemFontSize Size)[] Options = {
            ("Small", ItemFontSize.Small),
            ("Medium", ItemFontSize.Medium),
            ("Large", ItemFontSize.Large),
            ("X Large", ItemFontSize.XLarge),
        };

        private RectTransform content;
        private readonly List<(Image Background, ItemFontSize Size)> optionRows = new List<(Image, ItemFontSize)>();

        public void Start()
        {
            AuditLog.Log("Font size screen");
            BuildUi();
        }

        private void SelectFontSize(ItemFontSize size)
        {
            ItemFontSizeSettings.Current = size;
            RefreshSelection();
            AuditLog.Log($"Item font size set to {size}");
        }

        private void RefreshSelection()
        {
            ItemFontSize current = ItemFontSizeSettings.Current;
            foreach (var row in optionRows) {
                bool selected = row.Size == current;
                row.Background.color = selected
                    ? new Color(0.2f, 0.45f, 0.85f, 1f)
                    : new Color(1f, 1f, 1f, 0.06f);
            }
        }

        private void BuildUi()
        {
            RectTransform panelRect = panel.GetComponent<RectTransform>();

            GameObject scrollObj = new GameObject("FontSizeScrollView", typeof(RectTransform));
            scrollObj.SetActive(false);
            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.SetParent(panelRect, false);
            // Horizontally: a fixed 900px width centered on the panel (point anchor
            // at x=0.5 rather than a stretch). Vertically: only the top edge (where
            // the top-aligned content starts) moves down another 50px, from
            // panel-local y=755 to y=705 - the bottom edge stays put, 40px above
            // the BackButton, since moving it too would eat into that clearance.
            scrollRect.anchorMin = new Vector2(0.5f, 0f);
            scrollRect.anchorMax = new Vector2(0.5f, 1f);
            scrollRect.offsetMin = new Vector2(-450, 170);
            scrollRect.offsetMax = new Vector2(450, -465);

            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.SetParent(scrollRect, false);
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            content = contentObj.GetComponent<RectTransform>();
            content.SetParent(viewportRect, false);
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup vlg = contentObj.GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 20;
            vlg.padding = new RectOffset(20, 20, 20, 20);

            ContentSizeFitter csf = contentObj.GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.content = content;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30;

            foreach (var option in Options) {
                CreateOptionRow(option.Label, option.Size);
            }
            RefreshSelection();

            scrollObj.SetActive(true);
        }

        private void CreateOptionRow(string labelText, ItemFontSize size)
        {
            GameObject row = new GameObject("FontSizeOption_" + size, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(content, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = 110f;

            Image rowBackground = row.GetComponent<Image>();
            rowBackground.color = new Color(1f, 1f, 1f, 0.06f);

            Button rowButton = row.GetComponent<Button>();
            rowButton.targetGraphic = rowBackground;
            rowButton.onClick.AddListener(() => SelectFontSize(size));

            GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(rowRect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(30, 0);
            labelRect.offsetMax = new Vector2(-30, 0);
            TextMeshProUGUI label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = 48;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;

            optionRows.Add((rowBackground, size));
        }
    }
}
