//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-26 16:38
//  Ver : 1.0.0
//  Description : MissionItem.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using MissionType = DragonU3DSDK.Network.API.ILProtocol.Mission.Types.Type;
namespace GameModule
{
    public class MissionController
    {
        protected Mission missionData;
        public MissionType MissionType;
        public int Times=>(int)missionData.Times;
        public int CurrentTimes=>(int)missionData.CurrentTimes;

        public MissionController(MissionType type)
        {
            MissionType = type;
        }

        public void InitMission(Mission mission)
        {
            missionData = mission;
        }
        protected virtual string GetDescTextFormat()
        {
            return string.Empty;
        }

        public virtual string GetContentDescText()
        {
            return GetDescTextFormat();
        }

        public Mission GetMission()
        {
            return missionData;
        }

        public long GetExtraCoinAmount()
        {
            // var item = XItemUtility.GetItem(missionData.DiamondExtraItems, Item.Types.Type.Coin);
            // if (item != null)
            // {
            //     return (long)item.Coin.Amount;
            //
            // }
            return 0;
        }
        public virtual float GetProgress()
        {
            if (missionData == null) return 0;
            if (missionData.Times > 0)
            {
                return Mathf.Min(missionData.CurrentTimes / (float)missionData.Times,1f);   
            }
            if (missionData.TargetCoin > 0)
            {
                return Mathf.Min(missionData.CurrentCoin / (float)missionData.TargetCoin,1f);          
            }
            return 0f;
        }

        public virtual string GetProgressDescText()
        {
            if (missionData.Times > 0)
            {
                return missionData.CurrentTimes.GetCommaFormat() + "/" + missionData.Times.GetCommaFormat();   
            }
            if (missionData.TargetCoin > 0)
            {
                return missionData.CurrentCoin.GetCommaFormat() + "/" + missionData.TargetCoin.GetCommaFormat();   
            }
            return string.Empty;
        }

        public bool ProgressIsPercentFormat()
        {
            return missionData.TargetCoin > 0;
        }
        
        public string GetProgressPercentDescText()
        {
            if (missionData.TargetCoin > 0)
            {
                return Mathf.FloorToInt(GetProgress()*100) + "%";   
            }
            return string.Empty;
        }

        public string GetLockContentText()
        {
            return "COMPLETE \nPREVIOUS MISSION\nTO UNLOCK!";
        }
        
        public string GetFinishContentText()
        {
            return "WELL DONE!";
        }

        public uint GetMissionPoints()
        {
            var item = XItemUtility.GetItem(missionData.Items,Item.Types.Type.MissionPoints);
            return item != null ? item.MissionPoints.Amount : 0;
        }

        public uint GetMissionStar()
        {
            var item = XItemUtility.GetItem(missionData.Items,Item.Types.Type.MissionStar);
            return item != null ? item.MissionStar.Amount : 0;
        }

        public ulong CompleteNeedCostDiamond()
        {
            return missionData.Diamond;
        }
        
        public bool IsFinish()
        {
            if (missionData == null)
            {
                return false;
            }
            return missionData.Finished;
        }

        public bool IsClaimed()
        {
            if (missionData == null)
            {
                return false;
            }
            return missionData.Collected;
        }

        public virtual bool IsUpdateOnRoundFinish()
        {
            return true;
        }

        public virtual string GetMissionUI()
        {
            return missionData.Ui;
        }

        public virtual string GetSpinButtonDes()
        {
            if (IsFinish() && !IsClaimed())
            {
                return "COLLECT";
            }
            return "SPIN";
        }

        public ulong ExpectedBet=>missionData.ExpectedBet;

        public bool NeedMinimumBetMission => missionData.Type == MissionType.BetXCoinsWithMinimumBetOfY ||
                                    missionData.Type == MissionType.WinXTimesWithMinimumBetOfY;
    }

