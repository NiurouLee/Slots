using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIContactUs")]
    public class UIContactUsPopup : Popup<UIContactUsPopupController>
    {
        [ComponentBinder("Root/MainGroup/MailGroup/InputField")]
        public InputField inputEmail;

        [ComponentBinder("Root/MainGroup/DialogScrollView/Viewport/Content")]
        public Transform transformContent;

        [ComponentBinder("Root/MainGroup/DialogScrollView/Viewport/Content/ServerDialog")]
        public Transform transformServerDialog;

        [ComponentBinder("Root/MainGroup/DialogScrollView/Viewport/Content/PlayerDialog")]
        public Transform transformPlayerDialog;

        [ComponentBinder("Root/MainGroup/InputGroup/InputField")]
        public InputField inputContent;

        [ComponentBinder("Root/MainGroup/InputGroup/InputField/TextLimite")]
        public Text tmpTextCharacter;

        [ComponentBinder("Root/MainGroup/InputGroup/SendButton")]
        public Button buttonSend;

        [ComponentBinder("Root/MainGroup/DialogScrollView")]
        public Transform transformLoopList;

        private LoopListView2 _loopList;


        public UIContactUsPopup(string address) : base(address) { }


        private readonly Dictionary<Transform, UIContactUsDialogView> _transformToDialogView = new Dictionary<Transform, UIContactUsDialogView>();

        private ContactUsController _controller;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            _controller = Client.Get<ContactUsController>();

            _loopList = transformLoopList.gameObject.AddComponent<LoopListView2>();
            _loopList.Init();
            _loopList.ArrangeType = ListItemArrangeType.TopToBottom;

            ItemPrefabConfData item1 = new ItemPrefabConfData();
            item1.mItemPrefab = transformPlayerDialog.gameObject;
            item1.mInitCreateCount = 1;
            //item1.mPadding = 150;

            ItemPrefabConfData item2 = new ItemPrefabConfData();
            item2.mItemPrefab = transformServerDialog.gameObject;
            item2.mInitCreateCount = 1;
            //item2.mPadding = 150;

            _loopList.mItemPrefabDataList.Add(item1);
            _loopList.mItemPrefabDataList.Add(item2);

            _loopList.InitListView(0, OnGetItemByIndex);

            _loopList.ScrollRect.movementType = ScrollRect.MovementType.Clamped;

            inputContent.characterLimit = 200;

            var limit = inputContent.characterLimit;
            tmpTextCharacter.text = $"{limit}/{limit}";
        }

        public override void OnOpen()
        {
            base.OnOpen();
            Set();
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 loopList, int index)
        {
            var s = _controller.GetSListUserComplainMessage();
            int count = 0;
            if (s != null && s.Messages != null) { count = s.Messages.Count; }

            if (index < 0 || index >= count) { return null; }

            var data = s.Messages[index];
            var complain = data.MessageType == UserComplainMessage.Types.MessageType.Complain;

            var itemName = complain ? "PlayerDialog" : "ServerDialog";
            var item = loopList.NewListViewItem(itemName);

            var dialogView = GetDialogView(item.transform);
            if (dialogView == null)
            {
                dialogView = AddChild<UIContactUsDialogView>(item.transform);
                _transformToDialogView.Add(item.transform, dialogView);
            }

            if (dialogView != null) { dialogView.Set(data); loopList.OnItemSizeChanged(index); }

            return item;
        }

        private UIContactUsDialogView GetDialogView(Transform transform)
        {
            UIContactUsDialogView result = null;
            _transformToDialogView.TryGetValue(transform, out result);
            return result;
        }

        public void Set()
        {
            var count = 0;
            var s = _controller.GetSListUserComplainMessage();
            if (s != null && s.Messages != null) { count = s.Messages.Count; }

            _loopList.SetListItemCount(count);
            _loopList.RefreshAllShownItem();

            if (count > 0)
            {
                _loopList.MovePanelToItemIndex(count - 1, 0);
            }
        }

        public void ClearInput()
        {
            inputEmail.text = null;
            inputContent.text = null;
            buttonSend.interactable = false;
        }
    }

    public class UIContactUsPopupController : ViewController<UIContactUsPopup>
    {
        private ContactUsController _controller;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            _controller = Client.Get<ContactUsController>();
            view.ClearInput();
        }

        public override void OnViewDestroy()
        {
            base.OnViewDestroy();
            EventBus.Dispatch(new EventContactUsHasNewMessage(Client.Get<ContactUsController>().HasNewUnReadMessage()));
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventReceiveSListUserComplainMessage>(OnEventReceiveListUserComplainMessage);
            SubscribeEvent<EventReceiveSSendUserComplainMessage>(OnEventReceiveSSendUserComplainMessage);

            view.inputContent.onValueChanged.AddListener(OnValueChanged);

            view.buttonSend.onClick.AddListener(OnButtonSendClick);
        }

        private void OnValueChanged(string text)
        {
            view.buttonSend.interactable = !string.IsNullOrWhiteSpace(text);
            var limit = view.inputContent.characterLimit;
            view.tmpTextCharacter.text = $"{limit - text.Length}/{limit}";
        }

        private void OnButtonSendClick()
        {
            var email = view.inputEmail.text;
            var content = view.inputContent.text;
            if (string.IsNullOrWhiteSpace(content) == false)
            {
                _controller.SendCSendUserComplainMessage(email, content);
            }
            view.ClearInput();
        }

        private void UpdateView()
        {
            view.Set();
        }

        private void OnEventReceiveListUserComplainMessage(EventReceiveSListUserComplainMessage obj)
        {
            UpdateView();
        }

        private void OnEventReceiveSSendUserComplainMessage(EventReceiveSSendUserComplainMessage obj)
        {
            UpdateView();
        }
    }
}
