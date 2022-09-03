// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/17/20:08
// Ver : 1.0.0
// Description : GuideWelcomePopup.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIGuideDialogWithReward")]
    public class GuideWelcomePopup:Popup<GuideWelcomeViewController>
    {
        [ComponentBinder("GuideCell1")] public Transform rewardCell1;
        [ComponentBinder("GuideCell2")] public Transform rewardCell2;
        
        [ComponentBinder("Root/CoinEffect")] public Transform transCoinEffect;
        [ComponentBinder("Root/DiamondEffect")] public Transform transDiamondEffect;

        public GuideWelcomePopup(string address)
            : base(address)
        {
            
        }

        public override float GetPopUpMaskAlpha()
        {
            return 0;
        }
    }

    public class GuideWelcomeViewController : ViewController<GuideWelcomePopup>
    {
        private Guide welcomeGuide;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            welcomeGuide = Client.Get<GuideController>().GetWelcomeGuide();

            var reward = welcomeGuide.Reward;

            if (reward.Items.Count >= 2)
            {
                XItemUtility.InitItemUI(view.rewardCell1, reward.Items[0]);
                XItemUtility.InitItemUI(view.rewardCell2, reward.Items[1]);
            }
        }

        public override async void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuidePopWelcome);

            await WaitForSeconds(1.5f);

            Vector3 sourceCoinPos = TopPanel.GetCoinIcon().parent.TransformPoint(TopPanel.GetCoinIcon().localPosition);
            Vector3 toLocalCoinPos = view.transCoinEffect.parent.InverseTransformPoint(sourceCoinPos);
            
            view.transCoinEffect.gameObject.SetActive(true);
            view.transCoinEffect.DOLocalMove(toLocalCoinPos, 1f);
            
            var sourceEmeraldPos = TopPanel.GetEmeraldIcon().parent.TransformPoint(TopPanel.GetEmeraldIcon().localPosition);
            var toLocalEmeraldPos = view.transDiamondEffect.parent.InverseTransformPoint(sourceEmeraldPos);
            view.transDiamondEffect.gameObject.SetActive(true);
            view.transDiamondEffect.DOLocalMove(toLocalEmeraldPos, 1f);
             
            SoundController.PlaySfx("CashCrazy_Coins_Fly");   
            await WaitForSeconds(1f);
            
            view.transCoinEffect.gameObject.SetActive(false);
            view.transDiamondEffect.gameObject.SetActive(false);
            var reward = welcomeGuide.Reward;

            var coinItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Coin);
            var emeraldItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Emerald);
            
            EventBus.Dispatch(new EventGuideFinished(welcomeGuide, async () =>
            {
                var delayTime = 3.5f;
                //TODO ShowFlyFX
                EventBus.Dispatch(new EventCurrencyUpdate((long)coinItem.Coin.Amount, emeraldItem.Emerald.Amount,true,"GuideWelcome",delayTime));
            
                await WaitForSeconds(delayTime);
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuideCollectWelcome);
                view.Close();
            }));
        }
    }
}