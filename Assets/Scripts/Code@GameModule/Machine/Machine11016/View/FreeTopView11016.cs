//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-05 16:22
//  Ver : 1.0.0
//  Description : FreeTopView.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class FreeTopView11016:TransformHolder
    {
        [ComponentBinder("CollectSymbolGroup")]
        private Transform transMax;
        [ComponentBinder("PlayGroup")]
        private Transform transCollect;

        [ComponentBinder("PlayGroup/CountText1")]
        private TextMesh txtPanelCount;
        [ComponentBinder("PlayGroup/CountText2")]
        private TextMesh txtCollectCount;
        [ComponentBinder("PlayGroup/SymbolSprite")]
        private Transform transHeart;

        private Animator _animator;
        public FreeTopView11016(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            _animator = transform.GetComponent<Animator>();
        }

        public void UpdateCollectCount(int needBombCount, int panelCount, bool anim=false)
        {
            if (panelCount == 0)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            if (anim)
            {
                XUtility.PlayAnimation(_animator,"Blink");
            }
            txtPanelCount.SetText(panelCount.ToString());
            txtCollectCount.SetText(needBombCount.ToString());
        }

        public void UpdateCollectCountToZero()
        {
            txtCollectCount.SetText("0");     
        }

        public Vector3 GetEndPos()
        {
            return transHeart.transform.position;
        }
    }
}