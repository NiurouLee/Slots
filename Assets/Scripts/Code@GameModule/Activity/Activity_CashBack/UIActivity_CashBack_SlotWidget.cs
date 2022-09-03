// using DG.Tweening;
// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// namespace GameModule
// {
//     [AssetAddress("UICashBackTimeInSlot")]
//     public class UIActivity_CashBack_SlotWidget : View<UIActivity_CashBack_SlotWidgetController>, IOnContextDestroy
//     {
//         [ComponentBinder("Root")]
//         public RectTransform rootRect;
//
//         [ComponentBinder("Root")]
//         public RectTransform dragLayer;
//
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup")]
//         public Animator animatorBubble;
//
//         [ComponentBinder("Root/StateGroup/EnableTimerStateButton/Icon")]
//         public Animator animatorEnableIcon;
//
//         [ComponentBinder("Root/StateGroup")]
//         public RectTransform transformStateGroup;
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup")]
//         public RectTransform transformBubble;
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint")]
//         public RectTransform transformFit;
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/EnableState")]
//         public RectTransform transformEnableBubbleContent;
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/DisableGroup")]
//         public RectTransform transformDisableBubbleContent;
//
//         [ComponentBinder("Root/StateGroup/BubbleGroup/FitPoint/StateGroup/EnableState/IntegralGroup/IntegralText")]
//         public TMP_Text tmpTextCount;
//
//
//         [ComponentBinder("Root/StateGroup/DisableStateButton")]
//         public Button buttonDisabled;
//
//         [ComponentBinder("Root/StateGroup/EnableTimerStateButton")]
//         public Button buttonEnabled;
//
//         [ComponentBinder("Root/StateGroup/EnableMaxStateButton")]
//         public Button buttonMax;
//
//
//         [ComponentBinder("Root/StateGroup/EnableTimerStateButton/TimerGroup/TimerText")]
//         public TMP_Text tmpTextTimer;
//
//         [ComponentBinder("Root/StateGroup/EnableMaxStateButton/Icon/MaxIconw")]
//         public RectTransform transformMaxIcon;
//
//         public enum State { Disabled, Enabled, Max }
//
//         private State _state;
//         private long _coin;
//         private Tween _tween;
//         private Activity_CashBack _activity;
//
//         public UIActivity_CashBack_SlotWidget(string address) : base(address) { }
//
//
//         public Vector2 GetWidgetSize() { return rootRect.sizeDelta; }
//
//         public void UpdateDirection(bool isLeft)
//         {
//             var rotation = Quaternion.Euler(0, isLeft ? 180 : 0, 0);
//             var scale = new Vector3(isLeft ? -1 : 1, 1, 1);
//             transformStateGroup.localScale = scale;
//             transformFit.localScale = scale;
//             transformMaxIcon.localRotation = rotation;
//             tmpTextTimer.transform.localRotation = rotation;
//         }
//
//         public void SetState(State state, bool force = false)
//         {
//             if (state == _state && force == false) { return; }
//             _state = state;
//
//             switch (state)
//             {
//                 case State.Disabled:
//                     buttonEnabled.gameObject.SetActive(false);
//                     buttonDisabled.gameObject.SetActive(true);
//                     buttonMax.gameObject.SetActive(false);
//                     transformEnableBubbleContent.gameObject.SetActive(false);
//                     transformDisableBubbleContent.gameObject.SetActive(true);
//                     break;
//                 case State.Enabled:
//                     buttonEnabled.gameObject.SetActive(true);
//                     buttonDisabled.gameObject.SetActive(false);
//                     buttonMax.gameObject.SetActive(false);
//                     transformEnableBubbleContent.gameObject.SetActive(true);
//                     transformDisableBubbleContent.gameObject.SetActive(false);
//                     break;
//                 case State.Max:
//                     buttonEnabled.gameObject.SetActive(false);
//                     buttonDisabled.gameObject.SetActive(false);
//                     buttonMax.gameObject.SetActive(true);
//                     transformEnableBubbleContent.gameObject.SetActive(true);
//                     transformDisableBubbleContent.gameObject.SetActive(false);
//                     break;
//             }
//         }
//
//         public void Set(Activity_CashBack activity)
//         {
//             _activity = activity;
//             if (_activity == null || _activity.buff == null)
//             {
//                 SetState(State.Disabled, true);
//                 return;
//             }
//
//             UpdateTimer();
//
//             var buff = _activity.buff;
//             XDebug.Log($"11111111111111111 in Set(), back coin:{buff.BackCoin}, return limit:{buff.ReturnLimited}");
//             // XDebug.Log($"11111111111111111 in Set(), time left seconds:{timeLeftTimeSpan.TotalSeconds}, time string:{timeLeftString}");
//
//             SetCoin(buff.BackCoin);
//             if (buff.BackCoin >= buff.ReturnLimited)
//             {
//                 SetState(State.Max);
//                 return;
//             }
//
//             SetState(State.Enabled);
//         }
//
//         private void SetCoin(long coin)
//         {
//             _tween?.Kill();
//
//             if (coin == _coin) { return; }
//
//             if (animatorEnableIcon.enabled == false) { animatorEnableIcon.enabled = true; }
//             animatorEnableIcon.Play("collect", 0, 0);
//
//             _tween = DOTween.To(() => { return _coin; },
//             (c) =>
//             {
//                 _coin = c;
//                 tmpTextCount.text = _coin.GetCommaFormat();
//             }, coin, 2.0f);
//
//             _tween.onComplete = () =>
//             {
//                 tmpTextCount.text = coin.GetCommaFormat();
//                 _coin = coin;
//             };
//         }
//
//         private void UpdateTimer()
//         {
//             if (_activity == null || _activity.buff == null)
//             {
//                 return;
//             }
//
//             // XDebug.Log($"11111111111111111 in UpdateTimer(), time left seconds:{timeLeftTimeSpan.TotalSeconds}, time string:{timeLeftString}");
//             var leftTime = XUtility.GetLeftTime(_activity.buff.Expire * 1000);
//             if (leftTime <= 0)
//             {
//                 SetState(State.Disabled);
//             }
//             else
//             {
//                 tmpTextTimer.text = XUtility.GetTimeText(leftTime);
//             }
//         }
//
//         public void DoBubble()
//         {
//             var stateInfo = animatorBubble.GetCurrentAnimatorStateInfo(0);
//             if (stateInfo.IsName("Close"))
//             {
//                 animatorBubble.Play("Open");
//             }
//             else
//             {
//                 animatorBubble.Play("Close");
//             }
//         }
//
//         public void Update()
//         {
//             UpdateTimer();
//         }
//
//         public void OnContextDestroy()
//         {
//             Destroy();
//         }
//     }
//
//     public class UIActivity_CashBack_SlotWidgetController : ViewController<UIActivity_CashBack_SlotWidget>
//     {
//         protected bool isDrag;
//         protected Vector2 mouseOffset;
//         protected Vector2 startPos;
//         protected bool isLeft = true;
//         private Activity_CashBack _activity;
//
//         public override void OnViewEnabled()
//         {
//             base.OnViewEnabled();
//             InitializePosition();
//         }
//
//         public override async void OnViewDidLoad()
//         {
//             base.OnViewDidLoad();
//
//             var dragDropEventCustomHandler = view.dragLayer.gameObject.AddComponent<DragDropEventCustomHandler>();
//             dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
//             dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
//             dragDropEventCustomHandler.BindingDragAction(OnDrag);
//
//             view.animatorBubble.Play("Close", 0, 1);
//             view.tmpTextCount.text = "0";
//             view.tmpTextTimer.text = "00:00:00";
//
//             _activity = extraData as Activity_CashBack;
//             view.Set(_activity);
//             EnableUpdate(1);
//             await _activity.RequestCGetActivityUserDataAsync();
//         }
//
//         protected override void SubscribeEvents()
//         {
//             base.SubscribeEvents();
//             view.buttonDisabled.onClick.AddListener(OnButtonDisabled);
//             view.buttonEnabled.onClick.AddListener(OnButtonEnabled);
//             view.buttonMax.onClick.AddListener(OnButtonMax);
//             SubscribeEvent<EventCashBackUserDateChanged>(OnEventCashBackUserDateChanged);
//         }
//
//         private void OnEventCashBackUserDateChanged(EventCashBackUserDateChanged obj)
//         {
//             if (_activity != null && _activity.IsValid()
//             && (_activity.isStoreCommodityExpired == false ||  (_activity.buff != null && XUtility.GetLeftTime(_activity.buff.Expire*1000) > 0)))
//             {
//                 view.Show();
//                 view.Set(_activity);
//             }
//             else
//             {
//                 view.Hide();
//             }
//         }
//
//         private void OnButtonMax()
//         {
//             if (isDrag == false)
//             {
//                 view.transformEnableBubbleContent.gameObject.SetActive(true);
//                 view.transformDisableBubbleContent.gameObject.SetActive(false);
//                 view.DoBubble();
//             }
//         }
//
//         private void OnButtonEnabled()
//         {
//             if (isDrag == false)
//             {
//                 view.transformEnableBubbleContent.gameObject.SetActive(true);
//                 view.transformDisableBubbleContent.gameObject.SetActive(false);
//                 view.DoBubble();
//             }
//         }
//
//         private void OnButtonDisabled()
//         {
//             if (isDrag == false)
//             {
//                 view.transformEnableBubbleContent.gameObject.SetActive(false);
//                 view.transformDisableBubbleContent.gameObject.SetActive(true);
//                 view.DoBubble();
//             }
//         }
//
//         public override void Update()
//         {
//             base.Update();
//             view.Update();
//         }
//
//         private void InitializePosition()
//         {
//             Vector2 contentSize = view.GetWidgetSize();
//             var safeRect = GetScreenSafeRect(contentSize);
//             var localPosition = view.transform.localPosition;
//             var safePosition = GetSafePosition(safeRect, localPosition);
//             safePosition.x *= -1;
//             view.transform.localPosition = safePosition;
//             view.UpdateDirection(false);
//         }
//
//         private void OnBeginDrag(PointerEventData pointerEventData)
//         {
//             isDrag = true;
//             RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent, pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
//             mouseOffset = (Vector2)(view.transform.localPosition) - startPos;
//         }
//
//         private void OnDrag(PointerEventData pointerEventData)
//         {
//             if (isDrag)
//             {
//                 if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent,
//                     pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
//                     return;
//                 view.transform.localPosition = localCursor + mouseOffset;
//             }
//         }
//
//         private void OnEndDrag(PointerEventData pointerEventData)
//         {
//             if (!isDrag) return;
//             isDrag = false;
//             Vector2 contentSize = view.GetWidgetSize();
//             var safeRect = GetScreenSafeRect(contentSize);
//             var localPosition = view.transform.localPosition;
//             view.transform.localPosition = GetSafePosition(safeRect, localPosition);
//             view.UpdateDirection(isLeft);
//         }
//
//         public Rect GetScreenSafeRect(Vector2 contentSize)
//         {
//             bool isPortrait = ViewManager.Instance.IsPortrait;
//
//             var referenceResolution = ViewResolution.referenceResolutionLandscape;
//
//             if (isPortrait)
//             {
//                 referenceResolution = ViewResolution.referenceResolutionPortrait;
//             }
//
//             var controlPanelHeight = MachineConstant.controlPanelHeight;
//
//             var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;
//
//             float screenCutOut = 0;
//
//             if (isPortrait)
//             {
//                 controlPanelHeight = MachineConstant.controlPanelVHeight;
//                 machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
//             }
//
// #if UNITY_IOS
//             if (!isPortrait)
//             {
//                 screenCutOut = MachineConstant.widgetOffset;
//             }
// #endif
//
//             var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f + screenCutOut,
//                 -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
//                 referenceResolution.x - contentSize.x - screenCutOut * 2, referenceResolution.y - machineUIHeight - contentSize.y);
//
//             return safeRect;
//         }
//
//         public Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
//         {
//             var targetY = localPosition.y;
//
//             if (localPosition.y > safeRect.yMax)
//             {
//                 targetY = safeRect.yMax;
//             }
//             else if (localPosition.y < safeRect.yMin)
//             {
//                 targetY = safeRect.yMin;
//             }
//
//             var targetX = localPosition.x;
//
//             if (localPosition.x <= 0)
//             {
//                 isLeft = true;
//                 targetX = safeRect.xMin;
//             }
//             else if (localPosition.x > 0)
//             {
//                 isLeft = false;
//                 targetX = safeRect.xMax;
//             }
//
//             return new Vector2(targetX, targetY);
//         }
//     }
// }