//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-27 18:58
//  Ver : 1.0.0
//  Description : PiggyBankModel.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class PiggyBankModel: Model<PiggyBankInfo>
    {
        private ShopItemConfig _shopItemConfig;
        public ShopItemConfig ShopItemConfig => _shopItemConfig;
        public PiggyBankModel() : base(ModelType.TYPE_PIGGY_BANK)
        {
        }
        public override async Task FetchModalDataFromServerAsync()
        {
            await GetModalDataFromServer();
        }
        
        public async Task<SGetPiggyBank> GetModalDataFromServer()
        {
            CGetPiggyBank cGetPiggyBank = new CGetPiggyBank();
            var sGetPiggyBank =
                await APIManagerGameModule.Instance.SendAsync<CGetPiggyBank, SGetPiggyBank>(cGetPiggyBank);

            if (sGetPiggyBank.ErrorCode == 0)
            {
                UpdatePiggyData(sGetPiggyBank.Response);
                return sGetPiggyBank.Response;
            }
            else
            {
                XDebug.LogError("GetPiggyResponseError:" + sGetPiggyBank.ErrorInfo);
            }
            return null;
        }

        public void UpdatePiggyData(SGetPiggyBank sGetPiggyBank)
        {
            UpdateModelData(sGetPiggyBank.PbInfo);
            _shopItemConfig = sGetPiggyBank.PbPayItem;
        }
        

        public void UpdatePiggyBankSpin(PiggyBankResult piggyBankResult)
        {
            if (piggyBankResult != null)
            {
                modelData.MaxCoins = piggyBankResult.Max;
                modelData.CurrentCoins = piggyBankResult.Current;   
            }
        }

        public bool IsPiggyFull => modelData != null && modelData.CurrentCoins >= modelData.MaxCoins;
        public bool IsLocked => modelData != null && !modelData.Enable;
        public ulong CurrentCoins => modelData.CurrentCoins;
        public ulong LimitUserLevel => modelData.LimitUserLevel;
        public ulong FirstBonus => modelData.FirstBonus;
    }
}