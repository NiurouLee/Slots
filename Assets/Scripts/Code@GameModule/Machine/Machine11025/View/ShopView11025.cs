using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Audio;
using DragonU3DSDK.Network.API.ILProtocol;
using Tool;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GameModule
{
    public enum BtnType11025
    {
        Box,
        ChangePage,
        CloseShop,
        OpenShop,
    }
    public enum MoveDirection
    {
        Left,
        Right,
    }
    public class BoxPageView11025 : TransformHolder
    {
        private float pageMoveTime = 0.5f;
        private float boxPageDistanceX = 6;
        private Vector3 leftPagePos;
        private Vector3 showPagePos;
        private Vector3 rightPagePos;
        private int maxBoxIndex = 9;
        private List<BoxView11025> pageBoxList;
        public ChameleonGameResultExtraInfo.Types.Shop.Types.ShopTable nowPageData;
        public BoxPageView11025(Transform inTransform) : base(inTransform)
        {
            showPagePos = transform.localPosition;
            leftPagePos = showPagePos;
            leftPagePos.x -= boxPageDistanceX;
            rightPagePos = showPagePos;
            rightPagePos.x += boxPageDistanceX;
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            pageBoxList = new List<BoxView11025>();
            for (var j = 0; j < maxBoxIndex; j++)
            {
                var box = new BoxView11025(transform.Find("RewardIcon" + (j+1)));
                box.Initialize(context);
                pageBoxList.Add(box);
            }
        }

        public bool ReadyToMap()
        {
            return nowPageData.SuperReady;
        }

        public bool CompleteMap()
        {
            return nowPageData.SuperTriggered;
        }
        public void SetPageData(ChameleonGameResultExtraInfo.Types.Shop.Types.ShopTable pageData)
        {
            nowPageData = pageData;
            for (var i = 0; i < maxBoxIndex; i++)
            {
                GetBox(i).SetBoxData(nowPageData.Items[i],nowPageData.Available);
            }
        }
        public void RefreshPage()
        {
            for (var i = 0; i < maxBoxIndex; i++)
            {
                GetBox(i).RefreshAnimator();
                GetBox(i).RefreshText();
            }
        }

        public void InitPosition()
        {
            transform.DOKill();
            transform.localPosition = showPagePos;
        }
        public BoxView11025 GetBox(int boxIndex)
        {
            return pageBoxList[boxIndex];
        }

        public void MoveOutAndHide(MoveDirection moveDirection)
        {
            var targetPos = moveDirection == MoveDirection.Left ? rightPagePos : leftPagePos;
            transform.DOKill();
            transform.localPosition = showPagePos;
            transform.DOLocalMoveX(targetPos.x, pageMoveTime).OnComplete(() =>
            {
                transform.gameObject.SetActive(false);
            });
        }

        public void MoveInAndShow(MoveDirection moveDirection)
        {
            var startPos = moveDirection == MoveDirection.Left ? leftPagePos : rightPagePos;
            transform.DOKill();
            transform.localPosition = startPos;
            transform.gameObject.SetActive(true);
            float showTime = 0;
            for (var i = 0; i < maxBoxIndex; i++)
            {
                var tempBox = GetBox(i);
                tempBox.Hide();
                context.WaitSeconds(showTime, () =>
                {
                    tempBox.PerformShow();
                });
                showTime += 0.05f;
            }
            transform.DOLocalMoveX(showPagePos.x, pageMoveTime);
        }

    }
    public class ShopView11025:TransformHolder
    {
        [ComponentBinder("Root/Explain")] 
        protected Animator explainAnimator;
        [ComponentBinder("Root/quantity/QuantityText")]
        protected TextMesh coinsText;
        [ComponentBinder("Root/PromptPoint")]
        protected Transform pointGroup;
        [ComponentBinder("Root/BtnGrop")]
        protected Animator btnGroup;
        [ComponentBinder("Root/BtnGrop/RightBtn")]
        protected Transform btnLeft;
        [ComponentBinder("Root/BtnGrop/LeftBtn")]
        protected Transform btnRight;
        [ComponentBinder("Root/Reward")]
        protected Transform rewardGroup;
        [ComponentBinder("Root/completePrizeDiscribe")]
        protected Animator completePrizeDescribe;
        [ComponentBinder("Root/CloseBtn")]
        protected Transform closeBtn;
        protected Animator shopAnimator;
        private List<Transform> completePrizeDescribeBackList;
        private List<BoxPageView11025> rewardPageList;

        private List<Transform> explainList1;
        private List<Transform> explainList2;
        // private List<List<BoxView11025>> rewardBoxList;
        public bool isOpen;
        private int pageIndex;
        private int playingPageIndex;
        private int maxPageIndex = 4;
        private int maxBoxIndex = 9;
        private float pageMoveTime = 0.5f;
        private bool btnEnableClick;
        public List<Transform> boxBtnList;
        public ulong nowCoins;
        public ChameleonGameResultExtraInfo.Types.Shop nowShopData;
        private ExtraState11025 extraState;
        private BetState betState;
        private Sequence changeExplainSequence;
        
        public ShopView11025(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            shopAnimator = transform.GetComponent<Animator>();
            btnLeft.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickLeft);
            btnRight.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickRight);
            closeBtn.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickClose);
        }
        public void InitAfterBindingContext()
        {
            extraState = context.state.Get<ExtraState11025>();
            betState = context.state.Get<BetState>();
            nowCoins = 0;
            // nowShopData = extraState.GetShopData();
            
            pageIndex = 0;
            rewardPageList = new List<BoxPageView11025>();
            completePrizeDescribeBackList = new List<Transform>();
            explainList1 = new List<Transform>();
            explainList2 = new List<Transform>();
            for (var i = 0; i < maxPageIndex; i++)
            {
                var rewardPage = new BoxPageView11025(rewardGroup.Find("Reward" + (i + 1)));
                rewardPage.Initialize(context);
                rewardPageList.Add(rewardPage);
                
                completePrizeDescribeBackList.Add(completePrizeDescribe.transform.Find("completePrizeDiscribeIcon/completePrizeDiscribeIcon"+(i+1)));
                explainList1.Add(explainAnimator.transform.Find("Explain1/Explain1_"+(i+1)));
                explainList2.Add(explainAnimator.transform.Find("Explain2/Explain2_"+(i+1)));
            }
            boxBtnList = new List<Transform>();
            for (var i = 0; i < maxBoxIndex; i++)
            {
                var localI = i;
                var btnObject = new GameObject("BoxBtn"+(i+1));
                btnObject.AddComponent<Transform>();
                btnObject.AddComponent<BoxCollider2D>();
                btnObject.GetComponent<BoxCollider2D>().size = new Vector2(1.3f,1.2f);
                btnObject.transform.SetParent(btnGroup.transform,false);
                btnObject.transform.position = GetPage(0).GetBox(i).transform.position;
                var pointerEventCustomHandler = btnObject.AddComponent<PointerEventCustomHandler>();
                pointerEventCustomHandler.BindingPointerClick((pointerData) =>
                {
                    DealWithBoxClick(localI);
                });
                boxBtnList.Add(btnObject.transform);
            }
            isOpen = false;
            transform.gameObject.SetActive(false);
            btnEnableClick = false;

            var coroutine = IEnumeratorTool.instance.StartCoroutine(StartShakeBox());
            context.AddCoroutine(coroutine);
        }
        
        public async Task CloseShop()
        {
            if (isOpen)
            {
                AudioUtil.Instance.PlayAudioFx("Store_Close");
                SetBtnEnable(false);
                isOpen =false;
                await XUtility.PlayAnimationAsync(shopAnimator,"Close");
                context.view.Get<MachineSystemWidgetView>().SetActive(true);
            }
        }
        public async Task OpenShop()
        {
            if (!isOpen)
            {
                AudioUtil.Instance.PauseMusic();
                AudioUtil.Instance.PlayAudioFx("Store_Open");
                context.view.Get<MachineSystemWidgetView>().SetActive(false);
                context.view.Get<ControlPanel>().ShowSpinButton(false);
                context.view.Get<ControlPanel>().UpdateControlPanelState(false, true);
                SetBtnEnable(false);
                isOpen =true;
                Show();
                XUtility.PlayAnimation(shopAnimator,"Open");
                RefreshAll();
                context.WaitNFrame(5, () =>
                {
                    GetPage(pageIndex).RefreshPage();
                });
                await context.WaitSeconds(0.933f);
            }
        }
        public BoxPageView11025 GetPage(int pageIndex)
        {
            return rewardPageList[pageIndex];
        }
        public async void DealWithBoxClick(int boxIndex)
        {
            if (!btnEnableClick)
            {
                return;
            }
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11025.Box,pageIndex,boxIndex);
        }

        public bool CheckMapGame()
        {
            if (GetPage(playingPageIndex).ReadyToMap())
            {
                return true;
            }
            return false;
        }
        
        public async Task PerformCollectMapGame()
        {
            AudioUtil.Instance.PlayAudioFx("Store_Game_Boom");
            await XUtility.PlayAnimationAsync(completePrizeDescribe,"UnLock");
        }
        public void SetBtnEnable(bool enable)
        {
            btnEnableClick = enable;
            // btnLeft.gameObject.GetComponent<BoxCollider2D>().enabled = enable;
            // btnRight.gameObject.GetComponent<BoxCollider2D>().enabled = enable;
            if (enable)
            {
                XUtility.PlayAnimation(btnGroup,"Open");
            }
            else
            {
                XUtility.PlayAnimation(btnGroup,"Close");
            }
        }
        public async void ClickLeft(PointerEventData clickPoint)
        {
            if (!btnEnableClick)
            {
                return;
            }
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11025.ChangePage,MoveDirection.Left);
        }
        public async void ClickRight(PointerEventData clickPoint)
        {
            if (!btnEnableClick)
            {
                return;
            }
            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11025.ChangePage,MoveDirection.Right);
        }

        public async void ClickClose(PointerEventData clickPoint)
        {
            if (!btnEnableClick)
            {
                return;
            }

            context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK, BtnType11025.CloseShop);
        }

        public async Task PageMove(MoveDirection direction)
        {
            AudioUtil.Instance.PlayAudioFx("Store_Turn Pages");
            SetBtnEnable(false);
            var oldPage = GetPage(pageIndex);
            if (direction == MoveDirection.Left)
            {
                pageIndex--;
                if (pageIndex < 0)
                {
                    pageIndex += maxPageIndex;
                }
            }
            else
            {
                pageIndex++;
                if (pageIndex >= maxPageIndex)
                {
                    pageIndex -= maxPageIndex;
                }
            }
            var newPage = GetPage(pageIndex);
            oldPage.MoveOutAndHide(direction);
            newPage.MoveInAndShow(direction);
            PerformChangeExplain();
            RefreshPoint();
            RefreshDescribe();
            await context.WaitSeconds(pageMoveTime);
            SetBtnEnable(true);
        }

        public void SetShopData(ChameleonGameResultExtraInfo.Types.Shop inShopData)
        {
            nowCoins = inShopData.Credits;
            nowShopData = inShopData;
            playingPageIndex = extraState.GetPlayingPageIndex();
            for (var i = 0; i < maxPageIndex; i++)
            {
                GetPage(i).SetPageData(nowShopData.Tables[i]);
            }
        }
        
        public IEnumerator StartShakeBox () {
            while (true)
            {
                ShakeBox();
                yield return new WaitForSeconds(2);
            }
        }

        public static List<List<int>> RandomShakeList = new List<List<int>>()
        {
            new List<int>() { }
        };
        public void ShakeBox()
        {
            if (isOpen && nowShopData != null && playingPageIndex == pageIndex && btnEnableClick)
            {
                var boxList = new List<BoxView11025>();
                var page = GetPage(playingPageIndex);
                for (var i = 0; i < maxBoxIndex; i++)
                {
                    var box = page.GetBox(i);
                    if (!box.GetBoxData().Open)
                    {
                        boxList.Add(box);
                    }
                }

                if (boxList.Count > 0)
                {
                    var shakeCount = Random.Range(0, boxList.Count) + 1;
                    for (var i = 0; i < shakeCount; i++)
                    {
                        var shakeIndex = Random.Range(0, boxList.Count);
                        boxList[shakeIndex].PerformShake();
                        boxList.RemoveAt(shakeIndex);
                    }
                }
            }
        }

        public async void PerformChangeExplain()
        {
            if (changeExplainSequence != null)
            {
                changeExplainSequence.Kill();
            }
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                XUtility.PlayAnimation(explainAnimator,"Close");
            });
            sequence.AppendInterval(pageMoveTime/2);
            sequence.AppendCallback(RefreshExplain);
            sequence.AppendCallback(() =>
            {
                XUtility.PlayAnimation(explainAnimator,"Open");
            });
            sequence.target = context.transform;
            changeExplainSequence = sequence;
        }
        public void RefreshExplain()
        {
            for (var i = 0; i < maxPageIndex; i++)
            {
                explainList1[i].gameObject.SetActive(i == pageIndex);
                explainList2[i].gameObject.SetActive(i == pageIndex);
            }
        }
        public void RefreshPoint()
        {
            for (var i = 0; i < maxPageIndex; i++)
            {
                pointGroup.Find((i+1).ToString()).gameObject.SetActive(i == pageIndex);
            }
        }
        public void RefreshCoins()
        {
            coinsText.text = nowCoins.GetCommaFormat();
        }

        public void RefreshDescribe()
        {
            for (var i = 0; i < maxPageIndex; i++)
            {
                completePrizeDescribeBackList[i].gameObject.SetActive(i == pageIndex);
            }

            if (GetPage(pageIndex).CompleteMap())
            {
                XUtility.PlayAnimation(completePrizeDescribe,"UnLockIdle");
            }
            else
            {
                XUtility.PlayAnimation(completePrizeDescribe,"Lock");
            }
        }

        public void RefreshPage()
        {
            for (var i = 0; i < maxPageIndex; i++)
            {
                GetPage(i).InitPosition();
                if (i == pageIndex)
                {
                    GetPage(i).Show();
                }
                else
                {
                    GetPage(i).Hide();
                }
                GetPage(i).RefreshPage();
            }
        }
        public void RefreshAll()
        {
            RefreshDescribe();
            RefreshPage();
            RefreshCoins();
            RefreshPoint();
            RefreshExplain();
        }

        public void SetPageToPlayingPage()
        {
            pageIndex = playingPageIndex;
        }
    }
}