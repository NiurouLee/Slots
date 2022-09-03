using UnityEngine;

namespace GameModule
{
    public class Element11026 : Element
    {
        private  MaterialPropertyBlock blockLeft = null;
        private  MaterialPropertyBlock blockMiddle = null;
        private  MaterialPropertyBlock blockRight = null;
        public Element11026(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {

        }

        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11026>();
            UpdateShowGrayLayer(activeState.IsInLink);
        }
        
        public MaterialPropertyBlock GetMaterialBlock()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11026>();
            var listWheels = activeState.GetRunningWheel();
        
            for (int i = 0; i < listWheels.Count; i++)
            {
                var wheel = listWheels[i];
        
                if (wheel.wheelName == "WheelLinkGame1")
                {
                    if (transform.IsChildOf(wheel.transform))
                    {
                        return blockLeft;
                    }
                }
                else if (wheel.wheelName == "WheelLinkGame6")
                {
                    if (transform.IsChildOf(wheel.transform))
                    {
                        return blockRight;
                    }
                }
            }
        
            return blockMiddle;
        }
        
        
        protected override void UpdateShowGrayLayer(bool isEnable)
        {
            if (objGary != null)
            {
             
                SpriteRenderer spriteRendererOrigin = transform.GetComponent<SpriteRenderer>();
                spriteRendererOrigin.enabled = !isEnable;
                if (isEnable)
                {
                    SpriteRenderer spriteRenderer = objGary.GetComponent<SpriteRenderer>();
                    spriteRenderer.SetPropertyBlock(GetMaterialBlock());
                    objGary.SetActive(false);
                }
                
                objGary.SetActive(isEnable);
            }
        }
        
        protected override void InitGary()
        {
            if (isStaticElement && objGary==null)
            {
                var garyPram = sequenceElement.config.staticGrayLayerPrams;
                if (garyPram != EmptyGary)
                {
                    objGary = GameObject.Instantiate(transform.gameObject, transform);
                    objGary.name = "GrayLayer";
                    objGary.transform.localPosition = Vector3.zero;
                    objGary.transform.localEulerAngles = Vector3.zero;
                    SpriteRenderer spriteRenderer = objGary.GetComponent<SpriteRenderer>();
                    spriteRenderer.sharedMaterial = sequenceElement.machineContext.assetProvider.GetAsset<Material>("MatHueAdjust");

                    if (blockLeft == null)
                    {
                        blockLeft = new MaterialPropertyBlock();
                        blockMiddle = new MaterialPropertyBlock();
                        blockRight = new MaterialPropertyBlock();

                        blockLeft.SetFloat("_HueValue", 0.618f);
                        blockLeft.SetFloat("_LightValue", 0.4f);
                        blockLeft.SetFloat("_SaturateValue", 1);
                        blockMiddle.SetFloat("_HueValue", 0.3f);
                        blockMiddle.SetFloat("_LightValue", 0.25f);
                        blockMiddle.SetFloat("_SaturateValue", 1);
                        blockRight.SetFloat("_HueValue", 0.8f);
                        blockRight.SetFloat("_LightValue", 0.4f);
                        blockRight.SetFloat("_SaturateValue", 1);
                    }
                }
            }
        }
    }
}