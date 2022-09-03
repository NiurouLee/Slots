using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIMiniGameStart11005:MachinePopUp
    {
        
        
        [ComponentBinder("Root/BottomGroup/Button")]
        private Button startButton;
        
        [ComponentBinder("IntegralText")]
        protected Text txtNum;

        [ComponentBinder("CountText")]
        protected Text txtLetterCount;

        public UIMiniGameStart11005(Transform transform) : base(transform)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }


        public override bool IsCloseMustUnpauseMusic()
        {
            return true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("BonusTrigger_Open_1");
            base.OnOpen();
        }


        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task RefreshUI()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();

            var extralState = context.state.Get<ExtraState11005>();
            txtNum.SetText(extralState.GetMapInfo().LetterWin.GetCommaFormat());
            txtLetterCount.text = extralState.GetMoreLetterCount().ToString();
            
            await _taskCompletionSource.Task;
        }

        private void OnStartClicked()
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            Close();
        }


        public override async Task OnClose()
        {
            await base.OnClose();
            _taskCompletionSource?.SetResult(true);
        }
    }
}