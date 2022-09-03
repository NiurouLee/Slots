using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace GameModule{
    public class UIJackpotBase11301 : UIJackpotBase
    {
        public UIJackpotBase11301(Transform inTransform) : base(inTransform)
        {
        }
        public override async Task AutoClose()
        {
            await XUtility.PlayAnimationAsync(animator, "Open");
            await XUtility.WaitSeconds(2.2f);
            Close();
        }
    }
}

