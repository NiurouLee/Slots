using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class Constant11015
    {
	    
	    public static readonly Vector3 ElementFreeScale = new Vector3(0.82f, 0.82f, 1);

	    public static float ZeusOffsetX = 1.3f;
	    
	    public static int wheelDesignHeight = 1112 + 60;
	    
	    public static int wheelBaseDesignHeight = 1112 + 60;
        
	    public static int jackpotPanelHeight = 0 * 3;
	    

	    public static readonly uint WildElementId = 1;
	    
	    public static readonly uint ZeusElementId = 2;
	    
	    public static readonly List<uint> ListWildAllElementId = new List<uint>()
	    {
		    1,15,16,17,18
	    };
	    
	    public static readonly uint ShieldElementId = 13;
	    
	    public static readonly uint ScatterElementId = 14;
	    
	    
	    public static readonly List<uint> ListLowLevelElementIds = new List<uint>()
	    {
		    2,3,4,5,6,7,8,9,10,11,12
	    };
	    
	    
	    public static readonly List<uint> ListWildMultipleElementId = new List<uint>()
	    {
		    15,16,17,18
	    };

    }
}