using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class Constant11027
    {
        public static readonly uint WildElementId = 10;
        
        public static readonly uint ScatterElementId = 9;

        public static int wheelDesignHeight = 465+100;

        public static int jackpotPanelHeight = 83 + 72 + 72 + 10;

        public static int wheelPanelHeight = 430;

        public static readonly List<uint> ListWildElementIds = new List<uint>()
        {
            11,12,13,14,15,16,17,18,19,20,21
        };

        private static string BonusPopPath = "UIWheelBonusFinish";
        
        private static List<string> LstJackpotPath =
            new List<string>{"UIJackpotMini11027", "UIJackpotMinor11027", "UIJackpotMajor11027","UIJackpotMega11027","UIJackpotUltra11027","UIJackpotGrand11027"};
        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPath[index];
        }
        
	    public static async Task ShowJackpot(MachineContext machineContext, uint jackpotId,ulong jackpotPay)
        {
	        AudioUtil.Instance.PlayAudioFx("LinkGameEnd_Open");
	        TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
	        var assetAddress = GetJackpotAddress((int)jackpotId-1);
	        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase11027>(assetAddress);
	        view.SetJackpotWinNum(jackpotPay);
	        view.SetPopUpCloseAction(async () =>
	        {
		        taskGrandWin.SetResult(true);
	        });
	        await taskGrandWin.Task;
        }
	    
	    public static async Task ShowBonus(MachineContext machineContext,ulong jackpotPay)
        {
	        machineContext.view.Get<WheelRollingView11027>().StopMusic();
	        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
	        var view = PopUpManager.Instance.ShowPopUp<UIWheelBonusFinish11027>(BonusPopPath);
	        view.SetJackpotWinNum(jackpotPay);
	        view.SetPopUpCloseAction(async () =>
	        {
		        taskGrandWin.SetResult(true);
	        });
	        await taskGrandWin.Task;
        }
	    
	    public static async Task ShowBonusJackpot(MachineContext machineContext, uint jackpotId,ulong jackpotPay)
        {
	        machineContext.view.Get<WheelRollingView11027>().StopMusic();
	        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
	        var assetAddress = GetJackpotAddress((int)jackpotId-1);
	        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase11027>(assetAddress);
	        view.SetJackpotWinNum(jackpotPay);
	        view.SetPopUpCloseAction(async () =>
	        {
		        taskGrandWin.SetResult(true);
	        });
	        await taskGrandWin.Task;
        }
    }
}