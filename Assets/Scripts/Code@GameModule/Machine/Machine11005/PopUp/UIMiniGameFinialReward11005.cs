
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIMiniGameFinialReward11005: MachinePopUp
    {
        
        [ComponentBinder("IntegralText")]
        protected Text txtNum;
        
        [ComponentBinder("CollectButton")]
        protected Button btnCollect;
        
        public UIMiniGameFinialReward11005(Transform transform) : base(transform)
        {
            btnCollect.onClick.AddListener(OnBtnCollectClick);
        }
        
        
        
        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task RefresUI()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            var num = context.state.Get<ExtraState11005>().GetMapInfo().JackpotWin;
            txtNum.SetText(num.GetCommaFormat());

            await _taskCompletionSource.Task;
        }

        private void OnBtnCollectClick()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            _taskCompletionSource?.SetResult(true);
            Close();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            PopUpManager.Instance.SetGrayMaskState(false);
            AudioUtil.Instance.PlayAudioFx("BonusComplete_Open_1");
        }

        public override Task OnClose()
        {
            AudioUtil.Instance.PlayAudioFx("BonusComplete_Close");
            return base.OnClose();
        }
    }
}