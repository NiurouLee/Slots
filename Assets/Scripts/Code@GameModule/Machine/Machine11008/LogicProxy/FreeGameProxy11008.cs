using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace GameModule{
    public class FreeGameProxy11008 : FreeGameProxy
    {
        public FreeGameProxy11008(MachineContext context) : base(context)
        {
        }
        protected override void HandleFreeReTriggerLogic()
        {
            HandleFreeReTriggerStart();
        }
        
        protected async void HandleFreeReTriggerStart()
        {
            if (!IsFromMachineSetup())
            {
                await ShowFreeSpinTriggerLineAnimation();
            }
            var assetName = "UIFreeGameExtraNotice" + machineContext.assetProvider.AssetsId;
            if (machineContext.assetProvider.GetAsset<GameObject>(assetName) != null)
            {
                var popUp = ShowFreeSpinReTriggeredPopup(assetName);
                popUp.SetPopUpCloseAction(() =>
                {
                    UpdateFreeSpinUIState(true, UseAverageBet());
                    HandleFreeReTriggerEnd();
                });
                popUp.SetExtraCount(freeSpinState.NewCount);
            }
            else
            {
                UpdateFreeSpinUIState(true, UseAverageBet());
                HandleFreeReTriggerEnd();
            }
        }
        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            //更新Bet到FreeSpin的TriggerBet
            //正常情况下FreeSpin的Bet和Base的Bet是一致的，但是有点关卡会使用平均BET
            //所以这里要将Bet在触发的时候设置成FreeSpinBet，保证jackpot等其他依赖BET的View上的数值计算正确
            //如果使用AverageBet，这里ControlPanel上面的会显示AverageBet的文字，不显示具体的数字，
            machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
            UpdateSpinUiViewTotalBet(false,false);
            UpdateFreeSpinUIState(true, UseAverageBet());
            controlPanel.ShowSpinButton(false);
            machineContext.state.Get<WheelsActiveState11008>().UpdateRunningWheelState(null);
            // machineContext.view.Get<BgView11008>().PlayBgViewAnim("Free");
            // machineContext.view.Get<WheelBaseGame11008>().PlayBgViewAnim("Free");
            await Task.CompletedTask;
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            RestoreBasePanelRes();
            machineContext.view.Get<BgView11008>().PlayBgViewAnim("Base");
            machineContext.view.Get<WheelBaseGame11008>().PlayBgViewAnim("Base");
            machineContext.view.Get<WheelFreeGame11008>().PlayBgViewAnim("Base");
            machineContext.state.Get<WheelsActiveState11008>().UpdateRunningWheelState(null);
            await Task.CompletedTask;
        }
        /// <summary>
        /// free结束 回到base恢复触发Free时的轮盘结果
        /// </summary>
        /// <returns></returns>
        private void RestoreBasePanelRes(){
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if(triggerPanels.count>0){
                for (int i = 0; i < triggerPanels.count; i++)
                {
                    machineContext.state.Get<WheelState>(i).UpdateWheelStateInfo(triggerPanels[i]);
                    var curWheel = machineContext.view.Get<Wheel>(i);
                    curWheel.ForceUpdateElementOnWheel();
                }
            }
        }

        public override void RecoverFreeSpinStateWhenRoomSetup()
        {
            // machineContext.view.Get<BgView11008>().PlayBgViewAnim("Free");
            // machineContext.view.Get<WheelBaseGame11008>().PlayBgViewAnim("Free");
            base.RecoverFreeSpinStateWhenRoomSetup();
        }

        protected override async Task ShowFreeGameFinishPopUp()
        {
            await ShowFreeGameFinishPopUp();
        }
        protected async Task ShowFreeGameFinishPopUp(string address = null)
        {
            if (address == null)
            {
                address = "UIFreeGameFinish" + machineContext.assetProvider.AssetsId;
            }

            if (machineContext.assetProvider.GetAsset<GameObject>(address) == null)
            {
                XDebug.LogError($"ShowFreeGameFinishPopUp:{address} is Not Exist" );    
                return;
            }

            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            var startPopUp = PopUpManager.Instance.ShowPopUp<FreeSpinFinishPopUp11008>(address);
            startPopUp.SetPopUpCloseAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });
            if(startPopUp.IsAutoClose()){
                await XUtility.WaitSeconds(3);
                startPopUp.Close();
            }
            await waitTask.Task;
        }
    }
}

