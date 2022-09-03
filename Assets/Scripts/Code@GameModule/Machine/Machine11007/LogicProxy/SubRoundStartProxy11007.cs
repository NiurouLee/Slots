//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-19 16:19
//  Ver : 1.0.0
//  Description : SubRoundStartProxy11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class SubRoundStartProxy11007: SubRoundStartProxy
    {
        public SubRoundStartProxy11007(MachineContext context)
            : base(context)
        {
        }
        
        protected override async void Proceed()
        {
            var bonusState = machineContext.state.Get<BonusWheelState11007>();
            if (bonusState.IsBonusSpinSameWin())
            {
                machineContext.view.Get<LinkBonusTitle11007>().ShowBonusSameSpinStartAnim();
            }
            //处理倍数玩法 
            if (bonusState.IsBonusMultiWin() || bonusState.IsBonusTwoWild())
            {
                TaskCompletionSource<bool> taskSpinResult = new TaskCompletionSource<bool>();
                bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
                machineContext.serviceProvider.GetSpinResult(machineContext.state.Get<BetState>().totalBet,isAutoSpin,machineContext,async (sSpin) =>
                {
                    machineContext.state.UpdateMachineStateOnSpinResultReceived(sSpin);
                    if (bonusState.IsBonusMultiWin())
                    {
                        var multiply = bonusState.GetBonusMultiply();
                        if (multiply>0 && !bonusState.HasPlayMultiAnim)
                        {
                            GameObject go = machineContext.assetProvider.InstantiateGameObject(machineContext.assetProvider.GetAssetNameWithPrefix("UIRespinBonusBetNotice"));
                            BonusMultiPopUp11007 multiPopup = new BonusMultiPopUp11007(go.transform);
                            multiPopup.transform.SetParent(machineContext.MachinePopUpCanvasTransform, false);
                            multiPopup.transform.localPosition = Vector3.zero;
                            multiPopup.transform.SetAsLastSibling();
                            await multiPopup.InitializeWith(machineContext,(int)multiply, bonusState.HasPlayMultiAnim);
                            GameObject.Destroy(multiPopup.transform.gameObject);
                            bonusState.HasPlayMultiAnim = true;
                            await machineContext.WaitSeconds(1f);   
                        }
                        HandleToNextStep();
                        await machineContext.WaitSeconds(0.1f);
                        machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SERVER_SPIN_DATA_RECEIVED);
                        taskSpinResult.SetResult(true);   
                    }
                    else
                    {
                        var transform = machineContext.transform.Find("Wheels/WheelRespinBonus");
                        var animator0 = transform.Find("TwoWild/Active_WhellEffets_0").GetComponent<Animator>();
                        var animator1 = transform.Find("TwoWild/Active_WhellEffets_2").GetComponent<Animator>();
                        if (animator0.gameObject.activeInHierarchy)
                        {
                            HandleToNextStep();
                            await machineContext.WaitSeconds(0.1f);
                            machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SERVER_SPIN_DATA_RECEIVED);
                            taskSpinResult.SetResult(true);  
                        }
                        else
                        {
                            HandleToNextStep();
                            await machineContext.WaitSeconds(0.8f);
                            animator0.gameObject.SetActive(true);
                            animator1.gameObject.SetActive(true);
                            AudioUtil.Instance.PlayAudioFx("Respin_FullWild");
                            XUtility.PlayAnimation(animator0,"Active_WhellEffets", () =>
                            {
                                animator0.gameObject.SetActive(false);
                            });
                            XUtility.PlayAnimation(animator1,"Active_WhellEffets", () =>
                            {
                                animator1.gameObject.SetActive(false);
                            });
                            await machineContext.WaitSeconds(0.5f);
                            bonusState.GetWorldPosition(0);
                            var elementTrans0 = transform.Find("TwoWild/Static_W05_0");
                            var elementTrans2 = transform.Find("TwoWild/Static_W05_2");
                            elementTrans0.transform.position = bonusState.GetWorldPosition(0);
                            elementTrans2.transform.position = bonusState.GetWorldPosition(1);
                            elementTrans0.gameObject.SetActive(true);
                            elementTrans2.gameObject.SetActive(true);
                            
                            await machineContext.WaitSeconds(0.1f);
                            machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SERVER_SPIN_DATA_RECEIVED);
                            taskSpinResult.SetResult(true);  
                        }
                    }
                });
                await taskSpinResult.Task;
            }
            else
            {
                base.Proceed();
            }
        }
        
        protected override void PlayBgMusic()
        {
            if (machineContext.state.Get<ReSpinState>().NextIsReSpin)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetLinkBackgroundMusicName());
            }
            else if (machineContext.state.Get<WheelsActiveState11007>().IsBonusWheel)
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBonusBackgroundMusicName());
            }
            else
            {
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
            }
        }
    }
}