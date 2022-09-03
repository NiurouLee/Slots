// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:05 PM
// Ver : 1.0.0
// Description : ReSpinView.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class ReSpinRoll
    {
        private bool isActive;
        private bool[] elementLockStatus;
        private bool hideMaskWhenLocked;

        private SoloRoll[] soloRolls;

        private Roll roll;
        private Transform transform;

        public const int MaxRowCountPreRoll = 15;

        private Transform reSpinWheelTransform;
        
        protected int soloRollCount;
      
        public ReSpinRoll(Roll inRoll, bool driveByNormalRoll)
        {
            roll  = inRoll;
            BuildReSpinView(driveByNormalRoll);
        }

        public void BuildReSpinView(bool driveByNormalRoll)
        {
            reSpinWheelTransform = roll.transform.parent.parent.Find("ReSpinWheel");

            if (reSpinWheelTransform)
            {
                var soloRollTemplate = reSpinWheelTransform.Find("SoloRoll");

                if (!soloRollTemplate)
                {
                    return;
                }

                var rowCount = roll.rowCount;
                elementLockStatus = new bool[rowCount];
                soloRolls = new SoloRoll[rowCount];
                soloRollCount = rowCount;
                
                for (var i = 0; i < rowCount; i++)
                {
                    var soloRollTransform = GameObject.Instantiate(soloRollTemplate.gameObject).transform;

                    var soloRoll = new SoloRoll(soloRollTransform,false, false,"SoloElement");
                    
                    soloRolls[i] = soloRoll;
                    
                    var soloRollContentSize = roll.GetContentSize();
                    soloRollContentSize.y /= rowCount;

                    soloRoll.BuildRoll(roll.rollIndex, 1, soloRollContentSize, roll.elementSupplier, GetRollBuildingExtraConfig());

                    soloRollTransform.SetParent(reSpinWheelTransform, false);
                    soloRollTransform.localPosition =
                        reSpinWheelTransform.InverseTransformPoint(roll.GetContainer(i).transform.position);
                }

                if (driveByNormalRoll)
                {
                  //  roll.AttachRollToDrive(this);
                }
            }
        }
        public virtual RollBuildingExtraConfig GetRollBuildingExtraConfig()
        {
            RollBuildingExtraConfig extraConfig = new RollBuildingExtraConfig();
            extraConfig.elementMaxHeight = 1;
            extraConfig.extraTopElementCount = 0;
            return extraConfig;
        }
        public void DetachFromNormalRoll()
        {
          //  roll.AttachRollToDrive(null);
        }
        public void SetActive(bool inActive)
        {
            isActive = inActive && reSpinWheelTransform != null;
         
            if (reSpinWheelTransform != null)
                reSpinWheelTransform.gameObject.SetActive(inActive);
        }

        public void DoShift(float shiftAmount)
        {
            if (isActive)
            {
                for (var i = 0; i < soloRollCount; i++)
                {
                    if(!elementLockStatus[i])
                        soloRolls[i].DoShift(shiftAmount);
                }
            }
        }

        public void LockRow(int rowIndex, bool locked = true, bool hideMask = true)
        {
            elementLockStatus[rowIndex] = locked;
            soloRolls[rowIndex].EnableSoloRollMask(hideMask);
        }

        public void ShiftOneRow(int currentIndex)
        {
            if (isActive)
            {
                for (var i = 0; i < soloRollCount; i++)
                {
                    if (!elementLockStatus[i])
                        soloRolls[i].ShiftOneRow(currentIndex);
                }
            }
        }

        public SoloRoll GetSoloRoll(int rowIndex)
        {
            return soloRolls[rowIndex];
        }
        
        public void RemoveElements()
        {
            for (var i = 0; i < soloRollCount; i++)
            {
                if (!elementLockStatus[i])
                    soloRolls[i].RemoveElements();
            }
        }
    }
}