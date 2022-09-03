// // **********************************************
// // Copyright(c) 2021 by com.ustar
// // All right reserved
// // 
// // Author : Jian.Wang
// // Date : 2021/10/08/11:25
// // Ver : 1.0.0
// // Description : TimeBonusLobbyEntranceView.cs
// // ChangeLog :
// // **********************************************
//
// using System;
// using System.Collections.Generic;
// using DragonU3DSDK.Network.API.ILProtocol;
//
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace GameModule
// {
//     public class TimeBonusLobbyEntranceViewD : View<TimeBonusLobbyEntranceViewControllerD>
//     {
//         [ComponentBinder("CoinGroup")] public Transform coinGroup;
//
//         [ComponentBinder("CoinWaitState")] public Transform coinWaitState;
//
//         [ComponentBinder("CoinTimerText")] public TextMeshProUGUI coinTimerText;
//
//         [ComponentBinder("LuckyWheelTimerText")]
//         public TextMeshProUGUI luckyWheelTimerText;
//
//         [ComponentBinder("SuperWheelBonusGroup")]
//         public Transform superWheelBonusGroup;
//
//         [ComponentBinder("LuckWheelFillGroup")]
//         public Transform luckWheelFillGroup;
//
//         [ComponentBinder("SuperWheelFillGroup")]
//         public Transform superWheelFillGroup;
//
//         [ComponentBinder("CoinFillGroup")] public Transform coinFillGroup;
//
//         [ComponentBinder("CoinGroup/CoinWaitState/CoinIconGroup/IconGroup/CoinIcon")]
//         public Transform coinIcon;
//
//         [ComponentBinder("LuckyWheelBonusGroup/LuckyWheelWaitState/LuckyWheelIconGroup/IconGroup/CoinIconBG")]
//         public Transform luckWheelIcon;
//
//         [ComponentBinder("SuperWheelBonusGroup/SuperWheelWaitingState/SuperWheelIconGroup/IconGroup/CoinIconBG")]
//         public Transform superWheelIcon;
//
//         [ComponentBinder("CoinCollectButton")] public Button coinCollectButton;
//
//         [ComponentBinder("LuckyWheelSpinButton")]
//         public Button luckyWheelSpinButton;
//
//         [ComponentBinder("SuperWheelSpinButton")]
//         public Button superWheelSpinButton;
//
//         [ComponentBinder("CoinGroup/CoinCollectState/IconGroup/CoinIcon/jinbi_Animation/UIParticle/jinbi")]
//         public Button collectCoinIconButton;
//
//         [ComponentBinder("CoinGroup/CoinCollectState/IconGroup/CoinIcon")]
//         public Transform collectCoinIcon;
//
//         [ComponentBinder("LuckyWheelBonusGroup/LuckyWheelSpinState/Mask/IconGroup/CoinIconBG")]
//         public Button collectLuckyWheelIconButton;
//
//         [ComponentBinder("SuperWheelBonusGroup/SuperWheelSpinState/IconGroup/CoinIconBG")]
//         public Button collectSuperWheelIconButton;
//
//         [ComponentBinder("LuckyWheelBonusGroup/LuckyWheelWaitState/InformationGroup/GetBuffBG")]
//         public Transform wheelBonusBuffBg;
//
//         public List<Transform> coinStageUIList;
//         public List<Transform> superWheelStageUIList;
//         public List<Transform> luckyWheelStageUIList;
//
//         public Animator animator;
//
//         protected override void SetUpExtraView()
//         {
//             base.SetUpExtraView();
//
//             animator = transform.GetComponent<Animator>();
//             animator.keepAnimatorControllerStateOnDisable = true;
//
//             coinStageUIList = new List<Transform>();
//             for (var i = 0; i < coinFillGroup.childCount; i++)
//             {
//                 coinStageUIList.Add(coinFillGroup.GetChild(i));
//             }
//
//             luckyWheelStageUIList = new List<Transform>();
//             for (var i = 0; i < luckWheelFillGroup.childCount; i++)
//             {
//                 luckyWheelStageUIList.Add(luckWheelFillGroup.GetChild(i));
//             }
//
//             superWheelStageUIList = new List<Transform>();
//             for (var i = 0; i < superWheelFillGroup.childCount; i++)
//             {
//                 superWheelStageUIList.Add(superWheelFillGroup.GetChild(i));
//             }
//         }
//     }
//
//     public class TimeBonusLobbyEntranceViewControllerD : ViewController<TimeBonusLobbyEntranceViewD>
//     {
//         private TimeBonusController _bonusController;
//
//         private Vector3 _iconWaitInitPosition;
//         private Vector3 _iconWaitTargetPosition;
//         private Vector3 _superWheelWaitInitPosition;
//         private Vector3 _superWheelTargetPosition;
//         private Vector3 _luckyWheelWaitInitPosition;
//         private Vector3 _luckyWheelTargetPosition;
//         private string _lastAnimationStateName;
//
//         protected ClientBuff superWheelBuff;
//
//         public override void OnViewDidLoad()
//         {
//             base.OnViewDidLoad();
//
//             _bonusController = Client.Get<TimeBonusController>();
//
//             UpdateUIContent();
//  
//             //  RefreshEntranceUI();
//
//             //  EnableUpdate(2);
//
//             superWheelBuff = Client.Get<BuffController>().GetBuff<SuperWheelBuff>();
//
//             if (superWheelBuff != null)
//             {
//                 view.wheelBonusBuffBg.gameObject.SetActive(true);
//             }
//         }
//
//         protected override void SubscribeEvents()
//         {
//             base.SubscribeEvents();
//
//             SubscribeEvent<EventTimeBonusStateChanged>(OnTimeBonusStateChanged);
//             SubscribeEvent<EventBuffDataUpdated>(OnBufferDataUpdate);
//
//             view.coinCollectButton.onClick.AddListener(OnCoinCollectClicked);
//             view.luckyWheelSpinButton.onClick.AddListener(OnLuckyWheelCollectClicked);
//             view.superWheelSpinButton.onClick.AddListener(OnSuperWheelCollectClicked);
//
//             view.collectCoinIconButton.onClick.AddListener(OnCoinCollectClicked);
//             view.collectLuckyWheelIconButton.onClick.AddListener(OnLuckyWheelCollectClicked);
//             view.collectSuperWheelIconButton.onClick.AddListener(OnSuperWheelCollectClicked);
//         }
//
//         protected void OnBufferDataUpdate(EventBuffDataUpdated evt)
//         {
//             superWheelBuff = Client.Get<BuffController>().GetBuff<SuperWheelBuff>();
//
//             if (superWheelBuff != null)
//             {
//                 view.wheelBonusBuffBg.gameObject.SetActive(true);
//             }
//         }
//
//         protected void OnTimeBonusStateChanged(EventTimeBonusStateChanged evt)
//         {
//             UpdateUIContent();
//
//             RefreshEntranceUI();
//         }
//
//         public void UpdateUIContent()
//         {
//             var coinProgress = _bonusController.GetCoinProgress();
//             var wheelProgress = _bonusController.GetLuckyWheelProgress();
//
//             for (var i = 0; i < view.coinStageUIList.Count; i++)
//             {
//                 view.coinStageUIList[i].gameObject.SetActive(i < coinProgress);
//             }
//
//             for (var i = 0; i < view.luckyWheelStageUIList.Count; i++)
//             {
//                 view.luckyWheelStageUIList[i].gameObject.SetActive(i < coinProgress);
//             }
//
//             for (var i = 0; i < view.superWheelStageUIList.Count; i++)
//             {
//                 view.superWheelStageUIList[i].gameObject.SetActive(i < wheelProgress);
//             }
//         }
//
//         public override void OnViewEnabled()
//         {
//             base.OnViewEnabled();
//             
//             _iconWaitInitPosition = view.coinIcon.position;
//             _superWheelWaitInitPosition = view.superWheelIcon.position;
//             _luckyWheelWaitInitPosition = view.luckWheelIcon.position;
//
//             _iconWaitTargetPosition = view.collectCoinIcon.transform.position;
//             _superWheelTargetPosition = view.collectSuperWheelIconButton.transform.position;
//             _luckyWheelTargetPosition = view.collectLuckyWheelIconButton.transform.position;
//              
//             RefreshEntranceUI();
//         }
//
//         public void RefreshEntranceUI()
//         {
//             var countDown = _bonusController.GetTimeBonusCountdown();
//
//             string targetAnimationState = "";
//
//             if (_bonusController.IsBonusReady(TimerBonusStage.SuperWheelBonus))
//             {
//                 targetAnimationState = "SuperWheelCollect";
//                 view.superWheelSpinButton.interactable = true;
//                 view.collectSuperWheelIconButton.interactable = true;
//             }
//             else if (_bonusController.IsBonusReady(TimerBonusStage.LuckyWheelBonus))
//             {
//                 targetAnimationState = "LuckyWheelCollect";
//                 
//                 view.luckyWheelSpinButton.interactable = true;
//                 view.collectLuckyWheelIconButton.interactable = true;
//             }
//             else
//             {
//                 if (_bonusController.GetCoinProgress() != _bonusController.GetMaxCoinProgress())
//                 {
//                     if (countDown <= 0)
//                     {
//                         targetAnimationState = "CoinCollect";
//                         view.coinCollectButton.interactable = true;
//                         view.collectCoinIconButton.interactable = true;
//                     }
//                     else
//                     {
//                         if (_bonusController.GetLuckyWheelProgress() > 0)
//                         {
//                             targetAnimationState = "CoinSuperWheelWait";
//                         }
//                         else
//                         {
//                             targetAnimationState = "CoinWait";
//                         }
//                     }
//                 }
//                 else
//                 {
//                     if (countDown <= 0)
//                     {
//                         targetAnimationState = "LuckyWheelCollect";
//                     }
//                     else
//                     {
//                         if (_bonusController.GetLuckyWheelProgress() > 0)
//                         {
//                             targetAnimationState = "LuckyWheelSuperWheelWait";
//                         }
//                         else
//                         {
//                             targetAnimationState = "LuckyWheelWait";
//                         }
//                     }
//                 }
//             }
//
//             var switchState = _lastAnimationStateName + "To" + targetAnimationState;
//             if (view.animator.HasState(switchState) &&
//                 !view.animator.GetCurrentAnimatorStateInfo(0).IsName(switchState))
//             {
//                 XDebug.Log(switchState);
//                 view.animator.Play(switchState);
//             }
//             else if (!view.animator.GetCurrentAnimatorStateInfo(0).IsName(targetAnimationState))
//             {
//                 XDebug.Log(targetAnimationState);
//                 view.animator.Play(targetAnimationState);
//             }
//
//             _lastAnimationStateName = targetAnimationState;
//             //}
//
//             if (countDown > 0)
//             {
//                 EnableUpdate(2);
//             }
//         }
//
//         public override void Update()
//         {
//             var countDown = _bonusController.GetTimeBonusCountdown();
//
//             if (countDown > 0)
//             {
//                 view.coinTimerText.text = "FREE COINS " + TimeSpan.FromSeconds(countDown).ToString(@"hh\:mm\:ss");
//                 view.luckyWheelTimerText.text =
//                     "LUCKY WHEEL " + TimeSpan.FromSeconds(countDown).ToString(@"hh\:mm\:ss");
//
//                 if (_lastAnimationStateName.Contains("Wait"))
//                 {
//                     view.coinIcon.position =
//                         Vector3.Lerp(_iconWaitInitPosition, _iconWaitTargetPosition, 1 - (countDown / (float)_bonusController.GetBonusInterval()));
//                     view.luckWheelIcon.position = Vector3.Lerp(_luckyWheelWaitInitPosition,
//                         _luckyWheelTargetPosition,
//                         1 - (countDown / (float)_bonusController.GetBonusInterval()));
//                     var t = (float) _bonusController.GetLuckyWheelProgress() /
//                             _bonusController.GetMaxLuckyWheelProgress();
//                     if (view.superWheelIcon.gameObject.activeInHierarchy)
//                         view.superWheelIcon.position = Vector3.Lerp(_superWheelWaitInitPosition,
//                             _superWheelTargetPosition, t);
//                 }
//
//                 if (superWheelBuff != null && superWheelBuff.GetBuffLeftTimeInSecond() <= 0)
//                 {
//                     view.wheelBonusBuffBg.gameObject.SetActive(false);
//                 }
//             }
//             else
//             {
//                 RefreshEntranceUI();
//                 DisableUpdate();
//             }
//         }
//
//         public void OnCoinCollectClicked()
//         {
//             SoundController.PlayButtonClick();
//             view.coinCollectButton.interactable = false;
//             view.collectCoinIconButton.interactable = false;
//             if (_bonusController.GetTimeBonusState() ==
//                 TimerBonusStage.HourlyBonus && _bonusController.GetTimeBonusCountdown() <= 0)
//             {
//                 _bonusController.ClaimTimeBonus(async item =>
//                 {
//                     BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTimerBonusHourlyCollect, ("source", "Lobby"));
//                     if (item != null)
//                     {
//                         await XUtility.FlyCoins(view.coinCollectButton.transform,
//                             new EventBalanceUpdate(item.Coin.Amount, "HourlyBonus"));
//
//                         OnTimeBonusStateChanged(new EventTimeBonusStateChanged());
//                     }
//                     // else
//                     // {
//                     //     CommonNoticePopup.ShowCommonNoticePopUp("Unknown error Occured! Please try again later");
//                     // }
//                 });
//             }
//         }
//
//         public  void OnLuckyWheelCollectClicked()
//         {
//            // SoundController.PlayButtonClick();
//             view.luckyWheelSpinButton.interactable = false;
//             view.collectLuckyWheelIconButton.interactable = false;
//             EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusWheelBonusPopup),"Lobby")));
//             //await PopupStack.ShowPopup<TimeBonusWheelBonusPopup>();
//         }
//
//         public  void OnSuperWheelCollectClicked()
//         {
//             //SoundController.PlayButtonClick();
//             view.superWheelSpinButton.interactable = false;
//             view.collectSuperWheelIconButton.interactable = false;
//             EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TimeBonusSuperWheelPopup),"Lobby")));
//             //await PopupStack.ShowPopup<TimeBonusSuperWheelPopup>();
//         }
//     }
// }