// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/23/18:19
// Ver : 1.0.0
// Description : MiniGameFinishPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using System.Threading.Tasks;


namespace GameModule
{
    public class LuckyPotLastChangePopup11031 : MachinePopUp
    {
        public LuckyPotLastChangePopup11031(Transform transform)
            : base(transform)
        {
            AutoClose();
        }

        public virtual async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            Close();
        }
    }
}