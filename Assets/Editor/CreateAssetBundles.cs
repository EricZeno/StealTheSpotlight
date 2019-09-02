using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class CreateAssetBundles {
    #region Building AssetBundles
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles() {
        string dir = Path.Combine(Application.dataPath, Constants.ASSETBUNDLES_DIRECTORY);
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
    #endregion

    #region Copying AssetBundles during Build
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
        string sourcePath = Path.Combine(Application.dataPath, Constants.ASSETBUNDLES_DIRECTORY);
        string destinationPath = Path.Combine(Directory.GetParent(pathToBuiltProject).FullName, Constants.RUNTIME_STREAMING_ASSETS_DIRECTORY, Constants.ASSETBUNDLES_DIRECTORY);

        Debug.Log($"Copying {sourcePath} to {destinationPath}");

        Directory.CreateDirectory(destinationPath);

        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
        }

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
            File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }
    }
    #endregion
}
