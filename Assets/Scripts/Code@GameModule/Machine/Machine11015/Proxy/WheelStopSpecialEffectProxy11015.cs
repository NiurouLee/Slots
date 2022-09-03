using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11015: WheelStopSpecialEffectProxy
    {
        ElementConfigSet elementConfigSet = null;
        public WheelStopSpecialEffectProxy11015(MachineContext machineContext) : base(machineContext)
        {
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
        }


        protected async override void HandleCustomLogic()
        {
            
            AudioUtil.Instance.StopAudioFx("Turn");

            await ShowWildElement();
            
            machineContext.view.Get<LockView11015>().ClearLock();
            
            
            var reSpinState = machineContext.state.Get<ReSpinState>();
            if (reSpinState.IsInRespin && !reSpinState.NextIsReSpin)
            {
                machineContext.view.Get<RollMasks11015>().CloseFreeGlow();
            }

            
            base.HandleCustomLogic();
        }


        protected async Task ShowWildElement()
        {
            var extraState = machineContext.state.Get<ExtraState11015>();
            var listWildPos = extraState.GetWildsPos();
            if (listWildPos.Count > 0)
            {
                int index = 0;
                var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                foreach (var wildPos in listWildPos)
                {
                   var roll = wheel.GetRoll((int) wildPos.X);
                   var container = roll.GetVisibleContainer((int)wildPos.Y);
                   var elementConfig =
                       elementConfigSet.GetElementConfig(Constant11015.WildElementId);
                   container.UpdateElement(new SequenceElement(elementConfig,machineContext));
                   container.ShiftSortOrder(true);

                   var sortingGroup = container.transform.GetComponent<SortingGroup>();
                   sortingGroup.sortingOrder = 9999 + index;
                   
                   AudioUtil.Instance.PlayAudioFxOneShot("Wild_Add");
                   container.PlayElementAnimation("Appear");
                   await machineContext.WaitSeconds(1.5f);

                   index = index + 1;
                }
            }
            
            var freeSpinState = machineContext.state.Get<FreeSpinState>();
            if (!freeSpinState.IsInFreeSpin)
            {
                
                await machineContext.view.Get<RollMasks11015>().PlayCloseAnim();
            }
        }

    }
}