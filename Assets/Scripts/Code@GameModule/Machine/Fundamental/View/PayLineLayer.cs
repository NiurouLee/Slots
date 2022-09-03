//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-20 12:02
//  Ver : 1.0.0
//  Description : WinLineLayer.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.Protobuf.Collections;
using UnityEngine.Rendering;

namespace GameModule
{
    public class PayLineLayer : TransformHolder
    {
        /// <summary>
        /// <PayLineId, PayLineTrans>
        /// </summary>
        ///
        private readonly Dictionary<uint, LineRenderer> activePayLineDict;
        private readonly GameObject payLineObj;

        private string sortingOrderName;
        private Wheel wheel;

        public PayLineLayer(Wheel wheel, GameObject inWinFrameObj, string orderName = "Element")
            : base(wheel.transform)
        {
            this.wheel = wheel;
            activePayLineDict = new Dictionary<uint, LineRenderer>();
            payLineObj = inWinFrameObj;
            payLineObj.SetActive(false);
            payLineObj.transform.SetParent(wheel.transform, false);
            sortingOrderName = orderName;
        }

        public void ShowPayLine(WinLine winLine)
        {
            if (payLineObj != null && winLine != null && wheel.wheelState.IsShowPayLine)
            {
                if (activePayLineDict.ContainsKey(winLine.PayLineId))
                {
                    var payLine = activePayLineDict[winLine.PayLineId];
                    payLine.gameObject.SetActive(true);
                    UpdatePayLineSortingOrder(winLine.PayLineId, GetPayLineZorder());
                }
                else
                {
                    var winLinePositions = winLine.Positions;
                    var payLine = GameObject.Instantiate(payLineObj, transform);
                    payLine.name = "PayLine" + winLine.PayLineId;
                    var sortingGroup = payLine.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingOrderName);
                    sortingGroup.sortingOrder = GetPayLineZorder();
                    payLine.transform.SetParent(transform, false);
                    var payLineRender = payLine.GetComponent<LineRenderer>();

                    var count = winLinePositions.Count;
                    Vector3[] listPaths = new Vector3[count + 2];

                    float rollWidth = wheel.contentWidth * wheel.transform.lossyScale.x / wheel.rollCount;
                    var position0 = wheel.GetRoll((int) winLinePositions[0].X)
                        .GetVisibleContainer((int) winLinePositions[0].Y).transform.position;
                    listPaths[0] = new Vector3(position0.x - rollWidth * 0.5f, position0.y,
                        position0.z);
                    for (int i = 0; i < count; i++)
                    {
                        var position = winLinePositions[i];
                        var elementWorldPosition = wheel.GetRoll((int) position.X)
                            .GetVisibleContainer((int) position.Y).transform.position;
                        listPaths[i + 1] = elementWorldPosition;
                    }

                    var positionLast = wheel.GetRoll((int) winLinePositions[count - 1].X)
                        .GetVisibleContainer((int) winLinePositions[count - 1].Y).transform.position;
                    listPaths[count + 1] =
                        new Vector3(positionLast.x + rollWidth * 0.5f, positionLast.y, positionLast.z);
                    payLineRender.useWorldSpace = true;
                    payLineRender.positionCount = count + 2;
                    payLineRender.SetPositions(listPaths);
                    payLineRender.startColor = GetLineColor(winLine.PayLineId);
                    payLineRender.endColor = payLineRender.startColor;
                    payLine.gameObject.SetActive(true);
                    activePayLineDict[winLine.PayLineId] = payLine.GetComponent<LineRenderer>();
                }
            }
        }

        private Color GetLineColor(uint index)
        {
            var spriteRender = payLineObj.FindChild("Line" + (index-1));
            if (spriteRender)
            {
                return spriteRender.GetComponent<SpriteRenderer>().color;
            }
            return Color.white;
        }

        public void HidePayLine(uint payLineId)
        {
            if (activePayLineDict.ContainsKey(payLineId))
            {
                var payLine = activePayLineDict[payLineId];
                payLine.gameObject.SetActive(false);
            }
        }

        public void HideAllPayLines()
        {
            foreach (var item in activePayLineDict)
            {
                var payLine = item.Value;
                payLine.gameObject.SetActive(false);

            }
        }

        public void UpdatePayLineSortingOrder(uint payLineId, int sortingOrder)
        {
            if (payLineObj != null && activePayLineDict.ContainsKey(payLineId))
            {
                var sortingGroup = activePayLineDict[payLineId].GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = sortingOrder;
            }
        }

        private int GetPayLineZorder()
        {
            var offset = wheel.wheelState.PayLineOffsetZOrder;
            return offset + (wheel.wheelState.IsPayLineUpElement ? 500 : 0);;
        }
    }
}