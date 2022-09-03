using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameModule
{
    public class SuperBonusInfoState11020 : SubState
    {
        private SuperBonusInfo superBonusInfo;
        private bool isBonusGame = false;

        public SuperBonusInfoState11020(MachineState state) 
			: base(state)
        {

        }
        
        public override void UpdateStateOnRoomSetUp(SEnterGame gameEnterInfo)
        {
            SetSuperBonusInfo(gameEnterInfo.GameResult.SuperBonusInfo, true);

            base.UpdateStateOnRoomSetUp(gameEnterInfo);

            isBonusGame = gameEnterInfo.GameResult.IsBonusGame;
            if (IsTriggered())
            {
                isBonusGame = true;
            }
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);

            var byteArray = MessageExtensions.ToByteString(spinResult.GameResult).ToByteArray();
            var str = System.Text.Encoding.Default.GetString(byteArray);
            Debug.Log(str);

            SetSuperBonusInfo(spinResult.GameResult.SuperBonusInfo, false);

            isBonusGame = spinResult.GameResult.IsBonusGame;
            if (IsTriggered())
            {
                isBonusGame = true;
            }
        }

        protected void SetSuperBonusInfo(SuperBonusInfo _superBonusInfo, bool isInit)
        {
            superBonusInfo = _superBonusInfo;
            
            if (isInit)
            {
                UpdateSuperBonusUI(isInit);
            }
        }

        public void UpdateSuperBonusUI(bool isInit)
        {
            var view = machineState.machineContext.view.Get<SuperBonusInfoView11020>();
            if (view != null)
            {
                view.UpdateSuperBonusInfo((int)GetProgress(), !isInit);
            }
        }

        public bool IsBonusGame()
        {
            return isBonusGame;
        }

        public bool IsTriggered()
        {
            return superBonusInfo != null ? superBonusInfo.Triggered : false;
        }

        public uint GetProgress()
        {
            return superBonusInfo != null  ? superBonusInfo.Progress : 0;
        }

        public ulong GetAvgBet()
        {
            return superBonusInfo != null  ? superBonusInfo.AvgBet : 0;
        }

        public ulong GetStartTime()
        {
            return superBonusInfo != null  ? superBonusInfo.StartTime : 0;
        }
    }
}