using UnityEngine;
using UnityEngine.UI;

namespace GameModule.PopUp
{
    public class UIFeatureGameFinish11013 : MachinePopUp
    {
        [ComponentBinder("BetText")] protected Text txtMultiplier;

        [ComponentBinder("IntegralText")] protected Text txtTotelGet;

        [ComponentBinder("ArithmeticText")] protected Text txtArithmetical;

        [ComponentBinder("ConfirmButton")] protected Button btnClose;

        public UIFeatureGameFinish11013(Transform transform) : base(transform)
        {
            btnClose.onClick.AddListener(OnBtnCloseClick);
        }

        private async void OnBtnCloseClick()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            btnClose.interactable = false;
            var bounds = await extraState.SendBonusProcess();
            this.Close();
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("FeatureGame_CollectPanel");
            base.OnOpen();
        }

        private ExtraState11013 extraState;

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = this.context.state.Get<ExtraState11013>();
            var info = extraState.GetExtraInfo();
            txtMultiplier.text = $"*{info.MapMultiplier + 1}";
            ulong totalBet = info.MapBase * (info.MapMultiplier + 1);
            txtTotelGet.text = totalBet.GetCommaFormat();
            txtArithmetical.text =
                $"{info.MapBase.GetCommaFormat()} * {info.MapMultiplier + 1} = {totalBet.GetCommaFormat()}";
        }
    }
}