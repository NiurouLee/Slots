using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;
using System;


namespace GameModule
{
    public class WheelBonus11029 : TransformHolder, IElementProvider
    {
        private Animator animator;

        private Animator animatorEnter;

        private Animator animatorPop;

        private bool enableClick = false;

        private SingleColumnWheel _wheel;

        private Action spinFinishCallback;

        private Transform[] _elementArray;

        private ExtraState11029 extraState11029;

        private bool _enableWheelUpdate = false;

        private float spinTime = 3.0f;

        private bool isStart = false;

        private RepeatedField<EyeOfMedusaGameResultExtraInfo.Types.Wheel.Types.Item> wheelItems;

        [ComponentBinder("Root/bonusPop")] private Transform btnSpin;

        [ComponentBinder("Root/WheelMainGroup/Root/MainGroup/ElementTemplates")]
        private Transform _elementTemplates;
        
        [ComponentBinder("Root/WheelMainGroup/Root/Fx_Trail")] private Transform fxTrail;

        public WheelBonus11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.Find("Root").GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            animatorEnter = transform.Find("Root/WheelMainGroup/Root/Fx_Trail").GetComponent<Animator>();
            animatorEnter.keepAnimatorControllerStateOnDisable = true;
            animatorPop = transform.Find("Root/bonusPop").GetComponent<Animator>();
            animatorPop.keepAnimatorControllerStateOnDisable = true;
            var pointerEventCustomHandler = btnSpin.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(StartSpinClicked);
        }

        public virtual async void InitializeBonusWheelView()
        {
            StartShowJackpotWheel();
        }
        
        public virtual async Task PlayBonusWheelView()
        {
            await PlayEnterWheel();
            await PlayPressSpin();
            enableClick = true;
        }
        
