namespace GameModule
{
    public class IndependentSpinningController11024<TWheelAnimationController>:IndependentSpinningController<TWheelAnimationController>where TWheelAnimationController : IWheelAnimationController
    {
        protected override bool IsColumnHasAnticipation(int columnIndex)
        {
            var wheelConfig = wheelState.GetWheelConfig();

            if (columnIndex >= wheelConfig.rollCount)
            {
                return false;
            }

            var startRollIndex = columnIndex * wheelConfig.rollRowCount;
            var endRollIndex = startRollIndex + wheelConfig.rollRowCount;

            for (int i = startRollIndex; i < endRollIndex; i++)
            {
                if (wheelState.HasAnticipationAnimationInRollIndex(i))
                {
                    return true;
                }
            }
            return false;
        }
        
        public override bool OnSpinResultReceived(bool preWheelHasAnticipation)
        {
            if (runningUpdater.Count == 1)
            {
                wheelToControl.GetContext().WaitSeconds(0.5f, () =>
                {
                    base.OnSpinResultReceived(preWheelHasAnticipation);
                });
                return true;
            }
            else
            {
                base.OnSpinResultReceived(preWheelHasAnticipation);
                return false;
            }
        }
    }
}