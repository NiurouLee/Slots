#if !UNITY_EDITOR
#define USE_AD
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using UnityEngine;

namespace GameModule
{
    public class ADSController : LogicController
    {
        public ADSController(Client client) : base(client) { }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            // SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget, 0);
        }

        protected async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt,
            IEventHandlerScheduler eventHandlerScheduler)
        {
            if (!ShouldShowRV(eAdReward.SlotRV))
            {
                handleEndAction.Invoke();
                return;
            }
            var adsWidget = await View.CreateView<ADSButtonInSlotWidget>();
            adsWidget.Set();
            evt.viewController.AddSystemWidget(adsWidget);
            handleEndAction.Invoke();
        }

        public static string GetTimeLeft(float secondsRemain, out TimeSpan timeRemains)
        {
            timeRemains = TimeSpan.FromSeconds(Math.Max(0, secondsRemain));
            var days = timeRemains.Days;
            if (days == 1) { return days + " DAY"; }
            else if (days > 1) { return days + " DAYS"; }
            return timeRemains.ToString(@"hh\:mm\:ss");
        }

        public static ulong GetRewardCoin(ulong baseCount)
        {
            var userController = Client.Get<UserController>();
            var level = userController.GetUserLevel();
            var result = ((level / 10) * 10 + 5) * baseCount;
            return result;
        }

        public static ulong GetFirstBonusCount(eAdReward _adPlaceID)
        {
            var bonusList = GetADBonus(_adPlaceID);
            if (bonusList == null || bonusList.Count == 0) { return 0; }
            var bonus = bonusList[0];
            return (ulong)bonus.itemCnt1;
        }

        public static List<AdBonus> GetADBonus(eAdReward adPlaceID)
        {
//#if USE_AD
            var bonusIDs = AdController.Instance.adModel.GetRewardedVideoBonus(adPlaceID);
            if (bonusIDs == null || bonusIDs.Count == 0) { return null; }

            var adBonusList = new List<AdBonus>();
            foreach (var bonusID in bonusIDs)
            {
                var bonus = AdController.Instance.adModel.GetAdBonus(bonusID);
                if (bonus != null) { adBonusList.Add(bonus); }
            }
            return adBonusList;
//#else
            return null;
//#endif
        }

        public static int GetArg1(eAdReward adPlaceID)
        {
#if USE_AD
            return AdController.Instance.adModel.GetArg1(adPlaceID);
#else
            return 0;
#endif
        }

        public static List<int> GetArg2(eAdReward adPlaceID)
        {
#if USE_AD
            return AdController.Instance.adModel.GetArg2(adPlaceID);
#else
            return null;
#endif
        }

        public static long GetRewardedVideoCDinSeconds(eAdReward adPlaceID)
        {
#if USE_AD
            return AdController.Instance.adModel.GetRewardedVideoCDinSeconds(adPlaceID);
#else
            return 0;
#endif
        }

        public static bool ShouldShowRV(eAdReward adPlaceID, bool withBI = false)
        {
#if USE_AD
            return AdController.Instance.ShouldShowRV(adPlaceID, withBI);
#else
            return false;
#endif
        }

        public static bool TryShowRewardedVideo(eAdReward adPlaceID, Action<bool, string> onFinish)
        {
#if USE_AD
          

            return AdController.Instance.TryShowRewardedVideo(adPlaceID, onFinish);
#else
            return false;
#endif
        }

        public static bool TryShowRewardedVideoWithTransform(eAdReward adPlaceID, ulong rewardCoin, ulong flyCoin, string source, Transform from, Action<bool, string> onFinish = null)
        {

#if USE_AD
            return TryShowRewardedVideo(adPlaceID, async (b, s) =>
                {
                    if (b)
                    {
                        XDebug.Log("111111111111:watch ad success");
                        await DoClaimAdRewardWithTransform(adPlaceID, rewardCoin, flyCoin, source, from);
                    }
                    else
                    {
                        XDebug.LogError($"111111111:AdController.Instance.TryShowRewardedVideo error:{s}");
                    }
                    onFinish?.Invoke(b, s);
                });
#else
            onFinish?.Invoke(true, null);
            return false;
#endif
        }

        public static bool TryShowRewardedVideoWithCollectRewardPopup(eAdReward adPlaceID, ulong rewardCoin, ulong flyCoin, string source, Action<bool> onFinish = null)
        {
#if USE_AD
            return TryShowRewardedVideo(adPlaceID, async (b, s) =>
                {
                    if (b)
                    {
                        XDebug.Log("111111111111:watch ad success");
                        await DoClaimAdRewardWithCollectPopup(adPlaceID, rewardCoin, flyCoin, source, onFinish);
                    }
                    else
                    {
                        XDebug.LogError($"111111111:AdController.Instance.TryShowRewardedVideo error:{s}");
                        onFinish?.Invoke(false);
                    }
                });
#else
            onFinish?.Invoke(true);
            return false;
#endif
        }

        public static async void ShowGetMorePopup(eAdReward adPlaceId, ulong coin, Action onClick)
        {
            if (ADSController.ShouldShowRV(adPlaceId))
            {
                var popup = await PopupStack.ShowPopup<UIADSGetMorePopup>();
                popup.Set(coin,
                () =>
                {
                    onClick?.Invoke();
                    PopupStack.ClosePopup<UIADSGetMorePopup>();
                });
            }
        }

        private static async Task DoClaimAdRewardWithTransform(eAdReward adPlaceID, ulong rewardCoin, ulong flyCoin, string source, Transform from)
        {
            var result = await DoClaimAdReward_Coin(adPlaceID, rewardCoin);
            if (result != null && result.ErrorCode == 0)
            {
                XDebug.Log($"111111111:DoClaimAdRewardWithTransform, to add coin:{rewardCoin}");
                EventBus.Dispatch(new EventUserProfileUpdate(result.Response.UserProfile));
                XDebug.Log($"111111111:DoClaimAdRewardWithTransform, userprofile coin:{result.Response.UserProfile.Coins}");
                await XUtility.FlyCoins(from, new EventBalanceUpdate(flyCoin, source));
            }
            else
            {
                XDebug.LogError($"1111111111 receive SCliamAdReward with error, code={result.ErrorCode}, info={result.ErrorInfo}");
            }
        }

        private static async Task DoClaimAdRewardWithCollectPopup(eAdReward adPlaceID, ulong rewardCoin, ulong flyCoin, string source, Action<bool> onFinish = null)
        {
            var result = await DoClaimAdReward_Coin(adPlaceID, rewardCoin);
            if (result != null && result.ErrorCode == 0)
            {
                XDebug.Log($"1111111111 receive SCliamAdReward success");
                var popup = await PopupStack.ShowPopup<UIADSCollectRewardPopup>();
                popup.Set(rewardCoin, async () =>
                {
                    XDebug.Log($"111111111:DoClaimAdRewardWithCollectPopup, to add coin:{rewardCoin}");
                    EventBus.Dispatch(new EventUserProfileUpdate(result.Response.UserProfile));
                    XDebug.Log($"111111111:DoClaimAdRewardWithCollectPopup, userprofile coin:{result.Response.UserProfile.Coins}");
                    await popup.FlyCoins(flyCoin, source);
                    PopupStack.ClosePopup(popup);
                    onFinish?.Invoke(true);
                });
            }
            else
            {
                XDebug.LogError($"1111111111 receive SCliamAdReward with error, code={result.ErrorCode}, info={result.ErrorInfo}");
                onFinish?.Invoke(false);
            }
        }

        private static async Task<APIFixHotAsyncHandler<SClaimAdReward>> DoClaimAdReward_Coin(eAdReward adPlaceID, ulong coin)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventAdsCollect, ("placeId", adPlaceID.ToString()));
            var c = new CClaimAdReward() { PlaceId = (ulong)adPlaceID };
            c.ItemIdList.Add(300);
            c.ItemCountList.Add(coin);
            return await APIManagerGameModule.Instance.SendAsync<CClaimAdReward, SClaimAdReward>(c);
        }
    }
}
