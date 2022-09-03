// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-01 4:04 PM
// Ver : 1.0.0
// Description : PopupPool.cs
// ChangeLog :
// **********************************************


using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using ILRuntime.Runtime;

namespace GameModule
{
    public class PopupPool
    {
      
        private PopUpConfig.Types.PopupConfigPool _poolConfig;

        public class PopupServerConfig
        {
            public Type popupType;
            public int weight;
            public List<int> filterIds;
        }

        protected List<PopupServerConfig> _popUpConfigList;
        
        public string GetPoolId()
        {
            return _poolConfig.Id;
        }
        
        public PopupPool(PopUpConfig.Types.PopupConfigPool poolConfig)
        {
            _poolConfig = poolConfig;

            var popupNames = poolConfig.PopupName.Split(',');
            var popupWeights = poolConfig.Weight.Split(',');
            var popupFilters = poolConfig.Filter.Split('|');

            var count = popupNames.Length;

            _popUpConfigList = new List<PopupServerConfig>();
            
            for (var i = 0; i < count; i++)
            {
                if (!string.IsNullOrEmpty(popupNames[i]))
                {
                    var popUpType = Type.GetType($"GameModule.{popupNames[i]}");
                  
                    if (popUpType != null && popupWeights.Length > i)
                    {
                        try
                        {
                            var popupServerConfig = new PopupServerConfig();
                            popupServerConfig.popupType = popUpType;
                            popupServerConfig.weight = Int32.Parse(popupWeights[i]);
                            popupServerConfig.filterIds = new List<int>();
                        
                            if (!string.IsNullOrEmpty(popupFilters[i]))
                            {
                                var filterIds =popupFilters[i].Split(',');
                                if (filterIds.Length > 0)
                                {
                                    for (var c = 0; c < filterIds.Length; c++)
                                    {
                                        popupServerConfig.filterIds.Add(Int32.Parse(filterIds[c]));
                                    }
                                }
                            }
                            
                            _popUpConfigList.Add(popupServerConfig);
                        }
                        catch (Exception e)
                        {
                            XDebug.LogError(e.Message);
                          //  throw;
                        }
                    } 
                    else if (popupNames[i] == "Empty")
                    {
                        var popupServerConfig = new PopupServerConfig();
                        popupServerConfig.popupType = null;
                        popupServerConfig.weight = Int32.Parse(popupWeights[i]);
                        popupServerConfig.filterIds = new List<int>();
                        _popUpConfigList.Add(popupServerConfig);
                    }   
                }
            }
        }
        
        public Type GetPopup(PopupArgs popUpArgs)
        {
            var availablePopupIndexes = new List<PopupServerConfig>();

            for (var i = 0; i < _popUpConfigList.Count; i++)
            {
                var available = true;
                
                var filters = _popUpConfigList[i].filterIds;

                if (filters != null)
                {
                    for (var j = 0; j < filters.Count; j++)
                    {
                        var isValid = PopupFilter.CheckValid(filters[j], popUpArgs);

                        if (!isValid)
                        {
                            XDebug.Log("PopupLogic:failed on not filter (" + filters[j] + ")");
                            available = false;
                        }
                    }
                }
                
                if (available)
                {
                    availablePopupIndexes.Add(_popUpConfigList[i]);
                }
            }
            
            
            if (availablePopupIndexes.Count > 0)
            {
                if (availablePopupIndexes.Count == 1)
                {
                    LogPopupChoice("OnlyAvailable", availablePopupIndexes[0].popupType);
                    return availablePopupIndexes[0].popupType;
                }
            
                var totalWeight = 0;
                var availableWeights = new List<int>();
            
                int maxHighWeight = 9999;
                int maxHighWeightIndex = -1;
            
                for (var i = 0; i < availablePopupIndexes.Count; i++)
                {
                    int w = availablePopupIndexes[i].weight;
                    if (w > maxHighWeight)
                    {
                        maxHighWeight = w;
                        maxHighWeightIndex = i;
                    }
            
                    availableWeights.Add(w);
                    totalWeight += w;
                }
            
                if (maxHighWeightIndex >= 0)
                {
                    LogPopupChoice("MaxWeight", availablePopupIndexes[maxHighWeightIndex].popupType);
                    return availablePopupIndexes[maxHighWeightIndex].popupType;
                }
            
                var selectIndex = XUtility.RandomSelect(availableWeights, totalWeight);
            
                LogPopupChoice("Random", availablePopupIndexes[selectIndex].popupType);
                
                return availablePopupIndexes[selectIndex].popupType;
            }
            
            return null;
        }

        protected void LogPopupChoice(string choiceStrategy,  Type popupType)
        {
            if (popupType != null)
            {
                XDebug.Log($"[[ShowOnExceptionHandler]]: [{choiceStrategy}]Choose Popup: " + popupType.Name);
            }
            else
            {
                XDebug.Log($"[[ShowOnExceptionHandler]]: [{choiceStrategy}]Choose Popup: " + "Empty");
            }
        }
    }
}