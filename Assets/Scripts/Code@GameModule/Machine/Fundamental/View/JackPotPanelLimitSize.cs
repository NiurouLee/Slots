using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameModule
{

    public class JackPotPanelLimitSize: JackPotPanel
    {
        private List<float> maxSize = new List<float>();
        private List<float> maxWidth = new List<float>();
        public JackPotPanelLimitSize(Transform inTransform) : base(inTransform)
        {
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                var tempMesh = listTextJackpot[i];
                var tempWidth = Tools.GetTextWidth(tempMesh);
                maxWidth.Add(tempWidth);
                var tempSize = tempMesh.characterSize;
                maxSize.Add(tempSize);
            }
        }
        public void UpdateScale()
        {
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                var tempMesh = listTextJackpot[i];
                var tempWidth = Tools.GetTextWidth(tempMesh);
                var tempMaxWidth = maxWidth[i];
                var widthScale = tempWidth / tempMaxWidth;
                var tempSize = tempMesh.characterSize;
                var tempMaxSize = maxSize[i];
                var sizeScale = tempSize / tempMaxSize;
                var biggerScale = Math.Max(widthScale, sizeScale);
                var tempAdditionScale = 1 / biggerScale;
                tempMesh.characterSize *= tempAdditionScale;
            }
        }
        public override void UpdateJackpotValue()
        {
            base.UpdateJackpotValue();
            UpdateScale();
        }
    }
}
