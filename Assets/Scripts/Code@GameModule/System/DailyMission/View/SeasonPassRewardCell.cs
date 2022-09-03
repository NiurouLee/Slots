//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-08 16:13
//  Ver : 1.0.0
//  Description : SeasonPassRewardCell.cs
//  ChangeLog :
//  **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassRewardCell: View<SeasonPassRewardCellViewController>
    {
        [ComponentBinder("NumGroup")]
        public Transform transNumGroup;
        [ComponentBinder("NumGroup/NormalState/NumText")]
        public TextMeshProUGUI txtNeedPlayerLevel;
        [ComponentBinder("NumGroup/CollectAndFinishState")]
        public Transform transFinishState;
        [ComponentBinder("NumGroup/CollectAndFinishState/NumText")]
        public TextMeshProUGUI txtPlayerLevel;

        [ComponentBinder("Progress/ProgressBar")]
        public Slider progressBar;
        [ComponentBinder("Progress/ProgressBar")]
        public RectTransform rectPregressBar;
        
                
        [ComponentBinder("Top")]
        public Animator animatorTop;
        [ComponentBinder("Top/One")]
        public Transform transTopOneContainer;
        [ComponentBinder("Top/Two")]
        public Transform transTopTwoContainer;

        [ComponentBinder("Down")]
        public Animator animatorDown;
        [ComponentBinder("Down/One")]
        public Transform transDownOneContainer;
        [ComponentBinder("Down/Two")]
        public Transform transDownTwoContainer;


        public SeasonPassRewardCell()
        {
        }

    }

    public class SeasonPassRewardCellViewController : ViewController<SeasonPassRewardCell>
    {
        public int Level;
        public bool IsDoubleCell;
        public bool IsNextDouble;
        private void InitDoubleItems(SeasonPassPageViewController pageController)
        {
            ClearChilden(view.transTopTwoContainer);
            ClearChilden(view.transDownTwoContainer);
            ClearChilden(view.transTopOneContainer);
            ClearChilden(view.transDownOneContainer);
            var goldenItems = pageController.GetGoldenRewards(Level);
            var freeItems = pageController.GetFreeRewards(Level);
            if (IsDoubleCell)
            {
                if (goldenItems.Count == 2)
                {
                    var transDoubleTop = GetCellItemContainer(pageController, view.transTopTwoContainer, true, 2);   
                    var cellGolden = GetCellItem(transDoubleTop, "Cell1/StateGroup");
                    cellGolden.InitReward(pageController,true, Level, 0);
                    var cellGoldenLimited = GetCellItem(transDoubleTop, "Cell2/StateGroup");
                    cellGoldenLimited.InitReward(pageController,true, Level, 1);
                    var cellGoldenLimitedTime = GetCellItem(transDoubleTop, "Cell2/LimitedStateGroup");
                    cellGoldenLimitedTime.InitReward(pageController,true, Level, 1);  
                }
                if (freeItems.Count == 2)
                {
                    var transDoubleDown = GetCellItemContainer(pageController, view.transDownTwoContainer, false, 2); 
                    
                    var cellFree = GetCellItem(transDoubleDown, "Cell1/StateGroup");
                    cellFree.InitReward(pageController,false, Level, 0);
                    var cellFreeLimited = GetCellItem(transDoubleDown, "Cell2/StateGroup");
                    cellFreeLimited.InitReward(pageController,false, Level, 1);
                    var cellFreeLimitedTime = GetCellItem(transDoubleDown, "Cell2/LimitedStateGroup");
                    cellFreeLimitedTime.InitReward(pageController,false, Level, 1); 
                }   
            }
            if (goldenItems.Count == 1)
            {
                var isLimited = goldenItems[0].IsTimed;
                var  transSingleTop = GetCellItemContainer(pageController, view.transTopOneContainer, true, 1, isLimited);
                if (isLimited)
                {
                    var cellGoldenLimited = GetCellItem(transSingleTop, "StateGroup");
                    cellGoldenLimited.InitReward(pageController,true, Level, 0);
                    var cellGoldenLimitedTime = GetCellItem(transSingleTop, "LimitedStateGroup");
                    cellGoldenLimitedTime.InitReward(pageController,true, Level, 0);  
                }
                else
                
                {
                    var cellGolden = GetCellItem(transSingleTop, "StateGroup");
                    cellGolden.InitReward(pageController,true, Level, 0);   
                }
                transSingleTop.SetParent(view.transTopOneContainer,false);
                transSingleTop.gameObject.SetActive(true);
            }
            if (freeItems.Count == 1)
            {
                var isLimited = freeItems[0].IsTimed;
                var transSingleDown = GetCellItemContainer(pageController, view.transDownOneContainer, false, 1,isLimited);

                if (isLimited)
                {
                    var cellFreeLimited = GetCellItem(view.transDownOneContainer, "StateGroup");
                    cellFreeLimited.InitReward(pageController,false, Level, 0);
                    var cellFreeLimitedTime = GetCellItem(view.transDownOneContainer, "LimitedStateGroup");
                    cellFreeLimitedTime.InitReward(pageController,false, Level, 0); 
                }
                else
                {
                    var cellFree = GetCellItem(transSingleDown, "StateGroup");
                    cellFree.InitReward(pageController,false, Level, 0);
                }
                transSingleDown.SetParent(view.transDownOneContainer,false);
                transSingleDown.gameObject.SetActive(true);

            }
        } 
        public void InitRewardItem(SeasonPassPageViewController pageController, bool isDouble, bool isNextDouble, int level, bool isLast)
        {
            Level = level;
            IsDoubleCell = isDouble;
            IsNextDouble = isNextDouble;
            view.txtPlayerLevel.text = level.ToString();
            view.txtNeedPlayerLevel.text = level.ToString();

            var curLevel = Client.Get<SeasonPassController>().Level;
            view.transFinishState.gameObject.SetActive(curLevel >= level);
            view.progressBar.gameObject.SetActive(!isLast);
            PlayIdleAnim();
            InitDoubleItems(pageController);
            InitProgressBarSize(curLevel);
        }

        private void InitProgressBarSize(uint curLevel)
        {
            var progress = 0f;
            if (curLevel>Level)
            {
                progress = 1f;
            }
            else
            {
                progress = 0f;
            }
            var value = Client.Get<SeasonPassController>().Exp * 1f /
                       Client.Get<SeasonPassController>().ExpTotal;
            if (IsNextDouble || IsDoubleCell)
            {
                if (Level == curLevel)
                {
                    progress = 0.2f + value * (1 - 0.2f - 0.07f);
                }
                view.rectPregressBar.sizeDelta = new Vector2(350, 56f);
                view.rectPregressBar.anchoredPosition = new Vector2(10f, 0f);
            }
            else
            {
                if (Level == curLevel)
                {
                    progress = 0.33f + value * (1 - 0.33f - 0.07f);
                }
                view.rectPregressBar.sizeDelta = new Vector2(230, 56);
                view.rectPregressBar.anchoredPosition = new Vector2(10f, 0);
            }

            if (Level == curLevel)
            {
                view.progressBar.value = value;
            }
            view.progressBar.value = progress;
        }
        private void ClearChilden(Transform transform)
        {
            if (transform)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    GameObject.Destroy(transform.GetChild(i).gameObject);
                }   
            }
        }

        private Transform GetCellItemContainer(SeasonPassPageViewController pageController, Transform parent, bool goldItem, int num, bool isLimited=false)
        {
            var container = pageController.NewCellItem(goldItem, num, isLimited);
            container.SetParent(parent,false);
            container.gameObject.SetActive(true);
            return container;
        }

        private async void PlayIdleAnim()
        {
            if (view.transform.gameObject.activeInHierarchy)
            {
                await XUtility.WaitSeconds(Random.Range(0, 0.2f), this);
                XUtility.PlayAnimation(view.animatorTop, "Idle");
                await XUtility.WaitSeconds(Random.Range(0, 0.2f), this);
                XUtility.PlayAnimation(view.animatorDown, "Idle");
            }
        }

        private SeasonPassRewardCellItem GetCellItem(Transform parent, string path)
        {
            return view.AddChild<SeasonPassRewardCellItem>(parent.Find(path));
        }
    }
}