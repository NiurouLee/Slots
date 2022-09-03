// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : ElementConfig.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{

    public enum BlinkAnimationPlayStyleType
    {
        /// <summary>
        /// 默认方式，播完Blink就变成静态
        /// </summary>
        Default,
        /// <summary>
        /// 播放完成之后，一致播放Idle动画
        /// </summary>
        Idle,
        
        /// <summary>
        /// 播放Blink之后播放Idle动画，然后判断是否触发bonus，如果触发就一致播放IDLE，如果没有就停止Idle 
        /// </summary>
        IdleCondition
    }
    
    public class ElementConfig
    {
        /// <summary>
        /// element 的 ID
        /// </summary>
        public uint id;
        /// <summary>
        /// element的单位宽度，正常情况都为1
        /// </summary>
        public uint width = 1;
        /// <summary>
        /// element的高度，长图标的个数大于1，
        /// </summary>
        public uint height = 1;

        /// <summary>
        /// element的位置
        /// 如果是一个大图标1x3
        /// X -> 2
        /// X -> 1
        /// X -> 0
        /// </summary>
        public uint position = 0;

        /// <summary>
        /// 是否可以创建大图标Position非0部分
        /// </summary>
        public bool createBigElementParts = false;
        /// <summary>
        /// 图标的名字
        /// </summary>
        public string name;
        /// <summary>
        /// 图标相对层级偏移，用于某些关卡中将高级图标置于普通图标的上面
        /// </summary>
        public int zOffset = 0;
        
        /// <summary>
        /// highLightZOffset = 200;
        /// </summary>
        public int activeZOffset = 200;
        /// <summary>
        /// 图标动态资源，各种动画（Appear，HighLight）等
        /// </summary>
        public string activeAssetName;
        /// <summary>
        /// 图标静止状态下的资源
        /// </summary>
        public string staticAssetName;
        
        /// <summary>
        /// blink动画音效名称
        /// </summary>
        public string blinkSoundName;
        
        /// <summary>
        /// 音效播放顺序
        /// </summary>
        public int blinkSoundOrderId;

        public Type elementClassType;
        

        public int initializePoolSize = 5;

        /// <summary>
        /// 在调用StopAllAnimation的时候，是否保持State不变
        /// </summary>
        public bool keepStateWhenStopAllAnimation;
        
        public BlinkAnimationPlayStyleType blinkType;
 
        public ElementPool pool;

        /// <summary>
        /// 特殊图标的额外数据
        /// </summary>
        public Dictionary<string, object> extraInfo;
        

        public SpriteMaskInteraction defaultInteraction;
        public SpriteMaskInteraction activeStateInteraction;

        public (float, float, float) staticGrayLayerPrams;
        
        public Color staticGrayLayerMultiBlendModePrams;
        
        public T GetExtra<T>(string extraName)
        {
            if (extraInfo.ContainsKey(extraName))
            {
                return (T) extraInfo[extraName];
            }

            return default(T);
        }
 
        public void SetUpElementConfig(MachineAssetProvider machineAssetProvider)
        {
              SetUpElementPool(machineAssetProvider);
        }

        protected void SetUpElementPool(MachineAssetProvider machineAssetProvider)
        {
            pool = new ElementPool(this, machineAssetProvider);
        }
         
        public Element GetStaticElement()
        {
            return pool.GetStaticElement();
        }

        public Element GetActiveElement()
        {
            return pool.GetActiveElement();
        }

        public void RecycleElement(Element element)
        {
            pool.Recycle(element);
        }

        public ElementConfig(uint elementId)
        {
            id = elementId;
          
            width = 1;
            height = 1;
            zOffset = 0;
            name = id.ToString();
            elementClassType = typeof(Element);
            activeAssetName = "Active_" + id;
            staticAssetName = "Static_" + id;

            defaultInteraction = SpriteMaskInteraction.VisibleInsideMask;
            activeStateInteraction = SpriteMaskInteraction.None;
        }
    }
}