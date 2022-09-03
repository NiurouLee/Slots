using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AssetsSyncTools : MonoBehaviour
{
    /// <summary>
    /// 代码同步
    /// </summary>
    [MenuItem("ILRuntime/拷贝DLL到资源仓库")]
    static void CopyGameModuleCodeToResourceRepository()
    {
        string[] files =  
        {
            UnixPath2WSLPath(Path.Combine(Directory.GetCurrentDirectory(), "Library/ScriptAssemblies/MainCodeAssembly.dll")),
            UnixPath2WSLPath(Path.Combine(Directory.GetCurrentDirectory(), "Library/ScriptAssemblies/MainCodeAssembly.pdb")),
            UnixPath2WSLPath(Path.Combine(Directory.GetCurrentDirectory(), "Library/ScriptAssemblies/GameModuleAssembly.dll")),
            UnixPath2WSLPath(Path.Combine(Directory.GetCurrentDirectory(), "Library/ScriptAssemblies/GameModuleAssembly.pdb")),
        };

        StringBuilder targetFolder = new StringBuilder(UnixPath2WSLPath(Path.Combine(Directory.GetCurrentDirectory(), "Assets/BuildAssembly/")));
       
        string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath));
        UnixPath2WSLPath(workDirectory);

        targetFolder.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");
        for (var i = 0; i < files.Length; i++)
        {
            string command = "rsync -av --delete " + files[i] + " " + targetFolder;
            command = UnitCommand2WSLCommand(command);
            ShellHelper.ProcessCommand(command, "");
        }

        UnityEngine.Debug.Log(targetFolder);
        //拷贝DLL的同时拷贝Addressable配置,保证代码和Addressable配置一致
        var sourceAddressableFolder = Directory.GetCurrentDirectory() + "/Assets/AddressableAssetsData/";
        sourceAddressableFolder = UnixPath2WSLPath(sourceAddressableFolder);
        var destAddressableFolder = sourceAddressableFolder.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");


        string commandAdd = "rsync -av --delete " + sourceAddressableFolder + " " + destAddressableFolder;
        commandAdd = UnitCommand2WSLCommand(commandAdd);
        ShellHelper.ProcessCommand(commandAdd, "");
        // EditorUtility.DisplayDialog("Done", "......", "Ok");
    }

    /// <summary>
    /// 资源拷贝 CasinoResource工程 -> Casino工程
    /// </summary>
    [MenuItem("Assets/从资源仓库复制 ^n")]
    static void CopyResourceFromResourceRepository()
    {
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string clickedPath = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        string clickedPathFull = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);


        //确认弹框, 以免误点
        bool isOk = EditorUtility.DisplayDialog("Attention Please",
            $"是否确定从资源仓库同步:{clickedPath}", "Ok", "Cancel");
        if (!isOk)
        {
            return;
        }


        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);
        string parentDirectory = Path.GetDirectoryName(clickedPathFull);

        if (isDirectory)
        {
            
#if UNITY_EDITOR_WIN
            clickedPathFull = UnixPath2WSLPath(clickedPathFull);
#endif
            
            StringBuilder sourceFolder = new StringBuilder(clickedPathFull);

            string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath));
            sourceFolder.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");

            if (!Directory.Exists(sourceFolder.ToString()))
            {
                EditorUtility.DisplayDialog("Attention Please",
                    $"资源目录不存在:{sourceFolder}", "Ok", "Cancel");
                return;
            }

            string shell = $"rsync -av --delete {sourceFolder}/ {clickedPathFull}/";
 
#if UNITY_EDITOR_WIN
            shell = UnitCommand2WSLCommand(shell);
