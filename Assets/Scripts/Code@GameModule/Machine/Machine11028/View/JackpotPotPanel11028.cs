//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-07 15:07
//  Ver : 1.0.0
//  Description : JackpotPotPanel11028.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class JackpotPotPanel11028: TransformHolder
    {
        private Animator animator;
        private List<bool> _lockDayState;
        private List<Animator> _animatorsDay;
        private List<bool> _lockNightState;
        private List<Animator> _animatorsNight;
        protected JackpotInfoState jackpotInfoState;
        protected List<TextMesh> listDayTextJackpot = new List<TextMesh>();
        protected List<TextMesh> listNightTextJackpot = new List<TextMesh>();
        

        public JackpotPotPanel11028(Transform inTransform) : base(inTransform)
        {

            _animatorsDay = new List<Animator>();
            _animatorsNight = new List<Animator>();
            _lockDayState = new List<bool>();
            _lockNightState = new List<bool>();
            
            int index = 0;
            animator = transform.GetComponent<Animator>();
            var prefixs = new[] {"FiveGroup","SixGroup","SevenGroup","EightGroup","NineGroup"};
            while (index < 5 && transform.Find($"DayGroup/{prefixs[index]}/IntegralText"))
            {
                var transformTxt = transform.Find($"DayGroup/{prefixs[index]}/IntegralText");
                if (transformTxt && transformTxt.GetComponent<TextMesh>())
                {
                    listDayTextJackpot.Add(transformTxt.GetComponent<TextMesh>());
                    var parent = transform.Find($"DayGroup/{prefixs[index]}").GetComponent<Animator>();
                    _animatorsDay.Add(parent);
                    _lockDayState.Add(false);
                    _animatorsDay[_animatorsDay.Count - 1].keepAnimatorControllerStateOnDisable = true;
                }
                var transformNightTxt = transform.Find($"NightGroup/{prefixs[index]}/IntegralText");
                if (transformNightTxt && transformNightTxt.GetComponent<TextMesh>())
                {
                    listNightTextJackpot.Add(transformNightTxt.GetComponent<TextMesh>());      
                    var parent = transform.Find($"NightGroup/{prefixs[index]}").GetComponent<Animator>();
                    _animatorsNight.Add(parent);
                    _lockNightState.Add(false);
                    _animatorsNight[_animatorsNight.Count-1].keepAnimatorControllerStateOnDisable = true;
                }
                index++;
            }
        }
         
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            jackpotInfoState = context.state.Get<JackpotInfoState>();
            
            UpdateJackpotValue();
        }


        public override void Update()
        {
            base.Update();

            if (Time.frameCount % 3 == 0)
            {
                if (transform.gameObject.activeSelf)
                {
                    UpdateJackpotValue();
                }   
            }
        }
        
        public virtual void UpdateJackpotValue()
        {
            for (int i = 0; i < listDayTextJackpot.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listDayTextJackpot[i])
                {
                    listDayTextJackpot[i].SetText(numJackpot.GetCommaFormat());   
                }
                numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+6));
                if (listNightTextJackpot[i])
                {
                    listNightTextJackpot[i].SetText(numJackpot.GetCommaFormat());   
                }
            }
        }

        public void PlayAnimation(string prefix="", uint jackpotCount=0)
        {
            if (jackpotCount>0)
            {
                XUtility.PlayAnimation(animator,$"{prefix}_JackpotPanel_{jackpotCount}");   
            }
            else
            {
                XUtility.PlayAnimation(animator,"Idle");        
            }
        }
        
        public void UpdateJackpotLockState(bool hasAnimation, bool forceUpdate)
        {
            var betState = context.state.Get<BetState>();
           
            for (var i = 0; i < _lockDayState.Count; i++)
            {
                var isFeatureUnlocked = betState.IsFeatureUnlocked(i);
                
                LockJackpot(i,!isFeatureUnlocked, hasAnimation, forceUpdate);
            }
        }
        
        public void LockJackpot(int jackpotLevel, bool needLock, bool hasAnimation, bool forceUpdate = false)
        {
            if (forceUpdate || _lockDayState[jackpotLevel] != needLock)
            {
                if (needLock)
                {
                    if (hasAnimation)
                    {
                        AudioUtil.Instance.PlayAudioFx("Jackpot_Lock");
                    }
                    _animatorsDay[jackpotLevel].Play("Disable");
                    _animatorsNight[jackpotLevel].Play("Disable");
                }
                else
                {
                    if (hasAnimation)
                    {
                        AudioUtil.Instance.PlayAudioFx("Jackpot_UnLock");
                    }
                    
                    _animatorsDay[jackpotLevel].Play(hasAnimation ? "Open" : "Idle");
                    _animatorsNight[jackpotLevel].Play(hasAnimation ? "Open" : "Idle");
                }

                _lockDayState[jackpotLevel] = needLock;
                _lockNightState[jackpotLevel] = needLock;
            }
        }
    }
}