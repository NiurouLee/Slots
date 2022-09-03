using UnityEngine;

namespace GameModule
{
    public class ElementCoin11029: Element
    {
        
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoin;
        
        public ElementCoin11029(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            if (Constant11029.ListJSymbolElementIds.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    var coinText = chips.GetAbbreviationFormat(1);
                    var commaIndex = coinText.IndexOf('.');
                    if (commaIndex >= 3)
                    {
                        // coinText = coinText.Remove(commaIndex, 3);
                        coinText = chips.GetAbbreviationFormat(0);
                    }
                    // txtCoin.text = chips.GetAbbreviationFormat();
                    txtCoin.text = coinText;
                }

                // if (!this.isStaticElement)
                // {
                //     MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                //     meshRenderer.material.SetFloat("_StencilComp",8);
                // }
            }
        }
    }
}