//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 19:29
//  Ver : 1.0.0
//  Description : ElementExtraInfoProvider11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class ElementExtraInfoProvider11010: ElementExtraInfoProvider
    {
        public override Type GetElementClassType(uint id)
        {
            return typeof(Element11010);   
        }
        
        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11010.IsLinkElement(id))
            {
                return 50;
            }
            return 10;
        }
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11010.IsLinkElement(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (int) extraInfoObject["winRate"];
                    }
                }
            }
        }
    }
}