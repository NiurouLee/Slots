using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule{
    public class ReSpinStartPopUp11312 : ReSpinStartPopUp
    {
        [ComponentBinder("Root/MainGroup")]
        protected Transform MainGroup;
        public ReSpinStartPopUp11312(Transform transform) : base(transform)
        {
        }
        public void Init(bool isBlue){
            for (int i = 1; i <= 2; i++)
            {
                MainGroup.transform.Find("Content"+i).gameObject.SetActive(false);
            }
            var num = isBlue?2:1;
            MainGroup.transform.Find("Content"+num).gameObject.SetActive(true);
        }
    }
}

