using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIValentinesDay2022Main")]
    public class UIActivity_ValentinesDay_MapPopup : Popup<UIActivity_Valentine2022_MapPopupController>
    {
        [ComponentBinder("Root/TopGroup/HelpButton")]
        public Button helpButton;

        [ComponentBinder("Root/TopGroup/TimerGroup/TimerText")]
        public TMP_Text textTimer;

        [ComponentBinder("Root/TopGroup/ProgressGroup/ProgressText")]
        public TMP_Text textProgress;


        [ComponentBinder("Root/BottomGroup/PriceButton")]
        public Button priceButton;

        [ComponentBinder("Root/BottomGroup/PriceButton/PriceText")]
        public Text textPrice;

        [ComponentBinder("Root/BottomGroup/ExtraContentsButton")]
        public Button benefitButton;

        [ComponentBinder("Root/LevelPointGroup")]
        public Transform waypointsRoot;

        [ComponentBinder("Root/RewardGroup")]
        public Transform transformRewards;


        public GameObject[] waypoints = new GameObject[50];
        public Animator[] animatorWaypoints = new Animator[50];

        public UIActivity_ValentinesDay_LevelRewardsView rewardsView;

        public UIActivity_ValentinesDay_MapPopup(string address) : base(address)
        {
            contentDesignSize = new Vector2(1365, 768);
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            for (int i = 0; i < 50; i++)
            {
                waypoints[i] = waypointsRoot.GetChild(i).gameObject;
                animatorWaypoints[i] = waypoints[i].GetComponent<Animator>();
            }

            rewardsView = AddChild<UIActivity_ValentinesDay_LevelRewardsView>(transformRewards);
        }

        public void SetStep(int step)
        {
            for (int i = 0; i < 50; i++)
            {
                var shouldShow = i < step;
                waypoints[i].gameObject.SetActive(shouldShow);
                var stateName = (shouldShow ? "Nomal" : "Close");
                animatorWaypoints[i].Play(stateName, 0);
            }

            textProgress.text = $"{step}/50";
        }
    }

    public class UIActivity_Valentine2022_MapPopupController : ViewController<UIActivity_ValentinesDay_MapPopup>
    {
        private Activity_ValentinesDay _activity;
        private bool _animating;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);

            var controller = Client.Get<ActivityController>();
            _activity = controller.GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
            if (_activity != null)
            {
                view.textTimer.text = XUtility.GetTimeText(XUtility.GetLeftTime((ulong) _activity.config.EndTimestamp * 1000));;
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SetButtonInteractable(true);
            Refresh();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<Event_Activity_Valentine2022_ReceiveMainPageInfo>(OnEvent_Activity_Valentine2022_ReceiveMainPageInfo);
            view.priceButton.onClick.AddListener(OnPriceButton);
            view.benefitButton.onClick.AddListener(OnBenefit);
            view.helpButton.onClick.AddListener(OnHelp);
        }

        private async void OnHelp()
        {
            var popup = PopupStack.GetPopup<UIActivity_ValentinesDay_HelpPopup>();
            if (popup == null)
            {
                await PopupStack.ShowPopup<UIActivity_ValentinesDay_HelpPopup>();
            }
        }

        private void OnEvent_Activity_Valentine2022_ReceiveMainPageInfo(Event_Activity_Valentine2022_ReceiveMainPageInfo obj)
        {
            Refresh();
        }

        private void SetButtonInteractable(bool interactable)
        {
            if (view == null) { return; }
            if (view.benefitButton != null)
            {
                view.benefitButton.interactable = interactable;
            }
            if (view.priceButton != null)
            {
                view.priceButton.interactable = interactable;
            }
        }

        private async void OnBenefit()
        {
            if (_activity == null || _activity.shopItemConfig == null) { return; }
            var shopItemConfig = _activity.shopItemConfig;
            if (shopItemConfig == null || shopItemConfig.SubItemList == null
                || shopItemConfig.SubItemList.count == 0) { return; }
            SetButtonInteractable(false);
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
            SetButtonInteractable(true);
        }

        private void OnPriceButton()
        {
            if (_animating == true) { return; }
            if (_activity == null || _activity.shopItemConfig == null) { return; }
            SetButtonInteractable(false);
            var shopItemConfig = _activity.shopItemConfig;
            _activity.purchaseSource = 1;
            Client.Get<IapController>().BuyProduct(shopItemConfig);
            SetButtonInteractable(true);
        }

        private async void Refresh()
        {
            _animating = false;

            view.closeButton.gameObject.SetActive(true);

            if (_activity == null) { return; }
            var s = _activity.sGetValentineMainPageInfo;
            if (s == null) { return; }


            // set state
            if (_activity.purchased)
            {
                view.priceButton.gameObject.SetActive(false);
                view.benefitButton.gameObject.SetActive(false);
            }
            else
            {
                view.priceButton.gameObject.SetActive(true);
                view.benefitButton.gameObject.SetActive(true);
            }

            // set price
            if (_activity.shopItemConfig != null)
            {
                var shopItemConfig = _activity.shopItemConfig;
                view.textPrice.text = "$" + shopItemConfig.Price.ToString();
            }

            // set rewards
            var rewardsArray = _activity.sortedRewards;
            view.rewardsView.Set(rewardsArray);
            view.rewardsView.SetLocked(!_activity.purchased);

            var lastStep = _activity.lastStep;
            var currentStep = (int)s.Step;
            var actualLastStep = (int)Mathf.Min(lastStep, s.StepMax);

            view.SetStep(actualLastStep);
            view.rewardsView.SetStep(actualLastStep);

            if (currentStep != lastStep && lastStep < s.StepMax)
            {
                _animating = true;

                view.closeButton.gameObject.SetActive(false);

                _activity.lastStep = currentStep;

                var popup = PopupStack.GetPopup<UIActivity_ValentinesDay_RewardShowPopup>();

                if (popup == null)
                {
                    popup = await PopupStack.ShowPopup<UIActivity_ValentinesDay_RewardShowPopup>();
                    popup.Set(currentStep - lastStep);
                    SoundController.PlaySfx("valentine_win");
                    await Task.Delay(3000);
                    PopupStack.ClosePopup<UIActivity_ValentinesDay_RewardShowPopup>();
                }

                var count = (int)Mathf.Min(s.Step, s.StepMax);
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventValentinesdayProgress, ("step", $"{count}"), ("source", $"{_activity.itemSource}"));

                for (int i = lastStep; i < count; i++)
                {
                    var waypoint = view.waypoints[i];
                    if (waypoint != null) { waypoint.gameObject.SetActive(true); }
                    var animator = view.animatorWaypoints[i];
                    if (animator != null)
                    {
                        int step = i + 1;
                        view.textProgress.text = $"{step}/50";
                        animator.Play("Open");

                        var sfxName = Activity_ValentinesDay.stepToIndex.ContainsKey((uint)step) ? "valentine_hint" : "valentine_node";
                        SoundController.PlaySfx(sfxName);

                        view.rewardsView.AnimateSetStep(step);
                        await Task.Delay(1000);
                    }
                }


                _animating = false;
            }

            if (_activity.hasRewardToCollect)
            {
                _animating = true;

                var popup = PopupStack.GetPopup<UIActivity_ValentinesDay_CollectPopup>();
                if (popup == null)
                {
                    await Task.Delay(1000);
                    await PopupStack.ShowPopup<UIActivity_ValentinesDay_CollectPopup>();
                }
                _animating = false;
            }

            view.closeButton.gameObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            if (_activity != null)
            {
                view.textTimer.text =
                    XUtility.GetTimeText(XUtility.GetLeftTime((ulong) _activity.config.EndTimestamp * 1000));
            }
        }
    }
}
