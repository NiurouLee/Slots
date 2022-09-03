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
    public class BackGroundView11031 : TransformHolder
    {
        [ComponentBinder("BGBaseGame")] protected Transform baseBackGround;

        [ComponentBinder("BGBaseGame3")] protected Transform linkBackGround;

        [ComponentBinder("BGGroup/GainBodyBG2")]
        protected Transform linkLine1;
        
        [ComponentBinder("BGGroup/GainBodyBG3")]
        protected Transform linkLine2;


        public BackGroundView11031(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void ShowBackground(bool showLink)
        {
            if (showLink)
            {
                linkBackGround.gameObject.SetActive(true);
                baseBackGround.gameObject.SetActive(false);
            }
            else
            {
                linkBackGround.gameObject.SetActive(false);
                baseBackGround.gameObject.SetActive(true);
            }
        }

        public void ShowLinkLine(bool showLine)
        {
            if (showLine)
            {
                linkLine1.gameObject.SetActive(true);
                linkLine2.gameObject.SetActive(true);
            }
            else
            {
                linkLine1.gameObject.SetActive(false);
                linkLine2.gameObject.SetActive(false);
            }
        }
    }
}