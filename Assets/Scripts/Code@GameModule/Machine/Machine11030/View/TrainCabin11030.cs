using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class TrainCabin11030:TransformHolder
    {
        public GoldRushTrainGameResultExtraInfo.Types.Train.Types.Result nowData;
        public uint nowColor;
        public long containerValue;
        public GameObject container;
        public GameObject priceContainer;
        private MachineAssetProvider assetProvider;
        [ComponentBinder("BodyJPGrand")]
        protected Transform redBack;
        [ComponentBinder("BodyJPMajor")]
        protected Transform greenBack;
        [ComponentBinder("BodyJPMinor")]
        protected Transform blueBack;
        [ComponentBinder("BodyJPMini")]
        protected Transform purpleBack;
        [ComponentBinder("BodyGold")]
        protected Transform goldBack;
        [ComponentBinder("IntegralGroup/IntegralText")]
        protected TextMesh priceNum;
        [ComponentBinder("JPGroup/JPGrandSprite")]
        protected Transform grandIcon;
        [ComponentBinder("JPGroup/JPMajorSprite")]
        protected Transform majorIcon;
        [ComponentBinder("JPGroup/JPMinorSprite")]
        protected Transform minorIcon;
        [ComponentBinder("JPGroup/JPMiniSprite")]
        protected Transform miniIcon;
        public int nowCabinType;
        public Transform cabinPoolHolder;
        public static Dictionary<int, List<GameObject>> cabinPool;
        public Dictionary<uint, Transform> transTrainId2Back;
        public Dictionary<uint, Transform> transJackpotId2Icon;
        public TrainCabin11030(Transform inTransform) : base(inTransform)
        {
        }

        public static void CleanCabinPool()
        {
            cabinPool = new Dictionary<int, List<GameObject>>() 
            {
                {0,new List<GameObject>()},
                {2,new List<GameObject>()},
                {3,new List<GameObject>()},
                {4,new List<GameObject>()},
                {5,new List<GameObject>()},
                {6,new List<GameObject>()},
                {7,new List<GameObject>()},
            };
        }

        public Transform GetBackByTrainId(uint trainId)
        {
            return transTrainId2Back[trainId];
        }
        public Transform GetIconByJackpotId(uint jackpotId)
        {
            return transJackpotId2Icon[jackpotId];
        }

        public override void Initialize(MachineContext inContext)
        {
            context = inContext;
            InitAfterBindingContext();
        }
        public void InitAfterBindingContext()
        {
            assetProvider = context.assetProvider;
            cabinPoolHolder = assetProvider.GetPoolAttachTransform();
        }

        public GameObject GetContainer()
        {
            return priceContainer;
        }

        public long GetContainerValue()
        {
            return containerValue;
        }

        public uint GetJackpotType()
        {
            return nowData.JackpotId;
        }

        public void PlayBump()
        {
            var animator = container.transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator,"Bump");
        }
        public void SetCabinData(GoldRushTrainGameResultExtraInfo.Types.Train.Types.Result cabinData,uint cabinColor)
        {
            containerValue = 0;
            ReCycleCabin();
            nowData = cabinData;
            nowColor = cabinColor;
            GetNewCabin();
            UpdateView();
        }

        public void ReCycleCabin()
        {
            if (container != null)
            {
                cabinPool[nowCabinType].Add(container);
                container.transform.SetParent(cabinPoolHolder,false);
                container = null;
            }
        }

        public void GetNewCabin()
        {
            if (nowData.JackpotPay == 0 && nowData.JackpotId == 0 && nowData.WinRate == 0)
            {
                nowCabinType = 0;
                if (cabinPool[0].Count > 0)
                {
                    container = cabinPool[0].Pop();
                }
                else
                {
                    container = assetProvider.InstantiateGameObject("TrainHeadType");   
                }
            }
            else
            {
                nowCabinType = Random.Range(2,8);
                if (cabinPool[nowCabinType].Count > 0)
                {
                    container = cabinPool[nowCabinType].Pop();
                }
                else
                {
                    container = assetProvider.InstantiateGameObject("TrainBodyType"+nowCabinType);   
                }
            }
            var sortingGroup = container.GetComponent<SortingGroup>();
            sortingGroup.sortingLayerID = SortingLayer.NameToID("SoloElement");
            sortingGroup.sortingOrder = 100;
            ComponentBinder.BindingComponent(this,container.transform);
            container.transform.SetParent(transform,false);
            container.transform.localPosition = new Vector3(0, 0, 0);
            container.SetActive(true);
            transTrainId2Back = new Dictionary<uint, Transform>()
            {
                {14,goldBack},
                {15,redBack},
                {16,greenBack},
                {17,blueBack},
                {18,purpleBack},
            };
            transJackpotId2Icon = new Dictionary<uint, Transform>()
            {
                {1,miniIcon},
                {2,minorIcon},
                {3,majorIcon},
                {4,grandIcon},
            };
        }

        public void CleanView()
        {
            var backList = transTrainId2Back.Values.ToList();
            for (var i = 0; i < backList.Count; i++)
            {
                backList[i].gameObject.SetActive(false);
            }

            if (nowCabinType != 0)
            {
                priceNum.gameObject.SetActive(false);
                var iconList = new List<Transform>(transJackpotId2Icon.Values);
                for (var i = 0; i < iconList.Count; i++)
                {
                    iconList[i].gameObject.SetActive(false);
                }
            }
        }
        public void UpdateView()
        {
            CleanView();
            var cabinBack = GetBackByTrainId(nowColor);
            cabinBack.gameObject.SetActive(true);
            if (nowData.JackpotPay > 0)
            {
                var jackpotId = nowData.JackpotId;
                var jackpotIcon = GetIconByJackpotId(jackpotId);
                jackpotIcon.gameObject.SetActive(true);
                priceContainer = jackpotIcon.gameObject;
                var chips = context.state.Get<BetState>().GetPayWinChips(nowData.JackpotPay);
                containerValue += (long)chips;
            }
            else if(nowData.WinRate > 0)
            {
                var chips = context.state.Get<BetState>().GetPayWinChips(nowData.WinRate);
                priceNum.text = Tools.GetLeastDigits(chips,6);
                priceNum.characterSize = 0.1f;
                priceNum.gameObject.SetActive(true);
                priceContainer = priceNum.gameObject;
                containerValue += (long)chips;
            }
        }
    }
}