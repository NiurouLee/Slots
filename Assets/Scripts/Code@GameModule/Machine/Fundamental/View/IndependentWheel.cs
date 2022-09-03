// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/13/16:32
// Ver : 1.0.0
// Description : IndependentWheel.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class IndependentWheel : Wheel
    {
        public IndependentWheel(Transform transform) : base(transform)
        {
            
        }

        public void SetSpinningStopByEachVisibleColumn(bool byColumn)
        {
            for (var i = 0; i < rollCount; i++)
            {
                rolls[i].SetSpinningStopByEachVisibleColumn(byColumn);
            }
        }
 
        protected override void InitializeWheelInfo(WheelConfig wheelConfig)
        {
            base.InitializeWheelInfo(wheelConfig);
            
            rollCount = wheelConfig.rollCount * wheelConfig.rollRowCount;
            rollContentSize.y = rollContentSize.y / wheelConfig.rollRowCount;
            
            wheelMask.gameObject.SetActive(false);
        }

        protected override int GetRollStopIndex(int rollIndex)
        {
            var wheelConfig = wheelState.GetWheelConfig();
            int column = rollIndex / wheelConfig.rollRowCount;
            int row = rollIndex % wheelConfig.rollRowCount;
            return column * wheelConfig.rollRowCount + row;
        }
        
        public override ElementContainer GetWinLineElementContainer(int rollIndex, int rowIndex)
        {
            // var wheelConfig = wheelState.GetWheelConfig();
            // var actualIndex = rollIndex *  wheelConfig.rollRowCount + rowIndex;
            //
            Roll roll = GetRoll(rollIndex);
            return roll?.GetVisibleContainer(0);
        }
        public override ElementContainer GetWinFrameElementContainer(int rollIndex, int rowIndex)
        {
            // var wheelConfig = wheelState.GetWheelConfig();
            // var actualIndex = rollIndex *  wheelConfig.rollRowCount + rowIndex;
            //
            Roll roll = GetRoll(rollIndex);
            return roll?.GetVisibleContainer(0);
        }
 
        protected override Vector3 GetRollPosition(int rollIndex)
        {
            var wheelConfig = wheelState.GetWheelConfig();
            var centerRoll = (wheelConfig.rollCount - 1) * 0.5f;
            int column = rollIndex / wheelConfig.rollRowCount;
            int row = rollIndex % wheelConfig.rollRowCount;
            return new Vector3((column - centerRoll) * rollContentSize.x,
                ((wheelConfig.rollRowCount - row - 1) + 0.5f) * rollContentSize.y - contentHeight * 0.5f, 0);
        }
        
        public override Vector3 GetAnticipationAnimationPosition(int rollIndex)
        {
            var wheelConfig = wheelState.GetWheelConfig();
            var centerRoll = (wheelConfig.rollCount - 1) * 0.5f;
            int column = rollIndex / wheelConfig.rollRowCount;
            
            return new Vector3((column - centerRoll) * rollContentSize.x, 0, 0);
        }
    }
}