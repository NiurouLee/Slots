// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-02 3:28 PM
// Ver : 1.0.0
// Description : DialogArgs.cs
// ChangeLog :
// **********************************************

using System;

namespace GameModule
{
    public class PopupArgs
    {
        public bool isFence = false;
        /// <summary>
        /// 在加入到delayQueue的时候赋值
        /// </summary>
        public BlockLevel blockLevel = BlockLevel.DefaultLevel;
         
        public bool isPopupPool = false;
        //只有当ID为 DIALOG_POOL的时候才有意义
        public string poolId;
        
        public Type popupType;

        public Action popupCloseAction;
 
        public object extraArgs;
        public string source;

        public PerformCategory performCategory = PerformCategory.None;
 
        //放入队列之后是否需要让队列弹出最前面的弹板
        public bool needDequeue = false;
        
        public bool canBeDequeue = true;
  
        public PopupArgs(bool inIsFence, Action closeAction, PerformCategory inPerformCategory = PerformCategory.None)
        {
            isFence = inIsFence;
            popupCloseAction = closeAction;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(bool inIsPool, string inPoolId)
        {
            isPopupPool = inIsPool;
            poolId = inPoolId;
        }
         
        public PopupArgs(Type inPopupType, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType, BlockLevel inBlockLevel, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            blockLevel = inBlockLevel;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType, string inSource, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            source = inSource;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType, object inExtraArgs, string inSource, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            extraArgs = inExtraArgs;
            source = inSource;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType, Action closeAction, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            popupCloseAction = closeAction;
            performCategory = inPerformCategory;
        }
        public PopupArgs(Type inPopupType, Action closeAction, string inSource, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            popupCloseAction = closeAction;
            source = inSource;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType,  object inExtraArgs, Action closeAction,  BlockLevel inBlockLevel = BlockLevel.DefaultLevel, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            extraArgs = inExtraArgs;
            popupCloseAction = closeAction;
            blockLevel = inBlockLevel;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType,  object inExtraArgs, Action closeAction, string inSource, BlockLevel inBlockLevel = BlockLevel.DefaultLevel, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            source = inSource;
            extraArgs = inExtraArgs;
            popupCloseAction = closeAction;
            blockLevel = inBlockLevel;
            performCategory = inPerformCategory;
        }
        
        public PopupArgs(Type inPopupType,  object inExtraArgs, BlockLevel inBlockLevel = BlockLevel.DefaultLevel, PerformCategory inPerformCategory = PerformCategory.None)
        {
            popupType = inPopupType;
            extraArgs = inExtraArgs;
            blockLevel = inBlockLevel;
            performCategory = inPerformCategory;
        }

    }
}