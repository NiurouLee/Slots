//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-13 17:23
//  Ver : 1.0.0
//  Description : Constant11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public static class Constant11007
    {
        public const int ELEMENT_WILD = 12;

        //Jackpot Element
        public const int ELEMENT_MINI = 26;
        public const int ELEMENT_MINOR = 27;
        public const int ELEMENT_MAJOR = 28;
        private static List<uint> CoinElement = new List<uint> {21, 22, 23, 24, 25};
        private static List<uint> JackpotElement = new List<uint> {ELEMENT_MINI, ELEMENT_MINOR, ELEMENT_MAJOR};
        private static List<uint> NormalElement = new List<uint> {1, 2, 3, 4, 5, 11, 12};

        private static List<uint> BigElement = new List<uint>{13, 14, 15};
        public const int ELEMENT_EMPTY = 99;

        public static uint INVALID_ITEM_ID = 2000;

        public static bool IsCoinElement(uint symbolId)
        {
            return CoinElement.Contains(symbolId);
        }

        public static bool IsJackpotElement(uint symbolId)
        {
            return JackpotElement.Contains(symbolId);
        }
        
        //Bonus Element
        public const int ELEMENT_RESPIN_SAME_WIN = 29;    //id:29 Spin赢钱，再接着同样的赢钱结果Spin(2-10)次 
        public const int ELEMENT_RANDOM_FREE_WILDS = 30;  //id:30 Free随机Wild
        public const int ELEMENT_RESPIN_UNTIL_WIN = 31;   //id:31 Spin赢钱
        public const int ELEMENT_SPIN_MULTI_WIN = 32;     //id:32 倍数
        public static List<uint> BonusElement = new List<uint>
            {ELEMENT_RESPIN_SAME_WIN, ELEMENT_RANDOM_FREE_WILDS, ELEMENT_RESPIN_UNTIL_WIN, ELEMENT_SPIN_MULTI_WIN};
        public static bool IsBonusElement(uint symbolId)
        {
            return BonusElement.Contains(symbolId);
        }

        public static Dictionary<int, string> DictBgName = new Dictionary<int, string>
        {
            {29,"RespinBonusFoxinessBG"},
            {30,"RespinBonusJusticeBG"},
            {31,"RespinBonusStunnerBG"},
            {32,"RespinBonusHoundBG"}
        };

        public static bool IsLinkElement(uint elementId)
        {
            return IsBonusElement(elementId) || IsCoinElement(elementId) || IsJackpotElement(elementId);
        }
        
        public const int MAX_LINK_COUNT = 9;

        public static string LinkReels = "LinkReels";
        public static Vector3 LinkElementScale = new Vector3(0.945f, 0.856f, 1);
        public static Color LinkGrayColor = new Color(0.5f,0.5f, 0.5f, 1f);

        public static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMini", "UIJackpotMinor", "UIJackpotMajor", "UIJackpotGrand"};

        public static string GetGrandAddress(string machineId)
        {
            return LstJackpotPath[LstJackpotPath.Count - 1]+machineId;
        }

        public static string GetJackpotAddress(uint elementId,string machineId)
        {
            return LstJackpotPath[(int)elementId - ELEMENT_MINI]+machineId;
        }

        public static uint NextNormalElementId()
        {
            return NormalElement[Random.Range(0, NormalElement.Count)];
        }

        public static bool IsNormalElementId(uint elementId)
        {
            return NormalElement.Contains(elementId);
        }
        public static bool IsBigElement(uint elementId)
        {
            return BigElement.Contains(elementId);
        }
    }
}