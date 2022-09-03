//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 19:03
//  Ver : 1.0.0
//  Description : CollectBonusView11011.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class CollectBonusView11011:TransformHolder
    {
        [ComponentBinder("CollectPar")] 
        private Transform transParticle;
        [ComponentBinder("IntegralText")]
        private TextMesh integralText;
        public CollectBonusView11011(Transform inTransform):base(inTransform)
        {
           ComponentBinder.BindingComponent(this, inTransform);
        }

        public void UpdateCollectChips(long chips)
        {
            if (transParticle)
            {
                transParticle.gameObject.SetActive(false);
                transParticle.gameObject.SetActive(true);
            }
            integralText.text = chips.GetCommaFormat();
        }

        public Vector3 GetEndWorldPos()
        {
            return integralText.transform.position;
        }

        public void ToggleVisible(bool visible)
        {
            integralText.text = "";
            transform.gameObject.SetActive(visible);
        }
    }
}