//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 19:11
//  Ver : 1.0.0
//  Description : Constant11011.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;

namespace GameModule
{
    public class Constant11011
    {
        public static uint ElementWild = 9;
        public static uint ElementOnlyAddSpin = 13;
        //吸收所有
        public static List<uint> ElementWrapAll = new List<uint>{12,15};
        //吸收触发
        public static List<uint> ElementWrapLock = new List<uint>{11,14};
        private static readonly List<uint> ElementLink = new List<uint> {10,21,22,23,24,25};
        private static readonly List<uint> ElementAddSpin = new List<uint> {13, 14, 15};
        private static List<uint> NormalElement = new List<uint> {1, 2, 3, 4, 5, 6,7,8,9};
        private static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMini11011", "UIJackpotMinor11011", "UIJackpotMajor11011", "UIJackpotGrand11011"};
        public static bool IsLinkElement(uint elementId)
        {
            return ElementLink.Contains(elementId);
        }
        
        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPath[index];
        }

        public static bool IsWrapElement(uint elementId)
        {
            return ElementWrapLock.Contains(elementId) || ElementWrapAll.Contains(elementId) || ElementAddSpin.Contains(elementId);
        }
        
        public static bool IsWrapLockElement(uint elementId)
        {
            return ElementWrapLock.Contains(elementId);
        }
        
        public static bool IsWrapAllElement(uint elementId)
        {
            return ElementWrapAll.Contains(elementId);
        }

        public static bool IsAddSpinElement(uint elementId)
        {
            return ElementAddSpin.Contains(elementId);
        }
        
        public static bool IsAddSpinLinkElement(uint elementId)
        {
            return ElementAddSpin.Contains(elementId) && elementId != ElementOnlyAddSpin;
        }
        
        public static bool IsNormalElementId(uint elementId)
        {
            return NormalElement.Contains(elementId);
        }
    }
}