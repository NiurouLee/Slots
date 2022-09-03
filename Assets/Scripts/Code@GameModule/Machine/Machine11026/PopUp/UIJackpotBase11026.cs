using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIJackpotBase11026 : UIJackpotBase
    {
        [ComponentBinder("Root/TopGroup/GrandImage/x2")]
        protected Transform _imageMultiplier2;

        [ComponentBinder("Root/TopGroup/GrandImage/x3")]
        protected Transform _imageMultiplier3;

        [ComponentBinder("Root/TopGroup/GrandImage/x4")]
        protected Transform _imageMultiplier4;

        public UIJackpotBase11026(Transform inTransform) : base(inTransform)
        {
            
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var _extraState = context.state.Get<ExtraState11026>();
            uint multiplier = _extraState.GetAllWinMultiplier();
            if (multiplier == 2)
            {
                _imageMultiplier2.gameObject.SetActive(true);
                _imageMultiplier3.gameObject.SetActive(false);
                _imageMultiplier4.gameObject.SetActive(false);
            } else if (multiplier == 3)
            {
                _imageMultiplier2.gameObject.SetActive(false);
                _imageMultiplier3.gameObject.SetActive(true);
                _imageMultiplier4.gameObject.SetActive(false);
            } else if (multiplier == 4)
            {
                _imageMultiplier2.gameObject.SetActive(false);
                _imageMultiplier3.gameObject.SetActive(false);
                _imageMultiplier4.gameObject.SetActive(true);
            }
            else
            {
                _imageMultiplier2.gameObject.SetActive(false);
                _imageMultiplier3.gameObject.SetActive(false);
                _imageMultiplier4.gameObject.SetActive(false);
            }
        }
    }
}
