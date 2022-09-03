// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : ElementConfigSet.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{
    /// <summary>
    /// 一个老虎机的的所有图标的集合，包含所有图标的配置信息
    /// </summary>
    public class ElementConfigSet
    {
        private readonly Dictionary<uint, ElementConfig> _elementConfigDict;

        private readonly Dictionary<string, ElementConfig> dicNameElementConfigs;
        /// <summary>
        /// 图标的总个数
        /// </summary>
        public int elementNum;
 
        public ElementConfigSet()
        {
            _elementConfigDict = new Dictionary<uint, ElementConfig>();
            dicNameElementConfigs = new Dictionary<string, ElementConfig>();
        }

        public void AddElementConfig(ElementConfig elementConfig)
        {
#if !PRODUCTION_PACKAGE
            if (_elementConfigDict.ContainsKey(elementConfig.id))
            {
                XDebug.LogError($"ElementId-->{elementConfig.id} already Exist!!!");   
            }
#endif
            _elementConfigDict[elementConfig.id] = elementConfig;
            dicNameElementConfigs[elementConfig.name] = elementConfig;
            
            elementNum = _elementConfigDict.Keys.Count;
        }
        
        public ElementConfigSet(List<ElementConfig> elementConfigs)
        {
            _elementConfigDict = new Dictionary<uint, ElementConfig>();
            
            for (var i = 0; i < elementConfigs.Count; i++)
            {
                _elementConfigDict[elementConfigs[i].id] = elementConfigs[i];
                dicNameElementConfigs[elementConfigs[i].name] = elementConfigs[i];
            }

            elementNum = elementConfigs.Count;
        }

        public void SetUpElementConfig(MachineAssetProvider assetProvider)
        {
            foreach (var item in _elementConfigDict)
            {
                item.Value.SetUpElementConfig(assetProvider);
            }
        }

        public ElementConfig GetElementConfig(uint elementId)
        {
            if (_elementConfigDict.ContainsKey(elementId))
            {
                return _elementConfigDict[elementId];
            }
#if !PRODUCTION_PACKAGE
            Debug.LogError($"Can not find element config for ElementId[{elementId}]");
#endif
            return null;
        }

        public ElementConfig GetElementConfigByIndex(int index)
        {
            return _elementConfigDict.ToList()[index].Value;
        }

        public ElementConfig GetElementConfigByName(string name)
        {
            ElementConfig elementConfig = null;
            if(!dicNameElementConfigs.TryGetValue(name,out elementConfig))
            {
#if !PRODUCTION_PACKAGE
                Debug.LogError($"Can not find element config for ElementName[{name}]");
#endif
            }

            return elementConfig;
        }

        public int GetElementConfigCount()
        {
            return _elementConfigDict.Count;
        }
    }
}