//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-13 16:08
//  Ver : 1.0.0
//  Description : ReSpinLogicProxy11007.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class LinkLogicProxy11007: LinkLogicProxy
    {
        private Animator animatorTransition;
        private SpriteRenderer spriteBonusBg;
        public LinkLogicProxy11007(MachineContext context)
            : base(context)
        {
            strLinkTriggerSound = "J01_Trigger";
            animatorTransition = context.transform.Find($"Transition{context.assetProvider.AssetsId}").GetComponent<Animator>();
        }

        protected override bool CheckIsTriggerElement(ElementContainer container)
        {
            return Constant11007.IsLinkElement(container.sequenceElement.config.id);
        }
        protected override int GetElementTriggerOffsetRow()
        {
            return 1;
        }
        protected override void StopBackgroundMusic()
        {
            if (!machineContext.state.Get<WheelsActiveState11007>().IsBonusWheel)
            {
                base.StopBackgroundMusic();
            }
        }


        protected override Task HandleReSpinStartLogic()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            return base.HandleReSpinStartLogic();
        }

        protected override async  Task HandleLinkBeginCutSceneAnimation()
        {
            animatorTransition.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorTransition, "Transition11007");
            AudioUtil.Instance.PlayAudioFx("Video1");
            await machineContext.WaitSeconds(0.8f);
        }
        
        protected override async Task HandleLinkGame()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            if (IsLinkSpinFinished())
            {
                machineContext.view.Get<LinkCounter11007>().Hide(IsFromMachineSetup());
            }
            var wheelActiveState = machineContext.state.Get<WheelsActiveState11007>();
            Wheel wheelRemove=null;
            
            //改变Link图标锁定状态
            var hasNewLock = UpdateLinkWheelLockState();
            
            //如果当前是Base轮盘，切换到Link轮盘

            if (wheelActiveState.IsNormalWheel)
            {
                wheelRemove = wheelActiveState.GetRunningWheel()[0];
                machineContext.view.Get<LinkCounter11007>().Show();
                machineContext.state.Get<WheelsActiveState11007>().UpdateLinkWheelState();
            }
            //更新Link图标锁定状态到轮盘
            if (wheelActiveState.IsLinkWheel)
            {
                machineContext.view.Get<ControlPanel>().ShowSpinButton(false);   
            }
            UpdateLinkWheelLockElements();
            UpdateLinkCount(machineContext.state.Get<ReSpinState>().ReSpinCount);
            await PlayWheelChangeAnim(wheelActiveState.GetRunningWheel()[0], wheelRemove);
            if (animatorTransition.gameObject.activeSelf)
            {
                await machineContext.WaitSeconds(1.5f);  
                animatorTransition.gameObject.SetActive(false);
            }
            
            //触发时候直接触发Grand
            if (hasNewLock && IsTriggerGrand())
            {
                AudioUtil.Instance.PlayAudioFx("Grand");
                var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11007.GetGrandAddress(machineContext.assetProvider.AssetsId));
                view.SetJackpotWinNum(GetGrandJackpotWin());
                view.SetPopUpCloseAction(() =>
                {
                    view.Close();
                });   
                await machineContext.WaitSeconds(3.4f);
                await view.OnClose();
                ForceUpdateWinChipsToDisplayTotalWin(1);
                await machineContext.WaitSeconds(1.1f);  
            }
            await machineContext.WaitSeconds(hasNewLock?1.5f:0f);
            UpdateLinkCount(machineContext.state.Get<ReSpinState>().ReSpinCount - 1);
        }

        private void UpdateLinkCount(uint respinCount)
        {
            if (respinCount<4)
            {
                machineContext.view.Get<LinkCounter11007>().UpdateLinkCount(respinCount);      
            }
        }

        protected override async Task HandleLinkReward()
        {
            var wheelsActive = machineContext.state.Get<WheelsActiveState11007>();
            //Link图标 Bonus玩法是否已结束，是否需要切换回Link界面
            await HandleChangeToLinkWheel();
            
            //Link图标结算

            var extraState = machineContext.state.Get<ExtraState11007>();
            if (wheelsActive.IsLinkWheel)
            {
                while (!ReferenceEquals(extraState.CurrentItem, null) && !extraState.CurrentItem.Finished)
                {
                    StopBackgroundMusic();
                    var item = extraState.CurrentItem;
                    XDebug.Log($"[Link Game]->WheelStateItemId:{machineContext.state.Get<BonusWheelState11007>().CurrentItemId} ExtraDataItemId:{extraState.CurrentItemId}");
                    if ( Constant11007.IsCoinElement(item.SymbolId) || Constant11007.IsJackpotElement(item.SymbolId))
                    {
                        machineContext.state.Get<BonusWheelState11007>().CurrentItemId = item.Id;
                        TaskCompletionSource<bool> taskClaim = GetWaitTask();
                        HandleMoveToNextItem(taskClaim, item.SymbolId, item.Id);
                        await taskClaim.Task;
                    }
                    else
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11007>().GetRunningWheel()[0];
                        var lockRoll = wheel.GetRoll((int)item.Id);
                        AudioUtil.Instance.PlayAudioFx("J01_Win2");
                        lockRoll.GetVisibleContainer(0).PlayElementAnimation("Collect");
                        TaskCompletionSource<bool> taskClaim = GetWaitTask();
                        if (!item.Started)
                        {
                            HandleMoveToNextItem(taskClaim, item.SymbolId, item.Id);
                            await taskClaim.Task;   
                        }
                        await machineContext.WaitSeconds(4);
                        lockRoll.GetVisibleContainer(0).UpdateAnimationToStatic();
                        lockRoll.GetVisibleContainer(0).GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
                        var wheelActiveState = machineContext.state.Get<WheelsActiveState11007>();
                        machineContext.view.Get<JackPotPanel>().Hide();
                        var reelId = item.SymbolId - Constant11007.ELEMENT_RESPIN_SAME_WIN + 1;
                        machineContext.state.Get<BonusWheelState11007>().SetBonusState(item.Id, reelId, item.SymbolId);
                        var wheelRemove = wheelActiveState.GetRunningWheel()[0];
                        machineContext.state.Get<BonusWheelState11007>().ResetSpinResultSequenceElement();
                        machineContext.state.Get<WheelsActiveState11007>().UpdateBonusWheelState();
                        machineContext.view.Get<LinkBonusTitle11007>().ShowSpinInLinkTitle(item.SymbolId);
                        wheel = wheelActiveState.GetRunningWheel()[0];
                        spriteBonusBg = machineContext.transform.Find($"Background/{Constant11007.DictBgName[(int)item.SymbolId]}").GetComponent<SpriteRenderer>();
                        spriteBonusBg.color = new Color(1, 1, 1, 0);
                        spriteBonusBg.gameObject.SetActive(true);
                        FadeAllSpriteRender(new[] {spriteBonusBg}, 0, 1);
                        await PlayWheelChangeAnim(wheel, wheelRemove);
                        var position = wheel.GetRoll(0).GetVisibleContainer(3).transform.position;
                        machineContext.state.Get<BonusWheelState11007>().SetWorldPosition(0,position);
                        position = wheel.GetRoll(2).GetVisibleContainer(3).transform.position;
                        machineContext.state.Get<BonusWheelState11007>().SetWorldPosition(1,position);
                        break;
                    }
                }   
            }
            XDebug.Log($"[Link Game]->WheelStateItemId:{machineContext.state.Get<BonusWheelState11007>().CurrentItemId} ExtraDataItemId:{extraState.CurrentItemId} IsBonusWheel:{wheelsActive.IsBonusWheel}");
        }

        private async Task HandleChangeToLinkWheel()
        {       
            var extraState = machineContext.state.Get<ExtraState11007>();
            var bonusWheelState = machineContext.state.Get<BonusWheelState11007>();
            var wheelActiveState = machineContext.state.Get<WheelsActiveState11007>();
            //Bonus过程中，Bonus和Extra记录的ItemId不相同，说明当前Link图标Bonus玩法完成，需要回到Link界面，开始继续结算过程
            if (wheelActiveState.IsBonusWheel && bonusWheelState.CurrentItemId != extraState.CurrentItemId)
            {
                var wheelRemove = wheelActiveState.GetRunningWheel()[0];
                machineContext.view.Get<JackPotPanel>().Show();
                machineContext.state.Get<WheelsActiveState11007>().UpdateLinkWheelState();
                machineContext.view.Get<LinkBonusTitle11007>().ToggleFoxinessTitle(false);
                if (spriteBonusBg)
                {
                    FadeAllSpriteRender(new[] {spriteBonusBg}, 1, 0);   
                }
                await PlayWheelChangeAnim(wheelActiveState.GetRunningWheel()[0], wheelRemove);
                if (spriteBonusBg)
                {
                
                    ResetAllSpritesAlpha(new[] {spriteBonusBg});
                    spriteBonusBg.gameObject.SetActive(false);
                }
            }
        }
        private void HandleMoveToNextItem(TaskCompletionSource<bool> task, uint elementId, uint id)
        {
            bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
            machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet,isAutoSpin,machineContext,async (sSpin) =>
            {
                //先刷新数据，再结算
                machineContext.state.UpdateMachineStateOnSpinResultReceived(sSpin);
                //--
                if (Constant11007.IsCoinElement(elementId) || Constant11007.IsJackpotElement(elementId))
                {
                    var wheel = machineContext.state.Get<WheelsActiveState11007>().GetRunningWheel()[0];
                    var itemWin = machineContext.state.Get<BonusWheelState11007>().GetCoinElementWin();
                    if (Constant11007.IsJackpotElement(elementId))
                    {
                        var audioSource = AudioUtil.Instance.PlayAudioFx("Jackpot");
                        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(Constant11007.GetJackpotAddress(elementId,machineContext.assetProvider.AssetsId));
                        view.SetJackpotWinNum(itemWin);
                        view.SetPopUpCloseAction(() =>
                        {
                            view.Close();
                        });   
                        await machineContext.WaitSeconds(4f);
                        await view.OnClose();
                    }
                    var lockRoll = wheel.GetRoll((int)id);
                    AudioUtil.Instance.PlayAudioFx("J01_Win1");
                    lockRoll.GetVisibleContainer(0).PlayElementAnimation("Collect");
                    var totalWineffect = machineContext.assetProvider.InstantiateGameObject("TotaLWinEffetcs", true);
                    totalWineffect.transform.SetParent(machineContext.view.Get<ControlPanel>().transform, false);
                    totalWineffect.SetActive(true);
                    ForceUpdateWinChipsToDisplayTotalWin(1);
                    await machineContext.WaitSeconds(1f);   
                    lockRoll.GetVisibleContainer(0).UpdateAnimationToStatic();
                    lockRoll.GetVisibleContainer(0).GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
                    machineContext.assetProvider.RecycleGameObject("TotaLWinEffetcs",totalWineffect);
                    await machineContext.WaitSeconds(0.1f);
                }
                SetAndRemoveTask(task);
            });  
        }

        protected override async Task HandleLinkFinishPopup()
        {
            var task = GetWaitTask();
            var rewardLinkPopup = PopUpManager.Instance.ShowPopUp<ReSpinFinishPopUp11007>(GetLinkFinishAddress());
            rewardLinkPopup.SetPopUpCloseAction(() =>
            {
                SetAndRemoveTask(task);
            });
            await task.Task;
        }

        protected override async Task HandleLinkFinishCutSceneAnimation()
        {
            machineContext.state.Get<BonusWheelState11007>().ResetBonusState();
            machineContext.state.Get<LinkWheelState11007>().ResetLinkWheelState();
            var wheelActiveState = machineContext.state.Get<WheelsActiveState11007>();
            var wheelRemove = wheelActiveState.GetRunningWheel()[0];
            wheelActiveState.UpdateNormalWheelState();
            await PlayWheelChangeAnim(wheelActiveState.GetRunningWheel()[0], wheelRemove);
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
            machineContext.view.Get<JackPotPanel>().Show();
            machineContext.state.Get<WheelState>().ForceUpdateWinLineState();
            await base.HandleLinkFinishCutSceneAnimation();
        }

        private bool UpdateLinkWheelLockState()
        {
            bool hasNewLock = false;
            var items = machineContext.state.Get<ExtraState11007>().Items;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                int id = (int)item.Id;
                var wheelState = machineContext.state.Get<LinkWheelState11007>();
                if (!wheelState.IsRollLocked(id))
                {
                    hasNewLock = true;
                    wheelState.SetRollLockState(id, true);
                }
            }
            return hasNewLock;
        }

        private void UpdateLinkWheelLockElements()
        {
            var wheelsActive = machineContext.state.Get<WheelsActiveState11007>();
            if (wheelsActive.IsLinkWheel)
            {
                var linkWheelState = machineContext.state.Get<LinkWheelState11007>();
                var items = machineContext.state.Get<ExtraState11007>().Items;
                for (int i = 0; i < items.count; i++)
                {
                    var item = items[i];
                    int id = (int) item.Id;
                    if (linkWheelState.IsRollLocked(id))
                    {
                        var wheel = wheelsActive.GetRunningWheel()[0];
                        var lockRoll = wheel.GetRoll(id);
                        lockRoll.GetVisibleContainer(0).UpdateElement(GetSequenceElement(item.SymbolId));
                        lockRoll.GetVisibleContainer(0).GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
                    }
                }

                if (IsFromMachineSetup() || IsLinkTriggered())
                {
                    for (int i = 0; i < linkWheelState.rollCount; i++)
                    {
                        if (!linkWheelState.IsRollLocked(i))
                        {
                            if (!Constant11007.IsNormalElementId(GetRunningElementId(i)))
                            {
                                UpdateRunningElement(Constant11007.NextNormalElementId(), i);
                            }
                        }
                    }   
                }
            }
        }

        //下一次不是Respin，同时Items数目大于0 OR Items等于最大数目
        protected override bool IsLinkSpinFinished()
        {
            var extraState = machineContext.state.Get<ExtraState11007>();
            return (!NextIsLinkSpin() && extraState.Items.Count > 0 || IsTriggerGrand()) && !IsLinkSettled();
        }

        public override bool NeedSettle()
        {
            return base.NeedSettle() && machineContext.state.Get<ExtraState11007>().IsLinkRewardFinish();
        }

        private bool IsLinkSettled()
        {
            return machineContext.state.Get<ExtraState11007>().IsClaimFinished();
        }

        protected override bool IsTriggerGrand()
        {
            return !IsFromMachineSetup() && machineContext.state.Get<ExtraState11007>().IsTriggerGrand();
        }
        
        

        private ulong GetGrandJackpotWin()
        {
            return machineContext.state.Get<ExtraState11007>().TotalWin;
        }

        private async Task PlayWheelChangeAnim(Wheel wheelAdd, Wheel wheelRemove)
        {
            if (!ReferenceEquals(wheelAdd, null) && !ReferenceEquals(wheelRemove, null))
            {
                wheelAdd.SetActive(false);
                
                SpriteRenderer[] spritesAddChildren = wheelAdd.transform.gameObject.GetComponentsInChildren<SpriteRenderer>();
                FadeAllSpriteRender(spritesAddChildren, 0, 1);
                MeshRenderer[] txtAddChildren = wheelAdd.transform.gameObject.GetComponentsInChildren<MeshRenderer>();
                FadeAllMeshRender(txtAddChildren,0, 1);
                wheelRemove.SetActive(true);

                await XUtility.WaitNFrame(1);
                wheelAdd.SetActive(true);
                SpriteRenderer[] spritesRemoveChildren = wheelRemove.transform.gameObject.GetComponentsInChildren<SpriteRenderer>();
                FadeAllSpriteRender(spritesRemoveChildren, 1, 0,0.3f);
                MeshRenderer[] txtRemoveChildren = wheelRemove.transform.gameObject.GetComponentsInChildren<MeshRenderer>();
                FadeAllMeshRender(txtRemoveChildren,1, 0,0.3f);
                await machineContext.WaitSeconds(0.6f);
                wheelRemove.SetActive(false);

                ResetAllSpritesAlpha(spritesAddChildren);
                ResetAllSpritesAlpha(spritesRemoveChildren);
                ResetAllMeshRenderAlpha(txtAddChildren);
                ResetAllMeshRenderAlpha(txtRemoveChildren);
            }
        }
        
        private void FadeAllSpriteRender(SpriteRenderer[] spriteRenderers, float start, float end, float duration=0.5f)
        {
            for(var i=0; i<spriteRenderers.Length; i++)
            {
                var child = spriteRenderers[i];
                child.DOFade(start, 0);
                child.DOFade(end, duration);
            }
        }
        private void FadeAllMeshRender(MeshRenderer[] meshRenderers, float start, float end, float duration=0.5f)
        {
            for(var i=0; i<meshRenderers.Length; i++)
            {
                var child = meshRenderers[i];
                child.material.DOFade(start, 0);
                child.material.DOFade(end, duration);
            }
        }

        private void ResetAllSpritesAlpha(SpriteRenderer[] spriteRenderers)
        {
            for(var i=0; i<spriteRenderers.Length; i++)
            {
                var child = spriteRenderers[i];
                child.DOFade(1, 0);
            }
        }
        private void ResetAllMeshRenderAlpha(MeshRenderer[] meshRenderers)
        {
            for(var i=0; i<meshRenderers.Length; i++)
            {
                var child = meshRenderers[i];
                child.material.DOFade(1, 0);
            }
        }
        protected override float GetLinkBeginPopupDuration()
        {
            return 2.5f;
        }
    }
}