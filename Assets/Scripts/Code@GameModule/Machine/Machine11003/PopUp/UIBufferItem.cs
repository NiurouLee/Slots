using UnityEngine;

namespace GameModule
{
    public class UIBufferItem
    {
        private Transform tranEnable;
        private Transform tranDisable;
        private Transform root;
        public UIBufferItem(Transform tranRoot)
        {
            root = tranRoot;
            tranEnable = root.Find("Enable");
            tranDisable = root.Find("InactiveMask");
            
            tranEnable.gameObject.SetActive(false);
            tranDisable.gameObject.SetActive(false);
        }

        public void SetEnable(bool isEnable)
        {
            if (isEnable)
            {
                tranEnable.gameObject.SetActive(true);
                tranDisable.gameObject.SetActive(false);
            }
            else
            {
                tranEnable.gameObject.SetActive(true);
                tranDisable.gameObject.SetActive(true);
            }
        }
    }
}