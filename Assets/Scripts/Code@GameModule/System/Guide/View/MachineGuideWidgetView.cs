//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-20 13:52
//  Ver : 1.0.0
//  Description : MachineGuideWidgetView.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class GuideMachineWidgetView:TransformHolder
    {
        private GuideWidget _levelGuideWidget;
        public GuideMachineWidgetView(Transform transform)
            : base(transform)
        {
        
        }
        
        public async Task LoadGuideWidgetView()
        {
            if (!Client.Get<GuideController>().IsLevelGuideFinished())
            {
                _levelGuideWidget = await View.CreateView<GuideWidget>("LevelGuide", transform);   
            }
        }

        public override void OnDestroy()
        {
            _levelGuideWidget?.Destroy();
        }
    }
}