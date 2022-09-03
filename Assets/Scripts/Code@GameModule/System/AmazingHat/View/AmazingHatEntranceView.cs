using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class AmazingHatEntranceView : View<AmazingHatEntranceViewController>
    {
        [ComponentBinder("LockState")]
        public Transform _lockState;

        [ComponentBinder("Play")]
        public Transform _playBtnImg;

        [ComponentBinder("TimerGroup/TimerText")]
        public Text _timerText;

        [ComponentBinder("TimerGroup")]
        public Transform _timerGroup;

        public void RefreshUI(bool isOpen)
        {
            _timerGroup.gameObject.SetActive(!isOpen);
            _playBtnImg.gameObject.SetActive(isOpen);
            _lockState.gameObject.SetActive(!isOpen);
        }
    }

    public class AmazingHatEntranceViewController : ViewController<AmazingHatEntranceView>
    {
        private AmazingHatController _controller;

        private Button _playBtn;
        
        private bool _lastState;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _playBtn = view.transform.GetComponent<Button>();
            _playBtn.onClick.AddListener(OnEntranceBtnClick);

            _controller = Client.Get<AmazingHatController>();
        }

        private void OnEntranceBtnClick()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(AmazingHatMainPopup), "Album")));
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatEnter, ("Operation:", "AlbumIcon"),("OperationId","1"));
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            bool isOpen = _controller.IsOpen();
            view.RefreshUI(isOpen);
            _playBtn.interactable = isOpen;
            _lastState = isOpen;
            if (!isOpen)
            {
                float countDown = _controller.GetCountDown();
                view._timerText.text = XUtility.GetTimeText(countDown);
            }
            EnableUpdate(1);
        }

        public override void Update()
        {
            bool isOpen = _controller.IsOpen();
            // Debug.Log("-----------isOpen: " + isOpen);
            if (isOpen != _lastState)
            {
                _playBtn.interactable = isOpen;
                view.RefreshUI(isOpen);
            }
            if (!isOpen)
            {
                float countDown = _controller.GetCountDown();
                view._timerText.text = XUtility.GetTimeText(countDown);
            }
        }
    }
}