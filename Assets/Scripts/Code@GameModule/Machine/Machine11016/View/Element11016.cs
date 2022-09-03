//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 15:26
//  Ver : 1.0.0
//  Description : Element11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class Element11016:Element
    {
        [ComponentBinder("HeartFly")] 
        public Animator AnimatorHeart;
        private int _frontOrder = 0;
        private int _backOrder = 0;
        public int ShiftOrder = 0;
        public Element11016(Transform transform, bool inIsStatic)
            : base(transform,inIsStatic)
        {
            ComponentBinder.BindingComponent(this,transform);
        }

        public Vector3 GetStartWorldPos()
        {
            return AnimatorHeart.transform.position;
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            if (Constant11016.IsFeaturedElement(element.config.id))
            {
                AnimatorHeart.gameObject.SetActive(true);
                XUtility.PlayAnimation(AnimatorHeart,"Idle");
            }
        }

        public void HideTrail()
        {
            AnimatorHeart.gameObject.SetActive(false);
        }

        public async Task PlayHeartAnimation(string stateName="Blink")
        {
            if (AnimatorHeart)
            {
                AnimatorHeart.gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(AnimatorHeart, stateName);
            }
        }

        public override async void PlayAnimation(string animationName, bool maskByWheelMask, Action endCallback = null)
        {
            var isFeature = Constant11016.IsFeaturedElement(sequenceElement.config.id);
            if ( isFeature && animationName == "Blink")
            {
                HideTrail();
                await PlayHeartAnimation();
                endCallback?.Invoke();
            }
            else
            {
                base.PlayAnimation(animationName, maskByWheelMask, endCallback);
                if (isFeature & animationName == "Win")
                {
                    HideTrail();
                }
            }
        }
        
        public void ShiftMaskAndSortOrder(int shiftOrder)
        {
            ShiftOrder = shiftOrder;   
            var listSuffix = new[] { "Blue", "Red", "Yellow" };
            for (int i = 0; i < listSuffix.Length; i++)
            {
                var mask = transform.Find($"SoltMini{listSuffix[i]}/WheelMask");
                if (mask)
                {
                    var soloMask = mask.GetComponent<SpriteMask>();
                    if (_frontOrder == 0)
                    {
                        _frontOrder = soloMask.frontSortingOrder;
                        _backOrder = soloMask.backSortingOrder;
                    }
                    _frontOrder += shiftOrder;
                    _backOrder += shiftOrder;
                    soloMask.frontSortingOrder = _frontOrder;
                    soloMask.backSortingOrder = _backOrder;


                    var children = transform.Find($"SoltMini{listSuffix[i]}")
                        .GetComponentsInChildren<SortingGroup>(true);
                    for (int j = 0; j < children.Length; j++)
                    {
                        children[j].sortingOrder += shiftOrder;
                    }
                    
                    var transAnimation = transform.Find($"SoltMini{listSuffix[i]}/Animation");
                    var listAllSprites = transAnimation.GetComponentsInChildren<SpriteRenderer>(true);
                    for (int j = 0; j < listAllSprites.Length; j++)
                    {
                        listAllSprites[j].maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                    }
                } 
            }

            if (ShiftOrder<0)
            {
                ShiftOrder = 0;
            }
        }
    }
}