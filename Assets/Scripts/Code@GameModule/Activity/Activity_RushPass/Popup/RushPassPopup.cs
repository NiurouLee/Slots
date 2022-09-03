using System.Net.Mime;
using System;
using System.Collections.Generic;
using System.Xml.XPath;
using DragonU3DSDK.Network.API.ILProtocol;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIRushPassMain", "UIRushPassMainV")]
    public class RushPassPopup : Popup<RushPassViewController>
    {
        #region MyRegion

        [ComponentBinder("Root/Group/PaidGroup")]
        public RectTransform paidGroup;

        [ComponentBinder("Root/Group/LeveRushProgressBar/Slider")]
        public Slider slider;

        [ComponentBinder("Root/Group/FreeGroup")]
        public RectTransform freeGroup;

        [ComponentBinder("Root/Group/LeveRushProgressBar/STGroup")]
        public RectTransform sTGroup;

        [ComponentBinder("Root/Group/RushPassGroup")]
        public RectTransform rushPassGroupTrf;

        [ComponentBinder("Root/Group/RushPassGroup/RushPassBtn")]
        public Button rushPassBtn;

        [ComponentBinder("Root/Group/RushPassGroup/PurchaseBtn")]
        public Button purchaseBtn;

        [ComponentBinder("Root/Group/HelpBtn")]
        public Button helpBtn;

        [ComponentBinder("Root/Group/Time/TextTime")]
        public Text textTime;

        [ComponentBinder("Root/Group/RushPassGroup/RushPassBtn/TextPrice")]
        public Text textPrice;

        [ComponentBinder("Root/Group/RushPassGroup/RushPassBtn/TextPrice/Icon (1)")]
        public Image priceIcon;
        [ComponentBinder("Root/Group/Title")] public RectTransform titleTrf;

        #endregion

        public RushPassPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1324, 768);
            contentDesignSizeH = new Vector2(768, 1400);
        }

        /// <summary>
        /// 免费
        /// </summary>
        public List<RushPassRewardBoxCell> rewardBoxCells;

        /// <summary>
        /// 付费
        /// </summary>
        public List<RushPassPaidRewardBoxCell> paidRewardBoxCells;

        /// <summary>
        /// 进度节点
        /// </summary>
        public List<ScheduleCell> ScheduleCells;

        protected override void SetUpExtraView()
        {
            transform.localScale = CalculateScaleInfo();
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            int childCount = freeGroup.childCount;
            rewardBoxCells = new List<RushPassRewardBoxCell>(childCount);
            paidRewardBoxCells = new List<RushPassPaidRewardBoxCell>(childCount);
            ScheduleCells = new List<ScheduleCell>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                rewardBoxCells.Add(AddChild<RushPassRewardBoxCell>(freeGroup.GetChild(i)));
                paidRewardBoxCells.Add(AddChild<RushPassPaidRewardBoxCell>(paidGroup.GetChild(i)));
                ScheduleCells.Add(AddChild<ScheduleCell>(sTGroup.GetChild(i)));
            }
        }

        /// <summary>
        /// 领取免费的
        /// </summary>
        /// <param name="index"></param>
        public void CollectFree(int index)
        {
            rewardBoxCells[index].Collect();
        }

        /// <summary>
        /// 领取付费的
        /// </summary>
        /// <param name="indexs"></param>
        public void CollectPaid(List<int> indexs)
        {
            for (int i = 0; i < indexs.Count; i++)
            {
                paidRewardBoxCells[indexs[i]].Collect();
            }
        }
        


        /// <summary>
        /// n天的付费发光
        /// </summary>
        /// <param name="indexs"></param>
        public void ReachEffectPaids(List<int> indexs)
        {
            for (int i = 0; i < indexs.Count; i++)
            {
                paidRewardBoxCells[indexs[i]].ReachEffect();
            }
        }


        public void LockCollect(int index)
        {
            paidRewardBoxCells[index].LockCollect();
        }


        /// <summary>
        /// 解锁
        /// </summary>
        public void UnlockPaid()
        {
            for (int i = 0; i < paidRewardBoxCells.Count; i++)
            {
                paidRewardBoxCells[i].UnLock();
            }
        }

        /// <summary>
        /// 发光一天
        /// </summary>
        /// <param name="index"></param>
        public void ReachEffect(int index,bool inIsPaid)
        {
            rewardBoxCells[index].ReachEffect();
            if (inIsPaid)
            {
                paidRewardBoxCells[index].ReachEffect();
            }
            else
            {
                paidRewardBoxCells[index].LockCollect();
            }
        }
        
        /// <summary>
        /// 购买成功
        /// </summary>
        public void PurchaseSuccess()
        {
            rushPassGroupTrf.gameObject.SetActive(false);
            //关闭轮播动画  
            titleTrf.GetComponent<Animator>().enabled = false;
            titleTrf.Find("Tite1").gameObject.SetActive(true);
            titleTrf.Find("Tite1").GetComponent<Image>().color = new Color(1,1,1,1);
            titleTrf.Find("Tite2").gameObject.SetActive(false);
            titleTrf.Find("Tite2").GetComponent<Image>().color = new Color(1,1,1,0);
        }

    }

    //-----------------------------------viewController------------------------------------------
    public class RushPassViewController : ViewController<RushPassPopup>
    {
        private Activity_LevelRushRushPass _activity;
        private ShopItemConfig _shopItemConfig;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            EnableUpdate(1);
            InitializeReward();
            RefTime();
            CheckNeedCollect();
        }

        protected override void SubscribeEvents()
        {
            view.rushPassBtn.onClick.AddListener(OnRushPassBtnClicked);
            view.purchaseBtn.onClick.AddListener(OnPurchaseBtnClicked);
            view.helpBtn.onClick.AddListener(OnHelpBtnClicked);
            SubscribeEvent<EventRushPassPaidFinish>(PurchaseSuccess);
            SubscribeEvent<EventActivityExpire>(OnRushPassExpire);
            SubscribeEvent<EventLevelRushStateChanged>(OnLevelRushExpire);
            SubscribeEvent<EventRushPassPaidFail>(PurchaseFail);
        }

        

        private void OnLevelRushExpire(EventLevelRushStateChanged obj)
        {
            var levelRushIsEnable = Client.Get<LevelRushController>().IsLevelRushEnabled();
            if (!levelRushIsEnable)
            {
                view.Close();
            }
        }

        private void OnRushPassExpire(EventActivityExpire obj)
        {
            if (obj.activityType==ActivityType.RushPass)
            {
                view.Close();
            }
        }


        /// <summary>
        /// 检查是否有可以收集的 如果有走收集流程
        /// </summary>
        private  async  void CheckNeedCollect()
        {
            if (_activity.IsHaveCanCollect())
            {
                SetButtonInteractable(false);
                if (_activity.FreeIsHaveCanCollect())
                {
                    var day = _activity.GetFreeCanCollectDay();
                    var fillAmount = ((float) (day+1)) / view.ScheduleCells.Count;
                    view.slider.DOValue(fillAmount, 1.5f);
                    await WaitForSeconds(1.5f);
                    ScheduleCellCollectAnim();
                      
                }
                else
                {
                    var paisIndex = _activity.GetPaidCanCollectDays();
                    view.ReachEffectPaids(paisIndex);
                    await WaitForSeconds(0.5f);
                    var popup=await PopupStack.ShowPopup<RushPassUnPaidCollectPopup>();
                        popup.SubscribeCloseAction(async () =>
                        {
                            view.CollectPaid(paisIndex);
                            SetButtonInteractable(true);
                        });
                }
            }
            else
            {
                var day = _activity.CollectSchedule;
                var fillAmount = ((float) (day+1)) / view.ScheduleCells.Count;
                view.slider.value = fillAmount;
                SetButtonInteractable(true);
            }
        }


        /// <summary>
        /// 收集流程
        /// </summary>
        private  async void   ScheduleCellCollectAnim()
        {
            SetButtonInteractable(false);
            var freeIndex = _activity.GetFreeCanCollectDay();
            view.ScheduleCells[(int) freeIndex ].Collect();
            //发光+放大一下
            view.ReachEffect((int) freeIndex,_activity.IsPaid );
            await WaitForSeconds(0.5f);
            
            if (_activity.IsPaid)
                {
                    var paisIndexes = _activity.GetPaidCanCollectDays();
                    var popup=await PopupStack.ShowPopup<RushPassIsPaidCollectPopup>();
                    popup.SubscribeCloseAction(async () =>
                    {
                        view.CollectFree((int) freeIndex);
                        view.CollectPaid(paisIndexes);
                        SetButtonInteractable(true);
                    });
                }
                else
                {
                    var popup=await PopupStack.ShowPopup<RushPassUnPaidCollectPopup>();
                    popup.SubscribeCloseAction(async () =>
                    {
                        view.CollectFree((int) freeIndex);
                        SetButtonInteractable(true);
                    });
                }
        }


        /// <summary>
        /// 购买成功
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private async void PurchaseSuccess(EventRushPassPaidFinish obj)
        {
             await _activity.PurchaseSuccess();
             //购买成功  ui 更新
            view.PurchaseSuccess();
            //播放解锁动画
            view.UnlockPaid();
            if (_activity.IsHaveCanCollect())
            {
                SetButtonInteractable(false);
                WaitForSeconds(1, async () =>
                    {
                        var paisIndexes = _activity.GetPaidCanCollectDays();
                        //发光
                        view.ReachEffectPaids(paisIndexes);
                        await WaitForSeconds(0.5f);
                        var popup=await PopupStack.ShowPopup<RushPassIsPaidCollectPopup>();
                        popup.SubscribeCloseAction(async () =>
                        {
                            if (_activity.PayBiInfo=="2")
                            {
                                    view.CollectFree(_activity.CollectSchedule);       
                            }
                            
                            view.CollectPaid(paisIndexes);
                            SetButtonInteractable(true);
                        });
                    }
                );
            }
            else
            {
                SetButtonInteractable(true);
            }
        }
        
        /// <summary>
        /// 购买失败
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PurchaseFail(EventRushPassPaidFail obj)
        {
            if (_activity.PayBiInfo=="1")
            {
                view.Close();
            }
        }

        /// <summary>
        /// 购买
        /// </summary>
        private void OnRushPassBtnClicked()
        {
            _activity.SetPayBiInfo("1");
            Client.Get<IapController>().BuyProduct(_shopItemConfig);
        }

        /// <summary>
        /// 奖励通用弹窗
        /// </summary>
        /// <returns></returns>
        private async void OnPurchaseBtnClicked()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(_shopItemConfig.SubItemList);
        }

        /// <summary>
        /// 详细信息
        /// </summary>
        /// <returns></returns>
        private async void OnHelpBtnClicked()
        {
            await PopupStack.ShowPopup<RushPassHelpPopup>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitializeReward()
        {
            _activity =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            _shopItemConfig = _activity.ShopItemConfig;

            if (_activity == null)
            {
                XDebug.LogError(" rushpass _activity==null--");
                view.Close();
                return;
            }

            var infos = _activity.GetRewardInfos();

            if (infos == null || infos.Count == 0)
            {
                XDebug.LogError("rushPass get rewards info == null");
                view.Close();
                return;
            }

            bool isPaid = _activity.IsPaid;
            int schedule = _activity.CollectSchedule;

            view.slider.value = ((float) schedule / view.freeGroup.childCount);
            for (int i = 0; i < view.freeGroup.childCount; i++)
            {
                var info = infos[i];
                view.rewardBoxCells[i].InitializeBoxCell(info.RewardFree.Reward, info.RewardFree.Status);
                view.paidRewardBoxCells[i].InitializeBoxCell(info.RewardPaid.Reward, isPaid, info.RewardPaid.Status);
                view.ScheduleCells[i].InitializeScheduleCell(i  <= schedule);
            }

            if (isPaid)
            {
                view.rushPassGroupTrf.gameObject.SetActive(false);
                view.PurchaseSuccess();
            }
            else
            {
                view.textPrice.text = _shopItemConfig.Price.ToString();
            }
        }

     


        /// <summary>
        /// 倒计时
        /// </summary>
        public override void Update()
        {
            base.Update();
            RefTime();
        }

        //刷新时间
        private void RefTime()
        {
            var leftTime = _activity.GetCountDown();
            if (leftTime > 0)
            {
                view.textTime.text = XUtility.GetTimeText(leftTime).ToUpper();
            }
            else
            {
                view.textTime.text = "00:00:00";
                view.Close();
            }
        }
        

        private void OpenIsPaidPopup()
        {
            PopupStack.ShowPopupNoWait<RushPassIsPaidCollectPopup>();
        }
        
        
        private void SetButtonInteractable(bool inInteractable)
        {
            view.closeButton.interactable = inInteractable;
            view.purchaseBtn.interactable = inInteractable;
            view.rushPassBtn.interactable = inInteractable;
            view.textPrice.color = inInteractable ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
            view.priceIcon.color=inInteractable ? new Color(1, 1, 1, 1) : new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }


    //--------------------------------------------------------------//

    /// <summary>
    ///免费的一组item
    /// </summary>
    public class RushPassRewardBoxCell : View
    {
        [ComponentBinder("Received")] public Transform receivedTrf;
        [ComponentBinder("Icon")] public Transform iconTrf;


        private uint _status = 0;

        private Animator _animator;
        private List<Animator> _itemAnimators;

        public void InitializeBoxCell(Reward rewards, uint status)
        {
            _animator = transform.GetComponent<Animator>();
            _itemAnimators = new List<Animator>();
            if (status == 2)
            {
                receivedTrf.gameObject.SetActive(true);
                iconTrf.gameObject.SetActive(false);
                //以领取idle
                _animator.CrossFade("Show_idle", 0);
            }
            else
            {
                receivedTrf.gameObject.SetActive(false);
                iconTrf.gameObject.SetActive(true);
                XItemUtility.InitItemsUI(iconTrf, rewards.Items, iconTrf.Find("rushPassFreeCell"),GetItemDescribe);
                _animator.CrossFade("Show_idle", 0);
                for (var i = 0; i < iconTrf.childCount; i++)
                {
                    var animator = iconTrf.GetChild(i).Find("StandardType").GetComponent<Animator>();
                    _itemAnimators.Add(animator);
                }

                for (var i = 0; i < _itemAnimators.Count; i++)
                {
                    //金币idle
                    _itemAnimators[i].Play("idle", 0);
                }
            }
        }

        private string GetItemDescribe(Item inItem)
        {
            switch (inItem.Type)
            {
                case Item.Types.Type.Coin:
                    return ((long) inItem.Coin.Amount).GetAbbreviationFormat(1);
                default:
                    return  XItemUtility.GetItemDefaultDescText(inItem);
            }
        }

        
        /// <summary>
        /// 发光特效
        /// </summary>
        public void ReachEffect()
        {
            for (var i = 0; i < _itemAnimators.Count; i++)
            {
                //待领取idle
                _itemAnimators[i].Play("collect", 0);
            }
        }


        /// <summary>
        /// 领取展示
        /// </summary>
        public void Collect()
        {
            //打勾
            iconTrf.gameObject.SetActive(false);
            receivedTrf.gameObject.SetActive(true);
            _animator.Play("show", 0);
        }
    }


    /// <summary>
    /// 付费一组items  三种状态  已解锁  未解锁  已领取
    /// </summary>
    public class RushPassPaidRewardBoxCell : View
    {
        [ComponentBinder("Icon")] public Transform iconTrf;
        [ComponentBinder("Received")] public Transform receivedTrf;
        [ComponentBinder("Icon/PaidItem")] public Transform paidItem;

        public List<RushPassPaidRewardItemCell> cells;

        private Animator _animator;

        /// <summary>
        /// 初始化付费奖励  items
        /// </summary>
        /// <param name="rewards">items 数据 </param>
        /// <param name="isUnlock"> 是否解锁（付费） </param>
        public void InitializeBoxCell(Reward rewards, bool isUnlock, uint status)
        {
            _animator = transform.GetComponent<Animator>();
            cells = new List<RushPassPaidRewardItemCell>(rewards.Items.Count);
            for (int i = 0; i < rewards.Items.count; i++)
            {
                if (i >= 1)
                {
                    var o = GameObject.Instantiate(paidItem, iconTrf);
                    cells.Add(AddChild<RushPassPaidRewardItemCell>(o));
                }
                else
                {
                    cells.Add(AddChild<RushPassPaidRewardItemCell>(paidItem));
                }

                cells[i].InitializeItemCell(rewards.Items.array[i], isUnlock, (int) status);
            }


            if (isUnlock)
            {
                receivedTrf.gameObject.SetActive(status == 2);
                iconTrf.gameObject.SetActive(status != 2);
                _animator.CrossFade("Show_idle", 0);
            }
            else
            {
                receivedTrf.gameObject.SetActive(false);
                iconTrf.gameObject.SetActive(true);
                _animator.CrossFade("Show_idle", 0);
            }

            receivedTrf.gameObject.SetActive(isUnlock && status == 2);
            iconTrf.gameObject.SetActive(status != 2);
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void UnLock()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].UnLock();
            }
        }


        /// <summary>
        /// 发光
        /// </summary>
        public void ReachEffect()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].ReachEffect();
            }
        }


        /// <summary>
        /// 领取
        /// </summary>
        public void Collect()
        {
            receivedTrf.gameObject.SetActive(true);
            _animator.Play("show", 0);
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].Collect();
            }
        }

        public void LockCollect()
        {
            for (int i = 0; i < cells.Count; i++)
            {
                cells[i].LockCollect();
            }
        }
    }


    /// <summary>
    /// 付费的单个item
    /// </summary>
    public class RushPassPaidRewardItemCell : View
    {
        [ComponentBinder("Received")] public Transform receivedTrf;

        private bool _isUnlock;
        private int _status;
        public Animator animator;

        public Animator receiveAnimator;

        public void InitializeItemCell(Item item, bool isUnlock, int status)
        {
            _status = status;
            _isUnlock = isUnlock;
            animator = transform.Find("StandardType").GetComponent<Animator>();
            receiveAnimator = receivedTrf.GetComponent<Animator>();
            XItemUtility.InitItemUI(this.transform, item,GetItemDescribe);
            receivedTrf.gameObject.SetActive(!isUnlock);

            if (isUnlock)
            {
                animator.Play("idle", 0);
            }
            else
            {
                animator.Play("Lock", 0);
            }
        }
        
        
        private string GetItemDescribe(Item inItem)
        {
            switch (inItem.Type)
            {
                case Item.Types.Type.Coin: 
                    return ((long) inItem.Coin.Amount).GetAbbreviationFormat(1);
                default:
                    return  XItemUtility.GetItemDefaultDescText(inItem);
            }
        }
        /// <summary>
        /// 解锁动画
        /// </summary>
        public void UnLock()
        {
            XUtility.PlayAnimation(receiveAnimator, "Unlock", () =>
            {
                receivedTrf.gameObject.SetActive(false);
                if (_status == 1)
                {
                    animator.Play("collect", 0);
                }
                else
                {
                    animator.Play("idel", 0);
                }
            });
        }


        public void ReachEffect()
        {
            animator.Play("collect", 0);
        }


        /// <summary>
        /// 领取
        /// </summary>
        public void Collect()
        {
            Hide();
        }

        public void LockCollect()
        {
            animator.Play("Lock_Show", 0);
        }
    }


    /// <summary>
    /// 进度节点
    /// </summary>
    public class ScheduleCell : View
    {
        [ComponentBinder("ST_D")] public Transform dTrf;
        [ComponentBinder("ST_H")] public Transform hTrf;

        private Animator _animator;

        public void InitializeScheduleCell(bool isCollect)
        {
            _animator = hTrf.GetComponent<Animator>();
            dTrf.gameObject.SetActive(!isCollect);
            hTrf.gameObject.SetActive(isCollect);
        }

        public void Collect()
        {
            dTrf.gameObject.SetActive(false);
            hTrf.gameObject.SetActive(true);
            _animator.CrossFade("show", 0);
        }
    }
}