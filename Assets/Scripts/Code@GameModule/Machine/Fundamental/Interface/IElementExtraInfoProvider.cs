// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/06/11:53
// Ver : 1.0.0
// Description : ElementExtraInfoProvider.cs
// ChangeLog :
// **********************************************
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public interface IElementExtraInfoProvider
    {
        Type GetElementClassType(uint id);

        int GetElementSortingOffset(uint id);
       
        int GetElementActiveSortingOffset(uint id);

        string GetElementActiveAssetAddress(uint id, string name);
        
        string GetElementStaticAssetAddress(uint id, string name);

        /// <summary>
        /// 获取图标blink音效名称
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        /// 
        string GetElementBlinkSoundName(uint id, string name);
        
        /// <summary>
        /// 获取图标blink动画的播放方式
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BlinkAnimationPlayStyleType GetElementBlinkAnimationPlayStyleType(uint id);

        void ParseElementExtra(ElementConfig elementConfig, string extraInfo);
        
        SpriteMaskInteraction GetElementDefaultInteraction(uint id);
        
        SpriteMaskInteraction GetElementActiveInteraction(uint id);

        bool GetNeedKeepStateWhenStopAllAnimation(uint id);


        (float, float, float) GetElementStaticGrayLayerPrams(uint id);
        Color GetElementStaticGrayLayerMultiBlendModePrams(uint id); 
        List<uint> GetElementBlinkVariantList(uint id);
        int GetElementBlinkSoundOrderId(uint id);

        bool CanShowElementAnticipation(uint id);

        bool GetElementCreateBigElementParts(uint id);
    }
}