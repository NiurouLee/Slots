using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class Constant11004
    {
        public static readonly List<uint> ListLionElementIds = new List<uint>() {13,14};

        public static readonly uint WildElementId = 11;
        
        public static readonly uint ScatterElementId = 12;

        public static readonly uint NormalLinkElementId = 6;
        
        
        
        public static readonly List<uint> ListLinkAllElementIds = new List<uint>()
        {
            15,16,17,18,19,20,21,22,23,24,25,26
        };
        

        public static readonly Dictionary<uint, ulong> DicIngotsCoinIds = new Dictionary<uint, ulong>()
        {
            {15,50},
            {16,100},
            {17,150},
            {18,200},
            {19,500},
            {20,1000},
            {21,2000},
            {22,5000},
            {23,10000},
        };

        public static readonly Dictionary<uint, uint> DicIngotsJackpotIds = new Dictionary<uint, uint>()
        {
            {24,1},
            {25,2},
            {26,3},
        };

        public static readonly int RedFrameIndex = 1;
        public static readonly int GreenFrameIndex = 2;

        public static readonly string GreenFrameName = "GreenFrame";
        
        public static readonly string WildFrameName = "W01Frame";
        
        public static readonly List<string> ListFrameName = new List<string>()
        {
            "",
            "RedFrame",
            GreenFrameName
        };
        
        
        
        protected static readonly string[] _jackpotsAddress =
            {"UIJackpotMini11004","UIJackpotMinor11004", "UIJackpotMajor11004", "UIJackpotGrandJackpot11004","UIJackpotGrandBonus11004"};
        
        protected static readonly string[] _jackpotsAudio =
            {"MiniJackpot","MinorJackpot", "MajorJackpot", "GrandJackpot","GrandBonus"};

        
        public static async Task ShowJackpot(MachineContext machineContext,LogicStepProxy logicStepProxy)
        {
            var jackpotState = machineContext.state.Get<JackpotInfoState>();
            if (jackpotState.HasJackpotWin())
            {
                var jackpotWinInfo = jackpotState.GetJackpotWinInfo();

                await ShowJackpot(machineContext, logicStepProxy, jackpotWinInfo.jackpotId, jackpotWinInfo.jackpotPay,true);
            }
        }

        public static async Task ShowJackpot(MachineContext machineContext, LogicStepProxy logicStepProxy,
            uint jackpotId,ulong jackpotPay,bool isAddBelence)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(taskCompletionSource,null);

            var assetAddress = _jackpotsAddress[jackpotId - 1];

            var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase>(assetAddress);
            var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotPay);
            
            view.SetJackpotWinNum(jackpotWin);

            AudioUtil.Instance.PlayAudioFx(_jackpotsAudio[jackpotId - 1]);
            view.SetPopUpCloseAction( async ()=>
            {
                machineContext.RemoveTask(taskCompletionSource);
                if (isAddBelence)
                {
                    logicStepProxy.AddWinChipsToControlPanel(jackpotWin);
                    float time = machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime();
                    await machineContext.WaitSeconds(time);
                }

                taskCompletionSource.SetResult(true);
            });

            await taskCompletionSource.Task;
        }


        public static void SetLinkNumber(MachineContext context,TextMesh txtCoin, ElementConfig elementConfig)
        {
            if (Constant11004.DicIngotsCoinIds.ContainsKey(elementConfig.id))
            {
                BetState betState = context.state.Get<BetState>();
                ulong num = betState.GetPayWinChips(elementConfig.GetExtra<ulong>("winRate"));
                txtCoin.text = num.GetAbbreviationFormat();
            }

            
        }
        
        
        public static void SetLinkNumber(MachineContext context,ElementContainer container)
        {
            ElementConfig elementConfig = container.sequenceElement.config;
            TextMesh txtCoin = container.GetElement()?.transform.Find("IntegralGroup/IntegralText")?.GetComponent<TextMesh>();
            SetLinkNumber(context,txtCoin,elementConfig);

        }


    }
}