//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-14 15:12
//  Ver : 1.0.0
//  Description : PiggyBankRewardPopup.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIPiggyBankReward")]
    public class PiggyBankRewardPopup: Popup<PiggyBankRewardViewController>
    {
        public Animator animator;
        [ComponentBinder("CollectButton","MainGroup")]
        public Button btnCollect;
        [ComponentBinder("SmashButton","MainGroup")]
        public Button btnSmash;

        [ComponentBinder("IntegralText","IntegralGroup")]
        public Text txtTotalChips;

        [ComponentBinder("Root/MainGroup/RewardGroup/ExtraItem")]
        public Transform rewardGroup;
        [ComponentBinder("Root/MainGroup/RewardGroup/ExtraItem/CommonCell")]
        public Transform commonCell;
        
        
        [ComponentBinder("TagIcon","PigIcon")]
        public Transform transPigFullTag;
        
        

        public PiggyBankRewardPopup(string address)
            : base(address)
        {
            
        }

        protected override void SetUpController(object inExtraData, object inAsyncExtraData)
        { 
            base.SetUpController(inExtraData, inAsyncExtraData);
            animator = transform.GetComponent<Animator>();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            transPigFullTag.gameObject.SetActive(Client.Get<PiggyBankController>().IsPiggyFull);
        }
    }

    public class PiggyBankRewardViewController : ViewController<PiggyBankRewardPopup>
    {
        private int nSmashCount;
        private bool isSmashing = false;
        private const int MAX_SMASH_COUNT = 4;

        private string _productType;
        private Action<Action<bool, FulfillExtraInfo>> _collectActionHandler;
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            BindViewEvent();
        }

        protected void BindViewEvent()
        {
            view.btnCollect.onClick.AddListener(OnBtnCollectClicked);
            view.btnSmash.onClick.AddListener(OnBtnSmashClicked);
        }

        private async void OnBtnSmashClicked()
        {
            if (isSmashing || nSmashCount >= MAX_SMASH_COUNT) 
                return;
            ++nSmashCount;
            if (nSmashCount < MAX_SMASH_COUNT)
            {
                SoundController.PlaySfx("PiggyBank_Hit");
            }
            else
            {
                SoundController.PlaySfx("PiggyBank_Break");
            }
            isSmashing = true;
            XUtility.PlayAnimation(view.animator,"Knock0" + nSmashCount);
            await XUtility.WaitSeconds(0.1f,this);
            view.transform.DOShakePosition(0.2f,10,60);
            await XUtility.WaitSeconds(0.2f,this);
            CheckPiggyFinished();
        }

        private void CheckPiggyFinished()
        {
            isSmashing = false;
            if (nSmashCount >= MAX_SMASH_COUNT)
            {
                view.btnSmash.gameObject.SetActive(false);
            }
        }


        public void SetUpViewContent(VerifyExtraInfo verifyExtraInfo,
            Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (verifyExtraInfo == null || verifyExtraInfo.Item.SubItemList.Count < 0)
                return;
            _collectActionHandler = collectActionHandler;
            string productType = verifyExtraInfo.Item.ProductType;
            _productType = productType;
            RepeatedField<Item> itemList = new RepeatedField<Item>();
            for (int i = 0; i < verifyExtraInfo.Item.SubItemList.Count; i++)
            {
                var item = verifyExtraInfo.Item.SubItemList[i];
                if (item.Type != Item.Types.Type.Coin)
                {
                    itemList.Add(item);
                }
            }
            XItemUtility.InitItemsUI(view.rewardGroup, itemList);
            view.commonCell.SetAsLastSibling();
            view.txtTotalChips.SetText(XItemUtility.GetCoinItem(verifyExtraInfo.Item.SubItemList).Coin.Amount.GetCommaFormat());
                ;
        }

        private void OnBtnCollectClicked()
        {
            SoundController.PlayButtonClick();
            view.btnCollect.interactable = false;
            _collectActionHandler.Invoke((succeeded,fulfillExtraInfo) =>
            {
                if (succeeded)
                {
                    OnFulfilledSucceeded(fulfillExtraInfo);
                }
                else
                {
                    CommonNoticePopup.ShowCommonNoticePopUp("FulfillPaymentFailed");
                    view.Close();
                }
            });
        }
        
        public async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            if (_productType == "coin")
            {
                var coinItem = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.Coin);
                await XUtility.FlyCoins(view.btnCollect.transform,
                    new EventBalanceUpdate(coinItem.Coin.Amount, "IapPiggy"));
                view.Close();
            }
            else
            {
                view.Close();
            }
            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
        }
    }
}