using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeGameProxy11005: FreeGameProxy
    {
        protected Animator animatorTransition;
        protected Animator animatorZhenping;

        protected Transform tranFreeBG;
        public FreeGameProxy11005(MachineContext context) : base(context)
        {
            animatorTransition = context.transform.Find("ZhenpingAnim/Transition11005").GetComponent<Animator>();
            animatorZhenping = context.transform.Find("ZhenpingAnim").GetComponent<Animator>();
            tranFreeBG = context.transform.Find("ZhenpingAnim/Background/SceneBackGroundFree");
        }


        protected override void HandleCustomLogic()
        {
            base.HandleCustomLogic();

            machineContext.view.Get<FreeCollectView11005>().RefreshUINoAnim();
            
            

        }
        

        protected override async Task ShowFreeGameStartPopUp()
        {
            RestoreTriggerWheelElement();
            HideAllLetters();       

            await machineContext.WaitSeconds(1);
            await ShowFreeGameStartPopUp<FreeSpinStartPopUp>(machineContext.assetProvider.GetAssetNameWithPrefix("UIFreeGameStart"));
        }

        private void HideAllLetters()
        {
            var letterElementContainers = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id >= 15 &&
                    container.sequenceElement.config.id <= 114)
                {
                    return true;
                }
                
                return false;
            });

            if (letterElementContainers.Count > 0)
            {
                for (int i = 0; i < letterElementContainers.Count; i++)
                {
                    if (letterElementContainers[i].GetElement().transform.Find("LetterGroup"))
                    {
                        letterElementContainers[i].GetElement().transform.Find("LetterGroup").gameObject.SetActive(false);
                    }
                }
            }
        }

        protected override async Task ShowFreeGameFinishPopUp()
        {
            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp>(machineContext.assetProvider.GetAssetNameWithPrefix("UIFreeGameFinish"));
        }

        protected override void RecoverCustomFreeSpinState()
        {
            try
            {
                
                machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
                
                
              
                
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }


        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {



            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            XUtility.PlayAnimationAsync(animatorZhenping, "ZhenpingAnim",machineContext);
            animatorTransition.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorTransition, "Transition", () =>
            {
                if (animatorTransition != null)
                {
                    animatorTransition.gameObject.SetActive(false);
                }
            });
            await machineContext.WaitSeconds(1.7f);
            
            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(false);
            ExtraState11005 extraState = machineContext.state.Get<ExtraState11005>();
            uint luckCount = extraState.GetLuckCount();
                
            machineContext.state.Get<WheelsActiveState11005>()
                .UpdateRunningWheel(Constant11005.GetListFree(luckCount));
            tranFreeBG.gameObject.SetActive(true);
            
            RecoverCustomFreeSpinState();

            await machineContext.WaitSeconds(4 - 1.7f);
            
            await base.ShowFreeSpinStartCutSceneAnimation();
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            XUtility.PlayAnimationAsync(animatorZhenping, "ZhenpingAnim",machineContext);
            animatorTransition.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorTransition, "Transition", () =>
            {
                animatorTransition.gameObject.SetActive(false);
            });
            await machineContext.WaitSeconds(1.7f);
            
            
            
          
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11005>();
            
            wheelsActiveState.UpdateRunningWheel(new List<string>() {"WheelBaseGame"},true);
            tranFreeBG.gameObject.SetActive(false);
            RestoreTriggerWheelElement();


            machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(true);
            machineContext.view.Get<BaseLetterView11005>().ChangeBetLetter(false);
            
            //把字母替换掉
            var listWheels = wheelsActiveState.GetRunningWheel();
            var listElement = listWheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id >= 15 &&
                    container.sequenceElement.config.id <= 114)
                {
                    return true;
                }
                
                return false;
                
            }, 0);
            
            var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            for (int i = 0; i < listElement.Count; i++)
            {
                SequenceElement sequenceElement = null;
                string strId = listElement[i].sequenceElement.config.name;
                strId = strId.Replace("S", "");
                uint elementId = uint.Parse(strId);
                while (elementId > 10)
                {
                    elementId -= 10;
                }

                string newId = elementId.ToString().PadLeft(2, '0');
                newId = $"S{newId}";

                sequenceElement = new SequenceElement(elementConfigSet.GetElementConfigByName(newId),
                    machineContext);

                listElement[i].UpdateElement(sequenceElement);
            }
            
            
            
            
            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
            
            await machineContext.WaitSeconds(4 - 1.7f);
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }
    }
}