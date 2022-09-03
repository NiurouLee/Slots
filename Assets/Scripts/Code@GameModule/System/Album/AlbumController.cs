// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/08/14:10
// Ver : 1.0.0
// Description : AlbumController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class AlbumController : LogicController
    {
        private AlbumModel _albumModel;
        private bool isNewUnlocked = false;

        private bool _needUpdateAlbumOnRoundEnd = false;
        public AlbumController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _albumModel = new AlbumModel();
        }
        

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            await _albumModel.FetchModalDataFromServerAsync();
            EventBus.Dispatch(new EventAlbumInfoDataUpdate());
            SeasonEndCheck();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventShowSpinDropCardFinished>(OnShowSpinDropCardFinished);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnLevelChanged);
            SubscribeEvent<EventOnVipLevelUp>(OnVipLevelChanged);
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.Album);
        }
        
        protected async void OnVipLevelChanged(EventOnVipLevelUp evt)
        {
            await _albumModel.FetchModalDataFromServerAsync();
            EventBus.Dispatch(new EventAlbumInfoDataUpdate());
            SeasonEndCheck();
        }

        protected async void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (_needUpdateAlbumOnRoundEnd)
            {
                await _albumModel.FetchModalDataFromServerAsync();
                EventBus.Dispatch(new EventAlbumInfoDataUpdate());
                SeasonEndCheck();
                _needUpdateAlbumOnRoundEnd = false;
            }
            
            handleEndCallback.Invoke();
        }

        public uint GetAmazingHatCountDown()
        {
            if (_albumModel.GetModelData() != null)
                return _albumModel.GetModelData().HatGameCountDown;
            return 0;
        }
         
        protected void OnLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (evt.levelUpInfo.Level >= _albumModel.GetUnlockLevel())
            {
                if (evt.levelUpInfo.Level == _albumModel.GetUnlockLevel())
                    isNewUnlocked = true;
                
                _needUpdateAlbumOnRoundEnd = true;
            }
        }

        private CancelableCallback _cancelableCallback;
        
        protected void SeasonEndCheck()
        {
            var countDown = _albumModel.GetSeasonCountDown();
            if (countDown > 0)
            {
                if (_cancelableCallback != null)
                {
                    XDebug.LogOnExceptionHandler("SeasonEndCheck:CancelCallback" + countDown); 
                    _cancelableCallback.CancelCallback();
                }

                XDebug.LogOnExceptionHandler("SeasonEndCheck:" + countDown); 
                
                _cancelableCallback = WaitForSecondsRealTime(countDown, () =>
                {
                   
                    XDebug.LogOnExceptionHandler("SeasonEndCheck:CallbackExecuted" + countDown);
                   
                    if (IsUnlocked())
                    {
                        XDebug.LogOnExceptionHandler("SeasonEndCheck:CallbackExecutedIsUnlocked" + countDown);

                        PopupStack.ShowPopupNoWait<AlbumSeasonEndHPopup>();
                        EventBus.Dispatch(new EventAlbumSeasonEnd());
                    }
                });
            }
        }
        
        private void OnShowSpinDropCardFinished(EventShowSpinDropCardFinished evt)
        {
            CheckAndClaimLuckyChallenge(evt.cardUpdateInfo, null, "CardDrop");
        }
        /// <summary>
        /// 在打开卡册入口的地方，检查玩家是否有已经完成的收集的卡册由于意外没有领取，在这里补充领取一次，避免奖励不领取不正常的意外发生
        /// </summary>
        /// <param name="finishCallback"></param>
        public void CheckAndClaimFinishCardSetReward(Action finishCallback = null)
        {
            var listFinishCardSet = new List<uint>();

            var cardSetCount = GetCardSetCount();
            for (var i = 0; i < cardSetCount; i++)
            {
                var cardSetInfo = GetCardSetInfoByIndex(i);
                if (cardSetInfo.RewardForCollectAllStat == CardsRewardStat.CanReceive)
                {
                    listFinishCardSet.Add(cardSetInfo.SetId);
                }
            }

            if (listFinishCardSet.Count > 0)
            {
                CheckAndClaimArtSetCompleteReward(listFinishCardSet, finishCallback, "UnusualClaim");
            }
        }

        public float GetSeasonFinishCountDown()
        {
            return _albumModel.GetSeasonCountDown();
        }

        public bool IsNewSeasonStart()
        {
            return _albumModel.GetModelData().IsFirstIn;
        }

        public void RestSetIsNewSeason()
        {
            _albumModel.GetModelData().IsFirstIn = false;
        }


        public int GetCardSetCount()
        {
            return _albumModel.GetCardSetCount();
        }
        
        public uint GetOwnedCardCount()
        {
            return _albumModel.GetModelData().CardsCountOwned;
        }
        
        public uint GetTotalCardCount()
        {
            return  _albumModel.GetModelData().CardsCountTotal;
        }

        public Reward GetAllSetCompleteReward()
        {
            return _albumModel.GetModelData().RewardForCollectAll;
        }

        public uint GetLuckySpinCount()
        {
            return _albumModel.GetModelData().CardLuckySpinCount - _albumModel.GetModelData().UsedCardLuckySpinCount;
        }
        
        public uint GetUsedLuckySpinCount()
        {
            return _albumModel.GetModelData().UsedCardLuckySpinCount;
        }

        public CardSet GetCardSetInfoByIndex(int index)
        {
            return _albumModel.GetCardSetInfoByIndex(index);
        }
        
        public CardSet GetCardSetInfo(uint cardSetId)
        {
            return _albumModel.GetCardSetInfo(cardSetId);
        }


        public Card GetCardInfoByCardId(uint cardId)
        {
            var cardSetId = cardId / 100;
            var cardSet = _albumModel.GetCardSetInfo((uint)cardSetId);

            if (cardSet != null)
            {
                for (var i = 0; i < cardSet.Cards.count; i++)
                {
                    if (cardSet.Cards[i].CardId == cardId)
                    {
                        return cardSet.Cards[i];
                    }
                }
            }

            return null;
        }

        public bool IsUnlocked()
        {
            return _albumModel.IsUnlocked();
        }
        
        public bool IsOpen()
        {
            return _albumModel.IsOpen();
        }
 
        public bool IsAlbumSeasonOver()
        {
            return _albumModel.GetSeasonCountDown() <= 0;
        }

        public uint GetUnlockLevel()
        {
            if (_albumModel.GetModelData() != null)
            {
                return _albumModel.GetUnlockLevel();
            }

            return 1000000;
        }
 
        public RepeatedField<SGetCardAlbumInfo.Types.LuckyChallengeConfig> GetLuckyChallengeConfig()
        {
            return _albumModel.GetModelData().LuckyChallengeConfig;
        }
        
        public uint GetLuckyChallengeProgress()
        {
            return _albumModel.GetModelData().LuckyChallengeProgress;
        }
        
        public uint GetLuckyChallengeMaxProgress()
        {
            var luckyChallengeConfig =  _albumModel.GetModelData().LuckyChallengeConfig;
            return luckyChallengeConfig[luckyChallengeConfig.Count - 1].IndependentLuckyCardCount;
        }

        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            var dropData = GetSystemData<CollectCardRewardsWhenSpin>(evt.systemContent, "CollectCardRewardsWhenSpin");
            if (dropData != null)
            {
                var updateInfo = UpdateAlbumCardInfo(dropData.Reward);

                if (updateInfo.Count > 0 && IsOpen())
                {
                    //默认每次Spin只会掉落一个卡包
                    EventBus.Dispatch(new EventShowSpinDropCardInfo(updateInfo[0]));
                }
                
             //   EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(CardPackOpenPopup), updateInfo[0], "Machine")));
            }
        }

        protected CardUpdateInfo UpdateAlbumCardInfo(Item item)
        {
            var cardPackage = item.CardPackage;
            CardUpdateInfo cardUpdateInfo = new CardUpdateInfo();
            cardUpdateInfo.luckyChallengeUpdateInfo = new LuckyChallengeUpdateInfo();

            cardUpdateInfo.luckySpinNewAddCount = cardPackage.LuckySpinNewAddCount;
            cardUpdateInfo.luckyChallengeUpdateInfo.newProgress = cardPackage.LuckyChallengeNewProgress;
            cardUpdateInfo.luckyChallengeUpdateInfo.oldProgress = _albumModel.GetModelData().LuckyChallengeProgress;
            cardUpdateInfo.luckyChallengeUpdateInfo.rewardLuckySpinCount = cardPackage.LuckySpinNewAddCount;
            cardUpdateInfo.currentLuckySpinCount = cardPackage.CardLuckySpinCount;
            cardUpdateInfo.packageConfig = cardPackage.PackageConfig;
            cardUpdateInfo.packageType = cardPackage.PackageType;
 
            //更新modelData
            _albumModel.GetModelData().LuckyChallengeProgress = cardPackage.LuckyChallengeNewProgress;
            _albumModel.GetModelData().CardLuckySpinCount = cardPackage.CardLuckySpinCount;


            var listCardSet = new List<uint>();

            for (var c = 0; c < cardPackage.CardsIncludes.count; c++)
            {
                var cardId = cardPackage.CardsIncludes[c].CardId;
                var cardSetId = cardPackage.CardsIncludes[c].CardSetId;
                var cardInfo = GetCardInfoByCardId( cardId);

                if (cardInfo != null)
                {
                    cardUpdateInfo.cardAcquired.Add(cardInfo);

                    if (!listCardSet.Contains(cardSetId))
                        listCardSet.Add(cardSetId);

                    cardInfo.Count++;

                    if (cardInfo.Count == 1)
                    {
                        cardInfo.IsNew = true;

                        _albumModel.GetModelData().CardsCountOwned++;
                    }
                }
                else
                {
                    XDebug.LogError("Invalid CardId: " + cardId);
                }
            }

            var cardAcquiredCount = cardUpdateInfo.cardAcquired.Count;
            for (var i = 0; i < cardAcquiredCount; i++)
            {
                for (var j = i + 1; j < cardAcquiredCount; j++)
                {
                    var a = cardUpdateInfo.cardAcquired[i];
                    var b = cardUpdateInfo.cardAcquired[j];

                    if (CompareCard(a, b) > 0)
                    {
                        cardUpdateInfo.cardAcquired[i] = b;
                        cardUpdateInfo.cardAcquired[j] = a;
                    }
                }
            }
 
            //从发生卡牌变化的卡册中，查找未收集完成的卡册
            for (int s = 0; s < listCardSet.Count; s++)
            {
                var cardSetInfo = GetCardSetInfo(listCardSet[s]);

                if (cardSetInfo.RewardForCollectAllStat == CardsRewardStat.CannotReceive)
                {
                    bool setFinished = true;

                    uint cardsCountOwned = 0;
                    
                    foreach (var card in cardSetInfo.Cards)
                    {
                        if (card.Count > 0)
                        {
                            cardsCountOwned++;
                        }
                        else
                        {
                            setFinished = false;
                        }
                    }

                    cardSetInfo.CardsCountOwned = cardsCountOwned;
                   
                    if (setFinished)
                    {
                        cardSetInfo.RewardForCollectAllStat = CardsRewardStat.CanReceive;
                        cardUpdateInfo.finishCardSetIds.Add(listCardSet[s]);
                    }
                    
                }
            }

            return cardUpdateInfo;
        }

        protected int CompareCard(Card a, Card b)
        {
            if (a.Type != b.Type)
            {
                return -((int) a.Type - (int) b.Type);
            }
            
            if(a.Star != b.Star)
                return -((int) a.Star - (int) b.Star);
            
            return -((int) a.CardId - (int) b.CardId);
        }
        
        /// <summary>
        /// 获得卡册之后从卡包中同步卡册的数据
        /// </summary>
        /// <param name="itemLists"></param>
        /// <returns></returns>
        protected List<CardUpdateInfo> UpdateAlbumCardInfo(RepeatedField<Item> itemLists)
        {
            var items = XItemUtility.GetItems(itemLists, Item.Types.Type.CardPackage);

            var cardUpdateInfoList = new List<CardUpdateInfo>();
           
            if (items.Count > 0)
            {
                for (var i = 0; i < items.Count; i++)
                {
                    CardUpdateInfo cardUpdateInfo = UpdateAlbumCardInfo(items[i]);
                    cardUpdateInfoList.Add(cardUpdateInfo);
                }   
            }
            return cardUpdateInfoList;
        }
        
        protected List<CardUpdateInfo> UpdateAlbumCardInfo(Reward reward)
        {
            return UpdateAlbumCardInfo(reward.Items);
        }

        /// <summary>
        /// 获取LuckySpin的信息
        /// </summary>
        /// <param name="callBack"></param>
        public async Task<SGetCardLuckySpinInfo> GetLuckySpinInfo()
        {
            CGetCardLuckySpinInfo cGetCardLuckySpinInfo = new CGetCardLuckySpinInfo();
            var send =
                await APIManagerGameModule.Instance.SendAsync<CGetCardLuckySpinInfo, SGetCardLuckySpinInfo>(
                    cGetCardLuckySpinInfo);

            if (send.ErrorCode == ErrorCode.Success)
            {
                _albumModel.GetModelData().CardLuckySpinCount = send.Response.CardLuckySpinCount;
                _albumModel.GetModelData().UsedCardLuckySpinCount = send.Response.UsedCardLuckySpinCount;

                return send.Response;
            }

            return null;
        }

        /// <summary>
        /// Spin一次LuckySpin
        /// </summary>
        /// <param name="callBack"></param>
        public async void DoLuckySpin(Action<SCardLuckySpin> callBack)
        {
            CCardLuckySpin cardLuckySpin = new CCardLuckySpin();
            var send =
                await APIManagerGameModule.Instance.SendAsync<CCardLuckySpin, SCardLuckySpin>(
                    cardLuckySpin);

            if (send.ErrorCode == ErrorCode.Success)
            {
                _albumModel.GetModelData().UsedCardLuckySpinCount = send.Response.UsedCardLuckySpinCount;

                callBack.Invoke(send.Response);
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(send.ErrorInfo));
                callBack.Invoke(null);
            }
        }
        
        /// <summary>
        /// 一次转完所有的LuckySpin的奖励
        /// </summary>
        /// <param name="callBack"></param>
        public async void DoLuckySpinAll(Action<SCardLuckySpinAll> callBack)
        {
            CCardLuckySpinAll cardLuckySpin = new CCardLuckySpinAll();
            var send =
                await APIManagerGameModule.Instance.SendAsync<CCardLuckySpinAll, SCardLuckySpinAll>(
                    cardLuckySpin);

            if (send.ErrorCode == ErrorCode.Success)
            {
                _albumModel.GetModelData().UsedCardLuckySpinCount = send.Response.UsedCardLuckySpinCount;

                callBack.Invoke(send.Response);
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(send.ErrorInfo));
                callBack.Invoke(null);
            }
        }

        /// <summary>
        /// 从服务器领取玩家获得的所有LuckySpin的奖励
        /// </summary>
        /// <returns></returns>
        public async Task<SCollectLuckySpinRewards> ClaimLuckySpinReward()
        {
            var cCollectLuckySpinRewards = new CCollectLuckySpinRewards();
            var send =
                await APIManagerGameModule.Instance.SendAsync<CCollectLuckySpinRewards, SCollectLuckySpinRewards>(
                    cCollectLuckySpinRewards);

            if (send.ErrorCode == ErrorCode.Success && IsOpen())
            {
                EventBus.Dispatch(new EventUserProfileUpdate(send.Response.UserProfile));
                return send.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(send.ErrorInfo));
            }

            return null;
        }

        public async Task<SGetCardRecycleGameInfo> GetCardRecycleInfo()
        {
            CGetCardRecycleGameInfo cGetCardRecycleGameInfo = new CGetCardRecycleGameInfo();
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetCardRecycleGameInfo, SGetCardRecycleGameInfo>(cGetCardRecycleGameInfo);
            
            if (handler.ErrorCode == ErrorCode.Success)
            {
                return handler.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(handler.ErrorInfo));
                return null;
            }
        }

        public RepeatedField<SGetCardAlbumInfo.Types.CardProbobilityDisplayItem> GetBetTipDisplayInfo()
        {
            if (_albumModel.IsInitialized())
            {
                return _albumModel.GetModelData().CardProbabilityDisplayList;
            }

            return null;
        }
        
        /// <summary>
        /// 请求服务器做卡牌回收
        /// </summary>
        /// <param name="selectCardInfo"></param>
        /// <param name="doRecycleFinished"></param>
        public async void DoCardRecycle(Dictionary<uint,int> selectCardInfo, Action<SCardRecycleGameSpin> doRecycleFinished)
        {
 
            CCardRecycleGameSpin cCardRecycleGameSpin = new CCardRecycleGameSpin();

            var cardIds = selectCardInfo.Keys.ToList();
            for (var i = 0; i < cardIds.Count; i++)
            {
                cCardRecycleGameSpin.CardIds.Add(cardIds[i]);
            }
            
            var counts = selectCardInfo.Values.ToList();
            
            for(var i = 0; i < counts.Count; i++)
                cCardRecycleGameSpin.CardCounts.Add((uint)counts[i]);
             
            var handler = await APIManagerGameModule.Instance.SendAsync<CCardRecycleGameSpin, SCardRecycleGameSpin>(cCardRecycleGameSpin);
            
            
            if (handler.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(handler.Response.UserProfile));

                //卡牌回收后，本地卡牌数量要做数量上的修改
                foreach (var recycleCard in selectCardInfo)
                {
                    var cardInfo = GetCardInfoByCardId(recycleCard.Key);
                    
                    cardInfo.Count -= (uint)recycleCard.Value;
                }

                _albumModel.GetModelData().RecycleGameCountDown = handler.Response.GameCountDown;
                _albumModel.lastUpdateRecycleCountDownData = Time.realtimeSinceStartup;
                
                doRecycleFinished.Invoke(handler.Response);
                EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
            }
            else
            {
                if (IsOpen())
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(handler.ErrorInfo));
                }

                doRecycleFinished.Invoke(null);
            }
        }
        
        //领取卡册收集完成的奖励
        public async void ClaimCardSetReward(List<uint> cardSetIds, Action<SAcceptCardReward> finishCallback)
        {
            CAcceptCardReward cAcceptCardReward = new CAcceptCardReward();

            for(var i = 0; i < cardSetIds.Count; i++)
                cAcceptCardReward.SetIds.Add(cardSetIds[i]);

            var handler =
                await APIManagerGameModule.Instance.SendAsync<CAcceptCardReward, SAcceptCardReward>(cAcceptCardReward);

            if (handler.ErrorCode == ErrorCode.Success)
            {
                finishCallback?.Invoke(handler.Response);
                EventBus.Dispatch(new EventUserProfileUpdate(handler.Response.UserProfile));
            }
            else
            {
                if (IsOpen())
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(handler.ErrorInfo));
                }
                finishCallback?.Invoke(null);
            }
        }

        /// <summary>
        /// 重置卡牌上的New标签
        /// </summary>
        /// <param name="cardSetId"></param>
        public async void ResetNewTag(uint cardSetId)
        {
            CMarkViewCardSet cMarkViewCardSet = new CMarkViewCardSet();
            cMarkViewCardSet.SetId = cardSetId;
            
            var cardSet = GetCardSetInfo(cardSetId);
            for (var i = 0; i < cardSet.Cards.count; i++)
            {
                cardSet.Cards[i].IsNew = false;
            }
            
            EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
            
            var send =
                await APIManagerGameModule.Instance.SendAsync<CMarkViewCardSet, SMarkViewCardSet>(cMarkViewCardSet);
        }
        
        /// <summary>
        /// 获取玩家卡牌历史记录
        /// </summary>
        /// <returns></returns>
        public async Task<SGetCardGotRecords> GetCardHistoryInfo()
        {
            CGetCardGotRecords cGetCardGotRecords = new CGetCardGotRecords();
            var send = await APIManagerGameModule.Instance.SendAsync<CGetCardGotRecords, SGetCardGotRecords>(cGetCardGotRecords);
            if (send.ErrorCode == ErrorCode.Success)
            {
                return send.Response;
            }

            return null;
        }

        public void SettleCardPackages(RepeatedField<Item> cardPackageItems, Action finishCallback, string source)
        {
            var cardUpdateInfos = UpdateAlbumCardInfo(cardPackageItems);
            EventBus.Dispatch(
                new EventShowPopup(new PopupArgs(typeof(SummaryOfAlbumsPopup), cardUpdateInfos, ()=>
                {
                    //Merge luckyChallengeUpdateInfo and finishCardSetIds;
                    
                    for (var i = 1; i < cardUpdateInfos.Count; i++)
                    {
                        if (cardUpdateInfos[i].luckyChallengeUpdateInfo.oldProgress <
                            cardUpdateInfos[0].luckyChallengeUpdateInfo.oldProgress)
                        {
                            cardUpdateInfos[0].luckyChallengeUpdateInfo.oldProgress =
                                cardUpdateInfos[i].luckyChallengeUpdateInfo.oldProgress;
                        }
                        
                        if (cardUpdateInfos[i].luckyChallengeUpdateInfo.newProgress >
                            cardUpdateInfos[0].luckyChallengeUpdateInfo.newProgress)
                        {
                            cardUpdateInfos[0].luckyChallengeUpdateInfo.newProgress =
                                cardUpdateInfos[i].luckyChallengeUpdateInfo.newProgress;
                        }

                        cardUpdateInfos[0].luckyChallengeUpdateInfo.rewardLuckySpinCount +=
                            cardUpdateInfos[i].luckyChallengeUpdateInfo.rewardLuckySpinCount;
                        
                        for (var c = 0; c < cardUpdateInfos[i].finishCardSetIds.Count; c++)
                        {
                            if (!cardUpdateInfos[0].finishCardSetIds.Contains(cardUpdateInfos[i].finishCardSetIds[c]))
                            {
                                cardUpdateInfos[0].finishCardSetIds.Add(cardUpdateInfos[i].finishCardSetIds[c]);
                            }
                        }
                    }
                    
                    CheckAndClaimLuckyChallenge(cardUpdateInfos[0], finishCallback, source);
                })));
            
        }
        
        public void SettleCardPackage(Item cardPackageItem, Action finishCallback, string source)
        {
            if (cardPackageItem.CardPackage != null && cardPackageItem.CardPackage.CardsIncludes.count == 0)
            {
               XDebug.LogError($"Invalid-CardPackage:PackageIsEmpty:PackageId{cardPackageItem.CardPackage.PackageId},PackageConfigId:{cardPackageItem.CardPackage.PackageConfig.Id}"); 
               finishCallback.Invoke();
               return;
            }
            
            var cardUpdateInfo = UpdateAlbumCardInfo(cardPackageItem);
            EventBus.Dispatch(
                new EventShowPopup(new PopupArgs(typeof(CardPackOpenPopup), cardUpdateInfo, ()=>
                {
                    CheckAndClaimLuckyChallenge(cardUpdateInfo, finishCallback, source);
                })));
        }

        public void CheckAndClaimLuckyChallenge(CardUpdateInfo cardUpdateInfo, Action finishCallback, string source)
        {
            if (cardUpdateInfo.luckyChallengeUpdateInfo.newProgress >
                cardUpdateInfo.luckyChallengeUpdateInfo.oldProgress)
            {
                if (IsOpen())
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LuckyChallengePopup),
                        cardUpdateInfo.luckyChallengeUpdateInfo,
                        () => { CheckAndClaimArtSetCompleteReward(cardUpdateInfo, finishCallback, source); }, source)));
                }
                else
                {
                    finishCallback?.Invoke();
                }
            }
            else
            {
                CheckAndClaimArtSetCompleteReward(cardUpdateInfo, finishCallback, source);
            }
        }

        public void CheckAndClaimArtSetCompleteReward(List<uint> finishCardSetIds, Action finishCallback,
            string source)
        {
            ClaimCardSetReward(finishCardSetIds, async (sAcceptCardReward) =>
            {
                if (sAcceptCardReward == null)
                {
                    finishCallback?.Invoke();
                    return;
                }

                for (var i = 0; i < sAcceptCardReward.ReceivedCardSetReward.Count; i++)
                {
                    var cardSetInfo = GetCardSetInfo(sAcceptCardReward.ReceivedCardSetReward[i].SetId);
                    cardSetInfo.RewardForCollectAllStat = CardsRewardStat.Received;
                    
                    var waitClaimFinish = new TaskCompletionSource<bool>();
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(CardSetRewardPopup),
                        sAcceptCardReward.ReceivedCardSetReward[i],
                        () => { waitClaimFinish.SetResult(true); }, source)));

                    await waitClaimFinish.Task;
                }

                if (sAcceptCardReward.ReceivedCollectAllReward != null && sAcceptCardReward.ReceivedCollectAllReward.count > 0)
                {
                    EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(CardSetAllCompleteRewardPopup),
                        sAcceptCardReward.ReceivedCollectAllReward,
                        finishCallback, source)));
                }
                else
                {
                    finishCallback?.Invoke();
                }
            });
        }

        public void CheckAndClaimArtSetCompleteReward(CardUpdateInfo cardUpdateInfo, Action finishCallback,
            string source)
        {
            if (cardUpdateInfo.finishCardSetIds.Count > 0)
            {
                CheckAndClaimArtSetCompleteReward(cardUpdateInfo.finishCardSetIds, finishCallback, source);
            }
            else
            {
                finishCallback?.Invoke();
            }
        }

        public List<Card> GetExchangeableCardInfo()
        {
            var cardSetCount = GetCardSetCount();

            var exchangeableCard = new List<Card>();

            for (int i = 0; i < cardSetCount; i++)
            {
                var cardSetInfo = GetCardSetInfoByIndex(i);
                for (int c = 0; c < cardSetInfo.Cards.Count; c++)
                {
                    if (cardSetInfo.Cards[c].Count > 1)
                    {
                        exchangeableCard.Add(cardSetInfo.Cards[c]);
                    }
                }
            }

            return exchangeableCard;
        }

        public bool HasNewCardInCardSet(int setIndex)
        {
            var cardSetInfo = GetCardSetInfoByIndex(setIndex);
            for (var i = 0; i < cardSetInfo.Cards.count; i++)
            {
                if (cardSetInfo.Cards[i].IsNew)
                    return true;
            }

            return false;
        }
        public bool HasNewCard()
        {
            var cardSetCount = GetCardSetCount();
            for (var i = 0; i < cardSetCount; i++)
            {
                if (HasNewCardInCardSet(i))
                {
                    return true;
                }
            }

            return false;
        }

        public float GetFortuneExchangeCountDown()
        {
            return _albumModel.GetModelData().RecycleGameCountDown - (Time.realtimeSinceStartup - _albumModel.lastUpdateRecycleCountDownData);
        }

        public bool FortuneExchangeCanPlay()
        {
            if (GetFortuneExchangeCountDown() > 0)
                return false;

            var cardInfo = GetExchangeableCardInfo();

            if (cardInfo.Count > 0)
            {
                var leastPowerCanPlay = 8;

                uint currentPower = 0;
                for (var i = 0; i < cardInfo.Count; i++)
                {
                    currentPower += cardInfo[i].RecycleValue * (cardInfo[i].Count - 1);
                }

                if (leastPowerCanPlay <= currentPower)
                    return true;
            }
            
            return false;
        }

        public string GetAlbumSpriteAtlasName(int seasonId = -1)
        {
            if (seasonId < 0)
            {
                seasonId = GetSeasonId();
                return $"AlbumCardSetAtlasSeason{seasonId}";
            }
            
            return $"AlbumCardSetAtlasSeason{seasonId}";
        }

        public string GetCurrentSeasonAssetAddress(string address)
        {
            return address;
        }
        
        public uint GetGuideStep()
        {
            return _albumModel.GetModelData().BeginnersGuideStep;
        }

        public async void IncreaseGuideStep(Action<SIncBeginnersGuideStep> finishCallback)
        {
            var send = new CIncBeginnersGuideStep();
            
            _albumModel.GetModelData().BeginnersGuideStep++;
            
            var receive =
                await APIManagerGameModule.Instance.SendAsync<CIncBeginnersGuideStep, SIncBeginnersGuideStep>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                finishCallback?.Invoke(receive.Response);
                _albumModel.GetModelData().BeginnersGuideStep = receive.Response.BeginnersGuideStep;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(receive.ErrorInfo));
                finishCallback?.Invoke(null);
            }
        }

        public int GetLobbyAlbumEntranceRedDotNumber()
        {
            if (_albumModel != null && _albumModel.IsInitialized() && _albumModel.IsUnlocked() && _albumModel.IsOpen())
            {
                var count = GetLuckySpinCount() > 0 ? 1 : 0;
                count += FortuneExchangeCanPlay() ? 1 : 0;
                count += HasNewCard() ? 1 : 0;
                count += Client.Get<AmazingHatController>().IsOpen() ? 1 : 0;
                return count;
            }

            return 0;
        }

        public int GetSeasonId()
        {
            if (_albumModel != null && _albumModel.IsInitialized())
            {
                var modelData = _albumModel.GetModelData();
                if (modelData != null)
                {
                    return (int) modelData.SeasonId;
                }
            }
            
            return -1;
        }
        
        public void OnEventOnLobbyCreated(EventOnLobbyCreated evt)
        {
            
        }
    }
}