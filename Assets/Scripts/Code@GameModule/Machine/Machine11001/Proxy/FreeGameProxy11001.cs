// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/09/11:12
// Ver : 1.0.0
// Description : SpecialBonusProxy11001.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class FreeGameProxy11001 : FreeGameProxy
    {
        private ExtraState11001 _extraState11001;
        
        public FreeGameProxy11001(MachineContext context) : base(context)
        {
            
        }
        public override void SetUp()
        {
            base.SetUp();
            _extraState11001 = machineContext.state.Get<ExtraState11001>();
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            base.RegisterInterestedWaitEvent();
            waitEvents.Add(WaitEvent.WAIT_SPECIAL_EFFECT);
        }
        
        protected override async Task ShowFreeSpinTriggerLineAnimation()
        {
            if(machineContext.state.Get<BetState>().IsFeatureUnlocked(0))
                machineContext.view.Get<SuperFreeProgressView11001>().UpdateProgress(true);
            await base.ShowFreeSpinTriggerLineAnimation();
        }
        
        protected override async void HandleFreeReTriggerLogic()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
            base.HandleFreeReTriggerLogic();
        }
        
        public override bool UseAverageBet()
        {
            var bingoData = _extraState11001.GetBingoData(true);
            return  bingoData != null;
        }
        
        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            
                var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("TransitionAnimation");
                
                transitionAnimation.transform.SetParent(machineContext.transform);
               
                machineContext.WaitSeconds(0.5f, () =>
                {
                    var superFreeProgress = machineContext.view.Get<SuperFreeProgressView11001>();
                    superFreeProgress.transform.gameObject.SetActive(false);
                    machineContext.view.Get<BackGroundView11001>().ShowFreeBackground(true);
                    var bingoMapView = machineContext.view.Get<BingoMapView11001>();
                    bingoMapView.RefreshBingoItem(_extraState11001.GetBingoData(true));
                });
                AudioUtil.Instance.PlayAudioFx("Video2");
                await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionAnimation", machineContext);
                GameObject.Destroy(transitionAnimation);
                
                RecoverCustomFreeSpinState();

                await base.ShowFreeSpinStartCutSceneAnimation();
             
            await Task.CompletedTask;
        }
        
        

        protected override void RecoverCustomFreeSpinState()
        {
            var superFreeProgress = machineContext.view.Get<SuperFreeProgressView11001>();
            superFreeProgress.transform.gameObject.SetActive(false);
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("TransitionAnimation");
            
            transitionAnimation.transform.SetParent(machineContext.transform);
            AudioUtil.Instance.PlayAudioFx("Video2");
            
            machineContext.WaitSeconds(0.5f, () =>
            {
                var bingoMapView = machineContext.view.Get<BingoMapView11001>();
                var superFreeProgress = machineContext.view.Get<SuperFreeProgressView11001>();
                
                superFreeProgress.transform.gameObject.SetActive(true);
                superFreeProgress.UpdateProgress();
                
                machineContext.view.Get<BackGroundView11001>().ShowFreeBackground(false);

                RestoreBet();
                RestoreTriggerWheelElement();
                
                var superFreeProgressView11001 = machineContext.view.Get<SuperFreeProgressView11001>();
                superFreeProgressView11001.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
                
                bingoMapView.RefreshBingoItem(_extraState11001.GetNormalBingoData());
            });
            
            await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionAnimation", machineContext);
                
            GameObject.Destroy(transitionAnimation);
 
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }
        
        protected override async Task ShowFreeGameStartPopUp()
        {
            string addressName = machineContext.assetProvider.GetAssetNameWithPrefix("UIFreeGameStart");
            var bingoData = _extraState11001.GetBingoData(true);
            if (bingoData != null)
            {
                if (bingoData.IsCross)
                {
                    addressName =  machineContext.assetProvider.GetAssetNameWithPrefix("UIMegaBonusStart");
                }
                else
                {
                    addressName =  machineContext.assetProvider.GetAssetNameWithPrefix("UISuperBonusStart"); 
                }
            }
            
            await ShowFreeGameStartPopUp<FreeSpinStartPopUp>(addressName);
        }

        protected override async Task ShowFreeGameFinishPopUp()
        {
            string addressName = machineContext.assetProvider.GetAssetNameWithPrefix("UIFreeGameFinish");

            if (_extraState11001.IsMegaBonus())
            {
                addressName = machineContext.assetProvider.GetAssetNameWithPrefix("UIMegaBonusFinish");  
            }
            else if (_extraState11001.IsSuperBonus())
            {
                addressName =  machineContext.assetProvider.GetAssetNameWithPrefix("UISuperBonusFinish");
            }

            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>(addressName);
        }
    }
}