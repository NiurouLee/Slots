using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SuperFreeSpinStartPopUp11003 : FreeSpinStartPopUp
    {
        [ComponentBinder("IntegralText")]
        private Text txtIntegral;
 
        [ComponentBinder("5Extra")]
        private Transform tran5Extra;

        [ComponentBinder("IncreasePigGoin")]
        private Transform tranIncreasePigGoin;

        [ComponentBinder("ADDReel")]
        private Transform tranAddReel;

        [ComponentBinder("ADDRow")]
        private Transform tranAddRow;

        [ComponentBinder("ADD100")]
        private Transform tran100Pig;
        
        public SuperFreeSpinStartPopUp11003(Transform transform) : base(transform)
        {
            
        }

        private Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, UIBufferItem> dicBufferItem =
            new Dictionary<PiggyBankGameResultExtraInfo.Types.Buff.Types.Type, UIBufferItem>(); 
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            ulong num = context.state.Get<ExtraState11003>().GetSuperFreeGamePigCoins();

            txtIntegral.SetText(num.GetAbbreviationFormat(1));

            dicBufferItem[PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddFreeSpin] =
                new UIBufferItem(tran5Extra);
            dicBufferItem[PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddExtraBonus] =
                new UIBufferItem(tranIncreasePigGoin);
            dicBufferItem[PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddColumn] =
                new UIBufferItem(tranAddReel);
            dicBufferItem[PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddRow] =
                new UIBufferItem(tranAddRow);
            dicBufferItem[PiggyBankGameResultExtraInfo.Types.Buff.Types.Type.AddSymbols] =
                new UIBufferItem(tran100Pig);
            
            var buffers = context.state.Get<ExtraState11003>().GetBuffers();
            int count = buffers.count;
            for (int i = 0; i < count; i++)
            {
                if (dicBufferItem.TryGetValue(buffers[i].Type, out var item))
                {
                    item.SetEnable(buffers[i].Acquired);
                }
            }
            
            if (freeSpinCountText)
                freeSpinCountText.text = context.state.Get<ExtraState11003>().GetSuperFreeSpinInfo().Total.ToString();
        }
    }
}