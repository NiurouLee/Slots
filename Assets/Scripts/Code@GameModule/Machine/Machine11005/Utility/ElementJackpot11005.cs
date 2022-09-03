using GameModule;
using UnityEngine;

namespace Machine.Machine11005.Utility
{
    public class ElementJackpot11005: Element
    {
        public ElementJackpot11005(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
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