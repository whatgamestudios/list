// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    // Read-only view of a single archived list: titles and items cannot be edited,
    // and no new items can be added.
    public class ArchivedListScreen : MonoBehaviour {

        public GameObject listsPanel;
        public TextMeshProUGUI titleText;

        private const float ItemHeight = 200f;
        private const float LeftColumnWidth = 20f;

        private RectTransform listsContent;
        private ListEntry list;

        public void Start() {
            AuditLog.Log("Archived list screen");
            list = ListsStore.ArchivedLists[ListsStore.CurrentArchivedListIndex];
            if (titleText != null) {
                titleText.text = list.Title;
            }
            BuildListsPanel();
        }

        public void OnButtonClickRestore()
        {
            AuditLog.Log("Restored list: " + list.Title);
            ListsStore.RestoreList(ListsStore.CurrentArchivedListIndex);

            SceneStack.Instance().Reset();
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        public void OnButtonClickDeletePermanently()
        {
            SceneManager.LoadScene("DeleteAreYouSure", LoadSceneMode.Additive);
        }

        private void BuildListsPanel()
        {
            RectTransform panelRect = listsPanel.GetComponent<RectTransform>();

            GameObject scrollObj = new GameObject("ListsScrollView", typeof(RectTransform));
            scrollObj.SetActive(false);
            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.SetParent(panelRect, false);
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = new Vector2(0, 25);
            scrollRect.offsetMax = new Vector2(0, -25);

            GameObject viewportObj = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            RectTransform viewportRect = viewportObj.GetComponent<RectTransform>();
            viewportRect.SetParent(scrollRect, false);
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            GameObject contentObj = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            listsContent = contentObj.GetComponent<RectTransform>();
            listsContent.SetParent(viewportRect, false);
            listsContent.anchorMin = new Vector2(0, 1);
            listsContent.anchorMax = new Vector2(1, 1);
            listsContent.pivot = new Vector2(0.5f, 1);
            listsContent.anchoredPosition = Vector2.zero;
            listsContent.sizeDelta = new Vector2(0, 0);

            VerticalLayoutGroup vlg = contentObj.GetComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.spacing = 30;
            vlg.padding = new RectOffset(20, 20, 20, 20);

            ContentSizeFitter csf = contentObj.GetComponent<ContentSizeFitter>();
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
            scroll.content = listsContent;
            scroll.viewport = viewportRect;
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30;

            for (int i = 0; i < list.Items.Count; i++)
            {
                CreateListItemRow(i);
            }

            scrollObj.SetActive(true);
        }

        private RectTransform CreateListItemRow(int index)
        {
            GameObject row = new GameObject("ListItem", typeof(RectTransform), typeof(LayoutElement));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = ItemHeight;

            CreateItemLabel(rowRect, index);

            row.SetActive(true);
            return rowRect;
        }

        private void CreateItemLabel(RectTransform rowRect, int index)
        {
            GameObject labelObj = new GameObject("ItemLabel", typeof(RectTransform), typeof(Image));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(rowRect, false);
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(LeftColumnWidth, 0);
            labelRect.offsetMax = new Vector2(0, 0);
            Image labelBackground = labelObj.GetComponent<Image>();
            labelBackground.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(labelRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20, 10);
            textRect.offsetMax = new Vector2(-20, -10);
            TextMeshProUGUI itemText = textObj.GetComponent<TextMeshProUGUI>();
            itemText.text = list.Items[index];
            itemText.fontSize = 45;
            itemText.color = Color.black;
            itemText.alignment = TextAlignmentOptions.MidlineLeft;
        }
    }
}
