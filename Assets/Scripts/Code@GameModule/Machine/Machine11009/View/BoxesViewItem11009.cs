using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class BoxesViewItem11009 : TransformHolder
    {
        
        
        public enum BoxState
        {
            Half,
            Full,
            Gold,
        }
        

        protected Animator animator;

        protected BoxState boxState;

        protected Transform tranFlyPoint;

        public BoxesViewItem11009(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            boxState = BoxState.Half;
            tranFlyPoint = transform.Find("GameObject/FlyPoint");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        protected uint elementId;
        protected int indexBox;

        public void SetCollectElementId(uint elementId, int index)
        {
            this.elementId = elementId;
            this.indexBox = index;
        }


        

        public async Task CollectElements(ElementContainer elementContainer)
        {

            var extraState = context.state.Get<ExtraState11009>();
            bool isLastActivated = extraState.IsLastBoxActivated(indexBox);
            bool isBoxActivated = extraState.IsBoxActivated(indexBox);
            bool isBoxExaggerated = extraState.IsBoxExaggerated(indexBox);

            // if (isLastActivated && isBoxActivated)
            // {
            //     //已经激活过的不收集
            //     return;
            // }
            
            
            BoxState nowBoxState = BoxState.Half;
            if (!isBoxActivated && !isBoxExaggerated)
            {
                //半满
                nowBoxState = BoxState.Half;
            }
            else if (isBoxActivated)
            {
                //激活
                nowBoxState = BoxState.Gold;
            }
            else if (isBoxExaggerated)
            {
                //全满
                nowBoxState = BoxState.Full;
            }
            
            BoxState lastBoxState = boxState;
            boxState = nowBoxState;
            
            

            if (lastBoxState == BoxState.Gold && nowBoxState == BoxState.Gold )
            {
                //已经激活过的不收集
                return;
            }



            if (indexBox == Constant11009.IndexRed)
            {
                AudioUtil.Instance.PlayAudioFx("CollectRed");
            }
            else
            {
                AudioUtil.Instance.PlayAudioFx("Collect");
            }

            //飞
            await Constant11009.FlyElement(context,elementContainer,tranFlyPoint);


            //var freeSpinState = context.state.Get<FreeSpinState>();
            if (lastBoxState != nowBoxState && nowBoxState == BoxState.Gold)
            {
               await PlayBoxAnimationAsync(lastBoxState, nowBoxState);

            }
            else
            {
                PlayBoxAnimationAsync(lastBoxState, nowBoxState);
            }



        }

        protected async Task PlayBoxAnimationAsync(BoxState lastBoxState,BoxState nowBoxState)
        {
            string animCollect = string.Empty;
            //收集动画
            switch (lastBoxState)
            {
                case BoxState.Half:
                    int randomNear = Random.Range(1, 100);
                    if (randomNear <= 20 || nowBoxState == BoxState.Gold)
                    {
                        animCollect = "NearCollect";
                    }
                    else
                    {
                        animCollect = "HalfStateCollect";
                    }
                    break;
                case BoxState.Full:
                    
                    int randomFullNear = Random.Range(1, 100);
                    if (randomFullNear <= 20 || nowBoxState == BoxState.Gold)
                    {
                        AudioUtil.Instance.PlayAudioFx("CollectNear");
                        animCollect = "FullNearCollect";
                    }
                    else
                    {
                        animCollect = "FullStateCollect";
                    }
                    break;
                // case BoxState.Gold:
                //     await XUtility.PlayAnimationAsync(animator, "GoldStateCollect");
                //     break;
            }
            
            
            
            await XUtility.PlayAnimationAsync(animator, animCollect,context);


            


            //转换状态动画
            if (lastBoxState != nowBoxState)
            {
                if (nowBoxState == BoxState.Full)
                {
                    await XUtility.PlayAnimationAsync(animator, "Half2Full",context);
                }
                else if(nowBoxState == BoxState.Gold)
                {
                    AudioUtil.Instance.PlayAudioFx("FeatureTrigger");
                    if (lastBoxState == BoxState.Half)
                    {
                        await XUtility.PlayAnimationAsync(animator, "Half2Gold",context);

                    }
                    else
                    {
                        await XUtility.PlayAnimationAsync(animator, "Full2Gold",context);
                    }
                }
            }
        }



        public void RefreshBoxStateNoAnim()
        {
            var extraState = context.state.Get<ExtraState11009>();
            bool isLastActivated = extraState.IsLastBoxActivated(indexBox);
            bool isBoxActivated = extraState.IsBoxActivated(indexBox);
            bool isBoxExaggerated = extraState.IsBoxExaggerated(indexBox);
            
            if (!isBoxActivated && !isBoxExaggerated)
            {
                //半满
                boxState = BoxState.Half;
                animator.Play("HalfState");
            }
            else if (isBoxActivated)
            {
                //激活
                boxState = BoxState.Gold;
                if (context.state.Get<FreeSpinState11009>().IsTriggerFreeSpin)
                {
                    //trigger的这一把要显示成全满状态
                    animator.Play("FullState");
                    boxState = BoxState.Full;
                }
                else
                {
                    animator.Play("GoldState");
                }

                
            }
            else if (isBoxExaggerated)
            {
                //全满
                boxState = BoxState.Full;
                animator.Play("FullState");
            }
        }





    }
}