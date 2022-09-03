
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SuperBonusFinishPopUp11020 : MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/IntegralText")]
        protected Text freeSpinWinChipText;
        
        [ComponentBinder("Root/MainGroup/CountText")]
        protected Text freeSpinCountText;
        
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;

        protected Action finishAction;
 
        public SuperBonusFinishPopUp11020(Transform transform)
        :base(transform)
        {
            bool enableClick = true;

            collectButton.onClick.AddListener(async () =>
            {
                if (!enableClick)
                {
                    return;
                }
                AudioUtil.Instance.PlayAudioFx("Close");
                enableClick = false;
                
                var state = context.state.Get<FreeSpinState>();
                await state.SettleFreeSpin();

                if (!state.FreeNeedSettle)
                {
                    finishAction.Invoke();
                    Close();
                }
                else
                {
                    enableClick = true;
                }
            });
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if(freeSpinWinChipText)
                freeSpinWinChipText.SetText(context.state.Get<FreeSpinState>().TotalWin.GetCommaFormat());
            if(freeSpinCountText) 
                freeSpinCountText.SetText(context.state.Get<FreeSpinState>().TotalCount.ToString());
        }
         
        public void BindFinishAction(Action inFinishAction)
        {
            finishAction = inFinishAction;
        }
    }
}
