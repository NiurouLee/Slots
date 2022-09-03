// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/28/11:56
// Ver : 1.0.0
// Description : PigElement.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class PigElement:Element
    {
        [ComponentBinder("IntegralText")] private TextMesh _integralText;

        [ComponentBinder("FR+1")] 
        private Transform _freePlusOne;
        [ComponentBinder("FR+2")] 
        private Transform _freePlusTwo;
        [ComponentBinder("FR+3")] 
        private Transform _freePlusThree;

        private List<Transform> _freeExtraCountTransforms;
        public PigElement(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);

            _freeExtraCountTransforms = new List<Transform>() {_freePlusOne, _freePlusTwo, _freePlusThree};
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }

        public override void UpdateMaskInteraction(SpriteMaskInteraction inInteraction, bool force = false)
        {
            base.UpdateMaskInteraction(inInteraction,force);

            if (!isStaticElement)
            {
                if (inInteraction == SpriteMaskInteraction.VisibleInsideMask)
                {
                    var meshRenderer = _integralText.transform.GetComponent<MeshRenderer>();

                    if (meshRenderer != null)
                        meshRenderer.material.SetFloat("_StencilComp", 2);
                }
                else
                {
                    var meshRenderer = _integralText.transform.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                        meshRenderer.material.SetFloat("_StencilComp", 8);
                }
            }
        }

        public void UpdateElementContent()
        {
            var extraState11003 = sequenceElement.machineContext.state.Get<ExtraState11003>();
          
            if (extraState11003.IsSuperFreeGame())
            {
                _integralText.text = extraState11003.GetEachPigWinChipsInSuperFree().GetAbbreviationFormat(1);
            }
            else
            {
                _integralText.text = extraState11003.GetEachPigWinChipInFree().GetAbbreviationFormat(1);
            }

            if (!isStaticElement)
            {
                int extraFreeCount = sequenceElement.config.GetExtra<int>("count");

                for (var i = 0; i < _freeExtraCountTransforms.Count; i++)
                {
                    _freeExtraCountTransforms[i].gameObject.SetActive(false);
                }

                if (extraFreeCount > 0)
                {
                    if (_freeExtraCountTransforms.Count >= extraFreeCount)
                    {
                        _freeExtraCountTransforms[extraFreeCount - 1].gameObject.SetActive(true);
                    }
                }
            }
        }
    }
}