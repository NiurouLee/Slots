using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CommandTool 
{
    [MenuItem("Tools/同步Proto库到程序库")]
    public static void SyncProto()
    {
        //string strShell = $"bash ../Tools/sync_proto.sh";
        string workDirectory = Application.dataPath;
        workDirectory = AssetsSyncTools.UnixPath2WSLPath(workDirectory);

        //workDirectory = workDirectory.Replace("/Assets", "");

        string shell = $"cd {workDirectory};";
        
        string doShell = 
            @"rsync -av --delete ../../FortuneXProto/csharp-il/ ../Assets/Scripts/Code@GameModule/NetProtocbuf/Protocol/SpinX;rsync -av --delete ../../FortuneXProto/proto/ ../Assets/Scripts/Code@GameModule/NetProtocbuf/Proto/spinx";

        shell = $"{shell}{doShell}";
        shell = AssetsSyncTools.UnitCommand2WSLCommand(shell);

        //strShell = AssetsSyncTools.UnitCommand2WSLCommand(strShell);
        ShellHelper.ShellRequest req =
            ShellHelper.ProcessCommand(shell, "");
        Debug.Log($"=========strShell:{shell}");

        EditorUtility.DisplayDialog("Wait", "......", "Ok");

        
        req.onDone += () =>
        {
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
    }

    [MenuItem("Tools/生成ProtoC#")]
    public static void BuildProto2CSharp()
    {
        string workDirectory = Application.dataPath;
        string pathProtoFile = "../Assets/Scripts/Code@GameModule/NetProtocbuf/Proto";
        string pathProtoCSharp = "../Assets/Scripts/Code@GameModule/NetProtocbuf/Protocol/SpinX";
        string pathProtoC = "../Tools/";
        string strShell = $"cd {pathProtoC}\n " +
                          $"./protoc --proto_path={pathProtoFile}/ --csharp_out={pathProtoCSharp} {pathProtoFile}/spinx/*.proto";
        
        ShellHelper.ShellRequest req =
            ShellHelper.ProcessCommand(strShell, workDirectory);
        Debug.Log($"=========strShell:{strShell}");

        EditorUtility.DisplayDialog("Wait", "......", "Ok");

        req.onDone += () =>
        {
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
    }
    
    
    [MenuItem("Tools/同步Proto Bi库到程序库")]
    public static void SyncProtoBi()
    {
        //string strShell = $"bash ../Tools/sync_proto.sh";
        string workDirectory = Application.dataPath;
        workDirectory = AssetsSyncTools.UnixPath2WSLPath(workDirectory);

        //workDirectory = workDirectory.Replace("/Assets", "");

        string shell = $"cd {workDirectory};";
        
        string doShell = 
            @"rsync -av --delete ../../../ProtobufDefinition/csharp-lite/fortune_x/FortuneXBi.cs ../Assets/Scripts/Code@GameModule/NetProtocbuf/Protocol/Bi/;rsync -av --delete ../../../ProtobufDefinition/proto/fortune_x/fortune_x_bi.proto ../Assets/Scripts/Code@GameModule/NetProtocbuf/Proto/bi/";

        shell = $"{shell}{doShell}";
        shell = AssetsSyncTools.UnitCommand2WSLCommand(shell);

        //strShell = AssetsSyncTools.UnitCommand2WSLCommand(strShell);
        ShellHelper.ShellRequest req =
            ShellHelper.ProcessCommand(shell, "");
        Debug.Log($"=========strShell:{shell}");

        EditorUtility.DisplayDialog("Wait", "......", "Ok");

        
        req.onDone += () =>
        {
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
    }



    [MenuItem("Tools/Ad/生成Ad配置对应的C#结构")]
    public static void BuildAdCSharp()
    {
        string workDirectory = Application.dataPath;
        workDirectory = AssetsSyncTools.UnixPath2WSLPath(workDirectory);
        string pathProtoC = "../Tools/BuildTools/";

        string strShell = string.Empty;
        
#if UNITY_EDITOR_OSX
         strShell = $"cd {workDirectory};cd {pathProtoC};bash build_usergroup_ad.sh";
#else
        strShell = $"cd {workDirectory};cd {pathProtoC};fromdos *.sh;bash build_usergroup_ad.sh";
#endif
        
        strShell = AssetsSyncTools.UnitCommand2WSLCommand(strShell);
        
        ShellHelper.ShellRequest req =
            ShellHelper.ProcessCommand(strShell, "");
        Debug.Log($"=========strShell:{strShell}");

        EditorUtility.DisplayDialog("Wait", "......", "Ok");

        req.onDone += () =>
        {
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
        
        req.onError += () =>
        {
            AssetDatabase.Refresh();
        };
    }
    
    [MenuItem("Tools/Ad/同步Ad配置的Json文件")]
    public static void SyncAdConfig()
    {
        
        string workDirectory = Application.dataPath;
        workDirectory = AssetsSyncTools.UnixPath2WSLPath(workDirectory);
        string pathProtoC = "../Tools/ExcelTools/";
        
    
        string strShell = string.Empty;
#if UNITY_EDITOR_OSX
        strShell = $"cd {workDirectory};cd {pathProtoC};bash build_usergroup_ad.sh";
#else
        strShell = $"cd {workDirectory};cd {pathProtoC};fromdos *.sh;bash build_usergroup_ad.sh";

#endif       

        strShell = AssetsSyncTools.UnitCommand2WSLCommand(strShell);
        
        ShellHelper.ShellRequest req =
            ShellHelper.ProcessCommand(strShell, "");
        Debug.Log($"=========strShell:{strShell}");

        EditorUtility.DisplayDialog("Wait", "......", "Ok");

        req.onDone += () =>
        {
            EditorUtility.DisplayDialog("Done", "......", "Ok");
            AssetDatabase.Refresh();
        };
        
        req.onError += () =>
        {
            AssetDatabase.Refresh();
        };
        
    }
}
