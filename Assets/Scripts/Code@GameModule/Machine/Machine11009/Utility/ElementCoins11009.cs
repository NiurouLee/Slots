using UnityEngine;

namespace GameModule.Utility
{
    public class ElementCoins11009: Element
    {
        public ElementCoins11009(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
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