using System;
using UnityEngine;

namespace GameModule
{
    public class ElementZeus11015: Element
    {
        public ElementZeus11015(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
        }


        private bool isLock;

        public bool IsLock
        {
            get { return isLock; }
            set
            {
                isLock = value;
            }
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            IsLock = false;
            base.UpdateOnAttachToContainer(containerTransform, element);
        }


        public override void PlayAnimation(string animationName, bool maskByWheelMask, Action endCallback = null)
        {
            if (animationName == "Win" && IsLock)
            {
                animationName = "LockWin";
            }

            base.PlayAnimation(animationName, maskByWheelMask, endCallback);
        }
    }
}