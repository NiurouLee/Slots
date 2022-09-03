// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/11/20:06
// Ver : 1.0.0
// Description : FreeSpinReTriggerPopUp11003.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class FreeSpinReTriggerPopUp11003:FreeSpinReTriggerPopUp
    {
        [ComponentBinder("FreeGames")] 
        private Transform _freeGamesTransform;
        [ComponentBinder("FreeGame")]
        private Transform _freeGameTransform;
        
        public FreeSpinReTriggerPopUp11003(Transform transform)
            :base(transform)
        {
            
        }       
        
        public override async void SetExtraCount(uint extraCount)
        {
            if(_extraCountText)
                _extraCountText.text = extraCount.ToString();
            
            _freeGamesTransform.gameObject.SetActive(extraCount > 1);
            _freeGameTransform.gameObject.SetActive(extraCount == 1);
            
            await XUtility.PlayAnimationAsync(animator,"Retrigger");
            
            Close();
        }
    }
}