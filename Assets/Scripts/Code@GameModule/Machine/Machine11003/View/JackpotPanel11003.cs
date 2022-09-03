// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/06/15:29
// Ver : 1.0.0
// Description : JackpotPanel11003.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Field;
using UnityEngine;

namespace GameModule
{
    public class JackpotPanel11003:JackPotPanel
    {
        private List<bool> _lockState;
        private List<Animator> _animators;

        private bool _enableClickResponse = false;
        
        public JackpotPanel11003(Transform inTransform) : base(inTransform)
        {
            _animators = new List<Animator>();
            _lockState = new List<bool>();
            for (var i = 0; i < listTextJackpot.Count; i++)
            { 
               var jackpotLevel = i;
               var boxCollider = listTextJackpot[i].transform.parent.Find("BGhui");
               var eventHandler = boxCollider.gameObject.AddComponent<PointerEventCustomHandler>();
               eventHandler.BindingPointerClick((data) =>
               {
                   OnJackpotClicked(jackpotLevel);
               });

               _lockState.Add(false);
               _animators.Add(listTextJackpot[i].transform.parent.GetComponent<Animator>());
               _animators[i].keepAnimatorControllerStateOnDisable = true;
            }
        }

        public void OnJackpotClicked(int level)
        {
            if (!_enableClickResponse)
            {
                return;
            }
            
            if (_lockState[level])
            {
                _animators[level].Play("Unlocking");
                var featureLevel = level;

                if (featureLevel > 1)
                {
                    featureLevel += 1;
                }
                 
                context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, featureLevel);
            }
        }
        
        public void UpdateJackpotLockState(bool hasAnimation, bool forceUpdate)
        {
            var betState = context.state.Get<BetState>();
           
            for (var i = 0; i < _lockState.Count; i++)
            {
                var featureLevel = i;

                if (featureLevel > 1)
                {
                    featureLevel += 1;
                }

                var isFeatureUnlocked = betState.IsFeatureUnlocked(featureLevel);
                
                LockJackpot(i,!isFeatureUnlocked, hasAnimation, forceUpdate);
            }
        }
        
        public void LockJackpot(int jackpotLevel, bool needLock, bool hasAnimation, bool forceUpdate = false)
        {
            if (forceUpdate || _lockState[jackpotLevel] != needLock)
            {
                if (needLock)
                {
                    if (hasAnimation)
                    {
                        AudioUtil.Instance.PlayAudioFx("Jackpot_Lock");
                    }
                    _animators[jackpotLevel].Play(hasAnimation ? "Locking" : "Lock");
                }
                else
                {
                    if (hasAnimation)
                    {
                        AudioUtil.Instance.PlayAudioFx("Jackpot_Unlock");
                    }
                    
                    _animators[jackpotLevel].Play(hasAnimation ? "Unlocking" : "Unlock");
                }

                _lockState[jackpotLevel] = needLock;
            }
        }
        
        public void EnableButtonResponse(bool enable)
        {
            _enableClickResponse = enable;
        }
    }
}