    public class MissionContinuousWinTime : MissionController
    {
        public MissionContinuousWinTime():base(MissionType.WinXTimesInARow)
        {

        }
        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.Times} TIMES\n IN A ROW.";
        }
    }
    public class MissionSpinTime : MissionController
    {
        public MissionSpinTime():base(MissionType.SpinXTimes)
        {

        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times == 1)
            {
                return $"SPIN {missionData.Times} TIME";
            }
            
            return $"SPIN {missionData.Times} TIMES";
        }

        public override bool IsUpdateOnRoundFinish()
        {
            return false;
        }
    }
    public class MissionWinTime : MissionController
    {
        public MissionWinTime():base(MissionType.WinXTimes)
        {

        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times == 1)
            {
                return $"WIN {missionData.Times} TIME";
            }
            return $"WIN {missionData.Times} TIMES";
        }
    }
    public class MissionWinXCoinsInASingleSpinYTimes : MissionController
    {
        public MissionWinXCoinsInASingleSpinYTimes():base(MissionType.WinXCoinsInASingleSpinYTimes)
        {

        }
        protected override string GetDescTextFormat()
        {
            return $"WIN\n{missionData.TargetCoin.GetAbbreviationFormat()} COINS IN A SINGLE SPIN\n{missionData.Times} TIMES";
        }
    }
    public class MissionBet : MissionController
    {
        public MissionBet():base(MissionType.BetATotalOfXCoins)
        {
        }
        protected override string GetDescTextFormat()
        {
            return $"BET A TOTAL OF\n{missionData.TargetCoin.GetAbbreviationFormat()} COINS";
        }
        public override bool IsUpdateOnRoundFinish()
        {
            return false;
        }
        
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
    public class MissionWin : MissionController
    {
        public MissionWin():base(MissionType.WinATotalOfXCoins)
        {
        }
        protected override string GetDescTextFormat()
        {
            return $"WIN A TOTAL OF\n{missionData.TargetCoin.GetAbbreviationFormat()} COINS";
        }
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
    public class MissionWinInFreeSpin: MissionController
    {
        public MissionWinInFreeSpin():base(MissionType.WinXCoinsInYFreeSpins)
        {
        }
        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()} COINS IN FREE SPIN";
        }
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
    public class MissionBigWin : MissionController
    {
        public MissionBigWin():base(MissionType.GetBigWinXTimes)
        {
        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times > 1)
            {
                return $"GET BIG WIN {missionData.Times} TIMES";
            }
            
            return $"GET BIG WIN {missionData.Times} TIME";
        }
    }

    public class MissionWinXCoinsInYSpins : MissionController
    {
        public MissionWinXCoinsInYSpins():base(MissionType.WinXCoinsInYSpins)
        {
        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times == 1)
            {
                return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()}\nCOINS IN {missionData.Times} SPIN";
            }
            return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()}\nCOINS IN {missionData.Times} SPINS";
        }

        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
        
        public override float GetProgress()
        {
            if (missionData == null) return 0;
            if (missionData.TargetCoin > 0)
            {
                return Mathf.Min(missionData.CurrentCoin / (float)missionData.TargetCoin,1f);          
            }
            return 0f;
        }
        
        public override string GetSpinButtonDes()
        {
            if (IsFinish() && !IsClaimed())
            {
                return "COLLECT";
            }
            return (missionData.Times - missionData.CurrentTimes).ToString();
        }
    }
    public class MissionHugeWin : MissionController
    {
        public MissionHugeWin():base(MissionType.GetHugeWinXTimes)
        {
        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times > 1)
            {
                return $"GET HUGE WIN\n{missionData.Times} TIMES";
            }
           
            return $"GET HUGE WIN\n{missionData.Times} TIME";
        }
    }
    public class MissionFiveOfAKind : MissionController
    {
        public MissionFiveOfAKind():base(MissionType.Get5OfAKindXTimes)
        {
        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times > 1)
            {
                return $"GET 5 OF A KIND\n{missionData.Times} TIMES";
            }
            return $"GET 5 OF A KIND\n{missionData.Times} TIME";
        }
    }
    public class MissionLevelUp : MissionController
    {
        public MissionLevelUp():base(MissionType.LevelUpXTimes)
        {
        }
        protected override string GetDescTextFormat()
        {
            if (missionData.Times > 1)
            {
                return $"LEVEL UP\n{missionData.Times} TIMES";
            }
            
            return $"LEVEL UP\n{missionData.Times}\nTIME";
        }
    }
    
    public class MissionGetLockLinkXTimes : MissionController
    {
        public MissionGetLockLinkXTimes():base(MissionType.GetLockItLinkXTimes)
        {
        }

        protected override string GetDescTextFormat()
        {
            if (missionData.Times > 1)
            {
                return $"GET LINK FEATURE\n{missionData.Times} TIMES";
            }
            return $"GET LINK FEATURE\n{missionData.Times} TIME";
        }
    }
    
    public class MissionGetFreeGameXTimes : MissionController
    {
        public MissionGetFreeGameXTimes():base(MissionType.GetFreeGameXTimes)
        {
        }

        protected override string GetDescTextFormat()
        {
            if (missionData.Times == 1)
            {
                return $"GET FREE GAME {missionData.Times} TIME";
            }
            else
            {
                return $"GET FREE GAME {missionData.Times} TIMES";
            }
        }
    }
    
    public class MissionWinXTimesWithMiniBet : MissionController
    {
        public MissionWinXTimesWithMiniBet():base(MissionType.WinXTimesWithMinimumBetOfY)
        {
        }

        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.Times} TIMES WITH MINIMUM BET OF {missionData.ExpectedBet.GetCommaFormat()}";
        }
        
        public override string GetProgressDescText()
        {
            return base.GetProgressDescText();
        }
    }
    
    public class MissionBetXCoinsWithMiniBet : MissionController
    {
        public MissionBetXCoinsWithMiniBet():base(MissionType.BetXCoinsWithMinimumBetOfY)
        {
        }

        protected override string GetDescTextFormat()
        {
            return $"BET {missionData.TargetCoin.GetAbbreviationFormat()} COINS WITH MINIMUM BET OF {missionData.ExpectedBet.GetCommaFormat()}";
        }
        
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
    
    public class MissionWinXCoinsInLinkFeature: MissionController
    {
        public MissionWinXCoinsInLinkFeature():base(MissionType.WinXCoinsInLinkFeature)
        {
        }

        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()} COINS IN LINK FEATURE";
        }
        
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
    
    public class MissionWinXCoinsInBingo : MissionController
    {
        public MissionWinXCoinsInBingo():base(MissionType.WinXCoinsInBingo)
        {
        }

        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()} COINS IN BINGO";
        }
        
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    } 
    
    public class MissionWinXCoinsInFeature : MissionController
    {
        public MissionWinXCoinsInFeature():base(MissionType.WinXCoinsInFeature)
        {
        }

        protected override string GetDescTextFormat()
        {
            return $"WIN {missionData.TargetCoin.GetAbbreviationFormat()} COINS IN FEATURE";
        }
        
        public override string GetProgressDescText()
        {
            return GetProgressPercentDescText();
        }
    }
}