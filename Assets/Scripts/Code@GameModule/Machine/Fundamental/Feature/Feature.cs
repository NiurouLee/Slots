// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/24/15:05
// Ver : 1.0.0
// Description : Feature.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class Feature
    {
        protected string featureName;
        
        protected MachineContext machineContext;
        
        public Feature()
        {
            featureName = this.GetType().Name;
        }
        
        public Feature(string inFeatureName)
        {
            featureName = inFeatureName;
        }

        public virtual void Initialize(MachineContext inMachineContext)
        {
            machineContext = inMachineContext;
        }
         
        public virtual bool MatchFilter(string filter)
        {
            return featureName == filter;   
        }
 
        public void UpdateSpinUiViewTotalBet(bool lockBet, bool updateControlPanelState = true)
        {
            var betState = machineContext.state.Get<BetState>();
            var controlPanel = machineContext.view.Get<ControlPanel>();
            controlPanel.SetTotalBet(betState.totalBet, betState.IsMaxBet(), betState.IsMinBet(), betState.IsExtraBet(),
                lockBet);
            if (updateControlPanelState)
                controlPanel.UpdateControlPanelState(false, false);
        }

        public virtual void ForceUpdateWinChipsToDisplayTotalWin(float effectDuration = 0.5f,
            bool needPlayWinOutAnimation = false)
        {
            var winState = machineContext.state.Get<WinState>();

            winState.SetCurrentWin(winState.displayTotalWin);

            machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long) winState.currentWin,
                effectDuration, needPlayWinOutAnimation, "", "");
        }

        public virtual WinLevel UpdateWinChipsToDisplayTotalWin(bool withAudio = true, bool withAutoAni = true,
            bool noWinOutAnimation = false)
        {
            var winState = machineContext.state.Get<WinState>();

            var deltaWin = winState.displayTotalWin - winState.currentWin;

            return AddWinChipsToControlPanel(deltaWin, 0, withAudio, noWinOutAnimation);
        }

        public virtual WinLevel AddWinChipsToControlPanel(ulong winChips, float configDuration = 0f,
            bool withAudio = true, bool withAutoAni = true, string specifiedAudioName = null, bool audioLoop = false)
        {
            if (winChips > 0)
            {
                //Debug.LogError($"=======aaaa currentWin:{machineContext.state.Get<WinState>().currentWin}");

                machineContext.state.Get<WinState>().AddCurrentWin(winChips);
                //Debug.LogError($"=======bbbb currentWin:{machineContext.state.Get<WinState>().currentWin} add:{winChips} origin:{machineContext.state.Get<WinState>().currentWin - winChips}");

                var winLevel = machineContext.state.Get<BetState>().GetSmallWinLevel((long) winChips);

                //     var machineConfig = MachineState.GetMachineConfig();

                string audioName = null;
                string stopAudioName = null;
                float effectDuration = 0.5f;

                bool needPlayWinOutAnimation = winLevel == WinLevel.NiceWin && withAutoAni;

                effectDuration = 0.5f;

                if (winLevel != WinLevel.NoWin)
                {
                    if (winLevel == WinLevel.SmallWin)
                    {
                        audioName = "Symbol_SmallWin_" + machineContext.assetProvider.AssetsId;
                        stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                        effectDuration = 1.0f;
                    }
                    else if (winLevel == WinLevel.Win || !withAutoAni)
                    {
                        audioName = "Symbol_Win_" + machineContext.assetProvider.AssetsId;
                        stopAudioName = "Symbol_WinEnding_" + machineContext.assetProvider.AssetsId;
                        effectDuration = 2.0f;
                    }
                    else if (winLevel == WinLevel.NiceWin)
                    {
                        //Nice Win走通用音效
                        effectDuration = 6;
                    }
                }

                if (configDuration > 0)
                {
                    effectDuration = configDuration;
                }

                if (!string.IsNullOrEmpty(specifiedAudioName))
                {
                    audioName = specifiedAudioName;
                    stopAudioName = "Symbol_SmallWinEnding_" + machineContext.assetProvider.AssetsId;
                }

                if (!withAudio)
                {
                    audioName = string.Empty;
                    stopAudioName = string.Empty;
                }

                machineContext.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation(
                    (long) machineContext.state.Get<WinState>().currentWin,
                    effectDuration, needPlayWinOutAnimation, audioName, stopAudioName, audioLoop);

                return winLevel;
            }

            return WinLevel.NoWin;
        }

        public void OnDestroy()
        {
            
        }

    }
}