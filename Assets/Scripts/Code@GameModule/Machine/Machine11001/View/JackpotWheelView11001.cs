// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/14:58
// Ver : 1.0.0
// Description : JackpotWheelView11003.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
     
    public class JackpotWheelView11001 : TransformHolder, IElementProvider
    {
        private Transform[] _elementArray;

        private ExtraState11001 _extraState11001;

        [ComponentBinder("Root/WheelMainGroup/Root/MainGroup/ElementTemplates")]
        private Transform _elementTemplates;
        
        [ComponentBinder("Root/HotArea")]
        private Transform _hotArea;

        private RepeatedField<BingoCloverGameResultExtraInfo.Types.Wheel.Types.Item> _wheelItems;

        private SingleColumnWheel _wheel;

        private Action spinFinishCallback;

        private bool _enableHotArea;

        private bool _enableWheelUpdate = false;

        private Animator _animator;
         
        public JackpotWheelView11001(Transform transform)
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

            XUtility.PlayAnimation(_animator, "Open", () => { _enableHotArea = true; }, context);
            AudioUtil.Instance.PlayAudioFx("Wheel_Open");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
            var pointerEventCustomHandler = _hotArea.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnHotAreaClicked);
        }

        public void OnHotAreaClicked(PointerEventData data)
        {
            if (_enableHotArea)
            {
                StartSpinning();
               
            }
        }

        public void SetSpinFinishCallback(Action finishCallback)
        {
            spinFinishCallback = finishCallback;
        }

        public void SetUpJackpotRoll()
        {
            if(_wheel == null)
                _wheel = new SingleColumnWheel(transform.Find("Root/WheelMainGroup/Root/MainGroup"),6.2f,5, this,0);
        }

        public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
        {
            var wheel = _extraState11001.GetBingoJackpotWheelInfo();

            return (wheel.Choice - 3 + wheel.Items.Count) % wheel.Items.Count;
        }

        public async void StartSpinning()
        {
            _enableHotArea = false;
            AudioUtil.Instance.PlayAudioFx("Wheel_Click");
            await XUtility.PlayAnimationAsync(_animator, "Tap");
             
            var easingConfig = context.machineConfig.GetEasingConfig("Jackpot");
            _enableWheelUpdate = true;
            AudioUtil.Instance.PlayAudioFx("Wheel_Rolling");
            _wheel.StartSpinning(easingConfig, OnSpinningEnd, 0);

            XUtility.PlayAnimation(_animator, "Idle");
            
            XDebug.Log("Send Line Wheel Spin Proto");
            var bonusProcess = await _extraState11001.SendBonusProcess();
            
            if (bonusProcess != null && transform != null)
            {
                _wheel.OnSpinResultReceived();
            }
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
            AudioUtil.Instance.PlayAudioFx("Wheel_win");
            await XUtility.PlayAnimationAsync(_animator, "Finish", context);
            await XUtility.PlayAnimationAsync(_animator, "Close", context);

            transform.gameObject.SetActive(false);

            spinFinishCallback.Invoke();
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
            return _wheelItems.Count;
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
            if (_wheelItems[index].WinRate > 0)
            {
                var wheelInfo = _extraState11001.GetBingoJackpotWheelInfo();
                var gameObject = GameObject.Instantiate(_elementArray[0].gameObject);
                var winText = gameObject.transform.Find("IntegralText").GetComponent<TextMesh>();
                winText.text = BetState.GetWinChips((long)wheelInfo.Bet, _wheelItems[index].WinRate).GetCommaOrSimplify(6);

                gameObject.transform.localPosition = Vector3.zero;
                return gameObject;
            }
            else
            {
                var element = GameObject.Instantiate(_elementArray[_wheelItems[index].JackpotId].gameObject);
                element.transform.localPosition = Vector3.zero;
                return element;
            }
        }
    }
}