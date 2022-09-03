// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/10/17:06
// Ver : 1.0.0
// Description : LevelRushDoubleXpPopup.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UILevelRushDoubleXpPop", "UILevelRushDoubleXpPopV")]
    public class LevelDoubleXpPopup : Popup<LevelDoubleXpPopupViewController>
    {
        [ComponentBinder("TimeText")] public Text timeText;

        [ComponentBinder("TurnOnButton")] public Button turnOnButton;

        public LevelDoubleXpPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1300,1200);
        }
        
        public override Vector3 CalculateScaleInfo()
        {
            if(contentDesignSize == Vector2.zero)
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
    }

    public class LevelDoubleXpPopupViewController : ViewController<LevelDoubleXpPopup>
    {
        private LevelRushController _levelRushController;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.turnOnButton.onClick.AddListener(OnTurnOnButtonClicked);
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _levelRushController = Client.Get<LevelRushController>();
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(2);
        }

        public void OnTurnOnButtonClicked()
        {
            var closeAction = view.GetCloseAction();
            view.ResetCloseAction();
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventLevelrushTurniton);
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), closeAction, $"LevelRush")));
            view.Close();
        }

        public override void Update()
        {
            var leftTime = _levelRushController.GetLevelRushLeftTime();
            if (leftTime > 0)
            {
                view.timeText.text = XUtility.GetTimeText(leftTime);
            }
            else
            {
                view.Close();
            }
        }
    }
}