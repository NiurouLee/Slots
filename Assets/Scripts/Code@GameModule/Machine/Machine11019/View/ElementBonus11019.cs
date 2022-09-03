using UnityEngine;

namespace GameModule
{
    public class ElementBonus11019: Element
    {
        
        [ComponentBinder("IntegralText")]
        protected TextMesh txtCoin;

        [ComponentBinder("SpecialIntegralText")]
        protected TextMesh txtSuperCoin;
        
        public ElementBonus11019(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            

            if (Constant11019.ListBonusElementIds.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    txtCoin.text = chips.GetAbbreviationFormat();
                    txtSuperCoin.text = chips.GetAbbreviationFormat();
                    if (winRate < 500)
                    {
                        txtCoin.gameObject.SetActive(true);
                        txtSuperCoin.gameObject.SetActive(false);
                    }
                    else
                    {
                        txtCoin.gameObject.SetActive(false);
                        txtSuperCoin.gameObject.SetActive(true);
                    }
                }

                if (!this.isStaticElement)
                {
                    MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetFloat("_StencilComp",8);
                    MeshRenderer meshSuperRenderer = txtSuperCoin.GetComponent<MeshRenderer>();
                    meshSuperRenderer.material.SetFloat("_StencilComp",8);
                }
            }
            
            

        }
        
    }
}