using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class BonusWinView11019: TransformHolder
    {
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoins;

        protected Animator animator;
        
        public BonusWinView11019(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animator = transform.GetComponent<Animator>();
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
            txtCoins.text = "0";
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
            XUtility.PlayAnimationAsync(animator, "BonusWin", context);
            await RefreshWinAsync(winNum + winAdd,true);
        }

        protected long winNum = 0;
        public async Task RefreshWinAsync(long win,bool playAudio)
        {
            
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            context.AddWaitTask(taskCompletionSource,null);

            if (playAudio)
            {
                AudioUtil.Instance.PlayAudioFx("Link_AddPrize");
            }

            var tweener = DOTween.To(() => winNum, (nowWin) =>
            {
                txtCoins.text = nowWin.GetCommaFormat();
            }, win, 1f);
            context.AddTweener(tweener);
            tweener.onComplete += () =>
            {
                context.RemoveTweener(tweener);
                context.RemoveTask(taskCompletionSource);
                winNum = win;
                txtCoins.text = win.GetCommaFormat();
                taskCompletionSource.SetResult(true);
            };

            await taskCompletionSource.Task;
        }

        public void RefreshWin(long win)
        {
            winNum = win;
            txtCoins.text = win.GetCommaFormat();
        }


    }
}