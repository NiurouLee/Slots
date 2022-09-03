using UnityEngine;

namespace GameModule
{
    public class GrayElement11024:Element
    {
        public GrayElement11024(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
        
        }

        public static readonly Color MultiBlendModeEmptyGary = new Color(0, 0, 0, 0);
        protected override void UpdateShowGrayLayer()
        {
            var activeState = sequenceElement.machineContext.state.Get<WheelsActiveState11024>();
            UpdateShowGrayLayer(activeState.gameType == GameType11024.Link);
        }
        protected override void InitGary()
        {
            if (isStaticElement && objGary==null)
            {

                var garyPram = sequenceElement.config.staticGrayLayerMultiBlendModePrams;
                if (garyPram != MultiBlendModeEmptyGary)
                {
                    objGary = GameObject.Instantiate(transform.GetChild(0).gameObject, transform);
                    objGary.name = "GrayLayer";
                    objGary.transform.localPosition = Vector3.zero;
                    objGary.transform.localEulerAngles = Vector3.zero;

                    
                    SpriteRenderer spriteRenderer = objGary.GetComponent<SpriteRenderer>();
                    // if (matGary == null)
                    // {
                    //     matGary = new Material(Shader.Find("HueAdjust"));
                    // }
                    spriteRenderer.sharedMaterial = sequenceElement.machineContext.assetProvider.GetAsset<Material>("LinkDarkMaterial");
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    // block.SetFloat("_ShiftHue",1f);
                    block.SetFloat("_R",garyPram.r);
                    block.SetFloat("_G",garyPram.g);
                    block.SetFloat("_B",garyPram.b);
                    block.SetFloat("_A",garyPram.a);
                    spriteRenderer.SetPropertyBlock(block);
                }
            }
        }
        protected override void UpdateShowGrayLayer(bool isEnable)
        {
            if (objGary!=null)
            {
                objGary.SetActive(isEnable);
                SpriteRenderer spriteRendererOrigin = transform.GetChild(0).GetComponent<SpriteRenderer>();
                spriteRendererOrigin.enabled = !isEnable;
            }
        }
    }
}