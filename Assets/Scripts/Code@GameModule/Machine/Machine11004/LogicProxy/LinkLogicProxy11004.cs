using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class LinkLogicProxy11004: LinkLogicProxy
    {
        private LinkWheelState11004 linkWheelState;
        public LinkLogicProxy11004(MachineContext context) : base(context)
        {
            linkWheelState = context.state.Get<LinkWheelState11004>();
        }

        protected override string GetLinkBeginAddress()
        {
            return "UIEpicRespinStart" + machineContext.assetProvider.AssetsId;;
        }

        protected override string GetLinkFinishAddress()
        {
            return "UIEpicRespinFinish" + machineContext.assetProvider.AssetsId;;
        }

        protected override async Task HandleLinkBeginCutSceneAnimation()
        {
            machineContext.state.Get<WheelsActiveState11004>().UpdateRunningWheelState(true,false);
            RecoverLogicState();
            
        }


        protected async override void RecoverLogicState()
        {
            ResetLinkWheels();
            UpdateLinkWheelLockElements();
        }


        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            bool isFreeSpin = freeSpinState.IsInFreeSpin;
            machineContext.state.Get<WheelsActiveState11004>().UpdateRunningWheelState(false,isFreeSpin);
            if (!isFreeSpin)
            {
                RestoreTriggerWheelElement();
            }
        }
        
        
       


        protected async override Task HandleLinkGameTrigger()
        {
            
        }


        protected async override Task HandleLinkGame()
        {
             await UpdateLinkWheelLockElements();
        }


        private async Task UpdateLinkWheelLockElements()
        {
            var items = machineContext.state.Get<ExtraState11004>().GetLinkData().Items;

            // List<Task> listTask = new List<Task>();
            
            foreach (var item in items)
            {
                int id = (int) item.PositionId;
                //var elementContainer = GetRunningElementContainer(id);
                if (!linkWheelState.IsRollLocked(id) && item.SymbolId>0)
                {
                    //elementContainer.transform.position = GetElementPosition(id);
                    var container = UpdateRunningElement(item.SymbolId, id,0,false);
                    linkWheelState.SetRollLockState(id, true);

                    container.PlayElementAnimation("Idle");
                    container.ShiftSortOrder(true);
                    // if (!IsFromMachineSetup() && !IsLinkTriggered())
                    // {
                    //     listTask.Add(PlayLinkBlinkAnim(container));
                    // }

                }
            }

            // if (listTask.Count > 0)
            // {
            //     await Task.WhenAll(listTask);
            // }
        }


         
        
       
        
        private void ResetLinkWheels()
        {
            var items = machineContext.state.Get<ExtraState11004>().GetLinkData().Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int id = (int) item.PositionId;
                UpdateRunningElement(Constant11004.NormalLinkElementId, id);
                linkWheelState.SetRollLockState(id, false);
                
            }
        }
        
        
       
        
        private Vector3 GetElementPosition(int rollIndex, int rowIndex = 0)
        {
            var wheel = machineContext.view.Get<IndependentWheel>();
            return wheel.GetRoll(rollIndex).GetVisibleContainer(rowIndex).transform.position;
        }


        protected async override Task HandleLinkReward()
        {

            if (!IsFromMachineSetup())
            {
                var linkTitleView = machineContext.view.Get<LinkTitleView11004>();
                var extraState = machineContext.state.Get<ExtraState11004>();

                await linkTitleView.StartWin();

                var linkData = extraState.GetLinkData();
                if (linkData.GrandJackpot > 0)
                {
                    await Constant11004.ShowJackpot(machineContext, this, 5, linkData.GrandJackpot, false);
                    var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(linkData.GrandJackpot);
                    await linkTitleView.AddWin((long)jackpotWin);
                }


                var dicLink = extraState.GetLinkItems();

                var endPos = linkTitleView.GetIntegralPos();
                foreach (var linkKV in dicLink)
                {
                    var itemLink = linkKV.Value;
                    int id = (int) itemLink.PositionId;
                    var container = GetRunningElementContainer(id);

                    ulong linkWin = 0;
                    if (itemLink.JackpotId > 0)
                    {
                        await Constant11004.ShowJackpot(machineContext, this, itemLink.JackpotId, itemLink.JackpotPay,
                            false);
                        linkWin = machineContext.state.Get<BetState>().GetPayWinChips(itemLink.JackpotPay);
                        AudioUtil.Instance.PlayAudioFx("Jackpot_Win");
                    }
                    else
                    {
                        linkWin = machineContext.state.Get<BetState>().GetPayWinChips(itemLink.WinRate);
                        AudioUtil.Instance.PlayAudioFx("J01_Win");
                    }

                    

                    container.PlayElementAnimationAsync("Win");
                    
                    var objFly = machineContext.assetProvider.InstantiateGameObject("Active_JFly", true);
                    objFly.transform.parent = machineContext.transform;
                    var startPos = container.transform.position;
                    objFly.transform.position = startPos;
                    await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f, Ease.Linear, machineContext);
                    machineContext.assetProvider.RecycleGameObject("Active_JFly", objFly);

                    await linkTitleView.AddWin((long)linkWin);

                }
            }

        }

        protected override async Task HandleLinkBeginPopup()
        {
            await AudioUtil.Instance.PlayAudioFxAsync("J01_Trigger");
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            await base.HandleLinkBeginPopup();
        }

        protected async override Task HandleLinkFinishPopup()
        {
            await base.HandleLinkFinishPopup();
            
            var linkTitleView = machineContext.view.Get<LinkTitleView11004>();
            long winNum = linkTitleView.GetWinNum();
            if (winNum == 0)
            {
                //重连需要恢复
                var extraState = machineContext.state.Get<ExtraState11004>();
                winNum = extraState.GetRespinTotalWin();
                 
                
                linkTitleView.StartWin();
                linkTitleView.RefreshWin(winNum);
            }
            
            await SendToBalance(winNum);
        }


        protected async Task SendToBalance(long jackpotWin)
        {
            AddWinChipsToControlPanel((ulong)jackpotWin,0,true,false);
            //AddWinChipsToControlPanel((ulong)jackpotWin);
            float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
            await machineContext.WaitSeconds(time);
        }



    }
}