using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class UIJackpotPanel11312 : UIJackpotBase
    {
        [ComponentBinder("Root/MainGroup/IntegralGroup/BGGroup")]
        protected Transform BGGroup;
        [ComponentBinder("Root/TopGroup/TitleGroup")]
        protected Transform TitleGroup;
         [ComponentBinder("Root/Double/DoubleTX/MutipuleText1")]
        protected Text MutipuleText1;
        [ComponentBinder("Root/Double/DoubleTX/MutipuleText2")]
        protected Text MutipuleText2;
        private bool isShowBtn=false;
        public UIJackpotPanel11312(Transform inTransform) : base(inTransform)
        {
            MutipuleText1.gameObject.SetActive(false);
            MutipuleText1.gameObject.SetActive(false);
        }
        public async void InitBgAndTitle(int level,bool isShowBtns,int multiple=0){
            isShowBtn = isShowBtns;
            if(!isShowBtns)
                btnCollect.gameObject.SetActive(false);

            for (int i = 1; i <= 5; i++)
            {
                BGGroup.transform.Find("BG"+i).gameObject.SetActive(false);
                TitleGroup.transform.Find("Title"+i).gameObject.SetActive(false);
            }
            BGGroup.transform.Find("BG"+level).gameObject.SetActive(true);
            TitleGroup.transform.Find("Title"+level).gameObject.SetActive(true);

            if(multiple>1){
                await context.WaitSeconds(1.73f);
                Text MutipuleText =  MutipuleText1;
                if(level==1)
                    MutipuleText = MutipuleText2;
                MutipuleText.gameObject.SetActive(true);
                MutipuleText.text = "X"+multiple;
                await XUtility.PlayAnimationAsync(animator, "Double", context);
            }

            if(!isShowBtns){
                await context.WaitSeconds(3);
                Close();
            }
        }
        
        public override async Task OnClose()
        {
            if(isShowBtn)
                await SettleReSpin();
            if (animator && XUtility.IsAnimationExist(animator, "Close"))
                await XUtility.PlayAnimationAsync(animator, "Close");
        }

        public async Task SettleReSpin()
        {   
            var settleProcess = await context.serviceProvider.SettleGameProgress();
            
            if (settleProcess != null)
            {
                context.state.UpdateStateOnSettleProcess(settleProcess);
            }
        }
    }
}

