// Copyright (c) Whatgame Studios 2024 - 2026
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Security.Cryptography;

namespace Lists {

    // Sits between WelcomeScene and MainScene. Local, offline unlock: a PIN is
    // always set up on first run and always works as a fallback; Face ID /
    // BiometricPrompt can additionally be enabled at setup time as a faster
    // shortcut on top of it. Whichever way the user unlocks, the device secret
    // (created on first run, loaded on every run after that) gets loaded into
    // SecretStore for the rest of the session.
    public class AuthScreen : MonoBehaviour {

        public GameObject panel;

        private const string BiometricEnabledPrefKey = "BIOMETRIC_UNLOCK_ENABLED";

        private ISecretVault vault;
        private IBiometricAuth biometricAuth;
        private bool isSetupMode;

        private TMP_InputField pinField;
        private TMP_InputField confirmPinField;
        private Toggle enableBiometricToggle;
        private Button primaryButton;
        private Button biometricButton;
        private TextMeshProUGUI statusText;

        public void Start()
        {
            AuditLog.Log("Auth screen");

            vault = SecretVaultFactory.Get();
            biometricAuth = BiometricAuthFactory.Attach(gameObject);
            isSetupMode = !vault.TryLoad(PinAuth.HashVaultKey, out _);

            BuildUi();

            if (!isSetupMode && BiometricUnlockEnabled() && biometricAuth != null && biometricAuth.IsAvailable()) {
                TryBiometricUnlock();
            }
        }

        public void OnButtonClickPrimary()
        {
            if (isSetupMode) {
                CreatePin();
            } else {
                UnlockWithPin();
            }
        }

        public void OnButtonClickBiometric()
        {
            TryBiometricUnlock();
        }

        private void CreatePin()
        {
            string pin = pinField.text;
            string confirmPin = confirmPinField.text;

            if (!PinAuth.IsValidFormat(pin)) {
                SetStatus($"PIN must be {PinAuth.PinLength} digits");
                return;
            }
            if (pin != confirmPin) {
                SetStatus("PINs don't match");
                return;
            }

            PinAuth.ComputeSaltAndHash(pin, out byte[] salt, out byte[] hash);
            vault.Save(PinAuth.SaltVaultKey, salt);
            vault.Save(PinAuth.HashVaultKey, hash);

            bool enableBiometric = enableBiometricToggle != null && enableBiometricToggle.isOn;
            PlayerPrefs.SetInt(BiometricEnabledPrefKey, enableBiometric ? 1 : 0);
            PlayerPrefs.Save();

            CreateAndStoreNewSecret();
            AuditLog.Log("PIN created" + (enableBiometric ? " with biometric unlock enabled" : ""));
            GoToMainScene();
        }

        private void UnlockWithPin()
        {
            string pin = pinField.text;

            if (!vault.TryLoad(PinAuth.SaltVaultKey, out byte[] salt) ||
                !vault.TryLoad(PinAuth.HashVaultKey, out byte[] hash) ||
                !PinAuth.Verify(pin, salt, hash)) {
                SetStatus("Incorrect PIN");
                pinField.text = "";
                return;
            }

            AuditLog.Log("Unlocked with PIN");
            LoadOrCreateSecret();
            GoToMainScene();
        }

        private void TryBiometricUnlock()
        {
            if (biometricAuth == null || !biometricAuth.IsAvailable()) {
                return;
            }

            SetStatus("Checking Face ID...");
            biometricAuth.Authenticate("Unlock Lots of Lists", success => {
                if (success) {
                    AuditLog.Log("Unlocked with biometrics");
                    LoadOrCreateSecret();
                    GoToMainScene();
                } else {
                    SetStatus("Enter your PIN instead");
                }
            });
        }

        private bool BiometricUnlockEnabled()
        {
            return PlayerPrefs.GetInt(BiometricEnabledPrefKey, 0) == 1;
        }

        private void GoToMainScene()
        {
            SceneStack.Instance().Reset();
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        }

        private void LoadOrCreateSecret()
        {
            if (vault.TryLoad(SecretStore.VaultKey, out byte[] existingSecret)) {
                SecretStore.SetSecret(existingSecret);
                return;
            }

            // A PIN/biometric exists but the device secret doesn't - shouldn't
            // normally happen, but recover rather than leave SecretStore empty.
            CreateAndStoreNewSecret();
        }

        private void CreateAndStoreNewSecret()
        {
            byte[] newSecret = new byte[SecretStore.SecretLengthBytes];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetBytes(newSecret);
            }
            vault.Save(SecretStore.VaultKey, newSecret);
            SecretStore.SetSecret(newSecret);
            AuditLog.Log("Generated and stored new device secret");
        }

        private void SetStatus(string message)
        {
            if (statusText != null) {
                statusText.text = message;
            }
        }

