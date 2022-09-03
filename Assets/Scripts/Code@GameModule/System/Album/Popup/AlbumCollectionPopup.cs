// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/08/14:12
// Ver : 1.0.0
// Description : AlbumCollectionPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    public class CardSetView:View
    {
        [ComponentBinder("Icon")] public Image albumIcon;

        [ComponentBinder("NumText")] public Text numText;

        [ComponentBinder("Progress")] public Image progress;

        [ComponentBinder("NewCardHints")] public Transform newCardHint;
        
        //---------------------------------------------------------------------
        public Button entranceButton;

        protected int cardSetIndex = 0;
        protected CardSet cardSet;

        protected AlbumCollectionPopupViewController collectionPopupViewController;

        protected static bool isOpenAlbumSetPopup = false;

        public void SetUpEntranceView(int inCardSetIndex, CardSet inCardSet, SpriteAtlas spriteAtlas, AlbumCollectionPopupViewController inCollectionPopupViewController)
        {
            cardSet = inCardSet;

            cardSetIndex = inCardSetIndex;
            collectionPopupViewController = inCollectionPopupViewController;
            
            var setId = inCardSet.SetId;
            albumIcon.sprite = spriteAtlas.GetSprite($"{setId}_Cover");
            numText.text = $"{cardSet.CardsCountOwned}/{cardSet.CardsCountTotal}";

            progress.fillAmount = (float) cardSet.CardsCountOwned / cardSet.CardsCountTotal;

            newCardHint.gameObject.SetActive(Client.Get<AlbumController>().HasNewCardInCardSet(cardSetIndex));
        }
 
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            entranceButton = transform.GetComponent<Button>();
            entranceButton.onClick.AddListener(OnEntranceClicked);
        }

        public void UpdateNewTagState()
        {
            newCardHint.gameObject.SetActive(Client.Get<AlbumController>().HasNewCardInCardSet(cardSetIndex));
            progress.fillAmount = (float) cardSet.CardsCountOwned / cardSet.CardsCountTotal;
            numText.text = $"{cardSet.CardsCountOwned}/{cardSet.CardsCountTotal}";
        }
        
        protected async void OnEntranceClicked()
        {
            if (!isOpenAlbumSetPopup && !collectionPopupViewController.IsDrag)
            {
                isOpenAlbumSetPopup = true;
                var alumSetPopup = await PopupStack.ShowPopup<AlbumSetPopup>();

                alumSetPopup.viewController.SetUpCardViews(cardSetIndex, true);
                isOpenAlbumSetPopup = false;
                
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionAlbumEnter, ("cardSetIndex",cardSetIndex.ToString()));
            }
        }
    }
    
    [AssetAddress("UIAlbumCollectionPopup")]
    public class AlbumCollectionPopup : Popup<AlbumCollectionPopupViewController>
    {
        
        [ComponentBinder("Root")] public Transform root;
        [ComponentBinder("Root/AlbumContainer")] public Transform albumRoot;
        [ComponentBinder("Root/AlbumContainer/Content")] public Transform albumContainer;
        [ComponentBinder("Root/TopBg")] public Transform topBg;
        [ComponentBinder("Root/TopGroup")] public Transform topGroup;
        [ComponentBinder("Root/BottomBg")] public Transform bottomGroup;
        
        [ComponentBinder("Root/AlbumContainer/HotArea")] public Transform hotArea;
        
        [ComponentBinder("Root/AlbumContainer/Content/Container")] public Transform albumContent;
        
        [ComponentBinder("Root/TopGroup/MenuButton")] public Button menuButton;
        
        [ComponentBinder("Root/Navigation/NavigationBg/RulesButton")] public Button rulesButton;
        
        [ComponentBinder("Root/Navigation/NavigationBg/HistoryButton")] public Button historyButton;
        
        [ComponentBinder("Root/Navigation/NavigationBg/BackButton")] public Button backButton;
        [ComponentBinder("Root/Navigation")] public Transform navRoot;
        
        [ComponentBinder("FortuneExchangeButton")]
        public Button fortuneExchangeButton;

        [ComponentBinder("LuckySpinButton")] 
        public Button luckySpinButton;

        [ComponentBinder("LuckyChallengeButton")]
        public Button luckyChallengeButton;  
        
        [ComponentBinder("Root/TopBg/Bonus/CoinText")]
        public Text coinText;  
        
        [ComponentBinder("Root/TopBg/NumText")]
        public Text numText;
        
        [ComponentBinder("Root/TopBg/TipsText")]
        public Transform tipsText;

        [ComponentBinder("Root/BottomBg/AmazingTheHatButton")]
        private Transform amazingHat;

        public AmazingHatEntranceView amazingHatEntranceView;

        public float albumCount = 12;
        public float albumAngleInDegree = 6;
        public float albumContentMoveCycleRadius = 4250;
        public Vector3 albumDefaultScale = new Vector3(0.85f,0.85f,0.85f);

        public List<CardSetView> cardSetViews;
        
        public AlbumCollectionPopup(string address)
            : base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            
            AdaptScaleTransform(albumRoot, new Vector2(1200,768));
            AdaptScaleTransform(topBg, new Vector2(1200,768));
            AdaptScaleTransform(topGroup, new Vector2(1200,768));
            AdaptScaleTransform(bottomGroup, new Vector2(1200,768));
        }

        public void SetUpAlbumView(int inAlbumCount)
        {
            albumCount = inAlbumCount;

            var book = albumContent.Find("Album");

            cardSetViews = new List<CardSetView>();

            for (var i = 0; i < albumCount; i++)
            {
                var newBook = GameObject.Instantiate(book.gameObject, albumContent);
                var degrees = albumAngleInDegree * i;

                float sin = albumContentMoveCycleRadius * Mathf.Sin(degrees * Mathf.Deg2Rad);
                float cos = albumContentMoveCycleRadius * Mathf.Cos(degrees * Mathf.Deg2Rad);

                var anchoredPosition = new Vector2(sin, cos);

                ((RectTransform) newBook.transform).anchoredPosition = anchoredPosition;
                ((RectTransform) newBook.transform).localRotation= Quaternion.Euler(0, 0, -degrees);
                
                cardSetViews.Add(AddChild<CardSetView>(newBook.transform));

                if (i != 0)
                {
                    cardSetViews[i].transform.localScale = albumDefaultScale;
                }
            }
            
            book.gameObject.SetActive(false);
            
            //关联AmazingHat入口
            amazingHatEntranceView = AddChild<AmazingHatEntranceView>(amazingHat);
        }

        public override void Close()
        {
            SoundController.RecoverLastMusic();
            base.Close();
        }
    }
    
    public class AlbumCollectionPopupViewController : ViewController<AlbumCollectionPopup>
    {
        protected bool isDrag;
        protected Vector2 startPos;
        protected Vector2 lastDragPos;
        protected float moveVelocity;

        protected float maxAngle;
        protected float minAngle;
        protected float halfViewAngle;

        public int cardSetCount = 12;

        protected AlbumController albumController;

        protected AssetReference cardSetAtlasRef;
        
        public bool IsDrag => isDrag;
      
        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();
           
            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
            
            SetUpCardSetEntranceView();
            SetUpAllSetCompleteReward();

            SetBonusGameEntranceStatusUI();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            string source = GetTriggerSource();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionEnter,("source", source));
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.luckyChallengeButton.onClick.AddListener(OnLuckyChallengeClicked);
            view.luckySpinButton.onClick.AddListener(OnLuckySpinClicked);
            view.fortuneExchangeButton.onClick.AddListener(OnFortuneExchangeButtonClicked);
            
            view.menuButton.onClick.AddListener(OnMenuButtonClicked);
            view.backButton.onClick.AddListener(OnBackButtonClicked);
            view.rulesButton.onClick.AddListener(OnHelpButtonClicked);
            view.historyButton.onClick.AddListener(OnHistoryButtonClicked);
            
            SubscribeEvent<EventUpdateAlbumRedDotReminder>(OnUpdateRedDotReminder);
            SubscribeEvent<EventOnShowAlbumGuide4Finished>(OnGuideStep4Finished);
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        protected void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }

        public void OnUpdateRedDotReminder(EventUpdateAlbumRedDotReminder evt)
        {
            SetBonusGameEntranceStatusUI();

            for (var i = 0; i < view.cardSetViews.Count; i++)
            {
                view.cardSetViews[i].UpdateNewTagState();
            }

            //UpdateLuckyChallengeEntranceState();

            SetUpAllSetCompleteReward();
        }

        protected void UpdateLuckyChallengeEntranceState()
        {
            var luckyChallengeProgressText = view.luckyChallengeButton.transform.Find("NumText").GetComponent<Text>();

            if (albumController.GetLuckyChallengeProgress() == albumController.GetLuckyChallengeMaxProgress())
            {
                luckyChallengeProgressText.text = "COMPLETED";
            }
            else
            {
                luckyChallengeProgressText.text =
                    $"{albumController.GetLuckyChallengeProgress()}/{albumController.GetLuckyChallengeMaxProgress()}";
            }


            var progressBar = view.luckyChallengeButton.transform.Find("ProgressBg/Progress").GetComponent<Image>();
            progressBar.fillAmount = (float) albumController.GetLuckyChallengeProgress() /
                                     albumController.GetLuckyChallengeMaxProgress();
        }
        public void OnGuideStep4Finished(EventOnShowAlbumGuide4Finished evt)
        {
            ShowGuideStep5();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            view.amazingHatEntranceView?.viewController.OnViewEnabled();
            //检测是否有收集完成但是未领取奖励的卡册
            albumController.CheckAndClaimFinishCardSetReward();

            CheckNeedShowGuide();
            
            EnableUpdate();
            
            SoundController.PlayBgMusic("TravelAlbumBgMusic");
        }

        public void CheckNeedShowGuide()
        {
            //WaitForSeconds(1.5f, ShowGuideStep1);

            if (albumController.GetGuideStep() > 2)
            {
                return;
            }
            
            if (albumController.GetGuideStep() == 0)
            {
                minAngle = 0;
                ShowGuideStep1();
            }
            else if (albumController.GetGuideStep() == 1)
            {
                minAngle = 0;
                ShowGuideStep2();
            }
        }

        public async void ShowGuideStep1()
        {
            var guideStep1View = await View.CreateView<AlbumGuideStepView>("UITravelAlbumGuide_Step1", view.transform);
            guideStep1View.SetGuideClickHandler(ShowGuideStep1RewardPackage);
        }

        public  void ShowGuideStep1RewardPackage()
        {
            ViewManager.Instance.BlockingUserClick(true, "AlbumGuideStep1");
            albumController.IncreaseGuideStep((sIncBeginnersGuideStep) =>
            {
                ViewManager.Instance.BlockingUserClick(false, "AlbumGuideStep1");
                if (sIncBeginnersGuideStep != null && sIncBeginnersGuideStep.Reward != null)
                {
                    ItemSettleHelper.SettleItems(sIncBeginnersGuideStep.Reward.Items, ShowGuideStep2);
                }
            });
        }
        
        public async void ShowGuideStep2()
        {
            var guideStep2View = await View.CreateView<AlbumGuideStepView>("UITravelAlbumGuide_Step2", view.transform);

            var canvas = view.cardSetViews[0].transform.gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;
            canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            var handle = guideStep2View.transform.Find("Root/Click");
            var handleCanvas = handle.gameObject.AddComponent<Canvas>();
            handleCanvas.overrideSorting = true;
            handleCanvas.sortingOrder = 11;
            handleCanvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");

            guideStep2View.SetGuideClickHandler(()=>
            {
                GameObject.Destroy(canvas);
                albumController.IncreaseGuideStep(null);
                EnterGuideArtSet1();
            });
        }
        
        
        public async void ShowGuideStep5()
        {
            var guideStep5View = await View.CreateView<AlbumGuideStepView>("UITravelAlbumGuide_Step5", view.root);
            guideStep5View.SetGuideClickHandler(OnGuideStep5Finished);
            guideStep5View.transform.SetAsLastSibling();
            view.topBg.SetAsLastSibling();
        }

        public void OnGuideStep5Finished()
        {
            view.topBg.SetAsFirstSibling();
            albumController.IncreaseGuideStep(null);
        }
        
        public async void EnterGuideArtSet1()
        {
            var alumSetPopup = await PopupStack.ShowPopup<AlbumSetPopup>();

            alumSetPopup.viewController.SetUpCardViews(0, true);
        }

        public void OnLuckyChallengeClicked()
        {
            PopupStack.ShowPopupNoWait<LuckyChallengePopup>();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckyChallengeEnter);
        }

        public void OnLuckySpinClicked()
        {
            PopupStack.ShowPopupNoWait<LuckySpinPopup>();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionLuckySpinEnter);
        }

        public void OnFortuneExchangeButtonClicked()
        {
            PopupStack.ShowPopupNoWait<CardExchangePopup>();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionFortuneExchangeEnter);
        }

        public void OnMenuButtonClicked()
        {
            if (isHidding)
                return;
            view.navRoot.gameObject.SetActive(true);
            var animator = view.navRoot.GetComponent<Animator>();
            animator.Play("Navigation_Open");
        }

        private bool isHidding = false;
        public async void OnBackButtonClicked()
        {
            isHidding = true;
            view.navRoot.gameObject.SetActive(true);
            var animator =  view.navRoot.GetComponent<Animator>();
            await XUtility.PlayAnimationAsync(animator,"Navigation_Close");
            view.navRoot.gameObject.SetActive(false);
            isHidding = false;
        }
        
        public void OnHistoryButtonClicked()
        {
            if (isHidding)
            {
                return;
            }
            PopupStack.ShowPopupNoWait<CardHistoryPopup>();
        }
        
        public void OnHelpButtonClicked()
        {
            if (isHidding)
            {
                return;
            }
            PopupStack.ShowPopupNoWait<TravelAlbumHelpPopup>();
        }

        public void SetUpAllSetCompleteReward()
        {
            var reward = albumController.GetAllSetCompleteReward();
            var coinItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Coin);
         
            if (coinItem != null)
            {
                view.coinText.text = coinItem.Coin.Amount.GetCommaFormat();
            }
            var ownedCardCount = albumController.GetOwnedCardCount();
            var totalCardCount = albumController.GetTotalCardCount();
            
            view.tipsText.gameObject.SetActive(ownedCardCount == totalCardCount);
            view.coinText.transform.parent.gameObject.SetActive(ownedCardCount != totalCardCount);

            view.numText.text = $"{ownedCardCount}/{totalCardCount}";
        }

        public void SetBonusGameEntranceStatusUI()
        {
            
            //UpdateLuckyChallengeEntranceState();
             
            var luckySpinCount = albumController.GetLuckySpinCount();
            
            view.luckySpinButton.transform.Find("Reminder/BG/CountText").GetComponent<Text>().text =
                luckySpinCount.ToString();

            // if (luckySpinCount > 0)
            // {
            //     view.luckySpinButton.transform.Find("Reminder").gameObject.SetActive(true);
            //     view.luckySpinButton.transform.Find("Reminder/BG/CountText").GetComponent<Text>().text =
            //         luckySpinCount.ToString();
            // }
            // else
            // {
            //     view.luckySpinButton.transform.Find("Reminder").gameObject.SetActive(false);
            // }
            
            UpdateFortuneExchangeCountDown(true);
        }
        
        public void SetUpCardSetEntranceView()
        {
            var albumViewCount = view.cardSetViews.Count;
           
            for (var i = 0; i < albumViewCount; i++)
            {
                view.cardSetViews[i].SetUpEntranceView(i,albumController.GetCardSetInfoByIndex(i), cardSetAtlasRef.GetAsset<SpriteAtlas>(),this);
                
                var handler = view.cardSetViews[i].entranceButton.gameObject.AddComponent<DragDropEventCustomHandler>();
                handler.BindingBeginDragAction(OnDragAlbumBegin);
                handler.BindingDragAction(OnDragAlbum);
                handler.BindingEndDragAction(OnDragAlbumEnd);
            }
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            albumController = Client.Get<AlbumController>();

            cardSetCount = albumController.GetCardSetCount();
            
            view.SetUpAlbumView(cardSetCount);

            var dragDropEventCustomHandler = view.hotArea.gameObject.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnDragAlbumBegin);
            dragDropEventCustomHandler.BindingDragAction(OnDragAlbum);
            dragDropEventCustomHandler.BindingEndDragAction(OnDragAlbumEnd);
 
            SetUpRotationRange();
        }

        public void SetUpRotationRange()
        {
            var contentViewSize = ViewResolution.referenceResolutionLandscape;
            contentViewSize.x -= 400;
            
            halfViewAngle = Mathf.Rad2Deg * Mathf.Asin(contentViewSize.x / (2 * view.albumContentMoveCycleRadius));
            var bookAngleRange = view.albumAngleInDegree* (cardSetCount - 1);
         
            maxAngle = bookAngleRange - halfViewAngle;
            minAngle = halfViewAngle;
        }
        
        public void OnDragAlbumBegin(PointerEventData pointerEventData)
        {
            isDrag = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) view.albumContainer,
                pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
            lastDragPos = startPos;
        }
        
        public void OnDragAlbum(PointerEventData pointerEventData)
        {
            if (isDrag)
            {
                Vector2 pointerCurrentPos;
               
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) view.albumContainer,
                    pointerEventData.position, pointerEventData.pressEventCamera, out pointerCurrentPos);

                var origin = new Vector2(view.albumContent.localPosition.x, view.albumContent.localPosition.y);
                var angle = Vector3.SignedAngle(lastDragPos - origin, pointerCurrentPos - origin,Vector3.forward);
                
                XDebug.Log($"[angle]:{angle}");

                Vector3 eulerAngles = view.albumContent.localEulerAngles;;
                eulerAngles.z += angle;

                if (eulerAngles.z > 180)
                {
                    eulerAngles.z -= 360;
                }

                if (eulerAngles.z > maxAngle + halfViewAngle)
                {
                    eulerAngles.z = maxAngle + halfViewAngle;
                }
                
                if (eulerAngles.z < minAngle - halfViewAngle)
                {
                    eulerAngles.z = minAngle - halfViewAngle;
                }
                
                view.albumContent.localEulerAngles = eulerAngles;

                UpdateCardSetViewScale(eulerAngles.z);
               
                lastDragPos = pointerCurrentPos;
                
                var  deltaTime = Time.unscaledDeltaTime;
                var newRotateVelocity = angle / deltaTime;

                newRotateVelocity = Mathf.Clamp(newRotateVelocity, -40, 40);
                
                moveVelocity = Mathf.Lerp(moveVelocity, newRotateVelocity, deltaTime * 10);
                
                XDebug.Log($"moveVelocity:" + moveVelocity);
                
            }
        }

        private float lastCountDown = 0;
        
        public void UpdateFortuneExchangeCountDown(bool force = false)
        {
            var countDown =  albumController.GetFortuneExchangeCountDown();

            if ((lastCountDown > 0 && countDown <= 0)
                || (lastCountDown <= 0 && countDown > 0) || force)
            {
                if (countDown <= 0)
                {
                    view.fortuneExchangeButton.interactable = true;
                    view.fortuneExchangeButton.transform.Find("ProgressBg").gameObject.SetActive(false);
                    view.fortuneExchangeButton.transform.Find("Play").gameObject.SetActive(true);
                    view.fortuneExchangeButton.transform.Find("Lock").gameObject.SetActive(false);

                    var reminder = view.fortuneExchangeButton.transform.Find("Play/Reminder");
                    if (reminder)
                    {
                        reminder.gameObject.SetActive(albumController.FortuneExchangeCanPlay());
                    }
                }
                else
                {
                    view.fortuneExchangeButton.interactable = false;
                    view.fortuneExchangeButton.transform.Find("Play").gameObject.SetActive(false);
                    view.fortuneExchangeButton.transform.Find("ProgressBg").gameObject.SetActive(true);
                    view.fortuneExchangeButton.transform.Find("Lock").gameObject.SetActive(true);
                }
            }
            if(countDown >= 0)
                view.fortuneExchangeButton.transform.Find("ProgressBg/TimeText").GetComponent<Text>().text = XUtility.GetTimeText(countDown);
            lastCountDown = countDown;
        }
        public override void Update()
        {
            Vector3 eulerAngles = view.albumContent.localEulerAngles;
           
            float rotationOffset = 0;

            var angleZ = eulerAngles.z;

            if (angleZ > 180)
            {
                angleZ -= 360f;
            }
            
            if (angleZ > maxAngle + halfViewAngle * 0.3f)
            {
                rotationOffset = -(angleZ - maxAngle - halfViewAngle * 0.3f);
            }
            else if (angleZ < minAngle)
            {
                rotationOffset = minAngle - angleZ;
            }
            
            if (!isDrag && (moveVelocity != 0 || rotationOffset != 0))
            {
                var deltaTime = Time.unscaledDeltaTime;

                if (rotationOffset != 0)
                {
                    float speed = moveVelocity;
                    float smoothTime = 0.3f;
                
                    var target = angleZ + rotationOffset;
                    
                    if (target < 0.0f)
                    {
                        target = 360 + (angleZ + rotationOffset);
                    }
                    
                    if (target > 180)
                    {
                        target -= 360;
                    }
                    
                    eulerAngles.z = Mathf.SmoothDamp(angleZ, target, ref speed,
                        smoothTime, Mathf.Infinity, deltaTime);
                    
                    if (Mathf.Abs(speed) < 1)
                        speed = 0;
                    moveVelocity = speed;
                }
                else
                {
                    moveVelocity *= Mathf.Pow(0.135f, deltaTime);
                    
                    if (Mathf.Abs(moveVelocity) < 0.01f)
                        moveVelocity = 0;
                    
                    var deltaRotation = moveVelocity * deltaTime;
                    
                    eulerAngles.z += deltaRotation;
                }
              
                view.albumContent.localEulerAngles = eulerAngles;

                UpdateCardSetViewScale(eulerAngles.z);
            }
            
            UpdateFortuneExchangeCountDown();
        }

        public void UpdateCardSetViewScale(float eulerZ)
        {
            for (var i = 0; i < view.cardSetViews.Count; i++)
            {
                var z = view.cardSetViews[i].transform.localEulerAngles.z;

                if (z > 180)
                {
                    z -= 360;
                }
                    
                var deltaDegrees = z - (-eulerZ);

                if (Mathf.Abs(deltaDegrees) > 6)
                {
                    view.cardSetViews[i].transform.localScale = view.albumDefaultScale;
                }
                else
                {
                    var scale = view.albumDefaultScale.x + (1 - Mathf.Abs(deltaDegrees) / 6) *  (1 - view.albumDefaultScale.x);
                    view.cardSetViews[i].transform.localScale = new Vector3(scale, scale, scale);
                }
            }
        }
        
        public void OnDragAlbumEnd(PointerEventData pointerEventData)
        {
            
            isDrag = false;
        }
    }
}