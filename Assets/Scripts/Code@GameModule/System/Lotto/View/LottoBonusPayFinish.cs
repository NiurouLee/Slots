using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;


namespace GameModule {
    [AssetAddress("UILottoBonusPayFinish")]
    public class LottoBonusPayFinish: View {
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

        public Item result;

        public ulong resultReward = 0;
       
        public LottoBonusPayController parentController;
        public LottoBonusPayFinish(string address):base(address) 
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

        public async void SetData(Item reward)
        {
            result = reward;
            collectButton.enabled = true;
            var ballMultiple = (int)reward.LevelRushPaidGame.GameInfo.GameResult.Odds;
            var coinItem = XItemUtility.GetItem(reward.LevelRushPaidGame.GameInfo.GameResult.Reward.Items,
                Item.Types.Type.Coin);
            resultReward = coinItem.Coin.Amount;
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
            
            var coinBase = (long) reward.LevelRushPaidGame.GameInfo.GameResult.CoinBase;
            rewardText.text = coinBase.GetCommaOrSimplify(7);
            
            multipleText.text = ballMultiple.ToString();
            VIPBounsText.text = reward.LevelRushPaidGame.GameInfo.GameResult.VipBonusPercentage + "%";
            var coin = (long) resultReward;
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

        public void SetViewController(LottoBonusPayController viewController) {
            parentController = viewController;
        }

        private async void OnClickCollectButton()
        {
            collectButton.interactable = false;
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushPaycollect);
            await XUtility.FlyCoins(collectButton.transform, new EventBalanceUpdate(resultReward, "LottoBonusPay"));
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", FinishOver);
        }

        private void FinishOver() {
            Hide();
            parentController.CollectSuccess();
            parentController.ClosePopup();
        }
    }
}