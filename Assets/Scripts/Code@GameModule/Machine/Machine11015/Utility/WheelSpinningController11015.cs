using System;
using System.Collections.Generic;

namespace GameModule
{
   
    public class WheelSpinningController11015<TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
        where TWheelAnimationController : IWheelAnimationController
    {
        public override int StartSpinning(Action<Wheel> inOnWheelSpinningEnd, Action<Wheel> inOnWheelAnticipationEnd, Action inOnCanEnableQuickStop,
            int inUpdaterIndex = 0)
        {
            
            bool hasShield = false;
            int countRoll = wheelToControl.rollCount;
            for (int column = 0; column < countRoll; column++)
            {
                var roll = wheelToControl.GetRoll(column);
                var listElement = roll.GetVisibleSequenceElement();
            
                for (int i = 0; i < listElement.Count; i++)
                {
                    if (listElement[i]!=null && listElement[i].config.id == Constant11015.ShieldElementId)
                    {
                        hasShield = true;
                        break;
                    }
                }

                if (hasShield)
                {
                    break;
                }
            }


            if (hasShield)
            {
                AudioUtil.Instance.PlayAudioFx("Turn", true);
            }

            return base.StartSpinning(inOnWheelSpinningEnd, inOnWheelAnticipationEnd, inOnCanEnableQuickStop, inUpdaterIndex);
        }

        protected List<int> listWildRoll = new List<int>();
        public override bool OnSpinResultReceived(bool preWheelHasAnticipation)
        {
            int drumReelFirstIndex = wheelState.GetAnticipationAnimationStartRollIndex();


            var extraState = this.wheelToControl.GetContext().state.Get<ExtraState11015>();
            listWildRoll.Clear();
            
            for (var i = 0; i < runningUpdater.Count; i++)
            {
                int rollIndex = runningUpdater[i].RollIndex;
                bool needWaitAnticipation = (startUpdaterIndex > 0 && preWheelHasAnticipation) || rollIndex >= drumReelFirstIndex;

                if (!extraState.HasWildReel(rollIndex))
                {
                    runningUpdater[i].OnSpinResultReceived(needWaitAnticipation);
                }
                else
                {
                    listWildRoll.Add(i);
                }
                
                
            }


            if (listWildRoll.Count == 5)
            {
                SlowStopWildReels();
            }



            if (startUpdaterIndex == 0)
            {
                CheckAndShowAnticipationAnimation();
            }

            return (drumReelFirstIndex < wheelToControl.GetMaxSpinningUpdaterCount());

        }


        public override void OnRollLogicEnd()
        {
            base.OnRollLogicEnd();
            if (listWildRoll.Count > 0 && listWildRoll.Count!=5)
            {
                if (finishUpdaterIndex == runningUpdater.Count - listWildRoll.Count)
                {
                    SlowStopWildReels();
                }
            }
        }


        protected async void SlowStopWildReels()
        {
            if (listWildRoll.Count > 0)
            {
                await this.wheelToControl.GetContext().WaitSeconds(1);
                for (int i = 0; i < listWildRoll.Count; i++)
                {
                    runningUpdater[listWildRoll[i]].OnSpinResultReceived(false);
                }
                
                
            }
        }
    }
    
    
}