// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/14/16:34
// Ver : 1.0.0
// Description : TimeBonusSuperWheelView.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class TimeBonusSuperWheelView:TimeBonusWheelView
    {
        public int wheelIndex;

        public void SetWheelIndex(int inWheelIndex)
        {
            wheelIndex = inWheelIndex;
        }

        public void ToState(string state)
        {
            animator.Play(state);
        }
    }
}