using System;
using UnityEngine;

namespace GameModule.Utility
{
    public class WheelAnimationController11009: WheelAnimationController
    {
        private Material matFont;
        public WheelAnimationController11009()
        {
            var machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;

            var tran = machineContext.assetProvider.GetAsset<GameObject>("Active_B03");
            matFont = tran.transform.Find("Coins/CountText").GetComponent<TextMesh>().font.material;
        }


        protected BoxesView11009 boxesView;
        
        public async override void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            //飞element

            var machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;

            if (boxesView == null)
            {
                boxesView = machineContext.view.Get<BoxesView11009>();
            }
            
            // var font = machineContext.assetProvider.GetAsset<Font>("Slot_11009_font_2");
            matFont.SetFloat("_StencilComp",8);
            

            var roll = wheel.GetRoll(rollIndex);
            int count = roll.rowCount;
            for (int i = 0; i < count; i++)
            {
               var container =  roll.GetVisibleContainer(i);
               
               
               uint id = container.sequenceElement.config.id;
               if (Constant11009.ListCollectElementId.Contains(id) ||
                   Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                   Constant11009.ListElementIdPurpleVaraint.Contains(id))
               {
                   container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.None);
               }
               
               await boxesView.CollectElement(container);
            }
            
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
            
        }
        
        
        
        public override void OnWheelStartSpinning()
        {

            var listContainer = this.wheel.GetElementMatchFilter((container) =>
            {
                uint id = container.sequenceElement.config.id;
                if (Constant11009.ListCollectElementId.Contains(id) ||
                    Constant11009.ListElementIdGoldenVaraint.Contains(id) ||
                    Constant11009.ListElementIdPurpleVaraint.Contains(id))
                {
                    return true;
                }

                return false;
            });
            
          
            for (int i = 0; i < listContainer.Count; i++)
            {
                var container =  listContainer[i];
                container.GetElement().UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);

            }
            
            // var machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;
            // var font = machineContext.assetProvider.GetAsset<Font>("Slot_11009_font_2");
            matFont.SetFloat("_StencilComp",2);
            
            base.OnWheelStartSpinning();
            
        }
    }
}