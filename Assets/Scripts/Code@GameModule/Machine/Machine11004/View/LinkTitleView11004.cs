using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class LinkTitleView11004: TransformHolder
    {

        [ComponentBinder("CountText")]
        protected TextMesh txtReSpinCount;

        [ComponentBinder("DescriptionGroup")]
        protected Transform tranDescriptionGroup;

        [ComponentBinder("WinGroup")]
        protected Transform tranWinGroup;

        [ComponentBinder("IntegralText")]
        protected TextMesh txtIntegralText;

        protected Animator animator;
        
        public LinkTitleView11004(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animator = transform.GetComponent<Animator>();
            tranDescriptionGroup.gameObject.SetActive(false);
            tranWinGroup.gameObject.SetActive(false);
        }


        public async Task RefreshReSpinCount(bool isInit,bool isStartSpin)
        {
            ReSpinState reSpinState = context.state.Get<ReSpinState>();
            if (isInit)
            {
                winNum = 0;
                txtReSpinCount.text = reSpinState.ReSpinCount.ToString();
                tranDescriptionGroup.gameObject.SetActive(true);
                tranWinGroup.gameObject.SetActive(false);
                
                XUtility.PlayAnimationAsync(animator, "LinkGame_Open", context);

            }
            else
            {
                

                if (isStartSpin)
                {
                    txtReSpinCount.text = (reSpinState.ReSpinCount - 1).ToString();
                }
                else
                {
                    txtReSpinCount.text = reSpinState.ReSpinCount .ToString();
                    
                    if (reSpinState.ReSpinLimit == reSpinState.ReSpinCount)
                    {
                        await XUtility.PlayAnimationAsync(animator, "Reset", context);
                    }
                }
            }

        }


        public async Task StartWin()
        {
            tranDescriptionGroup.gameObject.SetActive(false);
            tranWinGroup.gameObject.SetActive(true);
            txtIntegralText.text = $"WIN : 0";
            await XUtility.PlayAnimationAsync(animator, "WinGroup_Open", context);
            //animator.Play("WinGroup_Idle");
        }


        public Vector3 GetIntegralPos()
        {
            return txtIntegralText.transform.position;
        }


        public long GetWinNum()
        {
            return winNum;
        }

        public async Task AddWin(long winAdd)
        {
            XUtility.PlayAnimationAsync(animator, "Bingo_Hit", context);
            await RefreshWinAsync(winNum + winAdd);
        }

        protected long winNum = 0;
        public async Task RefreshWinAsync(long win)
        {
            
            
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource,null);
            var tweener = DOTween.To(() => winNum, (nowWin) =>
            {
                txtIntegralText.text =$"WIN : {nowWin.GetCommaFormat()}";
            }, win, 1f);
            context.AddTweener(tweener);
            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                context.RemoveTask(taskCompletionSource);
                winNum = win;
                txtIntegralText.text =$"WIN : {win.GetCommaFormat()}";
                taskCompletionSource.SetResult(true);
            };

            await taskCompletionSource.Task;
        }

        public void RefreshWin(long win)
        {
            winNum = win;
            txtIntegralText.text =$"WIN : {win.GetCommaFormat()}";
        }

        public void Open()
        {
            this.transform.gameObject.SetActive(true);
            RefreshReSpinCount(true,false);
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }
    }
}