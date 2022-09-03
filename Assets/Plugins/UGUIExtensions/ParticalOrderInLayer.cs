using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI界面粒子特效层级控制
/// </summary>
public class ParticalOrderInLayer : MonoBehaviour
{
    Canvas parentCanvas;
    int parentOrder = 0;
    Renderer[] renders;
    Dictionary<Renderer, int> RenderSortingOrderDic = new Dictionary<Renderer, int>();

    private void Awake()
    {
        //记录美术设计的粒子层级
        RenderSortingOrderDic.Clear();
        renders = GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            RenderSortingOrderDic.Add(renders[i], renders[i].sortingOrder);
        }
    }

    private void OnEnable()
    {
        RefreshSortingOrder();
    }

    private void Update()
    {
        //当前UI的父层级发生变化时需要刷新粒子层级
        if (parentCanvas != null && parentOrder != parentCanvas.sortingOrder)
        {
            RefreshSortingOrder();
        }
    }

    private void RefreshSortingOrder()
    {
        //在美术设计的层级基础上添加当前UI的父层级
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            return;
        }
        parentOrder = parentCanvas.sortingOrder;
        if (RenderSortingOrderDic.Count > 0)
        {
            foreach (var rs in RenderSortingOrderDic)
            {
                rs.Key.sortingOrder = parentCanvas.sortingOrder + rs.Value;
            }
        }
    }
}
