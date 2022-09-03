using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class UIVIPLevelDetialItemTitle
    {
        protected List<Transform> listNormal = new List<Transform>();
        protected List<Transform> listNext = new List<Transform>();
        protected List<Transform> listCurrent = new List<Transform>();
        
        public UIVIPLevelDetialItemTitle(Transform transform)
        {
           
                var tranNormal = transform.Find("NormalState");
                if (tranNormal != null)
                {
                    listNormal.Add(tranNormal);
                    listNext.Add(transform.Find("NextState"));
                    listCurrent.Add(transform.Find("CurrentLevelState"));
                }
            
        }



        public void SetNormal()
        {
            foreach (Transform child in listNormal)
            {
                child.gameObject.SetActive(true);
            }

            foreach (Transform child in listNext)
            {
                child.gameObject.SetActive(false);
            }

            foreach (Transform child in listCurrent)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        
        public void SetNext()
        {
            foreach (Transform child in listNormal)
            {
                child.gameObject.SetActive(false);
            }

            foreach (Transform child in listNext)
            {
                child.gameObject.SetActive(true);
            }

            foreach (Transform child in listCurrent)
            {
                child.gameObject.SetActive(false);
            }
        }
        
        
        public void SetCurrent()
        {
            foreach (Transform child in listNormal)
            {
                child.gameObject.SetActive(false);
            }

            foreach (Transform child in listNext)
            {
                child.gameObject.SetActive(false);
            }

            foreach (Transform child in listCurrent)
            {
                child.gameObject.SetActive(true);
            }
        }
    }
}