using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace GameModule
{
    public class BananaActive
    {
        public bool active;
        public Vector3 pos;
    }

    public class FeatureGame11017 : TransformHolder
    {
        private List<Transform> bananasTrans;
        private Transform FeatureObj;
        private float basicRandomTime = 1.0f;
        private float randomTime = 0f;
        public bool isRand = false;
        private Transform lastRandomTrans;
        private List<BananaActive> basicBananasPos;
        private List<int> eatingList;
        private List<string> goldRewardList;
        private float rollPosY;//金币，香蕉掉落在轮盘金马上
        private bool hasEaten = false; //判断是否有被吃掉的香蕉
        public FeatureGame11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            FeatureObj = inTransform;
        }

        public override void Update()
        {
            if (isRand)
            {
                randomTime -= Time.deltaTime;
                if (randomTime <= 0)
                {
                    isRand = false;
                    randomTime = basicRandomTime;
                    RandomActive();
                }
            }
        }

        public void SetFeature()
        {
            isRand = false;

            if (bananasTrans != null && bananasTrans.Count > 0)
            {
                for (int i = 0; i < bananasTrans.Count; i++)
                {
                    GameObject.Destroy(bananasTrans[i].gameObject);
                }
            }
            
            bananasTrans = new List<Transform>();
            basicBananasPos = new List<BananaActive>();
            eatingList = new List<int>();
            hasEaten = false;
            goldRewardList = new List<string>();
            var _extraState = context.state.Get<ExtraState11017>();
            GorillaEatBananaGameResultExtraInfo.Types.Banana[] bananas = _extraState.GetBananas();
            
            float startY = 5.0f;
            float baseJSpace = 0.6f;
            float baseCoinSpace = 0.6f;
            int coinCount = 0;
            int JCount = 0;
            float localX = 0;
            float localY = 0;

            for (int i = 0; i < bananas.Length; i++)
            {
                var banana = bananas[i];
                if (banana == null) break;
                GameObject activeTemp = null;
                string goldNumber = "";
                var bet = context.state.Get<BetState>().totalBet;

                if (banana.IsCoin)
                {
                    ++coinCount;
                    activeTemp = context.assetProvider.InstantiateGameObject("Active_Coin");
                    goldNumber = (bet * (banana.WinRate + banana.JackpotPay) / 100).GetAbbreviationFormat();
                }
                else
                {
                    ++JCount;
                  
                    if (banana.JackpotId == 1)
                    {
                        activeTemp = context.assetProvider.InstantiateGameObject("Active_J03");
                        goldNumber = "1";
                    }
                    else if (banana.JackpotId == 2)
                    {
                        activeTemp = context.assetProvider.InstantiateGameObject("Active_J02");
                        goldNumber = "2";
                    }
                    else if (banana.JackpotId == 3)
                    {
                        activeTemp = context.assetProvider.InstantiateGameObject("Active_J01");
                        goldNumber = "3";
                    }
                    else if (banana.JackpotId == 0)
                    {
                        activeTemp = context.assetProvider.InstantiateGameObject("Active_J04");
                        goldNumber = (bet * (banana.WinRate + banana.JackpotPay) / 100).GetAbbreviationFormat();
                        if (activeTemp)
                        {
                            bet = context.state.Get<BetState>().totalBet;
                            activeTemp.transform.Find("Root").Find("ActiveControl").Find("IntegralText")
                                .GetComponent<TextMesh>().text = goldNumber;
                        }
                    }
                }

                goldRewardList.Add(goldNumber);

                if (i % 10 == 0)
                {
                    localX = (-2.2f) + (i / 10) * (2.2f);
                    JCount = 0;
                    coinCount = 0;
                }

                localY = startY - baseJSpace * JCount - baseCoinSpace * coinCount;
                activeTemp.transform.SetParent(FeatureObj);
                activeTemp.transform.localScale = Vector3.one;
                activeTemp.transform.localPosition = new Vector3(localX, localY, 0);
                bananasTrans.Add(activeTemp.transform);

                BananaActive bananaActive = new BananaActive();
                bananaActive.active = true;
                bananaActive.pos = activeTemp.transform.position;
                
                basicBananasPos.Add(bananaActive);
            }

            lastRandomTrans = null;
            RandomActive();
        }

        private void RandomActive()
        {
            if (lastRandomTrans)
            {
                lastRandomTrans.GetComponent<Animator>().Play("Idle", 0, 0);
            }

            if (bananasTrans != null && bananasTrans.Count > 0)
            {
                int len = bananasTrans.Count;
                int rand = Random.Range(0, len);
                bananasTrans[rand].GetComponent<Animator>().Play("RandomActive", 0, 0);
                randomTime = basicRandomTime;
                isRand = true;
                lastRandomTrans = bananasTrans[rand];
            }
        }

        public async Task EatingAction()
        {
            isRand = false;
            List<int> hideList = new List<int>();
          
            if (bananasTrans == null || bananasTrans.Count <= 0) return;
            
            var wheels = context.state.Get<WheelsActiveState>().GetRunningWheel();
            rollPosY = wheels[0].GetRoll(0).transform.position.y;

            var extraState = context.state.Get<ExtraState11017>();
            var bananas = extraState.GetBananas();

            List<int> eatenList = new List<int>(); // 本次被吃香蕉列表
            string[] colGoldWholeNumber = {"-1", "-1", "-1"};
            
            int maxEatenRow = 0; //被吃的最大的是哪一行（0-9）
            hasEaten = false;
            
            for (int i = 0; i < bananas.Length; i++)
            {
                GorillaEatBananaGameResultExtraInfo.Types.Banana banana = bananas[i];
                if (banana == null) break;
                if (i >= bananasTrans.Count) return;
                if (banana.Eaten)
                {
                    if (!eatingList.Contains(i))
                    {
                        eatingList.Add(i);
                        eatenList.Add(i);

                        int col = i / 10;
                        if (col < 3)
                        {
                            colGoldWholeNumber[col] = goldRewardList[i];
                        }

                        if (i % 10 > maxEatenRow)
                        {
                            maxEatenRow = i % 10;
                        }

                        hasEaten = true;
                    }
                }
            }

            //计算下落位移
            for (int k = 0; k < eatenList.Count; k++)
            {
                int col = eatenList[k] / 10;
                int row = eatenList[k] % 10;
                for (int w = 0; w < row; w++)
                {
                    BananaActive bananaActive = basicBananasPos[col * 10 + w];
                    if (!bananaActive.active) continue;
                    bananaActive.pos = SearchNextActiveBanana(col * 10 + w + 1, eatenList[k]);
                }
                basicBananasPos[eatenList[k]].active = false;
            }

            if (hasEaten)
            {
                //本轮有香蕉被吃
                //判断panel里面有没有金色大象
                //大象等待动画           
                await WaitGoldCollectAnimation();
                var wheel = context.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
                var reTriggerElementContainers = wheel.GetElementMatchFilter((container) =>
                {
                    if (container.sequenceElement.config.id == 16)
                    {
                        return true;
                    }

                    return false;
                });
                //当有金色大象的时候，香蕉需要播放当前列的选取动画。大象在哪列就播放哪列的随机动画
                //播放规则是当前列从上到下播放，再从下到上播放。最后从上到下播到被吃的那个香蕉停止。
                //此时需要播放大象的等待动画（两个动画是配合动画）
                if (reTriggerElementContainers.Count > 0)
                {
                    //根据大象在哪列播放哪列的香蕉动画
                    for (int i = 0; i < 3; i++)
                    {
                        PlayColAnimation(i, eatenList);
                    }

                    await context.WaitSeconds(0.1f * 20 + maxEatenRow * 0.1f); //列动画从上到下，从下到上，然后从上到最大行所花费的时间
                }
                AudioUtil.Instance.PlayAudioFx("Bouns_Drop");
                
                for (int k = 0; k < bananasTrans.Count; k++)
                {
                    if (eatenList.Contains(k))
                    {
                        //将大象身上需要显示的值传过去
                        //当上面的动画播完后，开始香蕉掉落流程。
                        float endDropY = rollPosY;//掉落在大象的y轴位置上
                        float dis = Math.Abs(endDropY - bananasTrans[k].position.y);
                        //传值给金色猛犸象
                        var a = k;
                        bananasTrans[k].GetComponent<Animator>().Play("Drop");
                        bananasTrans[k].DOMoveY(endDropY, 0.5f).OnComplete(() =>
                        {
                            AudioUtil.Instance.PlayAudioFx("Bouns_DropEnd");
                            bananasTrans[a].gameObject.SetActive(false);
                        }).SetEase(Ease.Linear);

                        continue;
                    }

                    if (bananasTrans[k].position != basicBananasPos[k].pos && basicBananasPos[k].active)
                    {
                        float dis = Math.Abs(basicBananasPos[k].pos.y - bananasTrans[k].position.y);
                        bananasTrans[k].DOMoveY(basicBananasPos[k].pos.y, 0.5f);
                    }
                }
                await context.WaitSeconds(0.25f);
                await StartGoldCollectAnimation(colGoldWholeNumber);
            }
            else
            {
                //本轮没有香蕉被吃
            }
            await context.WaitSeconds(1.0f-0.25f);
            randomTime = basicRandomTime;
            isRand = true;
            extraState.SetEatenAniFinish(true);
        }

        public async void PlayColAnimation(int i, List<int> eatenList)
        {
            var wheel = context.state.Get<WheelsActiveState11017>().GetRunningWheel()[0];
            var elementContainer = wheel.GetRoll(i).GetVisibleContainer(0);
            if (elementContainer.sequenceElement.config.id == 16)
            {
                for (var m = 0; m < 10; m++)
                {
                    if (!bananasTrans[i * 10 + m].gameObject.activeSelf) continue;
                    AudioUtil.Instance.PlayAudioFx("Bouns_Win_Random");
                    bananasTrans[i * 10 + m].GetComponent<Animator>().Play("RandomDrop", 0, 0);
                    await context.WaitSeconds(0.1f); //RandomActive动画的时长
                }

                for (var n = 9; n >= 0; n--)
                {
                    if (!bananasTrans[i * 10 + n].gameObject.activeSelf) continue;
                    AudioUtil.Instance.PlayAudioFx("Bouns_Win_Random");
                    bananasTrans[i * 10 + n].GetComponent<Animator>().Play("RandomDrop", 0, 0);
                    await context.WaitSeconds(0.1f);
                }

                for (var s = 1; s < 10; s++)
                {
                    int bananaId = i * 10 + s;
                    if (!bananasTrans[bananaId].gameObject.activeSelf) 
                        continue;
                  
                    if (eatenList.Contains(bananaId))
                    {
                        //播放到此列被吃的香蕉后直接退出再往下吃的操作
                        bananasTrans[i * 10 + s].GetComponent<Animator>().Play("BeforeDrop", 0, 0);
                        await context.WaitSeconds(0.33f);
                        break;
                    }
                    AudioUtil.Instance.PlayAudioFx("Bouns_Win_Random");
                    bananasTrans[i * 10 + s].GetComponent<Animator>().Play("RandomDrop", 0, 0);
                    await context.WaitSeconds(0.1f);
                }
            }
        }

        public async Task WaitGoldCollectAnimation()
        {
            var wheels = context.state.Get<WheelsActiveState>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 16)
                {
                    return true;
                }

                return false;
            });

            var pinkElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 15)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Idle");
                }

                if (pinkElementContainers.Count > 0)
                {
                    for (var i = 0; i < pinkElementContainers.Count; i++)
                    {
                        pinkElementContainers[i].PlayElementAnimation("Idle");

                    }
                }
            }
        }

        public async Task StartGoldCollectAnimation(string[] colGoldWholeNumber)
        {
            var wheels = context.state.Get<WheelsActiveState>().GetRunningWheel();
            List<int> bigEleCol = new List<int>(); // 哪一列有大象
            int index = -1;
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                index++;
                var id = container.sequenceElement.config.id;
                
                if ( id == 16 || id == 15)
                {
                    bigEleCol.Add(index);
                    return true;
                }
                
                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    int col = bigEleCol[i];
                    string itegralText = "";
                    if (col < colGoldWholeNumber.Length)
                    {
                        itegralText = colGoldWholeNumber[col];
                        if (itegralText == "-1" || itegralText == "0") //移除初始值和0的情况
                        {
                            itegralText = "";
                        }
                    }

                    reTriggerElementContainers[i].PlayElementAnimation("Collect");

                    var node = reTriggerElementContainers[i].transform.GetChild(0).Find("Node");
                   
                    node.Find("machine_11017_grand").gameObject.SetActive(itegralText == "3");
                    node.Find("machine_11017_major").gameObject.SetActive(itegralText == "2");
                    node.Find("machine_11017_minor").gameObject.SetActive(itegralText == "1");

                    if (itegralText != "1" || itegralText != "2" || itegralText != "3")
                    {
                        node.Find("IntegralText").gameObject.SetActive(true);
                        node.Find("IntegralText").GetComponent<TextMesh>().text = itegralText;
                    }
                    else
                    {
                        node.Find("IntegralText").gameObject.SetActive(false);
                    }
                }
            }
        }

        public async Task StartPlayPinkCollectAnimation()
        {
            var wheels = context.state.Get<WheelsActiveState>().GetRunningWheel();
            var reTriggerElementContainers = wheels[0].GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 15)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    reTriggerElementContainers[i].PlayElementAnimation("Collect");
                }
            }

        }

        public Vector3 SearchNextActiveBanana(int startIndex, int endIndex)
        {

            if (endIndex >= basicBananasPos.Count) return Vector3.zero;
            if (startIndex == endIndex) return basicBananasPos[startIndex].pos;
            Vector3 v = new Vector3();

            for (int i = startIndex; i <= endIndex; i++)
            {
                BananaActive bananaActive = basicBananasPos[i];
                if (bananaActive.active)
                {
                    v = bananaActive.pos;
                    break;
                }
            }
            return v;
        }

        public bool GetIsHaveEaten()
        {
            return hasEaten;
        }
    }
}