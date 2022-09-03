using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class Constant11031
    {
        public static readonly uint TruckElementId = 18;

        public static readonly uint SingleRedChill = 12;

        public static readonly uint SingleYellowChill = 15;


        public static readonly uint TwoRedChill = 13;

        public static readonly uint ThreeRedChill = 14;

        public static readonly List<uint> ListRedChilliWithJackpot = new List<uint>()
        {
            19, 20, 21, 22, 23
        };
       
        public static readonly List<uint> ListGreenChilliWithJackpot = new List<uint>()
        {
            29, 30, 31, 32, 33
        };

        public static readonly List<uint> ListYellowChilliWithJackpot = new List<uint>()
        {
            24, 25, 26, 27, 28
        };

        public static readonly List<uint> ListGreenChilli = new List<uint>()
        {
            16, 29, 30, 31, 32, 33
        };

        public static readonly List<uint> ListYellowChilli = new List<uint>()
        {
            15, 24, 25, 26, 27, 28
        };

        public static readonly List<uint> ListExceptGreenChilliIds = new List<uint>()
        {
            12, 13, 14, 15, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28
        };

        public static readonly List<uint> ListChilliIds = new List<uint>()
        {
            12, 13, 14, 15, 16, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33
        };

        public static readonly List<uint> ListExceptChilliIds = new List<uint>()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
        };

        public static readonly List<uint> ListLowLevelAllElementIds = new List<uint>()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11
        };

        public static readonly string[] JackpotsGameObjectAddress =
        {
            "FlyJackpot_Mini", "FlyJackpot_Minor", "FlyJackpot_Major", "FlyJackpot_Ultra",
            "FlyJackpot_Grand"
        };

        public static readonly List<uint> ListJackpotChilliIds = new List<uint>()
        {
            19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33
        };

        public static readonly List<string> ListLetter = new List<string>()
        {
            "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U",
            "V", "W", "X", "Y", "Z"
        };

        public static uint ChangeGreenChilliId(uint id)
        {
            switch (id)
            {
                case 16:
                    return 12;
                case 29:
                    return 19;
                case 30:
                    return 20;
                case 31:
                    return 21;
                case 32:
                    return 22;
                case 33:
                    return 23;
            }

            return 12;
        }

        public static uint ChangeYellowChilliId(uint id)
        {
            switch (id)
            {
                case 15:
                    return 12;
                case 24:
                    return 19;
                case 25:
                    return 20;
                case 26:
                    return 21;
                case 27:
                    return 22;
                case 28:
                    return 23;
            }

            return 12;
        }

        public static int GetFinalPepperIndex(int index)
        {
            if (index >= 6 && index <= 7) //6,7
            {
                return 0;
            }
            else if (index >= 8 && index <= 9) //8,9
            {
                return 1;
            }
            else if (index >= 10 && index <= 11) //10,11
            {
                return 2;
            }
            else if (index >= 12 && index <= 14) //12,13,14
            {
                return 3;
            }
            else if (index >= 15 && index <= 17) //14-18
            {
                return 4;
            }
            else if (index >= 18 && index <= 20) //18-20
            {
                return 5;
            }
            else if (index >= 21 && index <= 24)
            {
                return 6;
            }
            else if (index >= 25 && index <= 29)
            {
                return 7;
            }
            else if (index >= 30 && index <= 34)
            {
                return 8;
            }
            else if (index >= 35 && index <= 39)
                return 9;
            else
            {
                return 10;
            }
        }

    private static List<string> LstJackpotPath = new List<string>
        {
            "UIJackpotMini11031", "UIJackpotMinor11031", "UIJackpotMajor11031", "UIJackpotUltra11031",
            "UIJackpotGrand11031"
        };

        private static List<string> LstJackpotPopUpPath = new List<string>
        {
            "UIJackpotMiniView11031", "UIJackpotMInorView11031", "UIJackpotMajorView11031", "UIJackpotUltraView11031",
            "UIJackpotGrandView11031"
        };

        public static readonly List<string> ListMultiplrLink = new List<string>()
        {
            "WheeRespinGame1", "WheeRespinGame2", "WheeRespinGame3", "WheeRespinGame4"
        };

        public static List<int> WinRateLists = new List<int>
        {
            300, 500, 700, 1000, 1500, 2000, 3000, 5000, 8000, 15000
        };

        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPopUpPath[index];
        }

        public static async Task ShowJackpot(MachineContext machineContext, uint jackpotId, ulong jackpotPay)
        {
            if (jackpotId == 5)
            {
                await machineContext.view.Get<UIJackpotGrandView11031>().ShowJackpot(jackpotPay);
            }
            else if (jackpotId == 4)
            {
                await machineContext.view.Get<UIJackpotUltraView11031>().ShowJackpot(jackpotPay);
            }
            else if (jackpotId == 3)
            {
                await machineContext.view.Get<UIJackpotMajorView11031>().ShowJackpot(jackpotPay);
            }
            else if (jackpotId == 2)
            {
                await machineContext.view.Get<UIJackpotMInorView11031>().ShowJackpot(jackpotPay);
            }
            else if (jackpotId == 1)
            {
                await machineContext.view.Get<UIJackpotMiniView11031>().ShowJackpot(jackpotPay);
            }
        }
    }
}