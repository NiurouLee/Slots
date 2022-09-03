using System;
using System.Collections;
using System.Collections.Generic;
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
    [AssetAddress("UILottoBonusFree")]
    public class LottoBonusFreePopup : Popup<LottoBonusFreeController>
    {
        [ComponentBinder("pengzhuang")]
        public Transform pengzhuang;

        [ComponentBinder("BGGroup")]
        public Animator BGGroupAnimator;

        [ComponentBinder("Lqiu")]
        public Transform Lqiu;

        [ComponentBinder("Rqiu")]
        public Transform Rqiu;

        [ComponentBinder("End_qiu")]
        public Image endBall;

        public List<Transform> ballList = new List<Transform>();
        public LottoBonusFreeStart startView;
        public LottoBonusFreeFinish finishView;

        public LevelRushGameInfo freeLottoGameInfo;

        public bool _needForceLandscapeScreen = true;

        public LottoBonusFreePopup(string address) : base(address)
        {

        }

        public override bool NeedForceLandscapeScreen()
        {
            return _needForceLandscapeScreen;
        }

        protected override void SetUpController(object inExtraData, object inAsyncExtraData)
        {
            base.SetUpController(inExtraData, inAsyncExtraData);
            if (inExtraData != null && inExtraData is PopupArgs args)
            {
                freeLottoGameInfo = args.extraArgs as LevelRushGameInfo;
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            InitBalls();
            ShowStartView();
            SoundController.PlayBgMusic("Bg_Lotto");
        }

        private void InitBalls() {
            var allBalls = freeLottoGameInfo.BallsConfig;
            viewController.SetMultipleBallColor(allBalls);
            var atlas = viewController.GetAtlas();
            for (var i = 0; i < Lqiu.childCount; ++i)
            {
                var ball = Lqiu.GetChild(i);
                ballList.Add(ball);
                var ballMultipleNumber = (int) allBalls[i].Num;
                var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                SetBallText(ball, Int32.Parse(ballTextNumber), ballMultipleNumber);
            }
            
            for (var i = 0; i < Rqiu.childCount; ++i)
            {
                var ball = Rqiu.GetChild(i);
                ballList.Add(ball);
                var ballMultipleNumber = (int) allBalls[i + 10].Num;
                var ballSpriteName = viewController.GetBallAtlasName(ballMultipleNumber);
                ball.GetComponent<Image>().sprite = atlas.GetSprite(ballSpriteName);
                var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
                SetBallText(ball, Int32.Parse(ballTextNumber), ballMultipleNumber);
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

        public void RefreshEndBall(int ballMultiple)
        {
            var atlas = viewController.GetAtlas();
            // endBallMultiple.sprite = atlas.GetSprite(viewController.GetTextAtlasNameMin(ballMultiple));
            var ballSpriteName = viewController.GetBallAtlasName(ballMultiple);
            var ballTextNumber = ballSpriteName.Substring(ballSpriteName.Length - 1);
            endBall.sprite = atlas.GetSprite(ballSpriteName); 
            SetBallText(endBall.transform, Int32.Parse(ballTextNumber), ballMultiple);
        }

        private async void ShowStartView() {
            if (startView == null) {
                startView = await AddChild<LottoBonusFreeStart>();
                startView.SetViewContent(freeLottoGameInfo);
                startView.SetViewController(viewController);
            }else {
                startView.Show();
            }
        }

        public async void ShowFinishView(SLevelRushGamePlay result) {
            if (finishView == null) {
                finishView = await AddChild<LottoBonusFreeFinish>();
                finishView.SetViewController(viewController);
            }else {
                finishView.Show();
            }
            finishView.SetData(result);
            SoundController.PlaySfx("lotto_ball_win");
            // SoundController.StopSfx("lotto_ball_move");
        }
    }

    public class LottoBonusFreeController: ViewController<LottoBonusFreePopup>
    {
        public Dictionary<int, string> ballColors = new Dictionary<int, string>();

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
        
        private SLevelRushGamePlay sGetLottoGameResult;
        public async void StartGame() {
            // 开始游戏应该播放掉落动画，掉落动画自动切换到loop循环播放
            var animator = view.pengzhuang.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Drop");
            // 小球动画状态切换为drop
            PlayBallDrop();
            SoundController.PlaySfx("lotto_ball_move");
            XUtility.PlayAnimation(view.BGGroupAnimator, "UILottoBonusFree_BGGroup_Show");
            // 摇奖过程时间定义 
            var loopMinTime = 3.0f;
            // 向服务器发送获取结果的请求
            var sendRequestTime = APIManager.Instance.GetServerTime();
            var request = new CLevelRushGamePlay();
            request.Key = view.freeLottoGameInfo.Key;
            XDebug.LogWarning("free lotto game info key:" + request.Key);
            var handler = await APIManagerGameModule.Instance.SendAsync<CLevelRushGamePlay, SLevelRushGamePlay>(request);
            if (handler.ErrorCode == 0)
            {
                sGetLottoGameResult = handler.Response;
                EventBus.Dispatch(new EventUserProfileUpdate(sGetLottoGameResult.UserProfile));
                var ball = sGetLottoGameResult.GameResult.Odds;
                var getResponseTime = APIManager.Instance.GetServerTime();
                var leftTime = loopMinTime - ((getResponseTime - sendRequestTime) / 1000);
                if (leftTime > 0) {
                    await XUtility.WaitSeconds(leftTime);
                }
                view.RefreshEndBall((int)ball);
                view.endBall.gameObject.SetActive(true);
                // 小球动画3.6秒
                await XUtility.WaitSeconds(3.6f);
                view.endBall.gameObject.SetActive(false);
                view.ShowFinishView(sGetLottoGameResult);
            }
            else
                EventBus.Dispatch(new EventOnUnExceptedServerError(handler.ErrorInfo));
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

        public async void ShowPayView()
        {
            if (sGetLottoGameResult != null)
            {
                view._needForceLandscapeScreen = false;
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LottoBonusPayPopup),
                    new PayLottoArgsData(sGetLottoGameResult,
                        view.freeLottoGameInfo), "LottoFree", view.performCategory)));
            }
            await XUtility.WaitSeconds(2);
            view.Close();
        }

        public string GetBallAtlasName(int multiple) {
            return ballColors[multiple];
        }
        
        public void SetMultipleBallColor(RepeatedField<LevelRushGameInfo.Types.BallConfig> allBalls) {
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
            // Debug.Log($"当前list:{LitJson.JsonMapper.ToJsonField(ballColors)}");
        }
    }
}