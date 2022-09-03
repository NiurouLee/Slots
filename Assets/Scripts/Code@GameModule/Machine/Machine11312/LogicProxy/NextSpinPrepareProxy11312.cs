using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule{
    public class NextSpinPrepareProxy11312 : NextSpinPrepareProxy
    {
        public BetState BetState;
        public NextSpinPrepareProxy11312(MachineContext context) : base(context)
        {
            BetState = machineContext.state.Get<BetState>();
        }

        protected override void OnUnlockBetFeatureConfigChanged()
        {
            var betState = machineContext.state.Get<BetState>();
            var jackpotView = machineContext.view.Get<JackpotView11312>();
            jackpotView.InitSetJackpotStatus(betState.IsFeatureUnlocked(0));

        }
       

        protected override void OnBetChange()
        {
            base.OnBetChange();
            
            UpdateUnJackpotStatusLogic();
            
            var wheel = machineContext.state.Get<WheelsActiveState11312>().GetRunningWheel();
          
            if (wheel.Count > 0)
            {
                var elementContainers = wheel[0].GetElementMatchFilter((ElementContainer container) =>
                {
                    if (container.GetElement() is ElementCoin11312)
                        return true;
                    return false;
                });

                if (elementContainers.Count > 0)
                {
                    for (var i = 0; i < elementContainers.Count; i++)
                    {
                        var element = (ElementCoin11312) elementContainers[i].GetElement();
                        element.UpdateElementContent();
                    }
                }
            }
        }
        
        /// <summary>
        /// 更新解锁jackpot的状态
        /// </summary>
        public void UpdateUnJackpotStatusLogic(){
            // 最终把获取的unlockJackpot的下标，在jackpot中显示出来
            var isUnLocked = JudgeUnlockJackpotLevel();
            machineContext.view.Get<JackpotView11312>().UpdateUnJackpotStatus(isUnLocked,true);
        }

        /// <summary>
        /// 判断是否已经解锁
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool JudgeUnlockJackpotLevel(){
            return BetState.IsFeatureUnlocked(0);
        }

        /// <summary>
        /// 点击jackpot，反向设置totalBet与betLevel
        /// </summary>
        /// <param name="index"></param>
        public void HitJackpotUnlockTotalBet()
        {
            machineContext.state.Get<BetState>().SetBetBigEnoughToUnlockFeature(0);
            UpdateSpinUiViewTotalBet(false);

        }
    }
}

