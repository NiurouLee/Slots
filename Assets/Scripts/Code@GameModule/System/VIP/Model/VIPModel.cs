using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;


namespace GameModule
{
    public class VipModel : Model<SGetVipInfo>
    {
        private UserController userController;
        public VipModel() : base(ModelType.TYPE_VIP)
        {
            userController = Client.Get<UserController>();
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            CGetVipInfo cGetVipInfo = new CGetVipInfo();
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetVipInfo, SGetVipInfo>(cGetVipInfo);

            if (handler.ErrorCode == ErrorCode.Success)
            {
                UpdateModelData(handler.Response);
            }
        }

        public uint GetVipLevel()
        {
            return userController.GetVipInfo().Level;
        }

        public VipLevelConfig GetVipLevelConfig(int vipLevel)
        {
            return modelData.VipConfig[vipLevel - 1];
        }

        public ulong GetCurrentVipExp()
        {
            return userController.GetVipInfo().ExpCurrent;
        }

        public ulong GetNextVipExp()
        {
            return userController.GetVipInfo().ExpNextLevel;
        }
    }
}