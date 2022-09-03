using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class ElementCoin11312 : Element
    {
        [ComponentBinder("IntegralText")]
        public TextMesh txtCoin;
        public ElementCoin11312(Transform transform, bool inIsStatic) : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);

            UpdateElementContent();
        }

        public void UpdateElementContent()
        {
               var extraState = sequenceElement.machineContext.state.Get<ExtraState11312>();
            if (Constant11312.ListCoinElementIds.Contains(sequenceElement.config.id))
            {
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
               
                var respinState = sequenceElement.machineContext.state.Get<ReSpinState11312>();
                if(respinState.IsInRespin && respinState.ReSpinCount<respinState.ReSpinLimit && extraState.AddedToReels>0 
                    && Constant11312.ListCoinGoldElementIds.Contains(sequenceElement.config.id) && !IsRespinReconnection(respinState)){
                    winRate = extraState.AddedToReels;
                }
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    txtCoin.text = chips.GetAbbreviationFormat(1);
                }

                if (!this.isStaticElement)
                {
                    MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetFloat("_StencilComp",8);
                }
            }
            if(Constant11312.AllListSmallBlueCoinElementId.Contains(sequenceElement.config.id)){
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                
                if (chips > 0)
                {
                    txtCoin.text ="+"+ chips.GetAbbreviationFormat(1);
                }

                if (!this.isStaticElement)
                {
                    MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetFloat("_StencilComp",8);
                }
            }
             if(Constant11312.AllListSmallGoldCoinElementId.Contains(sequenceElement.config.id)){
                ulong winRate = sequenceElement.config.GetExtra<ulong>("winRate");
                ulong winPay = 0;
                if(extraState.YellowCoinTotalWinRate!=0)
                    winPay = extraState.YellowCoinTotalWinRate * (winRate/100);
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winPay);
                
                if (chips > 0)
                {
                    txtCoin.text ="+"+ chips.GetAbbreviationFormat(1);
                }

                if (!this.isStaticElement)
                {
                    MeshRenderer meshRenderer = txtCoin.GetComponent<MeshRenderer>();
                    meshRenderer.material.SetFloat("_StencilComp",8);
                }
            }
            // 凡是mini和minor的jackpot需要重新设置一下elementPool里这个图标的状态
            if(Constant11312.ListCoinElementIdsJackot.Contains(sequenceElement.config.id)){
                for (int x = 1; x < 5; x++)
                {
                    if (x == 1)
                        transform.Find("AnimRoot/JPGroup/JPSprite" + x).gameObject.SetActive(true);
                    else
                        transform.Find("AnimRoot/JPGroup/JPSprite" + x).gameObject.SetActive(false);
                }
            }
        }
        private bool IsRespinReconnection(ReSpinState11312 respinState){
            if(Constant11312.IsReconnection &&respinState.ReSpinCount==0)
                return true;
            return false;
        }
    }
}

