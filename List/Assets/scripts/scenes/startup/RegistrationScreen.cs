// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

namespace Lists {

    // Sits between WelcomeScene and AuthScene, the first time the app runs on
    // a device (WelcomeScreen only routes here if ProfileStore.HasProfileName()
    // is false). Picks a random profile image, lets the user change it via
    // ProfileScene, then registers the chosen name + image + this device's
    // public key (DeviceKeyPair) with the server before continuing to AuthScene.
    public class RegistrationScreen : MonoBehaviour {

        public GameObject panel;

        private const int MinNameLength = 3;
        private const int MaxNameLength = 15;
        private static readonly Regex ValidNamePattern = new Regex("^[A-Za-z0-9!@#$%^&*]+$");

        private TMP_InputField nameField;
        private GameObject imagePanel;
        private Button registerButton;
        private TextMeshProUGUI statusText;

        public void Start()
        {
            AuditLog.Log("Registration screen");

            if (!ProfileStore.HasProfileImageBeenSet()) {
                int randomImageId = Random.Range(0, 2); // 0 or 1 inclusive
                ProfileStore.SetProfileImageType(randomImageId);
            }

            BuildUi();
            RefreshImage();
        }

        public void OnImageClicked()
        {
            SceneStack.Instance().PushScene();
            SceneManager.LoadScene("ProfileImageScene", LoadSceneMode.Single);
        }

        public void OnButtonClickRegister()
        {
            string name = nameField.text;

            if (!IsValidName(name, out string validationError)) {
                SetStatus(validationError);
                return;
            }

            registerButton.interactable = false;
            SetStatus("Checking availability...");
            StartCoroutine(ServerClient.GetUser(name, (requestSucceeded, publicKey, image) => OnGetUserResult(name, requestSucceeded, publicKey)));
        }

        private void OnGetUserResult(string name, bool requestSucceeded, string publicKey)
        {
            if (!requestSucceeded) {
                SetStatus("Could not reach the server - try again");
                registerButton.interactable = true;
                return;
            }

            if (!string.IsNullOrEmpty(publicKey)) {
                SetStatus("That name is already taken - try another");
                registerButton.interactable = true;
                return;
            }

            string devicePublicKey = DeviceKeyPair.GetOrCreatePublicKeyBase64();
            int imageId = ProfileStore.GetProfileImageType();

            SetStatus("Registering...");
            StartCoroutine(ServerClient.Register(name, devicePublicKey, imageId, success => OnRegisterResult(name, success)));
        }

        private void OnRegisterResult(string name, bool success)
        {
            if (!success) {
                SetStatus("Registration failed - try again");
                registerButton.interactable = true;
                return;
            }

            ProfileStore.SetProfileName(name);
            ProfileStore.ClearDraftProfileName();
            AuditLog.Log("Registered as " + name);

            SceneStack.Instance().Reset();
            SceneManager.LoadScene("AuthScene", LoadSceneMode.Single);
        }

        private bool IsValidName(string name, out string validationError)
        {
            if (string.IsNullOrEmpty(name) || name.Length < MinNameLength || name.Length > MaxNameLength) {
                validationError = $"User name must be {MinNameLength} to {MaxNameLength} characters";
                return false;
            }
            if (!ValidNamePattern.IsMatch(name)) {
                validationError = "Only letters, digits, and !@#$%^&* are allowed";
                return false;
            }
            validationError = null;
            return true;
        }

        private void RefreshImage()
        {
            ProfileImageSetter.SetMyProfileImage(imagePanel);
        }

        private void SetStatus(string message)
        {
            if (statusText != null) {
                statusText.text = message;
            }
        }

        private void BuildUi()
        {
            // The panel already has a static "TItle" label ("Create Profile") from
            // the AuthScene this scene was duplicated from - no need to build another.
            RectTransform panelRect = panel.GetComponent<RectTransform>();

            nameField = CreateNameField(panelRect);
            imagePanel = CreateImagePanel(panelRect);
            registerButton = CreateButton(panelRect, "RegisterButton", new Vector2(0, -260),
                new Vector2(500, 140), "Register", OnButtonClickRegister);

            GameObject statusObj = new GameObject("Status", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.SetParent(panelRect, false);
            statusRect.anchorMin = new Vector2(0.5f, 0.5f);
            statusRect.anchorMax = new Vector2(0.5f, 0.5f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0, -400);
            statusRect.sizeDelta = new Vector2(900, 100);
            statusText = statusObj.GetComponent<TextMeshProUGUI>();
            statusText.text = "Tap the picture to change it";
            statusText.fontSize = 40;
            statusText.color = Color.white;
            statusText.alignment = TextAlignmentOptions.Center;
        }

        private TMP_InputField CreateNameField(RectTransform panelRect)
        {
            GameObject inputObj = new GameObject("NameField", typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            // Disabled while wiring up textComponent/textViewport/placeholder/text
            // below, and re-enabled once they're all set - otherwise OnEnable runs
            // immediately (the object defaults to active) with none of them set
            // yet, and the initial `.text` assignment doesn't visually take until
            // the field is tapped and forced to rebuild.
            inputObj.SetActive(false);
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.SetParent(panelRect, false);
            inputRect.anchorMin = new Vector2(0.5f, 0.5f);
            inputRect.anchorMax = new Vector2(0.5f, 0.5f);
            inputRect.pivot = new Vector2(0.5f, 0.5f);
            inputRect.anchoredPosition = new Vector2(0, 250);
            inputRect.sizeDelta = new Vector2(700, 120);
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
            placeholderText.text = "Choose a user name";
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
            fieldText.fontSize = 50;
            fieldText.color = Color.black;
            fieldText.alignment = TextAlignmentOptions.Center;

            TMP_InputField inputField = inputObj.GetComponent<TMP_InputField>();
            inputField.targetGraphic = inputBackground;
            inputField.textViewport = textAreaRect;
            inputField.textComponent = fieldText;
            inputField.placeholder = placeholderText;
            inputField.lineType = TMP_InputField.LineType.SingleLine;
            inputField.characterLimit = MaxNameLength;

            // Restore whatever the user had typed before a trip to ProfileScene
            // (a full scene reload) would otherwise have lost it, and keep
            // persisting it as they type so it survives future round trips too.
            inputField.text = ProfileStore.GetDraftProfileName();
            inputField.onValueChanged.AddListener(value => ProfileStore.SetDraftProfileName(value));

            inputObj.SetActive(true);
            return inputField;
        }

        private GameObject CreateImagePanel(RectTransform panelRect)
        {
            GameObject imageObj = new GameObject("ProfileImage", typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform imageRect = imageObj.GetComponent<RectTransform>();
            imageRect.SetParent(panelRect, false);
            imageRect.anchorMin = new Vector2(0.5f, 0.5f);
            imageRect.anchorMax = new Vector2(0.5f, 0.5f);
            imageRect.pivot = new Vector2(0.5f, 0.5f);
            imageRect.anchoredPosition = new Vector2(0, 40);
            imageRect.sizeDelta = new Vector2(240, 240);

            Image image = imageObj.GetComponent<Image>();
            image.preserveAspect = true;

            Button button = imageObj.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(OnImageClicked);

            return imageObj;
        }

        private Button CreateButton(RectTransform panelRect, string name, Vector2 anchoredPosition, Vector2 size, string labelText, UnityEngine.Events.UnityAction onClick)
        {
            GameObject buttonObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.SetParent(panelRect, false);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = anchoredPosition;
            buttonRect.sizeDelta = size;
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

            return button;
        }
    }
}
