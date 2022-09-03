using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class BaseLetterViewItem11005
    {
        protected Transform transform;
        protected MachineContext context;

        [ComponentBinder("Disable")]
        private Transform tranDisable;

        [ComponentBinder("Enable")]
        private Transform tranEnable;

        [ComponentBinder("CountGroup")]
        private Transform tranCountGroup;

        [ComponentBinder("CountText")]
        private TextMesh txtCount;

        private int index;
        private ExtraState11005 extraState;
        private Wheel wheel;
        public BaseLetterViewItem11005(Transform tran,MachineContext cont,int index)
        {
            transform = tran;
            context = cont;
            this.index = index;
            extraState = context.state.Get<ExtraState11005>();
            wheel = context.view.Get<Wheel>();
            ComponentBinder.BindingComponent(this,transform);
        }



        protected uint numLetter = 0;


        public void GetLetterNumber()
        {
            var collection = extraState.GetCollection();
            numLetter = collection.Now[index];
        }

        public async Task RefreshUI()
        {

            GetLetterNumber();
            
            var letterElementContainers = wheel.GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id >= 15 &&
                    container.sequenceElement.config.id <= 114 &&
                    container.sequenceElement.config.GetExtra<int>("letter")==index+1)
                {
                    return true;
                }
                
                return false;
            });

            if (letterElementContainers.Count > 0)
            {
                
                

                List<Task> listTask = new List<Task>();
                for (int i = 0; i < letterElementContainers.Count; i++)
                {
                    listTask.Add(FlyLetter(letterElementContainers[i]));
                }

                await Task.WhenAll(listTask);
            }

            RefreshLetterNumber();
            
        }

        public void RefreshLetterNumber()
        {
            
            if (numLetter > 1)
            {
                tranDisable.gameObject.SetActive(false);
                tranEnable.gameObject.SetActive(true);
                tranCountGroup.gameObject.SetActive(true);
                txtCount.text = numLetter.ToString();
            }
            else
            {
                tranCountGroup.gameObject.SetActive(false);
                if (numLetter == 0)
                {
                    tranDisable.gameObject.SetActive(true);
                    tranEnable.gameObject.SetActive(false);
                }
                else
                {
                    tranDisable.gameObject.SetActive(false);
                    tranEnable.gameObject.SetActive(true);
                }
            }
        }

        protected async Task FlyLetter(ElementContainer container)
        {
            
            

            try
            {
                // container.PlayElementAnimation("Dis",false, () =>
                // {
                //    
                // });
                
                SequenceElement sequenceElement = null;
                var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                string strId = container.sequenceElement.config.name;
                strId = strId.Replace("S", "");
                uint elementId = uint.Parse(strId);
                while (elementId > 10)
                {
                    elementId -= 10;
                }

                string newId = elementId.ToString().PadLeft(2, '0');
                newId = $"S{newId}";

                sequenceElement = new SequenceElement(elementConfigSet.GetElementConfigByName(newId),
                    context);

                
                var startPos = container.GetElement().transform.Find("LetterGroup").position;
                string txtSrc = container.GetElement().transform.Find("LetterGroup/Letter").GetComponent<TextMesh>().text;
                
                //container.UpdateElementMaskInteraction(false);
                container.UpdateElement(sequenceElement);
                
                
                
                //飞粒子
                var effectFly = context.assetProvider.InstantiateGameObject("Active_SFly",true);
                
                effectFly.transform.parent = transform;
                effectFly.transform.position = startPos;
                effectFly.transform.localScale = Vector3.one;
                
                TextMesh txtDst = effectFly.transform.Find("LetterGroup/Letter").GetComponent<TextMesh>();
                txtDst.text = txtSrc;

                var animatorFly = effectFly.GetComponent<Animator>();
                animatorFly.Play("Active_SFly");
                await context.WaitSeconds(0.266f);
                
               
            
                await XUtility.FlyAsync(effectFly.transform, startPos,
                    transform.position, 0, 0.6f,Ease.Linear,context);
                context.assetProvider.RecycleGameObject("Active_SFly",effectFly);
            
                //爆炸
                ShowBomb();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
           
        }


        protected async void ShowBomb()
        {
            var effectFlyExpload = context.assetProvider.InstantiateGameObject("Active_FlyExplode",true);
            effectFlyExpload.transform.parent = transform;
            effectFlyExpload.transform.position = transform.position;
            await context.WaitSeconds(0.8f);
            context.assetProvider.RecycleGameObject("Active_FlyExplode",effectFlyExpload);
        }
    }
}