// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/29/20:28
// Ver : 1.0.0
// Description : FreeSpinRetriggerPopUp.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinReTriggerPopUp:MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/CountText")]
        protected Text _extraCountText;
        
        public FreeSpinReTriggerPopUp(Transform transform) 
            :base(transform)
        {
            
        }
        
        public virtual async void SetExtraCount(uint extraCount)
        {
            if(_extraCountText)
                _extraCountText.text = extraCount.ToString();
            
            if (XUtility.IsAnimationExist(animator, "Retrigger"))
            {
                await XUtility.PlayAnimationAsync(animator,"Retrigger");
            }
            else
            {
                await XUtility.WaitSeconds(3.0f,context);
            }
            
            Close();
        }

        public override void OnOpen()
        {
            if (context.assetProvider.GetAsset<AudioClip>("FreeGameRetrigger_Open"))
            {
                AudioUtil.Instance.PlayAudioFx("FreeGameRetrigger_Open");   
            }
            base.OnOpen();
        }
    }
}