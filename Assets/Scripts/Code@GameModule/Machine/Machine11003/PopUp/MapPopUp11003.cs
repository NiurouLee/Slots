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
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class MapPopUp11003 : MachinePopUp
    {
        [ComponentBinder("Root/BackButton")] private Button _backToGameButton;

        [ComponentBinder("Root/Scroll View")] private ScrollRect _scrollRect;

        [ComponentBinder("Root/Scroll View/Viewport/Content")]
        private RectTransform _viewContent;


        [ComponentBinder("Root/Scroll View/Viewport/Content/Page1")]
        private Transform page1;

        [ComponentBinder("Root/Scroll View/Viewport/Content/Page2")]
        private Transform page2;

        [ComponentBinder("Root/Scroll View/Viewport/Content/Page3")]
        private Transform page3;

        [ComponentBinder("Root/Scroll View/Viewport/Content/Page4")]
        private Transform page4;

        [ComponentBinder("Root/Scroll View/Viewport/Content/Page1/StageCell0/FlagGroup")]
        protected Transform startPoint;

        [ComponentBinder("Root/Scroll View/Viewport/Content/Page1/StageCell0/FlagGroup/MapIndicator")]
        private Transform mapIndicator;


        private List<Transform> _pageList;
        private List<Transform> _potList;
        private List<int> _bankPotIndexList;

        private Action backToGameClickedAction;

        private Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string> _buffTypeToAssetNameDict =
            new Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string>()
            {
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn, "ADDReel"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols, "ADD100"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow, "ADDRow"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddFreeSpin, "5Extra"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddExtraBonus, "IncreasePigCoins"},
            };

        public MapPopUp11003(Transform transform) : base(transform)
        {
            _backToGameButton.onClick.AddListener(OnBackGameButtonClicked);

            _pageList = new List<Transform>() {page1, page2, page3, page4};
            _potList = new List<Transform>();
            _bankPotIndexList = new List<int>();

            for (var i = 0; i < _pageList.Count; i++)
            {
                for (var c = 0; c < _pageList[i].childCount; c++)
                {
                    if (i != 0 || c != 0)
                        _potList.Add(_pageList[i].GetChild(c));
                }

                _bankPotIndexList.Add(_potList.Count - 1);
            }

            var monoLateUpdateProxy  = transform.gameObject.AddComponent<MonoLateUpdateProxy>();
           bool focusCalled = false;
           monoLateUpdateProxy.BindingAction(() =>
           {
               if (!focusCalled)
               {
                   focusCalled = true;
                   FocusCurrentPot(mapIndicator);
               }
           });
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            InitializePageState();
        }

        public async void ShowIndicatorMoveAnimation()
        {
            //Indicator的位置要比从服务器获取到的Level小1步
            //跳转动画要从小两步的位置跳到小一步的位置
            _backToGameButton.interactable = false;

            var extraState11003 = context.state.Get<ExtraState11003>();
            var currentPotLevel = (int) extraState11003.GetPotLevel();

            var lastFinishLevel = currentPotLevel - 1;

            if (currentPotLevel == 0)
            {
                lastFinishLevel = _potList.Count - 1;
            }

            InitializePageState(lastFinishLevel);
            
            var potAnimator = _potList[lastFinishLevel].GetComponent<Animator>();
            await XUtility.PlayAnimationAsync(potAnimator, "Completing", context);
            var indicatorAnimator = mapIndicator.GetComponent<Animator>();

            string animationName = "Move01";

            if (_bankPotIndexList.Contains(lastFinishLevel))
            {
                animationName = "Move02";
            }
            else if (lastFinishLevel == 0)
            {
                animationName = "Move04";
            }
            else if (_bankPotIndexList.Contains(lastFinishLevel - 1))
            {
                animationName = "Move03";
            }
            
            await XUtility.PlayAnimationAsync(indicatorAnimator, animationName, context);
            
            var flagGroup = _potList[lastFinishLevel].transform.Find("FlagGroup");
            mapIndicator.SetParent(flagGroup, false);

            indicatorAnimator.Play("Idle");

            _backToGameButton.interactable = true;
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
                mapIndicator.SetParent(startPoint, false);
            }

            for (var i = 0; i < _potList.Count; i++)
            {
                var potAnimator = _potList[i].GetComponent<Animator>();

                if (i < currentPotLevel)
                {
                    potAnimator.Play("Finish");
                }
                else
                {
                    potAnimator.Play("Idle",-1,1);
                }

                var flagGroup = _potList[i].transform.Find("FlagGroup");

                if (i == currentPotLevel - 1)
                {
                    mapIndicator.SetParent(flagGroup, false);
                }

                var pigWinChips = extraState11003.GetEachPigWinChipsInSuperFree();

                if (_bankPotIndexList.Contains(i) && i >= currentPotLevel)
                {
                    var bufferGroup = _potList[i].transform.Find("Root/BaseState/TipsGroup/BUFFGroup");
                    var pigWinText = _potList[i].transform.Find("Root/BaseState/TipsGroup/NoticeGroup/IntegralText")
                        .GetComponent<Text>();

                    pigWinText.text = pigWinChips.GetAbbreviationFormat(1);

                    for (var c = 0; c < bufferGroup.childCount; c++)
                    {
                        var child = bufferGroup.GetChild(c);
                        if (child.name != "BGGroup")
                        {
                            child.gameObject.SetActive(false);
                        }
                    }

                    //   pigWinText.
                    var bufferCountText = bufferGroup.Find("BGGroup/TitleGroup/Text").GetComponent<Text>();

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

                            var name = _buffTypeToAssetNameDict[buff.Type];

                            bufferGroup.Find(name).gameObject.SetActive(true);
                            bufferGroup.Find($"{name}/StateGroup/DisabledImage").gameObject.SetActive(!buffAcquired);
                            bufferGroup.Find($"{name}/StateGroup/EnabledImage").gameObject.SetActive(buffAcquired);
                        }

                        bufferCountText.text = $"[{ownedBufferCount}/{_bankPotIndexList.IndexOf(i) + 2}]";
                    }
                    else
                    {
                        bufferCountText.text = $"[{0}/{_bankPotIndexList.IndexOf(i) + 2}]";
                    }
                }
            }
        }

        public  void FocusCurrentPot(Transform item)
        {
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_viewContent);
            
            var itemPosInContent = _viewContent.InverseTransformPoint(item.position);
            var posX = itemPosInContent.x;

            var viewPortWidth = _scrollRect.viewport.rect.width;
            var targetNormalizedPosition = (posX - viewPortWidth * 0.5f) /
                                           (_viewContent.rect.width - viewPortWidth);

            _scrollRect.horizontalNormalizedPosition = Math.Max(Math.Min(1, targetNormalizedPosition), 0);


            //   transform.gameObject.SetActive(true);
            // var viewContentPos = _viewContent.anchoredPosition;
            // _viewContent.anchoredPosition = new Vector2(-2144, viewContentPos.y);
        }

        public void OnBackGameButtonClicked()
        {
            Close();
        }

        public override async Task OnClose()
        {
            await base.OnClose();
            backToGameClickedAction?.Invoke();
        }
    }
}