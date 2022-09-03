using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.Rendering;

namespace GameModule
{
    public class ProgressBar11029 : TransformHolder
    {
        [ComponentBinder("Fill")] protected Transform fill;

        [ComponentBinder("LockState")] private Transform _lockState;

        [ComponentBinder("LockState/LockButton")]
        private Transform _lockBtn;

        [ComponentBinder("LockState/TipsGroup")]
        private Transform _lockTips;

        [ComponentBinder("LeftGroup")] private Transform _collectHorseBtn;

        [ComponentBinder("RightPointGroup")] private Transform _MapGameBtn;

        [ComponentBinder("RightPointGroup/PointIcon1")] private Transform _mapGameIcon1;

        [ComponentBinder("RightPointGroup/PointIcon2")] private Transform _mapGameIcon2;
        
        [ComponentBinder("RightPointGroup/PointIcon3")] private Transform _mapGameIcon3;
        
        [ComponentBinder("RightPointGroup/PointIcon4")] private Transform _mapGameIcon4;
        
        [ComponentBinder("RightPointGroup/PointIcon5")] private Transform _mapGameIcon5;
        
        [ComponentBinder("RightPointGroup/PointIcon6")] private Transform _mapGameIcon6;
        
        protected Transform[] _mapGameIconNodes;

        private Animator _animatorCollectHorse;

        private ExtraState11029 _extraState11029;

        private bool _buttonResponseEnabled = true;

        private Animator _animator;

        private Animator _animatorLock;

        private bool _isLockState = true;
        
        private bool _isFeatureLocked = false;

        public ProgressBar11029(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            _animator = transform.GetComponent<Animator>();
            // _animatorCollectHorse = _collectHorseBtn.GetComponent<Animator>();
            _animator.keepAnimatorControllerStateOnDisable = true;
            _animatorLock = _lockState.GetComponent<Animator>();
            
            _lockState.gameObject.SetActive(true);
            
            _animatorLock.keepAnimatorControllerStateOnDisable = true;
            _mapGameIconNodes = new[] {_mapGameIcon1, _mapGameIcon2, _mapGameIcon3,_mapGameIcon4,_mapGameIcon5,_mapGameIcon6};
            // if (_animatorCollectHorse)
            //     _animatorCollectHorse.keepAnimatorControllerStateOnDisable = true;

            var lockAnimator = _lockTips.GetComponent<Animator>();
         
            if (lockAnimator != null)
            {
                lockAnimator.enabled = false;
                _lockTips.transform.SetParent(transform.parent);
                _lockTips.transform.gameObject.SetActive(false);
            }
        }

        public Vector3 GetIntegralPos()
        {
            return _collectHorseBtn.transform.position;
        }

        public async Task TriggerCollectHorse()
        {
            await XUtility.PlayAnimationAsync(_animator, "Trigger");
        }
        
        public async Task OpenCollectHorse()
        {
            AudioUtil.Instance.PlayAudioFx("Map_FlyFull");
            await XUtility.PlayAnimationAsync(_animator, "Open");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            _extraState11029 = context.state.Get<ExtraState11029>();

            var pointerEventCustomHandler = _lockBtn.gameObject.AddComponent<PointerEventCustomHandler>();

            pointerEventCustomHandler.BindingPointerClick(OnUnlockFeatureClicked);

            var pointerMapGameEventCustomHandler = _MapGameBtn.gameObject.AddComponent<PointerEventCustomHandler>();

            pointerMapGameEventCustomHandler.BindingPointerClick(OnMapGameClicked);
        }

