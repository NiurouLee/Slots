// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/24/15:59
// Ver : 1.0.0
// Description : ExpBuffView.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameModule
{
    public class BuffView:View
    {
        [ComponentBinder("DescriptionText")]
        public TextMeshProUGUI descriptionText;      
        
        [ComponentBinder("TimerText")]
        public TextMeshProUGUI timerText;
 
        private ClientBuff _clientBuff;
         
        public virtual void UpdateBuffText()
        {
            descriptionText.text = _clientBuff.GetDescText();
            timerText.text =  XUtility.GetTimeText(_clientBuff.GetBuffLeftTimeInSecond());;
        }

        public virtual bool IsBuffStillValid()
        {
            if (_clientBuff.GetBuffLeftTimeInSecond() <= 0)
            {
                return false;
            }

            return true;
        }
         
        public void BindingBuff(ClientBuff clientBuff)
        {
            _clientBuff = clientBuff;
            UpdateBuffText();
        }

        public Sequence switchSeq;

        public void PlaySwitchAnimation(Action finishCallback)
        {
            descriptionText.transform.localScale = new Vector3(1, 0, 1);
            timerText.transform.localScale = new Vector3(1, 0, 1);
            timerText.gameObject.SetActive(false);
            descriptionText.gameObject.SetActive(true);

            if (switchSeq != null && !switchSeq.IsComplete())
                switchSeq.Kill();
            
            switchSeq = DOTween.Sequence();
            switchSeq.Append(descriptionText.transform.DOScaleY(1, 0.25f));
            switchSeq.AppendInterval(2.75f);
            switchSeq.Append(descriptionText.transform.DOScaleY(0, 0.25f));
            switchSeq.AppendCallback(() =>
            {
                timerText.gameObject.SetActive(true);
                descriptionText.gameObject.SetActive(false);
            });
            switchSeq.Append(timerText.transform.DOScaleY(1, 0.25f));
            switchSeq.AppendInterval(2.75f);
            switchSeq.Append(timerText.transform.DOScaleY(0, 0.25f));
            switchSeq.AppendCallback(() =>
            {
                switchSeq = null;
                if(transform != null)
                    finishCallback?.Invoke();
            });
            
            switchSeq.Play();
        }

        public override void Destroy()
        {
            if (switchSeq != null)
            {
                if (switchSeq != null && !switchSeq.IsComplete())
                    switchSeq.Kill();
            }
            
            base.Destroy();
        }
    }

    public class LevelRushView : BuffView
    {
        private LevelRushController _levelRushController;

        public LevelRushView()
        {
            _levelRushController = Client.Get<LevelRushController>();
        }
        public override void UpdateBuffText()
        {
            descriptionText.text =_levelRushController.GetLevelRushTopPanelDescText();
            timerText.text =  XUtility.GetTimeText(_levelRushController.GetLevelRushLeftTime());;
        }
        
        public override bool IsBuffStillValid()
        {
            var leftTime = _levelRushController.GetLevelRushLeftTime();
            if (leftTime <= 0 || !_levelRushController.IsLevelRushEnabled())
            {
                return false;
            }

            return true;
        }

        public override void Destroy()
        {
            base.Destroy();
            
            // if (!IsBuffStillValid())
            // {
            //     EventBus.Dispatch(new EventLevelRushStateChanged());
            // }
        }
    }
    
    public class ExpProgressView:View<ExpProgressViewController>
    {
        [ComponentBinder("BuffGroup")] 
        public Transform buffGroup;
        
        [ComponentBinder("ProgressBar")]
        public Slider progressBar;
        
        [ComponentBinder("ProgressBar/ProgressText")]
        public TextMeshProUGUI progressText;
        
        [ComponentBinder("BuffProgressBar/ProgressText")]
        public TextMeshProUGUI buffProgressText;
        
        [ComponentBinder("BuffProgressBar")]
        public Slider buffProgressBar;

        [ComponentBinder("ExpInfoButton")]
        public Button expInfoButton;
        
        [ComponentBinder("ProgressBar/Icon")]
        public Transform pBarIcon;
        
        [ComponentBinder("BuffProgressBar/Icon")]
        public Transform pBuffBarIcon;
        
        [ComponentBinder("LevelRush")]
        public Transform levelRush;
        
        [ComponentBinder("BottomBG")]
        public Transform bottomBg;
 
       
        public List<BuffView> buffViewList;
      
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            
            buffViewList = new List<BuffView>();
            buffGroup.gameObject.SetActive(false);
        }

        public void UpdateExpProgress(float levelUpProgress, uint currentLevel, bool playAnimation = true)
        {
            var activeProgressbar = progressBar.gameObject.activeSelf ? progressBar : buffProgressBar;
            var activeProgressText = activeProgressbar == progressBar ? progressText : buffProgressText;

            if (activeProgressbar.value > levelUpProgress)
            {
                activeProgressbar.value = levelUpProgress;
            }

            if (playAnimation && levelUpProgress > activeProgressbar.value)
            {
                activeProgressbar.DOValue(levelUpProgress, 0.5f).OnUpdate(() =>
                {
                    activeProgressText.text = Math.Floor(activeProgressbar.value * 100) + "%";
                }).OnComplete(() =>
                {
                    activeProgressText.text = currentLevel.ToString();
                    activeProgressbar.value = levelUpProgress;
                });
            }
            else
            {
                activeProgressText.text = currentLevel.ToString();
                activeProgressbar.value = levelUpProgress;
            }
        }

        public void EnableExpBuff(bool enable, float levelUpProgress, uint currentLevel, bool levelRushEnabled)
        {
            progressBar.gameObject.SetActive(!enable);
            buffProgressBar.gameObject.SetActive(enable);
            progressBar.value = levelUpProgress;
            progressText.text = currentLevel.ToString();
            buffProgressBar.value = levelUpProgress;
            buffProgressText.text = currentLevel.ToString();
             
            bottomBg.gameObject.SetActive(enable && !levelRushEnabled);
            
            levelRush.gameObject.SetActive(levelRushEnabled);
           
            if (enable)
            {
                pBuffBarIcon.gameObject.SetActive(!levelRushEnabled);
            }
            else
            {
                pBarIcon.gameObject.SetActive(!levelRushEnabled);
            }
        }
        
        public void AddBuffView(ClientBuff clientBuff)
        {
            var newBuff = GameObject.Instantiate(buffGroup.gameObject, buffGroup.parent);
            
            newBuff.gameObject.SetActive(true);
            
            var buffView = AddChild<BuffView>(newBuff.transform);
            buffView.BindingBuff(clientBuff);
            buffView.Hide();
            buffViewList.Add(buffView);
        }
        
        public void AddLevelRushView()
        {
            var newBuff = GameObject.Instantiate(buffGroup.gameObject, buffGroup.parent);
            newBuff.gameObject.SetActive(true);
            var buffView = AddChild<LevelRushView>(newBuff.transform);
            buffView.Hide();
            buffViewList.Add(buffView);
        }

        public void RemoveLevelRushView()
        {
            if (buffViewList.Count > 0)
            {
                for (var i = 0; i < buffViewList.Count; i++)
                {
                    if (buffViewList[i] is LevelRushView)
                    {
                        buffViewList.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        
        public void RemoveAllBuff()
        {
            for (var i = buffViewList.Count - 1; i >= 0; i--)
            {
                RemoveChild(buffViewList[i]);
            }
            
            buffViewList.Clear();
            currentPlayIndex = 0;
        }
        
        public void UpdateBuff()
        {
            if (buffViewList.Count == 0)
            {
                return;
            }

            bool needPlayBuffSwitchAnimation = false;
            
            for (var i = buffViewList.Count - 1; i >= 0; i--)
            {
                if (!buffViewList[i].IsBuffStillValid())
                {
                    if (buffViewList[i] is LevelRushView)
                    {
                        EventBus.Dispatch(new EventLevelRushStateChanged());
                        return;
                    }
                    
                    RemoveChild(buffViewList[i]);
                    buffViewList.RemoveAt(i);

                    if (i == currentPlayIndex)
                    {
                        needPlayBuffSwitchAnimation = true;
                    }
                }
                else
                {
                    buffViewList[i].UpdateBuffText();
                }
            }

            if (needPlayBuffSwitchAnimation)
            {
                PlayBuffSwitchAnimation();
            }
        }

        public int currentPlayIndex = 0;

        public void PlayBuffSwitchAnimation()
        {
            if (buffViewList.Count <= 0)
                return;

            if (buffViewList.Count <= currentPlayIndex)
            {
                currentPlayIndex = 0;
            }

            var playIndex = currentPlayIndex;
            buffViewList[playIndex].Show();
            buffViewList[playIndex].PlaySwitchAnimation(() =>
            {
                buffViewList[playIndex].Hide();
                playIndex++;
                currentPlayIndex = playIndex;
                PlayBuffSwitchAnimation();
            });
        }
    }

    public class ExpProgressViewController : ViewController<ExpProgressView>
    {
        private List<ClientBuff> activeBuff;

        private LevelRushController _levelRushController;
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventUpdateExp>(OnUpdateUserExp);
            SubscribeEvent<EventBuffDataUpdated>(EventBuffDataUpdated);
            SubscribeEvent<EventLevelRushStateChanged>(OnEventLevelRushStateChanged);
            
            view.expInfoButton.onClick.AddListener(OnExpInfoButtonClicked);

            base.SubscribeEvents();
        }

        public void OnExpInfoButtonClicked()
        {
            if (view.levelRush.gameObject.activeInHierarchy)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LevelRushPopup), "ExpBarView")));
            }
            else
            {
                var parentView = view.GetParentView();

                if (parentView != null)
                {
                    var topPanel = parentView as TopPanel;
                    if (topPanel != null)
                        topPanel.viewController.OnExpInfoButtonClicked();
                }
            }
        }

        public void EventBuffDataUpdated(EventBuffDataUpdated evt)
        {
            RefreshBuffView();
        }
        
        public void OnEventLevelRushStateChanged(EventLevelRushStateChanged evt)
        {
            RefreshBuffView();
        }

        public void RefreshBuffView()
        {
            CollectActiveBuff();
            
            view.RemoveAllBuff();
            
            var userLevelInfo = Client.Get<UserController>().GetUserLevelInfo();
            
            bool levelRushEnabled = _levelRushController.IsLevelRushEnabled();

            view.EnableExpBuff(activeBuff.Count > 0 || levelRushEnabled, (float)userLevelInfo.ExpCurrent / (userLevelInfo.ExpNextLevel + userLevelInfo.ExpCurrent), userLevelInfo.Level, levelRushEnabled);

            if (levelRushEnabled)
            {
                view.AddLevelRushView();
            }
            
            if (activeBuff.Count > 0)
            {
                for (var i = 0; i < activeBuff.Count; i++)
                {
                    view.AddBuffView(activeBuff[i]);
                }
            }
            
            if (view.buffViewList.Count > 0)
            {
                EnableUpdate(2);
                view.PlayBuffSwitchAnimation();
            }
        }
        
        public override void OnViewEnabled()
        {
            if (!view.transform.gameObject.activeInHierarchy)
                return;

            _levelRushController = Client.Get<LevelRushController>();
           
            base.OnViewEnabled();
            
            view.RemoveAllBuff();
            
            activeBuff = new List<ClientBuff>();
            
            CollectActiveBuff();
            
            var userLevelInfo = Client.Get<UserController>().GetUserLevelInfo();

            bool levelRushEnabled = _levelRushController.IsLevelRushEnabled();
            
            view.EnableExpBuff(activeBuff.Count > 0 || levelRushEnabled, (float)userLevelInfo.ExpCurrent / (userLevelInfo.ExpNextLevel + userLevelInfo.ExpCurrent), userLevelInfo.Level, levelRushEnabled);

            view.UpdateExpProgress(
                (float) userLevelInfo.ExpCurrent / (userLevelInfo.ExpNextLevel + userLevelInfo.ExpCurrent),
                userLevelInfo.Level, false);

            if (levelRushEnabled)
            {
                view.AddLevelRushView();
            }
            
            if (activeBuff.Count > 0)
            {
                for (var i = 0; i < activeBuff.Count; i++)
                {
                    view.AddBuffView(activeBuff[i]);
                }
            }

            if (view.buffViewList.Count > 0)
            {
                view.PlayBuffSwitchAnimation();
            }
            
            EnableUpdate(2);
        }

        public override void Update()
        {
            view.UpdateBuff();
         
            if (view.buffViewList.Count <= 0)
            {
                var userLevelInfo = Client.Get<UserController>().GetUserLevelInfo();
                view.EnableExpBuff(false,
                    (float) userLevelInfo.ExpCurrent / (userLevelInfo.ExpNextLevel + userLevelInfo.ExpCurrent),
                    userLevelInfo.Level, false);
              
                DisableUpdate();
            }
        }

        protected void CollectActiveBuff()
        {
            if (activeBuff == null)
            {
                activeBuff = new List<ClientBuff>();
            }
            activeBuff.Clear();
            
            var doubleExpBuff = Client.Get<BuffController>().GetBuff<DoubleExpBuff>();
            var levelBurstBuff = Client.Get<BuffController>().GetBuff<LevelUpBurstBuff>();

            if (doubleExpBuff != null)
            {
                activeBuff.Add(doubleExpBuff);
            }

            if (levelBurstBuff != null)
            {
                activeBuff.Add(levelBurstBuff);
            }
        }
        protected void OnUpdateUserExp(EventUpdateExp evt)
        {
            var userLevelInfo = Client.Get<UserController>().GetUserLevelInfo();
            if (evt.updateToFull)
            {
                view.UpdateExpProgress(1, userLevelInfo.Level - 1, true);
            }
            else
            {
                view.UpdateExpProgress(
                    (float) userLevelInfo.ExpCurrent / (userLevelInfo.ExpNextLevel + userLevelInfo.ExpCurrent),
                    userLevelInfo.Level, true);
            }
        }
    }
}