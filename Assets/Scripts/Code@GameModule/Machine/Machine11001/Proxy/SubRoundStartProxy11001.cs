// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/16/11:20
// Ver : 1.0.0
// Description : SubRoundStartProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class SubRoundStartProxy11001 : SubRoundStartProxy
    {
        private SequenceElement wildElement;

        public SubRoundStartProxy11001(MachineContext machineContext)
            : base(machineContext)
        {
        }
         
        protected override void HandleCustomLogic()
        {
            // if (machineContext.state.Get<FreeSpinState>().NextIsFreeSpin)
            // {
                var wheel = machineContext.view.Get<Wheel>(0);
                var container = wheel.GetRoll(12).GetVisibleContainer(0);
               
                if (container.sequenceElement.config.id != 11)
                {
                    if (wildElement == null)
                    {
                        wildElement = machineContext.machineConfig.GetSequenceElement(11, machineContext);
                    }

                    container.UpdateElement(wildElement);
                }
                //}

            base.HandleCustomLogic();
        }
    }
}