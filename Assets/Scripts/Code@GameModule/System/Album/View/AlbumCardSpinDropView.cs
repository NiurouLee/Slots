// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/16/10:45
// Ver : 1.0.0
// Description : AlbumCardSpinDropView.cs
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
    [AssetAddress("MachineSpinCardDropView")]
    public class AlbumCardSpinDropView : View<ViewController>
    {
        [ComponentBinder("Root")] public RectTransform root;

        [ComponentBinder("Root/MoreBetterCards/CloseButton")]
        public Button closeButton;

        [ComponentBinder("Root/NormalCardPack")]
        public RectTransform normalCardPack;

        [ComponentBinder("Root/LuckyCardPack")]
        public RectTransform luckyCardPack;

        [ComponentBinder("Root/MoreBetterCards/MoreBetterCardsBg")]
        public RectTransform moreBetterCardsBg;

        private AssetReference albumAssetRef = null;

        private List<AlbumCardView> _cardViews;

        private TaskCompletionSource<bool> assetRefLoadTask;

        public AlbumCardSpinDropView(string address)
            : base(address)
        {
        }

        protected override void OnViewSetUpped()
        {
            var childCount = moreBetterCardsBg.childCount;

            _cardViews = new List<AlbumCardView>();

            for (var i = 0; i < childCount; i++)
            {
                _cardViews.Add(AddChild<AlbumCardView>(moreBetterCardsBg.GetChild(i).GetChild(0)));
            }

            assetRefLoadTask = new TaskCompletionSource<bool>();
            var spriteAtlasName = Client.Get<AlbumController>().GetAlbumSpriteAtlasName();
            AssetHelper.PrepareAsset<SpriteAtlas>(spriteAtlasName, (assetRef) =>
            {
                albumAssetRef = assetRef;
                assetRefLoadTask.TrySetResult(true);
            });

            closeButton.onClick.AddListener(OnCloseButtonClicked);

            base.OnViewSetUpped();

            if (ViewManager.Instance.IsPortrait)
            {
                root.anchoredPosition = new Vector2(0, -180);
                root.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                var scale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                scale = scale < 1 ? 1 / scale : 1;
                root.anchoredPosition =
                    new Vector2(ViewResolution.referenceResolutionLandscape.x * (scale - 1) * 0.5f, -140);
            }
        }

        private bool _finishEventDispatched = false;
        private CardUpdateInfo _cardUpdateInfo = null;

        public async void OnCloseButtonClicked()
        {
            var animator = transform.GetComponent<Animator>();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Open"))
            {
                if (!_finishEventDispatched)
                {
                    EventBus.Dispatch(new EventShowSpinDropCardFinished(_cardUpdateInfo));
                    _finishEventDispatched = true;
                }

                await XUtility.PlayAnimationAsync(animator, "Close");
                Destroy();
            }
        }

        public override void Destroy()
        {
            base.Destroy();

            if (albumAssetRef != null)
            {
                albumAssetRef.ReleaseOperation();
            }
        }

        public async void ShowView(CardUpdateInfo cardUpdateInfo)
        {
            _cardUpdateInfo = cardUpdateInfo;

            await assetRefLoadTask.Task;

            var cardViewCount = _cardViews.Count;

            for (var i = 0; i < cardViewCount; i++)
            {
                if (cardUpdateInfo.cardAcquired.Count > i)
                {
                    _cardViews[i].transform.parent.gameObject.SetActive(true);
                    _cardViews[i].viewController.SetUpCard(cardUpdateInfo.cardAcquired[i],
                        albumAssetRef.GetAsset<SpriteAtlas>(), false, true);
                }
                else
                {
                    _cardViews[i].transform.parent.gameObject.SetActive(false);
                }
            }

            luckyCardPack.gameObject.SetActive(cardUpdateInfo.packageType ==
                                               Item.Types.CardPackage.Types.CardPackageType.Lucky);
            normalCardPack.gameObject.SetActive(cardUpdateInfo.packageType !=
                                                Item.Types.CardPackage.Types.CardPackageType.Lucky);

            var activeCardPack = luckyCardPack.gameObject.activeSelf ? luckyCardPack : normalCardPack;

            var packAnimator = activeCardPack.GetComponent<Animator>();

            XUtility.PlayAnimation(packAnimator, "Show", () =>
            {
                if (activeCardPack != null)
                    activeCardPack.gameObject.SetActive(false);
            });

            var animator = transform.GetComponent<Animator>();

            animator.Play("Open");
 
            viewController.WaitForSeconds(3, () =>
            {
                if (!_finishEventDispatched)
                {
                    EventBus.Dispatch(new EventShowSpinDropCardFinished(cardUpdateInfo));
                    _finishEventDispatched = true;
                }
                
                //显示卡牌完成之后，通知执行后续的表演，LuckyChallenge,以及卡册收集完成等。
                XUtility.PlayAnimation(animator, "Close", () =>
                {
                   
                    Destroy();
                }, viewController);
            });
        }
    }
}