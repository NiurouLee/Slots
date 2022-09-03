//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 15:28
//  Ver : 1.0.0
//  Description : Constant11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;

namespace GameModule
{
    public class Constant11016
    {
        public const int TotalFreePanels = 9;
        private static uint[] FeaturedElements= {8,9,10,11,12,13,14};
        private static uint[] FeaturedSlotElements= {15,16,17};
        private static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMinor11016", "UIJackpotMajor11016", "UIJackpotGrand11016"};

        public static bool IsFeaturedElement(uint elementId)
        {
            return Array.IndexOf(FeaturedElements, elementId)>=0;
        }
        public static bool IsFeaturedSlotElement(uint elementId)
        {
            return Array.IndexOf(FeaturedSlotElements, elementId)>=0;
        }
        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPath[index];
        }
    }
}