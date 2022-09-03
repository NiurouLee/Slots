//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 15:29
//  Ver : 1.0.0
//  Description : BonusProxy11016.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class BonusProxy11016:BonusProxy
    {
        private int _smallIndex;
        public BonusProxy11016(MachineContext context)
            : base(context)
        {
            
        }

        protected override async void HandleCustomLogic()
        {
           // EventBus.Dispatch(new EventSystemWidgetActive(false));
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            var extraState = machineContext.state.Get<ExtraState11016>();
            var miniGameIds = extraState.MiniGameIds;
            var runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];

            _smallIndex = 0;
            List<SmallWheel11016> listSmallWheels = new List<SmallWheel11016>();
            for (int i = 0; i < miniGameIds.Count; i++)
            {
                var position = extraState.GetBonusWinLinePosition(i);
                var elementContainer = runningWheel.GetRoll((int) position.X).GetVisibleContainer((int) position.Y);
                var element = elementContainer.GetElement() as Element11016;
                if (!IsFromMachineSetup())
                {
                    element.ShiftMaskAndSortOrder(-element.ShiftOrder);
                }
                elementContainer.PlayElementAnimation("Idle");
                element = elementContainer.GetElement() as Element11016;
                elementContainer.EnableSortingGroup(false);
                var smallTransform = element.transform.GetChild(0);
                var smallWheel = GetSmallWheel(smallTransform, (int)miniGameIds[i]);
                smallWheel.ElementContainer = elementContainer;
                elementContainer.PlayElementAnimation("SlideIdle");
                smallWheel.ResetSpritesInsideMask();
                smallWheel.ActionSpinEnd += () =>
                {
                    elementContainer.EnableSortingGroup(true);
                    elementContainer.UpdateAnimationToStatic();
                };
                listSmallWheels.Add(smallWheel);
            }
            if (!IsFromMachineSetup())
            {
                AudioUtil.Instance.PauseMusic();
                AudioUtil.Instance.PlayAudioFx("J01_Trigger");
                var positions = extraState.GetBonusWinLinePositions();
                for (int i = 0; i < positions.Count; i++)
                {
                    var position = positions[i];
                    var elementContainer = runningWheel.GetRoll((int) position.X).GetVisibleContainer((int) position.Y);   
                    elementContainer.PlayElementAnimation("Trigger");   
                }
                
                for (int i = 0; i < listSmallWheels.Count; i++)
                {
                    listSmallWheels[i].ResetSpritesInsideMask();
                }
                
                await machineContext.WaitSeconds(2);
            }
            if (IsFromMachineSetup())
            {
                for (int i = 0; i < extraState.MiniProgress; i++)
                {
                    var smallWheel = listSmallWheels[i];
                    smallWheel.wheelState.UpdateWheelStateInfo(extraState.MiniPanels[i]);
                    smallWheel.ShowResult();
                    smallWheel.UpdateTotalWin();
                    smallWheel.ElementContainer.PlayElementAnimation("Win");
                }
            }
            while (!extraState.IsBonusFinish)
            {
                var gameId = extraState.CurrentGameId;
                var miniProgress = extraState.MiniProgress;
                var smallWheel = listSmallWheels[miniProgress];
                AudioUtil.Instance.FadeMusicTo(0.7f,1);
                machineContext.view.Get<SmallSlotPaytableView11016>().ShowPaytable(gameId);
                await machineContext.WaitSeconds(1f);
                AudioUtil.Instance.PlayAudioFx("J01_Reel", true);
                smallWheel.PlaySpinAnimation("Start");
                await extraState.SendBonusProcess(process =>
                {
                    process.GameResult.IsJackpotStatic = true;
                });
                smallWheel.wheelState.UpdateWheelStateInfo(extraState.CurrentGamePanel);
                await machineContext.WaitSeconds(2f);
                smallWheel.ShowResult();
                smallWheel.PlaySpinAnimation("End");
                await machineContext.WaitSeconds(0.3f);
                AudioUtil.Instance.StopAudioFx("J01_Reel");
                AudioUtil.Instance.PlayAudioFx("J01_ReelStop");
                await machineContext.WaitSeconds(0.1f);
                AudioUtil.Instance.PlayAudioFx("J01_ReelStop");
                await machineContext.WaitSeconds(0.1f);
                AudioUtil.Instance.PlayAudioFx("J01_ReelStop");
                await machineContext.WaitSeconds(0.5f);

                if (smallWheel.wheelState.HasNormalWinLine())
                {
                    var normalWinLines = smallWheel.wheelState.GetNormalWinLine();
                    smallWheel.ShowWinLines(normalWinLines, true);
                    await machineContext.WaitSeconds(1f);   
                    smallWheel.ShowWinLines(normalWinLines, false);
                }
                smallWheel.UpdateTotalWin();
                AudioUtil.Instance.PlayAudioFx("J01_Win");
                smallWheel.ElementContainer.PlayElementAnimation("Win");
                await machineContext.WaitSeconds(1f);
            }
            machineContext.view.Get<SmallSlotPaytableView11016>().ClosePaytable();
            await machineContext.WaitSeconds(1f);
            for (int i = 0; i < listSmallWheels.Count; i++)
            {
                var smallWheel = listSmallWheels[i];
                var wheelState = smallWheel.wheelState as SmallWheelState11016;
                var wheelWin = wheelState.TotalSmallWin;
                if (smallWheel.wheelState.GetJackpotWinLines().Count > 0)
                {
                    var task = new TaskCompletionSource<bool>();
                    AudioUtil.Instance.PlayAudioFx("Jackpot");
                    var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11016.GetJackpotAddress(wheelState.GameId-1));
                    view.SetJackpotWinNum(wheelWin);
                    view.SetPopUpCloseAction(() =>
                    {
                        task.SetResult(true);
                    });
                    await task.Task;   
                }

                AudioUtil.Instance.PlayAudioFx("J01_WinCollect");
                smallWheel.ElementContainer.PlayElementAnimation("Settlement_Win");
                AddWinChipsToControlPanel(wheelWin,0.5f,false,false);
                await machineContext.WaitSeconds(1f);
            }

            var taskPopup = new TaskCompletionSource<bool>();
            var winState = machineContext.state.Get<WinState>();
            AudioUtil.Instance.PauseMusic();
            AudioUtil.Instance.PlayAudioFx("J01_WinOpen");
            var popup = PopUpManager.Instance.ShowPopUp<BonusFinishPopUp>("UIBonusGameFinish11016");
            popup.UpdateTotalWin(winState.currentWin);
            popup.SetPopUpCloseAction(() =>
            {
                taskPopup.SetResult(true);
            });
            await taskPopup.Task;
            winState = machineContext.state.Get<WinState>();
            if (winState.winLevel>= (int)WinLevel.BigWin)
            {
                await WinEffectHelper.ShowBigWinEffectAsync(winState.winLevel, winState.currentWin,
                    machineContext);   
            }

            for (int i = 0; i < listSmallWheels.Count; i++)
            {
                var smallWheel = listSmallWheels[i];
                smallWheel.OnSpinEnd();
                smallWheel.InitializeWheel();
            }
           // EventBus.Dispatch(new EventSystemWidgetActive(true));
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            Proceed(); 
        }

        private SmallWheel11016 GetSmallWheel(Transform smallTransform, int gameId)
        {
            var smallWheel = new SmallWheel11016(smallTransform, _smallIndex, gameId);
            var wheelState = new SmallWheelState11016(machineContext.state, _smallIndex, gameId);
            smallWheel.Initialize(machineContext);
            smallWheel.BuildWheel<StepperRoll11016, ElementSupplier, WheelSpinningController<WheelAnimationController>>(wheelState);
            smallWheel.SetUpWinLineAnimationController<WinLineAnimationController>();
            smallWheel.wheelState.UpdateCurrentActiveSequence($"Bonus{gameId}Reels");
            smallWheel.InitializeWheel();
            smallWheel.ForceUpdateElementOnWheel();
            _smallIndex++;
            return smallWheel;
        }
    }
}