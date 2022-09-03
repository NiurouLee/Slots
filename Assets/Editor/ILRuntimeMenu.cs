#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeMenu
{
   [MenuItem("ILRuntime/安装VS调试插件")]
    static void InstallDebugger()
    {   
        EditorUtility.OpenWithDefaultApp("Assets/Samples/ILRuntime/1.6.3/Demo/Debugger~/ILRuntimeDebuggerLauncher.vsix");
    }

    [MenuItem("ILRuntime/打开ILRuntime中文文档")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
    }

    [MenuItem("ILRuntime/打开ILRuntime Github项目")]
    static void OpenGithub()
    {
        Application.OpenURL("https://github.com/Ourpalm/ILRuntime");
    }

    [MenuItem("ILRuntime/生成常用容器的类型，减少热更出现的AOT")]
    static void GenerateCollectionCommonTypeCode()
    {
        List<string> commonTypeList = new List<string>()
        {
            "int", "long", "uint", "ulong", "string", "double", "float", "decimal",
            "TypePreRef", "Transform", "Vector3", "Vector2", "Quaternion", "Color"
        };
         
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("using System.Collections.Generic;\n");
        stringBuilder.Append("using Google.ilruntime.Protobuf.Collections;\n");
        stringBuilder.Append("using UnityEngine;\n");
        stringBuilder.Append("\n");
        stringBuilder.Append("public class TypePreRef\n");
        stringBuilder.Append("{\n");
        GenerateOneParamCollection(commonTypeList, stringBuilder, "List");
        GenerateOneParamCollection(commonTypeList, stringBuilder, "RepeatedField");
        GenerateTwoParamCollection(commonTypeList, stringBuilder, "Dictionary");
        GenerateTwoParamCollection(commonTypeList, stringBuilder, "MapField");
        stringBuilder.Append("}\n");
         
        GUIUtility.systemCopyBuffer = stringBuilder.ToString();
        
        string assetPath = $"{Application.dataPath}/Scripts/Code@GameModule/Utils/TypePreRef.cs";

        if (File.Exists(assetPath))
        {
            File.Delete(assetPath);
        }
        
       // File.Create(assetPath).Dispose();
        
        File.WriteAllText(assetPath, stringBuilder.ToString());
         
        AssetDatabase.Refresh();
    }

    static void GenerateOneParamCollection(List<string> commonTypeList, StringBuilder content, string collectTypeName)
    {
        content.Append("\n");
        content.Append($"//----------------{collectTypeName}-------------------------\n");
      
        for (var i = 0; i < commonTypeList.Count; i++)
        {
            content.Append($"\tpublic {collectTypeName}<{commonTypeList[i]}> {commonTypeList[i].Replace('.','_')}{collectTypeName} = new {collectTypeName}<{commonTypeList[i]}>();\n");
        }
    }
    
    static void GenerateTwoParamCollection(List<string> commonTypeList, StringBuilder content, string collectTypeName)
    {
        content.Append("\n");
        content.Append($"//----------------{collectTypeName}-------------------------\n");
        
        for (var i = 0; i < commonTypeList.Count; i++)
        {
            for (var j = 0; j < commonTypeList.Count; j++)
            {
                content.Append($"\tpublic {collectTypeName}<{commonTypeList[i]}, {commonTypeList[j]}> {commonTypeList[i].Replace('.','_')}_{commonTypeList[j].Replace('.','_')}{collectTypeName} = new  {collectTypeName}<{commonTypeList[i]}, {commonTypeList[j]}>();\n");
            }
        }
    }

}
#endif
