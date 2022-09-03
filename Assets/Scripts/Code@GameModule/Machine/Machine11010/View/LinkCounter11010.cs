//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-13 18:08
//  Ver : 1.0.0
//  Description : LinkCounter11010.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class LinkCounter11010:TransformHolder
    {
        private int nLastCounter = -2;
        private int nLeftRespinCount;
        private Animator[] animatorCounter;
        private Animator animatorBg;
        public LinkCounter11010(Transform inTransform)
            :base(inTransform)
        {

        }

        public void InitializeWith(int respinLimit)
        {
            nLastCounter = -1;
            nLeftRespinCount = respinLimit;
            animatorCounter = new Animator[nLeftRespinCount];
            for (int i = 1; i <= nLeftRespinCount; i++)
            {
                var transCount = transform.Find("Count"+i);
                animatorCounter[i-1] = transCount.GetComponent<Animator>();
                animatorBg = transform.Find("BG").GetComponent<Animator>();
            }
        }
        private void InitCounters()
        {
            if (nLastCounter == -2)
            {
                for (int i = 1; i <= nLeftRespinCount; i++)
                {
                    XUtility.PlayAnimation(animatorCounter[i-1], "Idle");
                }
                XUtility.PlayAnimation(animatorBg,"Idle");
            }
        }

        public void UpdateRespinCount(int leftCount)
        {
            InitCounters();
            if (leftCount == nLastCounter + 1)
            {
                return;
            }
            if (nLastCounter >= 0)
            {
                AudioUtil.Instance.PlayAudioFx("Bonus_J01_SpinReduce");
                XUtility.PlayAnimation(animatorCounter[nLastCounter], "Out");
                nLastCounter = -1;
            }
            if (leftCount>0)
            {
                AudioUtil.Instance.PlayAudioFx("Bonus_J01_SpinAdd");
                XUtility.PlayAnimation(animatorCounter[leftCount-1], "In");
                nLastCounter = leftCount - 1;
            }
        }

        public void Close()
        {
            for (int i = 1; i <= nLeftRespinCount; i++)
            {
                XUtility.PlayAnimation(animatorCounter[i-1], "Close");
            }
            XUtility.PlayAnimation(animatorBg,"Close");
        }
    }
}