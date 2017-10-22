using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Required override to get around Unity's default texture importer blocking any manipulation from scripts.
/// </summary>
public class TexturePostProcessor : AssetPostprocessor {
#if UNITY_EDITOR
    //void OnPreprocessTexture()
    //{
    //    //if (assetPath.Contains(UWBNetworkingPackage.Config.AssetBundle.Current.CompileUnityAssetDirectory()))
    //    if(assetPath.Contains(UWBNetworkingPackage.Config.Current.AssetBundle.CompileUnityAssetDirectory()))
    //    {
    //        TextureImporter importer = assetImporter as TextureImporter;
    //        importer.textureType = TextureImporterType.Default;
    //        importer.textureCompression = TextureImporterCompression.Uncompressed;
    //        importer.isReadable = true;
    //        importer.filterMode = FilterMode.Point;
    //        importer.npotScale = TextureImporterNPOTScale.None;

    //        Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
    //        if (asset)
    //        {
    //            EditorUtility.SetDirty(asset);
    //        }
    //        else
    //        {
    //            importer.textureType = TextureImporterType.Default;
    //        }
    //    }
    //}

    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.isReadable = true;
        //textureImporter.textureFormat = TextureImporterFormat.ARGB32;
        TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings();
        textureImporter.GetPlatformTextureSettings("Standalone").CopyTo(platformSettings);
        platformSettings.overridden = true;
        platformSettings.format = TextureImporterFormat.RGBA32;
        textureImporter.SetPlatformTextureSettings(platformSettings);
    }
#endif
}
