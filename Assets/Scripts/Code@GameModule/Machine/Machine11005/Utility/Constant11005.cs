using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class Constant11005
    {
        public static readonly Vector3 ElementFreeScale = new Vector3(0.5f, 0.5f, 1);

        public static readonly uint ElementIdBoom = 14;


        public static readonly uint ElementJackpot = 12;


        public static readonly List<string> listFree10 = new List<string>() {"WheelFreeGame1", "WheelFreeGame2"};

        public static readonly List<string> listFree20 = new List<string>() {"WheelFreeGame3","WheelFreeGame1", "WheelFreeGame2"};
        
        public static readonly List<string> listFree30 = new List<string>() {"WheelFreeGame3", "WheelFreeGame4","WheelFreeGame1", "WheelFreeGame2"};

        public static List<string> GetListFree(uint luckCount)
        {
            if (luckCount < 10)
            {
                return listFree10;

            }
            else if(luckCount < 20)
            {
                return listFree20;

            }
            else
            {
                return listFree30;
            }
        }
        
        
        public static List<int> listUnlockLetterNum = new List<int>()
        {
            0,2,4,6
        };
    }
}