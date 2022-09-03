using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelAnimationController11004: WheelAnimationController
    {
        public override void ShowAnticipationAnimation(int rollIndex)
        {
            base.ShowAnticipationAnimation(rollIndex);
            
            var anticipationAnimationGo = wheel.GetAttachedGameObject("AnticipationAnimation");
            if (anticipationAnimationGo != null)
            {
                var sortingGroup = anticipationAnimationGo.GetComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                sortingGroup.sortingOrder = 400;
            }


        }
    }
}