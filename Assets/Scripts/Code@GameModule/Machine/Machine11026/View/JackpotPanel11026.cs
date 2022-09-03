// using System.Collections.Generic;
// using UnityEngine;
//
// namespace GameModule
// {
//     public class JackpotPanel11026:JackPotPanel
//     {
//         private List<bool> _lockState;
//         private List<Animator> _animators;
//         private List<Animator> animatorTips;
//
//         private bool _enableClickResponse = false;
//         
//         public JackpotPanel11026(Transform inTransform) : base(inTransform)
//         {
//             _animators = new List<Animator>();
//             animatorTips = new List<Animator>();
//             _lockState = new List<bool>();
//             for (var i = 0; i < listTextJackpot.Count; i++)
//             { 
//                var jackpotLevel = i;
//                var boxCollider = listTextJackpot[i].transform.parent.Find("DisableState/BG");
//                boxCollider.gameObject.AddComponent<BoxCollider2D>();
//                var eventHandler = boxCollider.gameObject.AddComponent<PointerEventCustomHandler>();
//                eventHandler.BindingPointerClick((data) =>
//                {
//                    OnJackpotClicked(jackpotLevel);
//                });
//
//                _lockState.Add(false);
//                _animators.Add(listTextJackpot[i].transform.parent.GetComponent<Animator>());
//                _animators[i].keepAnimatorControllerStateOnDisable = true;
//                // animatorTips.Add(this.transform.parent.Find($"Wheels/JackpotPanel/GroupTips/JackpotTips{i+1}Group").GetComponent<Animator>());
//                // animatorTips[i].keepAnimatorControllerStateOnDisable = true;
//             }
//         }
//
//         public void OnJackpotClicked(int level)
//         {
//             if (!_enableClickResponse)
//             {
//                 return;
//             }
//
//             bool isHave = false;
//             if (_lockState[level])
//             {
//                 
//                 _animators[level].Play("Open");
//                 var featureLevel = level;
//                 isHave = true;
//                  
//                 context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, featureLevel);
//             }
//
//             if (isHave)
//             {
//                 AudioUtil.Instance.PlayAudioFx("IncreaseTotalBet");
//             }
//         }
//         
//         public void UpdateJackpotLockState(bool hasAnimation, bool forceUpdate)
//         {
//             var betState = context.state.Get<BetState>();
//            
//             for (var i = 0; i < _lockState.Count; i++)
//             {
//                 var featureLevel = i;
//
//                 
//
//                 var isFeatureUnlocked = betState.IsFeatureUnlocked(featureLevel);
//                 LockJackpot(i,!isFeatureUnlocked, hasAnimation, forceUpdate);
//             }
//         }
//
//
//         protected Animator animatorPlayingTip;
//         
//         public void LockJackpot(int jackpotLevel, bool needLock, bool hasAnimation, bool forceUpdate = false)
//         {
//             if (forceUpdate || _lockState[jackpotLevel] != needLock)
//             {
//
//                 if (animatorPlayingTip != null)
//                 {
//                     animatorPlayingTip.Play("Idle",-1,0);
//                 }
//
//                 if (needLock)
//                 {
//                     if (hasAnimation)
//                     {
//                         AudioUtil.Instance.PlayAudioFx("ReduceTotalBet");
//                     }
//                     _animators[jackpotLevel].Play(hasAnimation ? "Disable" : "Disable");
//                     // animatorTips[jackpotLevel].Play(hasAnimation ? "Disable" : "Disable");
//                     // animatorPlayingTip = animatorTips[jackpotLevel];
//                 }
//                 else
//                 {
//                     if (hasAnimation)
//                     {
//                         AudioUtil.Instance.PlayAudioFx("IncreaseTotalBet");
//                     }
//                     
//                     _animators[jackpotLevel].Play(hasAnimation ? "Open" : "Idle");
//                     // animatorTips[jackpotLevel].Play(hasAnimation ? "Open" : "Idle");
//                     // animatorPlayingTip = animatorTips[jackpotLevel];
//                 }
//
//                 _lockState[jackpotLevel] = needLock;
//             }
//         }
//         
//         public void EnableButtonResponse(bool enable)
//         {
//             _enableClickResponse = enable;
//         }
//     }
// }