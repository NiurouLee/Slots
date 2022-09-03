// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:28 PM
// Ver : 1.0.0
// Description : ElementView.cs
// ChangeLog :
// **********************************************

using System;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace GameModule
{
    public class Element: TransformHolder
    {
        public Animator animator;
     
        public bool isStaticElement;

        public SequenceElement sequenceElement;
        
        protected SpriteMaskInteraction interaction;
        protected SpriteMaskInteraction defaultInteraction;
        private bool foreUpdateInteraction = false;
        
        public Element(Transform transform, bool inIsStatic)
            : base(transform)
        {
            isStaticElement = inIsStatic;
            animator = transform.GetComponent<Animator>();
            
        }
        
        
        protected GameObject objGary;

        protected static readonly (float, float, float) EmptyGary = (0,0,0);
        //protected static Material matGary = null;
        protected virtual void InitGary()
        {
            if (isStaticElement && objGary==null)
            {

                var garyPram = sequenceElement.config.staticGrayLayerPrams;
                if (garyPram != EmptyGary)
                {
                    
                    
                    objGary = GameObject.Instantiate(transform.gameObject, transform);
                    objGary.name = "GrayLayer";
                    objGary.transform.localPosition = Vector3.zero;
                    objGary.transform.localEulerAngles = Vector3.zero;

                    
                    SpriteRenderer spriteRenderer = objGary.GetComponent<SpriteRenderer>();
                    if (spriteRenderer == null)
                        return;
                    // if (matGary == null)
                    // {
                    //     matGary = new Material(Shader.Find("HueAdjust"));
                    // }

                    spriteRenderer.sharedMaterial = sequenceElement.machineContext.assetProvider.GetAsset<Material>("MatHueAdjust");
                    MaterialPropertyBlock block = new MaterialPropertyBlock();
                    block.SetFloat("_HueValue",garyPram.Item1);
                    block.SetFloat("_LightValue",garyPram.Item2);
                    block.SetFloat("_SaturateValue",garyPram.Item3);
                    spriteRenderer.SetPropertyBlock(block);


                    
                }
            }
        }
        
        
        protected virtual void UpdateShowGrayLayer()
        {
            UpdateShowGrayLayer(true);
        }
        
        protected virtual void UpdateShowGrayLayer(bool isEnable)
        {
            if (objGary!=null)
            {
                objGary.SetActive(isEnable);
                SpriteRenderer spriteRendererOrigin = transform.GetComponent<SpriteRenderer>();
                if(spriteRendererOrigin)
                    spriteRendererOrigin.enabled = !isEnable;
            }
        }


        public virtual void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            sequenceElement = element;
            
            InitGary();
            
#if !PRODUCTION_PACKAGE
            if (!transform.gameObject.name.Contains(sequenceElement.config.name))
            {
                Debug.Log("PoolDirtyError");
            }
#endif
            transform.SetParent(containerTransform, false);

            defaultInteraction = element.config.defaultInteraction;

            if (foreUpdateInteraction && defaultInteraction != interaction)
                UpdateMaskInteraction(defaultInteraction);
            else
            {
                foreUpdateInteraction = true;
                UpdateMaskInteraction(defaultInteraction, true);
            }
            
            UpdateShowGrayLayer();
        }

        public virtual bool HasAnimState(string stateName)
        {
            return true;
        }

        public virtual void DoRecycle()
        {
#if !PRODUCTION_PACKAGE
            if (!transform.gameObject.name.Contains(sequenceElement.config.name))
            {
                XDebug.LogError("PoolDirtyError:");
            }
#endif            
            sequenceElement.config.pool.Recycle(this);
        }
      
        /// <summary>
        /// 判断内部组件的sortingOrder是否在内部的mask范围
        /// </summary>
        /// <param name="sortingOrder"></param>
        /// <param name="spriteMasks"></param>
        /// <returns></returns>
        public bool IsSpriteMaskedBySpriteMask(int sortingOrder, SpriteMask[] spriteMasks)
        {
            if (spriteMasks != null && spriteMasks.Length > 0)
            {
                for (var i = 0; i < spriteMasks.Length; i++)
                {
                    if (spriteMasks[i].backSortingOrder < sortingOrder &&
                        spriteMasks[i].frontSortingOrder >= sortingOrder)
                        return true;
                }
            }
            
            return false;
        }
        
        public virtual void UpdateMaskInteraction(SpriteMaskInteraction inInteraction, bool force = false)
        {
            if (transform == null || !force && inInteraction == interaction)
            {
                return;
            }

            interaction = inInteraction;
            
            var spriteMasks = transform.GetComponentsInChildren<SpriteMask>(true);
            
            if (transform)
            {
                var spriteList = transform.GetComponentsInChildren<SpriteRenderer>(true);
                for (int i = 0; i < spriteList.Length; i++)
                {
                    //如果这个sprite本身有被mask操作，那么不更改它的maskInteraction
                    if (!IsSpriteMaskedBySpriteMask(spriteList[i].sortingOrder, spriteMasks))
                    {
                        spriteList[i].maskInteraction = interaction;
                    }
                }
                
                var spineSkeletonList = transform.GetComponentsInChildren<SkeletonAnimation>(true);
             
                for (int i = 0; i < spineSkeletonList.Length; i++)
                {
                    if (!IsSpriteMaskedBySpriteMask(spineSkeletonList[i].GetComponent<MeshRenderer>().sortingOrder, spriteMasks))
                    {
                        spineSkeletonList[i].maskInteraction = interaction;
                    }
                }

                //粒子
                var particleList = transform.GetComponentsInChildren<ParticleSystemRenderer>(true);
                if (particleList != null && particleList.Length > 0)
                {
                    for (int i = 0; i < particleList.Length; i++)
                    {
                        if (!IsSpriteMaskedBySpriteMask(particleList[i].sortingOrder, spriteMasks))
                        {
                            particleList[i].maskInteraction = interaction;
                        }
                    }
                }
            }
        }
        
        public virtual void PlayAnimation(string animationName, bool maskByWheelMask, Action endCallback = null)
        {
            if (!maskByWheelMask)
            {
                UpdateMaskInteraction(sequenceElement.config.activeStateInteraction);
            }
            else
            {
                UpdateMaskInteraction(sequenceElement.config.defaultInteraction);
            }
            
            if (animator)
            {
                XUtility.PlayAnimation(animator, animationName, endCallback);
            }
            else
            {
                endCallback?.Invoke();
            }
        }
    }
}