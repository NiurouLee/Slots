using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace GameModule{
    public class NextSpinPrepareProxy11301 : NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11301(MachineContext context) : base(context)
        {
        }
        public override void StartNextSpin()
        {
            Constant11301.IsSpining = true;
            base.StartNextSpin();
        }
        protected override async void HandleCommonLogic(){
            base.HandleCommonLogic();
            await WaitCallBack();
        }
        public async Task WaitCallBack(){
            await XUtility.WaitSeconds(0.1f);
            var extraState = machineContext.state.Get<ExtraState11301>();
            var res = extraState.IsMapFeature();
            XDebug.Log("IsMapFeature===:"+res);
            //是否恢复商城
            if(extraState.IsMapFeature() && !Constant11301.IsShowShop){
                
                machineContext.view.Get<TransitionView11301>().UpdateShopMaskShow(true);
                var shopPopup = PopUpManager.Instance.ShowPopUp<UIShopPopUp11301>($"UIShop{machineContext.assetProvider.AssetsId}");
                shopPopup.InitShopInfos();
            }
            Constant11301.IsShowMapFeature = false;
        }
    }
}

