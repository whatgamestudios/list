// Copyright (c) Whatgame Studios 2024 - 2026
#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace Lists {

    // Apple requires NSFaceIDUsageDescription in Info.plist for any app that calls
    // LocalAuthentication's biometric APIs (see BiometricBridge.swift) - without it
    // the app crashes the instant Face ID is invoked. Unity generates Info.plist
    // fresh on every iOS build, so this has to be injected as a post-process step
    // rather than edited as a source file.
    public static class IosFaceIdInfoPlistPostProcessor {
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS) {
                return;
            }

            string plistPath = pathToBuiltProject + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            plist.root.SetString("NSFaceIDUsageDescription", "Used to unlock the app instead of entering your PIN.");

            plist.WriteToFile(plistPath);
        }
    }
}
#endif
