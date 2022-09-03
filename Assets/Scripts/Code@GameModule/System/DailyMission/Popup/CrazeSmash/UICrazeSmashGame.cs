using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UICrazeSmashGame : View<ViewController>
    {
        [ComponentBinder("MainGroup/EggGroup/BackButton")]
        public Button buttonBack;

        [ComponentBinder("MainGroup/EggGroup/HammerButton")]
        public Button buttonHammer;

        [ComponentBinder("MainGroup/EggGroup/RewardGroup/IntegralGroup/IntegralText")]
        public Text textIntegral;

        [ComponentBinder("MainGroup/EggGroup/HammerButton/ReminderGroup/NoticeText")]
        public TMP_Text textNotice;

        [ComponentBinder("MainGroup/EggGroup/RewardGroup/RewardBoostGroup")]
        public Transform transformRewardBoost;

        [ComponentBinder("MainGroup/EggGroup/EggLayout")]
        public Transform eggRoot;

        [ComponentBinder("PrefabRoot")]
        public Transform prefabRoot;
  
        public Transform[] eggAnchors;

        public UICrazeSmashGameEgg[] eggs;

        private ulong _currentCoin = 0;

        private Tween _tweenCoin;

        protected CrazeSmashController crazeSmashController;
        
        protected virtual string GetEggPrefabPath()
        {
            return null;
        }

        protected override void OnViewSetUpped()
        {
            crazeSmashController = Client.Get<CrazeSmashController>();
            base.OnViewSetUpped();
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            prefabRoot.gameObject.SetActive(false);

            var count = eggRoot.childCount;
            eggAnchors = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                eggAnchors[i] = eggRoot.GetChild(i);
            }

            var prefab = transform.Find(GetEggPrefabPath()).gameObject;
            if (prefab == null) { return; }
            prefab.gameObject.SetActive(false);

            eggs = new UICrazeSmashGameEgg[count];
           
            for (int i = 0; i < eggAnchors.Length; i++)
            {
                var anchor = eggAnchors[i];
                var go = Object.Instantiate(prefab, anchor);
                go.transform.localPosition = Vector3.zero;
                go.SetActive(true);
                var egg = AddChild<UICrazeSmashGameEgg>(go.transform);
                eggs[i] = egg;
            }
        }

        protected void SetEggs(RepeatedField<EggInfo.Types.Egg> eggDatas)
        {
            if (eggDatas == null || eggDatas.count != eggs.Length)
            {
                return;
            }

            for (int i = 0; i < eggs.Length; i++)
            {
                var egg = eggs[i];
                var eggData = eggDatas[i];
                egg.Set(eggData);
            }
        }

        protected void SetCoin(ulong coin, bool animateCoin = false)
        {
            _tweenCoin?.Kill(true);
          
            if (textIntegral != null)
            {
                if (animateCoin)
                {
                    _tweenCoin = textIntegral.DOCounter((long)_currentCoin, (long)coin, 1.0f);
                    _tweenCoin.onComplete += () =>
                    {
                        _currentCoin = coin;
                        textIntegral.text = coin.GetCommaFormat();
                    };
                }
                else
                {
                    _currentCoin = coin;
                    textIntegral.text = coin.GetCommaFormat();
                }
            }
        }

        protected void ClearItems()
        {
            for (int i = transformRewardBoost.childCount - 1; i >= 1; i--)
            {
                GameObject.DestroyImmediate(transformRewardBoost.GetChild(i).gameObject);
            }
        }

        protected void SetItems(RepeatedField<Item> items, bool animateCoin = false)
        {
            ClearItems();
            ulong coinAmount = 0;
            var clone = CrazeSmashController.FilterItems(items, out coinAmount);
            if (clone == null) { return; }
            SetCoin(coinAmount, animateCoin);
            XItemUtility.InitItemsUI(transformRewardBoost, clone, transformRewardBoost.GetChild(0));
        }

        protected void SetNotice(uint count)
        {
            if (textNotice != null && buttonHammer != null)
            {
                buttonHammer.onClick.RemoveAllListeners();

                if (count == 0)
                {
                    buttonHammer.onClick.AddListener(async () =>
                    {
                        var controller = Client.Get<CrazeSmashController>();
                        await controller.OpenBuyPopup();
                    });
                    textNotice.text = "+";
                }
                else
                {
                    textNotice.text = count.ToString();
                }
            }
        }
    }
}