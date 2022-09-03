using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class FreeGameProxy11003 : FreeGameProxy
    {
        private ExtraState11003 _extraState11003;

        public FreeGameProxy11003(MachineContext context) : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            _extraState11003 = machineContext.state.Get<ExtraState11003>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return freeSpinState.IsInFreeSpin || freeSpinState.NextIsFreeSpin || freeSpinState.IsTriggerFreeSpin ||
                   _extraState11003.IsSuperFreeGame() || _extraState11003.NextIsSuperFreeGame();
        }
        
        public override bool NeedSettle()
        {
            if (freeSpinState.FreeNeedSettle)
                return true;
            
            return _extraState11003.IsSuperFreeGameNeedSettle();
        }

        public override bool IsMatchCondition()
        {
            return !freeSpinState.NextIsFreeSpin && !_extraState11003.NextIsSuperFreeGame();
        }

        public override bool UseAverageBet()
        {
            if (_extraState11003.NextIsSuperFreeGame())
            {
                return true;
            }
            return false;
        }

        public override bool IsFreeSpinTriggered()
        {
            if (_extraState11003.IsSuperFreeGameNeedSettle())
            {
                return false;
            }
            
            if (_extraState11003.NextIsSuperFreeGame())
            {
                var superFreeSpinInfo = _extraState11003.GetSuperFreeSpinInfo();
                return superFreeSpinInfo.Total == superFreeSpinInfo.Left;
            }

            return freeSpinState.IsTriggerFreeSpin;
        }

        public override bool NextSpinIsFreeSpin()
        {
            if (_extraState11003.NextIsSuperFreeGame())
            {
                return true;
            }

            if (_extraState11003.IsSuperFreeGameNeedSettle())
            {
                return false;
            }

            return freeSpinState.NextIsFreeSpin;
        }

        public override bool IsFreeSpinReTriggered()
        {
            if (_extraState11003.IsSuperFreeGame())
            {
                return false;
            }

            return freeSpinState.NewCount > 0;
        }
        
        protected override ulong GetFreeSpinBet()
        {
            if (_extraState11003.NextIsSuperFreeGame())
            {
                return _extraState11003.GetSuperFreeSpinInfo().Bet;
            }
            return freeSpinState.freeSpinBet;
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            try
            {
                var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("TransitionAnimation");

                var sortingGroup = transitionAnimation.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                sortingGroup.sortingOrder = 100;
 
                transitionAnimation.transform.SetParent(machineContext.transform);

                machineContext.WaitSeconds(1.7f, () =>
                {
                    var extraState = machineContext.state.Get<ExtraState11003>();
                    if (extraState.NextIsSuperFreeGame())
                    {
                        machineContext.view.Get<BackgroundView11003>().OpenSuperFree();
                    }
                    else
                    {
                        machineContext.view.Get<BackgroundView11003>().OpenFree();
                    }

                    RecoverCustomFreeSpinState();
                });

                AudioUtil.Instance.PlayAudioFx("Free_Transitions");
                await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionAnimation",
                    machineContext);

                GameObject.Destroy(transitionAnimation);
  
                await base.ShowFreeSpinStartCutSceneAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void RecoverCustomFreeSpinState()
        {
            var freeWheel = _extraState11003.GetSuperFreeGameRunningWheel();

            if (freeWheel != null)
            {
                machineContext.state.Get<WheelsActiveState11003>()
                    .UpdateRunningWheel(new List<string>() {freeWheel.wheelName});
            }
            else
            {
                machineContext.view.Get<FreeSpinTitleView>().UpdateContent();
                machineContext.state.Get<WheelsActiveState11003>()
                    .UpdateRunningWheel(new List<string>() {"WheelFreeGame", "WheelFreeJackpot"});
            }
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("TransitionAnimation");

            var sortingGroup = transitionAnimation.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
            sortingGroup.sortingOrder = 100;

            transitionAnimation.transform.SetParent(machineContext.transform);
            
            UpdateFreeSpinUIState(true,_extraState11003.LastIsSuperFreeGame());
            
            machineContext.WaitSeconds(1.7f, () =>
            {
                RestoreTriggerWheelElement();
            
                machineContext.view.Get<BackgroundView11003>().OpenBase();
                
                machineContext.state.Get<WheelsActiveState11003>()
                    .UpdateRunningWheel(new List<string>() {"WheelBaseGame", "WheelBaseJackpot"});

                machineContext.view.Get<BaseSpinMapTitleView>().LockFeatureUI(!machineContext.state.Get<BetState>().IsFeatureUnlocked(2),false,true);

                if (_extraState11003.LastIsSuperFreeGame())
                {
                    machineContext.view.Get<JackpotPanel11003>().Show();
                    machineContext.view.Get<BaseSpinMapTitleView>().UpdateCollectionProgress(false);
                }
                
                machineContext.view.Get<JackpotPanel11003>().UpdateJackpotLockState(false,true);
            });
             
            AudioUtil.Instance.PlayAudioFx("Free_Transitions");
            
            await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionAnimation",
                machineContext);

            GameObject.Destroy(transitionAnimation);

            if (_extraState11003.LastIsSuperFreeGame())
            {
                var mapView = machineContext.view.Get<MapView11003>();

                await mapView.ShowIndicatorMoveAnimation();
                
                await base.ShowFreeSpinFinishCutSceneAnimation();
            }
            else
            {
                await base.ShowFreeSpinFinishCutSceneAnimation();
            }
        }
        
        protected override async Task ShowFreeSpinTriggerLineAnimation()
        {
            if (_extraState11003.NextIsSuperFreeGame())
            {
                return;
            }
            
            await base.ShowFreeSpinTriggerLineAnimation();
        }

        protected override async Task ShowFreeGameStartPopUp()
        {
            var extraState = machineContext.state.Get<ExtraState11003>();
            if (extraState.NextIsSuperFreeGame())
            {
                var titleView = machineContext.view.Get<BaseSpinMapTitleView>();
                titleView.ShowUpgradeAnimation(null);
                
                await machineContext.WaitSeconds(1);
                
                machineContext.view.Get<JackpotPanel11003>().Hide();
                await ShowFreeGameStartPopUp<SuperFreeSpinStartPopUp11003>("UISuperFreeGameStart11003");
            }
            else
            {
                await ShowFreeGameStartPopUp<FreeSpinStartPopUp11003>("UIFreeGameStart11003");
            }
        }
        
        public override void RecoverFreeSpinStateWhenRoomSetup()
        {
            base.RecoverFreeSpinStateWhenRoomSetup();
            if (_extraState11003.IsSuperFreeGame())
            {
                machineContext.view.Get<JackpotPanel11003>().Hide();
            }
        }

        protected override void UpdateFreeSpinUIState(bool isFreeSpin, bool isAverage = false)
        {
            controlPanel.UpdateControlPanelState(isFreeSpin, isAverage);
           
            if (_extraState11003.NextIsSuperFreeGame() || isAverage)
            {
                var superFreeSpinInfo = _extraState11003.GetSuperFreeSpinInfo();
                controlPanel.UpdateFreeSpinCountText((uint) (superFreeSpinInfo.Total - superFreeSpinInfo.Left),
                    (uint) superFreeSpinInfo.Total);
                
            }
            else
            {
                controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);
            }
        }

        protected override async Task ShowFreeGameFinishPopUp()
        {
            var extraState = machineContext.state.Get<ExtraState11003>();
           
            if (extraState.IsSuperFreeGame())
            {
                await ShowFreeGameFinishPopUp<SuperFreeSpinFinishPopUp11003>("UISuperFreeGameFinish11003");
            }
            else
            {
                await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIFreeGameFinish11003");
            }
        }
        
     
        protected override void OnHandleFreeFinishLogicEnd()
        {
            if (_extraState11003.LastIsSuperFreeGame())
            {
                if (freeSpinState.IsTriggerFreeSpin)
                {
                    HandleFreeStartLogic();
                    return;
                }
            }
            
            Proceed();
        }
        
        protected override FreeSpinReTriggerPopUp ShowFreeSpinReTriggeredPopup(string assetName)
        {
            return PopUpManager.Instance.ShowPopUp<FreeSpinReTriggerPopUp11003>(assetName);
        }
         
        protected override void RestoreTriggerWheelElement()
        {
            bool lastIsSuperFree = _extraState11003.LastIsSuperFreeGame();
            
            var triggerPanels = lastIsSuperFree ? _extraState11003.GetSuperFreeSpinInfo().Panels : freeSpinState.triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                if (freeSpinState.NextIsFreeSpin)
                {
                    machineContext.state.Get<BetState>().SetTotalBet(freeSpinState.freeSpinBet);
                    UpdateSpinUiViewTotalBet(false,false);
                }
                else
                {
                    RestoreBet();
                }
                
                controlPanel.UpdateControlPanelState(false, false);
                
                var baseWheel = machineContext.view.Get<Wheel>("WheelBaseGame");
                baseWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                var baseJackpotWheel = machineContext.view.Get<Wheel>("WheelBaseJackpot");
                baseJackpotWheel.wheelState.UpdateWheelStateInfo(triggerPanels[1]);
                
                baseWheel.ForceUpdateElementOnWheel();
                baseJackpotWheel.ForceUpdateElementOnWheel();
            }
        }
    }
}