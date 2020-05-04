using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class CreateAssetBundles {
    #region Building AssetBundles
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles() {
        string dir = Path.Combine(Application.dataPath, Consts.ASSETBUNDLES_DIRECTORY);
        if (!Directory.Exists(dir)) {
            Directory.CreateDirectory(dir);
        }
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
    #endregion

    #region Copying AssetBundles during Build
    [PostProcessBuildAttribute(1)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
        string sourcePath = Path.Combine(Application.dataPath, Consts.ASSETBUNDLES_DIRECTORY);
        string destinationPath = "";

        if (target.ToString().Contains("Windows")) {
            destinationPath = Path.Combine(Directory.GetParent(pathToBuiltProject).FullName, Consts.RUNTIME_STREAMING_ASSETS_DIRECTORY_WINDOWS, Consts.ASSETBUNDLES_DIRECTORY);
        }
        else if (target.ToString().Contains("OSX")) {
            destinationPath = Path.Combine(pathToBuiltProject, Consts.RUNTIME_STREAMING_ASSETS_DIRECTORY_OSX, Consts.ASSETBUNDLES_DIRECTORY);
        }
        else {
            throw new System.NotSupportedException("This build target is not supported");
        }


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
