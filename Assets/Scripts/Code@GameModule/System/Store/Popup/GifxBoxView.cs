// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/16/21:14
// Ver : 1.0.0
// Description : GifxBoxView.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class GiftBoxView:View<GiftBoxViewController>
    {
        [ComponentBinder("CoinsCollectGroup")]
        public Transform coinsCollectGroup;

        [ComponentBinder("GiftBoxDescriptionText")]
        public TextMeshProUGUI giftBoxDescriptionText;
        
        [ComponentBinder("TimerText")]
        public TextMeshProUGUI timerText;
        
    }

    public class GiftBoxViewController : ViewController<GiftBoxView>
    {
        private float _countdownTime = 0;
        private float _updateTime;
        private GiftBox _giftBoxInfo;
        public void SetUpView(GiftBox inGiftBoxInfo)
        {
            _giftBoxInfo = inGiftBoxInfo;
            
            _countdownTime = _giftBoxInfo.CountdownTime;
            _updateTime = Time.realtimeSinceStartup;
             
            var childCount = view.coinsCollectGroup.childCount;

            for (var i = 0; i < childCount; i++)
            {
                view.coinsCollectGroup.GetChild(i).Find("Icon").gameObject.SetActive(_giftBoxInfo.ProgressNow > i);
            }

            if (_giftBoxInfo.ProgressNow == 0)
            {
                view.timerText.transform.parent.gameObject.SetActive(false);
                view.giftBoxDescriptionText.text = "START COLLECTING GIFT BOX CHIPS!";
                DisableUpdate();
            }
            else
            {
                view.giftBoxDescriptionText.text =
                    _giftBoxInfo.AccumulateCoins.GetAbbreviationFormat() + " COINS IS WAITING FOR YOU!";
                
                view.timerText.transform.parent.gameObject.SetActive(true);
               
                var totalDay = TimeSpan.FromSeconds(_countdownTime).TotalDays;
               
                if (totalDay >= 2)
                {
                    view.timerText.text = Math.Floor(totalDay) + "Days";
                } else if (totalDay > 1)
                {
                    view.timerText.text = Math.Floor(totalDay) + "Day";
                }
                else
                {
                    view.timerText.text = Math.Floor(totalDay) + "Day";
                }

                if (totalDay > 0 && totalDay < 1)
                {
                    view.timerText.text = TimeSpan.FromSeconds(_countdownTime).ToString(@"hh\:mm\:ss");
                    EnableUpdate(1);
                }
                else
                {
                    DisableUpdate();
                }
            }
        }

        protected float GetLeftTime()
        {
            return _countdownTime - (Time.realtimeSinceStartup - _updateTime);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventGiftBoxSetToEmpty>(SetGiftBoxViewToEmpty);
        }

        protected void SetGiftBoxViewToEmpty(EventGiftBoxSetToEmpty evt)
        {
            GiftBox giftBox = new GiftBox();
            giftBox.ProgressNow = 0;
            SetUpView(giftBox);
        }

        public override void Update()
        {
            var leftTime = GetLeftTime();

            if (leftTime > 0)
            {
                view.timerText.text = XUtility.GetTimeText(GetLeftTime());
            }
            else
            {
                DisableUpdate();
                _giftBoxInfo.AccumulateCoins = 0;
                _giftBoxInfo.ProgressNow = 0;
                _giftBoxInfo.CountdownTime = 0;
                SetUpView(_giftBoxInfo);
            }
        }
    }
}