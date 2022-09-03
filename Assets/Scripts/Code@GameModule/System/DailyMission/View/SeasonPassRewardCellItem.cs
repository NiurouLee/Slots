//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-08 16:19
//  Ver : 1.0.0
//  Description : SeasonPassRewardCellItem.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class SeasonPassRewardCellItem: View<SeasonPassRewardCellItemViewController>
    {
        
        [ComponentBinder("Fx/NormalFx")] 
        public Transform _normalfxTransform;
        
        [ComponentBinder("Fx/GoldenFx")] 
        public Transform _goldenfxTransform;
        //Normal
        [ComponentBinder("CollectState")] 
        public Transform _transCollectState;
        [ComponentBinder("FinishState")] 
        public Transform _transFinishState;
        
        [ComponentBinder("Icon")] private Image _image;
        [ComponentBinder("CountGroup")] private Transform _transGroup;
        [ComponentBinder("CountGroup/CountText")] private TextMeshProUGUI _txtCount;
        [ComponentBinder("CountGroupGolden")] private Transform _transGoldenGroup;
        [ComponentBinder("CountGroupGolden/CountText")] private TextMeshProUGUI _txtGoldenCount;

        //Limited
        [ComponentBinder("TimerButtonState")]
        public Transform _transLeftTimeState;
        [ComponentBinder("TimerButtonState/TimerText")]
        public TextMeshProUGUI _txtTimeLeft;
        [ComponentBinder("TimeOverState")]
        public Transform _transTimeOverState;
        [ComponentBinder("DoneState")]
        public Transform _transDoneState;
        
        [ComponentBinder("LockState")] 
        public Transform _transLockState;

        public int _level;
        public bool _isGolden;
        public int _rewardIndex;
        public MissionPassReward _reward;


        private string _txtBubbleContent;
        private SeasonPassPageViewController _pageViewController;
        private SeasonPassController _passController;

        private bool _canCollect;
        
        

        public SeasonPassRewardCellItem()
        {
            _passController = Client.Get<SeasonPassController>();
        }

        public void InitReward(SeasonPassPageViewController pageViewController, bool isGolden, int level, int rewardIndex)
        {
            _level = level;
            _isGolden = isGolden;
            _rewardIndex = rewardIndex;
            _pageViewController = pageViewController;
            RefreshUI();
        }

        public void RefreshUI()
        {
            var rewardItems = _isGolden
                ? _pageViewController.GetGoldenRewards(_level)
                : _pageViewController.GetFreeRewards(_level);
            _reward = rewardItems[_rewardIndex];
            var isPaid = _isGolden ? _passController.Paid : true;
            var isLevelReach = _reward.Level <= _passController.Level;
            var reachCollect = isLevelReach && !_reward.TimeOver && !_reward.Collected;
            _canCollect =  reachCollect && isPaid;
            var needLock = !isPaid;
            SetTransformActive(_normalfxTransform, !_isGolden && _reward.ParticleAnima > 0 && !_reward.Collected);
            SetTransformActive(_goldenfxTransform, _isGolden && _reward.ParticleAnima > 0 && !_reward.Collected);
            SetTransformActive(_transTimeOverState, _reward.TimeOver);  
            //SetTransformActive(transDisableState, _reward.TimeOver || _reward.Collected);   
            SetTransformActive(_transFinishState, _reward.Collected);
            //等级没有达到，付费奖励但玩家没付费
            SetTransformActive(_transCollectState,  _canCollect);
            SetTransformActive(_transLockState, needLock);
            SetTransformActive(_transGroup, !_isGolden);
            SetTransformActive(_transGoldenGroup, _isGolden);
            if (_image)
            {
               ShowItemImage();
            }

            if (_txtTimeLeft)
            {
                _txtTimeLeft.text = Client.Get<SeasonPassController>().GetLimitedMissionTimeLeft(_reward);   
            }
            UpdateLeftTimeState();
        }

        public void UpdateLeftTimeState()
        {
            if (_txtTimeLeft)
            {
                CheckIsTimeOver();
                SetTransformActive(_normalfxTransform, !_isGolden && _reward.ParticleAnima > 0 && !_reward.Collected);
                SetTransformActive(_goldenfxTransform, _isGolden && _reward.ParticleAnima > 0 && !_reward.Collected);
                
                SetTransformActive(_transLockState,false);
                SetTransformActive(_transDoneState, false);
                SetTransformActive(_transTimeOverState, false);
                SetTransformActive(_transLeftTimeState, false);
                if (_canCollect || _reward.Collected)
                {
                    _txtBubbleContent = "";
                    SetTransformActive(_transDoneState, true);     
                }
                else if (_reward.IsElapsing && _passController.GetLimitedMissionTimeLeftLong(_reward) > 0)
                {
                    SetTransformActive(_transLeftTimeState, true);
                    _txtBubbleContent = $"REACH LEVEL {_reward.Level}\nIN {_passController.GetLimitedMissionTimeLeft(_reward)} TO WIN";
                    _txtTimeLeft.text = _passController.GetLimitedMissionTimeLeft(_reward);
                }

                else if (_reward.TimeOver)
                {
                    _txtBubbleContent = "TIME OVER\nTHIS REWARD HAS EXPIRED";
                    SetTransformActive(_transTimeOverState, true);
                }
                else
                {
                    _txtBubbleContent = "";
                    SetTransformActive(_transLockState,true);
                }
            }
        }

        private void CheckIsTimeOver()
        {
            if (_reward.IsElapsing && _passController.GetLimitedMissionTimeLeftLong(_reward) <= 0 && _reward.TimestampLeft > 0)
            {
                _reward.TimeOver = true;
            }
        }

        private void ShowItemImage()
        {
            var item =  _reward.Reward.Items[0];
            
            if (item != null)
            {
                _txtCount.text = "";
                _txtGoldenCount.text = "";
                _image.gameObject.SetActive(true);
                _image.sprite = XItemUtility.GetItemSprite(item.Type, item);

                if (_image.sprite == null)
                {
                    XDebug.Log(item.Type);
                }
               
                if (_txtCount && !_reward.Collected)
                {
                    if (item.Type == Item.Types.Type.Coin)
                    {
                        _txtCount.text = _reward.Desc;
                    }
                    else
                    {
                        _txtCount.text = XItemUtility.GetItemDefaultDescText(item);
                    }
                    _txtGoldenCount.text = _txtCount.text;
                }
            }
        }

        [ComponentBinder("CollectState")]
        public async void OnBtnCollectClick()
        {
            if (_isGolden)
            {
                var popup = await PopupStack.ShowPopup<SeasonPassRewardGolden>();
                popup.InitRewards(new RepeatedField<Reward>{_reward.Reward},_isGolden,_reward);
            }
            else
            {
                var popup = await PopupStack.ShowPopup<SeasonPassRewardFree>();
                popup.InitRewards(new RepeatedField<Reward>{_reward.Reward},_isGolden,_reward);
            }
        }

        [ComponentBinder("ButtonTips")]
        private void OnBtnTipsClick()
        {
            if (_reward.Collected) return;
            if (_txtBubbleContent.Length>0)
            {
                _pageViewController.PopupBubble(_txtTimeLeft.transform);
                UpdateBubbleContent();   
            }
        }

        public void UpdateBubbleContent()
        {
            _pageViewController.UpdateBubbleContent(_txtTimeLeft.transform, _txtBubbleContent);
        }
    }

    public class SeasonPassRewardCellItemViewController : ViewController<SeasonPassRewardCellItem>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
        }

        public override void Update()
        {
            base.Update();
            if (view._reward != null && view._reward.TimestampLeft > 0)
            {
                if (view._txtTimeLeft)
                {
                    view.UpdateLeftTimeState();
                    view.UpdateBubbleContent();
                }
            }
        }
    }
}