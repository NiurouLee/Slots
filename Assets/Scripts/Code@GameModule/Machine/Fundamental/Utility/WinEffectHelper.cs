// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/25/11:43
// Ver : 1.0.0
// Description : WinEffectHelper.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;

namespace GameModule
{
    public static class WinEffectHelper
    {
        public static async Task ShowBigWinEffectAsync(uint winLevel, ulong winChips, MachineContext context,
            bool isNeedFadeInMusic = false)
        {
            var waitTask = new TaskCompletionSource<bool>();
            context.AddWaitTask(waitTask, null);

            ShowBigWinEffect(winLevel, winChips,
                () =>
                {
                    context.RemoveTask(waitTask);
                    waitTask.SetResult(true);
                },
                isNeedFadeInMusic);
            await waitTask.Task;
        }
        
        public static void ShowBigWinEffect(uint winLevel, ulong winChips, Action winEffectEndAction, bool isNeedFadeInMusic = false)
        {
            var address = WinEffectPopUp.GetEffectAddress(winLevel);

            if (address == null)
            {
                winEffectEndAction.Invoke();
                return;
            }

            if (string.IsNullOrEmpty(address))
            {
                winEffectEndAction?.Invoke();
                return;
            }
           
            //      AudioUtil.Instance.FadeOutMusic();

            var winEffectDialog = PopUpManager.Instance.ShowPopUp<WinEffectPopUp>(address);

            winEffectDialog.InitWith(winLevel, winChips, address,() =>
            {
                if (!isNeedFadeInMusic)
                {
                    //  AudioUtil.Instance.FadeInMusic();
                }

                winEffectEndAction?.Invoke();
            });
        }
    }
}