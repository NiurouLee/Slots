using UnityEngine;

namespace GameModule
{
    public class MultiplierElement11006: Element
    {
        public MultiplierElement11006(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }

        public override bool HasAnimState(string stateName)
        {
            if (stateName == "Win")
            {
                return false;
            }

            return true;
        }
    }
}