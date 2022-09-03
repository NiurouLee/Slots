//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-12-07 21:47
//  Ver : 1.0.0
//  Description : LackOfBetTipView.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class LackOfBetTipView:View<LackOfBetTipViewController>
    {
        public Animator Animator;
        public LackOfBetTipView(string address)
            : base(address)
        {

        }

        protected override void EnableView()
        {
            Hide();
            Animator = transform.GetComponent<Animator>();
        }
    }

    public class LackOfBetTipViewController : ViewController<LackOfBetTipView>
    {
        private bool isClosing = false;
        public MachineContext Context;
        public List<MissionValidHandler> listValidHandler;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            listValidHandler = new List<MissionValidHandler>();
            listValidHandler.Add(new DailyMissionValidHandler());
            SubscribeEvent<EventSpinRoundStart>(OnSpinRoundStart);
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventBetChanged>(OnBetChanged);
        }
        
        public void Initialize(MachineContext context)
        {
            Context = context;
        }
        
        
        public async void ShowTip()
        {
            view.Show();
            await XUtility.PlayAnimationAsync(view.Animator,"Open",this);
            await Context.WaitSeconds(5f);
            CloseTip();
        }

        private void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            CheckAndShowTip();
        }

        public void OnSpinRoundStart(EventSpinRoundStart evt)
        {
            CheckAndShowTip();
        }

        private void OnBetChanged(EventBetChanged evt)
        {
            CloseTip();
            for (int i = 0; i < listValidHandler.Count; i++)
            {
                var handler = listValidHandler[i];
                if (handler != null)
                {
                    handler.OnBetChanged();
                }
            }  
        }

        private async void CloseTip()
        {
            if (!isClosing && view.transform.gameObject.activeSelf)
            {
                isClosing = true;
                await XUtility.PlayAnimationAsync(view.Animator,"Close",this);
                view.Hide();
                isClosing = false;
            }
        }

        private void CheckAndShowTip()
        {
            for (int i = 0; i < listValidHandler.Count; i++)
            {
                var handler = listValidHandler[i];
                if (handler.CheckValid(Context))
                {
                    ShowTip();
                    break;
                }
            }   
        }
    }

    public class MissionValidHandler
    {
        public virtual bool CheckValid(MachineContext context)
        {
            return true;
        }

        public virtual void OnBetChanged()
        {
            
        }
    }
    
    public class DailyMissionValidHandler: MissionValidHandler
    {
        public ulong curTotalBet;
        public override void OnBetChanged()
        {
            curTotalBet = 0;
        }

        public override bool CheckValid(MachineContext context)
        {
            var totalBet = context.state.Get<BetState>().totalBet;
            var mission = Client.Get<DailyMissionController>().CurrentMission;
            if (mission != null && !mission.IsFinish() &&
                mission.NeedMinimumBetMission &&
                mission.ExpectedBet > totalBet && curTotalBet != totalBet)
            {
                curTotalBet = totalBet;
                return true;
            }
            return false;
        }
    }
}