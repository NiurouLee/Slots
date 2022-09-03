using TMPro;
using UnityEngine;

namespace GameModule
{
    public class JackPotPanel11022:JackPotPanel
    {
        private int usingUniteScaleMashIndex;
        public JackPotPanel11022(Transform inTransform) : base(inTransform)
        {
            usingUniteScaleMashIndex = 3;
        }

        public void UpdateScale()
        {
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                var mashTest = listTextJackpot[i];
                if (i != usingUniteScaleMashIndex)
                {
                    mashTest.characterSize = listTextJackpot[usingUniteScaleMashIndex].characterSize;
                }
            }
        }
        
        public override void Update()
        {
            base.Update();
            
            UpdateScale();
        }
    }
}