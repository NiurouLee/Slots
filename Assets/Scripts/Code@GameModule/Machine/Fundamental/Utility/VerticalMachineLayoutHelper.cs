// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/10/15:02
// Ver : 1.0.0
// Description : VerticalMachineLayoutHelper.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class VerticalMachineLayoutHelper
    {
        public class LayoutElement
        {
            public Transform content;       //需要布局的元素的Transform
            public int heightInPixel;       //需要布局的元素的高度
            public float anchorPoint;       //锚点的位置
            public float bottomPaddingPercent; //当屏幕空间有赋予情况下，元素需要向上移动的富裕位置的百分比
            public float bottomPaddingInPixel; //当屏幕空间有赋予情况下，元素需要向上移动的绝对的像素位置
        }

        protected List<LayoutElement> layoutElements;

        protected float titleHeight = MachineConstant.topPanelVHeight;
        protected float controlPanelHeight = MachineConstant.controlPanelVHeight;

        public void ModifyTopPanelHeight(float height)
        {
            titleHeight = height;
        }

        public void ModifyControlPanelHeight(float height)
        {
            controlPanelHeight = height;
        }
        
        public void AddElement(Transform content, int heightInPixel, float anchorPoint = 0.0f,
            float bottomPaddingPercent = 0, float bottomPaddingInPixel = 0f)
        {
            var element = new LayoutElement();
            element.content = content;
            element.heightInPixel = heightInPixel;
            element.anchorPoint = anchorPoint;
            element.bottomPaddingPercent = bottomPaddingPercent;
            element.bottomPaddingInPixel = bottomPaddingInPixel;

            if (layoutElements == null)
            {
                layoutElements = new List<LayoutElement>();
            }

            layoutElements.Add(element);
        }

        public int GetLayOutElementCount()
        {
            return layoutElements.Count;
        }
        
        public LayoutElement GetLayOutElement(int index)
        {
            return layoutElements[index];
        }

        public void DoLayout()
        {
            var uiTotalHeight = 0;

            var systemUIHeight = titleHeight + controlPanelHeight;

            for (var i = 0; i < layoutElements.Count; i++)
            {
                uiTotalHeight += layoutElements[i].heightInPixel;
            }

            var totalHeight = systemUIHeight + uiTotalHeight;

            var deviceHeight = ViewResolution.referenceResolutionPortrait.y;

            float elementNeedScale = 1.0f;

            if (totalHeight > deviceHeight)
            {
                elementNeedScale = 1 - (totalHeight - deviceHeight) / uiTotalHeight;
            }

            if (elementNeedScale < 1)
            {
                var currentPosY = controlPanelHeight;

                for (var i = 0; i < layoutElements.Count; i++)
                {
                    layoutElements[i].content.localScale =
                        new Vector3(elementNeedScale, elementNeedScale, elementNeedScale);

                    var elementPosY = currentPosY + layoutElements[i].heightInPixel * elementNeedScale *
                        layoutElements[i].anchorPoint;
                    var localPos = layoutElements[i].content.localPosition;
                    localPos.y = (-deviceHeight * 0.5f + elementPosY) * MachineConstant.pixelPerUnitInv;

                    layoutElements[i].content.localPosition = localPos;

                    currentPosY += layoutElements[i].heightInPixel * elementNeedScale;
                }
            }
            else
            {
                var availableSpace = deviceHeight - totalHeight;

                var currentPosY = controlPanelHeight;

                for (var i = 0; i < layoutElements.Count; i++)
                {
                    var elementPosY = currentPosY + layoutElements[i].heightInPixel *
                                      layoutElements[i].anchorPoint +
                                      layoutElements[i].bottomPaddingPercent * availableSpace +
                                      layoutElements[i].bottomPaddingInPixel;

                    var localPos = layoutElements[i].content.localPosition;

                    localPos.y = (-deviceHeight * 0.5f + elementPosY) * MachineConstant.pixelPerUnitInv;

                    layoutElements[i].content.localPosition = localPos;

                    currentPosY += layoutElements[i].heightInPixel +
                                   layoutElements[i].bottomPaddingPercent * availableSpace +
                                   layoutElements[i].bottomPaddingInPixel;
                }
            }
        }
    }
}