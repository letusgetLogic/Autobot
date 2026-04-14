using UnityEditor;
using UnityEngine;

public class RenameScriptableObject
{
    public static void RenameAsset(ScriptableObject so, string newName)
    {
        string assetPath = AssetDatabase.GetAssetPath(so.GetInstanceID());
        Debug.Log(assetPath);
        // Ensure the new name has no extension if you only want the base name
        string newPath =  newName + ".asset";

        // Rename the asset
        string error = AssetDatabase.RenameAsset(assetPath, newPath);
        if (!string.IsNullOrEmpty(error))
        {
            Debug.LogError("Error renaming asset: " + error);
        }
        
    }
}

