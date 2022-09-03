using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class SubRoundStartProxy11026 : SubRoundStartProxy
    {
        ElementConfigSet elementConfigSet = null;
        private FreeSpinState freeSpinState;
        private ReSpinState reSpinState;
        private LockElementLayer11026 _layer;
        public List<DragonRisingGameResultExtraInfo.Types.Position> lastWildPosList;

        public SubRoundStartProxy11026(MachineContext context)
            : base(context)
        {
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            lastWildPosList = new List<DragonRisingGameResultExtraInfo.Types.Position>();
            reSpinState = machineContext.state.Get<ReSpinState>();
        }

        public override void SetUp()
        {
            base.SetUp();
            var wheel = machineContext.view.Get<IrregularWheel>(1);
            machineContext.view.Add<LockElementLayer11026>(wheel.transform);
             _layer = machineContext.view.Get<LockElementLayer11026>();
            _layer.BindingWheel(wheel);
            _layer.SetSortingGroup("Element", 9999);
        }


        protected override void HandleCommonLogic()
        {
            //superfree时将wild固定住
            if (machineContext.state.Get<ExtraState11026>().GetIsSuper() &&
                (!reSpinState.ReSpinTriggered && !reSpinState.NextIsReSpin))
            {
                _layer.ShowStickyWildElement();
            }

            machineContext.view.Get<LinkLockView11026>().RefreshReSpinCount();
            base.HandleCommonLogic();
        }
    }
}