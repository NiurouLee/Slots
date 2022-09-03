// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/10/15:09
// Ver : 1.0.0
// Description : IrregularWheel.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    //非规则轮盘
    public class IrregularWheel : Wheel
    {
        //每一列有几个元素
        protected List<int> rowCountInEachRoll;
        //每一列的位置
        protected List<Vector3> rollsPosition;
        //每个元素格子的大小[加上边界线的宽度]
        protected Vector2 elementContainerSize;
         
        public IrregularWheel(Transform transform) : base(transform)
        {
            
        }
        
        public void SetWheelExtraConfig(List<int> inRowCountInEachRoll, List<Vector3> inRollsPosition,
            Vector2 inElementContainerSize)
        {
            rowCountInEachRoll = inRowCountInEachRoll;
            rollsPosition = inRollsPosition;
            elementContainerSize = inElementContainerSize;
        }
        
        public override int GetRollRowCount(int rollIndex, WheelConfig wheelConfig)
        {
            return rowCountInEachRoll[rollIndex];
        }
        
        protected override Vector2 GetRollContentSize(int rollIndex)
        {
            return new Vector2(elementContainerSize.x, elementContainerSize.y * rowCountInEachRoll[rollIndex]);
        }

        public enum ElementAlignType
        {
            AlignTop,
            AlignBottom,
            AlignMiddle,
        }
        
        public static List<Vector3> GetIrregularRollPosition(int rollCount, List<int> inRowCountInEachRoll, Vector2 inElementContainerSize, ElementAlignType alignType)
        {
            List<Vector3> rollsPosition = new List<Vector3>(rollCount);

            int maxRowCount = inRowCountInEachRoll[0];
           
            for (var i = 1; i < rollCount; i++)
            {
                if (inRowCountInEachRoll[i] > maxRowCount)
                {
                    maxRowCount = inRowCountInEachRoll[i];
                }
            }
            
            for (var i = 0; i < rollCount; i++)
            {
                var centerRoll = (rollCount - 1) * 0.5f;
               
                float posY = 0;
                switch (alignType)
                {
                    case ElementAlignType.AlignTop:
                        posY = ((maxRowCount - inRowCountInEachRoll[i]) * 0.5f) * inElementContainerSize.y; 
                        break;
                    case ElementAlignType.AlignBottom:
                        posY = -((maxRowCount - inRowCountInEachRoll[i]) * 0.5f) * inElementContainerSize.y; 
                        break;
                    case ElementAlignType.AlignMiddle:
                        break;
                }
                rollsPosition.Add(new Vector3((i - centerRoll) * inElementContainerSize.x, posY, 0));
                // if (i == 4)
                // { 
                //     rollsPosition.Add(new Vector3((i - centerRoll) * inElementContainerSize.x+0.1f, posY, 0));
                // }
                //
                // if (i == 1 || i == 2)
                // {
                //     rollsPosition.Add(new Vector3((i - centerRoll) * inElementContainerSize.x, posY, 0));
                // }
                // if (i == 3)
                // {
                //        rollsPosition.Add(new Vector3((i - centerRoll) * inElementContainerSize.x+0.03f, posY, 0));
                // }
                // if (i == 0)
                // {
                //        rollsPosition.Add(new Vector3((i - centerRoll) * inElementContainerSize.x-0.09f, posY, 0));
                // }
                
            }

            return rollsPosition;
        }
        
        protected override Vector3 GetRollPosition(int rollIndex)
        {
            return rollsPosition[rollIndex];
        }
    }
}