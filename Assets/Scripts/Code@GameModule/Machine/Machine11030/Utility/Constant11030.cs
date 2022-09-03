using System;
using System.Collections.Generic;
using System.Globalization;
using SRF;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class Constant11030
    {
        public static bool debugType = false;
        public static Dictionary<int,string> TrainReelsNameDictionary = new Dictionary<int,string>()
        {
            {3,"Reels3"},
            {4,"Reels4"},
            {5,"Reels5"},
        };
        public static string WheelBaseGameName = "WheelBaseGame";
        public static string WheelFreeGameName = "WheelFreeGame";
        public static string WheelLineFeatureGameName = "WheelLineFeatureGame";
        public static readonly List<string> JackpotName = new List<string>{"","Mini","Minor","Major","Grand"};
        public static readonly List<uint> TrainList = new List<uint>() {15,16,17,18};
        public static readonly List<uint> ValueList = new List<uint>() {30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48};
        public static readonly List<uint> WildList = new List<uint>() {11,21,22};
        public static readonly List<uint> NormalList = new List<uint>() {1,2,3,4,5,6,7,8,9,10};
        public static readonly List<uint> ScatterList = new List<uint>() {12};
        public static readonly List<uint> ScatterAndWildList = new List<uint>() {12,11,21,22};
        public static readonly List<uint> StarList = new List<uint>() {13};
        public static readonly List<uint> GoldTrainList = new List<uint>() {14};

        public static readonly List<uint> WinLevelDictionary = new List<uint>() {0,100,300,800,2000,3000,5000,10000};
        
        public static uint GetRandomNormalElementId()
        {
            return NormalList[Random.Range(0, NormalList.Count)];
            // return NormalList.Random();
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