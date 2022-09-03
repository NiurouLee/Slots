using UnityEngine;

namespace GameModule
{
    public class ElementScatter11013: Element
    {
        public ElementScatter11013(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }

        public override bool HasAnimState(string stateName)
        {
            if (stateName == "Win")
            {
                return false;
            }

            return base.HasAnimState(stateName);
        }
    }
}