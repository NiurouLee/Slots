using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class Constant11026
    {


	    public static int wheelDesignHeight = 822 + 185;
	    
	    public static int jackpotPanelHeight = 203;

	    public static readonly uint B01ElementId = 1;
	    
	    public static readonly uint WildElementId = 11;
	    
	    public static readonly uint J01MultiplierElementId = 18;
	     
	    public static readonly uint J01RowElementId = 19;
	    
        public static readonly List<uint> ListBonusElementIds = new List<uint>()
        {
            12,18,19,20,21,22,23,24,25,26,27,28,29,30,31
        };
        
        public static readonly List<uint> ListBonusTextElementIds = new List<uint>()
        {
            12,20,21,22,23,24,25,26,27,28,29,30,31
        };
        
        public static readonly List<uint> ListBonusJackpotElementIds = new List<uint>()
        {
	        13,14,15,16,17
        };
        
        public static readonly List<uint> ListBonusAllElementIds = new List<uint>()
        {
	        12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31
        };
        
	    public static readonly List<uint> ListGrayAllElementIds = new List<uint>()
        {
	        1,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31
        };
        
        public static readonly List<uint> ListLowLevelElementIds = new List<uint>()
        {
	        1,2,3,4,5,6,7,8,9,10
        };

        public static readonly List<uint> ListLinkLoLevelElementIds = new List<uint>()
        {
            2,3,4,5          
        };
        
        public static readonly List<uint> fourRowPositionIds = new List<uint>()
        {
            0,1,2,7,8,9,14,15,16
        };
        
	    public static readonly List<uint> FiveRowPositionIds = new List<uint>()
        {
            0,1,7,8,14,15
        };
	    
	    public static readonly List<uint> SixRowPositionIds = new List<uint>()
        {
            0,7,14
        };
	    
	    public static readonly List<uint> ListShoLowLevelAllElementIds = new List<uint>()
        {
	        6,7,8,9,10
        };
	    
	    public static readonly List<uint> ListShowBigLevelAllElementIds = new List<uint>()
        {
	        1,2,3,4,5,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31
        };
        
        public static readonly List<string> Link0 = new List<string>() {"WheelLinkGame1", "WheelLinkGame2","WheelLinkGame6"};

        public static readonly List<string> Link1 =  new List<string>() {"WheelLinkGame1", "WheelLinkGame3","WheelLinkGame6"};
        
        public static readonly List<string> Link2 =  new List<string>() {"WheelLinkGame1", "WheelLinkGame4","WheelLinkGame6"};
        
        public static readonly List<string> Link3 =new List<string>() {"WheelLinkGame1", "WheelLinkGame5","WheelLinkGame6"};

        public static List<string> GetListLink(long rowNUmber)
        {
	        if (rowNUmber == 0)
            {
                return Link0;

            }else if(rowNUmber == 1) 
	        {
                return Link1;

            }else if (rowNUmber == 2)
	        {
		        return Link2;
	        }
	        else
	        {
		        return Link3;
	        }
        }


        protected static readonly string[] _jackpotsAddress =
	        {"UIJackpotMini11026","UIJackpotMinor11026", "UIJackpotMajor11026","UIJackpotMaxi11026","UIJackpotGrand11026"};

        public static async Task ShowJackpot(MachineContext machineContext, LogicStepProxy logicStepProxy,
	        uint jackpotId,ulong jackpotPay)
        {
	        TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
	        
	        XDebug.Log($"ShowJackpot:{jackpotId},{jackpotPay}");
	        
	        var assetAddress = _jackpotsAddress[jackpotId - 1];
	        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase11026>(assetAddress);
	        AudioUtil.Instance.PlayAudioFx("J01_JackpotOpen");
	        var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotPay);
	        view.SetJackpotWinNum(jackpotWin);
	        view.SetPopUpCloseAction(async () => { taskGrandWin.SetResult(true); });
	        await taskGrandWin.Task;
	        AudioUtil.Instance.PlayAudioFx("B01_Collect"); 
        }
    }
}