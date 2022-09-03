// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : WheelConfig.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class WheelConfig
    {
        /// <summary>
        /// 轮盘的名字
        /// </summary>
        public string wheelName = "Wheel";
        
        /// <summary>
        /// 轮盘的列数
        /// </summary>
        public int rollCount = 5;
     
        /// <summary>
        /// 轮盘的行数
        /// </summary>
        public int rollRowCount = 3;

        /// <summary>
        /// 轮盘上图标的最大高度
        /// </summary>
        public int elementMaxHeight = 1;
        /// <summary>
        /// 轮盘上方扩展高度
        /// </summary>
        public int extraTopElementCount = 0;
        /// <summary>
        /// 是否需要创建ReSpin轮盘
        /// </summary>
        public bool buildReSpinWheel = false;
        /// <summary>
        /// base下的缓动节奏
        /// </summary>
        public string normalEasingName;
        /// <summary>
        /// free模式下下的缓动节奏
        /// </summary>
        public string freeEasingName;
        /// <summary>
        /// reSpin模式下下的缓动节奏
        /// </summary>
        public string reSpinEasingName;
        /// <summary>
        /// 特殊模式模式下下的缓动节奏（用于扩展）
        /// </summary>
        public string specialEasingName;
        
        /// <summary>
        /// 图标赢钱框的资源名称
        /// </summary>
        public string winFrameAssetName;

        /// <summary>
        /// 赢钱线动画播放时长
        /// </summary>
        public float winLineBlinkDuration = 2;
        
        /// <summary>
        /// BonusLine 动画播放时长
        /// </summary>
        public float bonusLineBlinkDuration = 2;


        /// <summary>
        /// 图标排序是否是上压下
        /// </summary>
        public bool topRowHasHighSortOrder = false;
        
        /// <summary>
        /// 图标排序是否是左压右
        /// </summary>
        public bool leftColHasHighSortOrder = false;
        
        
        /// <summary>
        /// 是各个格子独立转的轮盘
        /// </summary>
        public bool isIndependentWheel = false;
        
        /// <summary>
        /// 赢钱线是否显示在图标上
        /// </summary>
        public bool isPayLineUpElement = false;
        
        /// <summary>
        /// anticipation动画资源名称
        /// </summary>
        public string anticipationAnimationAssetName = "AnticipationAnimation";
        /// <summary>
        /// anticipation动画音效
        /// </summary>
        public string anticipationSoundAssetName = "AnticipationSound";
         
        public WheelConfig()
        {
            normalEasingName = "";
            freeEasingName = "";
            reSpinEasingName = "";
            buildReSpinWheel = false;
        }
    }
}