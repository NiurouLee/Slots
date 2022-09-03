// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/27/18:00
// Ver : 1.0.0
// Description : CurrencyCoinView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("CurrencyEmerald")]
    public class CurrencyEmeraldView : View<CurrencyEmeraldViewController>
    {
        [ComponentBinder("CurrencyGroup")]
        public RectTransform currencyGroup;
        [ComponentBinder("CurrencyGroup/Emerald/BG")]
        public Button emeraldButton;

        [ComponentBinder("CurrencyGroup/Emerald/CountText")]
        public TextMeshProUGUI emeraldCountText;

        [ComponentBinder("CurrencyGroup/Emerald/Icon")]
        public Transform emeraldIcon;

        [ComponentBinder("CurrencyGroup/Emerald")]
        public RectTransform emeraldGroup;

        public CurrencyEmeraldView(string assetAddress)
            : base(assetAddress)
        {
        }

        public Transform GetEmeraldIcon()
        {
            return emeraldIcon;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            if (ViewManager.Instance.IsPortrait)
            {
                var anchoredPosition = currencyGroup.anchoredPosition;
                currencyGroup.anchoredPosition = new Vector2(0, anchoredPosition.y - MachineConstant.titleOffSetY);

                anchoredPosition = emeraldGroup.anchoredPosition;
                emeraldGroup.anchoredPosition = new Vector2(0, anchoredPosition.y);
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            AdaptUI();
        }

        protected void AdaptUI()
        {
            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    transform.localScale = new Vector3(localScale, localScale, localScale);
                }
            }
        }


        private Tween _emeraldTween;

        public void UpdateEmeraldCount(long startEmeraldNum, long delta, bool hasRollAnimation)
        {
            long target = startEmeraldNum + delta;

            if (_emeraldTween != null)
            {
                _emeraldTween.Kill();
                _emeraldTween = null;
            }

            DOTween.Kill(emeraldCountText);

            if (hasRollAnimation)
            {
                emeraldCountText.text = startEmeraldNum.GetCommaFormat();

                double v = startEmeraldNum;

                _emeraldTween = DOTween.To(() => v, (x) =>
                {
                    v = x;
                    emeraldCountText.text = v.GetCommaFormat();
                }, target, 2.0f).OnComplete(() => { emeraldCountText.text = target.GetCommaFormat(); });
            }
            else
            {
                emeraldCountText.text = target.GetCommaFormat();
            }
        }
    }

    public class CurrencyEmeraldViewController : ViewController<CurrencyEmeraldView>
    {
        protected long emeraldCountNum;


        public override void OnViewDidLoad()
        {
            emeraldCountNum = TopPanel.GetEmeraldCountNum();
            view.UpdateEmeraldCount(emeraldCountNum,0,false);
            
            base.OnViewDidLoad();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            SubscribeEvent<EventEmeraldBalanceUpdate>(OnEmeraldBalanceUpdate);
            SubscribeEvent<EventRefreshUserProfile>(OnRefreshUserProfile);
        }
        
        protected void OnRefreshUserProfile(EventRefreshUserProfile evt)
        {
            emeraldCountNum = (long)Client.Get<UserController>().GetDiamondCount();
            view.UpdateEmeraldCount(emeraldCountNum, 0, false);
        }


        protected void OnEmeraldBalanceUpdate(EventEmeraldBalanceUpdate eventBalanceUpdate)
        {
            view.UpdateEmeraldCount(emeraldCountNum, eventBalanceUpdate.delta, eventBalanceUpdate.hasAnimation);
            emeraldCountNum += eventBalanceUpdate.delta;
        }

        public void ShowCollectFx()
        {
            // var animator = view.emeraldGroup.transform.GetComponent<Animator>();
            // animator.Play("Open");
        }

    }
}