// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/09/15:15
// Ver : 1.0.0
// Description : LevelRushFailPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
namespace GameModule 
{
    [AssetAddress("UILevelRushLotto","UILevelRushLottoV")]
    public class LevelRushLottoPlayPopup: Popup
    {
        [ComponentBinder("PlayButton")] 
        public Button playButton;
        
        public LevelRushLottoPlayPopup(string address)
            :base(address)
        {
            contentDesignSize = new Vector2(1100, 768);
        }
        
        public override Vector3 CalculateScaleInfo()
        {
            if (contentDesignSize == Vector2.zero)
                return Vector3.one;

            var viewSize = ViewResolution.referenceResolutionLandscape;

            if (ViewManager.Instance.IsPortrait)
            {
                viewSize = ViewResolution.referenceResolutionPortrait;

                if (viewSize.y < contentDesignSize.y)
                {
                    var scale = viewSize.y / contentDesignSize.y;
                    return new Vector3(scale, scale, scale);
                }
            }
            else
            {
                if (viewSize.x < contentDesignSize.x)
                {
                    var scale = viewSize.x / contentDesignSize.x;
                    return new Vector3(scale, scale, scale);
                }
            }

            return Vector3.one;
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            playButton.onClick.AddListener(OnLottoPlayClick);
        }

        public void OnLottoPlayClick()
        {
            playButton.interactable = false;
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushLottopop,
                ("index", "1"));
            var levelRushController = Client.Get<LevelRushController>();
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LottoBonusFreePopup),
                levelRushController.GetFreeLottoGameInfo(), "LottoPlay", performCategory)));
            Close();
        }

        protected override void OnCloseClicked()
        {
            base.OnCloseClicked();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushLottopop,
                ("index", "2"));
        }
    }
}