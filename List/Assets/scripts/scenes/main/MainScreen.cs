// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class MainScreen : MonoBehaviour {

        public GameObject listsPanel;

        private const string DefaultTypeImagePath = "listtypes/listtype-scroll";
        private const string DefaultContactImagePath = "contact-images/contact-none";
        private const float ItemHeight = 200f;
        private const float ItemImageSize = 90f;
        private const float LeftColumnWidth = ItemImageSize + 20f;
        private const float AddListButtonWidthReduction = 100f;

        private RectTransform listsContent;
        private RectTransform addItemRow;

        public void Start() {
            AuditLog.Log("Main screen");
            BuildListsPanel();
        }

        public void OnButtonClickSettings()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickContacts()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ContractsScene", LoadSceneMode.Single);
        }

        public void OnButtonClickAddItem()
        {
            ListEntry entry = new ListEntry();
            ListsStore.Lists.Add(entry);
            RectTransform newRow = CreateListItemRow(entry, ListsStore.Lists.Count - 1);
            newRow.SetSiblingIndex(addItemRow.GetSiblingIndex());
            AuditLog.Log("Added new list");
        }

        private void OpenList(int index)
        {
            ListsStore.CurrentListIndex = index;
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ListScene", LoadSceneMode.Single);
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

            for (int i = 0; i < ListsStore.Lists.Count; i++)
            {
                CreateListItemRow(ListsStore.Lists[i], i);
            }

            addItemRow = CreateAddItemRow();

            scrollObj.SetActive(true);
        }

        private RectTransform CreateListItemRow(ListEntry entry, int index)
        {
            GameObject row = new GameObject("ListItem", typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = ItemHeight;

            Image rowBackground = row.GetComponent<Image>();
            rowBackground.color = new Color(1f, 1f, 1f, 0.06f);

            Button rowButton = row.GetComponent<Button>();
            rowButton.targetGraphic = rowBackground;
            rowButton.onClick.AddListener(() => OpenList(index));

            GameObject typeImageObj = new GameObject("TypeImage", typeof(RectTransform), typeof(Image));
            RectTransform typeImageRect = typeImageObj.GetComponent<RectTransform>();
            typeImageRect.SetParent(rowRect, false);
            typeImageRect.anchorMin = new Vector2(0, 1);
            typeImageRect.anchorMax = new Vector2(0, 1);
            typeImageRect.pivot = new Vector2(0, 1);
            typeImageRect.sizeDelta = new Vector2(ItemImageSize, ItemImageSize);
            typeImageRect.anchoredPosition = Vector2.zero;
            Image typeImage = typeImageObj.GetComponent<Image>();
            typeImage.sprite = Resources.Load<Sprite>(DefaultTypeImagePath);
            typeImage.preserveAspect = true;

            GameObject contactImageObj = new GameObject("ContactImage", typeof(RectTransform), typeof(Image));
            RectTransform contactImageRect = contactImageObj.GetComponent<RectTransform>();
            contactImageRect.SetParent(rowRect, false);
            contactImageRect.anchorMin = new Vector2(0, 0);
            contactImageRect.anchorMax = new Vector2(0, 0);
            contactImageRect.pivot = new Vector2(0, 0);
            contactImageRect.sizeDelta = new Vector2(ItemImageSize, ItemImageSize);
            contactImageRect.anchoredPosition = Vector2.zero;
            Image contactImage = contactImageObj.GetComponent<Image>();
            contactImage.sprite = Resources.Load<Sprite>(DefaultContactImagePath);
            contactImage.preserveAspect = true;

            // Text
            GameObject titleObj = new GameObject("Title", typeof(RectTransform), typeof(Image));
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.SetParent(rowRect, false);
            titleRect.anchorMin = new Vector2(0, 0);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.offsetMin = new Vector2(LeftColumnWidth, 0);
            titleRect.offsetMax = new Vector2(0, 0);
            Image titleBackground = titleObj.GetComponent<Image>();
            titleBackground.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(titleRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(20, 10);
            textRect.offsetMax = new Vector2(-20, -10);
            TextMeshProUGUI titleText = textObj.GetComponent<TextMeshProUGUI>();
            titleText.text = string.IsNullOrEmpty(entry.Title) ? "New Item" : entry.Title;
            titleText.fontSize = 80;
            titleText.color = Color.black;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;

            row.SetActive(true);
            return rowRect;
        }

        private RectTransform CreateAddItemRow()
        {
            GameObject row = new GameObject("AddItemButton", typeof(RectTransform), typeof(LayoutElement));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = 140f;

            // Narrower visual button inset within the full-width layout row, so it stays
            // AddListButtonWidthReduction pixels narrower than the other rows without fighting
            // the VerticalLayoutGroup's childForceExpandWidth on the row itself.
            GameObject buttonObj = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.SetParent(rowRect, false);
            buttonRect.anchorMin = Vector2.zero;
            buttonRect.anchorMax = Vector2.one;
            buttonRect.offsetMin = new Vector2(AddListButtonWidthReduction / 2f, 0);
            buttonRect.offsetMax = new Vector2(-AddListButtonWidthReduction / 2f, 0);

            Image background = buttonObj.GetComponent<Image>();
            background.color = new Color(0.2f, 0.45f, 0.85f, 1f);

            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(OnButtonClickAddItem);

            GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(buttonRect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.one;
            labelRect.offsetMax = Vector2.one;
            TextMeshProUGUI label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "+ Add List";
            label.fontSize = 50;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            row.SetActive(true);
            return rowRect;
        }
    }
}
