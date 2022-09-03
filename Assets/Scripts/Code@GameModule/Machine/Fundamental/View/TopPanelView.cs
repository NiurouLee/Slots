// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/23/17:35
// Ver : 1.0.0
// Description : TopPanelView.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class TopPanelView : TransformHolder
    {
        private TopPanel _topPanel;

        public TopPanelView(Transform transform)
            : base(transform)
        {
            _topPanel = View.CreateView<TopPanel>(transform);
            
            if(_topPanel.transform.name.Contains("V"))
                _topPanel.rectTransform.anchoredPosition = new Vector2(0, -MachineConstant.titleOffSetY);
        }

        public override void OnDestroy()
        {
            _topPanel?.Destroy();
        }
    }
}