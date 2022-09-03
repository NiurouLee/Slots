//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 19:48
//  Ver : 1.0.0
//  Description : MainPickView11011.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class MainPickView11011: TransformHolder
    {
        private bool _isFinish;
        private bool _canPick;
        private Action _closeAction;
        private int _curPickIndex;
        private List<int> _listShuffleIds;
        private List<PickItemView> _listJackpots;
        private Dictionary<int, int> _dictResult;
        private Animator _animator;
        public MainPickView11011(Transform inTransform):base(inTransform)
        {
            _listShuffleIds = new List<int>();
            _listJackpots = new List<PickItemView>();
            _dictResult = new Dictionary<int, int>();
            _animator = transform.Find("SymbolsPointGroup").GetComponent<Animator>();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            _listJackpots.Clear();
            for (int i = 0; i < 12; i++)
            {
                var container = transform.Find($"SymbolsPointGroup/Point{i + 1}/Active_B07");
                var itemPick = new PickItemView(container);
                itemPick.actionClick += OnPickAction;
                _listJackpots.Add(itemPick);
            }
        }

        public async void InitJackpotView()
        {
            _canPick = true;
            _isFinish = true;
            _curPickIndex = 0;
            ShuffleJackpotIds();
            for (int i = 0; i < _listJackpots.Count; i++)
            {
                _listJackpots[i].InitJackpot();
                _listJackpots[i].PlayFly();
            }
            await XUtility.PlayAnimationAsync(_animator, "FlyIn");
            _isFinish = false;
        }

        private void ShuffleJackpotIds()
        {
            _listShuffleIds.Clear();
            var pickJackpotId = context.state.Get<ExtraState11011>().PickJackpotId;
            for (int i = 1; i <= 4; i++)
            {
                _dictResult[i] = 0;
            }
            for (int i = 0; i < 8; i++)
            {
                _listShuffleIds.Add(i%4+1);  
            }
            _listShuffleIds.Add(pickJackpotId);
            _listShuffleIds.Shuffle();
            AddNotPickedJackpot(pickJackpotId);
        }

        private void AddNotPickedJackpot(int pickJackpotId)
        {
            for (int i = 1; i <= 4; i++)
            {
                if (i != pickJackpotId)
                {
                    _listShuffleIds.Add(i);   
                }
            }    
        }

        private void OnPickAction(PickItemView pickItem)
        {
            if (_isFinish) return;
            if (!_canPick) return;
            _canPick = false;
            var jackpotId = _listShuffleIds[_curPickIndex++];
            pickItem.PlayFlip(jackpotId);
            _dictResult[jackpotId]++;
            CheckAndSetFinish(jackpotId);
        }

        private async void CheckAndSetFinish(int jackpotId)
        {
            if (_dictResult[jackpotId] == 2)
            {
                await context.WaitSeconds(0.5f);
                for (int i = 0; i < _listJackpots.Count; i++)
                {
                    var item = _listJackpots[i];
                    if (item.JackpotId == jackpotId && item.IsPiked)
                    {
                        item.PlayAnticipation();
                    }
                }
            }
            if (_dictResult[jackpotId] >= 3)
            {
                _isFinish = true;
                AudioUtil.Instance.PlayAudioFx("Jackpot_Finish");
            }
            _canPick = true;
            if (_isFinish)
            {
                await context.WaitSeconds(0.5f);
                for (int i = 0; i < _listJackpots.Count; i++)
                {
                    var item = _listJackpots[i];
                    if (item.JackpotId == jackpotId)
                    {
                        item.PlayWin();
                        continue;
                    }
                    item.PlayLose();
                    if (item.IsPiked)
                        continue;
                    item.UpdateJackpot(_listShuffleIds[_curPickIndex++]);
                }
                await context.WaitSeconds(3);
                _closeAction?.Invoke();
            }
        }

        public void ToggleVisible(bool visible)
        {
            transform.gameObject.SetActive(visible);
            context.state.Get<JackpotInfoState>().LockJackpot = visible;
        }

        public void BindFinishJackpotAction(Action action)
        {
            _closeAction = action;
        }
    }
}