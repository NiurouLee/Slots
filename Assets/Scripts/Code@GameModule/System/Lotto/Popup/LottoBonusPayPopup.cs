using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DragonU3DSDK.Network.API.ILProtocol;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class PayLottoArgsData
    {
        public LevelRushGameInfo freeLottoGameInfo;
        public SLevelRushGamePlay freeLottoGameResult;

        public PayLottoArgsData(SLevelRushGamePlay inFreeLottoGameResult, LevelRushGameInfo inFreeLottoGameInfo)
        {
            freeLottoGameInfo = inFreeLottoGameInfo;
            freeLottoGameResult = inFreeLottoGameResult;
        }
    }

    [AssetAddress("UILottoBonusPay")]
    public class LottoBonusPayPopup : Popup<LottoBonusPayController>
    {
        [ComponentBinder("pengzhuang_fufei")]
        public Transform pengzhuang;

        [ComponentBinder("BGGroup")]
        public Animator BGGroupAnimator;

        // [ComponentBinder("Lqiu")]
        public Transform Lqiu;

        // [ComponentBinder("Rqiu")]  // 这里会取到免费的组件
        public Transform Rqiu;

        [ComponentBinder("End_qiu")]
        public Image endBall;

        [ComponentBinder("EffectsBeishu")]
        public Transform EffectsBeishu;

        [ComponentBinder("UI_lottoGoldenLottoBonus")]
        public Transform UI_lottoGoldenLottoBonus;

        [ComponentBinder("UI_lottoAllMultipliers")]
        public Transform UI_lottoAllMultipliers;

        [ComponentBinder("Root/UI_lottoGoldenLottoBonus/Root/MainGroup/Reward")]
        public Text AniMaxReward;
        
        [ComponentBinder("Root/UI_lottoAllMultipliers/Root/MainGroup/Text")]
        public Text MutiplierText;

        public List<Transform> ballList = new List<Transform>();

        public LottoBonusPayUpWin upWinView;
        public LottoBonusPayMultiple multipleView;
        public LottoBonusPayFinish finishView;

        public bool isReplenishmentOrder = false;

        public PayLottoArgsData _payLottoArgsData;
        public LottoPayInfo _lottoPayInfo;

        public LottoBonusPayPopup(string address) : base(address)
        {
            
        }

        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }

        protected override void SetUpController(object inExtraData, object inAsyncExtraData = null)
        {
            base.SetUpController(inExtraData,inAsyncExtraData);
            if (inExtraData != null && inExtraData is LottoPayInfo payInfo)
            {
                _lottoPayInfo = payInfo;
                isReplenishmentOrder = payInfo.isReplenishmentOrder;
            }
            else if (inExtraData != null && inExtraData is PopupArgs args)
            {
                _payLottoArgsData = args.extraArgs as PayLottoArgsData;
            }
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            if (isReplenishmentOrder) {
                // 补单要切换bg音乐
                SoundController.PlayBgMusic("Bg_Lotto"); 
            }
        }

        private async void ShowEnterAnimation() {
            await XUtility.WaitSeconds(0.5f);
            // 开始播动画
            // var ani1 = UI_lottoGoldenLottoBonus.GetComponent<Animator>();
            // await XUtility.PlayAnimationAsync(ani1);
            SoundController.PlaySfx("lotto_WinUpTo");
            UI_lottoGoldenLottoBonus.gameObject.SetActive(true);
            await XUtility.WaitSeconds(2.7f);
            SoundController.PlaySfx("lotto_Multi");
            UI_lottoGoldenLottoBonus.gameObject.SetActive(false);
            UI_lottoAllMultipliers.gameObject.SetActive(true);
            await XUtility.WaitSeconds(2.5f);
            UI_lottoAllMultipliers.gameObject.SetActive(false);
            
            EffectsBeishu.gameObject.SetActive(true);
            SoundController.PlaySfx("lotto_BallMulti");
            // 等待小球变化动画显示 进行小球倍数切换
            await XUtility.WaitSeconds(0.5f);
            // 小球倍数变化
            ChangBallMultiple();

            await XUtility.WaitSeconds(0.5f);
            // 补单不用显示购买界面，会自动开始
            ShowStartView();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            InitBalls();
        }

        private void InitBalls() 
        {
            Lqiu = pengzhuang.Find("Lqiu");
            Rqiu = pengzhuang.Find("Rqiu");

            var atlas = viewController.GetAtlas();
            if (_payLottoArgsData != null)
            {
                // 球初始化是free的，然后变成pay的
                var payBalls = _payLottoArgsData.freeLottoGameResult.PaidGameInfoForShow.BallsConfig;
                viewController.SetMultipleBallColor(payBalls);
                var freeBalls = _payLottoArgsData.freeLottoGameInfo.BallsConfig;
                for (var i = 0; i < Lqiu.childCount; ++i)
                {
                    var ball = Lqiu.GetChild(i);
                    ballList.Add(ball);
                    var ballMultipleNumber = (int) payBalls[i].Num;
                    var minBallMultipleNumber = (int) freeBalls[i].Num;  // 小球要有倍数变化，所以这里开始展示的就是免费的球的倍数
                    var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                    ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                    var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                    SetBallText(ball, Int32.Parse(ballTextNumber), minBallMultipleNumber);
                }

                for (var i = 0; i < Rqiu.childCount; ++i)
                {
                    var ball = Rqiu.GetChild(i);
                    ballList.Add(ball);
                    var ballMultipleNumber = (int) payBalls[i + 10].Num;
                    var minBallMultipleNumber = (int) freeBalls[i + 10].Num;
                    var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                    ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                    var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                    SetBallText(ball, Int32.Parse(ballTextNumber), minBallMultipleNumber);
                }
                var coinItem = XItemUtility.GetItem(_payLottoArgsData.freeLottoGameResult.PaidGameInfoForShow.GameCoinsMax.Items, Item.Types.Type.Coin);
                var maxReward = (long)coinItem.Coin.Amount;
                AniMaxReward.text = maxReward.GetCommaFormat();
                var mutiplier = _payLottoArgsData.freeLottoGameResult.PaidGameInfoForShow.BallsConfig[0].Num /
                                 _payLottoArgsData.freeLottoGameInfo.BallsConfig[0].Num;
                MutiplierText.SetText($"X{mutiplier}");
            }
            else
            {
                var payBalls = _lottoPayInfo.payLottoGameInfo.GameInfo.GameInfoBeforePlay.BallsConfig;
                viewController.SetMultipleBallColor(payBalls);
                for (var i = 0; i < Lqiu.childCount; ++i)
                {
                    var ball = Lqiu.GetChild(i);
                    ballList.Add(ball);
                    var ballMultipleNumber = (int) payBalls[i].Num;
                    var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                    ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                    var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                    SetBallText(ball, Int32.Parse(ballTextNumber), ballMultipleNumber);
                }

                for (var i = 0; i < Rqiu.childCount; ++i)
                {
                    var ball = Rqiu.GetChild(i);
                    ballList.Add(ball);
                    var ballMultipleNumber = (int) payBalls[i + 10].Num;
                    var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                    ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                    var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                    SetBallText(ball, Int32.Parse(ballTextNumber), ballMultipleNumber);
                }
            }
            if (!isReplenishmentOrder) 
            {
                ShowEnterAnimation();
            }
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

        private void ChangBallMultiple()
        {
            if (_payLottoArgsData == null)
                return;

            var payBalls = _payLottoArgsData.freeLottoGameResult.PaidGameInfoForShow.BallsConfig;
            for (var i = 0; i < Lqiu.childCount; ++i)
            {
                var ball = Lqiu.GetChild(i);
                var ballMultipleNumber = (int) payBalls[i].Num;  // 小球要有倍数变化，所以这里开始展示的就是免费的球的倍数
                var spriteName = ball.GetComponent<Image>().sprite.name;
                var indexStr = spriteName.Replace("UI_lotto_bonus_ball_", "");
                var index = indexStr.Replace("(Clone)", "");
                SetBallText(ball,Int32.Parse(index), ballMultipleNumber);
            }
            
            for (var i = 0; i < Rqiu.childCount; ++i)
            {
                var ball = Rqiu.GetChild(i);
                var ballMultipleNumber = (int) payBalls[i + 10].Num;
                var spriteName = ball.GetComponent<Image>().sprite.name;
                var indexStr = spriteName.Replace("UI_lotto_bonus_ball_", "");
                var index = indexStr.Replace("(Clone)", "");
                SetBallText(ball,Int32.Parse(index), ballMultipleNumber);
            }
        } 

        public void RefreshEndBall(int ballMultiple)
        {
            var atlas = viewController.GetAtlas();
            var ballSpriteName = viewController.GetBallAtlasName(ballMultiple);
            var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
            endBall.sprite = atlas.GetSprite(ballSpriteName); 
            SetBallText(endBall.transform, Int32.Parse(ballTextNumber), ballMultiple);
        }

        public async void ShowStartView() 
        {
            if (multipleView == null) 
            {
                SoundController.PlaySfx("lotto_popup02");
                multipleView = await AddChild<LottoBonusPayMultiple>();
                multipleView.SetViewController(viewController);
                multipleView.SetViewContent(_payLottoArgsData);
            } 
        }

        public async void ShowUpWinView() 
        {
            if (upWinView == null) 
            {
                SoundController.PlaySfx("lotto_popup01");
                upWinView = await AddChild<LottoBonusPayUpWin>();
                upWinView.SetViewController(viewController);
                upWinView.SetViewContent(_payLottoArgsData);
            }
            else 
            {
                SoundController.PlaySfx("lotto_popup01");
                upWinView.Show();
            }
        }

        public async void ShowFinishView(Item item) 
        {
            if (finishView == null) 
            {
                finishView = await AddChild<LottoBonusPayFinish>();
                finishView.SetViewController(viewController);
            }
            else 
            {
                finishView.Show();
            }
            finishView.SetData(item);
            SoundController.PlaySfx("lotto_ball_win");
        }

        public void ClosePopup() 
        {
            // 退出之前要把背景音乐恢复为大厅的
            SoundController.RecoverLastMusic();
            Close();
        }

        public override void Close()
        {
            base.Close();
            EventBus.Dispatch(new EventInBoxItemUpdated());
        }
    }

    public class LottoBonusPayController: ViewController<LottoBonusPayPopup>
    {
        private Action<Action<bool, FulfillExtraInfo>> _collectActionHandler;
        private int stopIndex;

        private Dictionary<int, string> ballColors = new Dictionary<int, string>();

        private RepeatedField<Item> rewards;

        private AssetReference atlasReference;
        private SpriteAtlas atlas;

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {"UILottoBonusFreeAtlas"};
            await base.LoadExtraAsyncAssets();
           
            atlasReference = GetAssetReference("UILottoBonusFreeAtlas");
            if (atlasReference != null)
            {
                atlas = atlasReference.GetAsset<SpriteAtlas>();   
            }
        }

        public SpriteAtlas GetAtlas()
        {
            return atlas;
        }

        public void TryToPay() 
        {
            var date = view._payLottoArgsData.freeLottoGameResult.ShopGamePayItem;
            Client.Get<IapController>().BuyProduct(date);
            var extras = new Dictionary<string, string>();
            extras.Add("productId", date.ProductId);
            extras.Add("productType", date.ProductType);
            extras.Add("price", date.Price.ToString());
            // BiManagerGameModule.Instance.SendGameEvent(BiEventSpinX.Types.GameEventType
            //     .GameEventStorePurchase, extras);
        }

        public ShopItemConfig GetShopItemConfig()
        {
            return view._payLottoArgsData.freeLottoGameResult.ShopGamePayItem;
        }

        public async void StartGame(Item item) 
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushPaypop, ("OperationId", "1"));
            // 补单的时候需要等界面完全显示
            if (view.isReplenishmentOrder) {
                await XUtility.WaitSeconds(1.0f);
            }
            // 开始游戏应该播放掉落动画，掉落动画自动切换到loop循环播放
            var animator = view.pengzhuang.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Drop");
            // 小球动画状态切换为drop
            SoundController.PlaySfx("lotto_ball_move");
            XUtility.PlayAnimation(view.BGGroupAnimator, "UILottoBonusFree_BGGroup_Show");
            PlayBallDrop();
            // 摇奖过程时间定义 
            var loopMinTime = 3.0f;
            var leftTime = loopMinTime; // 付费的时间要是固定的，成功过后才开始
            if (leftTime > 0) {
                await XUtility.WaitSeconds(leftTime);
            }
            view.RefreshEndBall(stopIndex);
            view.endBall.gameObject.SetActive(true);
            // 小球动画3.6秒
            await XUtility.WaitSeconds(3.6f);
            view.endBall.gameObject.SetActive(false);
            view.ShowFinishView(item);
        }

        private async void PlayBallDrop()
        {
            for (var i = 0; i < view.ballList.Count; ++i) {
                var ball = view.ballList[i];
                var ani = ball.GetComponent<Animator>();
                XUtility.PlayAnimation(ani, "Drop");
                await WaitForSeconds(0.1f);
            }
        }
        
        public void SetUpViewContent(VerifyExtraInfo inVerifyExtraInfo, Action<Action<bool, FulfillExtraInfo>> collectActionHandler)
        {
            if (inVerifyExtraInfo == null || inVerifyExtraInfo.Item.SubItemList.Count < 0)
                return;

            if (view.multipleView != null)
            {
                if (view.multipleView.isHide)
                    view.upWinView.SetBtnStatus(false);
                else
                    view.multipleView.SetBtnStatus(false);
            }
            
            _collectActionHandler = collectActionHandler;
           
            _collectActionHandler.Invoke((succeeded, fulfillExtraInfo) =>
            {
                if (succeeded)
                {
                    OnFulfilledSucceeded(fulfillExtraInfo);
                }
                else
                {
                    CommonNoticePopup.ShowCommonNoticePopUp("FulfillPaymentFailed");
                }
            });
        }

        public void CollectSuccess()
        {
            // Buff之类的数据更新，需要重新从服务器拉去状态
            ItemSettleHelper.SettleItems(rewards, null);
        }

        private async void OnFulfilledSucceeded(FulfillExtraInfo fulfillExtraInfo)
        {
            var item = XItemUtility.GetItem(fulfillExtraInfo.RewardItems, Item.Types.Type.LevelRushPaidGame);
            if (item == null)
            {
                XDebug.LogError("Lotto 购买没有LevelRushPaidGame Item，配表有问题！！！！！");
                return;
            }

            rewards = fulfillExtraInfo.RewardItems;
            EventBus.Dispatch(new EventUserProfileUpdate(fulfillExtraInfo.UserProfile));
            stopIndex = (int) item.LevelRushPaidGame.GameInfo.GameResult.Odds;
            if (view.isReplenishmentOrder) 
            {
                StartGame(item);
            }
            else 
            {
                if (view.multipleView.isHide)
                    view.upWinView.paySuccess(item);
                else
                    view.multipleView.paySuccess(item);
            }

        }

        public void TryToShowUpWin() 
        {
            view.ShowUpWinView();
        }

        public async void ClosePopup() 
        {
            view.ClosePopup();
        }

        // 倍数顺序依次为3,4,5,6,10，20,50,100
        public string GetBallAtlasName(int multiple) {
            return ballColors[multiple];
        }

        public void SetMultipleBallColor(RepeatedField<LevelRushGameInfo.Types.BallConfig> allBalls) 
        {
            var curMultiple = (int)allBalls[0].Num;
            var curBall = 1;
            ballColors.Add(curMultiple, "UI_lotto_bonus_ball_" + curBall);
            for (var i = 1; i < allBalls.Count; i ++) {
                if ((int)allBalls[i].Num != curMultiple) {
                    curMultiple = (int)allBalls[i].Num;
                    curBall += 1;
                    ballColors.Add(curMultiple, "UI_lotto_bonus_ball_" + curBall);
                }
            }
        }
    }

    public class LottoPayInfo {
        public bool isReplenishmentOrder = false;
        public Item.Types.LevelRushPaidGame payLottoGameInfo;

        public LottoPayInfo(Item.Types.LevelRushPaidGame inPayLottoGameInfo, bool isReplenishmentOrder = false)
        {
            payLottoGameInfo = inPayLottoGameInfo;
            this.isReplenishmentOrder = isReplenishmentOrder;
        } 
    }
}