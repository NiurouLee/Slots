using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class BonusClollectionPop11029 : TransformHolder
    {
        private Animator animator;

        private ExtraState11029 _extraState11029;

        [ComponentBinder("Root/BGGroup/IntegralText")]
        protected TextMesh _textJackpotWinNum;


        public BonusClollectionPop11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public async Task ShowCollectionPop(long chips)
        {
            AudioUtil.Instance.PlayAudioFx("WalletEnd_Open");
            transform.gameObject.SetActive(true);

            animator.Play("Open");
            
            _extraState11029 = context.state.Get<ExtraState11029>();
            if (_textJackpotWinNum)
            {
                // var wheel = _extraState11029.GetWheelData();
                // var wheelItems = wheel.Items;
                // var index = (int) wheel.Index;
                // ulong bonusWin = wheel.Bet * (wheelItems[index].WinRate + wheelItems[index].JackpotPay) / 100;
                _textJackpotWinNum.SetText(chips.GetCommaFormat());
                long v = 0;
                var target = chips;
                DOTween.To(() => v, (x) =>
                {
                    v = x;
                    if (_textJackpotWinNum)
                        _textJackpotWinNum.SetText(v.GetCommaFormat());
                }, target, 2.5f).OnComplete(() =>
                {
                    if (_textJackpotWinNum)
                        _textJackpotWinNum.SetText(chips.GetCommaFormat());
                });
            }
            //弹窗消失
            await context.WaitSeconds(3.0f);
            animator.Play("Close");
            
            //FreeSpin结束之后这里BAG表演需要加钱
            
            if (_extraState11029.NeedWheelSettle())
            {
                var bonusProxy11029 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11029;
                await bonusProxy11029.BonusBagAddWin();
            }
            await context.WaitSeconds(0.59f);
            transform.gameObject.SetActive(false);
        }
    }
}
