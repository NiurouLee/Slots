using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class Constant11301
    {
        //商城是否弹出
        public static bool IsShowShop=false;
        public static bool IsSpining=false;
        public static bool IsShowMapFeature=false;
        public static List<GameObject> AllGoldList;
        /// <summary>
        /// 代币最大数
        /// </summary>
        public static readonly int MaxTokenNum=150000;
        /// <summary>
        /// 代币警告快满状态
        /// </summary>
        public static readonly int WarnTokenNum=145500;
        public static readonly int pageWidth = 365;
        public static readonly uint NormalLinkElementId = 5;
        
        public static readonly uint ScatterElementId = 10;
        
        public static readonly Dictionary<uint,string> DicBigElementId2Name = new Dictionary<uint,string>()
        {
            {1,"BigSymbol_S01"},
            {2,"BigSymbol_S02"},
            {3,"BigSymbol_S03"},
            {4,"BigSymbol_S04"},
            {14,"BigSymbol_W01"},
        };
        
        
        public static readonly Dictionary<uint,string> DicBigElementId2AudioName = new Dictionary<uint,string>()
        {
	        {1,"FreeSpin_MoneyTree"},
	        {2,"FreeSpin_Caishen"},
	        {3,"FreeSpin_Shou"},
	        {4,"FreeSpin_Heshang"},
	        {14,"FreeSpin_Wild"},
        };

        public static readonly List<uint> ListWildElementId = new List<uint>()
        {
            14,35
        };

        public static readonly List<uint> ListNormalCoins = new List<uint>()
        {
            11,15,16,17,18,19,20,21
        };
        
        public static readonly List<uint> ListNormalJackpotCoins = new List<uint>()
        {
            22,23,24,25
        };
        
        
        public static readonly List<uint> ListDoorCoins = new List<uint>()
        {
            36,37,38,39,40,41,42,43
        };
        
        public static readonly List<uint> ListDoorJackpotCoins = new List<uint>()
        {
            44,45,46,47
        };
        
        
        public static readonly List<uint> ListDoorElementIds = new List<uint>()
        {
            26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47
        };
        public static readonly List<uint> AllListDoorElementIds = new List<uint>()
        {
            12,13,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47
        };
        
        public static readonly Dictionary<uint,uint> ListDoorToNoElementIds = new Dictionary<uint,uint>()
        {
            {26,1},
            {27,2},
            {28,3},
            {29,4},
            {30,5},
            {31,6},
            {32,7},
            {33,8},
            {34,9},
            {35,14},
            {36,11},
            {37,15},
            {38,16},
            {39,17},
            {40,18},
            {41,19},
            {42,20},
            {43,21},
            {44,22},
            {45,23},
            {46,24},
            {47,25},
        };

        /// <summary>
        /// 金币生成在symbol内的位置
        /// </summary>
        /// <returns></returns>
        public static Vector3  GoldLocalPos= new Vector3(0.6f, -0.5f, 0);
        /// <summary>
        /// 转化获取的金币索引到两个轮盘上的位置0-49，转成两个轮盘上的pos
        /// </summary>
        /// <param name="data"></param>
        /// <param name="rolNum">几行</param>
        /// <returns></returns>
        public static List<int> ConversionAttachFlagsDataToPos(int data,int line){
            //每个轮盘以3个图标数量分割，先能获取哪一列
            List<int> arr=new List<int>();
            var col = Math.Floor(((decimal)data / line));
            //获取当前列的第几行
            var row = data - line * col;
            
            arr.Add((int)col);
            arr.Add((int)row);
            return arr;
        }



        public static async Task PlayDoorAnim(GameObject container,MachineContext context,string stateName)
        {
            Transform tranDoorGroup = container.transform;
            if (tranDoorGroup != null)
            {
                Animator animatorDoor = tranDoorGroup.GetComponent<Animator>();
                await XUtility.PlayAnimationAsync(animatorDoor, stateName, context);
            }
        }
        
        
        public static async Task PlayLockAnim(GameObject container,MachineContext context,string stateName)
        {
            Transform tranDoorGroup = container.transform;
            if (tranDoorGroup != null)
            {
                Animator animatorLock = tranDoorGroup.Find("LockScale/Lock").GetComponent<Animator>();
                animatorLock.gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(animatorLock, stateName, context);
            }
        }
        
        
         
    /// <summary>
    ///  格式化一个字符串为K/M/B/T/Q的缩写形式
    /// </summary>
    /// <param name="inNum">要格式化的数字</param>
    /// <param name="decimalsNum">保留小数位位数</param>
    /// <param name="roundingFunction">取整函数（Floor，Round, Celling等）</param>
    /// <param name="needZeroPlaceHolder">小数部分如果位数不够是否需要0占位</param>
    /// <returns></returns>
    public static string GetAbbreviationFormats(object inNum, int decimalsNum = 2, bool needZeroPlaceHolder = false,
        Func<double, double> roundingFunction = null)
    {
        if (inNum.IsNumeric())
        {
            double num = Convert.ToDouble(inNum);
            
            roundingFunction = roundingFunction ?? Math.Floor;
            double toFormatValue = Math.Abs(num);
            var index = 0;
            string[] postfix = {"", "K", "M", "B", "T", "Q"};
            while (toFormatValue >= 1000)
            {
                toFormatValue *= 0.001;
                index++;
            }

            double f = roundingFunction((double)(toFormatValue * Math.Pow(10, decimalsNum)));
            toFormatValue = f / Math.Pow(10, decimalsNum);

            //四舍五入后可能会多一位数字，再次做一次检查
            if (toFormatValue >= 1000)
            {
                toFormatValue *= 0.001;
                index++;
            }

            index = Math.Min(index, postfix.Length - 1);

            toFormatValue = num > 0 ? toFormatValue : -toFormatValue;

            if (needZeroPlaceHolder)
            {
                return toFormatValue.ToString("N" + decimalsNum, CultureInfo.InvariantCulture) + postfix[index];
            }
            else
            {
                string decimalsFormat = ".##";
                if (decimalsNum != 2)
                {
                    decimalsFormat = ".";
                    for (var i = 0; i < decimalsNum; i++)
                        decimalsFormat += "#";
                }

                return toFormatValue.ToString(
                    "#,#" + decimalsFormat + postfix[index] + ";-#,#" + decimalsFormat + postfix[index] + ";0",
                    CultureInfo.InvariantCulture);
            }
        }
        return "";
    }
        
        
        
        public static async Task OpenDoor(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Open");
        }
        
        public static async Task OpenBaseDoor(GameObject container,MachineContext context)
        {
	        
	        await PlayDoorAnim(container, context, "Open");
        }
        
        public static async Task CloseDoor(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Close");
        }
        
        
        
        public static async Task OpenLockDoor(GameObject container,MachineContext context)
        {
            OpenLock(container, context);
            //await OpenDoor(container, context);
            await PlayDoorAnim(container, context, "FreeOpen");
        }
        
        
        public static async Task CloseLockDoor(GameObject container,MachineContext context)
        {
            CloseLock(container, context);
            await CloseDoor(container, context);
        }
        
        
        public static async Task OpenLock(GameObject container,MachineContext context)
        {
            await PlayLockAnim(container, context, "LockOpen");
        }


        public static async Task CloseLock(GameObject container,MachineContext context)
        {
            await PlayLockAnim(container, context,"LockClose");
        }

        public static async Task DoorIdle(GameObject container,MachineContext context)
        {
            await PlayDoorAnim(container, context, "Idle");
        }
        
        
        public static async Task LockDoorIdle(GameObject container,MachineContext context)
        {
            PlayLockAnim(container, context, "IdleClose");
            await PlayDoorAnim(container, context, "Idle");
        }


        public static async Task PlayOpenDoorAudio()
        {
	        await AudioUtil.Instance.PlayAudioFxAsync("FreeSpin_DoorOpen");
        }
        
        
        public static async Task PlayCloseDoorAudio()
        {
	        await AudioUtil.Instance.PlayAudioFxAsync("FreeSpin_DoorClose");
        }
        
      
        



        protected static readonly string[] _jackpotsAddress =
	        {"UIJackpotMini11301","UIJackpotMinor11301", "UIJackpotMajor11301","UIJackpotMega11301","UIJackpotGrand11301"};

        
        public static async Task ShowJackpot(MachineContext machineContext, LogicStepProxy logicStepProxy,
	        uint jackpotId,ulong jackpotPay,bool isAddBelence)
        {
	        TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
	        machineContext.AddWaitTask(taskCompletionSource,null);

	        var assetAddress = _jackpotsAddress[jackpotId - 1];

	        AudioUtil.Instance.PlayAudioFx("Link_Jackpot");
	        var view = PopUpManager.Instance.ShowPopUp<UIJackpotBase11301>(assetAddress);
	        var jackpotWin = machineContext.state.Get<BetState>().GetPayWinChips(jackpotPay);
            
	        view.SetJackpotWinNum(jackpotWin);

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
        
        
        
        public static async Task BounsFly(MachineContext machineContext, LogicStepProxy logicStepProxy,BonusWinView11301 bonusWinView)
        {
	        
	        var wheel = machineContext.state.Get<WheelsActiveState11301>().GetRunningWheel()[0];
	   


	        var extraState = machineContext.state.Get<ExtraState11301>();
	        var activeState = machineContext.state.Get<WheelsActiveState11301>();
	        var endPos = bonusWinView.GetIntegralPos();
	        
	        int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

	        
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++)
		        {
			        var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        uint elementId = container.sequenceElement.config.id;
			        if (Constant11301.ListNormalCoins.Contains(elementId) ||
			            Constant11301.ListNormalJackpotCoins.Contains(elementId) ||
			            ListDoorCoins.Contains(elementId) ||
			            Constant11301.ListDoorJackpotCoins.Contains(elementId))
			        {
				       

				        
				        ulong chips = 0;

				        if (Constant11301.ListNormalJackpotCoins.Contains(container.sequenceElement.config.id) ||
				            Constant11301.ListDoorJackpotCoins.Contains(container.sequenceElement.config.id))
				        {
					        
					        var item = extraState.GetLinkJackpotPay(x);
					        chips = machineContext.state.Get<BetState>().GetPayWinChips(item.JackpotPay);

					        await ShowJackpot(machineContext, logicStepProxy, item.JackpotId, item.JackpotPay, false);
				        }
				        else
				        {
					        ulong winRate = container.sequenceElement.config.GetExtra<ulong>("winRate");
					        chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
				        }
				        
				        
				        AudioUtil.Instance.PlayAudioFx("Link_J01_Fly");
				        container.PlayElementAnimation("Collect");

				        var objFly = machineContext.assetProvider.InstantiateGameObject("BonusFly", true);
				        objFly.transform.parent = machineContext.transform;
				        var startPos = container.transform.position;
				        objFly.transform.position = startPos;
				        await XUtility.FlyAsync(objFly.transform, startPos, endPos, 0, 0.333f, Ease.Linear, machineContext);
				        machineContext.assetProvider.RecycleGameObject("BonusFly", objFly);


		        
				        await bonusWinView.AddWin((long) chips);
			        }
		        }
		        
	        }
        }


        public static void ClearDoor(MachineContext context)
        {
	        var wheel = context.state.Get<WheelsActiveState11301>().GetRunningWheel()[0];
	        var elementConfigSet = context.state.machineConfig.elementConfigSet;
                
	        List<Task> listTask = new List<Task>();
	        int XCount = wheel.rollCount;
	        int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
	        int indexDoor = 0;
	        for (int x = 0; x < XCount; x++)
	        {
		        for (int y = 0; y < YCount; y++)
		        {
			        var container = wheel.GetRoll(x).GetVisibleContainer(y);
			        if (Constant11301.ListDoorElementIds.Contains(container.sequenceElement.config.id))
			        {
				        uint newId = Constant11301.ListDoorToNoElementIds[container.sequenceElement.config.id];
				        var elementConfig = elementConfigSet.GetElementConfig(newId);
				        container.UpdateElement(new SequenceElement(elementConfig, context));
			        }
		        }
	        }
        }
        public readonly static string pageIndexKey = "pageIndex";
        //存储这一次页面的index
        public static void SaveCurrentPageIndexData(int index){
            PlayerPrefs.SetInt(pageIndexKey,index);
        }
        //获取上一次页面的存储的页面index
        public static int GetLastPageIndexData(){
            return PlayerPrefs.GetInt(pageIndexKey);
        }

        public static async void ShowTipsToAutoHide(Transform transform){
            var anim = transform.GetComponent<Animator>();
            var clips = anim.GetCurrentAnimatorClipInfo(0);
            if(clips.Length>0){
                if (clips[0].clip.name == "Tips_Open" || clips[0].clip.name == "Tips_Idle")
                    return;
            }
            anim.Update(0);
            anim.Play("Open");
            EventSystem.current.SetSelectedGameObject(transform.gameObject);
            var selectEventCustomHandler = transform.gameObject.GetComponent<SelectEventCustomHandler>();
            if(selectEventCustomHandler==null)
                selectEventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();
            selectEventCustomHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
            {
                if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Tips_Close")
                {
                    anim.Play("Close");
                }
            });
        }
    }
}