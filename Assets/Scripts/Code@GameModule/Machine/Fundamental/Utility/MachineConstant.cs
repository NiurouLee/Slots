// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/19/18:11
// Ver : 1.0.0
// Description : MachineContext.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public static class MachineConstant
    {
        //控制面板横版高度
        public static float controlPanelHeight = 118;

        //控制面板竖版高度
        public static float controlPanelVHeight = 185;

        //顶部面板横版高度
        public static float topPanelHeight = 72;

        //顶部面板竖版高度
        public static float topPanelVHeight = 58 + 50;

        public static int pixelPerUnit = 100;

        public static float pixelPerUnitInv = 0.01f;

        public static float designAspectRatio = (float) 16 / 9;

        public static float aspectRatio18To9 = (float) 18 / 9;

        public static float aspectRatio16To9 = (float) 16 / 9;

        public static float aspectRatio4To3 = (float) 4 / 3;

        public static float titleOffSetY = 0;
        public static float controlPanelOffsetY = 0;
        public static float widgetOffset = 0;

        static MachineConstant()
        {
            UpdateConstant();

            XDebug.Log("titleOffSetY:" + titleOffSetY);
            XDebug.Log("topPanelVHeight:" + topPanelVHeight);
            XDebug.Log("controlPanelOffsetY:" + controlPanelOffsetY);
            XDebug.Log("controlPanelVHeight:" + controlPanelVHeight);
            XDebug.Log("widgetOffset:" + widgetOffset);
        }

        public static void UpdateConstant()
        {
            titleOffSetY = 0;
            widgetOffset = 0;
            controlPanelOffsetY = 0;
            
#if UNITY_IOS
            if (ViewManager.Instance.IsPortrait)
            {
                var safeArea = Screen.safeArea;

                XDebug.Log($"safeArea: [{safeArea.x},{safeArea.y},{safeArea.width},{safeArea.height}]");
                XDebug.Log($"Screen: [{Screen.width},{Screen.height}]");

                controlPanelOffsetY = safeArea.y;

                titleOffSetY = Screen.height - safeArea.height - safeArea.y;

                if(titleOffSetY > 101) {
                    titleOffSetY = 101;
                }

                if (Screen.height / (float)Screen.width < 1.79) 
                {
                     titleOffSetY = 0;
                }

                var scale = ViewResolution.referenceResolutionPortrait.y / Screen.height;

                titleOffSetY = titleOffSetY * scale;

                if (Screen.height / (float)Screen.width < 1.79)
                {
                    controlPanelOffsetY = 0;
                }
                
                if (controlPanelOffsetY > 0)
                {
                    controlPanelOffsetY = Mathf.Min(controlPanelOffsetY, 45);
                }
                
                controlPanelOffsetY = controlPanelOffsetY * scale;
 
                topPanelVHeight = titleOffSetY + 108;
                controlPanelVHeight = controlPanelOffsetY + 185;
            }
            else
            {
                widgetOffset = Mathf.Max(Screen.safeArea.x, Screen.width - Screen.safeArea.width - Screen.safeArea.x);
                
                if(widgetOffset > 101) {
                    widgetOffset = 101;
                }

                if (Screen.width / (float)Screen.height < 1.79) 
                {
                     widgetOffset = 0;
                }

                widgetOffset = widgetOffset * ViewResolution.referenceResolutionLandscape.x / Screen.width;
            }
#endif
        }
    }
}