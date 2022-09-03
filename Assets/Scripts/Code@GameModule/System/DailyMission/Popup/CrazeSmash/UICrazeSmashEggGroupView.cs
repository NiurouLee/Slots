
using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UICrazeSmashEggGroupView : View
    {
        [ComponentBinder("StateGroup/FinishState")]
        public Transform transformFinishState;

        [ComponentBinder("StateGroup/WaitingState")]
        public Transform transformWaitingState;
        
        [ComponentBinder("StateGroup/TheDayState")]
        public Transform transformTheDayState;

        [ComponentBinder("StateGroup/BigPrizeWaitingState")]
        public Transform transformBigPrizeWaiting;
        
        [ComponentBinder("StateGroup/BigPrizeFinishState")]
        public Transform transformBigPrizeFinishState;
 
        [ComponentBinder("StateGroup/TheDayState/PlayButton")]
        public Button buttonPlay;

        [ComponentBinder("StateGroup/WaitingState/ProgressGroup")]
        public Transform transformProgress;

        [ComponentBinder("StateGroup/BigPrizeWaitingState/RewardGroup/BoostGroup")]
        public Transform transformRewardRoot;

        [ComponentBinder("StateGroup/BigPrizeWaitingState/RewardGroup/BoostGroup/CrazeSmashCell")]
        public Transform transformReward;

        [ComponentBinder("StateGroup/BigPrizeWaitingState/RewardGroup/IntegralGroup/IntegralText")]
        public Text textIntegral;

        [ComponentBinder("Bubble")]
        public Animator animatorBubble;  
        
        [ComponentBinder("ToStoreBonus")]
        public Button toStoreBonusButton;
         
        [ComponentBinder("BGGroup/HammerButton")]
        public Button buttonHammer;

        public GameObject[] waypoints;

        public List<GameObject> rewardCells = new List<GameObject>();

        public int stepCount = 7;

        private Action _onButtonPlay;

        protected override void BindingComponent()
        {
            base.BindingComponent();

            transformReward.gameObject.SetActive(false);

            waypoints = new GameObject[stepCount];

            for (int i = 0; i < stepCount; i++)
            {
                waypoints[i] = transformProgress.Find($"FillCell{i + 1}").gameObject;
            }

            buttonPlay.onClick.AddListener(OnButtonPlay);

            buttonHammer.onClick.AddListener(OnButtonHammer);
            
            if(toStoreBonusButton != null)
                toStoreBonusButton.onClick.AddListener(OnToStoreBonusClicked);
        }

        private void OnButtonHammer()
        {
            animatorBubble.Play("Bubble", 0, 0);
        }

        public void OnToStoreBonusClicked()
        {
            if (Client.Get<IapController>().IsStoreBonusCollectable())
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), $"CrazeSmashSilverEgg")));
            }
        }

        private void OnButtonPlay()
        {
            _onButtonPlay?.Invoke();
        }

        public void SetFinishStyle()
        {
            transformFinishState.gameObject.SetActive(true);
            transformTheDayState.gameObject.SetActive(false);
            transformWaitingState.gameObject.SetActive(false);
            transformBigPrizeWaiting.gameObject.SetActive(false);
            transformBigPrizeFinishState.gameObject.SetActive(true);
        }

        public void SetTheDayStyle(bool hasWinBig)
        {
            transformFinishState.gameObject.SetActive(false);
            transformTheDayState.gameObject.SetActive(true);
            transformWaitingState.gameObject.SetActive(true);
            
            transformBigPrizeWaiting.gameObject.SetActive(!hasWinBig);
            transformBigPrizeFinishState.gameObject.SetActive(hasWinBig);
        }

        public void SetWaitingStyle()
        {
            transformFinishState.gameObject.SetActive(false);
            transformTheDayState.gameObject.SetActive(false);
            transformWaitingState.gameObject.SetActive(true);
            transformBigPrizeWaiting.gameObject.SetActive(true);
            transformBigPrizeFinishState.gameObject.SetActive(false);
        }

        public void SetProgress(uint progress)
        {
            if (progress < 0 || progress > stepCount) { return; }

            for (int i = 0; i < stepCount; i++)
            {
                var waypoint = waypoints[i];
                if (waypoint != null)
                {
                    waypoint.SetActive(progress > i);
                }
            }
        }

        private void SetCoin(ulong coin)
        {
            if (textIntegral != null)
            {
                textIntegral.text = coin.GetCommaFormat();
            }
        }
        

        public void SetOnButtonClick(Action onClick)
        {
            _onButtonPlay = onClick;
        }

        public void SetItems(RepeatedField<Item> items)
        {
            ClearItems();
            ulong coinAmount = 0;
            var clone = CrazeSmashController.FilterItems(items, out coinAmount);
            if (clone == null) { return; }
            SetCoin(coinAmount);
            XItemUtility.InitItemsUI(transformRewardRoot, clone, transformRewardRoot.GetChild(0));
        }

        private void ClearItems()
        {
            SetCoin(0);
            for (int i = transformRewardRoot.childCount - 1; i >= 1; i--)
            {
                GameObject.DestroyImmediate(transformRewardRoot.GetChild(i).gameObject);
            }
        }
    }
}