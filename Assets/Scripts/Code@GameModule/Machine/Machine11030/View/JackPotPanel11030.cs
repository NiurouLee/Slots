using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class JackPotPanel11030:JackPotPanel
    {
        private List<float> initWidth = new List<float>();
        private List<float> nowWidth = new List<float>();
        private List<float> initCharacterSize = new List<float>();
        private static float maxSize = 0.055f;
        public JackPotPanel11030(Transform inTransform) : base(inTransform)
        {
            float nowLeastSize = 100;
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                var tempMesh = listTextJackpot[i];
                var tempWidth = Tools.GetTextWidth(tempMesh);
                initWidth.Add(tempWidth);
                nowWidth.Add(tempWidth);
                var tempSize = tempMesh.characterSize;
                initCharacterSize.Add(tempSize);
                if (nowLeastSize > tempSize)
                {
                    nowLeastSize = tempSize;
                }
            }

            if (maxSize < nowLeastSize)
                nowLeastSize = maxSize;
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                var tempMesh = listTextJackpot[i];
                tempMesh.characterSize = nowLeastSize;
            }
        }
        public void UpdateScale()
        {
            float nowLeastSize = 100;
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                var tempSize =  initWidth[i]/ nowWidth[i] * initCharacterSize[i];
                if (nowLeastSize > tempSize)
                {
                    nowLeastSize = tempSize;
                }
            }
            if (maxSize < nowLeastSize)
                nowLeastSize = maxSize;
            for (int i = 0; i < listTextJackpot.Count; i++)
            {
                listTextJackpot[i].characterSize = nowLeastSize;
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