        public void ChangeMapRightBtn()
        {
            _extraState11029 = context.state.Get<ExtraState11029>();
            int level = (int) _extraState11029.GetMapLevel();
            int index = 0;
            if (level >= 0 && level < 3)
            {
                index = 0;
            }else if (level >= 3 && level <  7)
            {
                index = 1;
            }else if (level >= 7 && level < 12)
            {
                index = 2;
            }else if (level >= 12 && level < 18)
            {
                index = 3;
            }else if (level >= 18 && level < 25)
            {
                index = 4;
            }else if (level >= 25)
            {
                index = 5;
            }

            for (int i = 0; i < _mapGameIconNodes.Length; i++)
            {
                if (index == i)
                {
                    _mapGameIconNodes[i].transform.gameObject.SetActive(true);
                }
                else
                {
                    _mapGameIconNodes[i].transform.gameObject.SetActive(false);
                }
            }
        }

        public void OnUnlockFeatureClicked(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;

            if (_isFeatureLocked)
            {
                context.DispatchInternalEvent(MachineInternalEvent.EVENT_CLICK_UI_UNLOCK_GAME_FEATURE, 0);
            }
            else
            {
                if (!_lockTips.gameObject.activeSelf)
                {  
                    XUtility.ShowTipAndAutoHide(_lockTips.transform, 3, 0.2f, true, context);
                }
            }
         
        }

        public void OnMapGameClicked(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;
            AudioUtil.Instance.PlayAudioFxOneShot("Map_Open");
            var mapGamePopup11029 = PopUpManager.Instance.ShowPopUp<MapGamePopup11029>("MapGame");
            if (mapGamePopup11029 != null)
            {
                mapGamePopup11029.ShowLevel(false);
                mapGamePopup11029.EnableButton(true);
            }
        }
        
        public void ShowCollectTip()
        {
            if (!_lockTips.gameObject.activeSelf)
            {
                XUtility.ShowTipAndAutoHide(_lockTips.transform, 3, 0.2f, true, context);
            }
        }

        
        public void LockSuperFree(bool isLock, bool playAnimation = true)
        {
            if (_animatorLock == null)
                return;
           
            if (isLock && !_isLockState)
            {
                if (playAnimation)
                {
                    _animatorLock.Play("Unlock");
                    AudioUtil.Instance.PlayAudioFx("Tips_Lock");
                }
                else
                {
                    _animatorLock.Play("Idle");
                }

                _isLockState = true;
            }
            else if (!isLock && _isLockState)
            {
                if (playAnimation)
                {
                    AudioUtil.Instance.PlayAudioFx("Tips_Unlock");
                    _animatorLock.Play("Lock");
                    
                    XUtility.ShowTipAndAutoHide(_lockTips.transform, 4, 0.2f, true, context);
                    // _lockTips.gameObject.SetActive(true);
                }
                else
                {
                    _animatorLock.Play("LockIdle");
                }

                _isLockState = false;
            }
            
            _isFeatureLocked = isLock;
        }

        //从free/link转场的时候调用此方法
        public void LockSuperFreeIdle(bool isLock)
        {
            if (isLock)
            {
                _animatorLock.Play("Idle");
                _isLockState = true;
            }
            else
            {
                _animatorLock.Play("LockIdle");
                _isLockState = false;
            }
        }

        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
        }

        public async Task ChangeFill(bool playAnimation = true,bool audio = true, int currentPoint = -1)
        {
            _extraState11029 = context.state.Get<ExtraState11029>();
           
            if(currentPoint < 0)
                currentPoint =  (int) _extraState11029.GetMapPoint();
            
            var maxPoint = _extraState11029.GetMapMaxPoint();
            
            if (currentPoint < maxPoint)
            {
                 fill.transform.localScale = new Vector3(currentPoint*(1.0f/(int) maxPoint), 1.0f, 1.0f);
                 if (audio)
                 {
                      AudioUtil.Instance.PlayAudioFx("Map_FlySelect");
                 }
                 XDebug.Log("ChangeFillif:Trigger");
                 await TriggerCollectHorse();
            }
            else
            {
                fill.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                XDebug.Log("ChangeFill:Trigger");
                await TriggerCollectHorse();
                if (playAnimation)
                {
                    XDebug.Log("ChangeFill:Open");
                    await OpenCollectHorse();
                }
            }
        }
    }
}