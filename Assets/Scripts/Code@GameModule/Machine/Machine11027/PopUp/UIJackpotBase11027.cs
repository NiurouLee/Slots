using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIJackpotBase11027 : UIJackpotBase
    {
        private bool isSettle = false;

        public UIJackpotBase11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);
            animator = transform.GetComponent<Animator>();
            
            if (btnCollect)
            {
                btnCollect.onClick.AddListener(CollectClick); 
            }
            else
            {
                AutoClose();
            }
        }
        
        private void CollectClick()
        {
            if (!isSettle)
            {
                isSettle = true;
                AudioUtil.Instance.PlayAudioFx("Close");
                Close();
            }
        }
    }
}
