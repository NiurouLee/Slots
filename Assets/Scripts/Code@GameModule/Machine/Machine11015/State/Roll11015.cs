using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class Roll11015: Roll
    {
        //protected SpriteMask rollMask;
        public Roll11015(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
            
        }


        public override void BuildRoll(int inRollIndex, int inRowCount, Vector2 inContentSize, IElementSupplier inElementSupplier,
            RollBuildingExtraConfig extraConfig)
        {
            base.BuildRoll(inRollIndex, inRowCount, inContentSize, inElementSupplier, extraConfig);
            //rollMask = this.transform.parent.Find($"RollMask{inRollIndex}").GetComponent<SpriteMask>();
            
        }

        public override ElementContainer GetVisibleContainer(int index)
        {
            var extraState = context?.state.Get<ExtraState11015>();
            if (extraState!=null && extraState.HasWildReel(this.rollIndex))
            {
                index = index + 4;
            }
            else
            {
                index = index + 8;
            }

            
            return base.GetVisibleContainer(index);
        }

        public override ElementContainer GetWinLineContainer(int index)
        {

            bool hasMultipleElement = false;
            var listElement = GetVisibleSequenceElement();
            for (int i = 0; i < listElement.Count; i++)
            {
                if (Constant11015.ListWildMultipleElementId.Contains(listElement[i].config.id))
                {
                    hasMultipleElement = true;
                    break;
                }
            }
            
            
            var extraState = context?.state.Get<ExtraState11015>();
            if ( hasMultipleElement || (extraState!=null && extraState.HasWildReel(this.rollIndex)))
            {
                index = index + 4;
            }
            else
            {
                index = index + 8;
            }
            
            return base.GetWinLineContainer(index);
        }
        
        public override ElementContainer GetWinFrameContainer(int index)
        {

            bool hasMultipleElement = false;
            var listElement = GetVisibleSequenceElement();
            for (int i = 0; i < listElement.Count; i++)
            {
                if (Constant11015.ListWildMultipleElementId.Contains(listElement[i].config.id))
                {
                    hasMultipleElement = true;
                    break;
                }
            }
            
            
            var extraState = context?.state.Get<ExtraState11015>();
            if ( hasMultipleElement || (extraState!=null && extraState.HasWildReel(this.rollIndex)))
            {
                index = index + 4;
            }
            else
            {
                index = index + 8;
            }
            
            return base.GetWinLineContainer(index);
        }


        public override Vector3 GetVisibleContainerPosition(int index)
        {
            var extraState = context?.state.Get<ExtraState11015>();
            if (extraState!=null && extraState.HasWildReel(this.rollIndex))
            {
                index = index + 4;
            }
            else
            {
                index = index + 8;
            }
            return base.GetVisibleContainerPosition(index);
        }


        public override void ValidateSpinResult()
        {
            //base.ValidateSpinResult();
        }
        
        
        public override List<SequenceElement> GetVisibleSequenceElement()
        {
            var sequenceList = new List<SequenceElement>();
            for (int i = 0; i < 4; i++)
            {
                sequenceList.Add(GetVisibleContainer(i).sequenceElement);
            }
            
            return sequenceList;
        }
    }
}