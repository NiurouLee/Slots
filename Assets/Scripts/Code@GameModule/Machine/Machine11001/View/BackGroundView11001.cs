// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/01/14:17
// Ver : 1.0.0
// Description : BackGroundView11001.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class BackGroundView11001:TransformHolder
    {
        [ComponentBinder("SceneBackGroundNormal")]
        protected Transform normalBackground;
        
        [ComponentBinder("SceneBackGroundFreeGame")]
        protected Transform freeBackground;

        [ComponentBinder("ScreenFitterObject")]
        protected Transform logo;
        
        public BackGroundView11001(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            
        }
        
        public void ShowFreeBackground(bool showFree)
        {
            freeBackground.gameObject.SetActive(showFree);
            normalBackground.gameObject.SetActive(!showFree);
        }

        public void HideLogo()
        {
            if (logo != null)
            {
                logo.gameObject.SetActive(false);
            }
        }

        public void SetLogoYPosition(float posY)
        {
            if (logo != null)
            {
                logo.transform.localPosition = new Vector3(0, posY, 0);
            }
        }
    }
}