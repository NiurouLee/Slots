using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class StickyElementContainer11024:ElementContainer
    {
        private ElementConfigSet tempElementConfigSet;

        public PigGameResultExtraInfo.Types.LinkData.Types.Item nowStickyElementData;
        public StickyElementContainer11024(Transform transform, string sortingLayer):base(transform,sortingLayer)
        {
        }

        public int posX;
        public int posY;
        public void SetElementPosition(int rollIndex, int rowIndex)
        {
            posX = rollIndex;
            posY = rowIndex;
        }
        public void SetSortingOrder(int rollIndex, int rowIndex)
        {
            containerGroup.sortingLayerName = "Element";
            if (currentAttachElement != null)
            {
                currentAttachElement.UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);   
            }
            var positionId = rollIndex * 3 + rowIndex;
            // containerGroup.sortingOrder = 500 + 1 + rollIndex * 10 + rowIndex;
            containerGroup.sortingOrder = (positionId + 1) * 200 - 1;
        }
        public void SetSortingOrderBoost(int rollIndex, int rowIndex)
        {
            containerGroup.sortingLayerName = "SoloElement";
            currentAttachElement.UpdateMaskInteraction(SpriteMaskInteraction.None);
            containerGroup.sortingOrder = 2500 + 1 + rollIndex * 10 + rowIndex;
        }
        public void SetSortingOrderFly(int rollIndex, int rowIndex)
        {
            containerGroup.sortingLayerName = "SoloElement";
            currentAttachElement.UpdateMaskInteraction(SpriteMaskInteraction.None);
            containerGroup.sortingOrder = 500 + 1 + rollIndex * 10 + rowIndex;
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tempElementConfigSet = context.machineConfig.GetElementConfigSet();
        }
        public bool HasContainer()
        {
            return !ReferenceEquals(currentAttachElement, null);
        }

        public void SetStickyData(PigGameResultExtraInfo.Types.LinkData.Types.Item stickyElementData,bool force = true)
        {
            Show();
            SetStickyDataWithoutRefresh(stickyElementData);
            if (currentAttachElement == null)
            {
                var element = new SequenceElement(tempElementConfigSet.GetElementConfig(nowStickyElementData.SymbolId), context);
                UpdateElement(element,true);
                currentAttachElement.UpdateMaskInteraction(SpriteMaskInteraction.VisibleInsideMask);
                XUtility.PlayAnimation(GetElement().animator,"Idle");
            }
            UpdateToDataState(force);
        }

        public void SetStickyDataWithoutRefresh(PigGameResultExtraInfo.Types.LinkData.Types.Item stickyElementData)
        {
            nowStickyElementData = stickyElementData;
        }
        public void UpdateToDataState(bool afterAdd)
        {
            if (Constant11024.IsGoldId(nowStickyElementData.SymbolId))
            {
                if (afterAdd)
                {
                    if (Constant11024.IsGoldValue(nowStickyElementData.SymbolId))
                    {
                        ((ElementGoldValue11024) GetElement()).SetWinRate((long) nowStickyElementData.WinRate);   
                    }
                    else
                    {
                        ((ElementGoldJackpot11024) GetElement()).SetWinRate((long) nowStickyElementData.WinRate);  
                    }
                }
                else
                {
                    if (Constant11024.IsGoldValue(nowStickyElementData.SymbolId))
                    {
                        ((ElementGoldValue11024) GetElement()).SetWinRate(sequenceElement.config.GetExtra<int>("winRate"));      
                    }
                    else
                    {
                        ((ElementGoldJackpot11024) GetElement()).SetWinRate(0);  
                    }
                }
            }
        }

        public void JumpNum()
        {
            if (Constant11024.IsGoldValue(nowStickyElementData.SymbolId))
            {
                ((ElementGoldValue11024) GetElement()).JumpNum();   
            }
            else
            {
                ((ElementGoldJackpot11024) GetElement()).JumpNum();  
            }
        }

        public async Task ShowAddAnimation(ulong addWinRate)
        {
            AudioUtil.Instance.PlayAudioFx("LinkGame_BoostChecked");
            await XUtility.PlayAnimationAsync(GetElement().animator,"Trigger",context);
            var addValueAnimation = context.assetProvider.InstantiateGameObject("AddValueText", true);
            addValueAnimation.SetActive(false);
            addValueAnimation.SetActive(true);
            addValueAnimation.transform.SetParent(transform.parent,false);
            addValueAnimation.transform.position = transform.position;
            var chips = context.state.Get<BetState>().GetPayWinChips(addWinRate);
            addValueAnimation.transform.Find("NumFly").GetComponent<TextMesh>().text = "+ "+Constant11024.ChipNum2String(Convert.ToDouble(chips));
            await XUtility.PlayAnimationAsync(addValueAnimation.GetComponent<Animator>(),"Open",context);
            UpdateToDataState(true);
            AudioUtil.Instance.PlayAudioFx("LinkGame_BoostAdd");
            JumpNum();
            // await XUtility.PlayAnimationAsync(addValueAnimation.GetComponent<Animator>(),"Idle",context);
            await XUtility.PlayAnimationAsync(addValueAnimation.GetComponent<Animator>(),"Close",context);
            context.assetProvider.RecycleGameObject("AddValueText",addValueAnimation);
        }
        public async void ShowCollectAnimation()
        {
            SetSortingOrderFly(posX,posY);
            await XUtility.PlayAnimationAsync(GetElement().animator,"Fly");
            SetSortingOrder(posX,posY);
        }
        public void ShowCollectAnimationComplete()
        {
            var animator = GetElement().animator;
            var stateName = "Fly";
            if (animator.HasState(stateName))
            {
                animator.speed = 1;
                animator.Play(stateName, -1, 1);
            }
        }
        public void RemoveStickyElement()
        {
            nowStickyElementData = null;
            if (!ReferenceEquals(currentAttachElement, null))
            {
                var tempRotation = currentAttachElement.transform.rotation;
                tempRotation.eulerAngles = Vector3.zero;
                currentAttachElement.transform.rotation = tempRotation;
            }
            RemoveElement();
        }
    }
}