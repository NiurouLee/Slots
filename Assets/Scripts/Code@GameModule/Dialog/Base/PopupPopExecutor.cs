// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-01 7:04 PM
// Ver : 1.0.0
// Description : DialogPopExecutor.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;

namespace GameModule.UI
{
    public class PopupPopExecutor
    {
        public static Dictionary<string, int> sessionPopupCountDict;

        public static bool CanPopNext(BlockLevel blockLevel)
        {
            var maxBlockLevel = PopupStack.GetPopupMaxBlockLevel();
            
            return (int)blockLevel > maxBlockLevel;
        }
    
        public static async void ShowPopUp(PopupArgs popupArgs)
        {
            var popupType = popupArgs.popupType;
 
            var address = View.TryGetAssetAddressFromAttribute(popupType);
            
            if(string.IsNullOrEmpty(address))
                return;
             
            var popup = await PopupStack.ShowPopup(popupType, address, popupArgs);
            
            if (popup != null)
            {
                UpdateSessionPopupCount(popupType.Name);
                UpdateTodayPopCount(popupType.Name);
                UpdateLastPopTime(popupType.Name);

                popup.SubscribeCloseAction(() =>
                {
                    if (PopupStack.CanSpreadDialogCloseEvent)
                    {
                        popupArgs.popupCloseAction?.Invoke();
                    }
                });
            }
        }
 
        private static void UpdateSessionPopupCount(string typeName)
        {
            if (sessionPopupCountDict == null)
            {
                sessionPopupCountDict = new Dictionary<string, int>();
            }

            if (sessionPopupCountDict.ContainsKey(typeName))
            {
                sessionPopupCountDict[typeName]++;
            }
            else
            {
                sessionPopupCountDict.Add(typeName, 1);
            }
        }
        
        private static string GetLastPopTimeKey(string popupName)
        {
            return $"PopupType[{popupName}]LastPopTime";
        }

        private static string GetTodayPopCountKey(string popupName)
        {
            return $"PopupType[{popupName}]TodayPopCount";
        }

        public static int GetTodayPopCount(string popupName)
        {
            var lastTime = GetLastPopTime(popupName);

            DateTimeOffset storeOffset = DateTimeOffset.FromUnixTimeMilliseconds(lastTime);
            if (storeOffset.Day != DateTime.Now.Day || storeOffset.Month != DateTime.Now.Month
                                                    || storeOffset.Year != DateTime.Now.Year)
            {
                return 0;
            }

            var itemKey = GetTodayPopCountKey(popupName);
            
            return  Client.Storage.GetItem(itemKey, 0);
        }

        private static void UpdateTodayPopCount(string popupName)
        {
            int count = GetTodayPopCount(popupName);

            Client.Storage.SetItem(GetTodayPopCountKey(popupName), count + 1);
        }

        private static void UpdateLastPopTime(string popupName)
        {
            Client.Storage.SetItem(GetLastPopTimeKey(popupName), DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString());
        }

        private static long GetLastPopTime(string popupName)
        {
            var timeStamp = Convert.ToInt64(Client.Storage.GetItem(GetLastPopTimeKey(popupName), "0"));
            return timeStamp;
        }
    }
}