#endif
            
            Debug.Log($"shell :{shell}");
            ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(shell, "");


            EditorUtility.DisplayProgressBar("", "Wait", 0);
         
            req.onDone += () =>
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Done", "......", "Ok");
                AssetDatabase.Refresh();
            };

        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Only Work for Directory", "Ok");
        }
    }

    [MenuItem("Assets/从资源仓库同步文件 ^k")]
    static void CopyResourceFileFromResourceRepository()
    {
        string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath)); 
         
        int selectCount = Selection.assetGUIDs.Length;
        int copyCount = 0;
        
        //确认弹框, 以免误点
        bool isOk = EditorUtility.DisplayDialog("Attention Please",
            string.Format("You will copy {0} files to target Folder.\nWill replace them directly.\n Tap Ok to Continue", selectCount), "Ok", "Cancel");
        if (!isOk)
        {
            return;
        }
 
        for (int i = 0; i < selectCount; i++)
        {
            string clickedAssetGuid = Selection.assetGUIDs[i];
            string clickedFilePath      = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
            string clickedFilePathFull  = Path.Combine(Directory.GetCurrentDirectory(), clickedFilePath);
       
            FileAttributes attr = File.GetAttributes(clickedFilePathFull);
            bool isDirectory = attr.HasFlag(FileAttributes.Directory);
            string parentDirectory = Path.GetDirectoryName(clickedFilePathFull);
        
            //只能拷贝单一文件
            if (!isDirectory)
            {
                StringBuilder targetFile = new StringBuilder(clickedFilePathFull);
#if UNITY_EDITOR_WIN
                //targetFile.Replace("UCasino", "UCasinoResource");
                targetFile.Replace($@"\{workDirectory}\Assets", $@"\{workDirectory}Res\Assets");
                if (System.IO.File.Exists(targetFile.ToString()))
                {
                    System.IO.File.Copy(targetFile.ToString(), clickedFilePathFull, true);
                }
                else
                {
                    System.IO.File.Delete(clickedFilePathFull);
                }
#else
                targetFile.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");
                string shellCmd;
                if (!File.Exists(targetFile.ToString()))
                {
                    //强制删除
                    shellCmd = "rm -f " + clickedFilePathFull;
                }
                else
                {
                    //强制复制
                    shellCmd = "cp -f " + targetFile + " " + clickedFilePathFull;
                }
                UnityEngine.Debug.Log("shellCmd: " + shellCmd);
                ShellHelper.ShellRequest req =
                    ShellHelper.ProcessCommand(shellCmd, "");
#endif
                copyCount++;
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Only Work for Files", "Ok");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", string.Format("{0} files have been copy to target folder", copyCount),
            "Ok");
    }

    public static string UnixPath2WSLPath(string path)
    {
        return WindowsTools.UnixPath2WSLPath(path);
    }

    public static string UnitCommand2WSLCommand(string command)
    {
        return WindowsTools.UnitCommand2WSLCommand(command);
    }


    [MenuItem("Assets/同步目录修改到资源仓库 ^m")]
    static void SyncResourceToResourceRepository()
    {
        string clickedAssetGuid = Selection.assetGUIDs[0];
        string clickedPath      = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
        string clickedPathFull  = Path.Combine(Directory.GetCurrentDirectory(), clickedPath);
      
        FileAttributes attr = File.GetAttributes(clickedPathFull);
        bool isDirectory = attr.HasFlag(FileAttributes.Directory);
        string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath)); 
        
        //确认弹框, 以免误点
        bool isOk = EditorUtility.DisplayDialog("Attention Please",
            $"是否同步目录中的资源{clickedPath}到资源仓库", "Ok", "Cancel");
        if (!isOk)
        {
            return;
        }
        
        if (isDirectory)
        {
            
#if UNITY_EDITOR_WIN
            clickedPathFull = UnixPath2WSLPath(clickedPathFull);
#endif
            
            StringBuilder targetFolder = new StringBuilder(clickedPathFull);
            
            targetFolder.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");
            


            string destPath = targetFolder.ToString();

    
            string shell = $"rsync -av --delete {clickedPathFull}/ {destPath}/";
            
#if UNITY_EDITOR_WIN
            shell = UnitCommand2WSLCommand(shell);
#endif
            
            Debug.Log($"shell :{shell}");
            ShellHelper.ShellRequest req = ShellHelper.ProcessCommand(shell, "");
           
            
            EditorUtility.DisplayDialog("Wait", "......", "Ok");

            req.onDone += () =>
            {
                EditorUtility.DisplayDialog("Done", "......", "Ok");
                AssetDatabase.Refresh();
            };
            
            
            
        }
        else
        {
            EditorUtility.DisplayDialog("Notice", "Only Work for Directory", "Ok");
        }
    }
    
    /// <summary>
    /// 资源拷贝 code 工程 ->  Res
    /// 1. 简单Prefab 在Casino工程改完后, 快速同步到Res
    /// 2. 需要检查CasinoResource工程, 有没有冲突
    /// </summary>
    [MenuItem("Assets/同步文件修改到资源仓库 ^l")]
    static void CopyResourceToResourceRepository()
    {
        string workDirectory = Path.GetFileName(Path.GetDirectoryName(Application.dataPath)); 
         
        int selectCount = Selection.assetGUIDs.Length;
        int copyCount = 0;
        
        //确认弹框, 以免误点
        bool isOk = EditorUtility.DisplayDialog("Attention Please",
            string.Format("You will copy {0} files to target Folder.\nWill replace them directly.\n Tap Ok to Continue", selectCount), "Ok", "Cancel");
        if (!isOk)
        {
            return;
        }
 
        for (int i = 0; i < selectCount; i++)
        {
            string clickedAssetGuid = Selection.assetGUIDs[i];
            string clickedFilePath      = AssetDatabase.GUIDToAssetPath(clickedAssetGuid);
            string clickedFilePathFull  = Path.Combine(Directory.GetCurrentDirectory(), clickedFilePath);
       
            FileAttributes attr = File.GetAttributes(clickedFilePathFull);
            bool isDirectory = attr.HasFlag(FileAttributes.Directory);
            string parentDirectory = Path.GetDirectoryName(clickedFilePathFull);
        
            //只能拷贝单一文件
            if (!isDirectory)
            {
                StringBuilder targetFile = new StringBuilder(clickedFilePathFull);
#if UNITY_EDITOR_WIN
                //targetFile.Replace("UCasino", "UCasinoResource");
                targetFile.Replace($@"\{workDirectory}\Assets", $@"\{workDirectory}Res\Assets");
                System.IO.File.Copy(clickedFilePathFull, targetFile.ToString(), true);
#else
                targetFile.Replace($"/{workDirectory}/Assets", $"/{workDirectory}Res/Assets");
                //强制复制
                string shellCmd = "cp -f " + clickedFilePathFull + " " + targetFile;
                UnityEngine.Debug.Log("shellCmd: " + shellCmd);
                ShellHelper.ShellRequest req =
                    ShellHelper.ProcessCommand(shellCmd, "");
#endif
                copyCount++;
            }
            else
            {
                EditorUtility.DisplayDialog("Notice", "Only Work for Files", "Ok");
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Done", string.Format("{0} files have been copy to target folder", copyCount),
            "Ok");
    }
}
