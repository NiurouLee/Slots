// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/17/20:12
// Ver : 1.0.0
// Description : Model.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class GuideModel:Model<GuideInfo>
    {
        public GuideModel()
            : base(ModelType.TYPE_GUIDE)
        {
            
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            // return base.FetchModalDataFromServer();

            var cGetGuide = new CGetGuide();
            var sGetGuide = await APIManagerGameModule.Instance.SendAsync<CGetGuide, SGetGuide>(cGetGuide);

            if (sGetGuide.ErrorCode == ErrorCode.Success)
            {
                UpdateModelData(sGetGuide.Response.GuideInfo);
            }
        }
         
        public Guide GetGuideByName(string guideName)
        {
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Guides.Count; i++)
                {
                    if ((modelData.Guides[i].Type == guideName || modelData.Guides[i].Type.Contains(guideName))
                        && !modelData.Guides[i].Completed)
                    {
                        return modelData.Guides[i];
                    }
                }
            }
            return null;
        }

        public Guide GetCurrentGuide()
        {
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Guides.Count; i++)
                {
                    if (!modelData.Guides[i].Completed)
                    {
                        return modelData.Guides[i];
                    }
                }
            }
            return null;
        }

        public bool IsGuideComplete(string guideName)
        {
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Guides.Count; i++)
                {
                    if (modelData.Guides[i].Type == guideName || modelData.Guides[i].Type.Contains(guideName))
                    {
                        return modelData.Guides[i].Completed;
                    }
                }
            }
            
            return true;
        }
    }
}