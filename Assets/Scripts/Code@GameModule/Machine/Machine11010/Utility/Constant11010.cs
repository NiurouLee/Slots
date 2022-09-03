//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 19:32
//  Ver : 1.0.0
//  Description : Constant11010.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using Dlugin;
using UnityEngine;

namespace GameModule
{
    public static class Constant11010
    {
        private static readonly List<uint>  ElementLink;
        private static readonly List<uint> ElementStack;
        public static string LinkReels = "LinkReels";
        public static string FreeReels = "FreeReels";
        private static List<uint> NormalElement = new List<uint> {1, 2, 3, 4, 5, 6,7,8,9};
        private static List<uint> JackpotElement = new List<uint> {15, 16, 17, 18};
        public const int MIN_LINK_TRIGGER_COUNT = 3;
        private static List<uint> WildElement = new List<uint> {10, 11, 12};

        private static string[] JackpotName = new string[4]
            {"JP_LV1_mini", "JP_LV2_minor", "JP_LV3_major", "JP_LV4_grand"};
        private static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMini11010", "UIJackpotMinor11010", "UIJackpotMajor11010", "UIJackpotGrand11010"};
        
        static Constant11010()
        {
            ElementLink = new List<uint>();
            for (uint i = 14; i <= 24; i++)
            {
                ElementLink.Add(i);
            }
            ElementStack = new List<uint> {10, 11, 12};
        }

        public static bool IsLinkElement(uint elementId)
        {
            return ElementLink.Contains(elementId);
        }
        
        public static bool IsJackpotElement(uint elementId)
        {
            return JackpotElement.Contains(elementId);
        }

        public static bool IsStackElement(uint elementId)
        {
            return ElementStack.Contains(elementId);
        }
        
        public static uint NextNormalElementId()
        {
            return NormalElement[Random.Range(0, NormalElement.Count)];
        }

        public static string GetJackpotName(uint id)
        {
            return JackpotName[id - 1];
        }
        
        public static bool IsNormalElementId(uint elementId)
        {
            return NormalElement.Contains(elementId);
        }
        public static bool IsWildElementId(uint elementId)
        {
            return WildElement.Contains(elementId);
        }
        
        public static string GetJackpotAddress(uint elementId)
        {
            return LstJackpotPath[(int)elementId - 15];
        }
    }
}