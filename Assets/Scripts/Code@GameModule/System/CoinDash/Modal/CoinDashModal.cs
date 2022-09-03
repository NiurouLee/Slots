// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/01/10:47
// Ver : 1.0.0
// Description : CoinDashModal.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class CoinDashModel:Model<CoinDashInfo>
    {
        private int activateIndex = 0;
        public CoinDashModel()
            : base(ModelType.TYPE_COIN_DASH)
        {
            
        }
        
        public float GetStartLeftTime()
        {
            if (IsInitialized())
            {
                if (modelData != null)
                {
                    return -XUtility.GetLeftTime((ulong) modelData.StartAt * 1000);
                }
            }
            return 0;
        }
        
        public float GetLeftTime()
        {
            if (IsInitialized())
            {
                if (modelData != null)
                {
                    return XUtility.GetLeftTime((ulong) (modelData.EndAt - 60) * 1000);
                }
            }
            return 0;
        }

        public bool IsOpen()
        {
            if (IsInitialized())
            {
                if (modelData != null)
                {
                    if (activateIndex >= modelData.Goods.Count)
                    {
                        return false;
                    }
                    
                    var leftTime = XUtility.GetLeftTime((ulong) (modelData.EndAt - 60) * 1000);
                    var startTime = XUtility.GetLeftTime((ulong) (modelData.StartAt) * 1000);

                    if (leftTime >= 0 &&
                        startTime <= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 确认活动开启的时候才生效
        /// </summary>
        /// <returns></returns>
        public int GetCoinDashItemCount()
        {
            return modelData.Goods.Count;
        }
        
        /// <summary>
        /// 确认活动开启的时候才生效
        /// </summary>
        /// <returns></returns>
        public CoinDashInfo.Types.Goods GetCoinDashItemInfo(int index)
        {
            return modelData.Goods[index];
        }

        public int GetActiveItemIndex()
        {
            if (modelData != null)
            {
                var count = modelData.Goods.Count;
                for (var i = 0; i < count; i++)
                {
                    if (!modelData.Goods[i].HasBought)
                        return i;
                }

                return count;
            }

            return 5;
        }

        public override void UpdateModelData(CoinDashInfo inModelData)
        {
            base.UpdateModelData(inModelData);
            activateIndex = GetActiveItemIndex();
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetCoinDashInfo coinDashInfo = new CGetCoinDashInfo();
            var handler =
                await APIManagerGameModule.Instance.SendAsync<CGetCoinDashInfo, SGetCoinDashInfo>(coinDashInfo);

            if (handler.ErrorCode == 0)
            {
                UpdateModelData(handler.Response.CoinDashInfo);
            }
            else
            {
                XDebug.LogError("GetDailyBonusResponseError:" + handler.ErrorInfo);
            }
        }
    }
}