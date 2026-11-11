using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Tapestry.EditorTools
{
    /// <summary>Android settings and one-click test builds (Tapestry menu).</summary>
    public static class AndroidBuild
    {
        public const string AppName = "Ye Tapestrie Adventures";
        public const string Company = "pezacide";
        public const string PackageId = "com.pezacide.tapestryadventures";   // permanent once published
        public const string Version = "0.1.0";                                // shown to players
        public const int VersionCode = 1;                                     // bump for every Play upload
        const string ScenePath = "Assets/Scenes/Main.unity";
        const string IconPath = "Assets/Icons/icon.png";
        const string ApkPath = "Builds/Android/YeTapestrieAdventures.apk";
        const string AabPath = "Builds/Android/YeTapestrieAdventures.aab";

        [MenuItem("Tapestry/Android/Apply Android Settings")]
        public static void ApplySettings()
        {
            PlayerSettings.productName = AppName;
            PlayerSettings.companyName = Company;
            PlayerSettings.bundleVersion = Version;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, PackageId);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);   // required for 64-bit
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;                          // Google Play requires 64-bit
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;                     // Android 8.0 and newer
            PlayerSettings.Android.bundleVersionCode = VersionCode;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;

            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(IconPath);
            if (icon != null) PlayerSettings.SetIcons(NamedBuildTarget.Unknown, new[] { icon }, IconKind.Any);
            else Debug.LogWarning("Tapestry: icon not found at " + IconPath);
            Debug.Log("Tapestry: Android settings applied (" + PackageId + ", v" + Version + ").");
        }

        [MenuItem("Tapestry/Android/Build Test APK")]
        public static void BuildApk() { Build(false); }

        /// <summary>The Play Store takes an Android App Bundle (.aab), not an .apk. Before the first upload, make a signing key:
        /// Project Settings > Player > Android > Publishing Settings > Keystore Manager (and keep it safe, with its passwords).
        /// If Play App Signing is on (the default for new apps), Google holds the final key and this one is the upload key.</summary>
        [MenuItem("Tapestry/Android/Build Play Store Bundle (.aab)")]
        public static void BuildAab()
        {
            if (!EnsureSingleInputSystem()) return;
            ApplySettings();
            if (!PlayerSettings.Android.useCustomKeystore || string.IsNullOrEmpty(PlayerSettings.Android.keystoreName))
            {
                EditorUtility.DisplayDialog("Ye Tapestrie Adventures",
                    "No signing key is set.\n\nOpen Project Settings > Player > Android > Publishing Settings, tick \"Custom Keystore\", " +
                    "and use the Keystore Manager to create one. Keep the keystore file and its passwords safe: losing them means you cannot update the game.", "Very well");
                return;
            }
            if (!File.Exists(ScenePath)) TapestrySetup.CreateMainScene();
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.buildAppBundle = true;
            Directory.CreateDirectory(Path.GetDirectoryName(AabPath));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { ScenePath }, locationPathName = AabPath, target = BuildTarget.Android, options = BuildOptions.None });
            EditorUserBuildSettings.buildAppBundle = false;
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Tapestry: built " + AabPath + " (" + (report.summary.totalSize / (1024 * 1024)) + " MB). Upload it in the Play Console. Bump VersionCode for every upload.");
                EditorUtility.RevealInFinder(AabPath);
            }
            else Debug.LogError("Tapestry: bundle build failed - see the Console above for the reason.");
        }

        [MenuItem("Tapestry/Store/Forget the Purchase (to test the paywall)")]
        public static void ForgetPurchase()
        {
            PlayerPrefs.DeleteKey(Tapestry.Store.OwnedKey); PlayerPrefs.Save();
            Debug.Log("Tapestry: the unlock is forgotten; stop and press Play again to see Worlds 2-7 locked.");
        }

        [MenuItem("Tapestry/Android/Build and Run on Connected Phone")]
        public static void BuildAndRun() { Build(true); }

        /// <summary>Android needs exactly one input system. The game reads the new Input System (and the old one
        /// only if it is the one switched on), so "Both" is switched to "Input System Package (New)".
        /// Returns false if Unity must restart before building.</summary>
        static bool EnsureSingleInputSystem()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets == null || assets.Length == 0) return true;
            var so = new SerializedObject(assets[0]);
            var prop = so.FindProperty("activeInputHandler");   // 0 = old, 1 = new, 2 = both
            if (prop == null || prop.intValue != 2) return true;
            prop.intValue = 1;
            so.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            EditorUtility.DisplayDialog("Ye Tapestrie Adventures",
                "Active Input Handling was set to \"Both\", which Android does not support.\n\n" +
                "It is now \"Input System Package (New)\". Please close and reopen Unity, then build again.", "Very well");
            return false;
        }

        static void Build(bool run)
        {
            if (!EnsureSingleInputSystem()) return;
            ApplySettings();
            if (!File.Exists(ScenePath)) TapestrySetup.CreateMainScene();
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            EditorUserBuildSettings.buildAppBundle = false;   // an .apk for testing; the Play Store wants an .aab later
            Directory.CreateDirectory(Path.GetDirectoryName(ApkPath));
            var opts = new BuildPlayerOptions
            {
                scenes = new[] { ScenePath },
                locationPathName = ApkPath,
                target = BuildTarget.Android,
                options = run ? BuildOptions.AutoRunPlayer : BuildOptions.None,
            };
            var report = BuildPipeline.BuildPlayer(opts);
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Tapestry: built " + ApkPath + " (" + (report.summary.totalSize / (1024 * 1024)) + " MB).");
                if (!run) EditorUtility.RevealInFinder(ApkPath);
            }
            else Debug.LogError("Tapestry: Android build failed - see the Console above for the reason.");
        }
    }
}
