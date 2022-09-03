using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class UIVIPLevelDetialItem
    {
        protected List<Transform> listNormal = new List<Transform>();
        protected List<Transform> listNext = new List<Transform>();
        protected List<Transform> listCurrent = new List<Transform>();
        
        public UIVIPLevelDetialItem(Transform transform)
        {
            foreach (Transform child in transform)
            {
                var tranNormal = child.Find("NormalState");
                if (tranNormal != null)
                {
                    listNormal.Add(tranNormal);
                    listNext.Add(child.Find("NextState"));
                    listCurrent.Add(child.Find("CurrenctState"));
                }
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