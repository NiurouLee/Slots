using System.Collections.Generic;
using System.Threading.Tasks;
using GameModule.Utility;
using UnityEngine;

namespace GameModule
{
    public class JackpotItem11009: TransformHolder
    {
        [ComponentBinder("Title")]
        private Transform tranTitle;

        [ComponentBinder("TitleBase")]
        private Transform tranTitleBase;

        protected List<Animator> listAnimator = new List<Animator>();

        protected List<bool> listJackpotGet = new List<bool>();

        protected List<uint> listJackpotElementId;
        public JackpotItem11009(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            
            foreach (Transform tranItem in tranTitle)
            {
                listAnimator.Add(tranItem.Find("CollectEffect").GetComponent<Animator>());
            }

            for (int i = 0; i < listAnimator.Count; i++)
            {
                listJackpotGet.Add(false);
            }
        }
        
        
        

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            
        }



        protected static Animator lastPlayIdle;
        public async Task CollectJackpot(ElementContainer elementContainer,LogicStepProxy stepProxy)
        {
            uint elementId = elementContainer.sequenceElement.config.id;

            int index = listJackpotElementId.IndexOf(elementId);
            if (index != -1)
            {
                listJackpotGet[index] = true;
                await Constant11009.FlipElement(context,elementContainer,stepProxy);
                Animator animator = listAnimator[index];

                AudioUtil.Instance.PlayAudioFx("JackpotCollect");
                await Constant11009.FlyElement(context, elementContainer, animator.transform);
                await XUtility.PlayAnimationAsync(animator, "Bink",context);

                
                _animationPlaySync.RegisterAnimator(animator);
                
                
                //对齐idle动画，防止出现闪烁的情况
                // if (lastPlayIdle != null)
                // {
                //     animator.Play("Idle",0,lastPlayIdle.GetCurrentAnimatorStateInfo(0).normalizedTime);
                // }
                // else
                // {
                //     animator.Play("Idle");
                // }
                
                lastPlayIdle = animator;

                if (IsJackpotGet())
                {
                    await context.WaitSeconds(0.6f);
                    ClearJackpot();
                    await ShowJackpotPopUp(stepProxy);
                }
            }
        }



        private string[] _jackpotsAddress =
            {"UIJackpotMini11009","UIJackpotMinor11009", "UIJackpotMajor11009", "UIJackpotGrand11009"};
        protected async Task ShowJackpotPopUp(LogicStepProxy stepProxy)
        {
            
            var jackpotState = context.state.Get<JackpotState11009>();
            ulong jackpotPay = jackpotState.GetJackpotWinPay((uint)jackpotId);
            if (jackpotPay > 0)
            {

                var assetAddress = _jackpotsAddress[jackpotId-1];

                var view = PopUpManager.Instance.ShowPopUp<UIJackpot11009>(assetAddress);
                var jackpotWin = context.state.Get<BetState>().GetPayWinChips(jackpotPay);
            
                view.SetJackpotWinNum(jackpotWin);

                TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
                view.SetPopUpCloseAction(()=>
                {
                    taskCompletionSource.SetResult(true);
                });
                await taskCompletionSource.Task;
                stepProxy.AddWinChipsToControlPanel(jackpotWin);
                float winTime = context.view.Get<ControlPanel>().GetNumAnimationEndTime();
                await context.WaitSeconds(winTime);
            }
            else
            {
                Debug.LogError("=====jackpot pay is zero");
            }
        }


        public bool IsJackpotGet()
        {
            for (int i = 0; i < listJackpotGet.Count; i++)
            {
                if (!listJackpotGet[i])
                {
                    return false;
                }
            }

            return true;
        }



        protected int jackpotId;
        protected AnimationPlaySync _animationPlaySync;
        public void SetData(int jackpotId,AnimationPlaySync animationPlaySync)
        {
            this.jackpotId = jackpotId;
            _animationPlaySync = animationPlaySync;

            

            switch (jackpotId)
            {
                case 1:
                    listJackpotElementId = Constant11009.ListElementIdJackpot1;
                    break;
                case 2:
                    listJackpotElementId = Constant11009.ListElementIdJackpot2;
                    break;
                case 3:
                    listJackpotElementId = Constant11009.ListElementIdJackpot3;
                    break;
                case 4:
                    listJackpotElementId = Constant11009.ListElementIdJackpot4;
                    break;
            }
            
        }


        public void RefreshJackpotNoAnim()
        {
            if (transform.gameObject.activeInHierarchy)
            {
                var extraState = context.state.Get<ExtraState11009>();
                var jackpotWord = extraState.GetJackptWord(jackpotId);
                for (int i = 0; i < listAnimator.Count; i++)
                {
                    if (jackpotWord.State[i])
                    {
                        listJackpotGet[i] = true;
                        //listAnimator[i].Play("Idle");
                        _animationPlaySync.RegisterAnimator(listAnimator[i]);
                        lastPlayIdle = listAnimator[i];
                    }
                    else
                    {
                        listJackpotGet[i] = false;
                        _animationPlaySync.UnregisterAnimator(listAnimator[i]);
                        listAnimator[i].Play("Dis");
                    }
                }
            }
        }

        public void ClearJackpot()
        {
            for (int i = 0; i < listAnimator.Count; i++)
            {
                _animationPlaySync.UnregisterAnimator(listAnimator[i]);
                listAnimator[i].Play("Dis");
                listJackpotGet[i] = false;
            }
        }


        public void RefreshJackpotState()
        {
            var freeState = context.state.Get<FreeSpinState11009>();
            var extraState = context.state.Get<ExtraState11009>();
            if (extraState.GetFreeRedState()) //有红色才显示
            {
                tranTitle.gameObject.SetActive(true);
                tranTitleBase.gameObject.SetActive(false);
            }
            else
            {
                tranTitle.gameObject.SetActive(false);
                tranTitleBase.gameObject.SetActive(true);
            }
        }

    }
}