using UnityEngine;

namespace GameModule
{
    public class RollBuildingExtraConfig
    {

        public int elementMaxHeight;

        public int extraTopElementCount;

        public RollBuildingExtraConfig(int inElementMaxHeight=1,int inExtraTopElementCount=0)
        {
            elementMaxHeight = inElementMaxHeight;
            extraTopElementCount = inExtraTopElementCount;
        }
    }
}