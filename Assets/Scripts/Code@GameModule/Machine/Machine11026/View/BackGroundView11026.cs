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
    public class BackGroundView11026:TransformHolder
    {
        [ComponentBinder("Bg3")]
        protected Transform normalBackground;
        
        [ComponentBinder("Bg2")]
        protected Transform freeBackground;

        [ComponentBinder("Bg1")]
        protected Transform linkBackground;
        
        public BackGroundView11026(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            
        }
        
        public void ShowBackground(bool showFree,bool showLink)
        {
            if (showLink)
            { 
                linkBackground.gameObject.SetActive(true);
                freeBackground.gameObject.SetActive(false); 
                normalBackground.gameObject.SetActive(false);
            }else if (showFree)
            {
                linkBackground.gameObject.SetActive(false);
                freeBackground.gameObject.SetActive(true); 
                normalBackground.gameObject.SetActive(false);
            }
            else
            {
                linkBackground.gameObject.SetActive(true);
                freeBackground.gameObject.SetActive(false); 
                normalBackground.gameObject.SetActive(false);
            }
        }
        public void ShowLinkBackGround()
        {
            linkBackground.gameObject.SetActive(true);
            freeBackground.gameObject.SetActive(false); 
            normalBackground.gameObject.SetActive(false);
        }
        
        public void ShowFreeBackGround()
        {
            linkBackground.gameObject.SetActive(false);
            freeBackground.gameObject.SetActive(true); 
            normalBackground.gameObject.SetActive(false);
        }
        
        public void ShowBaseBackFround()
        {
            linkBackground.gameObject.SetActive(true);
            freeBackground.gameObject.SetActive(false); 
            normalBackground.gameObject.SetActive(false);
        }
    }
}