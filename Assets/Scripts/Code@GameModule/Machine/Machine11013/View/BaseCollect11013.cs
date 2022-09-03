using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class BaseCollect11013: TransformHolder
    {

        [ComponentBinder("CollectSymbol")]
        protected Transform tranCollectSymbol;

        [ComponentBinder("Fill")]
        protected SpriteRenderer picProgressFill;

        [ComponentBinder("IntegralText")]
        protected TextMesh txtProgress;


        protected Animator animator;
        public BaseCollect11013(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = this.transform.GetComponent<Animator>();
        }

        protected float lenghtMin = -10.63f;
        protected float lenghtMax = -3.617f;

        public void Open()
        {
            this.transform.gameObject.SetActive(true);
        }


        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }


        
        public bool CheckEarlyHighLevelWinToHandle()
        {
            var _extraState = context.state.Get<ExtraState>();
            var _winState = context.state.Get<WinState>();
            if (_extraState == null || !_extraState.HasSpecialBonus())
            {
                if(_winState.winLevel >= (int)WinLevel.BigWin)
                    return true;
            }

            return _winState.winLevel >= (int)WinLevel.BigWin;
        }
        public async Task CollectStarElements()
        {
            var extraState = context.state.Get<ExtraState11013>();
            
            if (extraState.GetNowMapStep() >= extraState.GetMaxMapStep() || CheckEarlyHighLevelWinToHandle())
            {
                await CollectAllElements();
            }
            else
            {
                CollectAllElements();
            }
        }

        protected async Task CollectAllElements()
        {
            
            var extraState = context.state.Get<ExtraState11013>();
            
            Wheel wheel = context.state.Get<WheelsActiveState11013>().GetRunningWheel()[0];
            var listContainer = wheel.GetElementMatchFilter((elementContainer) =>
            {
                if (elementContainer.sequenceElement.config.id == Constant11013.StarElement)
                {
                    return true;
                }

                return false;
            });
            
            
            decimal maxStep = extraState.GetMaxMapStep();
            int startValue = (int)extraState.GetLastMapStep();
            int endValue = (int)extraState.GetNowMapStep();

            long startBase = (long)extraState.GetLastExtraInfo().MapBase;
            long endBase = (long)extraState.GetExtraInfo().MapBase;

            bool hasSpecialBonus = extraState.HasSpecialBonus();
            

            List<Task> listTask = new List<Task>();
            foreach (var container in listContainer)
            {
                listTask.Add(FlyStar(container));
            }

            if (listTask.Count > 0)
            {

                PlayFlyStarAudio();
                await Task.WhenAll(listTask);

               listTask.Clear();
               
               listTask.Add(XUtility.PlayAnimationAsync(animator, "Blink"));
               
               listTask.Add(RefreshProgress(maxStep,startValue,endValue,startBase,endBase));

               AudioUtil.Instance.PlayAudioFx("Star_Collect");
               AudioUtil.Instance.PlayAudioFx("Progress_Grow");
               await Task.WhenAll(listTask);
               
               if (hasSpecialBonus)
               {
                   AudioUtil.Instance.PlayAudioFx("FeatureGame_Trigger");
                   await XUtility.PlayAnimationAsync(animator, "Finish");
               }
            }
            
        }

        protected async Task PlayFlyStarAudio()
        {
            AudioUtil.Instance.PlayAudioFx("B03_Blink01");
            await context.WaitSeconds(0.5f);
            AudioUtil.Instance.PlayAudioFx("Star_Fly");
        }


        protected async Task FlyStar(ElementContainer container)
        {
            container.PlayElementAnimation("Fly");
            
            var effectFly = context.assetProvider.InstantiateGameObject("Active_Fly",true);
            effectFly.transform.position = container.transform.position;
            effectFly.transform.parent = transform;
            effectFly.transform.localScale = Vector3.one;
                    
            var animatorFly = effectFly.GetComponent<Animator>();
            animatorFly.Play("Fly");

            Vector3 startPos = container.transform.position;

            await context.WaitSeconds(0.5f);
                    
                    
            await XUtility.FlyAsync(effectFly.transform,startPos ,
                tranCollectSymbol.position, 0, 0.45f,Ease.Linear,context);
                    
                     
            //XUtility.PlayAnimationAsync(tranCollectSymbol, "Blink",context);
            context.assetProvider.RecycleGameObject("Active_Fly",effectFly);
        }

        private Tweener tweener;
        private Tweener tweenerBase;
        public async Task RefreshProgress(decimal maxStep,int startValue,int endValue,
            long startBase,long endBase)
        {
            
            if (tweener != null)
            {
                tweener.Kill();
            }

            if (tweenerBase != null)
            {
                tweenerBase.Kill();
            }



            SetProgress((float)(startValue/maxStep));
            txtProgress.text = startBase.GetCommaFormat();


            


            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            
            tweener = DOTween.To(() => startValue, (changeNum) =>
            {
                SetProgress((float)(changeNum/maxStep));
            }, endValue, 0.5f);
            
            tweenerBase = DOTween.To(() => startBase, (changeNum) =>
            {
                txtProgress.text = changeNum.GetCommaFormat();
            }, endBase, 0.5f);

            context.AddTweener(tweener);
            context.AddTweener(tweenerBase);
            context.AddWaitTask(taskCompletionSource,null);

            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                context.RemoveTweener(tweenerBase);
                context.RemoveTask(taskCompletionSource);
                SetProgress((float)(endValue/maxStep));
                
                taskCompletionSource.SetResult(true);
            };
            tweenerBase.onComplete += () =>
            {
                txtProgress.text = endBase.GetCommaFormat();
            };
            

            await taskCompletionSource.Task;

        }

        public void RefreshProgressNoAnim()
        {

            var extraState = context.state.Get<ExtraState11013>();
            
                uint nowProgress = extraState.GetNowMapStep();
                float newProgress = nowProgress / (float) extraState.GetMaxMapStep();
                SetProgress(newProgress);
                txtProgress.text = extraState.GetExtraInfo().MapBase.GetCommaFormat();
          

        }


        public void Clear()
        {
            SetProgress(0);
            txtProgress.text = "0";
        }


        public void SetProgress(float num)
        {
            
            //Debug.LogError($"=========SetProgress:{num}");
            
            num = Mathf.Clamp(num, 0, 1);
            float newProgress = lenghtMin;
            newProgress = lenghtMin + (lenghtMax - lenghtMin) * num;
            // var size = picProgressFill.size;
            // size.x = newProgress;
            // picProgressFill.size = size;
            var pos = picProgressFill.transform.localPosition;
            pos.x = newProgress;
            picProgressFill.transform.localPosition = pos;
        }



    }
}