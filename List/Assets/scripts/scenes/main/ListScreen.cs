// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Lists {

    public class ListScreen : MonoBehaviour {

        public GameObject listsPanel;
        public TextMeshProUGUI titleText;

        private const string DefaultTypeImagePath = "listtypes/listtype-scroll";
        private const string DefaultContactImagePath = "contact-images/contact-none";
        private const float LeftColumnWidth = 20f;
        private const float AddListButtonWidthReduction = 100f;

        private RectTransform listsContent;
        private GameObject scrollViewObj;
        private ListEntry list;

        public void Start() {
            AuditLog.Log("List screen");
            list = ListsStore.Lists[ListsStore.CurrentListIndex];
            list.EnsureItemStatesLength();
            BuildTitleField();
            BuildListsPanel();
        }

        private void BuildTitleField()
        {
            if (titleText == null) return;

            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            GameObject titleObj = titleText.gameObject;
            titleObj.SetActive(false);
            Destroy(titleText);

            GameObject textAreaObj = new GameObject("TextArea", typeof(RectTransform), typeof(RectMask2D));
            RectTransform textAreaRect = textAreaObj.GetComponent<RectTransform>();
            textAreaRect.SetParent(titleRect, false);
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.offsetMin = Vector2.zero;
            textAreaRect.offsetMax = Vector2.zero;

            GameObject placeholderObj = new GameObject("Placeholder", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.SetParent(textAreaRect, false);
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = Vector2.zero;
            placeholderRect.offsetMax = Vector2.zero;
            TextMeshProUGUI placeholderText = placeholderObj.GetComponent<TextMeshProUGUI>();
            placeholderText.text = "New List";
            placeholderText.fontSize = 90;
            placeholderText.fontStyle = FontStyles.Bold;
            placeholderText.color = Color.gray;
            placeholderText.alignment = TextAlignmentOptions.Left;

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(textAreaRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI titleLabel = textObj.GetComponent<TextMeshProUGUI>();
            titleLabel.fontSize = 90;
            titleLabel.fontStyle = FontStyles.Bold;
            titleLabel.color = Color.black;
            titleLabel.alignment = TextAlignmentOptions.Left;

            TMP_InputField inputField = titleObj.AddComponent<TMP_InputField>();
            inputField.targetGraphic = titleLabel;
            inputField.textViewport = textAreaRect;
            inputField.textComponent = titleLabel;
            inputField.placeholder = placeholderText;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
            inputField.characterLimit = 60;
            inputField.text = list.Title;
            inputField.onValueChanged.AddListener(value => { list.Title = value; ListsStore.Save(); });

            titleObj.SetActive(true);
        }

        public void OnButtonClickArchive()
        {
            SceneManager.LoadScene("ArchiveAreYouSure", LoadSceneMode.Additive);
        }


        public void OnButtonClickAddItem()
        {
            list.Items.Add("");
            list.ItemStates.Add(ItemSwipeState.Default);
            ListsStore.Save();
            RectTransform newRow = CreateListItemRow(list.Items.Count - 1);
            newRow.SetSiblingIndex(list.Items.Count);

            TMP_InputField newField = newRow.GetComponentInChildren<TMP_InputField>();
            if (newField != null) {
                newField.Select();
                newField.ActivateInputField();
            }

            AuditLog.Log("Added new item");
        }

        private void DeleteItem(int index)
        {
            list.Items.RemoveAt(index);
            list.ItemStates.RemoveAt(index);
            ListsStore.Save();
            AuditLog.Log("Deleted item");

            // Every row's title-input and swipe closures capture a fixed index, so
            // once one is removed and later items shift down, the only safe way to
            // keep them all correct is to rebuild every row from scratch.
            BuildListsPanel();
        }

        private void MoveItem(int index, int direction)
        {
            int newIndex = index + direction;
            if (newIndex < 0 || newIndex >= list.Items.Count) {
                return;
            }

            string item = list.Items[index];
            ItemSwipeState itemState = list.ItemStates[index];
            list.Items.RemoveAt(index);
            list.ItemStates.RemoveAt(index);
            list.Items.Insert(newIndex, item);
            list.ItemStates.Insert(newIndex, itemState);
            ListsStore.Save();
            AuditLog.Log($"Moved item from {index} to {newIndex}");

            // Same reason as DeleteItem: rebuild rather than patch, so every
            // row's captured index stays correct.
            BuildListsPanel();
        }

        private void BuildListsPanel()
        {
            RectTransform panelRect = listsPanel.GetComponent<RectTransform>();

            if (scrollViewObj != null) {
                scrollViewObj.SetActive(false);
                Destroy(scrollViewObj);
            }

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

            // Set before the row-creation loop below, since each row's swipe
            // overlay needs it to forward vertical drags back to the scroll view.
            scrollViewObj = scrollObj;

            for (int i = 0; i < list.Items.Count; i++)
            {
                CreateListItemRow(i);
            }

            scrollObj.SetActive(true);
        }

        private RectTransform CreateListItemRow(int index)
        {
            GameObject row = new GameObject("ListItem", typeof(RectTransform), typeof(LayoutElement), typeof(Image));
            row.SetActive(false);
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(listsContent, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = ItemFontSizeSettings.CurrentRowHeight;
            Image rowBackground = row.GetComponent<Image>();
            rowBackground.color = new Color(1f, 1f, 1f, 0.06f);

            CreateTitleInputField(rowRect, index);
            CreateSwipeOverlay(rowRect, rowBackground, index);

            row.SetActive(true);
            return rowRect;
        }

        private void CreateSwipeOverlay(RectTransform rowRect, Image rowBackground, int index)
        {
            GameObject overlayObj = new GameObject("SwipeOverlay", typeof(RectTransform), typeof(Image), typeof(SwipeableListItem));
            RectTransform overlayRect = overlayObj.GetComponent<RectTransform>();
            overlayRect.SetParent(rowRect, false);
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            // Invisible but still a raycast target, so it's the topmost hit for the
            // whole row and drags reach it before the TMP_InputField underneath.
            Image overlayImage = overlayObj.GetComponent<Image>();
            overlayImage.color = Color.clear;

            SwipeableListItem swipe = overlayObj.GetComponent<SwipeableListItem>();
            swipe.RowBackground = rowBackground;
            swipe.ItemField = rowRect.GetComponentInChildren<TMP_InputField>();
            swipe.OnDeleteRequested = () => DeleteItem(index);
            swipe.OnMoveRequested = direction => MoveItem(index, direction);
            swipe.OnStateChanged = newState => { list.ItemStates[index] = newState; ListsStore.Save(); };
            swipe.ScrollTarget = scrollViewObj;
            swipe.ApplyInitialState(list.ItemStates[index]);
        }

        private void CreateTitleInputField(RectTransform rowRect, int index)
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
            placeholderText.text = "Enter item";
            placeholderText.fontSize = ItemFontSizeSettings.CurrentPointSize;
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
            titleText.fontSize = ItemFontSizeSettings.CurrentPointSize;
            titleText.color = Color.black;
            titleText.alignment = TextAlignmentOptions.MidlineLeft;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.targetGraphic = inputBackground;
            inputField.textViewport = textAreaRect;
            inputField.textComponent = titleText;
            inputField.placeholder = placeholderText;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
            inputField.characterLimit = 60;
            inputField.text = list.Items[index];
            inputField.onValueChanged.AddListener(value => { list.Items[index] = value; ListsStore.Save(); });
        }
    }
}
