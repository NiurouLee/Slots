//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-26 16:37
//  Ver : 1.0.0
//  Description : MissionFactory.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using MissionType = DragonU3DSDK.Network.API.ILProtocol.Mission.Types.Type;
namespace GameModule
{
    public class MissionFactory
    {
        private static Dictionary<MissionType, Type> dictMissions;
        
        static MissionFactory()
        {
            dictMissions = new Dictionary<MissionType, Type>();
            RegisterAllMissions();
        }
        
        static private void RegisterAllMissions()
        {
            dictMissions.Add(MissionType.WinXTimesInARow, typeof(MissionContinuousWinTime));
            dictMissions.Add(MissionType.SpinXTimes, typeof(MissionSpinTime));
            dictMissions.Add(MissionType.WinXTimes, typeof(MissionWinTime));
            dictMissions.Add(MissionType.WinXCoinsInASingleSpinYTimes, typeof(MissionWinXCoinsInASingleSpinYTimes));
            dictMissions.Add(MissionType.BetATotalOfXCoins, typeof(MissionBet));
            dictMissions.Add(MissionType.WinATotalOfXCoins, typeof(MissionWin));
            dictMissions.Add(MissionType.WinXCoinsInYSpins, typeof(MissionWinXCoinsInYSpins));
            dictMissions.Add(MissionType.WinXCoinsInYFreeSpins, typeof(MissionWinInFreeSpin));
            dictMissions.Add(MissionType.GetBigWinXTimes, typeof(MissionBigWin));
            dictMissions.Add(MissionType.GetHugeWinXTimes, typeof(MissionHugeWin));
            dictMissions.Add(MissionType.Get5OfAKindXTimes, typeof(MissionFiveOfAKind));
            dictMissions.Add(MissionType.LevelUpXTimes, typeof(MissionLevelUp));
            dictMissions.Add(MissionType.GetLockItLinkXTimes, typeof(MissionGetLockLinkXTimes));
            dictMissions.Add(MissionType.GetFreeGameXTimes, typeof(MissionGetFreeGameXTimes));
            dictMissions.Add(MissionType.WinXTimesWithMinimumBetOfY, typeof(MissionWinXTimesWithMiniBet));
            dictMissions.Add(MissionType.BetXCoinsWithMinimumBetOfY, typeof(MissionBetXCoinsWithMiniBet));
            dictMissions.Add(MissionType.WinXCoinsInLinkFeature, typeof(MissionWinXCoinsInLinkFeature));
            dictMissions.Add(MissionType.WinXCoinsInBingo, typeof(MissionWinXCoinsInBingo));
            dictMissions.Add(MissionType.WinXCoinsInFeature, typeof(MissionWinXCoinsInFeature));
        }

        public static  MissionController CreateMission(Mission mission)
        {
            if (dictMissions.ContainsKey(mission.Type))
            {
                 var missionController = Activator.CreateInstance(dictMissions[mission.Type]) as MissionController;
                 missionController?.InitMission(mission);
                 return missionController;
            }
            return null;
        }
    }
}