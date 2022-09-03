using UnityEngine;

namespace GameModule
{
    public class FreeRoll11015: Roll
    {
        public FreeRoll11015(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder, string inElementSortLayerName) : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
        }
        
        
        public override void BuildRoll(int inRollIndex, int inRowCount, Vector2 inContentSize, IElementSupplier inElementSupplier,
            RollBuildingExtraConfig extraConfig)
        {
            base.BuildRoll(inRollIndex, inRowCount, inContentSize, inElementSupplier, extraConfig);
            for (int i = 0; i < containers.Length; i++)
            {
                var container = containers[i];
                container.transform.localScale = Constant11015.ElementFreeScale;
                
            }
        }
    }
}