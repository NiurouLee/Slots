// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/31/21:04
// Ver : 1.0.0
// Description : AlbumBetTipView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class AlbumBetTipView : View<AlbumBetTipViewControler>
    {
        [ComponentBinder("GetCardBg/Slider")]
        public Slider slider;

        [ComponentBinder("GetCardBg/GetCardImg_add")]
        public Transform maxFx;

        public AlbumBetTipView(string address)
            : base(address)
        {
            
        }
        
        public void SetUpView(bool isPortrait)
        {
            if (isPortrait)
            {
                transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            }
        }
    }

    public class AlbumBetTipViewControler : ViewController<AlbumBetTipView>
    {
        protected RepeatedField<SGetCardAlbumInfo.Types.CardProbobilityDisplayItem> itemInfo;

        protected SGetCardAlbumInfo.Types.CardProbobilityDisplayItem currenLevelItemInfo;

        protected float lastProgress;
       
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventBetChanged>(OnBetChange);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnLevelChanged);
        }

        protected void OnLevelChanged(EventPreNoticeLevelChanged evt)
        {
            CalculateCurrentConfigInfo();
        }

        public override void OnViewDidLoad()
        {
            CalculateCurrentConfigInfo();
            base.OnViewDidLoad();
        }

        private void CalculateCurrentConfigInfo()
        {
            itemInfo = Client.Get<AlbumController>().GetBetTipDisplayInfo();

            if (itemInfo != null)
            {
                ulong currentLevel = Client.Get<UserController>().GetUserLevel();
                for (var i = 0; i < itemInfo.Count; i++)
                {
                    if (currentLevel >= itemInfo[i].LevelMin && currentLevel < itemInfo[i].LevelMax)
                    {
                        currenLevelItemInfo = itemInfo[i];
                    } 
                }
            }
        }

        private float GetDropProbability(int maxBetLevel, int currentBetLevel)
        {
            if (currenLevelItemInfo != null)
            {
                if (currenLevelItemInfo.BetCount == 0)
                    return -1;

                if (currentBetLevel <= currenLevelItemInfo.StartBet)
                    return 0;
                
                if (currentBetLevel >= currenLevelItemInfo.StartBet + currenLevelItemInfo.BetCount)
                {
                    return 1;
                }
                
                return (float) (currentBetLevel - currenLevelItemInfo.StartBet) / currenLevelItemInfo.BetCount;
            }

            return -1;
        }

        protected void OnBetChange(EventBetChanged betChanged)
        {
            if (!Client.Get<AlbumController>().IsOpen())
                return;
            if (!Client.Get<AlbumController>().IsUnlocked())
                return;

            var progress = GetDropProbability(betChanged.maxBetLevel,betChanged.betLevel);

            if (lastProgress >= 0)
            {
                ShowBetTip(progress);
            }
        }

        private CancelableCallback _cancelableCallback;
        protected void ShowBetTip(float progress)
        {
            var animator = view.transform.GetComponent<Animator>();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Close") ||
                animator.GetCurrentAnimatorStateInfo(0).IsName("Hide"))
            {
                animator.Play("Open");
            }

            if (view.slider.value < 1.0f)
            {
                view.maxFx.gameObject.SetActive(false);
            }

            if (progress < 1.0)
            {
                view.maxFx.gameObject.SetActive(false);
            }
            
            view.slider.DOValue(progress, 0.5f).OnComplete(() =>
            {
                if (progress >= 1.0)
                {
                    view.maxFx.gameObject.SetActive(true);
                }
            });
            
            if (_cancelableCallback != null)
            {
                _cancelableCallback.CancelCallback();
            }
          
            _cancelableCallback = WaitForSeconds(3, () =>
            {
                animator.Play("Close");
            });
        }
    }
}