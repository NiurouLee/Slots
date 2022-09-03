//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-06 14:17
//  Ver : 1.0.0
//  Description : MultiFreePanelView11016.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MultiFreePanelView11016:TransformHolder
    {
        private Dictionary<int, List<Transform>> _dictPanelsPosition;
        public MultiFreePanelView11016(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);

            _dictPanelsPosition = new Dictionary<int, List<Transform>>();
            var panelsCount = new []{1, 2, 3, 4, 6, 9};
            for (int i = 0; i < panelsCount.Length; i++)
            {
                var listPanels = new List<Transform>();
                var panelCount = panelsCount[i];
                for (int j = 0; j < panelCount; j++)
                {
                    listPanels.Add(transform.Find($"{panelCount}/Container{j}"));
                }
                _dictPanelsPosition.Add(panelCount,listPanels);
            }
        }

        public List<Transform> GetPanelsPositions(int panelCount)
        {
            return _dictPanelsPosition[panelCount];
        }
    }
}