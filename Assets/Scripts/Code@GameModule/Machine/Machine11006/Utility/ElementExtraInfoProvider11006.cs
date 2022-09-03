using System;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class ElementExtraInfoProvider11006: ElementExtraInfoProvider
    {
       
        public override Type GetElementClassType(uint id)
        {
            if (Constant11006.listBaseMultiplierElements.Contains(id)||
                Constant11006.listFreeMultiplierElements.Contains(id))
            {
                return typeof(MultiplierElement11006);
            }

            return typeof(Element);
        }
        
        
        public override int GetElementSortingOffset(uint id)
        {
            if (Constant11006.scatterElement == id)
            {
                return 12;
            }

            return 0;
        }

        public override int GetElementActiveSortingOffset(uint id)
        {
            if (Constant11006.scatterElement == id)
            {
                return 312;
            }

            return 300;
        }
    }
}