using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeGameProxy11006:FreeGameProxy
    {

        private GameObject objFreeGameEffect;

        private ExtraState11006 extraState;
        public FreeGameProxy11006(MachineContext context) : base(context)
        {
            objFreeGameEffect = context.transform.Find("ZhenpingAnim/Wheels/FreeGameEffects").gameObject;
            extraState = machineContext.state.Get<ExtraState11006>();
        }


        protected override   async Task  ShowFreeGameStartPopUp()
        {
            await ShowFreeGameStartPopUp<FreeSpinStartPopUp>("UIFreeGameStart" + machineContext.assetProvider.AssetsId);
            
        }


        protected override  async Task  ShowFreeGameFinishPopUp()
        {
            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>("UIFreeGameFinish" + machineContext.assetProvider.AssetsId);
        }


        

        protected override  async Task  ShowFreeSpinStartCutSceneAnimation()
        {
            var viewBase = machineContext.view.Get<BaseGameInfomationView11006>();
            var viewFree = machineContext.view.Get<FreeGameInfomationView11006>();
            viewBase.Close();
            viewFree.Open();
            viewFree.RefreshUINoAnim();
            objFreeGameEffect.SetActive(true);

            
            

            
           await base.ShowFreeSpinStartCutSceneAnimation();
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            var viewBase = machineContext.view.Get<BaseGameInfomationView11006>();
            var viewFree = machineContext.view.Get<FreeGameInfomationView11006>();
            viewBase.Open();
            viewFree.Close();
            objFreeGameEffect.SetActive(false);
            
            machineContext.view.Get<MultiplierNoticeView11006>().Close();
            StopMultAnim();
            StopWinAnim();
            RestoreTriggerWheelElement();
            
            
            extraState.changeBgm = !extraState.changeBgm;
            
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }

        protected void StopWinAnim()
        {
            var activeState = machineContext.state.Get<WheelsActiveState>();
            var runningWheel = activeState.GetRunningWheel();
            for (var i = 0; i < runningWheel.Count; i++)
                runningWheel[i].winLineAnimationController.StopAllElementAnimation();
        }

        protected void StopMultAnim()
        {
            var listElement = machineContext.view.Get<Wheel>().GetElementMatchFilter((container) =>
            {
                if (Constant11006.listFreeMultiplierElements.Contains(container.sequenceElement.config.id) ||
                    Constant11006.listBaseMultiplierElements.Contains(container.sequenceElement.config.id))
                {
                    return true;
                }
                
                return false;
            });
            
            for (int i = 0; i < listElement.Count; i++)
            {
                listElement[i].PlayElementAnimation("MultipliyIdle");
                listElement[i].UpdateAnimationToStatic();
            }
        }

        
        protected override async void HandleFreeStartLogic()
        {
            // var freeWheel = machineContext.view.Get<Wheel>();
            // var triggerElementContainers = freeWheel.GetElementMatchFilter((container) =>
            // {
            //     if (container.sequenceElement.config.id == Constant11006.scatterElement)
            //     {
            //         return true;
            //     }
            //     return false;
            // });
            //
            // if (triggerElementContainers.Count > 0)
            // {
            //     for (var i = 0; i < triggerElementContainers.Count; i++)
            //     {
            //         triggerElementContainers[i].PlayElementAnimation("Trigger");
            //     }
            // }

            StopMultAnim();
            //await machineContext.view.Get<Wheel>().winLineAnimationController.BlinkFreeSpinTriggerLine();
            
            base.HandleFreeStartLogic();
        }
        
        
        protected override async void HandleFreeReTriggerLogic()
        {
            
            StopMultAnim();
            await machineContext.view.Get<Wheel>().winLineAnimationController.BlinkFreeSpinTriggerLine();

            base.HandleFreeReTriggerLogic();
        }
        
        
         
    }
}