
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public static class Constant11020
    {   
        public static bool firstInitWheel = true;

        public static bool hasSpined = false;

        public static uint[,] neighborPositions = new uint[,] { 
            {2,5,6,0,0,0},//1
            {1,3,6,7,0,0}, //2
            {2,4,7,8,0,0}, //3
            {3,8,9,0,0,0},//4
            {1,6,10,0,0,0},//5
            {1,2,5,7,10,11},//6
            {2,3,6,8,11,12},//7
            {3,4,7,9,12,13},//8
            {4,8,13,0,0,0},//9
            {5,6,11,14,15,0},//10
            {6,7,10,12,15,16},//11
            {7,8,11,13,16,17},//12
            {8,9,12,17,18, 0},//13
            {10,15,19,0,0,0},//14
            {10,11,14,16,19,20},//15
            {11,12,15,17,20,21},//16
            {12,13,16,18,21,22},//17
            {13,17,22,0,0,0},//18
            {14,15,20,0,0,0},//19
            {15,16,19,21,0,0},//20
            {16,17,20,22,0,0},//21
            {17,18,21,0,0,0} //22
        };

        public static uint[,] rightCorners = new uint[,] { 
            {5,6},//1
            {6,7}, //2
            {7,8}, //3
            {8,9},//4
            
            {10,0},//5
            {10,11},//6
            {11,12},//7
            {12,13},//8
            {13,0},//9
            
            {14,15},//10
            {15,16},//11
            {16,17},//12
            {17,18},//13
            
            {19,0},//14
            {19,20},//15
            {20,21},//16
            {21,22},//17
            {22,0},//18
        };

        public static uint[,] leftCorners = new uint[,] { 
            {0,0},//1
            {0,0}, //2
            {0,0}, //3
            {0,0},//4
            
            {1,0},//5
            {1,2},//6
            {2,3},//7
            {3,4},//8
            {4,0},//9
            
            {5,6},//10
            {6,7},//11
            {7,8},//12
            {8,9},//13
            
            {10,0},//14
            {10,11},//15
            {11,12},//16
            {12,13},//17
            {13,0},//18
            
            {14,15},//19
            {15,16},//20
            {16,17},//21
            {17,18} //22
        };

        public static uint wildElement    = 41;
        public static uint buleWildElement = 42;

        public static uint bonusElement = 51;
        public static uint bonusWildElement = 53;

        public static uint lionElement  = 52;

        public static string baseWheelName          = "WheelBaseGame";
        public static string freeWheelName          = "WheelFreeGame";
        public static string superBonusWheelName    = "WheelSuperBonusGame";

        public static string GetReelName(string wheelName)
        {
            if (wheelName == superBonusWheelName)
            {
                return "SuperFreeReels";
            }
            else if (wheelName == freeWheelName)
            {
                return "FreeReels";
            }
            return "Reels";
        }
    }
}
