// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/20/17:17
// Ver : 1.0.0
// Description : CashbackWidget.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICashBackTimeInSlot")]
    public class CashBackMachineWidget : View<CashBackMachineWidgetViewController>, IOnContextDestroy
    {
        [ComponentBinder("Root")]
        public RectTransform rootRect;

        [ComponentBinder("Root")]
        public RectTransform dragLayer;


        [ComponentBinder("Root/StateGroup/BubbleGroup")]
        public Animator animatorBubble;

        [ComponentBinder("Root/StateGroup/EnableTimerStateButton/Icon")]
        public Animator animatorEnableIcon;

        [ComponentBinder("Root/StateGroup")]
        public RectTransform transformStateGroup;

        [ComponentBinder("Root/StateGroup/BubbleGroup")]
        public RectTransform transformBubble;

        [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint")]
        public RectTransform transformFit;

        [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/EnableState")]
        public RectTransform transformEnableBubbleContent;

        [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/DisableGroup")]
        public RectTransform transformDisableBubbleContent;

        [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/EnableState/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextCount;


        [ComponentBinder("Root/StateGroup/DisableStateButton")]
        public Button buttonDisabled; 
        
        [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/EnableState/Text")]
        public TMP_Text cashBackText;

        [ComponentBinder("Root/StateGroup/EnableTimerStateButton")]
        public Button buttonEnabled;

        [ComponentBinder("Root/StateGroup/EnableMaxStateButton")]
        public Button buttonMax;


        [ComponentBinder("Root/StateGroup/EnableTimerStateButton/TimerGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("Root/StateGroup/EnableMaxStateButton/Icon/MaxIconw")]
        public RectTransform transformMaxIcon;

        public enum State { Close, Disabled, Enabled, Max }

        private State _state;
        private long _coin;
        private Tween _tween;
        private Activity_CashBack _activity;

        public CashBackMachineWidget(string address) : base(address) { }


        public Vector2 GetWidgetSize()
        {
            return rootRect.sizeDelta;
        }

        public void UpdateDirection(bool isLeft)
        {
            var rotation = Quaternion.Euler(0, isLeft ? 180 : 0, 0);
            var scale = new Vector3(isLeft ? -1 : 1, 1, 1);
            transformStateGroup.localScale = scale;
            transformFit.localScale = scale;
            transformMaxIcon.localRotation = rotation;
            tmpTextTimer.transform.localRotation = rotation;
        }

        public void SetState(State state, bool force = false)
        {
            if (state == _state && force == false) { return; }
            _state = state;

            switch (state)
            {
                case State.Close:
                    transform.gameObject.SetActive(false);
                    break;
                case State.Disabled:
                    buttonEnabled.gameObject.SetActive(false);
                    buttonDisabled.gameObject.SetActive(true);
                    buttonMax.gameObject.SetActive(false);
                    transformEnableBubbleContent.gameObject.SetActive(false);
                    transformDisableBubbleContent.gameObject.SetActive(true);
                    break;
                case State.Enabled:
                    buttonEnabled.gameObject.SetActive(true);
                    buttonDisabled.gameObject.SetActive(false);
                    buttonMax.gameObject.SetActive(false);
                    transformEnableBubbleContent.gameObject.SetActive(true);
                    transformDisableBubbleContent.gameObject.SetActive(false);
                    break;
                case State.Max:
                    buttonEnabled.gameObject.SetActive(false);
                    buttonDisabled.gameObject.SetActive(false);
                    buttonMax.gameObject.SetActive(true);
                    transformEnableBubbleContent.gameObject.SetActive(true);
                    transformDisableBubbleContent.gameObject.SetActive(false);
                    break;
            }
        }
 
        public void SetCoin(long coin, bool animation = true)
        {
            _tween?.Kill();

            if (coin == _coin) { return; }

            if (animatorEnableIcon)
            {
                if (animatorEnableIcon.enabled == false)
                {
                    animatorEnableIcon.enabled = true;
                }
                
                animatorEnableIcon.Play("collect", 0, 0);

                _tween = DOTween.To(() => { return _coin; },
                    (c) =>
                    {
                        _coin = c;
                        tmpTextCount.text = _coin.GetCommaFormat();
                    }, coin, 2.0f);

                _tween.onComplete = () =>
                {
                    tmpTextCount.text = coin.GetCommaFormat();
                    _coin = coin;
                };
            }
            else
            {
                tmpTextCount.text = coin.GetCommaFormat();
                _coin = coin;
            }
        }

        public void UpdateBuffText(CashBackBuff buff)
        {
            if (buff != null && buff.GetRewardMode() == 1)
            {
                cashBackText.text = "CASHBACK TOMORROW";
            }
            else
            {
                cashBackText.text = "YOUR CASHBACK";
            }
        }

        public void UpdateTimer(CashBackBuff buff, bool hasActivity)
        {
            // XDebug.Log($"11111111111111111 in UpdateTimer(), time left seconds:{timeLeftTimeSpan.TotalSeconds}, time string:{timeLeftString}");
            float leftTime = 0;
          
            if (buff != null)
            {
                leftTime = buff.GetBuffLeftTimeInSecond();
            }
            
            if (leftTime <= 0)
            {
                SetState(hasActivity ? State.Disabled : State.Close);
            }
            else
            {
                tmpTextTimer.text = XUtility.GetTimeText(leftTime);
            }
        }

        public void DoBubble()
        {
            var stateInfo = animatorBubble.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Close"))
            {
                animatorBubble.Play("Open");
            }
            else
            {
                animatorBubble.Play("Close");
            }
        }
 
        public void OnContextDestroy()
        {
            Destroy();
        }
    }

    public class CashBackMachineWidgetViewController : ViewController<CashBackMachineWidget>
    {
        protected bool isDrag;
        protected Vector2 mouseOffset;
        protected Vector2 startPos;
        protected bool isLeft = true;
        private bool hasCashBackActivity = false;
 
        private BuffController _buffController;
        private ActivityController _activityController;
        private CashBackBuff _currentCashBackBuff;
        
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            
            InitializePosition();
            
            SetBuffViewState();
        }

        public override  void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            _buffController = Client.Get<BuffController>();
            _currentCashBackBuff = _buffController.GetBuff<CashBackBuff>();
            _activityController = Client.Get<ActivityController>();

            var dragDropEventCustomHandler = view.dragLayer.gameObject.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
            dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
            dragDropEventCustomHandler.BindingDragAction(OnDrag);

            view.animatorBubble.Play("Close", 0, 1);
            view.tmpTextCount.text = "0";
            view.tmpTextTimer.text = "00:00:00";
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonDisabled.onClick.AddListener(OnButtonDisabledClicked);
            view.buttonEnabled.onClick.AddListener(OnButtonEnabledClicked);
            view.buttonMax.onClick.AddListener(OnButtonMaxClicked);
            SubscribeEvent<EventBuffDataUpdated>(OnEventBuffDataUpdated);
        }
        
        private void OnEventBuffDataUpdated(EventBuffDataUpdated obj)
        {
            _currentCashBackBuff = Client.Get<BuffController>().GetBuff<CashBackBuff>();
            hasCashBackActivity = _activityController.GetDefaultActivity(ActivityType.CashBack) != null;
            
            SetBuffViewState();
        }

        private void OnButtonMaxClicked()
        {
            if (isDrag == false)
            {
                view.transformEnableBubbleContent.gameObject.SetActive(true);
                view.transformDisableBubbleContent.gameObject.SetActive(false);
                view.DoBubble();
            }
        }

        private async void OnButtonEnabledClicked()
        {
            if (isDrag == false)
            {
                await _currentCashBackBuff.RefreshCashBackBuffAsync();
                view.SetCoin(_currentCashBackBuff.GetCoin(),false);
                view.UpdateBuffText(_currentCashBackBuff);
                view.transformEnableBubbleContent.gameObject.SetActive(true);
                view.transformDisableBubbleContent.gameObject.SetActive(false);
                view.DoBubble();
            }
        }

        private void OnButtonDisabledClicked()
        {
            if (isDrag == false)
            {
                view.transformEnableBubbleContent.gameObject.SetActive(false);
                view.transformDisableBubbleContent.gameObject.SetActive(true);
                view.DoBubble();
            }
        }

        public override void Update()
        {
            view.UpdateTimer(_currentCashBackBuff, hasCashBackActivity);

            if (_currentCashBackBuff == null)
            {
                DisableUpdate();
            }
            else
            {
                if (updateEnabled)
                {
                    EnableUpdate(2);
                }
            }
        }
        
        public void SetBuffViewState()
        {
            if (_currentCashBackBuff == null)
            {
                view.SetState(!hasCashBackActivity ? CashBackMachineWidget.State.Close: CashBackMachineWidget.State.Disabled, true);
                return;
            }

            view.SetCoin(_currentCashBackBuff.GetCoin());
            view.UpdateBuffText(_currentCashBackBuff);
            if (_currentCashBackBuff.IsFull())
            {
                view.SetState(CashBackMachineWidget.State.Max);
                return;
            }

            view.SetState(CashBackMachineWidget.State.Enabled);
            
            if(!updateEnabled)
                EnableUpdate(2);
        }

        private void InitializePosition()
        {
            Vector2 contentSize = view.GetWidgetSize();
            var safeRect = GetScreenSafeRect(contentSize);
            var localPosition = view.transform.localPosition;
            var safePosition = GetSafePosition(safeRect, localPosition);
            safePosition.x *= -1;
            view.transform.localPosition = safePosition;
            view.UpdateDirection(false);
        }

        private void OnBeginDrag(PointerEventData pointerEventData)
        {
            isDrag = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent, pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
            mouseOffset = (Vector2)(view.transform.localPosition) - startPos;
        }

        private void OnDrag(PointerEventData pointerEventData)
        {
            if (isDrag)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent,
                    pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                    return;
                view.transform.localPosition = localCursor + mouseOffset;
            }
        }

        private void OnEndDrag(PointerEventData pointerEventData)
        {
            if (!isDrag) return;
            isDrag = false;
            Vector2 contentSize = view.GetWidgetSize();
            var safeRect = GetScreenSafeRect(contentSize);
            var localPosition = view.transform.localPosition;
            view.transform.localPosition = GetSafePosition(safeRect, localPosition);
            view.UpdateDirection(isLeft);
        }

        public Rect GetScreenSafeRect(Vector2 contentSize)
        {
            bool isPortrait = ViewManager.Instance.IsPortrait;

            var referenceResolution = ViewResolution.referenceResolutionLandscape;

            if (isPortrait)
            {
                referenceResolution = ViewResolution.referenceResolutionPortrait;
            }

            var controlPanelHeight = MachineConstant.controlPanelHeight;

            var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;

            float screenCutOut = 0;

            if (isPortrait)
            {
                controlPanelHeight = MachineConstant.controlPanelVHeight;
                machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
            }

#if UNITY_IOS
            if (!isPortrait)
            {
                screenCutOut = MachineConstant.widgetOffset;
            }
#endif

            var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f + screenCutOut,
                -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
                referenceResolution.x - contentSize.x - screenCutOut * 2, referenceResolution.y - machineUIHeight - contentSize.y);

            return safeRect;
        }

        public Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
        {
            var targetY = localPosition.y;

            if (localPosition.y > safeRect.yMax)
            {
                targetY = safeRect.yMax;
            }
            else if (localPosition.y < safeRect.yMin)
            {
                targetY = safeRect.yMin;
            }

            var targetX = localPosition.x;

            if (localPosition.x <= 0)
            {
                isLeft = true;
                targetX = safeRect.xMin;
            }
            else if (localPosition.x > 0)
            {
                isLeft = false;
                targetX = safeRect.xMax;
            }

            return new Vector2(targetX, targetY);
        }
    }
}