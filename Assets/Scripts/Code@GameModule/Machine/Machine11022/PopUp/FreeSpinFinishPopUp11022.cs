using UnityEngine;

namespace GameModule
{
    public class FreeSpinFinishPopUp11022:FreeSpinFinishPopUp
    {
        public FreeSpinFinishPopUp11022(Transform transform)
            :base(transform)
        {
            if (Constant11022.debugType)
            {
                context.WaitSeconds(0.5f, () =>
                {
                    collectButton.onClick.Invoke();
                });
            }
        }
    }
}