        private void BuildUi()
        {
            // The panel already has a static "TItle" label ("Lots-a-Lists") from the
            // WelcomeScene this scene was duplicated from - no need to build another.
            RectTransform panelRect = panel.GetComponent<RectTransform>();

            if (isSetupMode) {
                BuildSetupUi(panelRect);
            } else {
                BuildUnlockUi(panelRect);
            }

            GameObject statusObj = new GameObject("Status", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform statusRect = statusObj.GetComponent<RectTransform>();
            statusRect.SetParent(panelRect, false);
            statusRect.anchorMin = new Vector2(0.5f, 0.5f);
            statusRect.anchorMax = new Vector2(0.5f, 0.5f);
            statusRect.pivot = new Vector2(0.5f, 1f);
            statusRect.anchoredPosition = new Vector2(0, -400);
            statusRect.sizeDelta = new Vector2(900, 100);
            statusText = statusObj.GetComponent<TextMeshProUGUI>();
            statusText.text = isSetupMode ? "Choose a PIN to protect this app" : "";
            statusText.fontSize = 40;
            statusText.color = Color.white;
            statusText.alignment = TextAlignmentOptions.Center;
        }

        private void BuildSetupUi(RectTransform panelRect)
        {
            pinField = CreatePinField(panelRect, "PinField", new Vector2(0, 150), "New PIN");
            confirmPinField = CreatePinField(panelRect, "ConfirmPinField", new Vector2(0, 0), "Confirm PIN");

            bool biometricAvailable = biometricAuth != null && biometricAuth.IsAvailable();
            float primaryButtonY = -150;

            if (biometricAvailable) {
                enableBiometricToggle = CreateBiometricToggle(panelRect, new Vector2(0, -150));
                primaryButtonY = -280;
            }

            primaryButton = CreateButton(panelRect, "CreatePinButton", new Vector2(0, primaryButtonY),
                new Vector2(500, 140), "Create PIN", OnButtonClickPrimary);
        }

        private void BuildUnlockUi(RectTransform panelRect)
        {
            pinField = CreatePinField(panelRect, "PinField", new Vector2(0, 100), "Enter PIN");

            primaryButton = CreateButton(panelRect, "UnlockButton", new Vector2(0, -80),
                new Vector2(500, 140), "Unlock", OnButtonClickPrimary);

            if (biometricAuth != null && biometricAuth.IsAvailable() && BiometricUnlockEnabled()) {
                biometricButton = CreateButton(panelRect, "BiometricButton", new Vector2(0, -250),
                    new Vector2(500, 140), "Use Face ID", OnButtonClickBiometric);
            }
        }

        private Toggle CreateBiometricToggle(RectTransform panelRect, Vector2 anchoredPosition)
        {
            GameObject toggleObj = new GameObject("EnableBiometricToggle", typeof(RectTransform), typeof(Toggle));
            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.SetParent(panelRect, false);
            toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
            toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
            toggleRect.pivot = new Vector2(0.5f, 0.5f);
            toggleRect.anchoredPosition = anchoredPosition;
            toggleRect.sizeDelta = new Vector2(600, 80);

            GameObject checkAreaObj = new GameObject("CheckArea", typeof(RectTransform), typeof(Image));
            RectTransform checkAreaRect = checkAreaObj.GetComponent<RectTransform>();
            checkAreaRect.SetParent(toggleRect, false);
            checkAreaRect.anchorMin = new Vector2(0, 0.5f);
            checkAreaRect.anchorMax = new Vector2(0, 0.5f);
            checkAreaRect.pivot = new Vector2(0, 0.5f);
            checkAreaRect.anchoredPosition = Vector2.zero;
            checkAreaRect.sizeDelta = new Vector2(70, 70);
            Image checkAreaBackground = checkAreaObj.GetComponent<Image>();
            checkAreaBackground.color = new Color(0.95f, 0.95f, 0.95f, 1f);

            GameObject checkmarkObj = new GameObject("Checkmark", typeof(RectTransform), typeof(Image));
            RectTransform checkmarkRect = checkmarkObj.GetComponent<RectTransform>();
            checkmarkRect.SetParent(checkAreaRect, false);
            checkmarkRect.anchorMin = new Vector2(0.15f, 0.15f);
            checkmarkRect.anchorMax = new Vector2(0.85f, 0.85f);
            checkmarkRect.offsetMin = Vector2.zero;
            checkmarkRect.offsetMax = Vector2.zero;
            Image checkmarkImage = checkmarkObj.GetComponent<Image>();
            checkmarkImage.color = new Color(0.2f, 0.45f, 0.85f, 1f);

            GameObject labelObj = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.SetParent(toggleRect, false);
            labelRect.anchorMin = new Vector2(0, 0);
            labelRect.anchorMax = new Vector2(1, 1);
            labelRect.offsetMin = new Vector2(90, 0);
            labelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI label = labelObj.GetComponent<TextMeshProUGUI>();
            label.text = "Also unlock with Face ID";
            label.fontSize = 40;
            label.color = Color.white;
            label.alignment = TextAlignmentOptions.MidlineLeft;

            Toggle toggle = toggleObj.GetComponent<Toggle>();
            toggle.targetGraphic = checkAreaBackground;
            toggle.graphic = checkmarkImage;
            toggle.isOn = false;

            return toggle;
        }

        private TMP_InputField CreatePinField(RectTransform panelRect, string name, Vector2 anchoredPosition, string placeholder)
        {
            GameObject inputObj = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(TMP_InputField));
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.SetParent(panelRect, false);
            inputRect.anchorMin = new Vector2(0.5f, 0.5f);
            inputRect.anchorMax = new Vector2(0.5f, 0.5f);
            inputRect.pivot = new Vector2(0.5f, 0.5f);
            inputRect.anchoredPosition = anchoredPosition;
            inputRect.sizeDelta = new Vector2(500, 120);
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
