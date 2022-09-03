// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 10:08 PM
// Ver : 1.0.0
// Description : WheelPanel.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameModule
{
    public class Wheel : TransformHolder
    {
        public WheelState wheelState;
        
        /// <summary>
        /// 轮盘的宽度
        /// </summary>
        public float contentWidth;
        /// <summary>
        /// 轮盘的高度
        /// </summary>
        public float contentHeight;
        
        /// <summary>
        /// 轮盘自己的MASK
        /// </summary>
        public SpriteMask wheelMask;
        
        /// <summary>
        /// 轮盘的WheelName
        /// </summary>
        public string wheelName;
        
        /// <summary>
        /// 轮盘的拥有Roll数量
        /// </summary>
        public int rollCount;
        
        /// <summary>
        /// 辅助功能，方便其他逻辑存放一些东西在Wheel上
        /// </summary>
        public Dictionary<string, GameObject> objectAttachedOnWheel;
   
        /// <summary>
        /// Controller 控制轮盘的滚动
        /// </summary>
        public IWheelSpinningController spinningController;
 
        /// <summary>
        /// 赢钱动画播放控制器
        /// </summary>
        public IWinLineAnimationController winLineAnimationController;
         
     
        protected Roll[] rolls;
        protected Roll[] stopOrderRolls;

        /// <summary>
        /// Roll大小
        /// </summary>
        protected Vector2 rollContentSize;

        protected string wheelElementSortingLayerName;


        protected GameObject clickableGameObject;
         
        public Wheel(Transform transform) : base(transform)
        {
          
        }


        public virtual int GetMaxSpinningUpdaterCount()
        {
            return rollCount;
        }
        
        /// <summary>
        /// 根据Index获取滚轮
        /// </summary>
        /// <param name="rollIndex"></param>
        /// <returns></returns>
        public Roll GetRoll(int rollIndex)
        {
// #if DEBUG
//             if (rollIndex >= rollCount)
//                 return null;
// #endif
            return rolls[rollIndex];
        }


        public virtual Vector3 GetAnticipationAnimationPosition(int rollIndex)
        {
            return GetRoll(rollIndex).initialPosition;
        }

        public virtual ElementContainer GetWinLineElementContainer(int rollIndex, int rowIndex)
        {
            Roll roll = GetRoll(rollIndex);
            return roll?.GetWinLineContainer(rowIndex);
        }
        public virtual ElementContainer GetWinFrameElementContainer(int rollIndex, int rowIndex)
        {
            Roll roll = GetRoll(rollIndex);
            return roll?.GetWinFrameContainer(rowIndex);
        }

        public SequenceElement GetElement(int rollIndex, int rowIndex)
        {
            Roll roll = GetRoll(rollIndex);
            return roll?.GetVisibleContainer(rowIndex)?.sequenceElement;
        }

        /// <summary>
        /// 获取满足特定条件的ElementContainer
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="offsetRow"></param>
        /// <returns></returns>
        public List<ElementContainer> GetElementMatchFilter(Func<ElementContainer, bool> filter, int offsetRow = 0)
        {
            List<ElementContainer> elementContainers = new List<ElementContainer>();

            for (var i = 0; i < rollCount; i++)
            {
                var rowCount = rolls[i].rowCount + offsetRow;
                for (var j = offsetRow; j < rowCount; j++)
                {
                    var container = rolls[i].GetVisibleContainer(j);

                    if (filter.Invoke(container))
                    {
                        elementContainers.Add(container);
                    }
                }
            }
            
            return elementContainers;
        }
         
        public override bool MatchFilter(string filter)
        {
            return filter == wheelName;
        }
        
        /// <summary>
        /// 获得第几个停下的Roll
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Roll GetStopRoll(int index)
        {
            return stopOrderRolls[index];
        }
 
        /// <summary>
        /// 内部函数，初始化轮盘的一些基础属性
        /// </summary>
        /// <param name="wheelConfig"></param>
        protected virtual void InitializeWheelInfo(WheelConfig wheelConfig)
        {
            wheelName = transform.name;
            rollCount = wheelConfig.rollCount;
            wheelMask = transform.Find("WheelMask").GetComponent<SpriteMask>();
          
            if (wheelMask)
            {
                var parentScale = wheelMask.transform.parent.transform.lossyScale;
                var invScaleX = 1f / parentScale.x;
                var invScaleY = 1f / parentScale.y;
                var size = wheelMask.bounds.size;
               
                contentHeight = size.y * invScaleY;
                contentWidth = size.x * invScaleX;

                rollContentSize = new Vector2(contentWidth / rollCount, contentHeight);
                InitializeWheelMaskSortOrder();
            }
        }

        /// <summary>
        /// 扩大轮盘Mask的大小，某些关卡可能像让Mask的大小大一些，避免图标在轮盘框左右被裁掉，但是由于图标的位置是通过MASK大小定的，所以这个函数
        /// 要在InitializeWheelInfo之后调用，
        /// </summary>
        public void ExtendWheelMaskSize(int extendPixelX, int extendPixelY = 0)
        {
            var wheelMaskTransform = transform.Find("WheelMask");
            var localScale = wheelMaskTransform.localScale;
            //wheelMask使用的贴图大小是50Pixel
            //对应大小就是0.5个世界单位
            localScale.x += extendPixelX * 0.02f;
            localScale.y += extendPixelY * 0.02f;
            wheelMaskTransform.localScale = localScale;
        }

        /// <summary>
        /// 获取Roll在Wheel上的位置
        /// </summary>
        /// <param name="rollIndex"></param>
        /// <returns></returns>
        protected virtual Vector3 GetRollPosition(int rollIndex)
        {
            var centerRoll = (rollCount - 1) * 0.5f;
            return new Vector3((rollIndex - centerRoll) * rollContentSize.x, 0, 0);
        }
        
        protected virtual int GetRollStopIndex(int rollIndex)
        {
            return rollIndex;
        }
        
        /// <summary>
        /// 获取Roll的区域大小
        /// </summary>
        /// <param name="rollIndex"></param>
        /// <returns></returns>
        protected virtual Vector2 GetRollContentSize(int rollIndex)
        {
            return rollContentSize;
        }
        
        protected virtual void InitializeWheelMaskSortOrder()
        {
            const int ORDER_RANGE_PER_WHEEL = 200;

            wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            wheelMask.backSortingOrder = -1;
            wheelMask.frontSortingOrder = ORDER_RANGE_PER_WHEEL;
        }

        /// <summary>
        /// 轮盘的创建
        /// </summary>
        /// <param name="inWheelState">轮盘对应的WheelState</param>
        /// <typeparam name="TRoll">轮盘的Roll的具体类型</typeparam>
        /// <typeparam name="TElementSupplier">轮盘Roll提供卷轴数据的Supplier的类型</typeparam>
        /// <typeparam name="TWheelSpinningController">控制轮盘转动的类</typeparam>
        /// <param name="inWheelElementSortingLayerName">轮盘图标SortLayer层级</param>
        public virtual void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState, string inWheelElementSortingLayerName = "Element")
            where TRoll : Roll where TElementSupplier : IElementSupplier
            where TWheelSpinningController:IWheelSpinningController
        {
            
            wheelElementSortingLayerName = inWheelElementSortingLayerName;
            
            wheelState = inWheelState;
  
            wheelState.InitializeWheelState(transform.name);

            var wheelConfig = wheelState.GetWheelConfig();
            
            InitializeWheelInfo(wheelState.GetWheelConfig());
             
            rolls = new Roll[rollCount];
            stopOrderRolls = new Roll[rollCount];

            var rollsRoot = transform.Find("Rolls");

            rollsRoot.localPosition = wheelMask.transform.localPosition;
            
            var rollTransform = rollsRoot.Find("Roll");
            
            for (var i = 0; i < rollCount; i++)
            {
                var rollIndexI = GameObject.Instantiate(rollTransform.gameObject, rollsRoot);
                rollIndexI.name = $"Roll{i}";
                
                rollIndexI.transform.localPosition = GetRollPosition(i);
                rollIndexI.SetActive(true);

                var constructor = typeof(TRoll).GetConstructor(new[] {typeof(Transform),typeof(bool),typeof(bool),typeof(string)});
            
                if (constructor != null)
                {
                    var roll = (TRoll) constructor.Invoke(new object[] {rollIndexI.transform, wheelConfig.topRowHasHighSortOrder, wheelConfig.leftColHasHighSortOrder, wheelElementSortingLayerName});
                    roll.Initialize(context);
                    var supplier = Activator.CreateInstance<TElementSupplier>();
                  
                    roll.BuildRoll(i, GetRollRowCount(i, wheelConfig), GetRollContentSize(i), supplier, GetRollBuildingExtraConfig());
                    roll.stopIndex = GetRollStopIndex(i);
                    supplier.InitializeSupplier(wheelState, roll);
                    
                    rolls[i] = roll;
                    stopOrderRolls[roll.stopIndex] = roll;
                }
            }
            
            rollTransform.gameObject.SetActive(false);
            
            SetUpWheelSpinningController<TWheelSpinningController>();
            
            SetActive(false);
            
            SetUpWheelClickHandler();
        }

        public virtual RollBuildingExtraConfig GetRollBuildingExtraConfig()
        {
            RollBuildingExtraConfig extraConfig = new RollBuildingExtraConfig();
            extraConfig.elementMaxHeight = wheelState.GetElementMaxHeight();
            extraConfig.extraTopElementCount = wheelState.GetExtraTopElementCount();
            return extraConfig;
        }

        public virtual int GetRollRowCount(int rollIndex, WheelConfig wheelConfig)
        {
            return wheelConfig.rollRowCount;
        }
        
        //可能某些轮盘不需要播放赢钱动画，所以这个函数根据需要来创建
        public void SetUpWinLineAnimationController<TWinLineAnimationController>()
            where TWinLineAnimationController : IWinLineAnimationController
        {
            winLineAnimationController = Activator.CreateInstance<TWinLineAnimationController>();
            winLineAnimationController.BindingWheel(this);
        }
         
        protected void SetUpWheelSpinningController<TWheelSpinningController>()
            where TWheelSpinningController : IWheelSpinningController
        {
            spinningController = Activator.CreateInstance<TWheelSpinningController>();
            spinningController.BindingWheel(this);
        }
         
        /// <summary>
        /// 辅助功能，方便在轮盘上挂接一些特效之类的东西，并通过轮盘能够获取这些对象，
        /// 真正的挂接操作，以及挂接位置由具体逻辑自己操作，这里只是保存了一个引用
        /// </summary>
        /// <param name="refName"></param>
        /// <param name="gameObject"></param>
        public void AttachGameObject(string refName, GameObject gameObject)
        {
            if (objectAttachedOnWheel == null)
            {
                objectAttachedOnWheel = new Dictionary<string, GameObject>();
            }
            
            objectAttachedOnWheel.Add(refName, gameObject);
        }

        /// <summary>
        /// 获取挂接的GameObject
        /// </summary>
        /// <param name="refName"></param>
        /// <returns></returns>
        public GameObject GetAttachedGameObject(string refName)
        {
            if (objectAttachedOnWheel != null && objectAttachedOnWheel.ContainsKey(refName))
            {
                return objectAttachedOnWheel[refName];
            }

            return null;
        }
        /// <summary>
        /// 取消GameObject的挂接
        /// </summary>
        /// <param name="refName"></param>
        /// <returns></returns>
        public void DetachGameObject(string refName)
        {
            if (objectAttachedOnWheel != null)
            {
                objectAttachedOnWheel.Remove(refName);
            }
        }

        public void ToggleAttachedGameObject(string nameSub,bool visible=true)
        {
            if (objectAttachedOnWheel != null)
            {
                var listAttatch = objectAttachedOnWheel.Keys.ToList();
                for (int i = 0; i < listAttatch.Count; i++)
                {
                    var goAnimation = objectAttachedOnWheel[listAttatch[i]];
                    if (goAnimation != null && goAnimation.name.Contains(nameSub))
                    {
                        goAnimation.gameObject.SetActive(visible);
                    }
                }   
            }     
        }

        public void UpdateAnimationToStatic()
        {
            for (var i = 0; i < rollCount; i++)
            {
                for (var j = 0; j < rolls[i].rowCount; j++)
                {
                    var container = rolls[i].GetVisibleContainer(j);

                    container.UpdateAnimationToStatic();
                }
            }
        }
        
        public void UpdateElementMaskInteraction(bool active)
        {
            for (var i = 0; i < rollCount; i++)
            {
                if (wheelState.IsRollLocked(i)) continue;
                for (var j = 0; j < rolls[i].rowCount; j++)
                {
                    var container = rolls[i].GetVisibleContainer(j);

                    container.UpdateElementMaskInteraction(active);
                }
            }
        }

 
        public void ForceUpdateElementOnWheel(bool refreshLock = true, bool useSeqElement=false)
        {
            if (rollCount > 0)
            {
                for (var i = 0; i < rollCount; i++)
                {
                    var elementSupplier = rolls[i].elementSupplier;
                    int startIndex = elementSupplier.GetStartIndex(); 
                    elementSupplier.OnStopAtIndex(startIndex);
                    if (!refreshLock && wheelState.IsRollLocked(i))
                    {
                        rolls[i].ForceUpdateAllElementSortingOrder(startIndex);
                        continue;
                    }
                    rolls[i].ForceUpdateAllElement(startIndex,useSeqElement);
                }
            }
        }

        public void UpdateVisibleElementOnWheel(List<List<SequenceElement>> elementInfo)
        {
            if (rollCount > 0)
            {
                for (var i = 0; i < rollCount; i++)
                {
                    rolls[i].UpdateVisibleElement(elementInfo[i]);
                }
            }
        }
        
        public List<List<SequenceElement>> GetVisibleElementInfo()
        {
            List<List<SequenceElement>> elementInfo = new List<List<SequenceElement>>();
            if (rollCount > 0)
            {
                for (var i = 0; i < rollCount; i++)
                {
                    elementInfo.Add(rolls[i].GetVisibleSequenceElement());
                }
            }
            return elementInfo;
        }

        public void ResetDoneTag()
        {
            for (int i = 0; i < rollCount; i++)
            {
                rolls[i].ResetDoneTag();
            }
        }

        public virtual void SetActive(bool active,bool keepObjectState = false)
        {
            if (!keepObjectState)
            {
                transform.gameObject.SetActive(active);   
            }
           
            wheelState.UpdateWheelActiveState(active);
            
            if(winLineAnimationController != null && !active && winLineAnimationController.IsWinCyclePlaying) 
            {
                winLineAnimationController.StopAllElementAnimation();
            }
        }

        public virtual Transform GetWheelClickableTransform()
        {
            if (transform.Find("WheelClickHandler"))
            {
                clickableGameObject = transform.Find("WheelClickHandler").gameObject;
                return clickableGameObject.transform;
            }
            
            clickableGameObject = GameObject.Instantiate(wheelMask.gameObject, wheelMask.transform.parent);
            clickableGameObject.SetActive(true);
            var spriteMask = clickableGameObject.GetComponent<SpriteMask>();
            if (spriteMask != null)
            {
                spriteMask.enabled = false;
            }

            clickableGameObject.gameObject.name = "WheelClickHandler";
            
            if (!clickableGameObject.GetComponent<BoxCollider2D>())
            {
                clickableGameObject.gameObject.AddComponent<BoxCollider2D>();
            }
           
            return clickableGameObject.transform;
            
        }

        public void ToggleInteractableStatues(bool interactable)
        {
            clickableGameObject.SetActive(interactable);
        }

        public virtual void SetUpWheelClickHandler()
        {
            var clickableTransform = GetWheelClickableTransform();

            if (clickableTransform != null)
            {
                var pointerEventCustomHandler = clickableTransform.gameObject.AddComponent<PointerEventCustomHandler>();

                pointerEventCustomHandler.BindingPointerClick((data =>
                {
                    context.view.Get<ControlPanel>().OnWheelClicked();
                }));
            }
        }
    }
}