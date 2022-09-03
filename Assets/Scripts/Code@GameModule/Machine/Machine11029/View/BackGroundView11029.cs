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
    public class BackGroundView11029 : TransformHolder
    {
        [ComponentBinder("BGBaseGame")] protected Transform normalBackground;

        [ComponentBinder("BGFreeGame")]
        protected Transform freeBackground;
        
        [ComponentBinder("BGBonusGame")]
        protected Transform bonusBackground;

        [ComponentBinder("MiniGame")] protected Transform miniBackGame;

        [ComponentBinder("BGMapGame")] protected Transform mapBackGame;

        public BackGroundView11029(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void ShowFreeBackground(bool showFree, bool showMap, bool showMini)
        {
            if (showMini)
            {
                normalBackground.transform.gameObject.SetActive(false);
                freeBackground.transform.gameObject.SetActive(false);
                miniBackGame.transform.gameObject.SetActive(true);
                mapBackGame.transform.gameObject.SetActive(false);
                bonusBackground.transform.gameObject.SetActive(false);
            }
            else if (showFree)
            {
                if (context.state.Get<FreeSpinState>().freeSpinId == 1)
                {
                    freeBackground.transform.gameObject.SetActive(false);
                    bonusBackground.transform.gameObject.SetActive(true);
                }
                else
                {
                    freeBackground.transform.gameObject.SetActive(true);
                    bonusBackground.transform.gameObject.SetActive(false);
                }
                normalBackground.transform.gameObject.SetActive(false);
                miniBackGame.transform.gameObject.SetActive(false);
                mapBackGame.transform.gameObject.SetActive(false);
                
            }
            else if (showMap)
            {
                normalBackground.transform.gameObject.SetActive(false);
                freeBackground.transform.gameObject.SetActive(false);
                miniBackGame.transform.gameObject.SetActive(false);
                mapBackGame.transform.gameObject.SetActive(true);
                bonusBackground.transform.gameObject.SetActive(false);
            }
            else
            {
                normalBackground.transform.gameObject.SetActive(true);
                freeBackground.transform.gameObject.SetActive(false);
                miniBackGame.transform.gameObject.SetActive(false);
                mapBackGame.transform.gameObject.SetActive(false); 
                bonusBackground.transform.gameObject.SetActive(false);
            }
        }
    }
}