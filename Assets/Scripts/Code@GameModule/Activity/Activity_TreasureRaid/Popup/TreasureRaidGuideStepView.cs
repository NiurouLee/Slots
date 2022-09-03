using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    public class TreasureRaidGuideStepView:View
    {
        [ComponentBinder("Mask")]
        Transform mask;
        [ComponentBinder("CountText")]
        Text countText;

        public Action handler;

        private List<Component> _components;

        public TreasureRaidGuideStepView(string address)
            : base(address)
        {
         
        }

        private float blockStartTime = 0;
        public void SetGuideClickHandler(Action inHandler, List<Component> components = null)
        {
            handler = inHandler;
            blockStartTime = Time.realtimeSinceStartup;
            
            var pointerEventCustomHandler = mask.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnMaskClicked);
            if (countText != null)
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                    return;
                
                countText.SetText("3");
            }

            _components = components;
        }

        private void DestroyAddComponents()
        {
            if (_components == null)
                return;

            foreach (var component in _components)
            {
                if (component.gameObject == null)
                    continue;

                GameObject.DestroyImmediate(component);
            }
        }

        public override void Destroy()
        {
            DestroyAddComponents();
            base.Destroy();
        }

        private bool inResponse = false;
        public void OnMaskClicked(PointerEventData pd)
        {
            if(inResponse) 
                return;

            var deltaTime = Time.realtimeSinceStartup - blockStartTime;

            if (deltaTime < 1)
            {
                return;
            }
            
            inResponse = true;
            handler?.Invoke();
            Destroy();
        }
    }
}