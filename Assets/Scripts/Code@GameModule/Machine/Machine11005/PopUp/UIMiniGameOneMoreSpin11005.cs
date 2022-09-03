using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIMiniGameOneMoreSpin11005: MachinePopUp
    {
        [ComponentBinder("StartButton")]
        protected Button btnStart;
        
        public UIMiniGameOneMoreSpin11005(Transform transform) : base(transform)
        {
            btnStart.onClick.AddListener(OnBtnStartClick);
        }


        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("BonusSuprise1_Open");
            base.OnOpen();
        }

        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task RefresUI()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            await _taskCompletionSource.Task;
        }
        private void OnBtnStartClick()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            _taskCompletionSource?.TrySetResult(true);
            Close();
        }
        
        public override bool IsCloseMustUnpauseMusic()
        {
            return true;
        }
    }
}