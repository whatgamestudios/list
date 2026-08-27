package com.whatgamestudios.lists.biometric;

import android.os.Bundle;
import androidx.annotation.NonNull;
import androidx.biometric.BiometricManager;
import androidx.biometric.BiometricPrompt;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.FragmentActivity;
import com.unity3d.player.UnityPlayer;

// Unity's default player Activity is not a FragmentActivity, which
// androidx.biometric.BiometricPrompt requires. This is a small, transparent,
// non-exported Activity (see AndroidManifest.xml) whose only job is to host a
// single BiometricPrompt call and report the result back into Unity via
// UnitySendMessage before finishing itself. See AndroidBiometricAuth.cs.
public class BiometricActivity extends FragmentActivity {

    public static final String EXTRA_REASON = "reason";
    public static final String EXTRA_CALLBACK_OBJECT = "callbackObject";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        String reason = getIntent().getStringExtra(EXTRA_REASON);
        final String callbackObject = getIntent().getStringExtra(EXTRA_CALLBACK_OBJECT);

        BiometricPrompt.PromptInfo promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .setTitle(reason == null ? "Authenticate" : reason)
                .setAllowedAuthenticators(BiometricManager.Authenticators.BIOMETRIC_STRONG)
                .setNegativeButtonText("Use PIN instead")
                .build();

        BiometricPrompt prompt = new BiometricPrompt(this, ContextCompat.getMainExecutor(this),
                new BiometricPrompt.AuthenticationCallback() {
                    @Override
                    public void onAuthenticationSucceeded(@NonNull BiometricPrompt.AuthenticationResult result) {
                        super.onAuthenticationSucceeded(result);
                        UnityPlayer.UnitySendMessage(callbackObject, "OnBiometricResult", "1");
                        finish();
                    }

                    @Override
                    public void onAuthenticationError(int errorCode, @NonNull CharSequence errString) {
                        super.onAuthenticationError(errorCode, errString);
                        UnityPlayer.UnitySendMessage(callbackObject, "OnBiometricResult", "0");
                        finish();
                    }

                    @Override
                    public void onAuthenticationFailed() {
                        super.onAuthenticationFailed();
                        // A single failed attempt (e.g. wrong finger) - let the
                        // prompt keep retrying. onAuthenticationError fires once it
                        // actually gives up (too many attempts, user cancels, ...).
                    }
                });

        prompt.authenticate(promptInfo);
    }
}
