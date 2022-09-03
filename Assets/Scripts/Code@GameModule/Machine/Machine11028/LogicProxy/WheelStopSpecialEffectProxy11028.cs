//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-12 15:54
//  Ver : 1.0.0
//  Description : WheelStopSpecialEffectProxy11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11028:WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11028(MachineContext machineContext) : base(machineContext)
        {
            
        }

        protected override async void HandleCustomLogic()
        {
            await machineContext.WaitSeconds(0.5f);
            var wheelState = machineContext.state.Get<WheelState>();
            var jackpotLines = wheelState.GetJackpotWinLines();
            for (int i = 0; i < jackpotLines.Count; i++)
            {
                if (i == 0)
                {
                    AudioUtil.Instance.StopAudioFx("J01_Shake");
                    AudioUtil.Instance.PlayAudioFx("J01_Shake");
                    machineContext.view.Get<BackgroundView11028>().PlayBackgroundAnimation("Shock");
                    await machineContext.WaitSeconds(0.5f);   
                }
                var jackpotWinLine = jackpotLines[i];
                var isNight = machineContext.state.Get<ExtraState11028>().IsJackpotNight(jackpotWinLine);
                var jackpotCount = machineContext.state.Get<ExtraState11028>().GetJackpotCount(jackpotWinLine);
                machineContext.view.Get<JackpotPotPanel11028>()
                    .PlayAnimation(jackpotWinLine.JackpotId > 5 ? "Night" : "Day", jackpotCount);
                BlinkJackpotLine(jackpotWinLine);
                await machineContext.WaitSeconds(2f);
                var taskPopup = new TaskCompletionSource<bool>();
                machineContext.AddWaitTask(taskPopup,null);
                machineContext.view.Get<JackpotPotPanel11028>().PlayAnimation();
                var popup = PopUpManager.Instance.ShowPopUp<RapidHitPopUp11028>(machineContext.assetProvider.GetAssetNameWithPrefix("UIRapidStart"));
                popup.InitializeJackpot(isNight,jackpotCount, jackpotWinLine.Pay);
                popup.SetPopUpCloseAction(async () =>
                {
                    var numWin = (ulong)machineContext.state.Get<BetState>().GetPayWinChips(jackpotWinLine.Pay);
                    AddWinChipsToControlPanel(numWin,1,true,false, machineContext.assetProvider.GetAssetNameWithPrefix("Symbol_SmallWin_"));
                    await machineContext.WaitSeconds(1f);
                    taskPopup.TrySetResult(true);
                });
                await taskPopup.Task;
            }
            Proceed();
        }
        
        protected void BlinkJackpotLine(WinLine jackpotWinLine)
        {
            for (var index = 0; index < jackpotWinLine.Positions.Count; index++)
            {
                var pos = jackpotWinLine.Positions[index];
                var container =  machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0].GetWinLineElementContainer((int) pos.X,(int) pos.Y);
                container.PlayElementAnimation("Win");
                container.ShiftSortOrder(true);
                AudioUtil.Instance.StopAudioFx("J01_Trigger");
                AudioUtil.Instance.PlayAudioFx("J01_Trigger");
            }
        }
    }
}