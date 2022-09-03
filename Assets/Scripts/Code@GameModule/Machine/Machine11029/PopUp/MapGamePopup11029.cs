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
using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class MapGamePopup11029 : MachinePopUp
    {
        [ComponentBinder("Root")] 
        private Transform transRoot;
       
        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup")] 
        private RectTransform levelGroup;
        
        [ComponentBinder("Root/MapScrollView/Viewport")] 
        private RectTransform baseGroup;
        
        [ComponentBinder("Root/MapScrollView/Viewport/Content")] 
        private RectTransform contentGroup;

        [ComponentBinder("Root/BottomGroup/BackButton")]
        public Button backGameBtn;

        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup/PointGroup")]
        public Transform levelGroups;

        private Animator _animator;

        private ExtraState11029 _extraState11029;

        private List<Transform> levelGroupList;

        private float levelHeight;

        private float baseHeight;

        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup/PointGroup/PlayerIconBg")]
        public Transform horseIcon;
        
        [ComponentBinder("Root/MapScrollView/Viewport/Content/LevelGroup/PointGroup/PlayerIconBg/PlayerIcon")]
        public Transform horseBg;

        public MapGamePopup11029(Transform transform) :
            base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            backGameBtn.onClick.AddListener(BackToGame);
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
            AudioUtil.Instance.PlayAudioFxOneShot("Close");
            var scrollRect = contentGroup.parent.parent.GetComponent<ScrollRect>();
            scrollRect.enabled = false;
            backGameBtn.transform.DOScale(Vector3.zero, 0.2f);
            await context.WaitSeconds(0.2f);
            var target = baseHeight;
            var rectTransform = contentGroup.GetComponent<RectTransform>();
            rectTransform.DOAnchorPosY(target, 0.5f).SetEase(Ease.InSine);
            await context.WaitSeconds(0.5f);
            Close();
        }

        public override bool EnableAutoAdapt()
        {
            return false;
        }

        private float backHeight;
        public override async void OnOpen()
        {
            levelHeight = levelGroup.rect.height;
            baseHeight = baseGroup.rect.height;
            Debug.LogError("levelHeight = " + levelHeight);
            Debug.LogError("baseHeight = " + baseHeight);
            _extraState11029 = context.state.Get<ExtraState11029>();
            var scrollRect = contentGroup.parent.parent.GetComponent<ScrollRect>();
            scrollRect.enabled = false;
            var position = contentGroup.localPosition;
            position.y = baseHeight * 1.3f;
            backHeight = position.y;
            contentGroup.localPosition = position;
            int level = (int) _extraState11029.GetMapLevel();

            if(level > 0)
                level = (level - 1) / 5 * 5;
            
            // if (level <= 5)
            // {
            //     level = 0;
            // }
            // else if (level <= 10)
            // {
            //     level = 5;
            // }
            // else if (level <= 15)
            // {
            //     level = 10;
            // }
            // else if (level <= 20)
            // {
            //     level = 15;
            // }
            // else if (level <= 25)
            // {
            //     level = 20;
            // }
            // else
            // {
            //     level = 25;
            // }
            var firstPos = levelGroupList[0].localPosition.y;
            var endPos = levelGroupList[level].localPosition.y;
            var delta = endPos - firstPos;
            var max = levelHeight - baseHeight;
            delta = Mathf.Clamp(delta, 0, max);
            Debug.LogError($"delta = {delta}");
            Debug.LogError($"contentGroup local pos = {contentGroup.transform.localPosition}");
            var rectTransform = contentGroup.GetComponent<RectTransform>();
            rectTransform.DOAnchorPosY(-delta, 0.6f).OnComplete(() =>
            {
                scrollRect.enabled = true;
                Debug.LogError($"11backHeight = {backHeight}");
                Debug.LogError($"11contentGroup local pos = {contentGroup.transform.localPosition}");
            });
        }
        
        // public override async Task OnClose()
        // {
        //     var scrollRect = contentGroup.parent.parent.GetComponent<ScrollRect>();
        //     scrollRect.enabled = false;
        //  
        //     var target = baseHeight * 1.3f;
        //     var rectTransform = contentGroup.GetComponent<RectTransform>();
        //     scrollRect.enabled = false;
        //     rectTransform.DOAnchorPosY(target, 3.0f).OnComplete(() =>
        //     {
        //         scrollRect.enabled = false;
        //     });
        // }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            levelHeight = levelGroup.rect.height;
            baseHeight = baseGroup.rect.height;
        }

        public void ShowRightArea()
        {
            _extraState11029 = context.state.Get<ExtraState11029>();
            int level = (int) _extraState11029.GetMapLevel();
            var firstPos = levelGroupList[0].localPosition.y;
            var endPos = levelGroupList[level].localPosition.y;
            var delta = endPos - firstPos;
            var newContentGroupPosition = contentGroup.localPosition;
            newContentGroupPosition.y = -delta;
            contentGroup.localPosition = newContentGroupPosition;
        }

        public void ShowLevel(bool active = false)
        {
            _extraState11029 = context.state.Get<ExtraState11029>();
            int level = (int) _extraState11029.GetMapLevel();
            if (active == true)
            {
                if (level > 0)
                {
                    level = (int) _extraState11029.GetMapLevel() - 1;
                }
            }
            horseIcon.transform.localPosition = new Vector3(levelGroupList[level].localPosition.x,
                levelGroupList[level].localPosition.y+20, levelGroupList[level].localPosition.z);
            for (int i = 0; i < levelGroupList.Count; i++)
            {
                if (i <= level)
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
            horseIcon.transform.localPosition = new Vector3(levelGroupList[currentIndex].localPosition.x,
                levelGroupList[currentIndex].localPosition.y+20, levelGroupList[currentIndex].localPosition.z);

            for (int i = 0; i < levelGroupList.Count; i++)
            {
                if (i <= currentIndex)
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
            Vector3 startNode = horseIcon.localPosition;
            Vector3 endNode = new Vector3(levelGroupList[level].localPosition.x,
                levelGroupList[level].localPosition.y + 20, levelGroupList[level].localPosition.z);
            Debug.Log("endNode:" + endNode);
            Debug.Log("levelGroupList:" + levelGroupList[level].localPosition);
            AudioUtil.Instance.PlayAudioFx("Map_PegasusFly");
            context.WaitSeconds(0.2f, () =>
            {
                horseIcon.DOLocalMove(endNode, 0.4f);
            });
            await XUtility.PlayAnimationAsync(horseIcon.Find("PlayerIcon").transform.gameObject.GetComponent<Animator>(), "Jump", context);
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
                if (!levelGroupList[level].Find("Fx_light").GetComponent<SortingGroup>())
                {
                    var sortingGroup = levelGroupList[level].Find("Fx_light").gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("MachinePopup");
                    sortingGroup.sortingOrder = 0;
                }
            }

            if (levelGroupList[level].Find("UIParticle") != null)
            {
                levelGroupList[level].Find("UIParticle").gameObject.SetActive(true);
                if (!levelGroupList[level].Find("UIParticle").GetComponent<SortingGroup>())
                {
                    var sortingGroup = levelGroupList[level].Find("UIParticle").gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("MachinePopup");
                    sortingGroup.sortingOrder = 0;
                }
            }

            if (levelGroupList[level].Find("UIParticle1") != null)
            {
                levelGroupList[level].Find("UIParticle1").gameObject.SetActive(true);
                if (!levelGroupList[level].Find("UIParticle1").GetComponent<SortingGroup>())
                {
                    var sortingGroup = levelGroupList[level].Find("UIParticle1").gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("MachinePopup");
                    sortingGroup.sortingOrder = 0;
                }
            }
            await context.WaitSeconds(2.0f);
        }

        public void EnableButton(bool enable = true)
        {
            backGameBtn.transform.gameObject.SetActive(enable);
        }
    }
}