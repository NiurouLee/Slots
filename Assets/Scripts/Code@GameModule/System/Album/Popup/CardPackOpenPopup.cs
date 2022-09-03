// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/16/19:32
// Ver : 1.0.0
// Description : CardPackOpenPopup.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICardPackOpenPopup")]
    public class CardPackOpenPopup : Popup<CardPackOpenPopupViewController>
    {
        [ComponentBinder("Root/BgGroup/TitleText/AmazingCards")]
        public RectTransform amazingCardsTitle;

        [ComponentBinder("Root/BgGroup/TitleText/LuckyPack")]
        public RectTransform luckyPackTile;

        [ComponentBinder("Root/BgGroup/TitleText/NormalPack")]
        public RectTransform normalPackTitle;

        [ComponentBinder("Root/BgGroup/TitleText/AmazingCard")]
        public RectTransform amazingCardTitle;

        [ComponentBinder("Root/CashCraze/NormalCardPack")]
        public RectTransform normalCardPack;

        [ComponentBinder("Root/CashCraze/LuckyCardPack")]
        public RectTransform luckyCardPack;

        [ComponentBinder("Root/Bonus/Bonus01")]
        public RectTransform cardContainer;
        
        [ComponentBinder("Root/LuckySpin/Trail_tuowei")]
        public RectTransform luckySpinFlyFx;

        [ComponentBinder("Root/LuckySpin")] public RectTransform luckySpin;
        [ComponentBinder("Root/LuckySpin/RedDot/LuckySpinCount")] public Text luckySpinCount;

        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button collectButton;

        public CardPackOpenPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1200, 768);
        }

        protected override void OnCloseClicked()
        {
        }

        public override string GetCloseAnimationName()
        {
            return null;
        }
    }

    public class CardPackOpenPopupViewController : ViewController<CardPackOpenPopup>
    {
        protected AlbumController albumController;
        protected AssetReference cardSetAtlasRef;

        protected CardUpdateInfo cardUpdateInfo;

        protected List<AlbumCardView> albumCardViews;

        public override async Task LoadExtraAsyncAssets()
        {
            extraAssetNeedToLoad = new List<string>() {albumController.GetAlbumSpriteAtlasName()};
            await base.LoadExtraAsyncAssets();

            cardSetAtlasRef = GetAssetReference(albumController.GetAlbumSpriteAtlasName());
        }

        public override void OnViewDidLoad()
        {
            albumController = Client.Get<AlbumController>();
            
            albumCardViews = new List<AlbumCardView>();

            var childCount = view.cardContainer.childCount;

            for (var i = 0; i < childCount; i++)
            {
                albumCardViews.Add(view.AddChild<AlbumCardView>(view.cardContainer.GetChild(i).GetChild(0)));
            }

            base.OnViewDidLoad();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);

            if (inExtraData is PopupArgs popupArgs)
            {
                cardUpdateInfo = popupArgs.extraArgs as CardUpdateInfo;
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            if (cardUpdateInfo != null)
            {
                SoundController.PlaySfx("Album_Card_Get_Open1");
                
                var acquireCard = cardUpdateInfo.cardAcquired;

                var atlas = cardSetAtlasRef.GetAsset<SpriteAtlas>();

                for (var i = 0; i < albumCardViews.Count; i++)
                {
                    if (acquireCard.Count > i)
                    {
                        albumCardViews[i].viewController.SetUpCard(acquireCard[i], atlas, inShowNew: true);
                    }
                    else
                    {
                        albumCardViews[i].transform.parent.gameObject.SetActive(false);
                    }
                }
 
                var openType = SetUpOpenTitleAndGetOpenType();
                
                view.luckyCardPack.gameObject.SetActive(openType == 1);
                view.normalCardPack.gameObject.SetActive(openType == 2);
                
                if (openType == 3)
                {
                    ShowDirectOpenCardPack();
                }
                else
                {
                    ShowOpenCardPackHolder(openType);
                }
            }
        }

        protected async void ShowOpenCardPackHolder(int openType)
        {
            WaitForSeconds(0.5f, () =>
            {
                SoundController.PlaySfx("Album_Card_Get_Open2");
            });
         
            XUtility.PlayAnimation(view.animator, "CardHolder_Open", () =>
            {
                if (cardUpdateInfo.luckySpinNewAddCount > 0)
                {
                    ShowAddLuckySpinCountAnimation();
                }
                else
                {
                    view.luckySpin.gameObject.SetActive(false);
                    view.animator.Play("idle_BottomGroup");
                }
            },this);
            
            if (openType == 1)
            {
                view.luckyCardPack.GetComponent<Animator>().Play("Big_Show");
            }
            else
            {
                view.normalCardPack.GetComponent<Animator>().Play("Big_Show");
            }
            
            var cardCountCount = (int)Mathf.Min(cardUpdateInfo.cardAcquired.Count,5);
            
            SoundController.PreloadSoundAssets(new List<string>(){"Album_Card_Fly"}, null);
            await WaitForSeconds(2.7f);
            
            for (var i = 0; i < cardCountCount; i++)
            {
                SoundController.PlaySfx("Album_Card_Fly");
                await WaitNFrame(4);
            }
        }
        
        protected async void ShowDirectOpenCardPack()
        {
            XUtility.PlayAnimation(view.animator,"Normal_Open", () =>
            {
                if (cardUpdateInfo.luckySpinNewAddCount > 0)
                {
                    ShowAddLuckySpinCountAnimation();
                }
                else
                {
                    view.luckySpin.gameObject.SetActive(false);
                    view.animator.Play("idle_BottomGroup");
                }
            },this);
            
            var cardCountCount = (int)Mathf.Min(cardUpdateInfo.cardAcquired.Count,5);
            
            SoundController.PreloadSoundAssets(new List<string>(){"Album_Card_Fly"}, null);
            await WaitNFrame(13);
            
            for (var i = 0; i < cardCountCount; i++)
            {
                SoundController.PlaySfx("Album_Card_Fly");
                await WaitNFrame(4);
            }
        }

        protected void ShowAddLuckySpinCountAnimation()
        {
            var usedLuckySpinCount = albumController.GetUsedLuckySpinCount();
            var oldCount = cardUpdateInfo.currentLuckySpinCount - usedLuckySpinCount -
                           cardUpdateInfo.luckySpinNewAddCount;
            var newCount = cardUpdateInfo.currentLuckySpinCount - usedLuckySpinCount;

            view.animator.Play("LuckySpinIdle");

            var luckyCardList = new List<AlbumCardView>();
            
            view.luckySpinCount.text = oldCount.ToString();
            
            for (var i = 0; i < albumCardViews.Count; i++)
            {
                if (albumCardViews[i].IsActive())
                {
                    var card = cardUpdateInfo.cardAcquired[i];
                    if (card.Type == Card.Types.CardType.Lucky)
                    {
                        luckyCardList.Add(albumCardViews[i]);
                    }
                } 
            }

            WaitForSeconds(0.3f, () =>
            {
                if (view.transform)
                {
                    if (luckyCardList.Count > 0)
                    {
                        //TODO Fly Particle;
                        for (var i = 0; i < luckyCardList.Count; i++)
                        {
                            var fx = GameObject.Instantiate(view.luckySpinFlyFx.gameObject, view.luckySpin);
                            fx.gameObject.SetActive(true);
                            var startWoldPos = luckyCardList[i].transform.position;
                            var startLocalPos = view.luckySpin.InverseTransformPoint(startWoldPos);
                            XUtility.FlyLocal(fx.transform, startLocalPos, Vector3.zero, 100, 1f,
                                actionEnd: () => { GameObject.Destroy(fx); }, context: this);
                        }
                        //Do Local Move On LuckySpin
                    }
                }
            });
            
            WaitForSeconds(1.6f, () =>
            {
                if(view.transform)
                    view.luckySpinCount.text = newCount.ToString();
            });
        }

        //返回值1：表示打开lucky卡，2，表示普通卡包，3 直接出卡
        protected int SetUpOpenTitleAndGetOpenType()
        {
            if (cardUpdateInfo == null)
                return 0;
            
            var packageConfig = cardUpdateInfo.packageConfig;

            //如果服务器未配置TypeForShow，默认值走开卡包动画
            
            if (packageConfig.TypeForShow == 0)
            {
                view.luckyPackTile.gameObject.SetActive(cardUpdateInfo.packageType == Item.Types.CardPackage.Types
                    .CardPackageType.Lucky);
                view.normalPackTitle.gameObject.SetActive(cardUpdateInfo.packageType != Item.Types.CardPackage.Types
                    .CardPackageType.Lucky);
                
                view.amazingCardTitle.gameObject.SetActive(false);
                view.amazingCardsTitle.gameObject.SetActive(false);

                if (cardUpdateInfo.packageType == Item.Types.CardPackage.Types.CardPackageType.Lucky)
                    return 1;
                return 2;
            }
            
            {
                view.amazingCardTitle.gameObject.SetActive(packageConfig.TypeForShow <= 15);
                view.amazingCardsTitle.gameObject.SetActive(packageConfig.TypeForShow >= 16 && packageConfig.TypeForShow <= 18);
                view.luckyPackTile.gameObject.SetActive(packageConfig.TypeForShow == 20);
                view.normalPackTitle.gameObject.SetActive(packageConfig.TypeForShow == 19);

                if (packageConfig.TypeForShow <= 15)
                    return 3;
                
                if (packageConfig.TypeForShow == 19)
                    return 2;
                
                if (packageConfig.TypeForShow == 20)
                    return 1;
                
                return 2;
            }
        }

        protected override void SubscribeEvents()
        {
            view.collectButton.onClick.AddListener(OnCollectButtonClicked);
            view.closeButton.onClick.RemoveAllListeners();
            view.closeButton.onClick.AddListener(OnCloseClicked);
        }
 
        public void OnCollectButtonClicked()
        {
            view.Close();
        }


        public void OnCloseClicked()
        {
            view.Close();
        }
    }
}