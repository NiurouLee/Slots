// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/11/11:17
// Ver : 1.0.0
// Description : LevelController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class LevelController : LogicController
    {
        private LevelUpInfo _levelUpInfo;

        private List<uint> _levelsNeedBiPoint = new List<uint>
        {
            5, 9, 10, 15, 20, 23, 25, 30, 32, 35, 40, 50, 60, 65,
            70, 75, 80, 90, 95, 100, 105, 110, 120, 130, 150, 160,
            180, 200, 220, 240, 260, 280, 320, 350, 380, 400, 430,
            450, 470, 500, 520, 550, 570, 600
        };
        
        public LevelController(Client client) :
            base(client)
        {
        }
        
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.LevelUp);
        }

        protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (_levelUpInfo != null)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelUp, ("level",_levelUpInfo.Level.ToString()));
                
                EventBus.Dispatch(new EventLevelChanged(_levelUpInfo));
              
                if (_levelUpInfo.Ui == 1)
                {
                    EventBus.Dispatch(
                        new EventShowPopup(new PopupArgs(typeof(LevelUpPopUp), _levelUpInfo, handleEndCallback)));
                    _levelUpInfo = null;
    
                    return;
                }

                EventBus.Dispatch(new EventLevelAchieved(_levelUpInfo));
                _levelUpInfo = null;
            }
            
            handleEndCallback?.Invoke();
        }

        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            _levelUpInfo = GetSystemData<LevelUpInfo>(evt.systemContent, "LevelUpInfo");

            if (_levelUpInfo != null)
            {
                EventBus.Dispatch(new EventPreNoticeLevelChanged(_levelUpInfo));
                EventBus.Dispatch(new EventUpdateExp(true));
                LogBiEvent(_levelUpInfo);
            }
            else
            {
                EventBus.Dispatch(new EventUpdateExp(false));
            }
        }

        protected void LogBiEvent(LevelUpInfo levelUpInfo)
        {
            var newLevel = levelUpInfo.Level;
           
            if (_levelsNeedBiPoint.Contains(newLevel))
            {
                var eventType = (BiEventFortuneX.Types.GameEventType)Enum.Parse(typeof(BiEventFortuneX.Types.GameEventType), "GameEventPassLv" + newLevel);
                
                BiManagerGameModule.Instance.SendGameEvent(eventType);
            }
        }
    }
}