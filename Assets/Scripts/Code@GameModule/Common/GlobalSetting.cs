using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public static class GlobalSetting
    {
        public static string bundleVersion = "v6";
        
        public static string projectName = "FortuneX";
        
        public static string termsOfServiceURL = "https://www.casualjoygames.com/terms/";
        
        public static string fanPageUrl = "https://www.facebook.com/cashcrazevip";
        
#if PRODUCTION_PACKAGE
        public static string couponUrl = "";
#else
        public static string couponUrl = "";
#endif
        /// <summary>
        /// HELP SHIFT 配置
        /// </summary>
        ///
             //Server Info
        static GlobalSetting()
        {
        }
    }
}