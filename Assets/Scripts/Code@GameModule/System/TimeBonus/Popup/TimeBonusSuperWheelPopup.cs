// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/16:25
// Ver : 1.0.0
// Description : TimeBonusSuperWheelPopup.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITimerBonusSuperWheel")]
    public class TimeBonusSuperWheelPopup:Popup<TimeBonusSuperWheelViewController>
    {
        [ComponentBinder("UITimerBonusWheel1")]
        public Transform timerBonusWheel1;
        
        [ComponentBinder("UITimerBonusWheel2")]
        public Transform timerBonusWheel2;
        
        [ComponentBinder("UITimerBonusWheel3")]
        public Transform timerBonusWheel3;  
         
        [ComponentBinder("UITimerBonusFinish")]
        public Transform _timerBonusCollectView;  
        
        [ComponentBinder("UITimerBonusNextWheel")]
        public Transform _uiTimerBonusNextWheel;

        [ComponentBinder("EmeraldIntegralText")]
        public Text emeraldIntegralText;   
        
        [ComponentBinder("WinGroup")]
        public Transform winGroup; 
        
        [ComponentBinder("MultiplesGroup")]
        public Transform multiplesGroup;  
        
        [ComponentBinder("EmeraldWinGroup")]
        public Transform emeraldWinGroup; 
        
        [ComponentBinder("LeftGroup")]
        public Transform leftGroup;      
        
        [ComponentBinder("ContentCenter")]
        public Transform contentCenter; 
        
        [ComponentBinder("MultipleText")]
        public Text multipleText;
        
        [ComponentBinder("CoinIntegralText")]
        public Text coinIntegralText; 
        
        [ComponentBinder("SuperWheelCollectFx")]
        public Transform superWheelCollectFx; 
        
        [ComponentBinder("UITimerBonusSpinNow")]
        public Transform uiTimerBonusSpinNow;
         
        public Vector3 superWheelCollectFlyStartPosition;
  
        public TimeBonusSuperWheelView[] timeBonusSuperWheelViews;

        public WheelBonusCollectView wheelBonusCollectView;

        public TimeBonusSuperWheelPopup(string address)
            : base(address)
        {
            
        }

        public override bool NeedForceLandscapeScreen()
        {
            return true;
        }

        protected override void SetUpExtraView()
        {
            timeBonusSuperWheelViews = new TimeBonusSuperWheelView[3];
            
            timeBonusSuperWheelViews[0] = AddChild<TimeBonusSuperWheelView>(timerBonusWheel1);
            timeBonusSuperWheelViews[1] = AddChild<TimeBonusSuperWheelView>(timerBonusWheel2);
            timeBonusSuperWheelViews[2] = AddChild<TimeBonusSuperWheelView>(timerBonusWheel3);
           
            wheelBonusCollectView = AddChild<WheelBonusCollectView>(_timerBonusCollectView);

            emeraldIntegralText.text = "0";
            multipleText.text = "1";
            coinIntegralText.text = "0";

            superWheelCollectFlyStartPosition = superWheelCollectFx.localPosition;
            superWheelCollectFx.parent.gameObject.SetActive(false);

            if (ViewResolution.referenceResolutionLandscape.x < 1480)
            {
                float scale = ViewResolution.referenceResolutionLandscape.x / 1480.0f;
                contentCenter.localScale = new Vector3(scale, scale, scale);
            }
        }

        public override string GetOpenAudioName()
        {
            return "";
        }
        
        public override string GetCloseAudioName()
        {
            return "Wheel_Screen";
        }
        
        public override void Close()
        {
            base.Close();
            SoundController.RecoverLastMusic();
        }
    }

    public class TimeBonusSuperWheelViewController : ViewController<TimeBonusSuperWheelPopup>
    {
        private long _totalCoins;
        private long _totalMultiple;
        private long _totalEmerald;

        private int _curentWheelIndex;

        private int _currentSpinCount;

        private SSpinWheel _spinResult;
        private SGetWheelBonus _wheelBonus;

        public override void BindingView(View inView, object inExtraData, object inAsyncExtraData = null)
        {
            base.BindingView(inView, inExtraData);

            _wheelBonus = inAsyncExtraData as SGetWheelBonus;
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            var viewCount = view.timeBonusSuperWheelViews.Length;

            for (var i = 0; i < viewCount; i++)
            {
                view.timeBonusSuperWheelViews[i].SetWheelIndex(i);
                view.timeBonusSuperWheelViews[i].InitializeWheelView(_wheelBonus.WheelBonus[i], 5);
                view.timeBonusSuperWheelViews[i].ToState(i == 0 ? "Idle": "Lock");
            }
            
            view.timeBonusSuperWheelViews[0].spinButton.onClick.AddListener(OnWheelOneSpinButtonClicked);
            
            SoundController.PlayBgMusic("Bg_3Wheel");
        }

    
        /// <summary>
        /// 点击SPIN按钮
        /// 转轮盘
        /// 出结果
        /// 收集轮盘赢钱
        /// 如果是钥匙，就进级
        /// 切换轮盘
        /// 解锁，Complete
        /// </summary>
        // public override void OnViewEnabled()
        // {
        //     base.OnViewEnabled();
        // }
        //
        public void OnWheelOneSpinButtonClicked()
        {
            view.timeBonusSuperWheelViews[0].spinButton.interactable = false;
            
            _currentSpinCount = 0;
            _curentWheelIndex = 0;
            _totalCoins = 0;
            _totalEmerald = 0;
            _totalMultiple = 1;
            
            view.uiTimerBonusSpinNow.parent.gameObject.SetActive(false);
            
            DoWheelSpinProcess();
        }

        public async void DoWheelSpinProcess()
        {
            while (!IsSpinFinished())
            {
                if (NeedSwitchWheel())
                {
                    _curentWheelIndex++;
                    _currentSpinCount = 0;
                    await SwitchWheel(_curentWheelIndex);
                }
                
                await SpinWheelOnce();
                
                await CollectWheelSpinReward();
                _currentSpinCount++;
            }

            var source = GetTriggerSource();
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusExtraWheelCollect, 
                ("win",_totalCoins.ToString()),
                ("multiple", _totalMultiple.ToString()),
                ("emerald", _totalEmerald.ToString()),
                ("source",source));

            view.wheelBonusCollectView.SetCollectViewData(this.view, _spinResult.Item, () =>
            {
                view.Close();
            }, "SUPER WHEEL", (claimProcessHandler) =>
            {
                Client.Get<TimeBonusController>().ClaimWheelBonus(claimProcessHandler);
            });

            await WaitForSeconds(2);
            
            view.animator.Play("SuperWheelCollectAppear");
        }
        

        public bool NeedSwitchWheel()
        {
            if (_spinResult == null)
                return false;

            if (_currentSpinCount == _spinResult.Result[_curentWheelIndex].HitWedgeId.Count)
            {
                return true;
            }
            
            return false;
        }

        public bool IsSpinFinished()
        {
            if (_spinResult == null)
            {
                return false;
            }

            if (_curentWheelIndex == _spinResult.Result.Count - 1
                && _currentSpinCount == _spinResult.Result[_curentWheelIndex].HitWedgeId.Count)
            {
                return true;
            }
            
            return false;
        }
        
        public async Task<uint>GetSpinWheelHitIndex(int wheelIndex)
        {
            if (_spinResult == null)
            {
                _spinResult = await Client.Get<TimeBonusController>().GetWheelSpinResult(TimerBonusWheelId.SuperWheel);
            }

            return _spinResult.Result[wheelIndex].HitWedgeId[_currentSpinCount] - 1;
        }

        protected async Task SpinWheelOnce()
        {
          
            // SoundController.PlaySfx("Wheel_Spin2_1");
            //     
            // CancelableCallback finishCallback = new CancelableCallback(() =>
            // {
            //     SoundController.PlaySfx("Wheel_Spin2_2",true);
            // });
            SoundController.PlaySfx("Wheel_Spin3");
              
            view.timeBonusSuperWheelViews[_curentWheelIndex].StartSpinWheel();
            
            var hitWedgeId = await GetSpinWheelHitIndex(_curentWheelIndex);
            
            await WaitForSeconds(1.2f);
            
            // finishCallback.CancelCallback();
            
            WaitForSeconds(2, () =>
            {
                PlayWinFx();
            });
            
            await view.timeBonusSuperWheelViews[_curentWheelIndex].StopWheel((int)hitWedgeId);
          
           
            await WaitForSeconds(0.5f);
            
          
            await view.timeBonusSuperWheelViews[_curentWheelIndex].ShowWinEffect();
        }

        protected async Task SwitchWheel(int wheelIndex)
        {
            if (wheelIndex == 1)
            {
                view.timeBonusSuperWheelViews[0].animator.Play("To_Complete");
                
                view._uiTimerBonusNextWheel.transform.parent.gameObject.SetActive(true);
                view._uiTimerBonusNextWheel.GetComponent<Animator>().Play("Green_Open");
                SoundController.PlaySfx("Wheel_Unlock");
                await PlayAnimationAsync(view.animator, "WheelOneSwitch");
                view._uiTimerBonusNextWheel.transform.parent.gameObject.SetActive(false);
                await PlayAnimationAsync(view.timeBonusSuperWheelViews[1].animator, "Unlock_Wheel");
            }
            else
            {
                view._uiTimerBonusNextWheel.transform.parent.gameObject.SetActive(true);
                view._uiTimerBonusNextWheel.GetComponent<Animator>().Play("Red_Open");
                view.timeBonusSuperWheelViews[1].animator.Play("To_Complete");
                SoundController.PlaySfx("Wheel_Unlock");
                await PlayAnimationAsync(view.animator, "WheelTwoSwitch");
                view._uiTimerBonusNextWheel.transform.parent.gameObject.SetActive(false);
                await PlayAnimationAsync(view.timeBonusSuperWheelViews[2].animator, "Unlock_Wheel");
               
            }
        }

        protected async Task ShowCollectFx(Vector3 endPosition)
        {
            view.superWheelCollectFx.parent.gameObject.SetActive(true);
            view.superWheelCollectFx.localPosition = view.superWheelCollectFlyStartPosition;
            await XUtility.FlyLocalAsync(view.superWheelCollectFx, view.superWheelCollectFlyStartPosition,
                endPosition, 50, 0.5f); 
            view.superWheelCollectFx.parent.gameObject.SetActive(false);
        }

        protected void PlayWinFx()
        {
            var hitWedgeId = _spinResult.Result[_curentWheelIndex].HitWedgeId[_currentSpinCount] - 1;
            var wedgeInfo = _wheelBonus.WheelBonus[_curentWheelIndex].Wedge[(int) hitWedgeId];

            if (wedgeInfo.RewardType == "coin" || wedgeInfo.RewardType == "emerald" ||
                wedgeInfo.RewardType == "multiple")
            {
                SoundController.PlaySfx("Wheel_GemsWin");
                return;
            }

            SoundController.PlaySfx("Wheel_KeyWin");
        }

        protected async Task CollectWheelSpinReward()
        {
            var hitWedgeId = _spinResult.Result[_curentWheelIndex].HitWedgeId[_currentSpinCount] - 1;
            var wedgeInfo = _wheelBonus.WheelBonus[_curentWheelIndex].Wedge[(int)hitWedgeId];

            if (wedgeInfo.RewardType == "coin")
            {
                var item = wedgeInfo.Item;
                if (item.Type == Item.Types.Type.Coin)
                {
                    _totalCoins += (long) item.Coin.Amount;
                }

                SoundController.PlaySfx("Wheel_CoinsWin_fly");
                await ShowCollectFx(view.winGroup.localPosition);

                view.leftGroup.GetComponent<Animator>().Play("Win",-1,0);

                view.coinIntegralText.text = _totalCoins.GetCommaOrSimplify(8);
            }
            else if (wedgeInfo.RewardType == "emerald")
            {
                var item = wedgeInfo.Item;
                if (item.Type == Item.Types.Type.Emerald)
                {
                    _totalEmerald += (long) item.Emerald.Amount;
                }
                await ShowCollectFx(view.emeraldWinGroup.localPosition);
                SoundController.PlaySfx("Wheel_CoinsWin_fly");
                view.leftGroup.GetComponent<Animator>().Play("EmeraldWin",-1,0);

                view.emeraldIntegralText.text = _totalEmerald.GetCommaFormat();
            }
            else if (wedgeInfo.RewardType == "multiple")
            {
                _totalMultiple *= 2;
                if (_totalMultiple > 8)
                {
                    _totalMultiple = 8;
                }
                await ShowCollectFx(view.multiplesGroup.localPosition);
                SoundController.PlaySfx("Wheel_CoinsWin_fly");
                view.leftGroup.GetComponent<Animator>().Play("Multiples",-1,0);
                view.multipleText.text = "X" + _totalMultiple;
            }
        }
    }
}