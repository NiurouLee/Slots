using UnityEngine;

namespace GameModule
{
    public class ElementCoin11012: Element
    {
        
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoin;
        
        public ElementCoin11012(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);


            if (Constant11012.ListNormalCoins.Contains(sequenceElement.config.id) ||
                Constant11012.ListDoorCoins.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    txtCoin.text = chips.GetAbbreviationFormat();
                }

                if (!this.isStaticElement)
                {
                    MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetFloat("_StencilComp",8);
                }
            }

        }
    }
}