using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class WinState11035 : WinState
    {
        public WinState11035(MachineState machineState) : base(machineState)
        {
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);

            var panels = spinResult.GameResult.Panels;

            ulong totalWinRate = 0;
            for (var i = 0; i < panels.Count; i++)
            {
                totalWinRate += spinResult.GameResult.Panels[i].WinRate;
                var winLines = spinResult.GameResult.Panels[i].WinLines;
                // for (var l = 0; l < winLines.Count; l++)
                // {
                //     if (winLines[l].JackpotId > 0)
                //     {
                //         totalWinRate -= winLines[l].Pay;
                //     }
                //     else if (winLines[l].BonusGameId >= 1000)
                //     {
                //         totalWinRate -= winLines[l].Pay;
                //     }
                // }
            }

            wheelWin = totalWinRate * spinResult.GameResult.Bet / 100;

            displayCurrentWin = spinResult.GameResult.DisplayCurrentWin;
            displayTotalWin = spinResult.GameResult.DisplayTotalWin;
            totalWin = spinResult.GameResult.TotalWin;
            winLevel = spinResult.GameResult.WinLevel;

            currentWin = displayTotalWin - displayCurrentWin;


            //Debug.LogError($"=======Server currentWin:{currentWin}");

#if !PRODUCTION_PACKAGE
            XDebug.Log("Server:UpdateStateOnReceiveSpinResult\n"
                       + $"Server:DisplayCurrentWin:{displayCurrentWin}\n"
                       + $"Server:DisplayTotalWin:{displayTotalWin}\n"
                       + $"Server:WinLevel:{winLevel}\n"
                       + $"Server:TotalWin:{totalWin}\n"
                       + $"wheelWin:{wheelWin}");
#endif
        }
    }
}