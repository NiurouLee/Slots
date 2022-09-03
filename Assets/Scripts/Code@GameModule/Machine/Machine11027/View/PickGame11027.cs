using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using SRF;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class PickGame11027 : TransformHolder
    {
        private Transform pickGame;
        private ExtraState11027 _extraState11027;
        private List<Transform> eggsShellList;
        private List<Transform> miniList;
        private List<Transform> minorList;
        private List<Transform> majorList;
        private List<Transform> megaList;
        private List<Transform> ultraList;
        private List<int> eggIdleList;
        private List<bool> isClickedList;
        private float basicRandomTime = 2.0f;
        private float randomTime = 0f;
        public bool isRand = false;
        public bool isResponsed = true;
        private List<Animator> _animators;
        private Animator animator;
        [ComponentBinder("rabbitAnimationNode")] protected Transform animationNode;
        public PickGame11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
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
                    PlayIdle();
                }
            }
        }
        
        //彩蛋随机Idle动画，每次播放三个彩蛋同事播放Idle动画
        private void PlayIdle()
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            for (var q = 0; q < pickItems.Count; q++)
            {
                if (pickItems[q].JackpotId > 0 || pickItems[q].RemovedJackpotId > 0 || pickItems[q].IsLucky == true)
                {
                    if (eggIdleList.Contains(q))
                    {
                        eggIdleList.Remove(q);
                    }
                }
            }
            List<int> randomList = new List<int>();
            int i = 0;
            if (eggIdleList.Count > 0)
            {
                if (eggIdleList.Count < 4)
                {
                    for (var e = 0; e < eggIdleList.Count; e++)
                    {
                        randomList.Add(eggIdleList[e]);
                    }
                }
                else
                {
                    List<int> tempEggIdle = new List<int>(eggIdleList);
                    for (var t = 0; t < 3; t++)
                    {
                        int rand = Random.Range(0, tempEggIdle.Count);
                        int randV = tempEggIdle[rand];
                        tempEggIdle.RemoveAt(rand);
                        randomList.Add(randV);
                    }
                }

                for (var r = 0; r < randomList.Count; r++)
                {
                    _animators[randomList[r]].Play("Idle", 0, 0);
                }

                randomTime = basicRandomTime;
                isRand = true;
            }
        }
        
        public void PlayState()
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            Dictionary<uint, List<int>> pickItemType = new Dictionary<uint, List<int>>();
            for (var m = 0; m < pickItems.Count; m++)
            {
                var pickItem = pickItems[m];
                if (pickItem.JackpotId > 0)
                {
                    if (pickItemType.ContainsKey(pickItem.JackpotId))
                    {
                        List<int> temp = pickItemType[pickItem.JackpotId];
                        temp.Add(m);
                    }
                    else
                    {
                        pickItemType.Add(pickItem.JackpotId,new List<int>(){m});
                    }
                }
            }

            for (var i = 0; i < pickItems.Count; i++)
            {
                var pickItem = pickItems[i];
                if (pickItem.RemovedJackpotId > 0 && pickItem.IsLucky == true)
                {
                    if (pickItemType.ContainsKey(pickItem.RemovedJackpotId))
                    {
                        pickItemType.Remove(pickItem.RemovedJackpotId);
                    }
                }
            }

            foreach (var child in pickItemType)
            {
                List<int> aniPlay = child.Value;
                string aniName = "";
                switch (aniPlay.Count)
                {
                    case 1:
                        aniName = "State1";
                        break;
                    case 2:
                        aniName = "State1";
                        break;
                    case 3:
                         aniName = "State3";
                        break;
                }
                foreach (var anichild in aniPlay)
                {
                    // 播放动画 aniName
                    if (_animators.Count> anichild && aniName != "")
                    {
                        _animators[anichild].transform.Find("CandyState").Find(child.Key+"").gameObject.SetActive(true);
                       _animators[anichild].Play(aniName,0,0);
                    }
                }
            }
        }

        //彩蛋初始化
        public void CreateColorfulEggs()
        {
            var childCount = 22;
            _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            _animators = new List<Animator>();
            eggsShellList = new List<Transform>();
            eggIdleList = new List<int>();
            isClickedList = new List<bool>();
            for (var i = 1; i < childCount; i++)
            {
                var pot = transform.Find("MainGroup").GetChild(i);
                eggsShellList.Add(pot);
                eggIdleList.Add(i-1);
                isClickedList.Add(true);
            }

            for (var a = 0; a < eggsShellList.Count; a++)
            {
                _animators.Add(eggsShellList[a].gameObject.GetComponent<Animator>());
                _animators[a].keepAnimatorControllerStateOnDisable = true;
            }

            for (var i = 0; i < eggsShellList.Count; i++)
            {
                if (!eggsShellList[i].transform.gameObject.GetComponent<SortingGroup>())
                {
                    var sortingGroup = eggsShellList[i].transform.gameObject.AddComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("Default");
                    sortingGroup.sortingOrder = 0;
                }
                else
                {
                    var sortingGroupNew = eggsShellList[i].transform.gameObject.GetComponent<SortingGroup>();
                    sortingGroupNew.sortingLayerID = SortingLayer.NameToID("Default");
                    sortingGroupNew.sortingOrder = 0;
                }
                var j = i;
                var pointerEventNightHandler = eggsShellList[i].gameObject.GetComponentOrAdd<PointerEventCustomHandler>();
                pointerEventNightHandler.BindingPointerClick((e)=>iconClick(j));
                if (pickItems[i].JackpotId > 0)
                {
                    isClickedList[i] = false;
                    eggsShellList[i].Find("EggState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(true);
                    eggsShellList[i].Find("CandyState").Find(pickItems[i].JackpotId + "").gameObject.SetActive(true);
                }else if (pickItems[i].RemovedJackpotId > 0 && pickItems[i].IsLucky == true)
                {
                    isClickedList[i] = false;
                    PlayJackpotLockStateIdle(pickItems[i].RemovedJackpotId);
                    PlayDarkIdle(pickItems[i].RemovedJackpotId);
                }
                else
                {
                    //初始化彩蛋时，只有EggState状态是可见的，CandyState和RabbitState以及子控件都是不可见的。
                    eggsShellList[i].Find("EggState").gameObject.SetActive(true);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").Find("RabbitDark").gameObject
                                    .SetActive(false);
                    for (var q = 1; q < 7; q++)
                    {
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("RabbitIcon").gameObject.SetActive(false);
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("RemoveTextSprite").gameObject.SetActive(false);
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("JPTextSprite").gameObject.SetActive(false);
                        eggsShellList[i].Find("CandyState").Find(q + "").gameObject.SetActive(false);
                    }
                }
            }
            //开始播放彩蛋随机动画
            PlayIdle(); 
        }

        private void PlayDarkIdle(uint jackPotId)
        {
            var pickItems = _extraState11027.GetPickItems();
            for (var i = 0; i < pickItems.Count; i++)
            {
                var pickItem = pickItems[i];
                if (pickItem.JackpotId == jackPotId)
                {
                    isClickedList[i] = false;
                    eggsShellList[i].Find("EggState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(true);
                    eggsShellList[i].Find("CandyState").Find(pickItems[i].JackpotId + "").gameObject.SetActive(true);
                    _animators[i].Play("Dark");
                }
                if (pickItem.RemovedJackpotId == jackPotId)
                {
                    isClickedList[i] = false;
                    eggsShellList[i].Find("EggState").gameObject.SetActive(false);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(true);
                    eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "").gameObject.SetActive(true);
                    eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "").Find("RabbitIcon").gameObject.SetActive(true);
                    eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "")
                        .Find("RemoveTextSprite").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "").Find("JPTextSprite")
                        .gameObject.SetActive(false);
                    _animators[i].Play("RabbitIdle");
                }
            }
        }
        
        public void PlayState3(uint jackPotId)
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            for (var m = 0; m < pickItems.Count; m++)
            {
                var pickItem = pickItems[m];
                if (pickItem.JackpotId == jackPotId)
                {
                    eggsShellList[m].Find("EggState").gameObject.SetActive(false);
                    eggsShellList[m].Find("RabbitState").gameObject.SetActive(false);
                   _animators[m].transform.Find("CandyState").Find(jackPotId+"").gameObject.SetActive(true);
                   _animators[m].Play("State3",0,0);
                   if (!eggsShellList[m].transform.gameObject.GetComponent<SortingGroup>())
                   {
                       var sortingGroup = eggsShellList[m].transform.gameObject.AddComponent<SortingGroup>();
                       sortingGroup.sortingLayerID = SortingLayer.NameToID("Default");
                       sortingGroup.sortingOrder = m+11;
                   }
                   else
                   {
                       var sortingGroupNew = eggsShellList[m].transform.gameObject.GetComponent<SortingGroup>();
                       sortingGroupNew.sortingOrder = m + 1;
                   }
                }
            }
        }

        //点击彩蛋
        private async void iconClick(int i)
        {
            if (isClickedList[i] == false)
            {
                return;
            }
            if (!isResponsed)
            {
                return;
            }
            isRand = false;
            isResponsed = false;
            isClickedList[i] = false;
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = i + "";
            await context.state.Get<ExtraState11027>().SendBonusProcess(cBonusProcess);
            //结算时全部不可点击
            if (_extraState11027.NeedPickSettle())
            {
                for (var ick = 0; ick < isClickedList.Count; ick++)
                {
                    isClickedList[ick] = false;
                }
            }
            //pick动画
            _animators[i].Play("Pick");
            _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            eggsShellList[i].Find("EggState").gameObject.SetActive(false);
            //如果翻出来的是普通jackpot
            if (pickItems[i].JackpotId > 0)
            {
                if (pickItems[i].JackpotId > 3)
                {
                    AudioUtil.Instance.PlayAudioFx("J01_Checked02");
                }
                else
                {
                    AudioUtil.Instance.PlayAudioFx("J01_Checked01");
                }
                eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                eggsShellList[i].Find("CandyState").gameObject.SetActive(true);
                eggsShellList[i].Find("CandyState").Find(pickItems[i].JackpotId + "").gameObject.SetActive(true); 
                await context.WaitSeconds(1.0f);
                isResponsed = true;
                PlayState();
                //结算时变暗Dark
                if (_extraState11027.NeedPickSettle())
                {
                    PlayState3(pickItems[i].JackpotId);
                    for (var q = 0; q < eggsShellList.Count; q++)
                    {
                        if(pickItems[q].JackpotId != pickItems[i].JackpotId)
                        {
                            if(pickItems[q].JackpotId >0)
                            {
                                eggsShellList[q].Find("EggState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(false);
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(true);
                                eggsShellList[q].Find("CandyState").Find(pickItems[q].JackpotId + "").gameObject
                                    .SetActive(true); 
                                _animators[q].Play("Dark",0,0);
                            }
                            if (pickItems[q].RemovedJackpotId >0)
                            {
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "").gameObject
                                    .SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("RabbitIcon").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("RemoveTextSprite").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("JPTextSprite").gameObject.SetActive(false);
                                _animators[q].Play("Dark", 0, 0);
                            }
                            if (pickItems[q].IsLucky == true)
                            {
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find("RabbitDark").gameObject
                                    .SetActive(true);
                                _animators[q].Play("Dark", 0, 0);
                            }
                        }
                    }
                }
            }
            //如果翻出来的是移除卡
            else if (pickItems[i].RemovedJackpotId > 0 && pickItems[i].IsLucky == true)
            {
                AudioUtil.Instance.PlayAudioFx("J01_Checked02");
                eggsShellList[i].Find("CandyState").gameObject.SetActive(false);
                eggsShellList[i].Find("RabbitState").gameObject.SetActive(true);
                eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "").gameObject.SetActive(true);
                eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId + "").Find("RabbitIcon").gameObject.SetActive(true);
                eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId+"").Find("RemoveTextSprite").gameObject.SetActive(true);
                eggsShellList[i].Find("RabbitState").Find(pickItems[i].RemovedJackpotId+"").Find("JPTextSprite").gameObject.SetActive(true);
                await context.WaitSeconds(1.33f);
                _animators[i].Play("Remove_Rabbit");
                //先将需要翻转的彩蛋翻转过来
                await Remove(pickItems[i].RemovedJackpotId); 
                //彩蛋完成翻转后播放RemoveJackpot动画
                await PLayRemoveJackpot(pickItems[i].RemovedJackpotId);
                await RabbitFly(pickItems[i].RemovedJackpotId);
                //拖尾，拖尾时间0.5秒
                AudioUtil.Instance.PlayAudioFx("J01_PandaFly");
                var endPos = context.view.Get<JackpotPanel11027>().GetAnimationNodePos(pickItems[i].RemovedJackpotId);
                var objFly = context.assetProvider.InstantiateGameObject("Fly", true);
                objFly.transform.parent = context.transform;
                var startPos = animationNode.transform.position;
                objFly.transform.position = startPos;
                await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f);
                //锁Jakpot
                await PlayLockJackpot(pickItems[i].RemovedJackpotId);
                context.assetProvider.RecycleGameObject("Fly", objFly);
                //根据jackpot数量播放state动画
                isResponsed = true;
                PlayState();
                //结算时变暗
                if (_extraState11027.NeedPickSettle())
                {
                    PlayState3(pickItems[i].RemovedJackpotId);
                    for (var q = 0; q < eggsShellList.Count; q++)
                    {
                        if(pickItems[q].JackpotId != pickItems[i].JackpotId)
                        {
                            if(pickItems[q].JackpotId >0)
                            {
                                eggsShellList[q].Find("EggState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(false);
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(true);
                                eggsShellList[q].Find("CandyState").Find(pickItems[q].JackpotId + "").gameObject
                                    .SetActive(true); 
                                _animators[q].Play("Dark",0,0);
                            }
                            if (pickItems[q].RemovedJackpotId >0)
                            {
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "").gameObject
                                    .SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("RabbitIcon").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("RemoveTextSprite").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                    .Find("JPTextSprite").gameObject.SetActive(false);
                                _animators[q].Play("Dark", 0, 0);
                            }
                            if (pickItems[q].IsLucky == true)
                            {
                                eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                                eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                                eggsShellList[q].Find("RabbitState").Find("RabbitDark").gameObject
                                    .SetActive(true);
                                _animators[q].Play("Dark", 0, 0);
                            }
                        }
                    }
                }
            }
            //pickGame结算
            if (_extraState11027.NeedPickSettle())
            {
                StopMusic();
                AudioUtil.Instance.PlayAudioFx("J01_Trigger01");
                await context.WaitSeconds(1.8f);
                ulong bonusWin = _extraState11027.GetPickWin();
                // ulong bonusWin = _extraState11027.GetBonusTotalWin() - _extraState11027.GetPanelWin();
                await Constant11027.ShowJackpot(context, pickItems[i].JackpotId, bonusWin);
                var bonusProxy11027 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11027;
                await bonusProxy11027.PickGameFinish();
            }
            else
            {
                isRand = true;
            }
        }

        public async Task RecoverSettle()
        {
            if (_extraState11027.NeedPickSettle())
            {
                AudioUtil.Instance.PlayAudioFx("J01_Trigger01");
                var pickItems = _extraState11027.GetPickItems();
                uint jackPotId = _extraState11027.GetPickWinJackpotId();
                PlayState3(jackPotId);
                for (var q = 0; q < eggsShellList.Count; q++)
                {
                    if (pickItems[q].JackpotId != jackPotId)
                    {
                        if (pickItems[q].JackpotId > 0)
                        {
                            eggsShellList[q].Find("EggState").gameObject.SetActive(false);
                            eggsShellList[q].Find("RabbitState").gameObject.SetActive(false);
                            eggsShellList[q].Find("CandyState").gameObject.SetActive(true);
                            eggsShellList[q].Find("CandyState").Find(pickItems[q].JackpotId + "").gameObject
                                .SetActive(true);
                            _animators[q].Play("Dark", 0, 0);
                        }
                        if (pickItems[q].RemovedJackpotId > 0)
                        {
                            eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                            eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                            eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "").gameObject
                                .SetActive(true);
                            eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                .Find("RabbitIcon").gameObject.SetActive(true);
                            eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                .Find("RemoveTextSprite").gameObject.SetActive(false);
                            eggsShellList[q].Find("RabbitState").Find(pickItems[q].RemovedJackpotId + "")
                                .Find("JPTextSprite").gameObject.SetActive(false);
                            _animators[q].Play("Dark", 0, 0);
                        }
                        if (pickItems[q].IsLucky == true)
                        {
                            eggsShellList[q].Find("CandyState").gameObject.SetActive(false);
                            eggsShellList[q].Find("RabbitState").gameObject.SetActive(true);
                            eggsShellList[q].Find("RabbitState").Find("RabbitDark").gameObject
                                .SetActive(true);
                            _animators[q].Play("Dark", 0, 0);
                        }
                    }
                }
                await context.WaitSeconds(1.8f);
                StopMusic();
                ulong bonusWin = _extraState11027.GetPickWin();
                await Constant11027.ShowJackpot(context, jackPotId, bonusWin);
                var bonusProxy11027 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11027;
                await bonusProxy11027.PickGameFinish();
            }
        }

        private async Task RabbitFly(uint jackPotId)
        {
            animator.Play("WheelJPRemove");
            await context.WaitSeconds(1.0f);
            // await FlyStar(jackPotId);
        }
        
        private async Task FlyStar(uint jackPotId)
        {
            //拖尾，拖尾时间0.5秒
            var endPos = context.view.Get<JackpotPanel11027>().GetAnimationNodePos(jackPotId);
            var objFly = context.assetProvider.InstantiateGameObject("Fly", true);
            objFly.transform.parent = context.transform;
            var startPos = animationNode.transform.position;
            objFly.transform.position = startPos;
            await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.5f);
            await context.WaitSeconds(0.5f);
            context.assetProvider.RecycleGameObject("Fly", objFly);
        }

         //先将需要翻转的彩蛋翻转过来
        private async Task Remove(uint jackPotId)
        {
             _extraState11027 = context.state.Get<ExtraState11027>();
            var pickItems = _extraState11027.GetPickItems();
            List<int> removeJackpotList = new List<int>();
            for (var i = 0; i < pickItems.Count; i++)
            {
                var pickItem = pickItems[i];
                if (pickItem.JackpotId == jackPotId && pickItem.Removed == true)
                {
                     isClickedList[i] = false;
                    _animators[i].Play("Pick");
                    eggsShellList[i].Find("EggState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(true);
                    eggsShellList[i].Find("CandyState").Find(pickItems[i].JackpotId + "").gameObject
                        .SetActive(true);
                    removeJackpotList.Add(i);
                }
            }
            if (removeJackpotList.Count > 0)
            {
                 await context.WaitSeconds(1.33f);
            }
        }

        //彩蛋完成翻转后播放RemoveJackpot动画
        private async Task PLayRemoveJackpot(uint jackPotId)
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            AudioUtil.Instance.PlayAudioFx("J01_Remove");
            var pickItems = _extraState11027.GetPickItems();
            for (var i = 0; i < pickItems.Count; i++)
            {
                var pickItem = pickItems[i];
                if (pickItem.JackpotId == jackPotId )
                {
                     _animators[i].Play("Remove_Jackpot");
                }
            }
            await context.WaitSeconds(2.0f);
            for (var i = 0; i < pickItems.Count; i++)
            {
                var pickItem = pickItems[i];
                if (pickItem.JackpotId == jackPotId )
                {
                     _animators[i].Play("Dark");
                }
            }
        }

        //锁Jakpot
        private async Task PlayLockJackpot(uint jackPotId)
        {
            //jackpot锁定动画
             AudioUtil.Instance.PlayAudioFx("J01_PandaJackpotRemove");
             await context.view.Get<JackpotPanel11027>().UpdateJackpotLockState((int)jackPotId); 
        }
        
        private void PlayJackpotLockStateIdle(uint jackPotId)
        {
            //jackpot锁定动画
            context.view.Get<JackpotPanel11027>().PlayJackpotLockStateIdle((int)jackPotId); 
        }
        
        //清理动画状态
        public void Reset()
        {
            isRand = false;
            if (eggsShellList!= null)
            {
                for (var i = 0; i < eggsShellList.Count; i++)
                {
                    //初始化彩蛋时，只有EggState状态是可见的，CandyState和RabbitState以及子控件都是不可见的。
                    eggsShellList[i].Find("EggState").gameObject.SetActive(true);
                    eggsShellList[i].Find("CandyState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").gameObject.SetActive(false);
                    eggsShellList[i].Find("RabbitState").Find("RabbitDark").gameObject
                                    .SetActive(false);
                    for (var q = 1; q < 7; q++)
                    {
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("RemoveTextSprite").gameObject
                            .SetActive(false);
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("JPTextSprite").gameObject
                            .SetActive(false);
                        eggsShellList[i].Find("RabbitState").Find(q + "").Find("RabbitIcon").gameObject
                            .SetActive(false);
                        eggsShellList[i].Find("CandyState").Find(q + "").gameObject.SetActive(false);
                    }
                    _animators[i].Play("Default");
                    if (!eggsShellList[i].transform.gameObject.GetComponent<SortingGroup>())
                    {
                        var sortingGroup = eggsShellList[i].transform.gameObject.AddComponent<SortingGroup>();
                        sortingGroup.sortingLayerID = SortingLayer.NameToID("Default");
                        sortingGroup.sortingOrder = 0;
                    }
                    else
                    {
                        var sortingGroupNew = eggsShellList[i].transform.gameObject.GetComponent<SortingGroup>();
                        sortingGroupNew.sortingLayerID = SortingLayer.NameToID("Default");
                        sortingGroupNew.sortingOrder = 0;
                    }
                }
            }
        }
        public Vector3 GetIntegralAnimationNodePos()
        {
            return transform.position;
        }
        
        public void PLayPickMusic()
        {
            AudioUtil.Instance.PlayMusic("Bg_LinkGame_11027",true);
        }

        public void StopMusic()
        {
             AudioUtil.Instance.StopMusic();
        }
    }
}