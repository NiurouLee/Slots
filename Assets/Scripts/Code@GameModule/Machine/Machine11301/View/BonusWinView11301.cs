using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class BonusWinView11301: TransformHolder
    {
        
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoins;

        [ComponentBinder("LinkGame_Win")]
        protected Animator animator;
        
        public BonusWinView11301(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            
            animator.gameObject.SetActive(false);
            transform.gameObject.SetActive(false);
        }
        
        
        
        public override bool MatchFilter(string filter)
        {
            return transform.name == filter;
        }

        public void Open()
        {
            this.transform.gameObject.SetActive(true);
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }
        
        
        
        
        public async Task StartWin()
        {
            txtCoins.SetText("0");
            winNum = 0;
            //await XUtility.PlayAnimationAsync(animator, "WinGroup_Open", context);

        }


        public Vector3 GetIntegralPos()
        {
            return txtCoins.transform.position;
        }


        public long GetWinNum()
        {
            return winNum;
        }

        public async Task AddWin(long winAdd)
        {
            AudioUtil.Instance.PlayAudioFx("Link_Settlement");
            animator.gameObject.SetActive(true);
            XUtility.PlayAnimation(animator, "Blink", () =>
            {
                animator?.gameObject?.SetActive(false);
            }, context);
            await RefreshWinAsync(winNum + winAdd);
        }

        protected long winNum = 0;
        public async Task RefreshWinAsync(long win)
        {
            
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource,null);
            var tweener = DOTween.To(() => winNum, (nowWin) =>
            {
                txtCoins.SetText(nowWin.GetCommaFormat());
            }, win, 1f);
            context.AddTweener(tweener);
            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                context.RemoveTask(taskCompletionSource);
                winNum = win;
                txtCoins.SetText(win.GetCommaFormat());
                taskCompletionSource.SetResult(true);
            };

            await taskCompletionSource.Task;
        }

        public void RefreshWin(long win)
        {
            winNum = win;
            txtCoins.SetText(win.GetCommaFormat());
        }
        
        
    }
}