//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-17 15:47
//  Ver : 1.0.0
//  Description : BonusWheelState11007.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class BonusWheelState11007: WheelState
    {
        //当前Bonus的ItemId
        private uint _currentItemId;
        public uint CurrentItemId
        {
            get => _currentItemId;
            set => _currentItemId = value;
        }
        private uint _currentBonusReelId;
        public uint CurrentBonusReelId
        {
            get => _currentBonusReelId;
            set => _currentBonusReelId = value;
        }
        private uint _currentBonusElementId;
        public uint CurrentBonusElementId
        {
            get => _currentBonusElementId;
            set => _currentBonusElementId = value;
        }

        private bool _hasPlayMultiAnim;
        public bool HasPlayMultiAnim 
        {
            get => _hasPlayMultiAnim;
            set => _hasPlayMultiAnim = value;
        }
        private Vector3[] arrayTwoWildPosition;
        public BonusWheelState11007(MachineState state) : base(state)
        {
            wheelIsActive = false;
            currentSequenceName = "Bonus1Reels";
            CurrentItemId = Constant11007.INVALID_ITEM_ID;
            arrayTwoWildPosition = new Vector3[2];
        }

        public void ResetBonusState()
        {
            CurrentBonusReelId = 0;
            CurrentBonusElementId = 0;
            CurrentItemId = Constant11007.INVALID_ITEM_ID;
        }

        public void SetBonusState(uint id, uint reelId, uint symbolId)
        {
           CurrentItemId = id;
           CurrentBonusReelId = reelId;
           CurrentBonusElementId = symbolId;
           HasPlayMultiAnim = false;
        }

        public bool IsBonusRandomFreeWild()
        {
            return machineState.Get<WheelsActiveState11007>().IsBonusWheel && Constant11007.ELEMENT_RANDOM_FREE_WILDS == CurrentBonusElementId;
        }
        public bool IsBonusMultiWin()
        {
            return machineState.Get<WheelsActiveState11007>().IsBonusWheel && Constant11007.ELEMENT_SPIN_MULTI_WIN == CurrentBonusElementId;
        }

        public bool IsBonusTwoWild()
        {
            return machineState.Get<WheelsActiveState11007>().IsBonusWheel && Constant11007.ELEMENT_RESPIN_UNTIL_WIN == CurrentBonusElementId;
        }

        public bool IsBonusSpinSameWin()
        {
            return machineState.Get<WheelsActiveState11007>().IsBonusWheel && Constant11007.ELEMENT_RESPIN_SAME_WIN == CurrentBonusElementId;
        }

        public uint GetBonusMultiply()
        {
            var items = machineState.Get<ExtraState11007>().Items;
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                if (item != null && item.Id == CurrentItemId)
                {
                    return item.Multiplier;
                }
            }
            return 0;
        }
        public ulong GetCoinElementWin()
        {
            var items = machineState.Get<ExtraState11007>().Items;
            for (int i = 0; i < items.count; i++)
            {
                var item = items[i];
                if (item != null && item.Id == CurrentItemId)
                {
                    return item.Win;
                }
            }
            return 0;
        }

        public void SetWorldPosition(int index, Vector3 vector3)
        {
            arrayTwoWildPosition[index] = new Vector3(vector3.x, vector3.y, vector3.z);
        }

        public Vector3 GetWorldPosition(int index)
        {
            var position = arrayTwoWildPosition[index];
            return new Vector3(position.x,position.y,position.z);
        }
        
        public void ResetSpinResultSequenceElement()
        {
            spinResultElementDef = null;
        }
    }
}