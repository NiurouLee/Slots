using System.Collections.Generic;

namespace GameModule
{
    public class SubRoundStartProxy11006: SubRoundStartProxy
    {
        private ExtraState11006 extraState;
        private FreeSpinState freeSpinState;
        public SubRoundStartProxy11006(MachineContext context) : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11006>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }


        protected override void HandleCustomLogic()
        {
            
            var view = machineContext.view.Get<MultiplierNoticeView11006>();
            view.Close();
            
            RefreshMultiplierElement();
            
            base.HandleCustomLogic();
        }


        public  void RefreshMultiplierElement()
        {
            if (extraState.GetMultiplier() > 1)
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
                    
                }


                for (int i = 0; i < listElement.Count; i++)
                {
                    listElement[i].PlayElementAnimation("MultipliyIdle");
                    listElement[i].UpdateAnimationToStatic();
                }
                
                
                
            }
        }
    }
}