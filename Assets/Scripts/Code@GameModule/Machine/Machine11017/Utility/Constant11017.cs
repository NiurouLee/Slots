using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;

namespace GameModule
{
    public class Constant11017
    {
	    public static uint ScatterElementId = 11;
	    public static uint SmallGoldElementId = 12;
	    public static uint PuePleElementId = 13;
	    public static uint GoldElementId = 15;
	    public static uint FixedElementId = 16;
	    public static readonly List<uint> ListWildAllElementIds = new List<uint>()
        {
	        12,13
        };
	    public static readonly List<uint> ListLowLevelAllElementIds = new List<uint>()
        {
	        5,6,7,8,9,10
        };
	    public static int wheelDesignHeight = 1000;
        public static int jackpotPanelHeight = 100*2+35;
        private static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMinor11017", "UIJackpotMajor11017", "UIJackpotGrand11017"};
        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPath[index];
        }
    }
}