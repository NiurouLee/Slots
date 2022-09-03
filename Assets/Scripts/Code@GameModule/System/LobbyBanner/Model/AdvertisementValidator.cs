// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/03/14:25
// Ver : 1.0.0
// Description : BannerValidator.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class AdvertisementValidator
    {
        public static bool IsValid(Advertisement advertisement, ulong serverTimeStampNow, string checkPoint = "")
        {
            if (!IsTimeValid(advertisement, serverTimeStampNow))
                return false;

            switch (advertisement.Type)
            {
                case AdvertisementType.Deal:
                    return CheckDealPurchaseValid(advertisement, checkPoint);
                case AdvertisementType.Quickmachine:
                    return CheckMachineValid(advertisement, checkPoint);
                case AdvertisementType.Quicksystem:
                    return CheckSystemValid(advertisement, checkPoint);
            }

            return false;
        }

        static bool IsTimeValid(Advertisement advertisement, ulong serverTimeStampNow)
        {
            if (advertisement.StartTimestamp <= serverTimeStampNow &&
                advertisement.EndTimestamp > serverTimeStampNow)
            {
                return true;
            }

            return false;
        }

        static bool CheckSystemValid(Advertisement advertisement, string checkPoint = "")
        {
            if (advertisement.Jump == "Quest")
            {
                var newBieQuestController = Client.Get<NewBieQuestController>();
                if (newBieQuestController.IsLocked() || newBieQuestController.IsQuestFinished() ||
                    newBieQuestController.IsTimeOut())
                {
                    return false;
                }
                return true;
            }

            if (advertisement.Jump == "SeasonQuest")
            {
                var seasonQuestController = Client.Get<SeasonQuestController>();
                if (seasonQuestController.IsLocked() ||
                    seasonQuestController.IsTimeOut())
                {
                    return false;
                }
                return true;
            }

            if (advertisement.Jump == "DailyMission")
            {
                var dailyMissionController = Client.Get<DailyMissionController>();
                if (dailyMissionController.IsLocked)
                {
                    return false;
                }
                return true;
            }

            if (advertisement.Jump == "SeasonPass")
            {
                var seasonPassController = Client.Get<SeasonPassController>();
                if (seasonPassController.IsLocked)
                {
                    return false;
                }
                return true;
            }

            if (advertisement.Jump == "GoldCoupon")
            {
                var activityController = Client.Get<ActivityController>();
                var goldenCouponActivity = activityController.GetDefaultActivity(ActivityType.GoldenCoupon) as Activity_BonusCoupon;
                return goldenCouponActivity != null && goldenCouponActivity.GetLinkedCoupon() != null;
            }

            if (advertisement.Jump == "ADTask")
            {
                var adTaskInfo = AdController.Instance.GetAdTaskInfo();
                return adTaskInfo != null;
            }

            if (advertisement.Jump == "CrazeSmash")
            {
                var controller = Client.Get<CrazeSmashController>();
                return controller.available 
                    && (!controller.goldGameFinish || !controller.silverGameFinish);
            }

            if (advertisement.Jump == "CoinDash")
            {
                var controller = Client.Get<CoinDashController>();
                return controller.IsOpen();
            }

            if (advertisement.Jump == "Activity")
            {
                var activityController = Client.Get<ActivityController>();

                ActivityBase activity = activityController.GetDefaultActivity(advertisement.Jump2);;
                return activity != null && activity.IsValid();
                
                // switch (advertisement.Jump2)
                // {
                //     case "ValentinesDay2022":
                //         activity = activityController.GetDefaultActivity(ActivityType.Valentine2022);
                //         break;
                //     case "TreasureRaid":
                //         activity = activityController.GetDefaultActivity(ActivityType.TreasureRaid);
                //         break;
                // }
                //return activity != null && activity.IsValid();
            }

            return true;
        }

        static bool CheckMachineValid(Advertisement advertisement, string checkPoint = "")
        {
            return Client.Get<MachineLogicController>().IsMachineExist(advertisement.Jump);
        }

        static bool CheckDealPurchaseValid(Advertisement advertisement, string checkPoint = "")
        {
            if (advertisement.Jump == "PiggyBonus")
            {
                return !checkPoint.Contains("Lobby");
            }
            return true;
        }

        static bool CheckPurchaseValid(Advertisement advertisement, string checkPoint = "")
        {
            return true;
        }

    }
}