using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class BetState11005: BetState
    {
        public BetState11005(MachineState state) : base(state)
        {
        }

        protected List<ulong> listGameUnlockConfig;
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            base.UpdateStateOnRoomSetUp(gameEnterInfo);
            gameUnlockConfig = gameEnterInfo.GameConfigs[0].Unlocks;
            InitGameUnlockConfig();

        }

        public void InitGameUnlockConfig()
        {
            listGameUnlockConfig = new List<ulong>();
            var unlock = machineState.machineContext.serviceProvider.GetAvailableUnlockFeature(gameUnlockConfig);
            for (int i = 0; i < unlock.Count; i++)
            {
                listGameUnlockConfig.Add(unlock[i]);
            }
        }
        

        public bool GetMoreLetter()
        {
            int nowLetterNum = GetUnlockLetterNum(totalBet);
            int nowIndex = Constant11005.listUnlockLetterNum.IndexOf(nowLetterNum);

            if (nowIndex != Constant11005.listUnlockLetterNum.Count - 1)
            {
                ulong moreBet = listGameUnlockConfig[nowIndex];
                SetTotalBet(moreBet,true);
                return true;
            }

            return false;

        }

        public int GetUnlockLetterNum(ulong numBet)
        {
            for (int i = 0; i < listGameUnlockConfig.Count; i++)
            {
                if (numBet < listGameUnlockConfig[i])
                {
                    return Constant11005.listUnlockLetterNum[i];
                }
            }

            return Constant11005.listUnlockLetterNum[listGameUnlockConfig.Count];
        }
    }
}