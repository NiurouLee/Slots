using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class Constant11029
    {
        public static readonly uint ScatterElementId = 12;

        public static readonly uint WildElementId = 10;

        public static readonly uint BigScatterElementId = 39;

        public static readonly uint HorseElementId = 1;

        public static readonly uint StarElementId = 11;
        
        public static readonly uint BigBagElementId = 38;
        
        public static readonly uint BigWildElementId = 37;

        public static int wheelDesignHeight = 1000;

        public static int jackpotPanelHeight = 100 * 3 + 50;

        public static readonly List<uint> ListJSymbolElementIds = new List<uint>()
        {
            14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27
        };
 
        private static List<string> ListFreeReels = new List<string>
        {
            "WheelFreeGame", "WheelMagicBonusGame", "WheelMiniGame"
        };

        public static readonly List<string> ListFreeNodeGame = new List<string>()
        {
            "WheelBigPointGame1", "WheelBigPointGame2",
            "WheelBigPointGame3",
        };


        private static List<string> LstJackpotPath =
            new List<string> {"UIJackpotMinor11029", "UIJackpotMajor11029", "UIJackpotGrand11029"};

        public static string GetListFree(uint freeSpinId)
        {
            return ListFreeReels[(int) freeSpinId];
        }

        public static string GetJackpotAddress(int index)
        {
            return LstJackpotPath[index];
        }

        public static async Task ShowBonusJackpot(MachineContext machineContext, uint jackpotId, ulong jackpotPay)
        {
            AudioUtil.Instance.PlayAudioFx("Wheel_JackpotOpen");
            TaskCompletionSource<bool> taskGrandWin = new TaskCompletionSource<bool>();
            var assetAddress = GetJackpotAddress((int) jackpotId - 1);
            var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(assetAddress);
            view.SetJackpotWinNum(jackpotPay);
            view.SetPopUpCloseAction(async () =>
            {
                machineContext.view.Get<JackpotPanel11029>().HideWinFrame((int)jackpotId);
                taskGrandWin.SetResult(true);
            });
            await taskGrandWin.Task;
        }
    }
}