using System;

namespace GameModule
{
    public interface IWheelSpinningController
    {
        int StartSpinning(Action<Wheel> onWheelSpinningEnd, Action<Wheel> onWheelAnticipationEnd, Action onCanEnableQuickStop, int inUpdaterIndex = 0);
        void BindingWheel(Wheel wheel);
        
        void ForceUpdateReels(string reelName);

        void OnLogicUpdate();

        bool OnSpinResultReceived(bool preWheelHasAnticipation);
        
        void OnQuickStopped();

        void CheckAndShowAnticipationAnimation();
    }
}