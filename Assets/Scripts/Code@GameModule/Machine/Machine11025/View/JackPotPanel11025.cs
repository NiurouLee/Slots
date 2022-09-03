using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class JackPotPanel11025:JackPotPanel
    {
        private List<float> initWidth = new List<float>();
        private List<float> nowWidth = new List<float>();
        private List<float> initCharacterSize = new List<float>();
        private static List<float> maxSizeList = new List<float>() {0.07f,0.08f,0.1f,0.12f};
        public JackPotPanel11025(Transform inTransform) : base(inTransform)
        {
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                var tempMesh = listTextJackpot[i];
                var tempWidth = Tools.GetTextWidth(tempMesh);
                initWidth.Add(tempWidth);
                nowWidth.Add(tempWidth);
                var tempSize = tempMesh.characterSize;
                initCharacterSize.Add(tempSize);
                if (maxSizeList[i] < tempSize)
                {
                    tempSize = maxSizeList[i];
                }
                tempMesh.characterSize = tempSize;
            }
        }
        public void UpdateScale()
        {
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                var tempSize =  initWidth[i]/ nowWidth[i] * initCharacterSize[i];
                if (maxSizeList[i] < tempSize)
                {
                    tempSize = maxSizeList[i];
                }
                listTextJackpot[i].characterSize = tempSize;
            }
        }
        
        public override void Update()
        {
            base.Update();
            
            // UpdateScale();
        }
        public override void UpdateJackpotValue()
        {
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                ulong numJackpot = jackpotInfoState.GetJackpotValue((uint)(i+1));
                //listTextJackpot[i].text = numJackpot.GetCommaFormat();
                if (listTextJackpot[i])
                {
                    listTextJackpot[i].SetText(numJackpot.GetCommaFormat());
                    nowWidth[i] = Tools.GetTextWidth(listTextJackpot[i]);
                    UpdateScale();
                }
            }
        }
        
    }
}