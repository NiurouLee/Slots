using System.Collections.Generic;
using UnityEngine;
using System;
using GameModule;
using GameModule.PopUp;

namespace GameModule
{
    public class SpecialBonusProxy11013 : SpecialBonusProxy
    {
        public SpecialBonusProxy11013(MachineContext context)
            : base(context)
        {
        }


        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_STOP_SPECIAL_EFFECT);
        }

        protected override async void HandleCustomLogic()
        {
            var extraState = machineContext.state.Get<ExtraState11013>();

            await Constant11013.ClearStar(machineContext);
            if (!extraState.GetExtraInfo().Chosen)
            {
                await machineContext.view.Get<FeatureGame11013>().Open();
            }

            if (!extraState.GetExtraInfo().ChosenAgain)
            {
                var popUp = PopUpManager.Instance.ShowPopUp<UIFeatureGameFinish11013>(
                    $"UIFeaureGameFinish{machineContext.assetProvider.AssetsId}");

                popUp.SetPopUpCloseAction(() =>
                {
                    machineContext.view.Get<BaseCollect11013>().Clear();

                    base.HandleCustomLogic();
                });
            }
        }
    }
}