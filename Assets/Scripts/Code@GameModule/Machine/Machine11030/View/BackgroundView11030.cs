// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/30/21:26
// Ver : 1.0.0
// Description : BackgroundView11003.cs
// ChangeLog :
// **********************************************
using UnityEngine;

namespace GameModule
{
    public class BackgroundView11030:TransformHolder
    {
        [ComponentBinder("BGBaseGame")]
        protected Transform baseBgTransform;
        
        [ComponentBinder("BGFreeGame")]
        protected Transform freeBgTransform;
        
        [ComponentBinder("BGTrain")]
        protected Transform bonusBGTransForm;
        
        public BackgroundView11030(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        public void CloseAll()
        {
            baseBgTransform?.gameObject.SetActive(false);
            freeBgTransform?.gameObject.SetActive(false);
            bonusBGTransForm?.gameObject.SetActive(false);
        }


        public void OpenBase()
        {
            CloseAll();
            baseBgTransform?.gameObject.SetActive(true);
        }

        public void OpenFree()
        {
            CloseAll();
            freeBgTransform?.gameObject.SetActive(true);
        }
        
        public void OpenBonus()
        {
            CloseAll();
            bonusBGTransForm?.gameObject.SetActive(true);
        }
    }
}