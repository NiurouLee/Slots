using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIMiniGameReward11005: MachinePopUp
    {

        [ComponentBinder("IntegralText")]
        protected Text txtNum;

        [ComponentBinder("CollectButton")]
        protected Button btnCollect;
        
        public UIMiniGameReward11005(Transform transform) : base(transform)
        {
            btnCollect.onClick.AddListener(OnBtnCollectClick);
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("BonusSuprise2_Open");
            base.OnOpen();
        }

        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task RefresUI(int num)
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            txtNum.SetText(num.GetCommaFormat());

            await _taskCompletionSource.Task;
        }

        private void OnBtnCollectClick()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            _taskCompletionSource?.SetResult(true);
            Close();
        }

        
    }
}