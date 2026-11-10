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

        [MenuItem("Tapestry/Android/Build and Run on Connected Phone")]
        public static void BuildAndRun() { Build(true); }

        static void Build(bool run)
        {
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
