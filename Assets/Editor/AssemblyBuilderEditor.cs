using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEditor.Compilation;
using System.IO;
using UnityEditor.Scripting.ScriptCompilation;

public class AssemblyBuilderEditor
{
    [MenuItem("ILRuntime/Build HotFix Dll")]
    public static void BuildAssemblySync()
    {
        BuildAssembly(true,false);
    }
    
    
    public static void BuildAssemblySync(bool isDebug)
    {
        BuildAssembly(true,isDebug);
    }

    static void CopyFile()
    {
        if (Directory.Exists("AssemblyTemp"))
        {
            Directory.Delete("AssemblyTemp", true);
        }

        Directory.CreateDirectory("AssemblyTemp/Code");
        Directory.CreateDirectory("AssemblyTemp/MyAssembly");
        // string[] files = Directory.GetFiles("Assets/Scripts/Code@GameModule/", "*.cs", SearchOption.AllDirectories);
        // for (int i = 0; i < files.Length; i++)
        // {
        //     File.Copy(files[i], "AssemblyTemp/Code/" + Path.GetFileName(files[i]));
        // }
    }

    static void BuildAssembly(bool wait,bool isDebug)
    {
        
        Assembly[] playerAssemblies =
            CompilationPipeline.GetAssemblies(AssembliesType.Player);

        Assembly GameModuleAssembly = null;
       
        foreach (var assembly in playerAssemblies)
        {
            if(assembly.name == "GameModuleAssembly")
            {
                GameModuleAssembly = assembly;
            }
        }
        
        
        CopyFile();

        var scripts = new[] {"AssemblyTemp/Code/*.cs"};
        var outputAssembly = "AssemblyTemp/MyAssembly/GameModuleTemp.dll";
        var outputPdb = "AssemblyTemp/MyAssembly/GameModuleTemp.pdb";
        var excludeAssemblyPath = "Library/ScriptAssemblies/GameModuleAssembly.dll";
        var targetAssemblyPath = "Assets/GameModuleDll/GameModule.bytes";
        var targetPdbPath = "Assets/GameModuleDll/GameModule.PDB.bytes";

        
        var assemblyBuilder = new AssemblyBuilder(outputAssembly, GameModuleAssembly.sourceFiles);
#if UNITY_IOS || UNITY_EDITOR_OSX
        assemblyBuilder.additionalReferences = new string[]
        {
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.UIModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.AnimationModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.InputLegacyModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.AudioModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.SpriteMaskModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.CoreModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.Physics2DModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.TextRenderingModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.PhysicsModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.ParticleSystemModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.IMGUIModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.VideoModule.dll",
            EditorApplication.applicationPath + "/Contents/Managed/UnityEngine/UnityEngine.UnityWebRequestModule.dll",

            "Library/ScriptAssemblies/MainCodeAssembly.dll",
            "Library/ScriptAssemblies/Unity.Addressables.dll",
            "Library/ScriptAssemblies/Unity.ResourceManager.dll",
            "Library/ScriptAssemblies/DOTween.Modules.dll",
            "Library/ScriptAssemblies/ILRuntime.dll",
        };
#else
        string appPath = EditorApplication.applicationPath;
        appPath = Path.GetDirectoryName(appPath);
        assemblyBuilder.additionalReferences = new string[]
        {
            appPath + "/Data/Managed/UnityEngine/UnityEngine.UIModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.AnimationModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.InputLegacyModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.AudioModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.SpriteMaskModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.Physics2DModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.TextRenderingModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.PhysicsModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.ParticleSystemModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.IMGUIModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.VideoModule.dll",
            appPath + "/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestModule.dll",

            "Library/ScriptAssemblies/MainCodeAssembly.dll",
            "Library/ScriptAssemblies/Unity.Addressables.dll",
            "Library/ScriptAssemblies/Unity.ResourceManager.dll",
            "Library/ScriptAssemblies/DOTween.Modules.dll",
            "Library/ScriptAssemblies/ILRuntime.dll"
        };
#endif
        // Exclude a reference to the copy of the assembly in the Assets folder, if any.
        assemblyBuilder.excludeReferences = new string[] {excludeAssemblyPath, targetAssemblyPath};

        if (isDebug)
        {
            assemblyBuilder.flags = AssemblyBuilderFlags.DevelopmentBuild;

        }
        else
        {
            assemblyBuilder.flags = AssemblyBuilderFlags.None;

        }

        // Called on main thread
        assemblyBuilder.buildStarted += delegate(string assemblyPath)
        {
            Debug.LogFormat("Assembly build started for {0}", assemblyPath);
        };

        // Called on main thread
        assemblyBuilder.buildFinished += delegate(string assemblyPath, CompilerMessage[] compilerMessages)
        {
            for (int i = 0; i < compilerMessages.Length; i++)
            {
                Debug.Log(compilerMessages[i].message);
            }

            var errorCount = compilerMessages.Count(m => m.type == CompilerMessageType.Error);
            var warningCount = compilerMessages.Count(m => m.type == CompilerMessageType.Warning);

            Debug.LogFormat("Assembly build finished for {0}", assemblyPath);
            Debug.LogFormat("Warnings: {1} - Errors: {0}", errorCount, warningCount);

            for (var i = 0; i < errorCount; i++)
            {
                Debug.LogError(compilerMessages[i].message);
                if (compilerMessages[i].message.Contains(": warning"))
                {
                    errorCount--;
                }
            }

            if (errorCount == 0)
            {
                //File.Copy(outputAssembly, targetAssemblyPath, true);

                //加密
                byte[] bytesOrigin = File.ReadAllBytes(outputAssembly);

                if (ILRuntimeHelp.isDLLEncrypt)
                {
                    byte[] bytes = CryptoHelper.AesEncryptWithNoPadding(bytesOrigin,ILRuntimeHelp.dllKey);
                    File.WriteAllBytes(targetAssemblyPath,bytes);
                }
                else
                {
                    File.WriteAllBytes(targetAssemblyPath,bytesOrigin); 
                }
                
                File.Copy(outputPdb,targetPdbPath,true);
                
                AssetDatabase.ImportAsset(targetAssemblyPath);
                AssetDatabase.ImportAsset(targetPdbPath);
            }
        };

        // Start build of assembly
        if (!assemblyBuilder.Build())
        {
            Debug.LogErrorFormat("Failed to start build of assembly {0}!", assemblyBuilder.assemblyPath);
            return;
        }

        if (wait)
        {
            while (assemblyBuilder.status != AssemblyBuilderStatus.Finished)
            {
                System.Threading.Thread.Sleep(10);
            }

            Directory.Delete("AssemblyTemp", true);
        }
    }
}