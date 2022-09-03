// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/20/14:31
// Ver : 1.0.0
// Description : JackpotNotificationView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIBroadcast")]
    public class JackpotNotificationView : View<JackpotNotificationViewController>
    {
        [ComponentBinder("Root/MainGroup/PlayerAvatar/Icon")]
        public RawImage rawImageAvatar;

        [ComponentBinder("Root/MainGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("Root/MainGroup/PlayerNameText")]
        public Text textName;

        [ComponentBinder("Root/MainGroup/SlotIcon")]
        public Image imageSlotIcon;

        [ComponentBinder("Root/MainGroup/SlotIcon")]
        public Transform slotIconRoot;

        [ComponentBinder("Root")]
        private RectTransform rootNode;

        private const float UIWidth = 400;

        private Vector2 _startPosition = new Vector2(UIWidth, -68);

        private Vector2 _endPosition = new Vector2(0, -68);


        private Vector3 _scale = Vector3.one * 0.36f;

        private Sequence _sequenceShow;

        private AssetReference _assetReference;

        private GameObject _icon;

        private float _animationDuration = 0.7f;

        private float _showDuration = 3.0f;

        public JackpotNotificationView(string address)
            : base(address)
        {

        }

        public void AdaptUI()
        {
            float offset = 0;

            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;

                    offset = (1 - localScale) * ViewResolution.designSize.x * 0.5f;
                }
            }

            _endPosition.x = offset;

            _startPosition.x = offset + UIWidth;
        }

        public void ShowView(SJackpotNotification data)
        {
            tmpTextIntegral.text = data.Pay.GetCommaFormat();

            textName.text = data.Name;

            if (rawImageAvatar != null)
            {
                rawImageAvatar.texture = AvatarController.defaultAvatar;

                AvatarController.GetAvatar(data.AvatarId, data.FacebookId, (t =>
                {
                    if (rawImageAvatar != null)
                    {
                        rawImageAvatar.texture = t;
                    }
                }));
            }

            imageSlotIcon.enabled = false;

            var iconAddress = $"Icon{data.GameId}";

            ReleaseAsset();
            _assetReference = AssetHelper.PrepareAsset<GameObject>(iconAddress, (ar) =>
            {
                _icon = ar.InstantiateAsset<GameObject>();
                _icon.transform.SetParent(slotIconRoot, false);
                _icon.transform.localScale = _scale;
            });

            rootNode.anchoredPosition = _startPosition;

            if (_sequenceShow != null) { _sequenceShow.Kill(); }

            _sequenceShow = DOTween.Sequence();

            _sequenceShow.Append(DOTween.To(
                () => rootNode.anchoredPosition,
                (x => rootNode.anchoredPosition = x),
                _endPosition, _animationDuration).SetEase(Ease.InBack)
            );

            _sequenceShow.AppendInterval(_showDuration);
            _sequenceShow.AppendCallback(HideView);
        }

        public void ReleaseAsset()
        {
            if (_assetReference != null)
            {
                _assetReference.ReleaseOperation();
                _assetReference = null;
            }

            if (_icon != null)
            {
                GameObject.Destroy(_icon);
                _icon = null;
            }
        }

        private void HideView()
        {
            DOTween.To(
                () => rootNode.anchoredPosition,
                (x => rootNode.anchoredPosition = x),
                _startPosition, _animationDuration
            ).SetEase(Ease.OutBack);
        }
    }

    public class JackpotNotificationViewController : ViewController<JackpotNotificationView>
    {

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.AdaptUI();
        }

        public override void OnViewDestroy()
        {
            view.ReleaseAsset();

            base.OnViewDestroy();
        }
    }
}