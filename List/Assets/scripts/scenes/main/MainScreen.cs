// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class MenuScreen : MonoBehaviour {

        public GameObject listsPanel;

        private const string DefaultTypeImagePath = "listtypes/listtype-banana";
        private const string DefaultContactImagePath = "contact-images/contact-woman1";
        private const float ItemHeight = 300f;
        private const float ItemImageSize = 140f;
        private const float LeftColumnWidth = ItemImageSize + 20f;

        private RectTransform listsContent;
        private RectTransform addItemRow;

        public void Start() {
            AuditLog.Log("Main screen");
            BuildListsPanel();
            OnButtonClickAddItem();
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
            RectTransform newRow = CreateListItemRow();
            newRow.SetSiblingIndex(addItemRow.GetSiblingIndex());
            AuditLog.Log("Added new list item");
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
            scrollRect.offsetMin = Vector2.zero;
            scrollRect.offsetMax = Vector2.zero;

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

            addItemRow = CreateAddItemRow();

            scrollObj.SetActive(true);
        }

        private RectTransform CreateListItemRow()
        {
            GameObject row = new GameObject("ListItem", typeof(RectTransform), typeof(LayoutElement));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = ItemHeight;

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

            CreateTitleInputField(rowRect);

            row.SetActive(true);
            return rowRect;
        }

        private void CreateTitleInputField(RectTransform rowRect)
        {
            GameObject inputObj = new GameObject("TitleInput", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.SetParent(rowRect, false);
            inputRect.anchorMin = new Vector2(0, 0);
            inputRect.anchorMax = new Vector2(1, 1);
            inputRect.offsetMin = new Vector2(LeftColumnWidth, 0);
            inputRect.offsetMax = new Vector2(0, 0);
            Image inputBackground = inputObj.GetComponent<Image>();
            inputBackground.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            GameObject textAreaObj = new GameObject("TextArea", typeof(RectTransform), typeof(RectMask2D));
            RectTransform textAreaRect = textAreaObj.GetComponent<RectTransform>();
            textAreaRect.SetParent(inputRect, false);
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = new Vector2(20, 10);
            textAreaRect.offsetMax = new Vector2(-20, -10);

            GameObject placeholderObj = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.SetParent(textAreaRect, false);
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;
            TextMeshProUGUI placeholderText = placeholderObj.GetComponent<TextMeshProUGUI>();
            placeholderText.text = "Enter title";
            placeholderText.fontSize = 45;
            placeholderText.fontStyle = FontStyles.Italic;
            placeholderText.color = new Color(0f, 0f, 0f, 0.5f);
            placeholderText.alignment = TextAlignmentOptions.MidlineLeft;

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(textAreaRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI titleText = textObj.GetComponent<TextMeshProUGUI>();
            titleText.fontSize = 45;
            titleText.color = Color.black;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.targetGraphic = inputBackground;
            inputField.textViewport = textAreaRect;
            inputField.textComponent = titleText;
            inputField.placeholder = placeholderText;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
            inputField.characterLimit = 60;
        }

        private RectTransform CreateAddItemRow()
        {
            GameObject row = new GameObject("AddItemButton", typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = 140f;

            Image background = row.GetComponent<Image>();
            background.color = new Color(0.2f, 0.45f, 0.85f, 1f);

            Button button = row.GetComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(OnButtonClickAddItem);

            GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(rowRect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "+ Add Item";
            label.fontSize = 50;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;

            row.SetActive(true);
            return rowRect;
        }
    }
}
