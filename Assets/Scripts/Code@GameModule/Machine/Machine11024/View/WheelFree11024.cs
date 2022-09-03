using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class WheelFree11024:Wheel
    {
        private ElementConfigSet _elementConfigSet;

        public ElementConfigSet elementConfigSet
        {
            get
            {
                if (_elementConfigSet == null)
                {
                    _elementConfigSet = context.machineConfig.GetElementConfigSet();
                }

                return _elementConfigSet;
            }
        }
        private ExtraState11024 _extraState;

        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState = context.state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }
        public int FreeWheelIndex;
        private Animator wildCover;
        private bool isOpen;
        public WheelFree11024(Transform transform) : base(transform)
        {
            wildCover = transform.GetComponent<Animator>();
        }

        public override void BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(WheelState inWheelState,
            string inWheelElementSortingLayerName = "Element")
        {
            base.BuildWheel<TRoll, TElementSupplier, TWheelSpinningController>(inWheelState, inWheelElementSortingLayerName);
        }

        public override void SetActive(bool active, bool keepObjectState = false)
        {
            base.SetActive(active, keepObjectState);
            if (active)
            {
                isOpen = false;
                XUtility.PlayAnimation(wildCover,"CloseIdle");
            }
        }

        public void InitMidReel()
        {
            wheelState.SetRollLockState(2, true);
            {
                uint elementId = 83;
                var element = new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
                GetRoll(2).GetVisibleContainer(2).UpdateElement(element,false);   
            }
            {
                uint elementId = 831;
                var element = new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
                GetRoll(2).GetVisibleContainer(1).UpdateElement(element,false);
            }
            {
                uint elementId = 832;
                var element = new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
                GetRoll(2).GetVisibleContainer(0).UpdateElement(element,false);
            }
            for (var i = 0; i < 1; i++)
            {
                GetRoll(2).GetVisibleContainer(i).transform.gameObject.SetActive(false);   
            }
            for (var i = 3; i < 6; i++)
            {
                GetRoll(2).GetVisibleContainer(i).transform.gameObject.SetActive(false);   
            }
        }
        public async Task OpenWildCover()
        {
            var nowMulti = extraState.GetMapData().Multipliers[FreeWheelIndex];
            if (!Constant11024.LongWildMultiToId.ContainsKey(nowMulti))
            {
                throw new Exception("没找到对应倍数"+nowMulti+"的长wildID");
            }
            // InitMidReel();
            {
                var elementId = Constant11024.LongWildMultiToId[nowMulti];
                var element = new SequenceElement(elementConfigSet.GetElementConfig(elementId), context);
                GetRoll(2).GetVisibleContainer(2).UpdateElement(element,false);   
            }
            if (!isOpen)
            {
                isOpen = true;
                await XUtility.PlayAnimationAsync(wildCover,"Open");   
            }
        }
        public async Task CloseWildCover()
        {
            if (isOpen)
            {
                isOpen = false;
                await XUtility.PlayAnimationAsync(wildCover,"Close");   
            }
        }
        
        protected override void InitializeWheelMaskSortOrder()
        {
            // const int ORDER_RANGE_PER_WHEEL = 200;
            // wheelMask.backSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            // wheelMask.frontSortingLayerID = SortingLayer.NameToID(wheelElementSortingLayerName);
            // wheelMask.backSortingOrder = -1;
            // wheelMask.frontSortingOrder = ORDER_RANGE_PER_WHEEL;
        }
    }
}