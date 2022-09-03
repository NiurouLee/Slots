using System;
using System.Collections.Generic;
using System.Globalization;
using SRF;
using Random = UnityEngine.Random;

namespace GameModule
{
    public enum BtnType11024
    {
        OpenMap,
        CloseMap,
    }
    public class Constant11024
    {
        public static bool debugType = false;
        public static string WheelBaseGameName = "WheelBaseGame";
        public static readonly List<string> JackpotName = new List<string>{"","Minor","Major","Grand"};
        public static readonly float CollectS01Time = 0.5f;
        public static readonly float CollectCoinTime = 0.3f;
        public static readonly float CollectPigCoinTime = 0.6f;
        public static readonly float ElementFlyBeforeCollectPigCoinTime = 0.2f;
        public static readonly List<uint> WinLevelDictionary = new List<uint>() {0,100,300,800,2000,3000,5000,10000};
        public static readonly List<int> BigPointList = new List<int>() {5,10,15,20,25};
        public static readonly Dictionary<uint, uint> LongWildMultiToId = new Dictionary<uint, uint>()
        {
            {2,83},
            {3,84},
            {5,85},
            {8,86},
            {10,87},
            {15,88},
            {20,89},
            {25,90},
            {30,91},
            {50,92},
            {100,93},
        };
        public static bool IsBigPoint(int level)
        {
            return BigPointList.Contains(level);
        }

        public static bool IsEmptyElement(uint elementId)
        {
            if (elementId == 831 || elementId == 832)
                return true;
            return false;
        }
        public static bool IsPigValue(uint elementId,int pigType)
        {
            if (pigType == 0)
            {
                if (elementId >= 26 && elementId <= 36)
                    return true;   
            }
            else if (pigType == 1)
            {
                if (elementId >= 40 && elementId <= 50)
                    return true;   
            }
            else if (pigType == 2)
            {
                if (elementId >= 54 && elementId <= 64)
                    return true;   
            }
            return false;
        }
        public static bool IsPigJackpot(uint elementId,int pigType)
        {
            if (pigType == 0)
            {
                if (elementId >= 37 && elementId <= 39)
                    return true;   
            }
            else if (pigType == 1)
            {
                if (elementId >= 51 && elementId <= 53)
                    return true;   
            }
            else if (pigType == 2)
            {
                if (elementId >= 65 && elementId <= 67)
                    return true;   
            }
            return false;
        }

        public static bool IsGoldValue(uint elementId)
        {
            if (elementId >= 68 && elementId <= 78)
                return true;
            return false;
        }
        
        public static bool IsGoldJackpot(uint elementId)
        {
            if (elementId >= 79 && elementId <= 81)
                return true;
            return false;
        }
        public static bool IsValueId(uint elementId)
        {
            if (IsPigValue(elementId,0) ||
                IsPigValue(elementId,1) ||
                IsPigValue(elementId,2) ||
                IsGoldValue(elementId))
                return true;
            return false;
        }
        public static bool IsJackpotId(uint elementId)
        {
            if (IsPigJackpot(elementId,0) ||
                IsPigJackpot(elementId,1) ||
                IsPigJackpot(elementId,2) ||
                IsGoldJackpot(elementId))
                return true;
            return false;
        }
        public static bool IsGoldId(uint elementId)
        {
            if (IsGoldValue(elementId) ||
                IsGoldJackpot(elementId))
                return true;
            return false;
        }

        public static bool IsPigId(uint elementId, int pigType)
        {
            if (IsPigValue(elementId, pigType) ||
                IsPigJackpot(elementId, pigType))
                return true;
            return false;
        }

        public static int IsAnyPigId(uint elementId)
        {
            for (var i = 0; i < 3; i++)
            {
                if (IsPigId(elementId, i))
                {
                    return i;
                }
            }
            return -1;
        }
        public static bool IsS01Id(uint elementId)
        {
            if (elementId == 1)
                return true;
            return false;
        }

        public static bool IsLongWild(uint elementId)
        {
            if (elementId >= 83 && elementId <= 93)
                return true;
            return false;
        }
        public static bool IsWildId(uint elementId)
        {
            if (elementId == 14 || 
                IsLongWild(elementId) )
                return true;
            return false;
        }
        public static uint GetRandomNormalElementId()
        {
            return (uint)Random.Range(1, 11);
        }
        
        public static string ChipNum2String(double num)
        {
            var thresholdNum = 1000;
            var tempNum = num;
            var index = 0;
            string[] postfix = {"", "K", "M", "B", "T", "Q"};
            string GetPostFix(int postfixIndex)
            {
                string totalPostfix = "";
                var postfixCount = postfix.Length - 1;
                while (postfixIndex > postfixCount)
                {
                    totalPostfix = postfix[postfixCount] + totalPostfix;
                    postfixIndex = postfixIndex - postfixCount;
                }
                totalPostfix = postfix[postfixIndex] + totalPostfix;
                return totalPostfix;
            }
            while (tempNum >= thresholdNum)
            {
                tempNum *= 0.001;
                index++;
            }

            var tempTempNum = tempNum;
            var tempNumDigits = 0;
            while (tempTempNum >= 1)
            {
                tempTempNum *= 0.1;
                tempNumDigits++;
            }
            var leastDigits = 3;
            switch (tempNumDigits)
            {
                case 3:
                    leastDigits = 3;
                    break;
                case 2:
                    leastDigits = 2;
                    break;
                case 1:
                    leastDigits = 2;
                    break;
            }

            var decimalsNum = leastDigits - tempNumDigits;
            string decimalsFormat = ".";
            for (var i = 0; i < decimalsNum; i++)
                decimalsFormat += "#";
            double f = Math.Floor((double)(tempNum * Math.Pow(10, decimalsNum)));
            tempNum = f / Math.Pow(10, decimalsNum);
            return tempNum.ToString(
                "#,#" + decimalsFormat + GetPostFix(index) + ";-#,#" + decimalsFormat + GetPostFix(index) + ";0",
                CultureInfo.InvariantCulture);
            
        }
    }
}