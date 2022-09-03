using UnityEngine;

namespace GameModule
{
    public class ElementScatter11021: Element
    {
        public ElementScatter11021(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
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