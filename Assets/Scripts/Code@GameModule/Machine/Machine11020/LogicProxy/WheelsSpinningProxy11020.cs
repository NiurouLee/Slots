
using System;
using UnityEngine;

namespace GameModule
{
    public class WheelsSpinningProxy11020 : WheelsSpinningProxy
    {
        private LockedFramesView11020 lockedFramesView;
        private WheelView11020 wheelView;

        private bool showRandomFramesFinish;
        private bool runWheelFinish;

        public WheelsSpinningProxy11020(MachineContext context)
            : base(context)
        {
            
        }
        
        public override void SetUp()
        {
            base.SetUp();

            wheelView = machineContext.view.Get<WheelView11020>();

            lockedFramesView = machineContext.view.Get<LockedFramesView11020>();
        }

        public override void OnSpinResultReceived()
        {
            showRandomFramesFinish = false;
            runWheelFinish         = false;

            lockedFramesView.ShowRandomFrames(OnRandomFrameDisplayFinish);

            if (wheelView != null && wheelView.IsActive())
            {
                float delaySeconds = lockedFramesView.GetRandomFrameCount() * 1.0f;
                
                wheelView.StopWheel(OnRunWheelFinish, delaySeconds);
            }
            else
            {
                OnRunWheelFinish();
            }         
        }

        public void OnRunWheelFinish()
        {
            runWheelFinish = true;

            if (showRandomFramesFinish && runWheelFinish)
            {
                base.OnSpinResultReceived();
            }
        }

        public void OnRandomFrameDisplayFinish()
        {
            showRandomFramesFinish = true;

            if (showRandomFramesFinish && runWheelFinish)
            {
                base.OnSpinResultReceived();
            }
        }
    }
}
