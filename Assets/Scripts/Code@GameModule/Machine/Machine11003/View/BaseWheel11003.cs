// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/07/16:09
// Ver : 1.0.0
// Description : BaseWheel11003.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class BaseWheel11003:Wheel
    {
        public BaseWheel11003(Transform transform) : base(transform)
        {
          
        }
        
        public void HideSymbolContent()
        {
            transform.Find("Rolls").gameObject.SetActive(false);

            var winFrameRoot = transform.Find("WinFrameRoot");
            if(winFrameRoot)
                winFrameRoot.gameObject.SetActive(false);
        }
        
        public void ShowSymbolContent()
        {
            transform.Find("Rolls").gameObject.SetActive(true);

            var winFrameRoot = transform.Find("WinFrameRoot");
            if(winFrameRoot)
                winFrameRoot.gameObject.SetActive(true);
        }

    }
}