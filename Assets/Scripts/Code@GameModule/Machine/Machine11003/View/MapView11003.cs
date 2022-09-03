// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/23/20:30
// Ver : 1.0.0
// Description : MapView11003.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class MapView11003 : TransformHolder
    { 
        [ComponentBinder("BGGroup/Content/BGScrollViewGroup")]
        private Transform _content;
        
        [ComponentBinder("BGGroup/Mask")]
        private Transform _contentMask;
 
        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell1")]
        private Transform page1;

        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell2")]
        private Transform page2;

        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell3")]
        private Transform page3;

        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell4")]
        private Transform page4;

        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell1/MapIndicator0")]
        protected Transform startPoint;

        [ComponentBinder("BGGroup/Content/BGScrollViewGroup/StageCell1/MapIndicator0/FlagGroup/MapIndicator")]
        private Transform mapIndicator;


        private List<Transform> _pageList;
        private List<Transform> _potList;
        private List<int> _bankPotIndexList;

        private float contentWidth = 28.5f;

        private bool _isDrag = false;
        private float _velocity = 0;
        private float _contentPrePosition = 0;
        private float _contentPosition = 0;
        private Vector3 _startDragPosition;
        public bool isVisible = false;
        public bool inSwitching = false;
 
        private Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string> _buffTypeToAssetNameDict =
            new Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string>()
            {
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn, "ADDReel"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols, "ADD100"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow, "ADDRow"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddFreeSpin, "5Extra"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddExtraBonus, "IncreasePigCoins"},
            };

        public MapView11003(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _pageList = new List<Transform>() {page1, page2, page3, page4};
            _potList = new List<Transform>();
            _bankPotIndexList = new List<int>();

            for (var i = 0; i < _pageList.Count; i++)
            {
                for (var c = 0; c < _pageList[i].childCount; c++)
                {
                    _potList.Add(_pageList[i].GetChild(c));
                }

                _bankPotIndexList.Add(_potList.Count - 1);
            }
            
           var pointerEventCustomHandler = _contentMask.gameObject.AddComponent<DragDropEventCustomHandler>();

           pointerEventCustomHandler.BindingDragAction(OnDrag);
           pointerEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
           pointerEventCustomHandler.BindingEndDragAction(OnEndDrag);
        }

        void OnBeginDrag(PointerEventData pointerEventData)
        {
            _isDrag = true;
            
            _startDragPosition = pointerEventData.pressEventCamera.ScreenToWorldPoint(new Vector3(pointerEventData.position.x,pointerEventData.position.y,Camera.main.transform.position.z
            ));
            _contentPosition = _content.localPosition.x;
        }
        
        void OnEndDrag(PointerEventData pointerEventData)
        {
            _isDrag = false;
        }
        void OnDrag(PointerEventData pointerEventData)
        {
           var scrollX = pointerEventData.delta;
           var currentDragPosition = pointerEventData.pressEventCamera.ScreenToWorldPoint(new Vector3(pointerEventData.position.x,pointerEventData.position.y, Camera.main.transform.position.z));
           
           var dragDelta = currentDragPosition.x - _startDragPosition.x;
           
           var localPosition = _content.localPosition;

           var lastPos = localPosition.x;
           
           localPosition.x = _contentPosition - dragDelta;
           
           if (localPosition.x > 0)
           {
               localPosition.x = 0;
               _velocity = 0;
           }

           if (localPosition.x < -contentWidth)
           {
               localPosition.x = -contentWidth;
               _velocity = 0;
           }
           
           _content.localPosition = localPosition;
           
           var delta = localPosition.x - lastPos;
           
           var  deltaTime = Time.unscaledDeltaTime;
           var newVelocity = delta / deltaTime;
           
           _velocity = Mathf.Lerp(_velocity, newVelocity, deltaTime * 10);

           if (_velocity > 40)
           {
               _velocity = 40;
           }

           if (_velocity < -40)
           {
               _velocity = -40;
           }
           XDebug.Log("_velocity:" + _velocity);
        }
        
        public override void Update()
        {
            if (!_isDrag && _velocity != 0 && isVisible)
            {
                var deltaTime = Time.unscaledDeltaTime;
                _velocity *= Mathf.Pow(0.013f, deltaTime);
                if (Mathf.Abs(_velocity) < 0.01f)
                    _velocity = 0;
                var deltaMove = _velocity * deltaTime;
              
                var localPosition = _content.localPosition;
               
                localPosition.x += deltaMove;
          
                if (localPosition.x > 0)
                {
                    localPosition.x = 0;
                    _velocity = 0;
                }

                if (localPosition.x < -contentWidth)
                {
                    localPosition.x = -contentWidth;
                    _velocity = 0; 
                }

                _content.localPosition = localPosition;
            }
        }

        public void ShowMap(int level = -1)
        {
            if (!isVisible)
            {
                transform.gameObject.SetActive(true);
                InitializePageState(level);

                FocusCurrentPot(level);

                var animator = transform.GetComponent<Animator>();
                animator.Play("Open");

                AudioUtil.Instance.PlayAudioFx("Map_Open");
                
                _velocity = 0;
                isVisible = true;

                var baseWheel11003 = context.view.Get<BaseWheel11003>();
                
                context.WaitNFrame(12, () =>
                {
                    if (!inSwitching && context.GetRunningStep().StepType == LogicStepType.STEP_NEXT_SPIN_PREPARE)
                    {
                        baseWheel11003.HideSymbolContent();
                    }
                });
                
                EventSystem.current.SetSelectedGameObject(_contentMask.gameObject);
                XDebug.Log("ShowMap");
            }
        }

        public void HideMap()
        {
            if (isVisible && !inSwitching)
            {
                var animator = transform.GetComponent<Animator>();
                animator.Play("Close");
                AudioUtil.Instance.PlayAudioFx("Map_Close"); 
                _velocity = 0;
                
                inSwitching = true;
                var baseWheel11003 = context.view.Get<BaseWheel11003>();
                baseWheel11003.ShowSymbolContent();
                
                context.WaitNFrame(10, () =>
                {
                    isVisible = false;
                    inSwitching = false;
                });
                XDebug.Log("HideMap");
            }
        }
        
        public async Task ShowIndicatorMoveAnimation()
        {
            var extraState11003 = context.state.Get<ExtraState11003>();
            var currentPotLevel = (int) extraState11003.GetPotLevel();

            var lastFinishLevel = currentPotLevel;

            if (currentPotLevel == 0)
            {
                lastFinishLevel = _potList.Count - 1;
            }

            ShowMap(lastFinishLevel - 1);

            var potAnimator = _potList[lastFinishLevel].GetComponentInChildren<Animator>();
            
            var indicatorAnimator = mapIndicator.GetComponent<Animator>();

            string animationName = "jump";

            string soundName = "Map_Box_Open";

            if (_bankPotIndexList.Contains(lastFinishLevel))
            {
                animationName = "jump2";
                soundName = "Map_Bank_Open";
            }

            AudioUtil.Instance.PlayAudioFx(soundName);
            
            await XUtility.PlayAnimationAsync(potAnimator, "Completing", context);
             
            var lastFlagGroup = _potList[lastFinishLevel-1].transform.Find("FlagGroup");
            var flagGroup = _potList[lastFinishLevel].transform.Find("FlagGroup");

            var localPosition = flagGroup.InverseTransformPoint(lastFlagGroup.position);
            
            mapIndicator.SetParent(flagGroup, false);
            
            mapIndicator.localPosition = localPosition;

            context.WaitSeconds(0.2f, () =>
            {
                mapIndicator.DOLocalMove(new Vector3(0, 0, 0), 0.4f);
            });
            
            AudioUtil.Instance.PlayAudioFx("Map_Jump");
            await XUtility.PlayAnimationAsync(indicatorAnimator, animationName, context);
            
            indicatorAnimator.Play("Idle");

            await context.WaitSeconds(0.5f);
            HideMap();
            
            await context.WaitSeconds(1.0f);
            
        }
 
        protected void InitializePageState(int currentPotLevel = -1)
        {
            var extraState11003 = context.state.Get<ExtraState11003>();

            if (currentPotLevel < 0)
                currentPotLevel = (int) extraState11003.GetPotLevel();

            var buffers = extraState11003.GetBuffers();

            var nextBankPage = -1;

            if (currentPotLevel == 0)
            {
                mapIndicator.SetParent(startPoint.Find("FlagGroup"), false);
            }

            for (var i = 0; i < _potList.Count; i++)
            {
                if (i == 0)
                {
                    if (currentPotLevel > 0)
                    {
                        _potList[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        _potList[i].gameObject.SetActive(true);
                    }
                    
                    continue;
                }
                
                var potAnimator = _potList[i].GetComponent<Animator>();

                if (potAnimator == null)
                {
                    potAnimator = _potList[i].Find("Box").GetComponent<Animator>();
                }

                if (i <= currentPotLevel)
                {
               //     XDebug.Log("i:" + i);
                    potAnimator.Play("Finish");
                }
                else
                {
                    potAnimator.Play("Idle",-1,0);
                }

                var flagGroup = _potList[i].transform.Find("FlagGroup");

                if (i == currentPotLevel)
                {
                    mapIndicator.SetParent(flagGroup, false);
                }

                var pigWinChips = extraState11003.GetEachPigWinChipsInSuperFree();

                if (_bankPotIndexList.Contains(i) && i > currentPotLevel)
                {
                     var bufferGroup = _potList[i].transform.Find("Boosters/BuffContent");
                    var pigWinText = _potList[i].transform.Find("BG/Img/BoostersText")
                        .GetComponent<TextMesh>();

                    pigWinText.text = pigWinChips.GetAbbreviationFormat(0);

                    for (var c = 0; c < bufferGroup.childCount; c++)
                    {
                        var child = bufferGroup.GetChild(c);
                        if (child.name != "BGGroup")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }
                    //   pigWinText.
                    var bufferCountText =  _potList[i].transform.Find("Boosters/BoostersTitleBg/BoostersTitle/BoosterNumText1").GetComponent<TextMesh>();
                    var bufferCountText2 =  _potList[i].transform.Find("Boosters/BoostersTitleBg/BoostersTitle/BoosterNumText2").GetComponent<TextMesh>();

                    if (nextBankPage < 0)
                    {
                        nextBankPage = i;

                        var ownedBufferCount = 0;
                        
                        for (var index = 0; index < buffers.Count; index++)
                        {
                            var buff = buffers[index];
                            bool buffAcquired = buff.Acquired;

                            if (buffAcquired)
                            {
                                ownedBufferCount++;
                            }
                            bufferGroup.Find($"Buff{index+1}").gameObject.SetActive(true);
                            bufferGroup.Find($"Buff{index+1}/Win").gameObject.SetActive(buffAcquired);
                            bufferGroup.Find($"Buff{index+1}/Miss").gameObject.SetActive(!buffAcquired);
                        }

                        bufferCountText.text = $"{ownedBufferCount}";
                        bufferCountText2.text = $"{_bankPotIndexList.IndexOf(i) + 2}";
                    }
                    else
                    {
                        bufferCountText.text = "0";
                        bufferCountText2.text = $"{_bankPotIndexList.IndexOf(i) + 2}";
                    }
                }
            }
        }
        
        //public void OnDraw

        public  void FocusCurrentPot(int currentPotLevel = -1)
        {
            var extraState11003 = context.state.Get<ExtraState11003>();
            
            if(currentPotLevel < 0)
                currentPotLevel = (int) extraState11003.GetPotLevel();
 
            var flagTransform = _potList[currentPotLevel].Find("FlagGroup");

            var flagWorldPosition = flagTransform.TransformPoint(Vector3.zero);

            var localPosition = _content.transform.InverseTransformPoint(flagWorldPosition);
            
            _contentPosition = -localPosition.x + 4;
            var position = _content.localPosition;
            position.x = -localPosition.x + 4;
            if (position.x < -contentWidth)
            {
                position.x = -contentWidth;
            }

            if (position.x > 0)
            {
                position.x = 0;
            }
            _content.localPosition = position;
        }
   }
}