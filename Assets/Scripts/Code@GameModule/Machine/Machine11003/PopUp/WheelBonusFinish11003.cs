// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/26/19:41
// Ver : 1.0.0
// Description : WheelBonusFinish11003.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class WheelBonusFinish11003 : MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        private Button _collectButton;

        [ComponentBinder("Root/MainGroup/ExtraGroup/StateGroup")]
        private Transform _buffRootGroup;

        [ComponentBinder("Root/IntegralGroup/IntegralText")]
        private Text _winChipText;
        
        private Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string> _buffTypeToAssetNameDict =
            new Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, string>()
            {
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn, "AddReel"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols, "Add100"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow, "AddRow"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddFreeSpin, "5Extra"},
                {PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddExtraBonus, "IncreasePigCoins"},
            };

        public WheelBonusFinish11003(Transform transform)
            : base(transform)
        {
            if (_collectButton)
            {
                _collectButton.onClick.AddListener(OnCollectClicked);
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            AudioUtil.Instance.PlayAudioFx("MapBonusEnd_Open");
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var extraState11003 = context.state.Get<ExtraState11003>();
            var wheelInfo = extraState11003.GetBonusWheelInfo();

            var winChips = wheelInfo.Bet * wheelInfo.WheelWinRate[wheelInfo.Choice] * 0.01;

            _winChipText.text = winChips.GetCommaFormat();

            if (wheelInfo.Choice == 0)
            {
                for (var i = 0; i < _buffRootGroup.childCount; i++)
                {
                    _buffRootGroup.GetChild(i).gameObject.SetActive(false);
                }

                var buffName = _buffTypeToAssetNameDict[wheelInfo.WheelBuffType];

                _buffRootGroup.Find(buffName).gameObject.SetActive(true);
            }
        }
        
        public void OnCollectClicked()
        {
            Close();
        }
    }
}