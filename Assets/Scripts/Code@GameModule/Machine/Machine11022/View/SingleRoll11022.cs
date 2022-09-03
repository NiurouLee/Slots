using UnityEngine;

namespace GameModule
{
    public class SingleRoll11022:Roll
    {
        public SingleRoll11022(Transform inTransform, bool inTopRowHasHighSortOrder, bool inLeftColHasHighSortOrder,
            string inElementSortLayerName)
            : base(inTransform, inTopRowHasHighSortOrder, inLeftColHasHighSortOrder, inElementSortLayerName)
        {
            elementSortLayerName = "SoloElement";
        }
    }
}