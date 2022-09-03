using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class AmazingHatModel : Model<HatGameInfo>
    {
        public int Level => (int)modelData.CurrentStep;
        public HatGameInfo.Types.HatColor HatColor => modelData.HatColor;
        public bool HasRabitTip => modelData.HasRabitTip;
        public bool HasLuckyCardTip => modelData.HasLuckyCardTip;
        public RepeatedField<Reward> Rewards => modelData.RewardsCanCollected;
        public int CostDiamond => (int)modelData.ReviceCost;
        public RepeatedField<uint> SelectedHatIndex => modelData.SelectedHatIndexsRecord;
        public HatGameInfo.Types.HatGameStat HatGameStat => modelData.HatGameStat;

        public AmazingHatModel() : base(ModelType.TYPE_AMAZING_HAT_INFO)
        {
        }

        public async Task<SHatGameInfo> GetModalDataFromServer()
        {
            CHatGameInfo cHatGameInfo = new CHatGameInfo();
            var sGetHatGameInfo =
                await APIManagerGameModule.Instance.SendAsync<CHatGameInfo, SHatGameInfo>(cHatGameInfo);

            if (sGetHatGameInfo.ErrorCode == 0)
            {
                UpdateModelData(sGetHatGameInfo.Response.HatGameInfo);
                return sGetHatGameInfo.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sGetHatGameInfo.ErrorInfo));
                // XDebug.LogError("GetHatGame Response Error:" + sGetHatGameInfo.ErrorInfo);
            }
            return null;
        }

        public async Task<SHatGameSelect> SendSelectHat(uint selectIndex)
        {
            CHatGameSelect cHatGameSelect = new CHatGameSelect();
            cHatGameSelect.Index = selectIndex;
            var sHatGameSelect =
                await APIManagerGameModule.Instance.SendAsync<CHatGameSelect, SHatGameSelect>(cHatGameSelect);

            if (sHatGameSelect.ErrorCode == 0)
            {
                UpdateModelData(sHatGameSelect.Response.HatGameInfo);
                return sHatGameSelect.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sHatGameSelect.ErrorInfo));
                // XDebug.LogError("SendSelectHat Response Error:" + sHatGameSelect.ErrorInfo);
            }

            return null;
        }

        public async Task<SHatGameHandleRabbit> SendHatGameRevive(Action<bool> callback, bool hasWatchedAdv, bool revive)
        {
            CHatGameHandleRabbit cHatGameRevive = new CHatGameHandleRabbit();
            cHatGameRevive.HasWatchedAdv = hasWatchedAdv;
            cHatGameRevive.Revive = revive;
            var sHatGameRevive =
                await APIManagerGameModule.Instance.SendAsync<CHatGameHandleRabbit, SHatGameHandleRabbit>(
                    cHatGameRevive);
            if (sHatGameRevive.ErrorCode == 0)
            {
                UpdateModelData(sHatGameRevive.Response.HatGameInfo);
                EventBus.Dispatch(new EventUserProfileUpdate(sHatGameRevive.Response.UserProfile));

                callback?.Invoke(true);
                return sHatGameRevive.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(sHatGameRevive.ErrorInfo));
                // XDebug.LogWarning("SendHatGameRevive Response Error:" + sHatGameRevive.ErrorInfo);
            }

            callback?.Invoke(false);
            return null;
        }
    }
}