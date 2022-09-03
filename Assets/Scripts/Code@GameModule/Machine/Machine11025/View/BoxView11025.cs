using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class BoxView11025:TransformHolder
    {
        [ComponentBinder("TIps")] 
        protected Transform tip;

        [ComponentBinder("quantity/QuantityText")]
        protected TextMesh priceText;
        
        [ComponentBinder("CurrencyText")]
        protected TextMesh prizeText;
        private ExtraState11025 _extraState;
        public ExtraState11025 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState =  context.state.Get<ExtraState11025>();
                }
                return _extraState;
            }
        }

        private Sequence tipHideSequence;
        protected Animator boxAnimator;
        protected ChameleonGameResultExtraInfo.Types.Shop.Types.ShopTable.Types.ShopItem boxData;
        protected bool activePage;
        public BoxView11025(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
            boxAnimator = transform.GetComponent<Animator>();
            boxAnimator.keepAnimatorControllerStateOnDisable = true;
        }

        public void SetBoxData(ChameleonGameResultExtraInfo.Types.Shop.Types.ShopTable.Types.ShopItem inBoxData,bool isActivePage)
        {
            boxData = inBoxData;
            activePage = isActivePage;
        }

        public ChameleonGameResultExtraInfo.Types.Shop.Types.ShopTable.Types.ShopItem GetBoxData()
        {
            return boxData;
        }
        public void RefreshText()
        {
            if (boxData != null)
            {
                priceText.text = boxData.Price.GetCommaFormat();
                var chips = boxData.TotalWin;//boxData.WinRate * extraState.GetAverageBet() / 100;
                prizeText.text = Tools.GetLeastDigits(chips, 3);
            }
        }

        public void PerformShow()
        {
            transform.gameObject.SetActive(true);
            string idleStateName = GetIdleStateName();
            XUtility.PlayAnimation(boxAnimator,"Open"+idleStateName);
        }

        public void PerformShake()
        {
            XUtility.PlayAnimation(boxAnimator,"Shake");
        }

        public void ShowTip()
        {
            if (tipHideSequence != null)
            {
                tipHideSequence.Kill();
                tipHideSequence = null;
            }
            tip.gameObject.SetActive(true);
            var tempSequence = DOTween.Sequence();
            tempSequence.AppendInterval(2f);
            tempSequence.AppendCallback(() =>
            {
                tip.gameObject.SetActive(false);
            });
            tempSequence.target = context.transform;
            tipHideSequence = tempSequence;
        }

        public void HideTip()
        {
            if (tipHideSequence != null)
            {
                tipHideSequence.Kill();
                tipHideSequence = null;
            }
            tip.gameObject.SetActive(false);
        }

        public async Task PerformOpen()
        {
            if (boxData != null)
            {
                if (boxData.Open)
                {
                    AudioUtil.Instance.PlayAudioFx("Store_Chest_Open");
                    if (boxData.IsSuper)
                    {
                        await XUtility.PlayAnimationAsync(boxAnimator,"Freegame2");
                    }
                    else if (boxData.IsFree)
                    {
                        await XUtility.PlayAnimationAsync(boxAnimator,"Freegame");
                    }
                    else
                    {
                        await XUtility.PlayAnimationAsync(boxAnimator,"CurrencyText");
                    }
                }
            }
        }
        public void RefreshAnimator()
        {
            XUtility.PlayAnimation(boxAnimator, GetIdleStateName());
        }

        public string GetIdleStateName()
        {
            string idleStateName = "";
            if (boxData != null)
            {
                if (boxData.Open)
                {
                    if (boxData.IsSuper)
                    {
                        idleStateName = "FreegameIdle2";
                    }
                    else if (boxData.IsFree)
                    {
                        idleStateName = "FreegameIdle";
                    }
                    else
                    {
                        idleStateName = "CurrencyTextIdle";
                    }
                }
                else
                {
                    if (activePage)
                    {
                        idleStateName = "Idle";
                    }
                    else
                    {
                        idleStateName = "Ashes";
                    }
                }
            }
            return idleStateName;
        }
    }
}