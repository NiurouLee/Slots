using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Network.API;
using DragonU3DSDK.Network.API.Protocol;
using DragonU3DSDK.Storage;

namespace GameModule
{

    public class ContactUsController : LogicController
    {
        private static readonly DateTime T0 = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        public ContactUsController(Client client) : base(client) { }

        public static double GetTimeStampMilliseconds()
        {
            TimeSpan ts = DateTime.UtcNow - T0;
            return ts.TotalMilliseconds;
        }

        public static DateTime GetDateTimeFromMilliseconds(ulong milliseconds)
        {
            return T0.AddMilliseconds(milliseconds);
        }

        public static bool IsValidEmail(string email)
        {
            string pattern = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            var regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            SetUpCheckNewMessageAction();
            finishCallback?.Invoke();
            await Task.CompletedTask;
        }

        private SListUserComplainMessage _data;

        public async Task<SListUserComplainMessage> SendCListUserComplainMessage()
        {
            var tcs = new TaskCompletionSource<SListUserComplainMessage>();

            var c = new CListUserComplainMessage();
            APIManagerBridge.Send(c,
                (response) =>
                {
                    XDebug.Log($"11111111111 Receive SListUserComplainMessage:{_data}");

                    _data = response as SListUserComplainMessage;

                    PrependMessage(CreateServiceMessage());

                    tcs.SetResult(_data);
                },
                (errorCode, message, response) =>
                {
                    XDebug.Log($"11111111111 Receive SListUserComplainMessage with error:{message}");

                    tcs.SetResult(null);
                });

            return await tcs.Task;
        }

        public static UserComplainMessage CreateMessage(string email, string message)
        {
            var storageCommon = StorageManager.Instance.GetStorage<StorageCommon>();

            return new UserComplainMessage
            {
                Message = message,
                MessageType = UserComplainMessage.Types.MessageType.Complain,
                PlayerId = StorageManager.Instance.GetStorage<StorageCommon>().PlayerId,
                CreatedAt = APIManager.Instance.GetServerTime(),
                Email = email,
                RevenueUsdCents = storageCommon.RevenueUSDCents,
                LastLevel = (uint)Client.Get<UserController>().GetUserLevel(),
                DeviceType = DeviceHelper.GetDeviceType(),
                DeviceModel = DeviceHelper.GetDeviceModel(),
                DeviceMemory = DeviceHelper.GetTotalMemory().ToString(),
                NetworkType = DeviceHelper.GetNetworkStatus().ToString(),
                ResVersion = storageCommon.ResVersion,
                NativeVersion = DragonNativeBridge.GetVersionCode().ToString(),
                Platform = DeviceHelper.GetPlatform(),
            };
        }

        public UserComplainMessage CreateServiceMessage()
        {
            return new UserComplainMessage
            {
                Message = "Please tell us your suggestions and questions.",
                MessageType = UserComplainMessage.Types.MessageType.Reply,
                PlayerId = 0,
                CreatedAt = APIManager.Instance.GetServerTime(),
                Email = string.Empty,
                RevenueUsdCents = 0,
                LastLevel = 0,
                DeviceType = string.Empty,
                DeviceModel = string.Empty,
                DeviceMemory = string.Empty,
                NetworkType = string.Empty,
                ResVersion = string.Empty,
                NativeVersion = string.Empty,
                Platform = default(Platform),
            };
        }

        public void PrependMessage(UserComplainMessage message)
        {
            if (_data == null) { _data = new SListUserComplainMessage(); }
            var messages = _data.Messages;
            if (messages.Count < 50)
            {
                messages.Insert(0, message);
            }
        }

        public void AppendMessage(UserComplainMessage message)
        {
            if (_data == null) { _data = new SListUserComplainMessage(); }
            var messages = _data.Messages;
            messages.Add(message);
        }

        public void SendCSendUserComplainMessage(string email, string message)
        {
            var sendMessageData = CreateMessage(email, message);
            var c = new CSendUserComplainMessage() { Message = sendMessageData };
            APIManagerBridge.Send(c,
                (response) =>
                {
                    AppendMessage(sendMessageData);
                    EventBus.Dispatch(new EventReceiveSSendUserComplainMessage());
                    XDebug.Log($"11111111111 Receive SSendUserComplainMessage");
                },
                (errorCode, msg, response) =>
                {
                    XDebug.Log($"11111111111 Receive SSendUserComplainMessage with error:{msg}");
                });
        }

        public SListUserComplainMessage GetSListUserComplainMessage()
        {
            return _data;
        }

        private bool _haveNewUnReadMessage = false;
        
        public void ResetNewMessageState()
        {
            if (_haveNewUnReadMessage)
            {
                _haveNewUnReadMessage = false;
                SetUpCheckNewMessageAction();
            }
        }
  
        public void SetUpCheckNewMessageAction()
        {
            if (!_haveNewUnReadMessage)
            {
                CheckNewMessage();

                //三分钟轮询一次
                WaitForSeconds(180, SetUpCheckNewMessageAction);
            }
        }
      
        public void CheckNewMessage()
        {
            CCheckUserComplainMessage c = new CCheckUserComplainMessage();
            
            APIManagerBridge.Send(c,
                (response) =>
                {
                    var checkResult = response as SCheckUserComplainMessage;

                    if (checkResult != null && checkResult.Result)
                    {
                        if (_haveNewUnReadMessage != checkResult.Result)
                        {
                            _haveNewUnReadMessage = checkResult.Result;
                            
                            if (!PopupStack.HasPopup(typeof(UIContactUsPopup)))
                            {
                                EventBus.Dispatch(new EventContactUsHasNewMessage(_haveNewUnReadMessage));
                            }
                        }
                    }
                },
                (errorCode, msg, response) =>
                {
                    XDebug.Log($"11111111111 Receive SSendUserComplainMessage with error:{msg}");
                });
        }

        public bool HasNewUnReadMessage()
        {
            return _haveNewUnReadMessage;
        }
    }
}
