using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;

namespace GameModule
{
    public class TreasureRaidWheelView : View<TreasureRaidWheelViewController>
    {
        [ComponentBinder("SpinButton")] 
        public Button spinButton;

        [ComponentBinder("ExtraGroup/CounterGroup/PlusButton")]
        public Button addBtn;

        [ComponentBinder("WheelRoot/MainGroup")] 
        public Transform mainGroup;

        [ComponentBinder("ExtraGroup/CounterGroup/CountText")] 
        public TextMeshProUGUI ticketCountText;

        public Animator animator;

        public float anglePerFan = 0;

        protected override void OnViewSetUpped()
        {
            anglePerFan = (float) 360 / 8;
            base.OnViewSetUpped();
            animator = transform.GetComponent<Animator>();
        }
        
        public void StartSpinWheel()
        {
            animator.Play("Start");
        }

        public async Task StopWheel(int hitWedgeId)
        {
            float targetAngle = anglePerFan * hitWedgeId;
            
            mainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);
            
            await XUtility.PlayAnimationAsync(animator, "Finish");
        }

        public async Task ShowWinEffect()
        {
            await XUtility.PlayAnimationAsync(animator, "Win");
        }

        public void PlayEmptyAni()
        {
            XUtility.PlayAnimation(animator, "Empty");
        }
    }

    public class TreasureRaidWheelViewController : ViewController<TreasureRaidWheelView>
    {
        private Activity_TreasureRaid _activityTreasureRaid;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityServerDataUpdated>(RefreshUserData);
        }

        private void RefreshUserData(EventActivityServerDataUpdated evt)
        {
            if (evt.activityType != ActivityType.TreasureRaid)
                return;
            view.ticketCountText.SetText(_activityTreasureRaid.TicketCount.ToString());
        }

        public override void OnViewDidLoad()
        {
            _activityTreasureRaid =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.OnViewDidLoad();
            view.spinButton.onClick.AddListener(OnSpinBtnClicked);
            view.addBtn.onClick.AddListener(OnAddBtnClicked);
            view.ticketCountText.SetText(_activityTreasureRaid.TicketCount.ToString());
        }

        private void OnAddBtnClicked()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidBuyTicketPopup))));
        }

        private async void OnSpinBtnClicked()
        {
            // 暂时注释掉
            if (_activityTreasureRaid.TicketCount <= 0)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidBoosterPopup))));
                return;
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidWhirl);

            var parentView = view.GetParentView() as TreasureRaidMainPopup;
            parentView?.SetBtnState(false);

            view.spinButton.interactable = false;
            view.addBtn.interactable = false;
            
            var sMonopolySpin = await _activityTreasureRaid.MonopolySpin();
            if (sMonopolySpin == null)
            {
                view.spinButton.interactable = true;
                view.addBtn.interactable = true;
                return;
            }

            var hitWedgeId = (int)sMonopolySpin.ForwardStep;
            parentView?.SetChestBoxPositionExceptCurrent(sMonopolySpin);
            EventBus.Dispatch(new EventActivityServerDataUpdated(ActivityType.TreasureRaid));
            view.StartSpinWheel();
            SoundController.PlaySfx("TreasureRaid_WheelSpin");
            await WaitForSeconds(0.5f);
            await view.StopWheel(hitWedgeId);
            await view.ShowWinEffect();
            
            parentView?.BeginShowSpinEndAni(sMonopolySpin, hitWedgeId, () =>
            {
                view.PlayEmptyAni();
                view.spinButton.interactable = true;
                view.addBtn.interactable = true;
                parentView.SetBtnState(true);
            });
        }
    }
}