        public virtual async Task From()
        {
            enableClick = false;
            btnSpin.transform.gameObject.SetActive(true);
            animatorPop.transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animatorPop, "Open");
            animatorPop.Play("Idle");
            StartFromSpinning();
        }

        //进入转盘时的特效
        public async Task PlayEnterWheel()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_Fire");
            fxTrail.transform.gameObject.SetActive(true);
            animatorEnter.transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animatorEnter, "Fx_Trail");
            fxTrail.transform.gameObject.SetActive(false);
            animatorEnter.transform.gameObject.SetActive(false);
        }

        //PressSpin特效
        public async Task PlayPressSpin()
        {
            btnSpin.transform.gameObject.SetActive(true);
            animatorPop.transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animatorPop, "Open");
            animatorPop.Play("Idle");
        }

        public void StartShowJackpotWheel()
        {
            transform.gameObject.SetActive(true);
            SetUpElementTemplate();
            SetUpElementReel();
            SetUpJackpotRoll();
        }

        public void SetUpElementTemplate()
        {
            _elementArray = new Transform[6];
            for (var i = 0; i < 6; i++)
            {
                _elementArray[i] = _elementTemplates.GetChild(i);
            }

            _elementTemplates.gameObject.SetActive(false);
        }

        public void SetUpElementReel()
        {
            extraState11029 = context.state.Get<ExtraState11029>();
            var wheel = extraState11029.GetWheelData();
            wheelItems = wheel.Items;
        }

        public void SetUpJackpotRoll()
        {
            if (_wheel == null)
                _wheel = new SingleColumnWheel(transform.Find("Root/WheelMainGroup/Root/MainGroup"), 1.42f * 8, 8,
                    this, 0);
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            var wheel = extraState11029.GetWheelData();
            return ((int) wheel.Index - 3 + wheel.Items.Count) % wheel.Items.Count;
        }

        public int GetReelMaxLength()
        {
            return wheelItems.Count;
        }

        public int GetElementMaxHeight()
        {
            return 1;
        }
        
        public int OnReelStopAtIndex(int currentIndex)
        {
            return currentIndex;
        }

        public GameObject GetElement(int index)
        {
            if (wheelItems[index].IsBagBonus)
            {
                var element = GameObject.Instantiate(_elementArray[5].gameObject);
                element.transform.localPosition = Vector3.zero;
                return element;
            }
            else if (wheelItems[index].IsNormalFree)
            {
                var element = GameObject.Instantiate(_elementArray[3].gameObject);
                element.transform.localPosition = Vector3.zero;
                return element;
            }
            else if (wheelItems[index].IsFeatureFree)
            {
                var element = GameObject.Instantiate(_elementArray[1].gameObject);
                element.transform.localPosition = Vector3.zero;
                return element;
            }
            else
            {
                var cellIndex = 0;
                if (wheelItems[index].JackpotId > 0)
                {
                    switch (wheelItems[index].JackpotId)
                    {
                        case 1:
                            cellIndex = 0;
                            break;
                        case 2:
                            cellIndex = 4;
                            break;
                        case 3:
                            cellIndex = 2;
                            break;
                    }
                }

                var element = GameObject.Instantiate(_elementArray[cellIndex].gameObject);
                element.transform.localPosition = Vector3.zero;
                return element;
            }
        }

        //点击弹窗开始spin，弹窗消失。
        public void StartSpinClicked(PointerEventData data)
        {
            if (enableClick)
            {
                AudioUtil.Instance.PlayAudioFx("Close");
                StartSpinning();
            }
        }
        
        public void StartSpinClicked()
        {
          
                StartSpinning();
        }

        private async void OnSpinningEnd()
        {
            AudioUtil.Instance.StopMusic();
            AudioUtil.Instance.PlayAudioFx("Wheel_Select");
            _enableWheelUpdate = false;
            var wheel = extraState11029.GetWheelData();
            var index = (int) wheel.Index;
            if (wheelItems[index].JackpotId > 0)
            {
                context.view.Get<JackpotPanel11029>().ShowWinFrame((int) wheelItems[index].JackpotId);
            }
            await XUtility.PlayAnimationAsync(animator, "Open", context);
            // spinFinishCallback.Invoke();
            //根据中奖结果判断接下来的弹窗
            await BonusWinPop();
        }

        private async Task BonusWinPop()
        {
            var wheel = extraState11029.GetWheelData();
            var index = (int) wheel.Index;
            ulong bonusWin = wheel.Bet * (wheel.Items[index].WinRate + wheel.Items[index].JackpotPay) / 100;
            var bonusProxy11029 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11029;
            if (wheelItems[index].IsBagBonus)
            {
                await bonusProxy11029.BonusBagFinish();
            }
            else if (wheelItems[index].IsNormalFree)
            {
                await context.state.Get<ExtraState11029>().SettleBonusProgress();
                await bonusProxy11029.BonusFreeFinish();
            }
            else if (wheelItems[index].IsFeatureFree)
            {
                await context.state.Get<ExtraState11029>().SettleBonusProgress();
                await bonusProxy11029.BonusFreeFinish();
            }
            else if (wheelItems[index].JackpotId > 0)
            {
                await Constant11029.ShowBonusJackpot(context, wheelItems[index].JackpotId, bonusWin);
                await bonusProxy11029.BonusGameFinish();
            }
        }

        public override void Update()
        {
            if (_wheel != null && _enableWheelUpdate)
            {
                _wheel.Update();
            }
        }

        public async void StartSpinning()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
            enableClick = false;
            await XUtility.PlayAnimationAsync(animatorPop, "Close");
            btnSpin.transform.gameObject.SetActive(false);
            //转盘转动
            animator.Play("Idle");
            var easingConfig = context.machineConfig.GetEasingConfig("Jackpot");
            _enableWheelUpdate = true;
            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling");
            _wheel.StartSpinning(easingConfig, OnSpinningEnd, 0);
            var bonusProcess = await extraState11029.SendBonusProcess();
            if (bonusProcess != null && transform != null)
            {
                _wheel.OnSpinResultReceived();
            }
        }
        
        public async void StartFromSpinning()
        {
            enableClick = false;
            AudioUtil.Instance.PlayAudioFx("Wheel_Spin");
            animator.Play("Idle");
            await XUtility.PlayAnimationAsync(animatorPop, "Close");
            btnSpin.transform.gameObject.SetActive(false);
            var easingConfig = context.machineConfig.GetEasingConfig("Jackpot");
            _enableWheelUpdate = true;
            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling");
            _wheel.StartSpinning(easingConfig, OnSpinningEnd, 0);
            _wheel.OnSpinResultReceived();
        }
    }
}