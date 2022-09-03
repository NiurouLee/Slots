using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class FreeSpinFinishPopUp11008 : MachinePopUp
    {
        [ComponentBinder("IntegralText")]
        protected Text freeSpinWinChipText;
        
        [ComponentBinder("CountText")]
        protected Text freeSpinCountText;
        
        [ComponentBinder("CollectButton")]
        protected Button collectButton;

        protected Action finishAction;


        private bool isSettle = false;
        public FreeSpinFinishPopUp11008(Transform transform) : base(transform)
        {
            if (collectButton != null)
            {
                collectButton.onClick.AddListener(async () =>
                {
                    if (!isSettle)
                    {
                        // AudioUtil.Instance.PlayAudioFx("Close");
                        // isSettle = true;
                        // await context.state.Get<FreeSpinState>().SettleFreeSpin();
                        finishAction?.Invoke();
                        Close();
                    }

                });
            }

        }
        public async void Close()
        {
            await context.state.Get<FreeSpinState>().SettleFreeSpin();
            PopUpManager.Instance.ClosePopUp(this);
            AudioUtil.Instance.PlayAudioFx("Close");
            isSettle = true;
            
        }
         public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("FreeGameEnd_Open");
            base.OnOpen();
        }
        public bool IsAutoClose()
        {
            return collectButton == null;
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if(freeSpinWinChipText)
                freeSpinWinChipText.SetText(context.state.Get<FreeSpinState>().TotalWin.GetCommaFormat());
            if(freeSpinCountText) 
                freeSpinCountText.SetText(context.state.Get<FreeSpinState>().TotalCount.ToString());
        }
    }
}

