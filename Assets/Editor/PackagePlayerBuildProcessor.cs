// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/28/14:31
// Ver : 1.0.0
// Description : PackagePlayerBuildProcessor.cs
// ChangeLog :
// **********************************************

using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PackagePlayerBuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
{
    public static string RemoteBuildPath { get; set; }
    
    public static string PackageBuildInAssetBundle
    {
        get { return Application.streamingAssetsPath + "/" + ContentUpdater.InPackageAssetBundleFolderName; }
    }

    /// <summary>
    /// Returns the player build processor callback order.
    /// </summary>
    public int callbackOrder
    {
        get { return 2; }
    }

    /// <summary>
    /// Restores temporary data created as part of a build.
    /// </summary>
    /// <param name="report">Stores temporary player build data.</param>
    public void OnPostprocessBuild(BuildReport report)
    {
        CleanTemporaryPlayerBuildData();
    }

    [InitializeOnLoadMethod]
    internal static void CleanTemporaryPlayerBuildData()
    {
        if (Directory.Exists(PackageBuildInAssetBundle))
        {
            Directory.Delete(PackageBuildInAssetBundle, true);
            File.Delete(PackageBuildInAssetBundle + ".meta");
        }
        
        string inPackageBundleAssetInfo = Path.Combine(Application.dataPath, "Resources/PackageAssetBundlesInfo.txt");
    
        if (File.Exists(inPackageBundleAssetInfo))
        {
            File.Delete(inPackageBundleAssetInfo);
            File.Delete(inPackageBundleAssetInfo + ".meta");
        }
    }

    ///<summary>
    /// Initializes temporary build data.
    /// </summary>
    /// <param name="report">Contains build data information.</param>
    public void OnPreprocessBuild(BuildReport report)
    {
        CopyTemporaryPlayerBuildData();
    }

    internal static void CopyTemporaryPlayerBuildData()
    {
        string inPackageBundleSourcePath = Path.GetDirectoryName(Application.dataPath);
        
        if (!string.IsNullOrEmpty(inPackageBundleSourcePath))
        {
            string sourceFolder = Path.Combine(inPackageBundleSourcePath, RemoteBuildPath);
          
            if (Directory.Exists(sourceFolder))
            {
                if (Directory.Exists(PackageBuildInAssetBundle))
                {
                    Debug.LogWarning(
                        $"Found and deleting directory \"{PackageBuildInAssetBundle}\", directory is managed By App.");

                    Directory.Delete(PackageBuildInAssetBundle, true);
                }

                string parentDir = Path.GetDirectoryName(Addressables.BuildPath);

                if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
                    Directory.CreateDirectory(parentDir);


                if (!Directory.Exists(PackageBuildInAssetBundle))
                {
                    Directory.CreateDirectory(PackageBuildInAssetBundle);
                }
 
                string[] bundleFiles = Directory.GetFiles(sourceFolder, "in_package_*.bundle");

                string bundleFileInfo = "";
                for (var i = 0; i < bundleFiles.Length; i++)
                {
                    var targetFileName = Path.GetFileName(bundleFiles[i]);
                    bundleFileInfo += targetFileName + ";";
                    File.Copy(bundleFiles[i], Path.Combine(PackageBuildInAssetBundle, targetFileName));
                }

                string inPackageBundleAssetInfo = Path.Combine(Application.dataPath, "Resources/PackageAssetBundlesInfo.txt");
                
                File.WriteAllText(inPackageBundleAssetInfo, bundleFileInfo);
            }
        }
    }
}