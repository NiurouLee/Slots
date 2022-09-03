//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 20:16
//  Ver : 1.0.0
//  Description : PickItemView.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameModule
{
    public class PickItemView: TransformHolder
    {
        public bool IsPiked;
        public int JackpotId;
        private Animator _animator;
        private PointerEventCustomHandler _monoProxy;
        private List<string> _listJackpotName = new List<string>{"Mini", "Minor", "Major", "Grand"};
        private List<Transform> _listJackpot;
        public Action<PickItemView> actionClick;
        public PickItemView(Transform inTransform):base(inTransform)
        {
            _listJackpot = new List<Transform>();
            for (int i = 0; i < _listJackpotName.Count; i++)
            {
                _listJackpot.Add(transform.Find("Root/SymbolsGroup/Jackpot/"+_listJackpotName[i]));
            }
            _animator = transform.GetComponent<Animator>();
            _monoProxy = transform.Bind<PointerEventCustomHandler>(true);
            _monoProxy.BindingPointerClick((a) =>
            {
                if (IsPiked) return;
                actionClick?.Invoke(this);
            });
        }

        public void InitJackpot()
        {
            JackpotId = -1;
            IsPiked = false;
            XUtility.PlayAnimation(_animator, "Idle");
        }

        public void PlayFlip(int jackpotId)
        {
            IsPiked = true;
            JackpotId = jackpotId;
            UpdateJackpot(jackpotId);
            XUtility.PlayAnimation(_animator, "Flip");
            AudioUtil.Instance.PlayAudioFx("Jackpot_Switch_Pick");
        }

        public void UpdateJackpot(int jackpotId)
        {
            for (int i = 0; i < _listJackpot.Count; i++)
            {
                if (_listJackpot[i].name.Contains(_listJackpotName[jackpotId-1]))
                {
                    _listJackpot[i].gameObject.SetActive(true);
                }
                else
                {
                    _listJackpot[i].gameObject.SetActive(false);   
                }
            }
        }
        public async void PlayFly()
        {
            await XUtility.PlayAnimationAsync(_animator, "Fly");
        }
        public void PlayLose()
        {
            XUtility.PlayAnimation(_animator, "EndShow");     
        }
        public void PlayWin()
        {
            XUtility.PlayAnimation(_animator, "Pick3");
        }
        public void PlayAnticipation()
        {
            XUtility.PlayAnimation(_animator, "Pick2");
        }
    }
}