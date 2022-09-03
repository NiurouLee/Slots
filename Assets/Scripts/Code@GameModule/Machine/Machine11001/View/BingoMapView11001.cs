// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/14:57
// Ver : 1.0.0
// Description : BingoMapView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class BingoItemView : TransformHolder
    {
        [ComponentBinder("Root/BingoWheel")] 
        private Transform _bingoWheel;

        [ComponentBinder("Root/BingoWheel/Jackpot")]
        private Transform _bingoWheelJackpot;

        [ComponentBinder("Root/BingoWheel/Coin")]
        private Transform _bingoWheelCoin;

        [ComponentBinder("Root/BingoWheel/Icon")]
        private Transform _bingoWheelIcon;

        [ComponentBinder("Root/BingoCoin")] 
        private Transform _bingoCoin;

        [ComponentBinder("Root/BingoCoin/Jackpot")]
        private Transform _bingoCoinJackpot;

        [ComponentBinder("Root/BingoCoin/Coin")]
        private Transform _bingoCoinCoin;     
     
        [ComponentBinder("Root/BingoCoin/JackpotPlusCoin")]
        private Transform _bingoCoinJackpotPlusCoin;

        private Animator _wheelAnimtor;
        private Animator _coinAnimtor;

     

        private BingoCloverGameResultExtraInfo.Types.BingoItem _bingoItem;

        private BingoMapView11001 _mapView11001;
        

        public BingoItemView(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);

            _wheelAnimtor = _bingoWheel.GetComponent<Animator>();
            _coinAnimtor = _bingoCoin.GetComponent<Animator>();
            _wheelAnimtor.keepAnimatorControllerStateOnDisable = true;
            _coinAnimtor.keepAnimatorControllerStateOnDisable = true;
        }

        public void EnableWheel(bool enable)
        {
            if (_bingoWheel.gameObject.activeSelf == enable)
                return;

            if (!enable)
            {
                _wheelAnimtor.CrossFade("Normal", 0f);
                _wheelAnimtor.Update(0);
            }

            _bingoWheel.gameObject.SetActive(enable);
        }

        public void EnableCoin(bool enable)
        {
            if (_bingoCoin.gameObject.activeSelf == enable)
                return;

            if (!enable)
            {
                _coinAnimtor.CrossFade("Normal", 0f);
                _coinAnimtor.Update(0);
            }

            _bingoCoin.gameObject.SetActive(enable);
        }
        
        
        public void UpdateItemView(BingoCloverGameResultExtraInfo.Types.BingoItem bingoItem, long bet, bool animation = false, string animationName = null)
        {
            _bingoItem = bingoItem;
            
            if (bingoItem.IsCentre || bingoItem.JackpotId > 0 || bingoItem.WinRate > 0)
            {
                transform.gameObject.SetActive(true);
               
                EnableWheel(bingoItem.IsCentre);
                EnableCoin(!bingoItem.IsCentre);
                
                // _bingoWheel.gameObject.SetActive(bingoItem.IsCentre);
                // _bingoCoin.gameObject.SetActive(!bingoItem.IsCentre);

                if (bingoItem.IsCentre)
                {
                    if (bingoItem.JackpotId > 0)
                    {
                        _bingoWheelJackpot.gameObject.SetActive(true);
                        _bingoWheelCoin.gameObject.SetActive(false);

                        _bingoWheelJackpot.Find("JPL1").gameObject.SetActive(bingoItem.JackpotId == 1);
                        _bingoWheelJackpot.Find("JPL2").gameObject.SetActive(bingoItem.JackpotId == 2);
                        _bingoWheelJackpot.Find("JPL3").gameObject.SetActive(bingoItem.JackpotId == 3);
                        _bingoWheelJackpot.Find("JPL4").gameObject.SetActive(bingoItem.JackpotId == 4);
                        _bingoWheelJackpot.Find("JPL5").gameObject.SetActive(bingoItem.JackpotId == 5);

                        _bingoWheelIcon.Find("Wheel").gameObject.SetActive(false);
                    }
                    else if (bingoItem.WinRate > 0)
                    {
                        _bingoWheelJackpot.gameObject.SetActive(false);
                        _bingoWheelCoin.gameObject.SetActive(true);

                        var coinText = _bingoWheelCoin.Find("IntegralText").GetComponent<TextMesh>();
                        
                        var chips = BetState.GetWinChips(bet, (long)bingoItem.WinRate);
                        coinText.text = XUtility.GetLimitLengthAbbreviationFormat(chips);
                        
                        _bingoWheelIcon.Find("Wheel").gameObject.SetActive(false);
                    }
                    else
                    {
                        _bingoWheelJackpot.gameObject.SetActive(false);
                        _bingoWheelCoin.gameObject.SetActive(false);

                        _bingoWheelIcon.Find("Wheel").gameObject.SetActive(true);
                    }
                }
                else
                {
                    if (bingoItem.JackpotId > 0 && bingoItem.WinRate > 0)
                    {
                        _bingoCoinCoin.gameObject.SetActive(false);
                        _bingoCoinJackpot.gameObject.SetActive(false);
                        _bingoCoinJackpotPlusCoin.gameObject.SetActive(true);


                        _bingoCoinJackpotPlusCoin.Find("JPL1").gameObject.SetActive(bingoItem.JackpotId == 1);
                        _bingoCoinJackpotPlusCoin.Find("JPL2").gameObject.SetActive(bingoItem.JackpotId == 2);
                        _bingoCoinJackpotPlusCoin.Find("JPL3").gameObject.SetActive(bingoItem.JackpotId == 3);

                        var coinText = _bingoCoinJackpotPlusCoin.Find("IntegralText").GetComponent<TextMesh>();
                        
                        var chips = BetState.GetWinChips(bet, (long)bingoItem.WinRate); 
                        coinText.text = XUtility.GetLimitLengthAbbreviationFormat(chips);
                    }
                    else if (bingoItem.JackpotId > 0)
                    {
                        _bingoCoinCoin.gameObject.SetActive(false);
                        _bingoCoinJackpot.gameObject.SetActive(true);
                        _bingoCoinJackpotPlusCoin.gameObject.SetActive(false);

                        _bingoCoinJackpot.Find("JPL1").gameObject.SetActive(bingoItem.JackpotId == 1);
                        _bingoCoinJackpot.Find("JPL2").gameObject.SetActive(bingoItem.JackpotId == 2);
                        _bingoCoinJackpot.Find("JPL3").gameObject.SetActive(bingoItem.JackpotId == 3);
                    }
                    else if (bingoItem.WinRate > 0)
                    {
                        _bingoCoinCoin.gameObject.SetActive(true);
                        _bingoCoinJackpot.gameObject.SetActive(false);
                        _bingoCoinJackpotPlusCoin.gameObject.SetActive(false);

                        var coinText = _bingoCoinCoin.Find("IntegralText").GetComponent<TextMesh>();

                        var chips = BetState.GetWinChips(bet, (long)bingoItem.WinRate);
                        
                        coinText.text = XUtility.GetLimitLengthAbbreviationFormat(chips);
                    }
                }

                if (animation)
                {
                    if (animationName == null)
                        PlayAnimation("Down");
                    else
                    {
                        PlayAnimation(animationName);
                    }
                }
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        public void PlayAnimation(string animation)
        {
            // XDebug.Log($"{id} PlayCWAnimation[{animation}]");
            // XDebug.Log($"{id} PlayCWAnimation[{_bingoItem.JackpotId}]");
            // XDebug.Log($"{id} PlayCWAnimation[{_bingoItem.WinRate}]");
            // XDebug.Log($"{id} PlayCWAnimation[{_bingoItem.IsCentre}]");

            if (_bingoItem.WinRate > 0 || _bingoItem.JackpotId > 0 || _bingoItem.IsCentre)
            {
                if (_wheelAnimtor && _bingoItem.IsCentre)
                {
               //     XDebug.Log($"{id} PlayWAnimation[{animation}]");
                    _wheelAnimtor.Play(animation, 0, 0);
               //     XDebug.Log($"{id} PlayWAnimation[{animation}]");
                }
                else if (_coinAnimtor && _bingoCoin)
                {
              //      XDebug.Log($"{id} PlayCAnimation[{animation}]");
                    _coinAnimtor.Play(animation, 0, 0);
              //      XDebug.Log($"{id} PlayCAnimation[{animation}]");
                }
            }
        }
    }

    public class BingoMapView11001 : TransformHolder
    {
        private ExtraState11001 _extraState11001;

        [ComponentBinder("ItemGroup")] private Transform _itemGroup;
        [ComponentBinder("RewardIncreased")] private Transform _rewardIncreaseTransform;
        [ComponentBinder("GameFinish")] private Transform _gameFinish;

        [ComponentBinder("GameFinish/IntegralGroup/WinText")]
        private TextMesh _winText;

        [ComponentBinder("GameFinish/ep_Bingo_Hit")]
        private Transform _hitFx;
        
        [ComponentBinder("BingoReset")]
        private Transform _bingoReset;

        [ComponentBinder("Shiny")]
        private Transform _shinyLines;
        
        private List<BingoItemView> _bingoItemList;

        private Dictionary<int, GameObject> _hostSpotGameObjectDict;

        private Transform hotSpotLayer;

        private Animator _animator;

        private bool _needRestBingoMap;

        private BingoCloverGameResultExtraInfo.Types.BingoData _lastBingoData;
        
        private List<int> _currentHotLineIds;

        private long bingoCurrentWin = 0;
        public BingoMapView11001(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);

            _bingoItemList = new List<BingoItemView>();
            _hostSpotGameObjectDict = new Dictionary<int, GameObject>();
            _animator = transform.GetComponent<Animator>();
            _currentHotLineIds = new List<int>();
            _animator.keepAnimatorControllerStateOnDisable = true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            _extraState11001 = context.state.Get<ExtraState11001>();

            CreateBingoItemContainer();
            CreateHotSpotLayer();
        }
        
        public void RefreshBingoItem(BingoCloverGameResultExtraInfo.Types.BingoData bingoData = null, bool animation = false)
        {
            if(bingoData == null)
                bingoData = _extraState11001.GetBingoData();
            
            XDebug.Log("RefreshBingoItem:" + bingoData.Bet);
            
            for (int i = 0; i < bingoData.Items.count; i++)
            {
          
                XDebug.Log($"PosId:{bingoData.Items[i].PositionId}; JackpotID:{bingoData.Items[i].JackpotId}; WinRate:{bingoData.Items[i].WinRate}");
             
                _bingoItemList[bingoData.Items[i].PositionId].UpdateItemView(bingoData.Items[i], (long)bingoData.Bet, animation);
             
                if(!animation)
                    _bingoItemList[bingoData.Items[i].PositionId].PlayAnimation("Normal");
            }
            
            RefreshHotSpot(bingoData.HotLines);
        }
       
        public void PlayHotLineEffect(RepeatedField<BingoCloverGameResultExtraInfo.Types.BingoHotLine> hotLines, RepeatedField<BingoCloverGameResultExtraInfo.Types.BingoItem> newItems)
        {
            if (hotLines.Count <= 0)
            {
                context.RemoveWaitEvent(WaitEvent.WAIT_SPECIAL_EFFECT);
                return;
            }
            
            var listPostId = new List<int>(10);

            var lineToPlayShinyEffect = new List<int>();
       
            for (var i = 0; i < newItems.Count; i++)
            {
                var posId =  newItems[i].PositionId;
               listPostId.Add(posId);
             
               if (newItems[i].OtherPositionIds.Count > 0)
               {
                   for(var o = 0; o < newItems[i].OtherPositionIds.Count; o++)
                        listPostId.Add(newItems[i].OtherPositionIds[o]);
               }

               for (var lineIndex = 0; lineIndex < hotLines.Count; lineIndex++)
               {
                   var line = hotLines[lineIndex];
                   for (var id = 0; id < listPostId.Count; id++)
                   {
                       if (line.PositionIds.Contains(listPostId[id]))
                       {
                           if(!lineToPlayShinyEffect.Contains(line.LineId))
                                lineToPlayShinyEffect.Add(line.LineId);
                       }
                   }
               }
            }

            XDebug.Log("PlayShinyEffect:" + lineToPlayShinyEffect.Count);
            
            if (lineToPlayShinyEffect.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFx("J01_SaoGuang");
                for (var id = 0; id < lineToPlayShinyEffect.Count; id++)
                {
                    XDebug.Log("PlayShinyEffect:LineId:" + lineToPlayShinyEffect[id]);
                    
                    var shinyLine = _shinyLines.Find("Line" + lineToPlayShinyEffect[id]);
                    shinyLine.gameObject.SetActive(true);
                    
                    var animator = shinyLine.GetComponent<Animator>();
                    
                    animator.Play("Forward",0,0);
                }
            }
            
            context.WaitSeconds(0.5f, () =>
            {
                context.RemoveWaitEvent(WaitEvent.WAIT_SPECIAL_EFFECT);
            });
        }
       
        public async Task StartBingoAnimation()
        {
            var bingoData = _extraState11001.GetBingoData();
            
            AudioUtil.Instance.FadeOutMusic(0.5f);
           
            context.state.Get<JackpotInfoState>().LockJackpot = true;
            context.view.Get<JackPotPanel>().UpdateJackpotAndLockJackpotPanel();
            
            ShowBingoItemTriggerAnimation();

            await context.WaitSeconds(2);
         
            // CurrentBonusLine == 0 表示正常结算，大于0表示，恢复之前的结算
            bingoCurrentWin = 0;
            
            if (bingoData.CurrentBonusLine == 0)
            {
                _winText.SetText("");
                
                AudioUtil.Instance.PlayAudioFxOneShot("Bingo");
                _animator.Play("Open", -1, 0);

                await context.WaitSeconds(2.75f);
                    
                AudioUtil.Instance.PlayAudioFxOneShot("WinnerAppear");
                
                await context.WaitSeconds(3.05f);
            }
            else
            {
                long currentWin = 0;
                for (var i = 0; i < bingoData.CurrentBonusLine; i++)
                {
                    currentWin += (long)bingoData.Lines[i].TotalWinRate;
                }
                
                _winText.SetText(BetState.GetWinChips((long)bingoData.Bet, currentWin).GetCommaFormat());
                
                bingoCurrentWin = currentWin;
                
                _animator.Play("Idle", -1, 0);
            }

            _lastBingoData = bingoData;

            while (bingoData.CurrentBonusLine < bingoData.Lines.Count)
            {
                bingoData = _extraState11001.GetBingoData();
                _lastBingoData = bingoData;
                
                await CollectBingoLine();

                var bingoTotalWin = _lastBingoData.TotalWin;
                
                XDebug.Log("bingoTotalWin:" + bingoTotalWin);
                
                //收集完成之后新的BingoData
                bingoData = _extraState11001.GetBingoData();
                if (bingoData.CurrentBonusLine == 0)
                {
                    PlayAllBingoTriggerAnimation();
                    break;
                }
            }
            
            await context.WaitSeconds(1);
            await XUtility.PlayAnimationAsync(_animator, "Close", context);
          
            //
            // var winState = context.state.Get<WinState>();
            //  
            // if (winState.winLevel >= (int)WinLevel.BigWin)
            // {
            //     await WinEffectHelper.ShowBigWinEffectAsync(winState.winLevel, winState.displayTotalWin, context);
            // }
          
            _needRestBingoMap = true;
        }

        public void PlayAllBingoTriggerAnimation()
        {
            for (var i = 0; i < _lastBingoData.Lines.Count; i++)
            {
                var line = _lastBingoData.Lines[i];
                for (var index = 0; index < line.Items.Count; index++)
                {
                    var item = line.Items[index];
                    _bingoItemList[item.PositionId].PlayAnimation("Trigger");
                }
            }
        }
        public async Task ResetBingoMap()
        {
             _needRestBingoMap = false;
            
             PlayAllBingoTriggerAnimation();
             
             //PlayerReset Animation 
             AudioUtil.Instance.PlayAudioFxOneShot("Bingo_Reset");
             await context.WaitSeconds(1.0f);

             var animator = _bingoReset.GetComponent<Animator>();
             _bingoReset.gameObject.SetActive(true);
             
             AudioUtil.Instance.PlayAudioFxOneShot("Video1");
             animator.Play("Open");
             
             await context.WaitSeconds(1.0f);
            
             ClearBingoMap();
             
             await context.WaitSeconds(4f);
             
             _bingoReset.gameObject.SetActive(false);
            
             RefreshBingoItem(null,true);
             
             RefreshHotSpot(_extraState11001.GetHotLines());
             
             await context.WaitSeconds(1.5f);
             
             AudioUtil.Instance.FadeInMusic(0.5f);
        }

        protected void ClearBingoMap()
        {
            var item = new BingoCloverGameResultExtraInfo.Types.BingoItem();
            for (int i = 0; i < _bingoItemList.Count; i++)
            {
                item.IsCentre = i == 12 ? true : false;
                item.PositionId = i;
                
                _bingoItemList[i].UpdateItemView(item, 0);
            }
        }

        public bool IsNeedRestBingoMap()
        {
            return _needRestBingoMap;
        }

        public async Task CollectBingoLine()
        {
            var bingoData = _extraState11001.GetBingoData();

            if (bingoData.CurrentBonusLine >= bingoData.Lines.Count)
                return;

            var currentLine = bingoData.Lines[bingoData.CurrentBonusLine];

            for (var i = 0; i < currentLine.Items.Count; i++)
            {
                var bingoItem = currentLine.Items[i];
               
                if (bingoItem.IsCentre)
                {
                    //14关Bonus玩法，需要断线之后重新再玩一遍，服务器其实已经算玩了，这个时候客户端需要做一个假的表现，所以这里不能显示bonus结果
                    if (context.assetProvider.AssetsId == "11014" && (bingoItem.JackpotId > 0 || bingoItem.WinRate > 0))
                    {
                        bingoItem = new BingoCloverGameResultExtraInfo.Types.BingoItem();
                        bingoItem.IsCentre = true;
                        bingoItem.PositionId = 12;
                    }
                    
                    _bingoItemList[bingoItem.PositionId].UpdateItemView(bingoItem, (long)bingoData.Bet);

                    break;
                }
            }
            
            for (var i = 0; i < currentLine.Items.Count; i++)
            {
                var bingoItem = currentLine.Items[i];
                
                _bingoItemList[bingoItem.PositionId].PlayAnimation("Win");
             
                await context.WaitSeconds(0.5f);
                
                if (!bingoItem.IsCentre)
                {
                    await FlyCollectFx(bingoItem);
                    
                    DoWinTextUpdate((long)bingoData.Bet,(long)(bingoItem.WinRate + bingoItem.JackpotPay));
                    await context.WaitSeconds(0.5f);
                }
                else
                {
                    if (context.assetProvider.AssetsId == "11001" && (bingoItem.JackpotId > 0 || bingoItem.WinRate > 0))
                    {
                        await FlyCollectFx(bingoItem);

                        DoWinTextUpdate((long) bingoData.Bet, (long) (bingoItem.WinRate + bingoItem.JackpotPay));
                        await context.WaitSeconds(0.5f);
                    }
                    else
                    {
                        TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
                        
                        var index = i;
                        ProcessBingoBonus(i, async () =>
                        {
                            context.view.Get<SuperFreeProgressView11001>().UpdateIndicatorSortOrder(true);

                            bingoData = _extraState11001.GetBingoData();
                            currentLine = bingoData.Lines[bingoData.CurrentBonusLine];
                            bingoItem = currentLine.Items[index];

                            _bingoItemList[bingoItem.PositionId].UpdateItemView(bingoItem, (long) bingoData.Bet);

                            await FlyCollectFx(bingoItem);

                            DoWinTextUpdate((long) bingoData.Bet, (long) (bingoItem.WinRate + bingoItem.JackpotPay));

                            await context.WaitSeconds(0.5f);

                            waitTask.SetResult(true);
                        });
                       
                        await waitTask.Task;
                    }
                }
            }
            
            
            XDebug.Log("Send Line Finish Proto");
            var bonusProcess = await _extraState11001.SendBonusProcess();
        }

        private void ProcessBingoBonus(int index, Action bonusFinishCallback)
        {
            context.view.Get<SuperFreeProgressView11001>().UpdateIndicatorSortOrder(false);
            if (context.assetProvider.AssetsId == "11001")
            {
                var jackpotWheelView = context.view.Get<JackpotWheelView11001>();
                jackpotWheelView.StartShowJackpotWheel();
                jackpotWheelView.SetSpinFinishCallback(bonusFinishCallback);
            }
            else if (context.assetProvider.AssetsId == "11014")
            {
                var jackpotWheelView = context.view.Get<BonusWheelView11014>();
                jackpotWheelView.StartShowJackpotWheel();
                jackpotWheelView.SetSpinFinishCallback(bonusFinishCallback);
            }
        }

        private void DoWinTextUpdate(long bet, long deltaWin)
        {
            DOTween.Kill(_winText);
            
            long v = BetState.GetWinChips(bet, bingoCurrentWin);
            ;
            var endChips = BetState.GetWinChips(bet, bingoCurrentWin + deltaWin);
           
            AudioUtil.Instance.PlayAudioFxOneShot("J01Win");
           
            DOTween.To(() => v, (x) =>
            {
                v = x;
                _winText.SetText(v.GetCommaFormat());
            }, endChips, 0.5f).OnComplete(() =>
            {
                _winText.SetText(endChips.GetCommaFormat());
            });
            bingoCurrentWin += deltaWin;
        }

        private async Task FlyCollectFx(BingoCloverGameResultExtraInfo.Types.BingoItem bingoItem)
        {
            var tailFx = context.assetProvider.InstantiateGameObject("ep_Bingo_Trail");

            AudioUtil.Instance.PlayAudioFx("J01_Breathe");
            
            tailFx.transform.SetParent(_gameFinish);
            tailFx.transform.position = _bingoItemList[bingoItem.PositionId].transform.position;
            XUtility.Fly(tailFx.transform, tailFx.transform.position, _winText.transform.position, 0, 0.5f,
                async () =>
                {
                    _hitFx.gameObject.SetActive(true);
                    await context.WaitSeconds(0.5f);
                    GameObject.Destroy(tailFx);
                    //TODO ADD Chips
                }, Ease.Linear, context);
            
            await context.WaitSeconds(0.5f);
            
            context.WaitSeconds( 1, () =>
            {
                _hitFx.gameObject.SetActive(false);
            });
        }

        public void ShowBingoItemTriggerAnimation()
        {
            RecycleHotSpot();

            var bingoData = _extraState11001.GetBingoData();
            var triggerItemPositionIdList = new List<int>();

            if (bingoData.Lines.Count > 0)
            {
                AudioUtil.Instance.PlayAudioFx("J01_Trigger");
                for (var i = 0; i < bingoData.Lines.Count; i++)
                {
                    var line = bingoData.Lines[i];
                    for (var index = 0; index < line.Items.Count; index++)
                    {
                        var item = line.Items[index];
                        triggerItemPositionIdList.Add(item.PositionId);

                        if (i >= bingoData.CurrentBonusLine)
                            _bingoItemList[item.PositionId].PlayAnimation("Trigger");
                        else
                        {
                            _bingoItemList[item.PositionId].PlayAnimation("Normal");
                        }
                    }
                }
            }

            var items = bingoData.Items;

            for (var i = 0; i < items.Count; i++)
            {
                if (!triggerItemPositionIdList.Contains(items[i].PositionId))
                {
                    _bingoItemList[items[i].PositionId].PlayAnimation("Dark");
                }
            }
        }

        public  void ShowPanelNewBingoItemCollectFx()
        {
            var bingoData = _extraState11001.GetBingoData();

            var hotLines = bingoData.HotLines;

            var panelBingoItem = _extraState11001.GetNewBingoItemFromPanel();
            
            context.AddWaitEvent(WaitEvent.WAIT_SPECIAL_EFFECT);
            context.AddWaitEvent(WaitEvent.WAIT_SPECIAL_EFFECT_2);

            for (var i = 0; i < bingoData.Items.Count; i++)
            {
                if (bingoData.Items[i].JackpotId > 0 || bingoData.Items[i].WinRate > 0)
                {
                    XDebug.Log(
                        $"BingoData{i}:Pos{bingoData.Items[i].PositionId}:winRate:{bingoData.Items[i].WinRate}:jackpotId:{bingoData.Items[i].JackpotId}");
                }
            }

            bool needPlayDiffusionAnimation = false;
            
            for (var i = 0; i < panelBingoItem.Count; i++)
            {
                var index = i;
                var item = panelBingoItem[i];

                XDebug.Log($"New Bingo{i}:Pos{item.PositionId}:winRate:{item.WinRate}:jackpotId:{item.JackpotId}");

                var wheel = context.view.Get<Wheel>(0);
                var roll = wheel.GetRoll(item.PositionId);
                var flyStartPosition = roll.GetVisibleContainer(0).transform.position;

                var coinFlyFx = context.assetProvider.InstantiateGameObject("CoinFlyFx");

                var coinFlyFxParent = new GameObject();
                coinFlyFxParent.transform.SetParent(transform, false);
                coinFlyFx.transform.SetParent(coinFlyFxParent.transform);
                coinFlyFxParent.transform.position = flyStartPosition;
                
                var sortingGroup = coinFlyFx.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                sortingGroup.sortingOrder = 10;

                if (item.OtherPositionIds.Count > 0)
                {
                    needPlayDiffusionAnimation = true;
                }

                if (index == 0)
                {
                    AudioUtil.Instance.PlayAudioFx("J01_Fly");
                }

                XUtility.PlayAnimation(coinFlyFx.GetComponent<Animator>(), "Open", async () =>
                {
                    GameObject.Destroy(coinFlyFxParent);
                    
                    if (index == 0)
                    {
                        AudioUtil.Instance.PlayAudioFx("J01_Landing");
                    }
                    _bingoItemList[item.PositionId].UpdateItemView(bingoData.Items[item.PositionId], (long)bingoData.Bet,true);

                    if (item.OtherPositionIds != null && item.OtherPositionIds.Count > 0)
                    {
                        await context.WaitSeconds(0.5f);
                        var itemCount = item.OtherPositionIds.Count;

                        if (index == 0 && itemCount > 0)
                        {
                            AudioUtil.Instance.PlayAudioFx("J01_KuoSan");
                        }
                        for (var c = 0; c < itemCount; c++)
                        {
                            var diffusionIndex = c;
                            PlayDiffusionAnimation(item.PositionId, item.OtherPositionIds[c]);
                            
                            context.WaitSeconds(0.5f, () =>
                            {
                                _bingoItemList[item.OtherPositionIds[diffusionIndex]]
                                    .UpdateItemView(bingoData.Items[item.OtherPositionIds[diffusionIndex]],
                                        (long)bingoData.Bet, true);
                            });
                        }
                    }

                }, context);
            }

            float waitTime = 1f;

            if (needPlayDiffusionAnimation)
            {
                waitTime = 2.5f;
            }
            
            context.WaitSeconds(waitTime, () =>
            {
                context.RemoveWaitEvent(WaitEvent.WAIT_SPECIAL_EFFECT_2);
                var currentBingoData = _extraState11001.GetBingoData();
                if (currentBingoData.Bet == bingoData.Bet)
                {
                    RefreshHotSpot(hotLines);
                    PlayHotLineEffect(hotLines, panelBingoItem);
                }
            });
        }

        public void PlayDiffusionAnimation(int fromStartId, int endStartIndex)
        {
            var diffusion = context.assetProvider.InstantiateGameObject("Diffusion");

            diffusion.transform.SetParent(_itemGroup,false);
            diffusion.transform.position = _bingoItemList[fromStartId].transform.position;
            
            diffusion.transform.DOMove(_bingoItemList[endStartIndex].transform.position, 0.5f).OnComplete(
                () =>
                {
                    GameObject.Destroy(diffusion);
                });
        }

        public async Task ShowRewardIncreaseEffect()
        {
            _rewardIncreaseTransform.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Video2");
            await context.WaitSeconds(3.8f);
            
            _rewardIncreaseTransform.gameObject.SetActive(false);
            
            var bingoData = _extraState11001.GetBingoData();

            var randomRewardItem = bingoData.RandomIncrease;

            var cloneBingoData = ProtocolUtils.CloneFrom(bingoData);

            var panelItemData = cloneBingoData.PanelIncrease;

            //服务器下发的的BingoData中的ItemData是最终的Data，这里需要计算一个最终结果
            
            for (var i = 0; i < panelItemData.Count; i++)
            {
                cloneBingoData.Items[panelItemData[i].PositionId].WinRate -= panelItemData[i].WinRate;
            }
            
            //剔除由于这次SPIN Panel上的金币图标导致的的HOT SPOT
            var hotLines = cloneBingoData.HotLines;
       
            if (hotLines.Count > 0)
            {
                var hotLineToRemove = new List<int>();
                for (var i = 0; i < hotLines.Count; i++)
                {
                    var lines = hotLines[i];
                    // var emptyId = 0;
                    for (var pos = 0; pos < lines.PositionIds.Count; pos++)
                    {
                        if (lines.PositionIds[pos] > 0)
                        {
                            var item = cloneBingoData.Items[lines.PositionIds[pos]];
                            //没有WinRate也没有Jackpot说明是一个空的Item
                            if (item.WinRate <= 0 && item.JackpotId <= 0 && !item.IsCentre)
                            {
                                hotLineToRemove.Add(i);
                            }
                        } 
                        // else if (lines.PositionIds[pos] < 0)
                        // {
                        //     emptyId = -lines.PositionIds[pos];
                        // }
                    }
                }

                for (var i = hotLines.Count - 1; i >= 0; i--)
                {
                    if (hotLineToRemove.Contains(i))
                    {
                        hotLines.RemoveAt(i);
                    }
                }
            }
            
            //更新BING MAP到中间结果
            for (int i = 0; i < randomRewardItem.Count; i++)
            {
                AudioUtil.Instance.PlayAudioFx("J01_Landing");
                _bingoItemList[randomRewardItem[i].PositionId].UpdateItemView(cloneBingoData.Items[randomRewardItem[i].PositionId], (long)bingoData.Bet,true, "Down2");
                await context.WaitSeconds(0.5f);
            }

            await context.WaitSeconds(0.5f);
            
            RefreshHotSpot(hotLines);
            PlayHotLineEffect(hotLines, randomRewardItem);
        }

        public void CreateHotSpotLayer()
        {
            hotSpotLayer = new GameObject("HotSpotLayer").transform;
            var sortingGroup = hotSpotLayer.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalUI");
            sortingGroup.sortingOrder = 8;
            hotSpotLayer.SetParent(_itemGroup);
        }

        protected void RecycleHotSpot()
        {
            foreach (var item in _hostSpotGameObjectDict)
            {
                context.assetProvider.RecycleGameObject("HotSpot", item.Value);
            }
            _hostSpotGameObjectDict.Clear();
        }

        public void RefreshHotSpot(RepeatedField<BingoCloverGameResultExtraInfo.Types.BingoHotLine> hotSpotLines)
        {
            
            var wheel = context.view.Get<Wheel>();

        //    RecycleHotSpot();

            var hotSpotPos = new List<int>();
          
            _currentHotLineIds.Clear();
            
            for (var i = 0; i < hotSpotLines.Count; i++)
            {
                var line = hotSpotLines[i];
                int id = 0;
                
                _currentHotLineIds.Add(hotSpotLines[i].LineId);
                
                for (var index = 0; index < line.PositionIds.Count; index++)
                {
                    if (line.PositionIds[index] < 0)
                    {
                        id = -line.PositionIds[index];
                        break;
                    }
                }
                
                if(!hotSpotPos.Contains(id))
                    hotSpotPos.Add(id);
            }

            var keysInDict = _hostSpotGameObjectDict.Keys.ToList();
          
            foreach (var id in  keysInDict)
            {
                if (!hotSpotPos.Contains(id) && !hotSpotPos.Contains(id - 30))
                {
                    var item = _hostSpotGameObjectDict[id];
                    _hostSpotGameObjectDict.Remove(id);
                    context.assetProvider.RecycleGameObject("HotSpot", item);
                }
            }
            
            for (var i = 0; i < hotSpotPos.Count; i++)
            {
                var id = hotSpotPos[i];
                
                if(id < 0 || _hostSpotGameObjectDict.ContainsKey(id))
                    continue;

                var hotSpotGameObjectOnBingo = context.assetProvider.InstantiateGameObject("HotSpot", true);
                _hostSpotGameObjectDict.Add(id, hotSpotGameObjectOnBingo);

                hotSpotGameObjectOnBingo.transform.SetParent(hotSpotLayer, false);
                hotSpotGameObjectOnBingo.transform.position = _bingoItemList[id].transform.position;
                
                var hotSpotGameObjectOnPanel = context.assetProvider.InstantiateGameObject("HotSpot", true);
              
                //Panel上的HotSpot
                _hostSpotGameObjectDict.Add(id + 30, hotSpotGameObjectOnPanel);

                var roll = wheel.GetRoll(id);

                hotSpotGameObjectOnPanel.transform.SetParent(wheel.transform, false);
                var elementPosition = roll.GetVisibleContainerPosition(0);
                
                hotSpotGameObjectOnPanel.transform.position = elementPosition;
            }
        }

        public void CreateBingoItemContainer()
        {
            var containerSize = _itemGroup.Find("ItemMask").GetComponent<SpriteMask>().bounds.size;
            var cellSize = containerSize * 0.2f;

            var sortingGroup = _itemGroup.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
            sortingGroup.sortingOrder = 5;

            for (var i = 0; i < 5; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    var itemContainer = new GameObject($"BingoContainer{i * 5 + j}");
                    itemContainer.transform.SetParent(_itemGroup, false);
                    itemContainer.transform.localPosition =
                        new Vector3(-cellSize.x * 2 + cellSize.x * i, cellSize.y * 2f - cellSize.y * j);

                    var item = context.assetProvider.InstantiateGameObject("BingoItem");

                    item.transform.SetParent(itemContainer.transform, false);
                    var itemView = new BingoItemView(item.transform);

                    _bingoItemList.Add(itemView);
                }
            }
        }
    }
}