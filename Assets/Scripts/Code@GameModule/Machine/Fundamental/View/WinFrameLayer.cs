// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/07/18:49
// Ver : 1.0.0
// Description : WinFrameLayer.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WinFrameLayer:TransformHolder
    {
        public float Scale;
        private readonly Queue<Transform> winFrameQueue;

        private readonly Dictionary<ElementContainer, Transform> activeWinFrameDict;

        private readonly GameObject winFrameObj;

        private string sortingOrderName;
        
        public WinFrameLayer(Transform parentTransform, GameObject inWinFrameObj, string orderName = "SlotSymbol")
            : base(parentTransform)
        {
            winFrameQueue = new Queue<Transform>();
            activeWinFrameDict = new Dictionary<ElementContainer, Transform>();
            winFrameObj = inWinFrameObj;
            winFrameObj.SetActive(false);
            winFrameObj.transform.SetParent(parentTransform,false);
            sortingOrderName = orderName;
        }
        
        public void ShowWinFrame(ElementContainer elementContainer)
        {
            if (winFrameObj != null && !activeWinFrameDict.ContainsKey(elementContainer))
            {
                if (winFrameQueue.Count > 0)
                {
                    var winFrame = winFrameQueue.Dequeue();
                    var localPosition = transform.InverseTransformPoint(elementContainer.transform.position);
                    Utils.SetLocalPosition(winFrame, localPosition.x,localPosition.y,localPosition.z);
                    if (Scale == 0)
                    {
                        Utils.SetLocalScale(winFrame);
                    }
                    else
                    {
                        Utils.SetLocalScale(winFrame, Scale, Scale, Scale);
                    }
                    activeWinFrameDict.Add(elementContainer, winFrame);

                    var listLine = winFrame.GetComponentsInChildren<TrailRenderer>();
                    foreach (var line in listLine)
                    {
                        line.Clear();
                    }
                    
                    winFrame.gameObject.SetActive(true);
                }
                else
                {
                    var winFrame = GameObject.Instantiate(winFrameObj, transform);
                    // var sortingGroup = winFrame.AddComponent<SortingGroup>();
                    // sortingGroup.sortingLayerID = SortingLayer.NameToID(sortingOrderName);
                    // sortingGroup.sortingOrder = 2500;
                    
                    winFrame.transform.SetParent(transform,false);
                    var localPosition = transform.InverseTransformPoint(elementContainer.transform.position);
                    Utils.SetLocalPosition(winFrame.transform, localPosition.x,localPosition.y,localPosition.z);
                    if (Scale == 0)
                    {
                        Utils.SetLocalScale(winFrame.transform);
                    }
                    else
                    {
                        Utils.SetLocalScale(winFrame.transform, Scale, Scale, Scale);
                    }

                    activeWinFrameDict.Add(elementContainer, winFrame.transform);
                    
                    var listLine = winFrame.GetComponentsInChildren<TrailRenderer>();
                    foreach (var line in listLine)
                    {
                        line.Clear();
                    }
                    
                    winFrame.gameObject.SetActive(true);
                }
            }
        }
        
        public void HideWinFrame(ElementContainer elementContainer)
        {
            if (activeWinFrameDict.ContainsKey(elementContainer))
            {
                var winFrame = activeWinFrameDict[elementContainer];
                winFrameQueue.Enqueue(winFrame);
                winFrame.gameObject.SetActive(false);
                activeWinFrameDict.Remove(elementContainer);
            }
        }
        
        public void HideAllWinFrame()
        {
            foreach (var item in activeWinFrameDict)
            {
                var winFrame = item.Value;
                winFrameQueue.Enqueue(winFrame);
                winFrame.gameObject.SetActive(false);
               
            }
            
            activeWinFrameDict.Clear();
        }
        
        public void ResetWinFrameSortingOrder(ElementContainer elementContainer, int sortingOrder)
        {
            if (activeWinFrameDict.ContainsKey(elementContainer))
            {
                Transform winFrame = activeWinFrameDict[elementContainer];
                if (winFrame != null)
                {
                    var sortingGroup = winFrame.GetComponent<SortingGroup>();
                    if (sortingGroup != null)
                    {
                        sortingGroup.sortingOrder = sortingOrder;  
                    }
                }
            }
        }
    }
}