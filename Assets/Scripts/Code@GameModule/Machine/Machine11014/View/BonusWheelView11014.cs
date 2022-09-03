// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/09/16:33
// Ver : 1.0.0
// Description : BonusWheelView11014.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class BonusWheelView11014 : TransformHolder, IElementProvider
    {
        private Transform[] _elementArray;

        private ExtraState11001 _extraState11001;

        [ComponentBinder("ElementTemplates")]
        private Transform _elementTemplates; 
       
        [ComponentBinder("Roll")]
        private Transform _roll;
 
        [ComponentBinder("PickableEggs")] private Transform _pickableEggs;
        [ComponentBinder("IntegralGroup")] private Transform _integralGroup;
        [ComponentBinder("HighlightFx")] private Transform _highlightFx;
        [ComponentBinder("TipText")] private Transform _pickTipText;

        private RepeatedField<BingoCloverGameResultExtraInfo.Types.Wheel.Types.Item> _wheelItems;

        private SingleColumnWheel _wheel;

        private Action spinFinishCallback;

        private bool _enableEggPick;

        private bool _enableWheelUpdate = false;

        private Animator _animator;

        private Transform[] _eggArray;
        private Transform[] _highlightTargets;

        private int _pickEggCount;
        private List<int> eggsCanPick;
         
        public BonusWheelView11014(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
        public void StartShowJackpotWheel()
        {
            transform.gameObject.SetActive(true);

            SetUpElementTemplate();
            SetUpElementReel();
            SetUpJackpotRoll();
            SetUpEggState();
           
            XUtility.PlayAnimation(_animator, "Open", () => { _enableEggPick = true; }, context);
            
            AudioUtil.Instance.MakeASavePoint();
            AudioUtil.Instance.PlayMusic("Bg_Bonus_" + context.assetProvider.AssetsId, true);
            
            SetUpHighlightState();
        }

        private void SetUpEggState()
        {
            eggsCanPick = new List<int>();
            
            for (var i = 0; i < _eggArray.Length; i++)
            {
                var eggAnimator = _eggArray[i].GetComponent<Animator>();
                if (eggAnimator)
                {
                    eggAnimator.keepAnimatorControllerStateOnDisable = true;
                    eggAnimator.Play("WaitPick");
                }
                eggsCanPick.Add(i);
            }

            _pickEggCount = 0;
        }
        
        private void SetUpHighlightState()
        {
            _highlightFx.transform.localPosition = _highlightTargets[0].localPosition;
            _highlightFx.GetComponent<Animator>().Play("HighlightFx1");
            
            var wheelInfo = _extraState11001.GetBingoJackpotWheelInfo();
            
            var integralText = _integralGroup.Find("Stage1/Stage1IntegralText").GetComponent<TextMesh>();
          
            integralText.text = BetState.GetWinChips((long)wheelInfo.Bet, _wheelItems[0].WinRate).GetCommaOrSimplify(10);

            integralText = _integralGroup.Find("Stage2/Stage2IntegralText").GetComponent<TextMesh>();
            integralText.text = BetState.GetWinChips((long)wheelInfo.Bet, _wheelItems[1].WinRate).GetCommaOrSimplify(10);

            integralText = _integralGroup.Find("Stage3/Stage3IntegralText").GetComponent<TextMesh>();
            integralText.text = BetState.GetWinChips((long)wheelInfo.Bet, _wheelItems[2].WinRate).GetCommaOrSimplify(10);
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            
            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
            
            _eggArray = new Transform[_pickableEggs.childCount];

            for (var i = 0; i < _pickableEggs.childCount; i++)
            {
                var eggIndex = i;
                _eggArray[i] = _pickableEggs.GetChild(i);

               
                var clickPoint = _eggArray[i].Find("StateGroup/NormalState");

                var pointerEventCustomHandler = clickPoint.gameObject.GetComponent<PointerEventCustomHandler>();

                if (pointerEventCustomHandler == null)
                {
                    pointerEventCustomHandler = clickPoint.gameObject.AddComponent<PointerEventCustomHandler>();
                }
                
                pointerEventCustomHandler.BindingPointerClick((data =>
                {
                    if(_enableEggPick)
                        OnEggClicked(eggIndex);
                }));
            }


            _highlightTargets = new Transform[4];

            _highlightTargets[0] = _integralGroup.Find("Stage1");
            _highlightTargets[1] = _integralGroup.Find("Stage2");
            _highlightTargets[2] = _integralGroup.Find("Stage3");
            _highlightTargets[3] = _roll;
        }

        public async void OnEggClicked(int eggIndex)
        {
            _enableEggPick = false;
            
            var wheel = _extraState11001.GetBingoJackpotWheelInfo();
            if (!wheel.Chosen)
            {
                await _extraState11001.SendBonusProcess();
            }
            
            wheel = _extraState11001.GetBingoJackpotWheelInfo();
            
            var eggAnimator = _eggArray[eggIndex].GetComponent<Animator>();
         
            
            eggsCanPick.Remove(eggIndex);
            
            if (wheel.Choice <= 2)
            {
                if (eggAnimator)
                {
                    if (_pickEggCount >= wheel.Choice)
                    {
                        
                        context.WaitSeconds(0.85f, () => {
                                AudioUtil.Instance.PlayAudioFxOneShot("Wheel_Click");
                                context.WaitSeconds(0.4f, () => { AudioUtil.Instance.PlayAudioFx("Wheel_Collect"); });
                        });
                        
                        await XUtility.PlayAnimationAsync(eggAnimator, "PickCollect", context);

                        for (var i = 0; i < eggsCanPick.Count; i++)
                        {
                            eggAnimator = _eggArray[eggsCanPick[i]].GetComponent<Animator>();
                            eggAnimator.Play("DarkLevelUp");
                        }
                        
                        var pickTipAnimator = _pickTipText.GetComponent<Animator>();
                        if(pickTipAnimator != null)
                            pickTipAnimator.Play("Out");

                        await context.WaitSeconds(2f);
                        await XUtility.PlayAnimationAsync(_animator, "Close", context);

                        transform.gameObject.SetActive(false);

                        OnBonusEnd();
                        return;
                    }
                }
            }
            
            eggAnimator.Play("PickLevelUp");
        
            context.WaitSeconds(0.85f, () =>
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Wheel_Click");
                
                context.WaitSeconds(1.0f, () => { AudioUtil.Instance.PlayAudioFx("Wheel_Rolling"); });
            });
            
            await context.WaitSeconds(2.45f);
           
            FlyFxToCurrentHighLightLevel(_eggArray[eggIndex], async () =>
            {
                _pickEggCount++;

                AudioUtil.Instance.PlayAudioFxOneShot("Bonus_BorderMove");
                _highlightFx.DOMove(_highlightTargets[_pickEggCount].position, 0.5f);
              
                if (_pickEggCount == 3)
                {
                    var fxAnimator = _highlightFx.GetComponent<Animator>();
                    fxAnimator.Play("HighlightFx1To2");

                    for (var i = 0; i < eggsCanPick.Count; i++)
                    {
                        eggAnimator = _eggArray[eggsCanPick[i]].GetComponent<Animator>();
                        eggAnimator.Play("DarkCollect");
                    }

                    await context.WaitSeconds(0.5f);
               
                    var pickTipAnimator = _pickTipText.GetComponent<Animator>();
                    if(pickTipAnimator != null)
                        pickTipAnimator.Play("Out");
               
                    StartSpinning();
                }
                else
                {
                    _enableEggPick = true;
                }
            });
        }

        public void FlyFxToCurrentHighLightLevel(Transform eggTransform, Action flyFinishCallback)
        {
            var tailFx = context.assetProvider.InstantiateGameObject("ep_Bingo_Trail");
 
            AudioUtil.Instance.PlayAudioFxOneShot("Bonus_Fly");

            var parent = _highlightTargets[_pickEggCount];

            tailFx.transform.SetParent(parent);
            tailFx.transform.position = eggTransform.position;
            XUtility.Fly(tailFx.transform, tailFx.transform.position, parent.position, 0, 0.5f,
                async () =>
                {
                    flyFinishCallback.Invoke();
                    GameObject.Destroy(tailFx);
                    //TODO ADD Chips
                }, Ease.Linear, context);
        }
 
        public void SetSpinFinishCallback(Action finishCallback)
        {
            spinFinishCallback = finishCallback;
        }

        public void SetUpJackpotRoll()
        {
            if (_wheel == null)
                _wheel = new SingleColumnWheel(transform.Find("Root/WheelMainGroup/Root/UpperMachineGroup"), 1.12f, 1, this, 0);
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            var wheel = _extraState11001.GetBingoJackpotWheelInfo();
            return (wheel.Choice - 4 + GetReelMaxLength())% GetReelMaxLength();
        }
        
        public int GetElementMaxHeight()
        {
            return 2;
        }


        public void OnBonusEnd()
        {
            AudioUtil.Instance.RecoverLastSavePointMusicPlay();
            spinFinishCallback?.Invoke();
            spinFinishCallback = null;
        }
 
        public void StartSpinning()
        {
            _enableEggPick = false;
  
            var easingConfig = context.machineConfig.GetEasingConfig("Jackpot");
            _enableWheelUpdate = true;

            AudioUtil.Instance.PlayAudioFxOneShot("Wheel_Spin");
            _wheel.StartSpinning(easingConfig, OnSpinningEnd, 0);

//            XUtility.PlayAnimation(_animator, "Idle");

            context.WaitSeconds(0.5f, () =>
            {
                _wheel.OnSpinResultReceived();
            });
        }

        public override void Update()
        {
            if (_wheel != null && _enableWheelUpdate)
            {
                _wheel.Update();
            }
        }

        public async void OnSpinningEnd()
        {
            _enableWheelUpdate = false;
            AudioUtil.Instance.PlayAudioFxOneShot("Wheel_Stop");
           await XUtility.PlayAnimationAsync(_highlightFx.GetComponent<Animator>(), "HighlightFxWin", context);
           await XUtility.PlayAnimationAsync(_animator, "Close", context);

           transform.gameObject.SetActive(false);
           OnBonusEnd();
        }

        public void SetUpElementReel()
        {
            _extraState11001 = context.state.Get<ExtraState11001>();
            var wheel = _extraState11001.GetBingoJackpotWheelInfo();
            _wheelItems = wheel.Items;
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

        public int GetReelMaxLength()
        {
            return _wheelItems.Count - 3;
        }
        
        public int OnReelStopAtIndex(int currentIndex)
        {
            return currentIndex;
        }
        public GameObject GetElement(int index)
        {
            var element = GameObject.Instantiate(_elementArray[_wheelItems[index + 3].JackpotId].gameObject);
            element.transform.localPosition = Vector3.zero;
            return element;
        }
    }
}