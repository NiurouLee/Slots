using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11006: WheelStopSpecialEffectProxy
    {
        private ExtraState11006 extraState;
        private FreeSpinState freeSpinState;
        
        public WheelStopSpecialEffectProxy11006(MachineContext machineContext) : base(machineContext)
        {
            extraState = machineContext.state.Get<ExtraState11006>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }


        protected override void HandleCustomLogic()
        {
            if (freeSpinState.IsInFreeSpin)
            {
                machineContext.view.Get<FreeGameInfomationView11006>().RefreshUI(async () =>
                {
                    await RefreshMultiplierElement();
                    base.HandleCustomLogic();
                });
            }
            else
            {
                RefreshMultiplierElement();
                base.HandleCustomLogic();
            }
        }


        public async Task RefreshMultiplierElement()
        {
            int multiplier = extraState.GetMultiplier();
            if (multiplier > 1)
            {
                var wheel = machineContext.view.Get<Wheel>();
                List<ElementContainer> listElement;
                if (freeSpinState.IsInFreeSpin)
                {
                    
                    listElement = wheel.GetElementMatchFilter((container) =>
                    {
                        if (Constant11006.listFreeMultiplierElements.Contains(container.sequenceElement.config.id))
                        {
                            return true;
                        }
                
                        return false;
                    });
                    
                    
                    
                    for (int i = 0; i < listElement.Count; i++)
                    { 
                        AudioUtil.Instance.PlayAudioFx("WildBuffalo_W03_Appear");
                       await  listElement[i].PlayElementAnimationAsync("MultipliyAppear");
                    }

                    //await XUtility.WaitSeconds(0.5f);
                    
                    for (int i = 0; i < listElement.Count; i++)
                    {
                        listElement[i].PlayElementAnimation("MultipliyWin");
                    }
                    
                    //Debug.LogError($"=======FreeMultiplier:{multiplier}");
                }
                else
                {
                    listElement = wheel.GetElementMatchFilter((container) =>
                    {
                        if (Constant11006.listBaseMultiplierElements.Contains(container.sequenceElement.config.id))
                        {
                            return true;
                        }
                
                        return false;
                    });
                    
                    for (int i = 0; i < listElement.Count; i++)
                    {
                        listElement[i].PlayElementAnimation("MultipliyWin");
                    }
                    //Debug.LogError($"=======BaseMultiplier:{multiplier}");
                }


                
                
                
            }
        }


    }
}