using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;


namespace GameModule {
    [AssetAddress("UILottoBonusFreeStart")]
    public class LottoBonusFreeStart: View {
        [ComponentBinder("Button")]
        public Button playButton;

        [ComponentBinder("Multiple")]
        public Text multipleText;

        [ComponentBinder("Reward")]
        public Text rewardText;

        public LottoBonusFreeController parentController;
        public LottoBonusFreeStart(string address):base(address) {
            
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

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            playButton.onClick.AddListener(OnClickPlayButton);
        }

        public void SetViewContent(LevelRushGameInfo freeLottoGameInfo)
        {
            XDebug.LogOnExceptionHandler("FreeLotto Base Coin:" + freeLottoGameInfo.CoinBase);
            var coins = (long) freeLottoGameInfo.CoinBase;
            XDebug.LogOnExceptionHandler("FreeLotto Base Coin:" + coins);
            rewardText.text =  coins.GetCommaOrSimplify(9);  // 最终策划还是说显示这个基础值。。。。
            multipleText.text = "X" + freeLottoGameInfo.MaxOdds;
        }

        public void SetViewController(LottoBonusFreeController viewController) {
            parentController = viewController;
        }

        private void OnClickPlayButton()
        {
            playButton.interactable = false;
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushFreeplay);
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", Hide);
            parentController.StartGame();
        }
    }
}