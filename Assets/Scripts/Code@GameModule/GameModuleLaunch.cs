using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DragonU3DSDK.Network.API.ILProtocol;

using SimpleJson;
using Tool;
using UnityEngine.Networking;
using UnityEngine;

namespace GameModule
{
    public class GameModuleLaunch
    {
        public static bool isGameModule;
        public static  void Start(bool inIsGameModule)
        {
            isGameModule = inIsGameModule;
//临时逻辑，国服地址变了            
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            if (ConfigurationController.Instance.API_Server_URL_Beta.Contains(
                "http://ec2-52-81-245-97.cn-north-1.compute.amazonaws.com.cn"))
            {
                ConfigurationController.Instance.API_Server_URL_Beta = ConfigurationController.Instance.API_Server_URL_Beta.Replace(
                    "http://ec2-52-81-245-97.cn-north-1.compute.amazonaws.com.cn", "http://52.81.148.216");
            }
#endif
            
//#if PRODUCTION_PACKAGE
            // XDebug.Log("GameModuleLaunch.Start");
            // IEnumeratorTool.instance.StartCoroutine(CheckPackageIsSupport(RunGame));
//#else
            XDebug.Log("GameModuleLaunch.Start->RunGame");

            
            //BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLaunchApp);
             RunGame();
//#endif            
        }

        public static List<Type> GetAllType()
        {
            List<Type> allTypes = new List<Type>();

            if (isGameModule)
            {
                var values = ILRuntimeHelp.appdomain.LoadedTypes.Values.ToList();
                foreach (var v in values)
                    allTypes.Add(v.ReflectionType);
            }
            else
            {
                var assembly = Assembly.GetAssembly(typeof(GameModuleLaunch));
                if (assembly == null)
                {
                    Debug.LogError("当前dll is null");
                    return null;
                }

                allTypes = assembly.GetTypes().ToList();
            }

            return allTypes;
        }

        private static Client client;
        public static void RunGame()
        {
            if (isGameModule)
            {
                new Client().RunGame();

            }
            else
            {
                if (client != null)
                {
                    client.CleanUp();
                }

                client = new Client();
                client.RunGame();
            }

        }
    }
}