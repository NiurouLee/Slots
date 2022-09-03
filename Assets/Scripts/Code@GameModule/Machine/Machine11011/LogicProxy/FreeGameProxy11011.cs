//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-24 20:38
//  Ver : 1.0.0
//  Description : FreeGameProxy11010.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeGameProxy11011: FreeGameProxy
    {
        public FreeGameProxy11011(MachineContext context) : base(context)
        {
        }
        
        protected override void RecoverCustomFreeSpinState()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            machineContext.state.Get<WheelsActiveState11011>().UpdateFreeWheelState();
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("Video");
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            machineContext.view.Get<FeatureCutView11011>().PlayCutScreen();
            await machineContext.WaitSeconds(0.5f);
            machineContext.state.Get<WheelsActiveState11011>().UpdateFreeWheelState();
            await base.ShowFreeSpinStartCutSceneAnimation();
            await machineContext.WaitSeconds(1.5f);
            machineContext.view.Get<FeatureCutView11011>().ToggleVisible(false);
        }

        protected override Task ShowFreeSpinFinishCutSceneAnimation()
        {
            machineContext.state.Get<WheelsActiveState11011>().UpdateBaseWheelState();
            RestoreTriggerWheelElement();
            return base.ShowFreeSpinFinishCutSceneAnimation();
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
            base.HandleFreeReTriggerLogic();
        }
        
        protected override async Task ShowFreeGameFinishPopUp()
        {
            var totalTxt = machineContext.view.Get<FeatureView11011>(1).GetTotalTxt();
            var flyContainer = machineContext.assetProvider.InstantiateGameObject("FlyTxt",true);
            flyContainer.GetComponent<TextMesh>().text = totalTxt.text;
            var startPos = totalTxt.transform.position;
            flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
            flyContainer.transform.SetParent(machineContext.transform,false);
            var endPos = machineContext.view.Get<ControlPanel>().WinTextRefWorldPosition;
            await XUtility.FlyAsync(flyContainer.transform, startPos, endPos, 0, 0.5f);
            var totalWineffect = machineContext.assetProvider.InstantiateGameObject("TotaLWinEffetcsFree", true);
            totalWineffect.transform.SetParent(machineContext.view.Get<ControlPanel>().transform, false);
            totalWineffect.SetActive(true);
            machineContext.assetProvider.RecycleGameObject("FlyTxt",flyContainer);
            var winRate = machineContext.state.Get<FreeSpinState11011>().LastTotalWinRate;
            var totalWin = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
            AddWinChipsToControlPanel(totalWin, 0.6f);
            await machineContext.WaitSeconds(2f);
            machineContext.assetProvider.RecycleGameObject("TotaLWinEffetcsFree",totalWineffect);

            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>();
        }

        protected override void HandleFreeFinishLogic()
        {
            base.HandleFreeFinishLogic();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
        }
    }
}