using UnityEngine;

namespace GameModule
{
    public class ElementCoin11026: Element
    {
        
        [ComponentBinder("Sprite")]
        protected Transform spriteCoin;
        
        public ElementCoin11026(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);


            if (Constant11026.ListBonusAllElementIds.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    var coinText = chips.GetAbbreviationFormat(1);

                    var commaIndex = coinText.IndexOf('.');
                    if (commaIndex >= 2)
                    {
                        coinText = coinText.Remove(commaIndex, 2);
                    }

                    spriteCoin.GetComponent<TextMesh>().text = coinText;
                }
                //
                // if (!this.isStaticElement)
                // {
                //     MeshRenderer meshRenderer = spriteCoin.GetComponent<MeshRenderer>();
                //     meshRenderer.material.SetFloat("_StencilComp",8);
                // }
            }
        }

        public void UpdateWinChipsWithMultiplier(uint multiplier)
        {
            if (Constant11026.ListBonusAllElementIds.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    var coinText = (multiplier * chips).GetAbbreviationFormat(1);

                    var commaIndex = coinText.IndexOf('.');
                    if (commaIndex >= 2)
                    {
                       coinText = coinText.Remove(commaIndex, 2);
                    }

                    spriteCoin.GetComponent<TextMesh>().text = coinText;
                }
                //
                // if (!this.isStaticElement)
                // {
                //     MeshRenderer meshRenderer = spriteCoin.GetComponent<MeshRenderer>();
                //     meshRenderer.material.SetFloat("_StencilComp",8);
                // }
            }
        }
    }
}