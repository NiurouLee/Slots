//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-05 16:19
//  Ver : 1.0.0
//  Description : SmallSlotPaytableView.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SmallSlotPaytableView11016:TransformHolder
    {
        private int _gameId = 0;
        private Animator _animator;
        private List<Transform> listPaytables;
        public SmallSlotPaytableView11016(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            _animator = transform.GetComponent<Animator>();

            listPaytables = new List<Transform>();
            listPaytables.Push(null);
            var listPrefix = new List<string> {"Minor", "Major", "Grand"};
            for (int i = 0; i < listPrefix.Count; i++)
            {
                var prefix = listPrefix[i];
                listPaytables.Push(transform.Find($"PayTableGroup/{prefix}Group"));
            }
        }

        public void ShowPaytable(int gameId)
        {
            if (_gameId == gameId)
                return;
            _gameId = gameId;
            transform.gameObject.SetActive(true);
            XUtility.PlayAnimation(_animator, "Open");
            for (int i = 0; i < listPaytables.Count; i++)
            {
                listPaytables[i]?.gameObject.SetActive(i==gameId);
            }

            AudioUtil.Instance.PlayAudioFx("J01_Patyble");
        }

        public async void ClosePaytable()
        {
            await XUtility.PlayAnimationAsync(_animator, "Close"); 
            transform.gameObject.SetActive(false);
            _gameId = 0;
        }
    }
}