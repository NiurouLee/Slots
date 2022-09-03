using UnityEngine;

namespace GameModule
{
    public class ElementStar11013: Element
    {
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoin;
        
        public ElementStar11013(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }


        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);


            var betState = sequenceElement.machineContext.state.Get<BetState>();
            ulong winRate = betState.totalBet;
            var chips = winRate / 100;
            if (chips > 0)
            {
                txtCoin.text = chips.GetAbbreviationFormat();
            }

            MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
            if (!this.isStaticElement)
            {

                meshRenderer.material.SetFloat("_StencilComp", 8);
            }
            else
            {
                meshRenderer.sharedMaterial.SetFloat("_StencilComp", 2);
            }




        }
    }
}