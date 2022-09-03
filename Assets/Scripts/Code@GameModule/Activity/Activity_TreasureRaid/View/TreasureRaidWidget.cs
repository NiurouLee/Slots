using System;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidWidget")]
    public class TreasureRaidWidget: SystemWidgetView<TreasureRaidWidgetController>
    {
        [ComponentBinder("ProgressBar")]
        public Slider progress;

        [ComponentBinder("ReminderGroup/NoticeText")]
        private TextMeshProUGUI ticketCountText;

        [ComponentBinder("ProgressBar/ProgressText")]
        public TextMeshProUGUI progressText;

        [ComponentBinder("ReminderGroup")]
        private Transform ticketGroup;
        
        [ComponentBinder("FLy_idle")]
        public Transform flyParticle;

        [ComponentBinder("CollectBubbleGroup/ContentGroupR")]
        public Transform collectRGroup;

        [ComponentBinder("CollectBubbleGroup/ContentGroupL")]
        public Transform collectLGroup;

        [ComponentBinder("TreasureRaidCollect")]
        public Transform collectPrefab;
        public TreasureRaidWidget(string address) : base(address)
        {
            
        }

        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            viewController.OnWidgetClicked(widgetContainerViewController);
        }

        public void RefreshUI()
        {
            if(viewController.activityTreasureRaid == null)
                return;

            SetCountText();
            var ticketCount = viewController.activityTreasureRaid.TicketCount;
            var dataPb = viewController.activityTreasureRaid.GetDataPb();
            progressText.SetText(ticketCount >= dataPb.EnergyInfoWhenSpin.AddedCountMax ? "MAXIMUM" : $"{dataPb.EnergyInfoWhenSpin.Energy}%");
            progress.DOKill();
            progress.value = ticketCount >= dataPb.EnergyInfoWhenSpin.AddedCountMax ? 1 : dataPb.EnergyInfoWhenSpin.Energy / (float) 100;
        }

        public void SetCountText()
        {
            var ticketCount = viewController.activityTreasureRaid.TicketCount;
            ticketGroup.gameObject.SetActive(ticketCount > 0);
            ticketCountText.SetText(ticketCount > 99 ? "99+" : ticketCount.ToString());
        }
        
        public override int GetWidgetPriority()
        {
            return HandlerPrioritySystemCollectWidget.TreasureRaidWidget;
        }
    }

    public class TreasureRaidWidgetController : ViewController<TreasureRaidWidget>
    {
        public Activity_TreasureRaid activityTreasureRaid;
        private bool isShowAni;

        private uint lastEnergy;

        private CancelableCallback cancelableRCallback;
        private CancelableCallback cancelableLCallback;

        private Sequence _sequence;
        public override void OnViewDidLoad()
        {
            var activityController = Client.Get<ActivityController>();
            activityTreasureRaid = activityController.GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            base.OnViewDidLoad();
            view.transform.gameObject.SetActive(true);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.RefreshUI();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            // 监听活动结束事件
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
            SubscribeEvent<EventActivityServerDataUpdated>(RefreshUserData);
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.ActivityTreasureRaid);
        }

         protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (!activityTreasureRaid.HasSpinRewardData)
            {
                handleEndCallback?.Invoke();
                return;
            }

            activityTreasureRaid.HasSpinRewardData = false;
            var monopolyEnergyInfoWhenSpin = activityTreasureRaid.GetDataPb().EnergyInfoWhenSpin;
            if (monopolyEnergyInfoWhenSpin.Reward != null && activityTreasureRaid.ShowTip)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidTicketNoticePopup),
                    monopolyEnergyInfoWhenSpin)));
            }
            else if (monopolyEnergyInfoWhenSpin.Reward != null && activityTreasureRaid.TicketCount >= monopolyEnergyInfoWhenSpin.AddedCountMax)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidTicketNoticePopup),
                    monopolyEnergyInfoWhenSpin)));
            }
            else if (monopolyEnergyInfoWhenSpin.Reward != null && !activityTreasureRaid.ShowTip)
            {
                var item = XItemUtility.GetItem(monopolyEnergyInfoWhenSpin.Reward.Items,
                    Item.Types.Type.MonopolyActivityTicket);
                if (cancelableRCallback != null)
                {
                    view.collectRGroup.gameObject.SetActive(false);
                    cancelableRCallback.CancelCallback();
                }
                if (cancelableLCallback != null)
                {
                    view.collectLGroup.gameObject.SetActive(false);
                    cancelableLCallback.CancelCallback();
                }

                if (view.transform.position.x < 0)
                {
                    view.collectRGroup.Find("CountText").GetComponent<Text>().SetText($"X{item.MonopolyActivityTicket.Amount}");
                    view.collectRGroup.gameObject.SetActive(true);
                    cancelableRCallback = WaitForSeconds(2.5f, () =>
                    {
                        view.collectRGroup.gameObject.SetActive(false);
                    });
                }
                else
                {
                    view.collectLGroup.Find("CountText").GetComponent<Text>().SetText($"X{item.MonopolyActivityTicket.Amount}");
                    view.collectLGroup.gameObject.SetActive(true);
                    cancelableLCallback = WaitForSeconds(2.5f, () =>
                    {
                        view.collectLGroup.gameObject.SetActive(false);
                    });
                }
            }

            handleEndCallback?.Invoke();
        }

        private async Task PlayCollectAni()
        {
            await view.systemWidgetContainerViewController.FocusOnWidget(view);
            Vector3 wheelPosition = Vector3.zero;
            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

            if (machineScene != null)
            {
                wheelPosition = machineScene.viewController.GetRunningWheelPosition();
            }
            Transform container = GameObject.Find("HighPriorityUIContainerCanvas/FlyUtils").transform;

            var cameraPos = Camera.main.transform.position;
            var relativeToCamera = wheelPosition - cameraPos;
            var uiRelativeToCamaraZ = container.position.z - cameraPos.z;
            var uiPosX = relativeToCamera.x * uiRelativeToCamaraZ / relativeToCamera.z;
            var uiPosY = relativeToCamera.y * uiRelativeToCamaraZ / relativeToCamera.z;
            
            var initPos = new Vector3(cameraPos.x + uiPosX, cameraPos.y + uiPosY, cameraPos.z + uiRelativeToCamaraZ);
            
            Vector3 fromLocalPos = container.InverseTransformPoint(initPos);
            Transform target = view.transform;
            if (!view.transform.gameObject.activeInHierarchy)
            {
                target = view.systemWidgetContainerViewController.GetSecondLevelWidget().GetActivityCollectTargetTransform();
            }
            Vector3 targetWorldPos = target.parent.TransformPoint(target.localPosition);
            Vector3 targetLocalPos = container.InverseTransformPoint(targetWorldPos);

            Vector3 midLocalPos = (fromLocalPos + targetLocalPos) * 0.5f;
            midLocalPos.x += 50;
            midLocalPos.y += 50;
            midLocalPos.z = -100;

            Vector3[] wayPoints = new[] {fromLocalPos, midLocalPos, targetLocalPos};

            int coinCount = 6;

            var particle = GameObject.Instantiate(view.flyParticle, target, false);
            particle.localPosition = Vector3.zero;
            particle.gameObject.SetActive(false);
            
            for (int i = 0; i < coinCount; i++)
            {
                if (view.transform == null)
                    break;
                var index = i;
                var flyTr = GameObject.Instantiate(view.collectPrefab, container, false);
                flyTr.SetSiblingIndex(1);
                flyTr.position = initPos;
                flyTr.localScale = Vector3.one * 0.5f;
                flyTr.gameObject.SetActive(true);
                
                flyTr.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBounce).OnComplete(() =>
                {
                    flyTr.DOScale(Vector3.one * 0.5f, 0.3f).SetDelay(0.25f);
                    flyTr.DOLocalPath(wayPoints, 0.6f, PathType.CatmullRom, PathMode.Full3D, 10)
                        .OnComplete(() =>
                        {
                            GameObject.Destroy(flyTr.gameObject);
                            if (view.transform == null)
                                return;
                            if (index == 0)
                            {
                                particle.gameObject.SetActive(true);
                            }
                        }).SetEase(Ease.InQuad);
                
                });
                await WaitForSeconds(0.15f);
            }

            WaitForSeconds(0.6f, () =>
            {
                if (target == null)
                    return;
                GameObject.Destroy(particle.gameObject);
            });
        }

        private async void RefreshUserData(EventActivityServerDataUpdated data)
        {
            if (data.activityType != ActivityType.TreasureRaid)
                return;

            if (_sequence != null)
            {
                _sequence.Kill();
            }
            XDebug.LogWarning("lastEnergy:" + lastEnergy + "-------");
            if (data.showAni && data.monopolyEnergyInfoWhenSpin != null && lastEnergy != data.monopolyEnergyInfoWhenSpin.Energy)
            {
                var lastCount = lastEnergy;
                lastEnergy = data.monopolyEnergyInfoWhenSpin.Energy;
                // 进度条增长动画
                isShowAni = true;
                await PlayCollectAni();

                if (view.transform == null)
                    return;

                view.progress.DOKill();

                _sequence = DOTween.Sequence();
                //Reward 不为空说明收集满了一次
                if (data.monopolyEnergyInfoWhenSpin.Reward != null)
                {
                    _sequence.AppendCallback(() =>
                    {
                        DOTween.To(value =>
                        {
                            view.progressText.SetText($"{(int) value}%");
                        }, lastCount, 100, 0.3f);
                        view.progress.DOValue(1f, 0.3f);
                    });
                    _sequence.AppendInterval(0.4f);
                    _sequence.AppendCallback(() =>
                    {
                        lastCount = 0;
                    });
                }

                if (data.monopolyEnergyInfoWhenSpin.TicketCount < data.monopolyEnergyInfoWhenSpin.AddedCountMax)
                {
                    _sequence.AppendCallback(() =>
                    {
                        if (data.monopolyEnergyInfoWhenSpin.Reward != null)
                        {
                            view.progress.value = 0;
                        }
                        DOTween.To(value =>
                        {
                            view.progressText.SetText($"{(int) value}%");
                        }, lastCount, data.monopolyEnergyInfoWhenSpin.Energy, 0.3f);
                        XDebug.LogWarning("data.monopolyEnergyInfoWhenSpin.Energy:" + data.monopolyEnergyInfoWhenSpin.Energy + "-------");
                        view.progress.DOValue((float) data.monopolyEnergyInfoWhenSpin.Energy / 100, 0.3f).OnComplete(
                        () =>
                        {
                            isShowAni = false;
                            view.SetCountText();
                        });
                    });
                }
                else
                {
                    _sequence.AppendCallback(() =>
                    {
                        isShowAni = false;
                        view.RefreshUI();
                    });
                }
            }
            else
            {
                view.RefreshUI();
            }
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.systemWidgetContainerViewController.RemoveSystemWidgetView(view);
        }

        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            if (!activityTreasureRaid.IsValid())
                return;

            if (isShowAni)
                return;

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidEnter, ("OperationId","3"));

            // 这里要判断当前是否已经开始大富翁游戏，如果开始了直接进入关卡，如果没开始进地图。
            if (activityTreasureRaid.GetCurrentRunningRoundID() == 0)
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMapPopup), true, "Machine")));
            else
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(TreasureRaidMainPopup), true,"Machine")));
            // BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatEnter, ("Operation:", "Machine"),("OperationId","2"));
        }

        public override void OnViewDestroy()
        {
            if (cancelableRCallback != null)
            {
                cancelableRCallback.CancelCallback();
            }
            if (cancelableLCallback != null)
            {
                cancelableLCallback.CancelCallback();
            }
            base.OnViewDestroy();
        }
    }
}