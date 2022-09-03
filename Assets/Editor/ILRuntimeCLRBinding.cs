#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using ILRuntimeAdapters;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
   [MenuItem("ILRuntime/通过自动分析热更DLL生成CLR绑定")]
    public static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        var dllAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GameModuleDll/GameModule.bytes");
        // using (FileStream fs = new FileStream("Assets/GameModuleDll/GameModule.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        // {
        // using (DllStream fs = new DllStream(dllAsset.bytes, ILRuntimeHelp.dllKey))
        // {

        Stream fs = null;
        if (ILRuntimeHelp.isDLLEncrypt)
        {
            fs = new DllStream(dllAsset.bytes, ILRuntimeHelp.dllKey);
        }
        else
        {
            fs = new FileStream("Assets/GameModuleDll/GameModule.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read);
        }

        domain.LoadAssembly(fs);

            //Crossbind Adapter is needed to generate the correct binding code
            InitILRuntime(domain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain,
                "Assets/Scripts/Code/BindingCode");
            
            fs?.Close();
            fs?.Dispose();
        // }
        // }

        AssetDatabase.Refresh();
    }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        domain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdapter());
        domain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
        domain.RegisterCrossBindingAdaptor(new ConfigManagerBaseAdapter());
        //domain.RegisterCrossBindingAdaptor(new InvalidOperationExceptionAdapter());
       // domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
    }
}
#endif
