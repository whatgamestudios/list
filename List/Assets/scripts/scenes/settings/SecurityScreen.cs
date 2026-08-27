// Copyright (c) Whatgame Studios 2024 - 2025
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace Lists {

    public class SecurityScreen : MonoBehaviour {

        public GameObject panel;

        private static readonly (string Label, float Seconds)[] TimeoutOptions = {
            ("5 seconds", 5f),
            ("15 seconds", 15f),
            ("30 seconds", 30f),
            ("1 minute", 60f),
            ("5 minutes", 300f),
            ("30 minutes", 1800f),
            ("1 hour", 3600f),
            ("1 day", 86400f),
            ("Never", AppLockMonitor.NeverTimeoutValue),
        };

        private RectTransform content;
        private TMP_InputField newPinField;
        private TMP_InputField confirmPinField;
        private TextMeshProUGUI pinStatusText;
        private readonly List<(Image Background, float Seconds)> timeoutRows = new List<(Image, float)>();

        public void Start()
        {
            AuditLog.Log("Security screen");
            BuildUi();
        }

        public void OnButtonClickSavePin()
        {
            string newPin = newPinField.text;
            string confirmPin = confirmPinField.text;

            if (!PinAuth.IsValidFormat(newPin)) {
                SetPinStatus($"PIN must be {PinAuth.PinLength} digits");
                return;
            }
            if (newPin != confirmPin) {
                SetPinStatus("PINs don't match");
                return;
            }

            ISecretVault vault = SecretVaultFactory.Get();
            PinAuth.ComputeSaltAndHash(newPin, out byte[] salt, out byte[] hash);
            vault.Save(PinAuth.SaltVaultKey, salt);
            vault.Save(PinAuth.HashVaultKey, hash);

            newPinField.text = "";
            confirmPinField.text = "";
            SetPinStatus("PIN updated");
            AuditLog.Log("PIN reset from Security screen");
        }

        private void SelectTimeout(float seconds)
        {
            AppLockMonitor.ReauthTimeoutSeconds = seconds;
            RefreshTimeoutSelection();
            AuditLog.Log($"Re-authentication timeout set to {seconds}s");
        }

        private void RefreshTimeoutSelection()
        {
            float current = AppLockMonitor.ReauthTimeoutSeconds;
            foreach (var row in timeoutRows) {
                bool selected = Mathf.Approximately(row.Seconds, current);
                row.Background.color = selected
                    ? new Color(0.2f, 0.45f, 0.85f, 1f)
                    : new Color(1f, 1f, 1f, 0.06f);
            }
        }

        private void SetPinStatus(string message)
        {
            if (pinStatusText != null) {
                pinStatusText.text = message;
            }
        }

        private void BuildUi()
        {
            RectTransform panelRect = panel.GetComponent<RectTransform>();

            GameObject scrollObj = new GameObject("SecurityScrollView", typeof(RectTransform));
            scrollObj.SetActive(false);
            RectTransform scrollRect = scrollObj.GetComponent<RectTransform>();
            scrollRect.SetParent(panelRect, false);
            // Horizontally: a fixed 900px width centered on the panel (point anchor
            // at x=0.5 rather than a stretch). Vertically: unchanged full-height
            // stretch with a 25px inset top and bottom.
            scrollRect.anchorMin = new Vector2(0.5f, 0f);
            scrollRect.anchorMax = new Vector2(0.5f, 1f);
            scrollRect.offsetMin = new Vector2(-450, 25);
            scrollRect.offsetMax = new Vector2(450, -25);

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

            CreateSectionHeader("Change PIN");
            newPinField = CreatePinField("NewPinField", "New PIN");
            confirmPinField = CreatePinField("ConfirmPinField", "Confirm PIN");
            CreateButton("SavePinButton", "Save PIN", OnButtonClickSavePin);
            pinStatusText = CreateStatusText();

            CreateSectionHeader("Re-authentication Timeout");
            foreach (var option in TimeoutOptions) {
                CreateTimeoutRow(option.Label, option.Seconds);
            }
            RefreshTimeoutSelection();

            scrollObj.SetActive(true);
        }

        private void CreateSectionHeader(string text)
        {
            GameObject headerObj = new GameObject("SectionHeader", typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
            RectTransform headerRect = headerObj.GetComponent<RectTransform>();
            headerRect.SetParent(content, false);
            LayoutElement headerLayout = headerObj.GetComponent<LayoutElement>();
            headerLayout.preferredHeight = 100f;
            TextMeshProUGUI header = headerObj.GetComponent<TextMeshProUGUI>();
            header.text = text;
            header.fontSize = 55;
            header.fontStyle = FontStyles.Bold;
            header.color = Color.white;
            header.alignment = TextAlignmentOptions.MidlineLeft;
        }

        private TextMeshProUGUI CreateStatusText()
        {
            GameObject statusObj = new GameObject("Status", typeof(RectTransform), typeof(LayoutElement), typeof(TextMeshProUGUI));
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.SetParent(content, false);
            LayoutElement statusLayout = statusObj.GetComponent<LayoutElement>();
            statusLayout.preferredHeight = 70f;
            TextMeshProUGUI status = statusObj.GetComponent<TextMeshProUGUI>();
            status.text = "";
            status.fontSize = 40;
            status.color = Color.white;
            status.alignment = TextAlignmentOptions.Center;
            return status;
        }

        private TMP_InputField CreatePinField(string name, string placeholder)
        {
            GameObject inputObj = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(TMP_InputField));
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.SetParent(content, false);
            LayoutElement inputLayout = inputObj.GetComponent<LayoutElement>();
            inputLayout.preferredHeight = 120f;
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
            placeholderText.text = placeholder;
            placeholderText.fontSize = 45;
            placeholderText.fontStyle = FontStyles.Italic;
            placeholderText.color = new Color(0f, 0f, 0f, 0.5f);
            placeholderText.alignment = TextAlignmentOptions.Center;

            GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.SetParent(textAreaRect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            TextMeshProUGUI fieldText = textObj.GetComponent<TextMeshProUGUI>();
            fieldText.fontSize = 55;
            fieldText.color = Color.black;
            fieldText.alignment = TextAlignmentOptions.Center;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.targetGraphic = inputBackground;
            inputField.textViewport = textAreaRect;
            inputField.textComponent = fieldText;
            inputField.placeholder = placeholderText;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
            inputField.contentType = TMP_InputField.ContentType.Pin;
            inputField.characterLimit = PinAuth.PinLength;

            return inputField;
        }

        private void CreateButton(string name, string labelText, UnityEngine.Events.UnityAction onClick)
        {
            GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.SetParent(content, false);
            LayoutElement buttonLayout = buttonObj.GetComponent<LayoutElement>();
            buttonLayout.preferredHeight = 140f;
            Image buttonBackground = buttonObj.GetComponent<Image>();
            buttonBackground.color = new Color(0.2f, 0.45f, 0.85f, 1f);
            Button button = buttonObj.GetComponent<Button>();
            button.targetGraphic = buttonBackground;
            button.onClick.AddListener(onClick);

            GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(buttonRect, false);
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = labelText;
            label.fontSize = 50;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.Center;
        }

        private void CreateTimeoutRow(string labelText, float seconds)
        {
            GameObject row = new GameObject("TimeoutOption_" + labelText, typeof(RectTransform), typeof(LayoutElement), typeof(Image), typeof(Button));
            RectTransform rowRect = row.GetComponent<RectTransform>();
            rowRect.SetParent(content, false);
            LayoutElement rowLayout = row.GetComponent<LayoutElement>();
            rowLayout.preferredHeight = 110f;

            Image rowBackground = row.GetComponent<Image>();
            rowBackground.color = new Color(1f, 1f, 1f, 0.06f);

            Button rowButton = row.GetComponent<Button>();
            rowButton.targetGraphic = rowBackground;
            rowButton.onClick.AddListener(() => SelectTimeout(seconds));

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

            timeoutRows.Add((rowBackground, seconds));
        }
    }
}
