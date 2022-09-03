using System;
using UnityEngine;

namespace GameModule
{
    public class ElementCoin11024:Element
    {
        private WheelsActiveState11024 _activeWheelState;
        public WheelsActiveState11024 activeWheelState
        {
            get
            {
                if (_activeWheelState == null)
                {
                    _activeWheelState =  sequenceElement.machineContext.state.Get<WheelsActiveState11024>();
                }
                return _activeWheelState;
            }
        }
        public ElementCoin11024(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }
        

        public virtual void UpdateElementContent()
        {
            if (activeWheelState.gameType == GameType11024.Link)
            {
                var localScale = Vector3.one;
                localScale *= 0.95f;
                transform.localScale = localScale;
            }
            else
            {
                transform.localScale = Vector3.one;
            }
        }
        public override void DoRecycle()
        {
            // transform.gameObject.SetActive(true);
            transform.localScale = Vector3.one;
            base.DoRecycle();
        }
    }
}