using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class LinkWheel11312 : Wheel
    {
        public LinkWheel11312(Transform transform) : base(transform)
        {
        }

        public void ForceUpdateElementSupplier(WheelState wheelState)
        {
            if (rollCount > 0)
            {
                for (var i = 0; i < rollCount; i++)
                {
                    rolls[i].elementSupplier.InitializeSupplier(wheelState,rolls[i]);
                    rolls[i].elementSupplier.GetStartIndex();
                }
            }
        }
    }
}

