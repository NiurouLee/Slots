// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-24 7:36 PM
// Ver : 1.0.0
// Description : ReSpinWheel.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class ReSpinWheel
    {
        protected Wheel parentWheel;
        protected ReSpinRoll[] reSpinRolls;
        protected bool driveByParentWheel;

        public ReSpinWheel(Wheel inParentWheel, bool inDriveByParentWheel = true)
        {
            parentWheel = inParentWheel;
            driveByParentWheel = inDriveByParentWheel;
        }
        
        public void BuildReSpinWheel()
        {
            reSpinRolls = new ReSpinRoll[parentWheel.rollCount];

            for (var i = 0; i < reSpinRolls.Length; i++)
            {
                reSpinRolls[i] = new ReSpinRoll(parentWheel.GetRoll(i), driveByParentWheel);
            }
        }
        
        public void SetActive(bool inActive)
        {
            for (var i = 0; i < reSpinRolls.Length; i++)
            {
                reSpinRolls[i].SetActive(inActive);
            }
        }

        public SoloRoll GetSoloRoll(int rollIndex, int rowIndex)
        {
            return reSpinRolls[rollIndex].GetSoloRoll(rowIndex);
        }

        public void LockSoloRoll(int rollIndex, int rowIndex, bool locked = true, bool hideMask = true)
        {
            reSpinRolls[rollIndex].LockRow(rowIndex, locked, hideMask);
        }

        public ReSpinRoll GetReSpinRoll(int rollIndex)
        {
            return reSpinRolls[rollIndex];
        }
        
        public void DetachFromParentWheel()
        {
            for (var i = 0; i < reSpinRolls.Length; i++)
            {
                reSpinRolls[i].DetachFromNormalRoll();
            }
        }
        
        public void RemoveElements()
        {
            for (var i = 0; i < reSpinRolls.Length; i++)
            {
                reSpinRolls[i].RemoveElements();
            }
        }
    }
}