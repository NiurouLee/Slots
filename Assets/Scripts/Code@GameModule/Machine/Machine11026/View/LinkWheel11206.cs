// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/03/20:46
// Ver : 1.0.0
// Description : LinkWheel11206.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class LinkWheel11206 : IndependentWheel
    {
        public LinkWheel11206(Transform transform) : base(transform)
        {
        
        }

        public void RefreshWheelInitElement()
        {
            List<SequenceElement> sequenceElements = null;

            if (wheelState is WheelStateLeft11026 leftState)
            {
                sequenceElements = leftState.GetLinkSequenceElementsOnWheel();
            }

            else if (wheelState is WheelStateCenter11026 centerState)
            {
                sequenceElements = centerState.GetLinkSequenceElementsOnWheel();
            }
            else if (wheelState is WheelStateRight11026 rightState)
            {
                sequenceElements = rightState.GetLinkSequenceElementsOnWheel();
            }

            if (sequenceElements != null)
            {
                for (var i = 0; i < sequenceElements.Count; i++)
                {
                    var container = this.GetRoll(i).GetVisibleContainer(0);
                    container.UpdateElement(sequenceElements[i], false);
                     
                    var hideContainer = GetRoll(i).GetContainer(0);
                     
                    var symbolId = Constant11026.ListLinkLoLevelElementIds[0];
                    var elementConfig = context.machineConfig.GetElementConfigSet()
                        .GetElementConfig(symbolId);
                    
                    hideContainer.UpdateElement(new SequenceElement(elementConfig, context));
                    
                    if (Constant11026.ListBonusAllElementIds.Contains(sequenceElements[i].config.id))
                    {
                        container.ShiftSortOrder(true);
                        container.PlayElementAnimation("Idle");
                    }
                }
            }
        }
    }
}