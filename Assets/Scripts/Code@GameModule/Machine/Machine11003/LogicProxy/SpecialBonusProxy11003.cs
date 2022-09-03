// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/26/17:35
// Ver : 1.0.0
// Description : SpecialBonusProxy.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class SpecialBonusProxy11003 : SpecialBonusProxy
    {
        private ExtraState11003 _extraState11003;
        public SpecialBonusProxy11003(MachineContext machineContext)
            : base(machineContext)
        {
            
        }

        public override void SetUp()
        {
            _extraState11003 = machineContext.state.Get<ExtraState11003>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            var extraState = _extraState11003;
            
            if (!extraState.IsSuperFreeGame() && !machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                return true;
            }

            return false;
        }

        protected override async void HandleCustomLogic()
        {
            XDebug.Log("HandleCustomLogic");
            List<ElementContainer> coinElementContainers = null;
          
            if (machineContext.state.Get<BetState>().IsFeatureUnlocked(2))
            {
                var wheel = machineContext.view.Get<Wheel>("WheelBaseGame");

                coinElementContainers = wheel.GetElementMatchFilter((container) =>
                {
                    if (Constant11003.coinElement.Contains(container.sequenceElement.config.id))
                    {
                        return true;
                    }

                    return false;
                });
            }
            
            if (coinElementContainers != null && coinElementContainers.Count > 0)
            {
                XDebug.Log("ShowCoinElementCollectAnimation");
                if (_extraState11003.HasSpecialBonus()
                    || _extraState11003.NextIsSuperFreeGame()
                    || machineContext.state.Get<FreeSpinState>().NextIsFreeSpin
                    || machineContext.serviceProvider.IsBetListNeedUpdate())
                {
                    await ShowCoinElementCollectAnimationAsync(coinElementContainers);
                }
                else
                { 
                    ShowCoinElementCollectAnimation(coinElementContainers);
                }
            }
            
            if(_extraState11003.HasSpecialBonus())
            {
                XDebug.Log("HasSpecialBonus");
                StartWheelBonus();
            }
            else
            { 
                Proceed();
            }
        }

        public async void ShowCoinElementCollectAnimation(List<ElementContainer> elementContainers)
        {
            await ShowCoinElementCollectAnimationAsync(elementContainers);
        }

        public async Task ShowCoinElementCollectAnimationAsync(List<ElementContainer> elementContainers)
        {
            
            var titleView = machineContext.view.Get<BaseSpinMapTitleView>();

            bool audioPlayed = false;
         
            for (var i = 0; i < elementContainers.Count; i++)
            {
               var goldCoins = machineContext.assetProvider.InstantiateGameObject("GoldCoins");
             
               var pigTransform = titleView.GetPigIcon();
               
               goldCoins.transform.SetParent(titleView.transform.parent,false);
               var targetPosition = titleView.transform.parent.InverseTransformPoint(pigTransform.position);
               goldCoins.transform.position = elementContainers[i].transform.position;
               var sortingGroup = goldCoins.AddComponent<SortingGroup>();
               sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalUI");

               machineContext.WaitSeconds(0.2f, () =>
               {
                   if(!audioPlayed)
                        AudioUtil.Instance.PlayAudioFxOneShot("Bonus_Collect");
                   audioPlayed = true;
                   XUtility.FlyLocal(goldCoins.transform, goldCoins.transform.localPosition, targetPosition, 0, 0.35f, 0.43f,
                       () => { GameObject.Destroy(goldCoins); });
               });
            }
            
            await machineContext.WaitSeconds(0.55f);
            titleView.ShowCollectFx();
            titleView.UpdateCollectionProgress(true);
        }

        public void StartWheelBonus()
        {
            if (_extraState11003.HasSpecialBonus())
            {
                var titleView = machineContext.view.Get<BaseSpinMapTitleView>();
                machineContext.WaitSeconds(0.5f, () =>
                {
                    titleView.ShowUpgradeAnimation(() =>
                    {
                        XDebug.Log("StartWheelBonus");
                        var popUp = PopUpManager.Instance.ShowPopUp<WheelBonusStartPopUp>("UIWheelBonusStart11003");
                        popUp.BindStartAction(ShowWheelBonusPopUp);
                    });
                });
            }
        }
        
        protected void ShowWheelBonusPopUp()
        {
            var wheelBonus = PopUpManager.Instance.ShowPopUp<UIWheelBonus11003PopUp>("UIWheelBonus11003");
            wheelBonus.SetPopUpCloseAction(OnWheelBonusEndPopUp);
        }

        protected void OnWheelBonusEndPopUp()
        {
            var extraState = machineContext.state.Get<ExtraState11003>();

            bool hasWinBuffer = extraState.GetBonusWheelInfo().Choice == 0;
            
            string address = machineContext.assetProvider.GetAssetNameWithPrefix("UIWheelBonusFinish");
           
            if (hasWinBuffer)
            {
                address =  machineContext.assetProvider.GetAssetNameWithPrefix("UIWheelBonusFinishWithExtra");
            }
           
            var bonusFinish11003 = PopUpManager.Instance.ShowPopUp<WheelBonusFinish11003>(address);
             
            bonusFinish11003.SetPopUpCloseAction(OnWheelBonusCollected);
        }
        
        protected async void OnWheelBonusCollected()
        {
            machineContext.view.Get<BaseSpinMapTitleView>().UpdateCollectionProgress(false);  
      
            var mapView11003 = machineContext.view.Get<MapView11003>();
            await mapView11003.ShowIndicatorMoveAnimation();
            
            UpdateBonusWinToPanel();
        }

        protected void UpdateBonusWinToPanel()
        {
            // var extraState11003 = machineContext.state.Get<ExtraState11003>();
            // var wheelInfo = extraState11003.GetBonusWheelInfo();
            
            UpdateWinChipsToDisplayTotalWin();
           
            Proceed();
        }
    }
}