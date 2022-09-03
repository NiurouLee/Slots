using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIWheelBonus11003PopUp : MachinePopUp
    {
        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup/ContentGroup/UpGroup/StateGroup")]
        private Transform bufferRoot;

        [ComponentBinder("Root/WheelMainGroup/Root/CenterGroup/WheelBonusButton")]
        private Button _wheelBonusSpinButton;

        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup")]
        private Transform tranMainGroup;

        [ComponentBinder("Root/WheelMainGroup")]
        private Animator animatorWheel;

        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup/ContentGroup/UpGroup/IntegralText")]
        private Text wheelText1;

        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup/ContentGroup/RightGroup/IntegralText")]
        private Text wheelText2;

        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup/ContentGroup/BottomGroup/IntegralText")]
        private Text wheelText3;

        [ComponentBinder("Root/WheelMainGroup/Root/AnimControl/MainGroup/ContentGroup/LeftGroup/IntegralText")]
        private Text wheelText4;

        private Animator animatorView;

        private Action _wheelBonusEndAction;
        private float anglePerFan = 0;


        private Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string> _buffTypeToAssetNameDict =
            new Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string>()
            {
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn, "ADDReel"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols, "ADD100"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow, "ADDRow"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddFreeSpin, "5Extra"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddExtraBonus, "IncreasePigCoins"},
            };

        public UIWheelBonus11003PopUp(Transform inTransform) : base(inTransform)
        {
            _wheelBonusSpinButton.onClick.AddListener(OnWheelBonusSpinButtonClicked);
            anglePerFan = 360f / 4;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            AudioUtil.Instance.PlayAudioFx("Map_Wheel_Open");
            AudioUtil.Instance.UnPauseMusic();
            AudioUtil.Instance.MakeASavePoint();
            AudioUtil.Instance.PlayMusic("Map_Wheel_Music", true);
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var extraState = context.state.Get<ExtraState11003>();
            var wheelInfo = extraState.GetBonusWheelInfo();

            var wheelTexts = new List<Text>() {wheelText1, wheelText2, wheelText3, wheelText4};

            for (var i = 0; i < wheelTexts.Count; i++)
            {
                wheelTexts[i].text = (wheelInfo.WheelWinRate[i] * wheelInfo.Bet * 0.01).GetAbbreviationFormat(1);
            }

            for (var i = 0; i < bufferRoot.childCount; i++)
            {
                bufferRoot.GetChild(i).gameObject.SetActive(false);
            }

            var buffName = _buffTypeToAssetNameDict[wheelInfo.WheelBuffType];

            bufferRoot.Find(buffName).gameObject.SetActive(true);
        }

        private SBonusProcess sBonusProcess;

        private async void OnWheelBonusSpinButtonClicked()
        {
            _wheelBonusSpinButton.interactable = false;
            
            StartWheelBonus();
            
            var extraState = context.state.Get<ExtraState11003>();
            sBonusProcess = await extraState.SendBonusProcess();
        }

        public override Task OnClose()
        {
            AudioUtil.Instance.RecoverLastSavePointMusicPlay();
            return base.OnClose();
        }

        protected async void StartWheelBonus()
        {
            
            AudioUtil.Instance.PlayAudioFx("Map_Wheel_Loop");
            
            await XUtility.PlayAnimationAsync(animatorWheel, "Start");

            while (sBonusProcess == null)
            {
                await XUtility.WaitSeconds(0.1f, context);
            }

            var extraState = context.state.Get<ExtraState11003>();

            int index = extraState.GetBonusWheelInfo().Choice;
            float targetAngle = anglePerFan * index;

            tranMainGroup.localEulerAngles = new Vector3(0, 0, targetAngle);

            await context.WaitSeconds(2);

            context.WaitSeconds(2.76f, () =>
            {
                AudioUtil.Instance.PlayAudioFx("Map_Wheel_Win");
            });
            
            await XUtility.PlayAnimationAsync(animatorWheel, "Finish");
            await context.WaitSeconds(2);

            Close();
        }
    }
}