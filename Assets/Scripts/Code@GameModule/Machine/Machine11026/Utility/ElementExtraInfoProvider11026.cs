using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;

namespace GameModule
{
	public class ElementExtraInfoProvider11026 : ElementExtraInfoProvider
	{
		public ElementExtraInfoProvider11026()
		{

		}
		
		public override Type GetElementClassType(uint id)
        {
	        if (Constant11026.ListBonusAllElementIds.Contains(id))
            {
                return typeof(ElementCoin11026);
            }
	        return typeof(Element11026);   
        } 
		
        public override void ParseElementExtra(ElementConfig elementConfig, string extraInfo)
        {
            if (Constant11026.ListBonusAllElementIds.Contains(elementConfig.id))
            {
                var extraInfoObject = LitJson.JsonMapper.ToObject(extraInfo);
                elementConfig.extraInfo = new Dictionary<string, object>();
                if (extraInfoObject != null && extraInfoObject.IsObject)
                {
                    if (extraInfoObject.Keys.Contains("winRate"))
                    {
                        elementConfig.extraInfo["winRate"] = (ulong)(int) extraInfoObject["winRate"];
                    }
                }
            }
        }
        
		
        public override int GetElementSortingOffset(uint id)
        {
             if (Constant11026.ListBonusAllElementIds.Contains(id) || Constant11026.B01ElementId == id)
                return 90; 
             return 0;
        }
        
        public override int GetElementActiveSortingOffset(uint id)
        {
             if (Constant11026.ListBonusAllElementIds.Contains(id) || Constant11026.B01ElementId == id)
                return 250; 
             return 200;
        }
        public override (float, float, float) GetElementStaticGrayLayerPrams(uint id)
        {
            if (!Constant11026.ListBonusAllElementIds.Contains(id))
            {
                return (0.5f, 0.5f, 1);
            }

            return base.GetElementStaticGrayLayerPrams(id);
        }
        
        public override string GetElementBlinkSoundName(uint id, string name)
        {
            if (Constant11026.ListBonusAllElementIds.Contains(id))
            {
                return "J01_Blink";
            }
            
            return base.GetElementBlinkSoundName(id, name);
        }

        public override List<uint> GetElementBlinkVariantList(uint id)
        {
	        if (Constant11026.ListBonusAllElementIds.Contains(id))
	        {
		        return Constant11026.ListBonusAllElementIds;
	        }

	        return base.GetElementBlinkVariantList(id);
        }
        
	}
}