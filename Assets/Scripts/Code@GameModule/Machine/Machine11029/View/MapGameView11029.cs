//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-10 14:57
//  Ver : 1.0.0
//  Description : WheelView11028.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameModule
{
    public class MapGameView11029 : TransformHolder
    {
        [ComponentBinder("Root")] private Transform transRoot;

        [ComponentBinder("Root/BottomGroup/BackButton")]
        public Button backGameBtn;

        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup/PointGroup")]
        public Transform levelGroups;

        private Animator _animator;

        private ExtraState11029 _extraState11029;

        private List<Transform> levelGroupList;

        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup/PointGroup/PlayerIcon")]
        public Transform horseIcon;

        public MapGameView11029(Transform transform) :
            base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            backGameBtn.onClick.AddListener(BackToGame);
            _animator = transform.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
            var childCount = 35;
            levelGroupList = new List<Transform>(childCount);
            for (var i = 0; i < childCount; i++)
            {
                var pot = levelGroups.transform.GetChild(i);
                levelGroupList.Add(pot);
            }
        }

        private async void BackToGame()
        {
            await Close();
        }
        
        public virtual async Task Close()
        {
            await XUtility.PlayAnimationAsync(_animator, "Close");
        }

        public virtual async Task Open()
        {
            await XUtility.PlayAnimationAsync(_animator, "Open");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            _extraState11029 = context.state.Get<ExtraState11029>();
            int level = (int) _extraState11029.GetMapLevel();
            horseIcon.transform.position = new Vector3(levelGroupList[level].position.x,
                levelGroupList[level].position.y+76, levelGroupList[level].position.z);
            for (int i = 0; i < levelGroupList.Count; i++)
            {
                if (i < level)
                {
                    if (levelGroupList[i].Find("PointIcon") != null)
                    {
                        levelGroupList[i].Find("PointIcon").Find("DisableState").gameObject.SetActive(true);
                        levelGroupList[i].Find("PointIcon").Find("EnableState").gameObject.SetActive(false);
                    }

                    if (levelGroupList[i].Find("BoxIcon") != null)
                    {
                        levelGroupList[i].Find("BoxIcon").Find("BoxGroup").gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (levelGroupList[i].Find("PointIcon") != null)
                    {
                        levelGroupList[i].Find("PointIcon").Find("DisableState").gameObject.SetActive(false);
                        levelGroupList[i].Find("PointIcon").Find("EnableState").gameObject.SetActive(true);
                    }

                    if (levelGroupList[i].Find("BoxIcon") != null)
                    {
                        levelGroupList[i].Find("BoxIcon").Find("BoxGroup").gameObject.SetActive(true);
                    }
                }
            }
        }

        public async Task ShowMapGame(bool animation = false)
        {
            _extraState11029 = context.state.Get<ExtraState11029>();
            int level = (int) _extraState11029.GetMapLevel();
            int currentIndex = level - 1;
              horseIcon.transform.position = new Vector3(levelGroupList[currentIndex].position.x,
                levelGroupList[currentIndex].position.y+76, levelGroupList[currentIndex].position.z);
            
            for (int i = 0; i < levelGroupList.Count; i++)
            {
                if (i < currentIndex)
                {
                    if (levelGroupList[i].Find("PointIcon") != null)
                    {
                        levelGroupList[i].Find("PointIcon").Find("DisableState").gameObject.SetActive(true);
                        levelGroupList[i].Find("PointIcon").Find("EnableState").gameObject.SetActive(false);
                    }

                    if (levelGroupList[i].Find("BoxIcon") != null)
                    {
                        levelGroupList[i].Find("BoxIcon").Find("BoxGroup").gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (levelGroupList[i].Find("PointIcon") != null)
                    {
                        levelGroupList[i].Find("PointIcon").Find("DisableState").gameObject.SetActive(false);
                        levelGroupList[i].Find("PointIcon").Find("EnableState").gameObject.SetActive(true);
                    }

                    if (levelGroupList[i].Find("BoxIcon") != null)
                    {
                        levelGroupList[i].Find("BoxIcon").Find("BoxGroup").gameObject.SetActive(true);
                    }
                }
            }

            //horse开始跳
            if (levelGroupList[currentIndex].Find("Fx_light") != null)
            {
                levelGroupList[currentIndex].Find("Fx_light").gameObject.SetActive(true);
            }

            //horseIcon.transform.gameObject.GetComponent<Animator>().Play("Jump");

            Vector3 startNode = horseIcon.localPosition;
            Vector3 endNode = new Vector3(levelGroupList[level].localPosition.x,levelGroupList[level].localPosition.y+76,levelGroupList[level].localPosition.z);
            Debug.Log("endNode:"+endNode);
            Debug.Log("levelGroupList:"+levelGroupList[level].localPosition);
            XUtility.FlyLocal(horseIcon, startNode, endNode, 0, 0.5f);
            await context.WaitSeconds(0.5f);
            if (levelGroupList[currentIndex].Find("Fx_light") != null)
            {
                levelGroupList[currentIndex].Find("Fx_light").gameObject.SetActive(false);
            }

            if (levelGroupList[currentIndex].Find("BoxIcon") != null)
            {
                levelGroupList[currentIndex].Find("BoxIcon").Find("BoxGroup").gameObject.SetActive(false);
            }

            if (levelGroupList[currentIndex].Find("PointIcon") != null)
            {
                levelGroupList[currentIndex].Find("PointIcon").Find("DisableState").gameObject.SetActive(true);
                levelGroupList[currentIndex].Find("PointIcon").Find("EnableState").gameObject.SetActive(false);
            }

            if (levelGroupList[level].Find("Fx_light") != null)
            {
                levelGroupList[level].Find("Fx_light").gameObject.SetActive(true);
            }

            await context.WaitSeconds(0.5f);
        }

        public void EnableButton(bool enable = true)
        {
            backGameBtn.transform.gameObject.SetActive(enable);
        }
    }
}