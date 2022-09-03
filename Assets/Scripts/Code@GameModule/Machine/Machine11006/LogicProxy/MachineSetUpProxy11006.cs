using UnityEngine;

namespace GameModule
{
    public class MachineSetUpProxy11006 : MachineSetUpProxy
    {
        private GameObject objFreeGameEffect;
        public MachineSetUpProxy11006(MachineContext context) : base(context)
        {
            objFreeGameEffect = context.transform.Find("ZhenpingAnim/Wheels/FreeGameEffects").gameObject;

        }

        protected override void HandleCustomLogic()
        {
            var viewBase = machineContext.view.Get<BaseGameInfomationView11006>();
            var viewFree = machineContext.view.Get<FreeGameInfomationView11006>();
            
            if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                viewBase.Close();
                viewFree.Open();
                viewFree.RefreshUINoAnim();
                objFreeGameEffect.SetActive(true);
            }
            else
            {
                viewBase.Open();
                viewFree.Close();
                objFreeGameEffect.SetActive(false);
            }
            
            


            base.HandleCustomLogic();
        }
    }
}