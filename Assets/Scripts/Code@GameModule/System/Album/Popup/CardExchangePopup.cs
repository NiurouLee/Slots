// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/19/17:33
// Ver : 1.0.0
// Description : CardExchangePopup.cs
// ChangeLog :
// **********************************************

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule 
{
	public class ExchangeableCardView : View
	{
		[ComponentBinder("PlusButton")] public Button  addButton;
		[ComponentBinder("ReduceButton")] public Button minusButton;
		[ComponentBinder("Num")] public Text cardNum;
		[ComponentBinder("Card")] public Transform cardTransform;
		[ComponentBinder("X2Star")] public Transform x2Star;

		private CardExchangePopupViewController _exchangePopupViewController;
		private Card _cardInfo;
		private AlbumCardView _cardView;
		private int usedCardCount = 0;
		 
		public void SetUpExchangeableCardView(Card cardInfo, SpriteAtlas spriteAtlas, CardExchangePopupViewController exchangeController)
		{
			_cardInfo = cardInfo;
			_exchangePopupViewController = exchangeController;
			_cardView.viewController.SetUpCard(cardInfo,spriteAtlas,false,false,true);

			usedCardCount = 0;

			if (usedCardCount <= 0)
			{
				minusButton.interactable = false;
			}

			if (usedCardCount >= cardInfo.Count)
			{
				addButton.interactable = false;
			}

			x2Star.gameObject.SetActive(cardInfo.Type == Card.Types.CardType.Lucky);

			cardNum.text = usedCardCount.ToString();
		}

		protected override void OnViewSetUpped()
		{
			base.OnViewSetUpped();
			addButton.onClick.AddListener(OnAddButtonClicked);
			minusButton.onClick.AddListener(OnMinusButtonClicked);
			_cardView = AddChild<AlbumCardView>(cardTransform.Find("CardView"));
		}

		protected void OnAddButtonClicked()
		{
			if (usedCardCount < _cardInfo.Count - 1)
			{
				usedCardCount++;
				if (usedCardCount >= _cardInfo.Count -1)
				{
					addButton.interactable = false;
				}
				
				_cardView.viewController.UpdateUseCardNum(usedCardCount);

				minusButton.interactable = true;
				
				_exchangePopupViewController.OnCardSelectInfoChange(_cardInfo.CardId, usedCardCount);
				cardNum.text = usedCardCount.ToString();
			}
		}

		public void OnSelectFullStateChange(bool isFull)
		{
			if (isFull)
			{
				addButton.interactable = false;
			}
			else
			{
				if (usedCardCount < _cardInfo.Count - 1)
				{
					addButton.interactable = true;
				}
				else
				{
					addButton.interactable = false;
				}
			}
		}

		public void SelectForFullFill(long needPowerPoint)
		{
			if (usedCardCount < _cardInfo.Count - 1)
			{
				long accPower = 0;
				long powerPerCard = _cardInfo.RecycleValue;

				while (usedCardCount < _cardInfo.Count - 1)
				{
					usedCardCount++;
					accPower += powerPerCard;
					if (accPower >= needPowerPoint)
					{
						break;
					}
				}

				_cardView.viewController.UpdateUseCardNum(usedCardCount);

				_exchangePopupViewController.OnCardSelectInfoChange(_cardInfo.CardId, usedCardCount,false);
				cardNum.text = usedCardCount.ToString();
				minusButton.interactable = true;
				addButton.interactable = usedCardCount < _cardInfo.Count - 1;
			}
		}

		protected void OnMinusButtonClicked()
		{
			if (usedCardCount > 0)
			{
				usedCardCount--;
				if (usedCardCount <= 0)
				{
					minusButton.interactable = false;
				}
				_cardView.viewController.UpdateUseCardNum(usedCardCount);
				
				addButton.interactable = true;
				
				_exchangePopupViewController.OnCardSelectInfoChange(_cardInfo.CardId, usedCardCount);
				cardNum.text = usedCardCount.ToString();
			}
		}
	}
	public class WheelElementProvider : IElementProvider
	{
		protected Transform elementTransform;

		private RepeatedField<SGetCardRecycleGameInfo.Types.GameRewardItem> _rewardItems;

		private RepeatedField<uint> resultIds = null;
		public WheelElementProvider(Transform inElementTransform, RepeatedField<SGetCardRecycleGameInfo.Types.GameRewardItem> rewardItems)
		{
			_rewardItems = rewardItems;
			elementTransform = inElementTransform;
		}
	    
		public int GetReelMaxLength()
		{
			return _rewardItems.Count;
		}
 
		protected int GetRewardIndex(int index)
		{
			if (resultIds == null || stopIndex < 0)
			{
				return index;
			}
			else
			{
				if (resultIndexes.IndexOf(index) >= 0)
				{
					return (int)resultIds[resultIndexes.IndexOf(index)] - 1;
				}
				
				return index;
			}
		}
		
		public GameObject GetElement(int index)
		{
			var element = GameObject.Instantiate(elementTransform);

			var rewardIndex = GetRewardIndex(index);
			var cardColor = _rewardItems[rewardIndex].CardColor;
		    
			for (var i = 0; i < 8; i++)
			{
				element.Find("RollerBg" + (i + 1)).gameObject.SetActive(i == cardColor);
			}

			if (_rewardItems[rewardIndex].ItemOther != null && _rewardItems[rewardIndex].ItemOther.Count > 1)
			{
			    element.Find("NumText").gameObject.SetActive(false);
			    element.Find("GrandPrize").gameObject.SetActive(true);
			    element.Find("GrandPrize").GetComponent<Text>().text = _rewardItems[rewardIndex].ShowCoinCount.GetAbbreviationFormat(0);
			}
			else
			{
				element.Find("NumText").gameObject.SetActive(true);
				element.Find("GrandPrize").gameObject.SetActive(false);
				element.Find("NumText").GetComponent<Text>().text = _rewardItems[rewardIndex].ShowCoinCount.GetAbbreviationFormat(0);
			}

			element.gameObject.SetActive(true);
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

		public int stopIndex = -1;
		public List<int> resultIndexes = new List<int>();

		public int ComputeReelStopIndex(int currentIndex, int slowDownStepCount)
		{
			stopIndex = (currentIndex - 20 + _rewardItems.Count) % _rewardItems.Count;
			
			for (var i = 0; i < resultIds.Count; i++)
			{
				var resultIndex = (stopIndex + 3 + i) % _rewardItems.Count;
				resultIndexes.Add(resultIndex);
			}
			
			return stopIndex;
		}

		public void SetGameResult(RepeatedField<uint> gameRewardIds)
		{
			resultIds = gameRewardIds;
		}

	}
    [AssetAddress("UICardExchangePopup")]
    public class CardExchangePopup: Popup<CardExchangePopupViewController>
    {
        [ComponentBinder("Root/CardsNum/LeftButton")]
        public Button leftButton;

        [ComponentBinder("Root/CardsNum/RightButton")]
        public Button rightButton;

        [ComponentBinder("Root/CardsNum/ScrollView/Viewport/Content")]
        public RectTransform content;

        [ComponentBinder("Root/SpinButton")]
        public Button spinButton;

        [ComponentBinder("Root/SelectForMeButton")]
        public Button selectForMeButton;

        [ComponentBinder("Root/TopGroup/HelpButton")]
        public Button helpButton;

        [ComponentBinder("Root/RollerPanel")]
        public RectTransform rollerPanel;

        [ComponentBinder("Root/RollerPanel/RollerLayout")]
        public RectTransform rollerLayout;
        
        [ComponentBinder("Root/RollerPanel/RollerLayout/XYZ")]
        public RectTransform xyz;  
         
        [ComponentBinder("Root/RollerPanel/ProgressBarBg/Fill Area/ProgressBar")]
        public RectTransform stageNode;

        [ComponentBinder("Root/CardsNum/ScrollView")]
        public ScrollRect scrollView; 
        
        [ComponentBinder("Root/RollerPanel/ProgressBarBg")]
        public Slider progressBar;
        
        [ComponentBinder("Root/CardsNum")]
        public Transform cardsNum;
        
  
        public CardExchangePopup(string address)
            :base(address)
        {
	         
        }

        protected override void OnViewSetUpped()
        {
	        base.OnViewSetUpped();
	        AdaptScaleTransform(rollerPanel, new Vector2(1365,768));
	       
	        if (rollerPanel.localScale.x < 0.76f)
	        {
		        var anchoredPosition = rollerPanel.anchoredPosition;
		        anchoredPosition.y -= 110;
		        rollerPanel.anchoredPosition = anchoredPosition;
	        }
        }
    }


    public class CardExchangePopupViewController: ViewController<CardExchangePopup>
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected SingleColumnWheel wheel;
        protected SingleColumnWheel wheelBig;
        protected WheelElementProvider smallWheelElementProvider;
        protected WheelElementProvider bigWheelElementProvider;

        protected int currentFillIndex = 0;

        /// <summary>
        /// 当前玩家选择的Card,key为CardId, value为张数
        /// </summary>
        protected Dictionary<uint, int> currentSelectCardInfo;

        protected List<Card> exchangeableCardList;

        protected RepeatedField<uint> powersNeedForTarget;

        protected List<ExchangeableCardView> exchangeableCardViews;

        protected SGetCardRecycleGameInfo sGetCardRecycleGameInfo;

        protected int currentPowerStage = 0;

        protected Reward fortuneExchangeReward;

        protected bool seasonIsEnd = false;
        protected bool isPlaying = false;
        
        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();
           
            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
        }
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            albumController = Client.Get<AlbumController>();

            currentSelectCardInfo = new Dictionary<uint, int>();
            exchangeableCardList = albumController.GetExchangeableCardInfo();
            
            powersNeedForTarget = new RepeatedField<uint>();
            
            for (var i = 0; i < sGetCardRecycleGameInfo.RecycleLevelConfigs.Count; i++)
            {
	            var config = sGetCardRecycleGameInfo.RecycleLevelConfigs[i];
	            powersNeedForTarget.Add(config.CoverEnergyNeed);
            }

            view.spinButton.interactable = false;
            
            bigWheelElementProvider = new WheelElementProvider(view.xyz.Find("ElementBig"), sGetCardRecycleGameInfo.GameRewardItems);
 
            smallWheelElementProvider = new WheelElementProvider(view.xyz.Find("Element"),sGetCardRecycleGameInfo.GameRewardItems);
            
            wheel = new SingleColumnWheel(view.xyz, 2158, 13, smallWheelElementProvider, 0,true);
            wheelBig = new SingleColumnWheel(view.xyz.Find("RollBig"), 2158, 13, bigWheelElementProvider, 0,true);
        }

        public override void OnViewEnabled()
        {
	        base.OnViewEnabled();
	        view.progressBar.value = 0;
	        
	        UpdateStageNode(0);

	        StartCoroutine(SetUpExchangeableCardView());

	        var easingConfig = new SimpleRollUpdaterEasingConfig();
	        easingConfig.spinSpeed = 0.3f;
	        wheel.StartSpinning(easingConfig, OnSpinFinish, 0);
	        wheel.ForceStateToLoop();
	        EnableUpdate();
        }

		/// <summary>
		/// 当前选择的卡牌数量发生了变化
		/// </summary>
		/// <param name="cardId"></param>
		/// <param name="usedCardCount"></param>
		/// <param name="showAnimation"></param>
		public void OnCardSelectInfoChange(uint cardId, int usedCardCount, bool showAnimation = true)
		{
			currentSelectCardInfo[cardId] = usedCardCount;

			var totalPower = GetCurrentTotalPower();
			var fillAmount = CalcFillAmount(totalPower);


			if (view.progressBar.value < fillAmount)
			{
				view.progressBar.transform.Find("Uilizi").gameObject.SetActive(true);
			}

			view.progressBar.DOValue(fillAmount,0.5f).OnComplete(() =>
			{
				view.progressBar.transform.Find("Uilizi").gameObject.SetActive(false);
			});

		
			bool stateChange = IsFull != fillAmount >= 1;
			IsFull = fillAmount >= 1;
			
			if (stateChange)
			{
				for (var i = 0; i < exchangeableCardViews.Count; i++)
				{
					exchangeableCardViews[i].OnSelectFullStateChange(IsFull);
				}
			}
			
			UpdateStageNode(totalPower);
		}

		public void UpdateStageNode(long totalPower)
		{
			currentPowerStage = -1;
			
			for (var i = 0; i < powersNeedForTarget.Count; i++)
			{
				var activeNode = view.stageNode.Find($"NodeBg{i + 1}/Node");
				if (totalPower >= powersNeedForTarget[i])
				{
					if (!activeNode.gameObject.activeInHierarchy)
					{
						activeNode.gameObject.SetActive(true);
						SoundController.PlaySfx("Album_FortuneExchange_OndeOn");
					}

					currentPowerStage = i + 1;
				}
				else
				{
					if (activeNode.gameObject.activeInHierarchy)
						activeNode.gameObject.SetActive(false);
				}
			}

			view.spinButton.interactable = currentPowerStage > 0;
		}
		
		public float CalcFillAmount(long power)
		{
			float start = 0.044f;
			float interval = 0.09f;
			float stage = 0.035f;

			int index = 0;
			while (index < powersNeedForTarget.Count && power >= powersNeedForTarget[index])
				index++;

			if (index == 0)
			{
				var progress = (float) power / powersNeedForTarget[0];
				return progress * start;
			}

			if (index >= powersNeedForTarget.Count)
			{
				return 1;
			}

			var powerInterval = powersNeedForTarget[index] - powersNeedForTarget[index - 1];
			var powerLeft = power - powersNeedForTarget[index - 1];

			return start + interval * (index - 1 + (float) powerLeft / powerInterval) + stage * (index);
		}

		//当前填充的卡牌的总能量数量
		public long GetCurrentTotalPower()
		{
			long power = 0;
			foreach (var cardItem in currentSelectCardInfo)
			{
				var cardInfo = albumController.GetCardInfoByCardId(cardItem.Key);

				power += cardInfo.RecycleValue * cardItem.Value;
			}
			
			return power;
		}
		
		public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
		{
			base.BindingView(inView, inExtraData, inExtraAsyncData);
			sGetCardRecycleGameInfo = (SGetCardRecycleGameInfo) inExtraAsyncData;
		}

		protected override void SubscribeEvents()
        {
            view.leftButton.onClick.AddListener(OnLeftButtonClicked);
            view.rightButton.onClick.AddListener(OnRightButtonClicked);
            view.spinButton.onClick.AddListener(OnSpinButtonClicked);
            view.selectForMeButton.onClick.AddListener(OnSelectForMeButtonClicked);
            view.helpButton.onClick.AddListener(OnHelpButtonClicked);
            view.scrollView.onValueChanged.AddListener(OnScrollValueChanged);

            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

		protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd eventAlbumSeasonEnd)
		{
			seasonIsEnd = true;
			
			if (!isPlaying)
			{
				view.Close();
			}
		} 

		protected void OnScrollValueChanged(Vector2 pos)
		{
			var viewSize = view.scrollView.viewport.rect.width;
			var contentSize = view.scrollView.content.sizeDelta;

			if (viewSize >= contentSize.x)
				return;
			
			if (pos.x <= 0)
			{
				if (view.leftButton.interactable)
					view.leftButton.interactable = false;
				if (!view.rightButton.interactable)
					view.rightButton.interactable = true;
			}
			else if (pos.x > 0.999)
			{
				if (view.rightButton.interactable)
					view.rightButton.interactable = false;
				if (!view.leftButton.interactable)
					view.leftButton.interactable = true;
			}
			else
			{
				if (!view.leftButton.interactable)
					view.leftButton.interactable = true;
				if (!view.rightButton.interactable)
					view.rightButton.interactable = true;
			}
		}
    
        public IEnumerator SetUpExchangeableCardView()
        {
	        var template = view.content.Find("CardsPanel");
	        var spriteAtlas = cardSetAtlasRef.GetAsset<SpriteAtlas>();
	        exchangeableCardViews = new List<ExchangeableCardView>();
   
	       // var rectTransform = (RectTransform) view.scrollView.transform;

	      //  var delta = ViewResolution.referenceResolutionLandscape.x - (rectTransform.sizeDelta.x + 223);

	        if (ViewResolution.referenceResolutionLandscape.x < 1365)
	        {
		        var scale = ViewResolution.referenceResolutionLandscape.x / 1365;
		        view.cardsNum.localScale = new Vector3(scale, scale, scale);
		        view.selectForMeButton.transform.localScale = view.cardsNum.localScale;
		        view.spinButton.transform.localScale = view.cardsNum.localScale;
	        }
	       
	        // if (delta < 0)
	        // {
		       //  var shrinkAmount =  Mathf.Ceil(-delta / 223)*223;
		       //  var sizeDelta = rectTransform.sizeDelta;
		       //  sizeDelta.x -= shrinkAmount;
		       //  rectTransform.sizeDelta = sizeDelta;
	        //
		       //  var leftButtonTransform = (RectTransform) view.leftButton.transform;
		       //  var rightButtonTransform = (RectTransform) view.rightButton.transform;
		       //  var anchorPos = leftButtonTransform.anchoredPosition;
		       //  leftButtonTransform.anchoredPosition = new Vector2(anchorPos.x + shrinkAmount * 0.5f, anchorPos.y);
		       //  anchorPos = rightButtonTransform.anchoredPosition;
		       //  rightButtonTransform.anchoredPosition = new Vector2(anchorPos.x - shrinkAmount * 0.5f, anchorPos.y);
	        // }
	        
	        var cardsCanRecycle = sGetCardRecycleGameInfo.CardsCanRecycle;

	        if (cardsCanRecycle.Count > 0)
	        {
		        for (var i = 0; i < cardsCanRecycle.Count; i++)
		        {
			        var cardInfo = cardsCanRecycle[i];
			        var cardPanelTransform = i > 0 ? GameObject.Instantiate(template, view.content) : template;
			        var exchangeableCardView = view.AddChild<ExchangeableCardView>(cardPanelTransform);
			        exchangeableCardView.SetUpExchangeableCardView(cardInfo, spriteAtlas, this);

			        exchangeableCardViews.Add(exchangeableCardView);

			        if (i > 5)
			        {
				        yield return null;
			        }
		        }
		        
		        view.leftButton.interactable = false;
		        
		        if (cardsCanRecycle.Count <= 5)
		        {
			        view.rightButton.interactable = false;
		        }
	        }
	        else
	        {
		        view.cardsNum.Find("ScrollView/TIPS").gameObject.SetActive(true);
		        view.leftButton.interactable = false;
		        view.rightButton.interactable = false;
		        view.selectForMeButton.interactable = false;
		        template.gameObject.SetActive(false);
	        }
        }

        public void OnLeftButtonClicked()
        {
	        var viewSize = view.scrollView.viewport.rect.width;
	        var contentSize = view.scrollView.content.sizeDelta;
	        var normalizePosition = view.scrollView.normalizedPosition.x;

	        var width = contentSize.x - viewSize;
	        var moveDelta = viewSize / width;
	        
	        SoundController.PlayButtonClick();
	        
	        view.scrollView.horizontalNormalizedPosition = Mathf.Max(normalizePosition - moveDelta,0);

	        if (view.scrollView.horizontalNormalizedPosition <= 0)
	        {
		        view.leftButton.interactable = false;
		      
		        if (viewSize < contentSize.x)
		        {
			        view.rightButton.interactable = true;
		        }
	        }

        }
        public void OnRightButtonClicked()
        {
	        var viewSize = view.scrollView.viewport.rect.width;
	        var contentSize = view.scrollView.content.sizeDelta;
	        var normalizePosition = view.scrollView.normalizedPosition.x;

	        var width = contentSize.x - viewSize;
	        var moveDelta = viewSize / width;

	        SoundController.PlayButtonClick();
	        
	        if (normalizePosition + moveDelta >= 1.0f)
	        {
		        view.scrollView.horizontalNormalizedPosition = 1.0f;
	        }
	        else
	        {
		        view.scrollView.horizontalNormalizedPosition = normalizePosition + moveDelta;
	        }

	        if (Mathf.Abs(view.scrollView.horizontalNormalizedPosition - 1.0f) < 1e-5) 
	        {
		        view.rightButton.interactable = false;
		         
		        if (viewSize < contentSize.x)
		        {
			        view.leftButton.interactable = true;
		        }
	        }
        }
        public async void OnSpinButtonClicked()
        {
	        isPlaying = true;
	        view.spinButton.interactable = false;
	        
	        
	        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionFortuneExchangeStart,("Operation:", "FortuneExchangeSpinButton"),("OperationId","2"));

	        view.helpButton.gameObject.SetActive(false);
	        view.closeButton.gameObject.SetActive(false);
	         
	        view.animator.Play("Show");
 
	        var index = wheel.GetCurrentIndex();
	        wheel.ForceStateToIdle();
	      
	        wheelBig.ForeUpdateElementContainer(index);
	        
	        await WaitForSeconds(0.8f);
	         
	        var easingConfig = new SimpleRollUpdaterEasingConfig();

	        easingConfig.spinSpeed = 18;
	        easingConfig.slowDownDuration = 5;
	        easingConfig.speedUpDuration = 2;
	        easingConfig.leastSpinDuration = 6;
	        easingConfig.slowDownStepCount = 13;
	        easingConfig.overShootAmount = 1.01f;

	        wheelBig.StartSpinning(easingConfig, OnSpinFinish, 0,startSlowDown:
		        () => { SoundController.PlaySfx("Album_FortuneExchange_Start3"); });

	        SoundController.PlaySfx("Album_FortuneExchange_Start1");

	        WaitForSeconds(2, () => { SoundController.PlaySfx("Album_FortuneExchange_Start2"); });
			
			var keys = currentSelectCardInfo.Keys.ToList();
			
			for (var i = 0; i < keys.Count; i++)
			{
				if (currentSelectCardInfo[keys[i]] <= 0)
				{
					currentSelectCardInfo.Remove(keys[i]);
				}
			}

			albumController.DoCardRecycle(currentSelectCardInfo, (sCardRecycleGameSpin) =>
			{
				if (sCardRecycleGameSpin != null)
				{
					bigWheelElementProvider.SetGameResult(sCardRecycleGameSpin.GameRewardItemIds);

					fortuneExchangeReward = sCardRecycleGameSpin.Reward;

					view.helpButton.gameObject.SetActive(false);
					view.closeButton.gameObject.SetActive(false);

					wheelBig.OnSpinResultReceived();
				}
				else
				{
					if (seasonIsEnd)
					{
						view.Close();
					}
				}
			});
        }

        public void ShowWheelElementWinFx()
        {
	        for (var i = 0; i < 13; i++)
	        {
		        var element = wheelBig.GetElement(i);
		        element.GetComponent<Animator>().Play(i >= 2 && i < currentPowerStage + 2 ? "Show" : "Mask");
	        }
        }

        public void OnSpinFinish()
        {
	        view.helpButton.gameObject.SetActive(true);
	        view.closeButton.gameObject.SetActive(true);

	        ShowWheelElementWinFx();
	        
	        SoundController.PlaySfx("Album_FortuneExchange_Win");

	        WaitForSeconds(2, async () =>
	        {
		       var popup = await PopupStack.ShowPopup<CardExchangeRewardPopup>();
		       
		       popup.viewController.SetUpRewardUI(fortuneExchangeReward, () =>
		      {
			      view.Close();
		      });

	        });	
        }

        public bool IsFull { get; set; }
      
        public void OnSelectForMeButtonClicked()
        {
	        
	        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionFortuneExchangeStart,("Operation:", "FortuneExchangeSelectButton"),("OperationId","1"));

	        float fillAmount = 0;
	        var currentTotalPower = GetCurrentTotalPower();
	        var targetPower = powersNeedForTarget[powersNeedForTarget.Count - 1];
	        
	        for (var i = 0; i < exchangeableCardViews.Count; i++)
	        {
		        exchangeableCardViews[i].SelectForFullFill(targetPower - currentTotalPower);
		        
		        currentTotalPower = GetCurrentTotalPower();
		        
		        fillAmount = CalcFillAmount(currentTotalPower);
		       
		        if (fillAmount >= 1)
		        {
			        break;
		        }
	        }

	        UpdateStageNode(currentTotalPower);

	        IsFull = fillAmount >= 1;
	        view.progressBar.transform.Find("Uilizi").gameObject.SetActive(true);
	        
	        view.progressBar.DOValue(fillAmount,0.5f).OnComplete(() =>
	        {
		        view.progressBar.transform.Find("Uilizi").gameObject.SetActive(false);
	        });

	        for (var i = 0; i < exchangeableCardViews.Count; i++)
	        {
		        exchangeableCardViews[i].OnSelectFullStateChange(IsFull);
	        }
        }
        public async void OnHelpButtonClicked()
        {
	        var popup = await PopupStack.ShowPopup<TravelAlbumHelpPopup>();
	        popup.viewController.ShowPage(5);
	        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionFortuneExchangeRule);
        }

        public override void Update()
        {
	        wheel.Update();
	        wheelBig.Update();
        }
    }
}