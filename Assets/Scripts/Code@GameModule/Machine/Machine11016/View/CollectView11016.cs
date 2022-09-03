//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-05 16:23
//  Ver : 1.0.0
//  Description : CollectView.cs
//  ChangeLog :
//  **********************************************

using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class CollectView11016:TransformHolder
    {
        private List<List<Transform>> listMileStone;
        private List<Animator> listMileStoneAnimator;
        public CollectView11016(Transform inTransform):base(inTransform)
        {
            listMileStone = new List<List<Transform>>();
            listMileStoneAnimator = new List<Animator>();
            ComponentBinder.BindingComponent(this, inTransform);

            var listCount = new[] {4, 4, 9};
            var state = new[] {"Enable", "Disable"};
            var suffix = new[] {"MegaSpins2Group", "MegaSpins1Group", "SuperSpinsGroup"};
            for (int i = 0; i < suffix.Length; i++)
            {
                List<Transform> listState;
                for (int j = 0; j < listCount[i]; j++)
                {
                    listMileStone.Add(null);
                    listMileStoneAnimator.Add(transform.Find($"BG/{suffix[i]}/CellGroup/Cell{j+1}/Cell").GetComponent<Animator>());
                }
                listState = new List<Transform>();
                for (int k = 0; k < state.Length; k++)
                {
                    var transState = transform.Find($"BG/{suffix[i]}/Title/SuperSpins{state[k]}");
                    listState.Add(transState);
                }
                listMileStoneAnimator.Add(null);
                listMileStone.Add(listState);
            }

            UpdateFreeCount(0);
        }

        public void UpdateFreeCount(int count, bool anim=false)
        {
            for (int i = 0; i < listMileStone.Count; i++)
            {
                if (i<count)
                {
                    if (listMileStone[i] == null)
                    {
                        XUtility.PlayAnimation(listMileStoneAnimator[i],"Cell1_Enable");
                        if (i==count-1 && anim)
                        {
                            XUtility.PlayAnimation(listMileStoneAnimator[i],"Cell1_Open");   
                        }
                    }
                    else
                    {
                        listMileStone[i][0].transform.gameObject.SetActive(true);
                        listMileStone[i][1].transform.gameObject.SetActive(false);   
                    }
                }
                else
                {
                    if (listMileStone[i] == null)
                    {
                        XUtility.PlayAnimation(listMileStoneAnimator[i],"Cell1_Disable");
                    }
                    else
                    {
                        listMileStone[i][0].transform.gameObject.SetActive(false);
                        listMileStone[i][1].transform.gameObject.SetActive(true);   
                    }
                }
            }
        }

        public Vector3 GetEndPos(int count)
        {
            if (listMileStone[count-1] != null)
            {
                return listMileStone[count - 1][0].transform.position;   
            }
            return listMileStoneAnimator[count - 1].transform.position;
        }
    }
}