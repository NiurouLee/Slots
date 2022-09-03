// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/15/14:23
// Ver : 1.0.0
// Description : LuckySpinPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule 
{
	[AssetAddress("UILuckySpinPopup")]
	public class LuckySpinPopup: Popup<LuckySpinPopupViewController>
	{
		[ComponentBinder("AdaptNode/Root/RotateAll/ShaftMask/AnimControl")]
		public RectTransform animControl;

		[ComponentBinder("AdaptNode")] public RectTransform adaptNode;
		
		[ComponentBinder("AdaptNode/Root/RotateAll/ShaftMask/ElementTemplate")]
		public RectTransform elementTemplate;

		[ComponentBinder("AdaptNode/Root/RotateAll/StopButton")]
		public Button stopButton;

		[ComponentBinder("AdaptNode/Root/RotateAll/SpinLeftCountText")]
		public Text spinLeftCountText;

		[ComponentBinder("AdaptNode/Root/RotateAll/AutoStopButton")]
		public Button autoStopButton;

		[ComponentBinder("AdaptNode/Root/RotateAll/SpinButton")]
		public Button spinButton;
		
		[ComponentBinder("AdaptNode/Root/TopGroup/HelpButton")]
		public Button helpButton;

		[ComponentBinder("AdaptNode/Root/SpinAllButton")]
		public Button spinAllButton;

		[ComponentBinder("AdaptNode/Root/CheckPrizeButton")]
		public Button checkPrizeButton;

		[ComponentBinder("AdaptNode/Root/BubbleTip")]
		public RectTransform bubbleTip;
		 

		[ComponentBinder("AdaptNode/Root/RotateAll/ShaftMask/AnimControl/Roll")]
		public RectTransform roll;
		
		public HoldClickButton holdClickSpinButton;


		public LuckySpinPopup(string address)
			:base(address)
		{
		}

		protected override void OnViewSetUpped()
		{
			base.OnViewSetUpped();
			
			AdaptScaleTransform(adaptNode, new Vector2(1200,768));
		}

		public override float GetPopUpMaskAlpha()
		{
			return 0;
		}

		protected override void OnCloseClicked()
		{
			base.OnCloseClicked();
			BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "LuckySpinCloseButton"),("OperationId","6"));
		}
	}
	public class LuckySpinPopupViewController: ViewController<LuckySpinPopup>,IElementProvider
	{
		public SingleColumnWheel _wheel;

		private SimpleRollUpdaterEasingConfig _easingConfig;

		protected SGetCardLuckySpinInfo sGetCardLuckySpinInfo;

		protected AlbumController albumController;

		protected int spinResultIndex = 0;

		protected bool isAutoSpin = false;

		protected bool seasonIsEnd = false;
		protected bool isPlaying = false;
		
		public override void OnViewDidLoad()
		{
			base.OnViewDidLoad();

			albumController = Client.Get<AlbumController>();
			
			_wheel = new SingleColumnWheel(view.animControl, 385, 1, this, sGetCardLuckySpinInfo.SpinRewardConfs.Count - 1);

			_easingConfig = new SimpleRollUpdaterEasingConfig();
			_easingConfig.leastSpinDuration = 2;
			_easingConfig.spinSpeed = 20;
			_easingConfig.speedUpDuration = 0.5f;
			_easingConfig.slowDownStepCount = 3;
			_easingConfig.overShootAmount = 1.5f;
			_easingConfig.stopIntervalTime = 1f;
		}

		public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
		{
			base.BindingView(inView, inExtraData, inExtraAsyncData);
			
			sGetCardLuckySpinInfo = inExtraAsyncData as SGetCardLuckySpinInfo;
		}
 
		public int GetReelMaxLength()
		{
			return sGetCardLuckySpinInfo.SpinRewardConfs.Count;
		}
		public GameObject GetElement(int index)
		{
			Transform element = null;

			if (quickStopIndex >= 0)
			{
				if (index == (quickStopIndex + 1) % GetReelMaxLength())
				{
					index = spinResultIndex;
				}
			}

			var rewardConfig = sGetCardLuckySpinInfo.SpinRewardConfs[index];
			switch (rewardConfig.Reward.Items[0].Type)
			{
				case Item.Types.Type.Coin:
					if (rewardConfig.IsSuperBonus)
					{
						element = GameObject.Instantiate(view.elementTemplate.Find("Bonus").gameObject).transform;
					}
					else
					{
						element = GameObject.Instantiate(view.elementTemplate.Find("Coin").gameObject).transform;
						element.transform.Find("CountText").GetComponent<TMP_Text>().text =
							rewardConfig.Reward.Items[0].Coin.Amount.GetCommaFormat();
					}
					break;
				
				case Item.Types.Type.Emerald:
					element = GameObject.Instantiate(view.elementTemplate.Find("Emerald").gameObject).transform;
					element.transform.Find("CountText").GetComponent<TMP_Text>().text =
						rewardConfig.Reward.Items[0].Emerald.Amount.GetCommaFormat();
					break;
				
				case Item.Types.Type.CardPackage:
					var cardPackage = rewardConfig.Reward.Items[0].CardPackage;
					var luckyPackage = cardPackage.PackageConfig.TypeForShow == 20;
					element = GameObject.Instantiate(view.elementTemplate
						.Find(luckyPackage ? "LuckyCardPackage" : "NormalCardPackage").gameObject).transform;
					element.transform.Find("CountText").GetComponent<TMP_Text>().text = "+1";
					break;
			
				case Item.Types.Type.VipPoints:
					element = GameObject.Instantiate(view.elementTemplate.Find("VipPoints").gameObject).transform;
					element.transform.Find("CountText").GetComponent<TMP_Text>().text =
						rewardConfig.Reward.Items[0].VipPoints.Amount.GetCommaFormat();
					break;
			}

			if (element == null)
			{
				element = new GameObject("UnknowReward").transform;
			}

			return element.gameObject;
		}

		public int GetElementMaxHeight()
		{
			return 1;
		}

		public int quickStopIndex = -1;

		public int OnReelStopAtIndex(int currentIndex)
		{
			quickStopIndex = currentIndex;
			return quickStopIndex;
		}

		public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
		{
			return (spinResultIndex - 1 + GetReelMaxLength() ) % GetReelMaxLength();
		}

		public void UpdateLuckySpinCount(uint spinCount)
		{
			if (spinCount > 1)
				view.spinLeftCountText.text = $"{spinCount} SPINS LEFT";
			else
				view.spinLeftCountText.text = $"{spinCount} SPIN LEFT";
		}

		public void UpdateViewUIState()
		{
			var luckySpinCount = albumController.GetLuckySpinCount();
			UpdateLuckySpinCount(albumController.GetLuckySpinCount());
			
			view.spinAllButton.gameObject.SetActive(luckySpinCount > 0);
			view.checkPrizeButton.interactable = sGetCardLuckySpinInfo.RewardsForCollect.count > 0;
			view.holdClickSpinButton.interactable = albumController.GetLuckySpinCount() > 0;
			view.autoStopButton.gameObject.SetActive(false);
			view.stopButton.gameObject.SetActive(false);
		}

		protected override void SubscribeEvents()
		{
			var spinButtonGameObject = view.spinButton.gameObject;

			var colors = view.spinButton.colors;
			UnityEngine.Object.DestroyImmediate(view.spinButton);
			
			view.holdClickSpinButton = spinButtonGameObject.AddComponent<HoldClickButton>();
			view.holdClickSpinButton.transition = Selectable.Transition.ColorTint;
			view.holdClickSpinButton.colors = colors;
		  
			view.holdClickSpinButton.onLongClick.AddListener(OnSpinButtonLongClick);
			view.holdClickSpinButton.onClick.AddListener(OnSpinButtonClicked);
			
			view.stopButton.onClick.AddListener(OnStopButtonClicked);
			view.autoStopButton.onClick.AddListener(OnAutoStopButtonClicked);
			
			view.spinAllButton.onClick.AddListener(OnSpinAllButtonClicked);
			view.checkPrizeButton.onClick.AddListener(OnCheckPrizeButtonClicked);
			
			view.closeButton = view.transform.Find("AdaptNode/Root/TopGroup/CloseButton").GetComponent<Button>();
			view.closeButton.onClick.RemoveAllListeners();
			view.closeButton.onClick.AddListener(OnCloseButtonClicked);
			
			view.helpButton.onClick.AddListener(OnHelpButtonClick);

			SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
			
			base.SubscribeEvents();
		}

		protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
		{
			seasonIsEnd = true;

			if (!isPlaying)
			{
				view.Close();
			}
		}

		private bool _spinResultReceived = false;
		
		protected async void OnHelpButtonClick()
		{
			var popup = await PopupStack.ShowPopup<TravelAlbumHelpPopup>();
			popup.viewController.ShowPage(3);
		}
		protected void OnSpinButtonLongClick()
		{
			StartSpinning(true);
		}

		protected void ToggleButton(bool enable)
		{
			view.stopButton.interactable = enable;
			view.autoStopButton.interactable = enable;
			view.holdClickSpinButton.interactable = enable;
		}
		public void OnStopButtonClicked()
		{
			if (_spinResultReceived)
			{
				SoundController.StopSfx("Album_LuckySpin_Start2");
			    _wheel.OnQuickStopped();
				view.stopButton.interactable = false;
			}
		}
		public void OnSpinEndCallback()
		{
			DisableUpdate();
			
			//ToggleButton(true);
			
			view.animator.Play("Show_Stop");
			SoundController.PlaySfx("Album_LuckySpin_Win");
			
			view.stopButton.gameObject.SetActive(false);

			
			if (!isAutoSpin)
			{
				view.holdClickSpinButton.gameObject.SetActive(true);
				view.holdClickSpinButton.interactable = albumController.GetLuckySpinCount() > 0;
				view.spinAllButton.gameObject.SetActive(albumController.GetLuckySpinCount() > 0);
			}
			else
			{
				if (albumController.GetLuckySpinCount() > 0)
				{
					WaitForSeconds(1, () => { StartSpinning(true); });
				}
				else
				{
					isAutoSpin = false;
					view.autoStopButton.gameObject.SetActive(false);
					view.holdClickSpinButton.gameObject.SetActive(true);
					view.holdClickSpinButton.interactable = false;
					view.spinAllButton.gameObject.SetActive(false);
				}
			}

			if (albumController.GetLuckySpinCount() == 0)
			{
				OnCheckPrizeButtonClicked();
			}
			
			if(sGetCardLuckySpinInfo.RewardsForCollect.Count > 0) {}
			{
				view.checkPrizeButton.interactable = true;
			}
			
			isPlaying = false;

			if (seasonIsEnd)
			{
				view.Close();
			}
		}
		public void OnAutoStopButtonClicked()
		{
			view.autoStopButton.gameObject.SetActive(false);
			view.stopButton.gameObject.SetActive(true);
			view.stopButton.interactable = true;
			isAutoSpin = false;
		}
		public void OnSpinButtonClicked()
		{
			if (albumController.GetLuckySpinCount() > 0)
			{
				StartSpinning(false);
				BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "SpinButton"),("OperationId","1"));
			}
		}

		private void UpdateUiState()
		{
			view.holdClickSpinButton.gameObject.SetActive(true);
			view.holdClickSpinButton.interactable = albumController.GetLuckySpinCount() > 0;
			view.spinAllButton.gameObject.SetActive(albumController.GetLuckySpinCount() > 0);
			UpdateLuckySpinCount(albumController.GetLuckySpinCount());
		}

		public void StartSpinning(bool autoSpin = false)
		{
			isAutoSpin = autoSpin;
	
			quickStopIndex = -1;

			//ToggleButton(false);
			_wheel.StartSpinning(_easingConfig, OnSpinEndCallback, 0, () =>
			{
				SoundController.StopSfx("Album_LuckySpin_Start2");
				SoundController.PlaySfx("Album_LuckySpin_Start3");
			});
			EnableUpdate();

			var luckySpinCount = albumController.GetLuckySpinCount();
			UpdateLuckySpinCount(luckySpinCount - 1);

			view.bubbleTip.gameObject.SetActive(false);
			_spinResultReceived = false;

			float startTime = Time.realtimeSinceStartup;
			isPlaying = true;
			
			albumController.DoLuckySpin((sCardLuckySpin) =>
			{
				if (sCardLuckySpin != null)
				{
					sGetCardLuckySpinInfo.RewardsForCollect.Add(sCardLuckySpin.Reward);
					
					for (var i = 0; i < sGetCardLuckySpinInfo.SpinRewardConfs.Count; i++)
					{
						if (sGetCardLuckySpinInfo.SpinRewardConfs[i].RewardId == sCardLuckySpin.RewardId)
						{
							spinResultIndex = i;
							break;
						}
					}
					
					//spinResultIndex = (int) sCardLuckySpin.RewardId - (int)sGetCardLuckySpinInfo.SpinRewardConfs[0].RewardId;
				
					_spinResultReceived = true;

					XDebug.Log("spinResultIndex:" + spinResultIndex);

					EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());

					if (!autoSpin)
					{
						view.holdClickSpinButton.gameObject.SetActive(false);
						view.stopButton.gameObject.SetActive(true);
						view.stopButton.interactable = true;
					}

					_wheel.OnSpinResultReceived();
				}
				else
				{
					if (seasonIsEnd)
					{
						view.Close();
					}
				}
			});

			if (autoSpin)
			{
				view.autoStopButton.interactable = true;
				view.autoStopButton.gameObject.SetActive(true);

				view.holdClickSpinButton.gameObject.SetActive(false);
				view.stopButton.gameObject.SetActive(false);
			}
			else
			{
				view.autoStopButton.gameObject.SetActive(false);
				view.holdClickSpinButton.interactable = false;
				view.stopButton.gameObject.SetActive(false);
			}

			view.holdClickSpinButton.interactable = false;
			view.stopButton.gameObject.SetActive(false);

			view.animator.Play("Show");
		
			SoundController.PlaySfx("Album_LuckySpin_Start1");
			
			WaitForSeconds(0.3f, () =>
			{
				SoundController.PlaySfx("Album_LuckySpin_Start2", true);
			});
			
		}
		
		public async void OnSpinAllButtonClicked()
		{
			if (!view.holdClickSpinButton.interactable)
				return;

			if (albumController.GetLuckySpinCount() > 0)
			{
				BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "SpinAllButton"),("OperationId","2"));

				var popup = await PopupStack.ShowPopup<LuckySpinSpinAllPopup>();

				popup.SetConfirmAction(() =>
				{
					isPlaying = true;
					
					albumController.DoLuckySpinAll((sCardLuckySpinAll) =>
					{
						if (sCardLuckySpinAll != null)
						{
							for (var i = 0; i < sCardLuckySpinAll.Results.Count; i++)
							{
								sGetCardLuckySpinInfo.RewardsForCollect.Add(sCardLuckySpinAll.Results[i].Reward);
							}

							UpdateLuckySpinCount(albumController.GetLuckySpinCount());

							EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());

							OnSpinEndCallback();
						}
						else
						{
							if (seasonIsEnd)
							{
								view.Close();
							}
						}
					});
				});
			}
		}
		public async void OnCheckPrizeButtonClicked()
		{
			if (!view.holdClickSpinButton.interactable && albumController.GetLuckySpinCount() > 0 || isAutoSpin)
				return;

			BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinStart,("Operation:", "CheckPrizeButton"),("OperationId","5"));

			if (sGetCardLuckySpinInfo.RewardsForCollect.Count > 0)
			{
				var luckySpinRewardPopup = await PopupStack.ShowPopup<LuckySpinRewardPopup>();

				luckySpinRewardPopup.viewController.SetUpRewardUI(sGetCardLuckySpinInfo.RewardsForCollect,
					() =>
					{
						sGetCardLuckySpinInfo.RewardsForCollect.Clear();
						view.checkPrizeButton.interactable = false;
						UpdateUiState();
					}, albumController.GetLuckySpinCount() > 0);
			}
		}

		public async void OnCloseButtonClicked()
		{
			if (!view.holdClickSpinButton.interactable && albumController.GetLuckySpinCount() > 0 || isAutoSpin)
				return;
			
			if (sGetCardLuckySpinInfo.RewardsForCollect.Count > 0)
			{
				var luckySpinRewardPopup = await PopupStack.ShowPopup<LuckySpinRewardPopup>();

				luckySpinRewardPopup.viewController.SetUpRewardUI(sGetCardLuckySpinInfo.RewardsForCollect,
					() =>
					{
						sGetCardLuckySpinInfo.RewardsForCollect.Clear();
						view.checkPrizeButton.interactable = false; 
						UpdateUiState();
					},false);
			}
			else
			{
				view.Close();
			}
		}

		public override void OnViewEnabled()
		{
			base.OnViewEnabled();
			UpdateViewUIState();
		}

		public override void Update()
		{
			_wheel.Update();
		}
	}
}