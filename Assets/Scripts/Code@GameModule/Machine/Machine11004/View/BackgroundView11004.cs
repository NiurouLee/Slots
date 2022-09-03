using UnityEngine;

namespace GameModule
{
    public class BackgroundView11004: TransformHolder
    {
        [ComponentBinder("BGBaseGame")]
        protected Transform tranBase;
        
        [ComponentBinder("BGLinkGame")]
        protected Transform tranLink;
        
        [ComponentBinder("BGFreeGame")]
        protected Transform tranFree;
        
        public BackgroundView11004(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }



        public void CloseAll()
        {
            tranBase.gameObject.SetActive(false);
            tranLink.gameObject.SetActive(false);
            tranFree.gameObject.SetActive(false);
        }


        public void OpenBase()
        {
            CloseAll();
            tranBase.gameObject.SetActive(true);
        }

        public void OpenLink()
        {
            CloseAll();
            tranLink.gameObject.SetActive(true);
        }

        public void OpenFree()
        {
            CloseAll();
            tranFree.gameObject.SetActive(true);
        }
    }
}