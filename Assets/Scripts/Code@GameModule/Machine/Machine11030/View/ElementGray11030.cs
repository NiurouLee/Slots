using UnityEngine;

namespace GameModule
{
    public class ElementGray11030:Element
    {
        public ElementGray11030(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
        
        }
        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11030>();
            var extraState = sequenceElement.machineContext.state.Get<ExtraState11030>();
            
            // UpdateShowGrayLayer(extraState.IsInTrain() && extraState.IsTrainFromChoose());
            UpdateShowGrayLayer(activeState.IsChooseTrainWheel);
            // UpdateShowGrayLayer(false);
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
                    // if (matGary == null)
                    // {
                    //     matGary = new Material(Shader.Find("HueAdjust"));
                    // }

                    spriteRenderer.sharedMaterial = sequenceElement.machineContext.assetProvider.GetAsset<Material>("LinkDrakMaterial");
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    // block.SetFloat("_ShiftHue",1f);
                    block.SetFloat("_HueValue",garyPram.Item1);
                    block.SetFloat("_LightValue",garyPram.Item2);
                    block.SetFloat("_SaturateValue",garyPram.Item3);
                    spriteRenderer.SetPropertyBlock(block);
                }
            }
        }
        protected override void UpdateShowGrayLayer(bool isEnable)
        {
            if (objGary!=null)
            {
                objGary.SetActive(isEnable);
                SpriteRenderer spriteRendererOrigin = transform.GetComponent<SpriteRenderer>();
                spriteRendererOrigin.enabled = !isEnable;
            }
        }
    }
}