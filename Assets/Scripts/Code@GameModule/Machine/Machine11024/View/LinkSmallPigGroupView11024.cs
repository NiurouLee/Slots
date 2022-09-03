using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{
    public class LinkSmallPigGroupView11024:LinkBigPigGroupView11024
    {
        public LinkSmallPigGroupView11024(Transform inTransform):base(inTransform)
        {
        }

        public override void InitPigGroupList()
        {
            {
                string pigGroupType = "2";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
            {
                string pigGroupType = "23";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
            {
                string pigGroupType = "12";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
            {
                string pigGroupType = "123";
                pigGroupList[pigGroupType] = transform.Find("Pig" + pigGroupType + "Group");
            }
        }
    }
}