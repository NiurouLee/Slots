using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class TitlePrizeViewItem: TransformHolder
    {

        [ComponentBinder("PrizeDiskCellPoint")]
        protected Transform tranPoint;

        [ComponentBinder("EnableState")]
        protected Transform tranEnableState;

        [ComponentBinder("DisabeState")]
        protected Transform tranDisableState;

        [ComponentBinder("CrownSprite")]
        protected Transform tranCrown;
        
        [ComponentBinder("OpenState")]
        protected Transform tranOpenState;
        

        protected Animator animator;

        

        protected static readonly float offsetX = 2.128f;
        protected static readonly ulong hightLevel = 15;
        public TitlePrizeViewItem(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            
        }


        protected int index;
        protected TitlePrizeView titlePrizeView;
        public void SetData(int index,TitlePrizeView titlePrizeView)
        {
            this.index = index;
            this.titlePrizeView = titlePrizeView;
        }


        protected Transform tranWheels;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tranWheels = context.transform.Find("ZhenpingAnim/Wheels");
        }


        public bool IsEnable { get; protected set; }

        public void SetEnableState(bool isEnable)
        {
            // this.tranEnableState.gameObject.SetActive(isEnable);
            // this.tranDisableState.gameObject.SetActive(!isEnable);

                if (isEnable)
                {
                    animator.Play("EnableState");
                }
                else
                {
                    animator.Play("DisabeState");
                }

                tranCrown.gameObject.SetActive(isEnable);
                tranOpenState.gameObject.SetActive(isEnable);
            
            IsEnable = isEnable;
            
        }

        public void SetVisible(bool isVisible)
        {
            if (isVisible)
            {
                if (IsEnable)
                {
                    animator.Play("EnableState",-1,0);
                    if (tranCellTop)
                    {
                        //tranCellTop.gameObject.SetActive(true);
                        animatorCellTop.Play("Loop",-1,animatorCell.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    }
                    animatorCell.Play("Loop",-1,animatorCell.GetCurrentAnimatorStateInfo(0).normalizedTime);

                }
            }
            else
            {
                animator.Play("DisabeState");
                animatorCell.Play("Idle");
                if (tranCellTop)
                {
                    tranCellTop.gameObject.SetActive(false);
                }
            }
        }


        protected string tranCellName;
        protected Transform tranCell;
        protected Animator animatorCell;

        protected Transform tranCellTop;
        protected Animator animatorCellTop;


        protected string[] listJackpotName = {"PrizeDiskCellMinor","PrizeDiskCellMajor","PrizeDiskCellGrand"};
        
        public void RefreshInfo(RhinoGameResultExtraInfo.Types.DiskItem diskItem,bool isFree)
        {
            if (tranCell != null && !string.IsNullOrEmpty(tranCellName))
            {
                context.assetProvider.RecycleGameObject(tranCellName,tranCell.gameObject);
                if (tranCellTop != null)
                {
                    context.assetProvider.RecycleGameObject($"{tranCellName}Top", tranCellTop.gameObject);
                    tranCellTop = null;
                }

            }

            if (diskItem.IsFreeSpin)
            {
                tranCellName = "PrizeDiskCellFreeGames";
                tranCell = context.assetProvider.InstantiateGameObject(tranCellName, true).transform;
                tranCellTop = context.assetProvider.InstantiateGameObject($"{tranCellName}Top", true).transform;
                

                SetPrizeDiskCellFree(tranCell,isFree);
                SetPrizeDiskCellFree(tranCellTop,isFree);
            }
            else if (diskItem.JackpotId > 0)
            {
                tranCellName = listJackpotName[diskItem.JackpotId - 1];
                tranCell = context.assetProvider.InstantiateGameObject(tranCellName, true).transform;
                tranCellTop = context.assetProvider.InstantiateGameObject($"{tranCellName}Top", true).transform;

            }
            else
            {
                if (diskItem.WinRate/100 >= hightLevel)
                {
                    tranCellName = "PrizeDiskCellIntegralGreen";
                }
                else
                {
                    tranCellName = "PrizeDiskCellIntegralYellow";
                }
                tranCell = context.assetProvider.InstantiateGameObject(tranCellName, true).transform;
                tranCellTop = context.assetProvider.InstantiateGameObject($"{tranCellName}Top", true).transform;
                SetPrizeDiskCellIntegral(tranCell, diskItem);
                SetPrizeDiskCellIntegral(tranCellTop, diskItem);

            }

            

            animatorCell = tranCell.GetComponent<Animator>();
            animatorCellTop = tranCellTop.GetComponent<Animator>();
            tranCell.SetParent(this.tranPoint);
            tranCell.localPosition = Vector3.zero;
            tranCell.localScale = Vector3.one;
            
            tranCellTop.SetParent(tranWheels);
            tranCellTop.position = tranCell.position;
            tranCellTop.localScale = Vector3.one;

            
            tranCellTop.gameObject.SetActive(false);
            
            // if (this.index == 5 || !this.IsEnable)
            // {
            //     tranCellTop.gameObject.SetActive(false);
            // }
            // else
            // {
            //     tranCellTop.gameObject.SetActive(true);
            // }

            if (this.IsEnable)
            {
                animatorCell.Play("Loop");
                //animatorCellTop.Play("Loop");
            }
            
        }


        protected void SetPrizeDiskCellIntegral(Transform cell,RhinoGameResultExtraInfo.Types.DiskItem diskItem)
        {
            TextMesh textGreen = cell.Find("IntegralGreenText").GetComponent<TextMesh>();
            TextMesh textYellow = cell.Find("IntegralYellowText").GetComponent<TextMesh>();
            var betState = context.state.Get<BetState>();
            string numStr = betState.GetPayWinChips(diskItem.WinRate).GetAbbreviationFormat();
            textGreen.SetText(numStr);
            textYellow.SetText(numStr);

        }

        protected void SetPrizeDiskCellFree(Transform cell,bool isFree)
        {
            Transform tranIcon = cell.Find("Icon");
            Transform tranExtraSpins = cell.Find("IconExtraSpins");
            if (isFree)
            {
                tranIcon.gameObject.SetActive(false);
                tranExtraSpins.gameObject.SetActive(true);
            }
            else
            {
                tranIcon.gameObject.SetActive(true);
                tranExtraSpins.gameObject.SetActive(false);
            }
        }


        public async Task MoveToNext()
        {
            
            if (tranCellTop != null)
            {
                context.assetProvider.RecycleGameObject($"{tranCellName}Top", tranCellTop.gameObject);
                tranCellTop = null;
            }
            
            Vector3 startPos = tranCell.localPosition;
            Vector3 endPos = tranCell.localPosition;
            endPos.x = endPos.x - offsetX;
            animatorCell.Play("Idle");
            await XUtility.FlyLocalAsync(tranCell, startPos, endPos, 0, 0.36f, -1f, Ease.Linear, context);
        }


        public async Task CollectReward(LogicStepProxy logicStep)
        {
            if (this.IsEnable)
            {

                var wheel = context.state.Get<WheelsActiveState11021>().GetRunningWheel()[0];
                var roll = wheel.GetRoll(this.index);

                int rollCount = roll.lastVisibleRowIndex - roll.firstVisibleRowIndex;
                ElementContainer containerScatter = null;
                for (int i = 0; i < rollCount; i++)
                {
                    var container = roll.GetVisibleContainer(i);
                    if (container.sequenceElement.config.id == Constant11021.ElementScatter)
                    {
                        containerScatter = container;
                        break;
                    }
                }


                if (containerScatter != null)
                {

                    await context.WaitSeconds(0.8f);
                    
                    var freeSpinState = context.state.Get<FreeSpinState>();
                    var extralInfo = context.state.Get<ExtraState11021>();
                    var deskData = extralInfo.GetDiskData().Items[this.index];
                    if (freeSpinState.IsInFreeSpin && !freeSpinState.IsOver)
                    {
                        deskData = extralInfo.GetDiskData().FreeItems[this.index];
                    }

                    if (deskData.JackpotId > 0)
                    {
                        this.titlePrizeView.ShowOneItem(this.index);
                        this.tranCellTop.gameObject.SetActive(true);
                        await CollectJackpot(containerScatter,deskData,logicStep);
                    }
                    else if (!deskData.IsFreeSpin)
                    {
                        this.titlePrizeView.ShowOneItem(this.index);
                        this.tranCellTop.gameObject.SetActive(true);
                        await CollectGold(containerScatter,deskData,logicStep);
                    }
                }
            }
        }
        
        
        public async Task CollectRewardFree(LogicStepProxy logicStep)
        {
            if (this.IsEnable)
            {

                var wheel = context.state.Get<WheelsActiveState11021>().GetRunningWheel()[0];
                var roll = wheel.GetRoll(this.index);

                int rollCount = roll.lastVisibleRowIndex - roll.firstVisibleRowIndex;
                ElementContainer containerScatter = null;
                for (int i = 0; i < rollCount; i++)
                {
                    var container = roll.GetVisibleContainer(i);
                    if (container.sequenceElement.config.id == Constant11021.ElementScatter)
                    {
                        containerScatter = container;
                        break;
                    }
                }


                if (containerScatter != null)
                {
                    
                    await context.WaitSeconds(0.8f);
                    
                    var extralInfo = context.state.Get<ExtraState11021>();
                    
                    var freeSpinState = context.state.Get<FreeSpinState>();
                    
                    var deskData = extralInfo.GetDiskData().Items[this.index];
                    if (freeSpinState.IsInFreeSpin && !freeSpinState.IsOver)
                    {
                        deskData = extralInfo.GetDiskData().FreeItems[this.index];
                    }
                    
                    if (deskData.IsFreeSpin)
                    {
                        
                        if (freeSpinState.NewCount>0)
                        {
                            if (freeSpinState.IsTriggerFreeSpin)
                            {
                                this.titlePrizeView.ShowOneItem(this.index);
                                this.tranCellTop.gameObject.SetActive(true);
                                await CollectFreeTrigger(containerScatter);

                            }
                            else
                            {
                                this.titlePrizeView.ShowOneItem(this.index);
                                this.tranCellTop.gameObject.SetActive(true);
                                await CollectFreeRetrigger(containerScatter);

                            }

                        }
                    }
                }
            }
        }
        
        
        public void NiceWinComplet()
        {
            if (taskNiceWin != null)
            {
                context.RemoveTask(taskNiceWin);
                taskNiceWin.SetResult(true);
                taskNiceWin = null;
            }
        }

        public async Task AddWinChipsToControlPanel(LogicStepProxy logicStep,ulong jackpotWin)
        {
            if (!context.state.Get<AutoSpinState>().IsAutoSpin)
            {
                context.view.Get<ControlPanel>().ShowStopButton(true);
            }
            logicStep.AddWinChipsToControlPanel(jackpotWin,1.37f,true,false,"Symbol_SmallWin_11021");
            taskNiceWin = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskNiceWin, null);
            await taskNiceWin.Task;
            
        }


        protected static int numUpCount = 0;
        protected void UpContainerLayer(ElementContainer containerScatter)
        {
            containerScatter.ShiftSortOrder(true);
            SortingGroup sortingGroup = containerScatter.transform.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = 9999 + numUpCount;
            numUpCount = numUpCount + 1;
        }


        private string[] _jackpotsAddress =
            {"UIJackpotMinor11021", "UIJackpotMajor11021", "UIJackpotGrand11021"};

        private TaskCompletionSource<bool> taskNiceWin;
        protected async Task CollectJackpot(ElementContainer containerScatter,
            RhinoGameResultExtraInfo.Types.DiskItem diskItem, LogicStepProxy logicStep)
        {

            

            List<Task> listTask = new List<Task>();

            AudioUtil.Instance.PlayAudioFxOneShot("B01_Trigger");
            listTask.Add(containerScatter.PlayElementAnimationAsync("Get"));
            UpContainerLayer(containerScatter);
            listTask.Add(PlayCellWin());
            var transitionView = context.view.Get<TransitionView11021>();
            listTask.Add(transitionView.ShowWinShanks());
            await Task.WhenAll(listTask);

            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource, null);
            var assetAddress = _jackpotsAddress[diskItem.JackpotId - 1];

            AudioUtil.Instance.PlayAudioFxOneShot("JpStart_Open");
            var view = PopUpManager.Instance.ShowPopUp<UIJackpot11021>(assetAddress);
            var jackpotWin = context.state.Get<BetState>().GetPayWinChips(diskItem.JackpotPay);
            var betState = context.state.Get<BetState>();
            var isFeatureUnlocked = betState.IsFeatureUnlocked((int)(diskItem.JackpotId - 1));
            
            view.SetJackpotWinNum(jackpotWin);
            view.SetIsJackpot(isFeatureUnlocked);

            view.SetPopUpCloseAction(async () =>
            {

                await AddWinChipsToControlPanel(logicStep, jackpotWin);

                context.RemoveTask(taskCompletionSource);
                taskCompletionSource.SetResult(true);
            });

            await taskCompletionSource.Task;
            
            //containerScatter.ShiftSortOrder(false);
        }



        protected async Task CollectGold(ElementContainer containerScatter,RhinoGameResultExtraInfo.Types.DiskItem diskItem,LogicStepProxy logicStep)
        {
            
            List<Task> listTask = new List<Task>();
            AudioUtil.Instance.PlayAudioFxOneShot("B01_Trigger");
            listTask.Add(containerScatter.PlayElementAnimationAsync("Get"));
            UpContainerLayer(containerScatter);
            listTask.Add(PlayCellWin());
            var transitionView = context.view.Get<TransitionView11021>();
            listTask.Add(transitionView.ShowWinShanks());
            await Task.WhenAll(listTask);
            
            var numWin = context.state.Get<BetState>().GetPayWinChips(diskItem.WinRate);

           
            await AddWinChipsToControlPanel(logicStep, numWin);
            //containerScatter.ShiftSortOrder(false);
        }


        public async Task CollectFreeRetrigger(ElementContainer containerScatter)
        {
            List<Task> listTask = new List<Task>();
            
            
            AudioUtil.Instance.PlayAudioFxOneShot("FreeGme_ExtraSpin");
            listTask.Add(containerScatter.PlayElementAnimationAsync("Free"));
            UpContainerLayer(containerScatter);
            
            var transitionView = context.view.Get<TransitionView11021>();
            AudioUtil.Instance.PlayAudioFxOneShot("Border02");
            listTask.Add(transitionView.ShowFreeShanks());
            
            listTask.Add( PlayCellWin());
            await Task.WhenAll(listTask);
            //containerScatter.ShiftSortOrder(false);
        }
        
        public async Task CollectFreeTrigger(ElementContainer containerScatter)
        {
            List<Task> listTask = new List<Task>();
            AudioUtil.Instance.PlayAudioFxOneShot("B01_Trigger");
            listTask.Add(containerScatter.PlayElementAnimationAsync("Get"));
            UpContainerLayer(containerScatter);

            var transitionView = context.view.Get<TransitionView11021>();
            AudioUtil.Instance.PlayAudioFxOneShot("Border");
            listTask.Add(transitionView.ShowFreeShanks());
            
            listTask.Add( PlayCellWin());
            
            await Task.WhenAll(listTask);
            //containerScatter.ShiftSortOrder(false);
        }

        protected async Task PlayCellWin()
        {
            List<Task> listTask = new List<Task>();
            if (animatorCellTop != null)
            {
                animatorCellTop.Play("Win");
            }

            await XUtility.PlayAnimationAsync(animatorCell, "Win", context);

            if (animatorCellTop != null)
            {
                animatorCellTop.Play("Idle");
            }

            animatorCell.Play("Idle");
        }
    }
}