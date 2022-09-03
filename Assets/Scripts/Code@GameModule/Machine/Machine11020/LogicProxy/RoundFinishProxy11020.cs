//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-12-13 13:24
//  Ver : 1.0.0
//  Description : RoundFinishProxy11020.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class RoundFinishProxy11020:RoundFinishProxy
    {
        public RoundFinishProxy11020(MachineContext context)
            : base(context)
        {
            
        }

        protected override void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            CheckNewFireBall();
        }

        private void CheckNewFireBall()
        {
            var extraState = machineContext.state.Get<ExtraState11020>();
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState11020>();
            var panel = wheelsActiveState.GetPanel();
            if (panel == null)
            {
                return;
            }

            var betFrames = extraState.GetBetLockedFrames(); 
            var isSuperBonusGame = machineContext.view.Get<LockedFramesView11020>().isSuperBonusGame;

            var bonusframes = isSuperBonusGame ? extraState.GetSuperBonusLockedFrames() : null;
            
            uint index = 1;
            uint id    = 0;

            var columns = panel.Columns;

            var bUpdateCorner = false;

            for (var i = 0; i < columns.Count; ++i)
            {
                var symbols = columns[i].Symbols;

                for (var k = 0; k < symbols.Count; ++k)
                {
                    id = symbols[k];

                    if (id == Constant11020.lionElement && betFrames != null && betFrames.Contains(index))
                    {
                        if (!isSuperBonusGame || bonusframes == null || !bonusframes.Contains(index))
                        {
                            if (!machineContext.view.Get<LockedFramesView11020>().HasLockedFrame(index))
                            {
                                machineContext.state.Get<WheelsActiveState11020>().ListNewIds.Add(index);
                            }
                        }
                    }
                    index++;
                }
            }
        }
    }
}