// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/23/18:19
// Ver : 1.0.0
// Description : MiniGameFinishPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class MapFreeFinishPopup11029:FreeSpinFinishPopUp
    {
        public MapFreeFinishPopup11029(Transform transform)
            :base(transform)
        {
            
        }


        public override void OnOpen()
        {
	        var freeSpinState = context.state.Get<FreeSpinState>();
	        if (freeSpinState.freeSpinId == 0)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 1)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 2)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_ClassicEnd_Open");
	        }
	        else if (freeSpinState.freeSpinId == 3 || freeSpinState.freeSpinId == 4 || freeSpinState.freeSpinId == 5)
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_FreeGameEnd_Open");
	        }
	        else
	        {
		        AudioUtil.Instance.StopMusic();
		        AudioUtil.Instance.PlayAudioFx("Map_FreeGameEnd_Open");
	        }

	        if (animator != null && animator.HasState("Open"))
		        animator.Play("Open");
        }
        
  
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if (freeSpinWinChipText)
            {
                var totalWin = context.state.Get<FreeSpinState>().TotalWin;
                var triggerWin = context.state.Get<ExtraState11029>().GetMapPreWin();
                freeSpinWinChipText.SetText(((long) totalWin - (long)triggerWin).GetCommaFormat());
            }
        }
       
    }
}