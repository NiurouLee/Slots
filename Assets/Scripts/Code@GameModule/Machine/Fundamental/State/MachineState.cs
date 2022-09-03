// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 11:48 AM
// Ver : 1.0.0
// Description : MachineState.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class MachineState
    {
        private readonly List<SubState> subStates;

        public MachineContext machineContext;
        public MachineConfig machineConfig;

        public MachineState(MachineContext context, MachineConfig config)
        {
            machineContext = context;
            machineConfig = config;
            subStates = new List<SubState>();
        }

        public T Add<T>(object args = null) where T : SubState
        {
            return (T) Add(typeof(T), args);
        }

     
        public TTarget Replace<TSource,TTarget>(object args = null) where TSource : SubState where TTarget: SubState
        {
            TSource t = Get<TSource>();
            if (t != null)
            {
                Remove(t);
            }

            return Add<TTarget>(args);
        }

        public bool Remove(SubState t)
        {
            return subStates.Remove(t);
        }

        public T Get<T>(int index = 0) where T : SubState
        {
            int count = 0;
            for (var i = 0; i < subStates.Count; i++)
            {
                if (subStates[i] is T)
                {
                    if (count == index)
                        return (T) subStates[i];
                    count++;
                }
            }

            return null;
        }
        
        public T Get<T>(string filter) where T : SubState
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                if (subStates[i] is T)
                {
                   if (subStates[i].MatchFilter(filter))
                        return (T) subStates[i];
                }
            }

            return null;
        }
         
        public List<T> GetAll<T>() where T : SubState
        {
            var listT = new List<T>();
            for (var i = 0; i < subStates.Count; i++)
            {
                if (subStates[i] is T)
                {
                    listT.Add((T)subStates[i]);
                }
            }
            
            return listT;
        }

        public object Add(Type type, object args)
        {
            SubState state;
            if (args != null)
                state = (SubState) Activator.CreateInstance(type, this, args);
            else
                state = (SubState) Activator.CreateInstance(type, this);

            subStates.Add(state);
            return state;
        }

        public void UpdateStateOnSubRoundStart()
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnSubRoundStart();
            }
        }

        public void UpdateStateOnSubRoundFinish()
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnSubRoundFinish();
            }
        }

        public void UpdateMachineStateOnSpinResultReceived(SSpin spinResult)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnReceiveSpinResult(spinResult);
            }
        }
        
        public void UpdateStatePreRoomSetUp(SEnterGame enterGameInfo)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStatePreRoomSetUp(enterGameInfo);
            }
        }

        public void UpdateStateOnRoomSetUp(SEnterGame enterGameInfo)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnRoomSetUp(enterGameInfo);
            }
        }

        public void UpdateStateBeforeCallRoundFinish()
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateBeforeCallRoundFinish();
            }
        }

        public void UpdateStateOnRoundFinish()
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnRoundFinish();
            }
        }

        public void UpdateStateOnRoundStart()
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnRoundStart();
            }
        }

        public void UpdateStateOnBonusProcess(SBonusProcess sBonusProcess)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnBonusProcess(sBonusProcess);
            }
        }
        
        public void UpdateStateOnSettleProcess(SSettleProcess settleProcess)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnSettleProcess(settleProcess);
            }
        }

        public void UpdateStateOnSpecialProcess(SSpecialProcess sSpecialProcess)
        {
            for (var i = 0; i < subStates.Count; i++)
            {
                subStates[i].UpdateStateOnSpecialProcess(sSpecialProcess);
            }
        }

    }
}