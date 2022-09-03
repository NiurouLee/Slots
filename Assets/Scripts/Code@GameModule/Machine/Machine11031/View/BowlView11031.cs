using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using DG.Tweening;

namespace GameModule
{
    public class BowlView11031 : TransformHolder
    {
        private Animator animator; 
        private ExtraState11031 _extraState11031;
        private bool _buttonResponseEnabled = false;

        public BowlView11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var pointerEventCustomHandler = transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnIconClick);
            _extraState11031 = context.state.Get<ExtraState11031>();
        }

        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
        }

        private void OnIconClick(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled)
                return;
            if (!context.view.Get<LuckyPot11031>().GetActiveSelf())
            {
                context.view.Get<ControlPanel>().ShowSpinButton(false);
                context.view.Get<LuckyPot11031>().ShowLuckyPot();
                context.view.Get<LuckyPot11031>().PlayIntro();
            }
        }

        public async Task Fly()
        {
            EnableButtonResponse(false);
            context.view.Get<LuckyPot11031>().EnableButtonResponse(false);
            SortingGroup sortingGroup = transform.GetComponent<SortingGroup>(); 
            sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
            sortingGroup.sortingOrder = 200;
            var mapLevel = _extraState11031.GetMapLevel();
            var endPos = context.transform.Find("flyLuckyNode").position; 
            XUtility.PlayAnimation(animator, "Fly");
            XUtility.Fly(transform, transform.position, endPos, 0, 1.0f, null);
            await context.WaitSeconds(1.0f);
            if (mapLevel < 25)
            {
               context.view.Get<LuckyPot11031>().ShowLuckyPot(false, true); 
               context.view.Get<LuckyPot11031>().PlayIntro();
            }
            else
            {
                context.view.Get<LuckyPot11031>().ShowLuckyPot(false, true);
                context.view.Get<LuckyPot11031>().PlayIntro();
            }
            await context.WaitSeconds(2.67f - 1.0f);
            var progressBar11031 = context.view.Get<CollectGroupView11031>();
            progressBar11031.ChangeFill(false, false,true);
            await Landing();
            context.view.Get<LuckyPot11031>().EnableButtonResponse(true);
            await context.WaitSeconds(1.0f);
            EnableButtonResponse(true);
        }

        public async Task Landing()
        {
            var endPos = context.view.Get<LuckyPot11031>().GetInternalPos();
            XUtility.Fly(transform, transform.position, endPos, 0, 1.0f, null);
            await XUtility.PlayAnimationAsync(animator, "Landing");
            AudioUtil.Instance.PlayAudioFx("MiniGame_Pot_Add");
            var mapLevel = _extraState11031.GetMapLevel();
            if (mapLevel < 25)
            {
               context.view.Get<LuckyPot11031>().ShowLuckyPot(false, false); 
            }
            else
            {
                context.view.Get<LuckyPot11031>().ShowLuckyPot(false, false);
            }
            ShowBowlInBase();
        }

        public void ShowBowlInBase()
        {
            transform.gameObject.SetActive(true);
            SortingGroup sortingGroup = transform.GetComponent<SortingGroup>();
            sortingGroup.sortingLayerID = SortingLayer.NameToID("Element");
            sortingGroup.sortingOrder = 100;
            transform.position = context.transform.Find("BowlPositionNode").position;
        }

        public void ShowBowl(bool enable)
        {
            transform.gameObject.SetActive(enable);
        }

        public void PlayActive()
        {
            XUtility.PlayAnimation(animator, "Active");
        }
        public void PlayIdle()
        {
            XUtility.PlayAnimation(animator, "Idle");
        }
    }
}