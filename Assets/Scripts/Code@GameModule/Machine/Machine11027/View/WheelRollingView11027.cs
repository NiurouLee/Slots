using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class WheelRollingView11027 : TransformHolder
    {
        private Animator animator;
        
        private Animator winAnimator;
        
        [ComponentBinder("Root/MainGroup")] private Transform mainGroup;

        [ComponentBinder("Root/CenterGroup/WheelBonusButton")]
        private Transform btnSpin;
        
        [ComponentBinder("Root/MainGroup/AnimControl")]
        private Transform animControl;

        public Action wheelEndAction;
        
        private bool _buttonResponseEnabled = false;

        private List<Transform> wheelItemsList;
        public float anglePerFan = 0;
        private List<Animator> _animators;

        public WheelRollingView11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            animator = transform.GetComponent<Animator>();
            // winAnimator = animControl.GetComponent<Animator>();
            InitWheelUI();
        }

        private void InitWheelUI()
        {
            var pointerEventCustomHandler = btnSpin.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(SpinClicked);
            EnableButton(true);
            wheelItemsList = new List<Transform>();
            for (int i = 0; i < 18; i++)
            {
                var pot = transform.Find($"Root/MainGroup/AnimControl/Cell{i + 1}");
                wheelItemsList.Add(pot);
            }
            PlayAnimation("Default");
        }
        
        public void EnableButton(bool enable)
        {
            _buttonResponseEnabled = enable;
        }
        

        //轮盘初始化显示
        public virtual void InitializeWheelView(bool idleanimation = false,bool reset = false)
        {
            _buttonResponseEnabled = false;
            mainGroup.localEulerAngles = new Vector3(0, 0, 0);
            if (idleanimation)
            {
                PlayAnimation("Spin_Idle");
            }
            else if(reset)
            {
                PlayAnimation("WheelReset");
            }
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            _animators = new List<Animator>();
            for (int i = 0; i < wheelItemsList.Count; i++)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].JackpotId > 0)
                    {
                        wheelItemsList[i].Find("CellEFX").Find($"BGStart").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"BGLReward").gameObject.SetActive(false);
                        wheelItemsList[i].Find("CellEFX").Find($"BGRReward").gameObject.SetActive(false);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").gameObject.SetActive(false);
                        for (int q = 1; q < 7; q++)
                        {
                           wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").Find("rewardIcon" + q + "")
                                .gameObject.SetActive(false);
                        }
                        _animators.Add(wheelItemsList[i].Find("CellEFX").gameObject.GetComponent<Animator>());
                    }
                    else
                    {
                        var chips = context
                            .state.Get<BetState>().GetPayWinChips(wheelItems[i].WinRate);
                        //一位数时保留一位小数点
                        var coinText = chips.GetAbbreviationFormat(1);
                        if (chips > 0)
                        {
                            var commaIndex = coinText.IndexOf('.');
                            if (commaIndex >= 2)
                            {
                                coinText = coinText.Remove(commaIndex, 2);
                            }
                            
                        }
                        wheelItemsList[i].Find($"IntegralText").gameObject.GetComponent<TextMeshPro>().text = coinText;
                        // wheelItemsList[i].Find($"IntegralText").gameObject.GetComponent<TextMeshPro>().text = context
                        //     .state.Get<BetState>().GetPayWinChips(wheelItems[i].WinRate)
                        //     .GetAbbreviationFormat(0);
                        wheelItemsList[i].Find($"BetText").gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGLMul").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGRMul").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGLBase").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGRBase").gameObject.gameObject.SetActive(true);
                        _animators.Add(wheelItemsList[i].Find("Cell").gameObject.GetComponent<Animator>());
                    }
                }
                _animators[i].keepAnimatorControllerStateOnDisable = true;
                _animators[i].Play("Default");
            }
        }

        public virtual async void InitializeBonusWheelView()
        {
            _buttonResponseEnabled = false;
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            anglePerFan = 360 / (float) wheelItems.Count;
            //轮盘动画
            ChangeWheelView();
        }
        
        public void RecoverInitializeBonusWheelView()
        {
            _buttonResponseEnabled = false;
            PlayAnimation("WheelReset");
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            anglePerFan = 360 / (float) wheelItems.Count;
        }

        public virtual async void ChangeWheelView()
        {
            await ShowJackpotItemIdle();
            await ShowJackpotItem();
            await ShowMulItem();
            _buttonResponseEnabled = true;
        }
        
        //先显示出jackpot的轮盘
        public virtual async Task ShowJackpotItemIdle()
        {
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 0; i < wheelItemsList.Count; i++)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].JackpotId > 0)
                    {
                        _animators[i].Play("Idle"); 
                        wheelItemsList[i].Find("CellEFX").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"BGStart").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"BGLReward").gameObject.SetActive(false);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").Find("rewardIcon" + wheelItems[i].JackpotId + "")
                            .gameObject.SetActive(false);
                    }
                }
            }
             //暂时写0.67f,正确的打开方式应该是小兔子转完圈后结束Idle，暂时还没有这个特效。
             await context.WaitSeconds(0.67f);
        }

        public void PLayIntro()
        {
            PlayAnimation("WheelIntro");
        }
        
        //逐个打开jackpot
        public virtual async Task ShowJackpotItem()
        {
            PlayAnimation("JackpotActive");
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 0; i < wheelItemsList.Count; i++)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].JackpotId > 0)
                    {
                        wheelItemsList[i].Find("CellEFX").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"BGStart").gameObject.SetActive(false);
                        wheelItemsList[i].Find("CellEFX").Find($"BGLReward").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").Find("rewardIcon" + wheelItems[i].JackpotId + "")
                            .gameObject.SetActive(true);
                        _animators[i].Play("Active");
                        AudioUtil.Instance.PlayAudioFx("Wheel_JackpotRefesh");
                        await context.WaitSeconds(0.9f);
                    }
                }
            }
            await context.WaitSeconds(3.67f-2.7f);
        } 
        
        //显示出加倍数的轮盘Item，分两次显示出来
        public virtual async Task ShowMulItem()
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_PandaFly1");
            PlayAnimation("Before");
            float time = 0f;
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 11; i >= 0; i--)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].Multiplier > 1)
                    {
                        var chips = context
                            .state.Get<BetState>().GetPayWinChips(wheelItems[i].WinRate);
                        //一位数时保留一位小数点
                        var coinText = chips.GetAbbreviationFormat(1);
                        if (chips > 0)
                        {
                            var commaIndex = coinText.IndexOf('.');
                            if (commaIndex >= 2)
                            {
                                coinText = coinText.Remove(commaIndex, 2);
                            }
                            
                        }
                        wheelItemsList[i].Find($"IntegralText").gameObject.GetComponent<TextMeshPro>().text = coinText;
                        // wheelItemsList[i].Find($"IntegralText").gameObject.GetComponent<TextMeshPro>().text = context
                        //     .state.Get<BetState>().GetPayWinChips(wheelItems[i].WinRate)
                        //     .GetAbbreviationFormat(0);
                        wheelItemsList[i].Find($"BetText").gameObject.GetComponent<TextMesh>().text =
                            wheelItems[i].Multiplier + "X";
                        wheelItemsList[i].Find($"BetText").gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGRMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLBase").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGRBase").gameObject.gameObject.SetActive(false);
                        _animators[i].Play("CellMulti");
                        AudioUtil.Instance.PlayAudioFx("Wheel_Wedge");
                        await context.WaitSeconds(0.27f);
                        time = time + 0.27f;
                    }
                }
            }
             
            for (int i = 17; i > 11; i--)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].Multiplier > 1)
                    {
                        wheelItemsList[i].Find($"BetText").gameObject.GetComponent<TextMesh>().text = wheelItems[i].Multiplier + "X";
                        wheelItemsList[i].Find($"BetText").gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGRMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLBase").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGRBase").gameObject.gameObject.SetActive(false);
                        _animators[i].Play("CellMulti"); 
                        await context.WaitSeconds(0.27f);
                        AudioUtil.Instance.PlayAudioFx("Wheel_Wedge");
                        time = time + 0.27f;
                    }
                }
            }
            if (time < 3.67f)
            {
                 await context.WaitSeconds(3.67f - time);
            }
        }
        
            //逐个打开jackpot
        public void RecoverShowJackpotItem()
        {
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 0; i < wheelItemsList.Count; i++)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].JackpotId > 0)
                    {
                        wheelItemsList[i].Find("CellEFX").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"BGStart").gameObject.SetActive(false);
                        wheelItemsList[i].Find("CellEFX").Find($"BGLReward").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").gameObject.SetActive(true);
                        wheelItemsList[i].Find("CellEFX").Find($"RewardGroup").Find("rewardIcon" + wheelItems[i].JackpotId + "")
                            .gameObject.SetActive(true);
                        _animators[i].Play("ActiveIdle");
                    }
                }
            }
        } 
        
        //显示出加倍数的轮盘Item，分两次显示出来
        public void RecoverShowMulItem()
        {
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 11; i >= 0; i--)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].Multiplier > 1)
                    {
                        var chips = context
                            .state.Get<BetState>().GetPayWinChips(wheelItems[i].WinRate);
                        //一位数时保留一位小数点
                        var coinText = chips.GetAbbreviationFormat(1);
                        if (chips > 0)
                        {
                            var commaIndex = coinText.IndexOf('.');
                            if (commaIndex >= 2)
                            {
                                coinText = coinText.Remove(commaIndex, 2);
                            }
                            
                        }
                        wheelItemsList[i].Find($"IntegralText").gameObject.GetComponent<TextMeshPro>().text = coinText;
                        wheelItemsList[i].Find($"BetText").gameObject.GetComponent<TextMesh>().text =
                            wheelItems[i].Multiplier + "X";
                        wheelItemsList[i].Find($"BetText").gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGRMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLBase").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGRBase").gameObject.gameObject.SetActive(false);
                        _animators[i].Play("CellIdleMulti");
                    }
                }
            }
             
            for (int i = 17; i > 11; i--)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].Multiplier > 1)
                    {
                        wheelItemsList[i].Find($"BetText").gameObject.GetComponent<TextMesh>().text = wheelItems[i].Multiplier + "X";
                        wheelItemsList[i].Find($"BetText").gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGRMul").gameObject.gameObject.SetActive(true);
                        wheelItemsList[i].Find("Cell").Find($"BGLBase").gameObject.gameObject.SetActive(false);
                        wheelItemsList[i].Find("Cell").Find($"BGRBase").gameObject.gameObject.SetActive(false);
                        _animators[i].Play("CellIdleMulti");
                    }
                }
            }
        }

        public void SpinClicked(PointerEventData pointerEventData)
        {
            if (_buttonResponseEnabled)
            {
                 OnStartSpin();
            }
        }
        
        public async Task OnStartSpin()
        {
            _buttonResponseEnabled = false;
            AudioUtil.Instance.PlayAudioFx("Wheel_Fly");
            PlayAnimation("Start");
            await SendBonusProcess();
            await context.WaitSeconds(2.0f);
            await StopWheel(GetWheelNudgeIndex());
            if (GetWheelHitIndex() != GetWheelNudgeIndex())
            {
                await StopFinalWheel(GetWheelHitIndex());
            }
            await ShowWheelWinAsync();
            wheelEndAction?.Invoke();
            await Settle();
        }

        public async Task RecoverSettle()
        {
            var _extraState11027 = context.state.Get<ExtraState11027>();
            if (_extraState11027.NeedRollingSettle())
            {
                _buttonResponseEnabled = false;
                RecoverShowJackpotItem();
                RecoverShowMulItem();
                PlayAnimation("Start");
                await context.WaitSeconds(2.0f);
                await StopWheel(GetWheelNudgeIndex());
                if (GetWheelHitIndex() != GetWheelNudgeIndex())
                {
                    await StopFinalWheel(GetWheelHitIndex());
                }
                await ShowWheelWinAsync();
                wheelEndAction?.Invoke();
                await Settle();
            }
        }

        public async Task Settle()
        {
            var _extraState11027 = context.state.Get<ExtraState11027>();
            _extraState11027 = context.state.Get<ExtraState11027>();
            if (_extraState11027.NeedRollingSettle())
            {
                // ulong bonusWin = _extraState11027.GetBonusTotalWin() - _extraState11027.GetPanelWin();
                ulong bonusWin = _extraState11027.GetWheelWin();
                var index = _extraState11027.GetWheelEndIndex();
                var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
                if (wheelItemsList[(int)index])
                {
                    if (wheelItems[(int)index].JackpotId > 0)
                    {
                        await Constant11027.ShowBonusJackpot(context, wheelItems[(int)index].JackpotId,bonusWin);
                    }
                    else
                    {
                        await Constant11027.ShowBonus(context, bonusWin);
                    }
                }
                else
                {
                    await Constant11027.ShowBonus(context, bonusWin);
                }
                var bonusProxy11027 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11027;
                bonusProxy11027.WheelRollingFinish();
            }
        }

        private uint GetWheelHitIndex()
        {
            return context.state.Get<ExtraState11027>().GetWheelEndIndex();
        }
        
        private uint GetWheelNudgeIndex()
        {
            return context.state.Get<ExtraState11027>().GetWheelNudgeIndex();
        }

        public async Task PlayAnimationAsync(string animName)
        {
            await XUtility.PlayAnimationAsync(animator, animName);
        }

        public void PlayAnimation(string animName)
        {
            XUtility.PlayAnimation(animator, animName);
        }

        public async Task SendBonusProcess()
        {
            CBonusProcess cBonusProcess = new CBonusProcess();
            await context.state.Get<ExtraState11027>().SendBonusProcess();
        }

        public async Task StopWheel(uint hitWedgeId)
        {
            float targetAngle = anglePerFan * hitWedgeId;
            targetAngle = targetAngle - 20;
            mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
            await PlayAnimationAsync("Finish");
        }
        
        public async Task StopFinalWheel(uint hitWedgeId)
        {
            uint startIndex = GetWheelNudgeIndex();
            uint finalIndex = GetWheelHitIndex();
            uint oldHitWedgeId = startIndex;
            if (finalIndex > startIndex)
            {
                if ((int) finalIndex == 17 && (int) startIndex == 0)
                {
                    PlayAnimation("WinMoreRight");
                    // await context.WaitSeconds(5.67f);
                    await context.WaitSeconds(2.63f);
                    AudioUtil.Instance.PlayAudioFx("Wheel_WedgePanda");
                    // float targetAngle = anglePerFan * oldHitWedgeId;
                    // targetAngle = targetAngle - 40;
                    // mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
                    await context.WaitSeconds(5.67f - 2.63f);
                }
                else
                {
                    PlayAnimation("WinMoreLeft");
                     // await context.WaitSeconds(5.67f);
                    await context.WaitSeconds(2.63f);
                    AudioUtil.Instance.PlayAudioFx("Wheel_WedgePanda");
                    // float targetAngle = anglePerFan * oldHitWedgeId;
                    // mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
                    await context.WaitSeconds(5.67f - 2.63f);
                }
            }
            else
            {
                if ((int) finalIndex == 0 && (int) startIndex == 17)
                {
                    PlayAnimation("WinMoreLeft");
                    // await context.WaitSeconds(5.67f);
                    await context.WaitSeconds(2.63f);
                    AudioUtil.Instance.PlayAudioFx("Wheel_WedgePanda");
                    // float targetAngle = anglePerFan * oldHitWedgeId;
                    // mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
                    await context.WaitSeconds(5.67f - 2.63f);
                }
                else
                {
                    PlayAnimation("WinMoreRight"); 
                    // await context.WaitSeconds(5.67f);
                    await context.WaitSeconds(2.63f);
                    AudioUtil.Instance.PlayAudioFx("Wheel_WedgePanda");
                    // float targetAngle = anglePerFan * oldHitWedgeId;
                    // targetAngle = targetAngle - 40;
                    // mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
                    await context.WaitSeconds(5.67f-2.63f);
                }
            }
        }

        public async Task ShowWheelWinAsync()
        {
            var wheelItems = context.state.Get<ExtraState11027>().GetWheelItems();
            for (int i = 0; i < wheelItemsList.Count; i++)
            {
                if (wheelItemsList[i])
                {
                    if (wheelItems[i].JackpotId > 0)
                    {
                        if (i != (int) GetWheelHitIndex())
                        {
                            _animators[i].Play("CellDark");
                        }
                    }
                    else
                    {
                        if (i != (int) GetWheelHitIndex())
                        {
                            if (wheelItems[i].Multiplier > 1)
                            {
                                 _animators[i].Play("CellDarkMulti");
                            }
                            else
                            {
                                 _animators[i].Play("CellDark");
                            }
                        }
                    }
                }
            }
            context.view.Get<WheelRollingView11027>().StopMusic();
            AudioUtil.Instance.PlayAudioFx("Wheel_WedgeCollect");
            if (GetWheelHitIndex() == GetWheelNudgeIndex())
            {
                 PlayAnimation("Win");
                 await context.WaitSeconds(2.0f);
            }
            else
            {
                 PlayAnimation("Winidle");
                 await context.WaitSeconds(1.33f);
            }
        }
        public void PLayWheelMusic()
        {
            AudioUtil.Instance.PlayMusic("Bg_FreeGame_11027",true);
        }

        public void StopMusic()
        {
             AudioUtil.Instance.StopMusic();
        }
    }
}