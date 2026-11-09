using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tapestry.EditorTools
{
    /// <summary>First-open setup: creates and opens the main scene, adds it to the build, and sets player options.
    /// Also adds a "Tapestry" menu with the same actions.</summary>
    [InitializeOnLoad]
    public static class TapestrySetup
    {
        const string ScenePath = "Assets/Scenes/Main.unity";

        static TapestrySetup() { EditorApplication.delayCall += EnsureSetup; }

        static void EnsureSetup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!File.Exists(ScenePath)) CreateMainScene();
            else if (EditorBuildSettings.scenes.Length == 0) AddToBuild();
        }

        [MenuItem("Tapestry/Create Main Scene")]
        public static void CreateMainScene()
        {
            Directory.CreateDirectory("Assets/Scenes");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Tapestry Game").AddComponent<GameManager>();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AddToBuild();
            ApplyPlayerSettings();
            Debug.Log("Tapestry Adventures: created " + ScenePath + ". Press Play!");
        }

        static void AddToBuild()
        {
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        }

        [MenuItem("Tapestry/Apply Player Settings")]
        public static void ApplyPlayerSettings()
        {
            PlayerSettings.productName = "Tapestry Adventures";
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
        }

        [MenuItem("Tapestry/Reimport Art")]
        public static void ReimportArt()
        {
            AssetDatabase.ImportAsset("Assets/Resources/Art", ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
        }
    }

    /// <summary>Import settings for the embroidered art: keep full size, no mip-maps, clamp edges, high-quality compression.</summary>
    public class TapestryArtImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!assetPath.Replace('\\', '/').Contains("/Resources/Art/")) return;
            var ti = (TextureImporter)assetImporter;
            ti.textureType = TextureImporterType.Default;
            ti.alphaIsTransparency = true;
            ti.mipmapEnabled = false;
            ti.npotScale = TextureImporterNPOTScale.None;
            ti.wrapMode = TextureWrapMode.Clamp;
            ti.filterMode = FilterMode.Bilinear;
            ti.maxTextureSize = 4096;
            ti.textureCompression = TextureImporterCompression.CompressedHQ;
        }
    }
}
