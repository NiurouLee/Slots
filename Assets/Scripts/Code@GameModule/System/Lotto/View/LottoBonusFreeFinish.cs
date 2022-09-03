using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;


namespace GameModule {
    [AssetAddress("UILottoBonusFreeFinish")]
    public class LottoBonusFreeFinish: View {
        [ComponentBinder("Button")]
        public Button collectButton;

        [ComponentBinder("RewardText")]
        public Text rewardText;

        [ComponentBinder("MultipleText")]
        public Text multipleText;

        [ComponentBinder("VIPBounsText")]
        public Text VIPBounsText;

        [ComponentBinder("Result")]
        public Text resultRewardText;

        [ComponentBinder("UILottoBonusBall")]
        public Image ballImage;

        private AssetReference atlasReference;
        private SpriteAtlas atlas;

        public LottoBonusFreeController parentController;

        private SLevelRushGamePlay result;

        public LottoBonusFreeFinish(string address):base(address) 
        {

        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            var scaleInfo = CalculateScaleInfo();
            transform.localScale = scaleInfo;
        }

        public virtual Vector3 CalculateScaleInfo()
        {
            Vector2 contentDesignSize = new Vector2(1365, 1000);
            if(contentDesignSize == Vector2.zero)
                return Vector3.one;

            var viewSize = ViewResolution.referenceResolutionLandscape;

            if (viewSize.x < contentDesignSize.x)
            {
                var scale = viewSize.x / contentDesignSize.x;
                return new Vector3(scale, scale, scale);
            }

            return Vector3.one;
        }
        
        public override void Destroy()
        {
            if (atlasReference != null)
            { 
                atlasReference.ReleaseOperation();
            }
            base.Destroy();
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            collectButton.onClick.AddListener(OnClickCollectButton);
            InitView();
           
        }

        private void InitView() {
            rewardText.text = "0";
            multipleText.text = "0";
            VIPBounsText.text = "0";
            resultRewardText.text = "0";
        }

        public async void SetData(SLevelRushGamePlay inResult)
        {
            result = inResult;
            var vipAddNum = result.GameResult.VipBonusPercentage;
            collectButton.enabled = true;

            var ballMultiple = (int) result.GameResult.Odds;
            // 动态更新图片的方法
            if (atlas == null)
            {
                atlasReference = await AssetHelper.PrepareAssetAsync<SpriteAtlas>("UILottoBonusFreeAtlas");
                if (atlasReference != null)
                {
                    atlas = atlasReference.GetAsset<SpriteAtlas>();   
                }
            }
            if (atlas != null)
            { 
                ballImage.sprite = atlas.GetSprite(parentController.GetBallAtlasName(ballMultiple)); 
                var ballSpriteName = parentController.GetBallAtlasName(ballMultiple);
                var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                SetBallText(ballImage.transform, Int32.Parse(ballTextNumber), ballMultiple);
            }

            var coinBase = (long) result.GameResult.CoinBase;
            rewardText.text = coinBase.GetCommaOrSimplify(7);
            multipleText.text = ballMultiple.ToString();
            VIPBounsText.text = vipAddNum + "%";
            var coinItem = XItemUtility.GetItem(result.GameResult.Reward.Items, Item.Types.Type.Coin);
            var coin = (long) coinItem.Coin.Amount;
            resultRewardText.text = coin.GetCommaOrSimplify(9);
        }
        
        private void SetBallText(Transform ball, int index, int multiple)
        {
            for (int j = 1; j < 9; j++)
            {
                var multipleText = ball.Find($"Root/Text/MultipleText{j}").GetComponent<TextMeshProUGUI>();
                if (j==index)
                {
                    multipleText.gameObject.SetActive(true);     
                    multipleText.text = "X" + multiple;
                }
                else
                {
                    multipleText.gameObject.SetActive(false);       
                }
            }
        }

        public void SetViewController(LottoBonusFreeController viewController) {
            parentController = viewController;
        }

        private async void OnClickCollectButton() {
            collectButton.enabled = false;
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushFreecollect);
            var coinItem = XItemUtility.GetItem(result.GameResult.Reward.Items, Item.Types.Type.Coin);
            var rewardNum = coinItem.Coin.Amount;
            await XUtility.FlyCoins(collectButton.transform, new EventBalanceUpdate(rewardNum, "LottoBonusFreePopup"));
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", FinishOver);
        }

        private void FinishOver() {
            Hide();
            parentController.ShowPayView();
        }
    }
}