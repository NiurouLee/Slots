using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Audio;


namespace GameModule
{
    public class SpecialBonusProxy11031 : SpecialBonusProxy
    {
        private ExtraState11031 _extraState11031;
        protected ControlPanel controlPanel;

        public SpecialBonusProxy11031(MachineContext context) : base(context)
        {
            _extraState11031 = context.state.Get<ExtraState11031>();
        }

        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_WIN_NUM_ANIMTION);
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }


        protected override async void HandleCustomLogic()
        {
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            var mapLevel = _extraState11031.GetMapLevel();
            var round = _extraState11031.GetMapRound();
            AudioUtil.Instance.StopMusic();
            if (IsFromMachineSetup())
            {
                if (round == 0)
                {
                    //碗开始飞
                    if (mapLevel == 26)
                    {
                        machineContext.view.Get<CollectGroupView11031>().SetProgress(1);
                        await machineContext.view.Get<CollectGroupView11031>().OpenCollectHorse();
                        await ShowLuckPotStartPopup();
                        Proceed();
                    }
                    else
                    {
                        Proceed();
                        AudioUtil.Instance.PlayAudioFx("Welcome");
                        machineContext.view.Get<CollectGroupView11031>().ShowTipView();
                    }
                }
                else
                {
                    Proceed();
                }
            }
            else
            {
                //碗开始飞
                if (mapLevel == 26)
                {
                    AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Start");
                    await ShowLuckPotStartPopup();
                    await machineContext.view.Get<CollectGroupView11031>().OpenCollectHorse();
                    await machineContext.view.Get<BowlView11031>().Fly();
                    Proceed();
                }
                else
                {
                    await machineContext.view.Get<CollectGroupView11031>().OpenCollectHorse();
                    await machineContext.view.Get<BowlView11031>().Fly();
                    await machineContext.view.Get<LuckyPot11031>().Outro();
                    Proceed();
                }
            }
        }

        private async Task ShowLuckPotStartPopup()
        {
            TaskCompletionSource<bool> taskStartPopup = new TaskCompletionSource<bool>();
            var view = PopUpManager.Instance.ShowPopUp<LuckyPotStart1Popup11031>("UIMiniGame_Start");
            view.SetPopUpCloseAction(async () => { taskStartPopup.SetResult(true); });
            await taskStartPopup.Task;
        }
    }
}