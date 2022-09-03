using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class VipController : LogicController
    {
        private VipModel _model;
        
        public VipController(Client client) : base(client)
        {
            
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetVipInfo);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (_model.LastTimeUpdateData == 0)
            {
                await _model.FetchModalDataFromServerAsync();
            }

            finishCallback?.Invoke();
        }
        
        protected override void Initialization()
        {
            base.Initialization();
            _model = new VipModel();
        }

        public uint GetVipLevel()
        {
            return _model.GetVipLevel();
        }

        public string GetVipName()
        {
            var levelConfig = _model.GetVipLevelConfig((int)GetVipLevel());
            return levelConfig.VipName;
        }
        
        public VipLevelConfig GetCurrentVipLevelConfig()
        {
            return _model.GetVipLevelConfig((int)GetVipLevel());
        }


        public VipLevelConfig GetVipLevelConfig(int vipLevel)
        {
            return _model.GetVipLevelConfig(vipLevel);
        }

        public ulong GetCurrentVipExp()
        {
            return _model.GetCurrentVipExp();
        }

        public ulong GetNextVipExp()
        {
            return _model.GetNextVipExp();
        }
        
        public ulong GetNowLevelNeedExp()
        {
            return _model.GetCurrentVipExp() + _model.GetNextVipExp();
        }
       
        public float GetProgress()
        {
            float progress = ((float) _model.GetCurrentVipExp() / GetNowLevelNeedExp());
            progress = Mathf.Clamp(progress, 0, 0.9999f);
            return progress;
        }

    }
}