using System;
using System.Collections.Generic;

namespace GameModule
{
    public class RollSingleWheelUpdater11022:RollSimpleUpdater
    {
        private List<float> paramList = new List<float>() {1,-2,-7,-7,-3};
        protected override float GetSlowStateByProcess(float process)
        {
            var x = process;
            // var y = -3 * x * x - 2 * x + 1;
            float y = 0;
            for (int i = 0; i < paramList.Count; i++)
            {
                y += paramList[i] * (float) Math.Pow(x, i);
            }
            // var y = (-3) * (float)Math.Pow(x, 4) + (-7) * (float)Math.Pow(x, 3)+ (-7) * (float)Math.Pow(x, 2) + (-2) * x + 1;
            return y;
        }
    }
}