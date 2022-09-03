using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class WheelsSpinningProxy11312 : WheelsSpinningProxy
    {
        private ExtraState11312 extraInfo;
        private FeatureView11312 featureView;
        private FreeFeatureView11312 freeFeatureView;
        private List<float> tornadoArr = new List<float>() {1.6f, 1.8f, 2.2f, 2.5f, 2.8f};

        public WheelsSpinningProxy11312(MachineContext context) : base(context)
        {
            spinningOrder = machineContext.state.Get<WheelsActiveState11312>().SpinningOrder;
        }

        protected override void HandleCommonLogic()
        {
            runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            spinningOrder = machineContext.state.Get<WheelsActiveState11312>().SpinningOrder;

            waitingWheel.Clear();

            if (spinningOrder == WheelSpinningOrder.ONE_BY_ONE)
            {
                waitingWheel = new List<Wheel>(runningWheel.ToArray());
            }

            spinningWheel.Clear();
            finishWheel.Clear();
        }

        public override void OnSpinResultReceived()
        {
            DealLogic(() =>
            {
                base.OnSpinResultReceived();
                // 如果当前这一把是小轮盘则 切换roll滚动时的ElementSupplier卷轴
                var respinState = machineContext.state.Get<ReSpinState11312>();
                if (respinState.NextIsReSpin && Constant11312.LastRespinIsHasSmall)
                {
                    var wheel = machineContext.view.Get<LinkWheel11312>();
                    wheel.ForceUpdateElementSupplier(wheel.wheelState);
                }
            });
        }

        // base,free --
        public async void DealLogic(Action callback)
        {
            // 三种Feature
            if (featureView == null)
                featureView = machineContext.view.Get<FeatureView11312>();
            if (freeFeatureView == null)
                freeFeatureView = machineContext.view.Get<FreeFeatureView11312>();
            if (extraInfo == null)
                extraInfo = machineContext.state.Get<ExtraState11312>();
            if (Constant11312.UIRespinFeature != null)
            {
                Constant11312.UIRespinFeature = null;
                await machineContext.WaitSeconds(1);
            }

            // 如果是bomb类型
            if (extraInfo.RandomS01 != null && extraInfo.RandomS01.count != 0)
            {
                Constant11312.LastHasRandomS01 = true;
                await BombFxShow();
            }
            else if (extraInfo.RandomWild != null && extraInfo.RandomWild.Count != 0)
            {
                await WildShow();
            }
            else if (extraInfo.RandomScatter != null && extraInfo.RandomScatter.Count != 0)
            {
                await ScatterShow();
            }

            callback?.Invoke();
        }

        public async Task BombFxShow()
        {
            AudioUtil.Instance.PlayAudioFx("Board");
            var board = machineContext.assetProvider.InstantiateGameObject("Boards");
            board.transform.SetParent(machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels"));
            board.transform.localScale = Vector3.one;
            board.transform.localPosition = new Vector3(0, 0.7f, 0);
            await machineContext.WaitSeconds(1f);
            var colTime = 0;
            var lastCol = -1;
            var curColRolTime = 0;

            List<int> cols = new List<int>() {0, 0, 0, 0, 0};
            for (int i = 0; i < extraInfo.RandomS01.count; i++)
            {
                var Id = (int) extraInfo.RandomS01[i];
                var posArr = Constant11312.ConversionDataToPos(Id, 4);
                var col = posArr[0];
                var rol = posArr[1];
                var targetPos = machineContext.view.Get<Wheel>().GetRoll(col).GetVisibleContainerPosition(rol);

                var bombFX = machineContext.assetProvider.InstantiateGameObject("BombFX");
                bombFX.transform.SetParent(featureView.transform);
                bombFX.transform.position = targetPos;
                bombFX.transform.localScale = Vector3.one;
                var sort = bombFX.AddComponent<SortingGroup>();
                sort.sortingLayerName = "LocalUI";
                sort.sortingOrder = i;
                bombFX.gameObject.SetActive(false);

                var symbol = machineContext.assetProvider.InstantiateGameObject("Static_S01");
                symbol.transform.SetParent(featureView.transform);
                symbol.transform.position = targetPos;
                symbol.transform.localScale = Vector3.one;
                symbol.gameObject.SetActive(false);
                symbol.name = "S01_" + col + "_" + rol + "_" + 1;
                featureView.S01Symbols.Add(symbol);
                featureView.BombFxs.Add(bombFX);

                if (lastCol != col)
                {
                    colTime++;
                    lastCol = col;
                }

                if (lastCol == col)
                {
                    curColRolTime = UnityEngine.Random.Range(0, 3);
                }

                if (cols[col] < 1)
                    cols[col] = 1;

                XDebug.Log("colTime:" + col + "_" + colTime);

                machineContext.WaitSeconds(1f * colTime + curColRolTime * 0.1f, async () =>
                {
                    if (bombFX != null)
                    {
                        bombFX.SetActive(true);
                        bombFX.GetComponent<Animator>().Play("BombFX");
                    }

                    await machineContext.WaitSeconds(0.75f);
                    board.transform.Find("root/board" + Id + "/board").gameObject.SetActive(false);
                    if (symbol != null)
                    {
                        symbol.SetActive(true);
                    }
                });
            }

            var tempTimes = 0;
            for (int i = 0; i < cols.Count; i++)
            {
                if (cols[i] != 0)
                {
                    tempTimes++;
                    machineContext.WaitSeconds(1f * tempTimes, () =>
                    {
                        XDebug.Log("Add_S01===");
                        AudioUtil.Instance.PlayAudioFx("Add_S01");
                    });
                }
            }

            await machineContext.WaitSeconds(1.5f + 1f * colTime);
            board.GetComponent<Animator>().Play("Close");

            await machineContext.WaitSeconds(1f);
            GameObject.Destroy(board.gameObject);
        }

        public async Task WildShow()
        {
            AudioUtil.Instance.PlayAudioFx("Add_Wild");
            machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>().Play("Tornado");

            var tornado = machineContext.assetProvider.InstantiateGameObject("Tornado");
            tornado.transform.SetParent(machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels"));
            tornado.transform.localScale = Vector3.one;

            machineContext.WaitSeconds(4, () => { GameObject.Destroy(tornado.gameObject); });

            List<int> cols = new List<int>() {0, 0, 0, 0, 0};
            foreach (var item in extraInfo.RandomWild)
            {
                var posArr = Constant11312.ConversionDataToPos((int) item.Key, 4);
                var col = posArr[0];
                var rol = posArr[1];
                var symbolId = item.Value;
                var targetPos = machineContext.view.Get<Wheel>().GetRoll(col).GetVisibleContainerPosition(rol);
                var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(symbolId), machineContext);
                var symbol = machineContext.assetProvider.InstantiateGameObject(seqElement.config.staticAssetName);
                symbol.transform.SetParent(featureView.transform);
                symbol.transform.position = targetPos;
                symbol.transform.localScale = Vector3.one;
                symbol.name = "S01_" + col + "_" + rol + "_" + symbolId;
                var wild = symbol.transform.Find("Wild");
                Constant11312.SetStencilCompValue(wild, 8);
                symbol.gameObject.SetActive(false);
                featureView.WildSymbols.Add(symbol);

                if (cols[col] < 1)
                    cols[col] = 1;
                machineContext.WaitSeconds(tornadoArr[col], () =>
                {
                    if (symbol != null)
                        symbol.gameObject.SetActive(true);
                });
            }

            for (int i = 0; i < cols.Count; i++)
            {
                if (cols[i] != 0)
                {
                    machineContext.WaitSeconds(tornadoArr[i], () => { AudioUtil.Instance.PlayAudioFx("Wild_Added"); });
                }
            }

            await machineContext.WaitSeconds(4f);
        }

        public async Task ScatterShow()
        {
            var seaWave = machineContext.assetProvider.InstantiateGameObject("SeaWave");
            seaWave.transform.SetParent(machineContext.transform.Find("ZhenpingAnim/checkerboard/Wheels"));
            seaWave.gameObject.GetComponent<Animator>().Play("open");
            seaWave.transform.localScale = Vector3.one;
            seaWave.transform.localPosition = new Vector3(0, 0.7f, 0);
            seaWave.transform.Find("root/mask1").gameObject.SetActive(true);

            machineContext.WaitSeconds(2, () => { GameObject.Destroy(seaWave.gameObject); });

            await machineContext.WaitSeconds(1);
            AudioUtil.Instance.PlayAudioFx("Add_J01_B01");
            foreach (var item in extraInfo.RandomScatter)
            {
                var posArr = Constant11312.ConversionDataToPos((int) item.Key, 4);
                var col = posArr[0];
                var rol = posArr[1];
                var symbolId = item.Value;
                var targetPos = machineContext.view.Get<Wheel>().GetRoll(col).GetVisibleContainerPosition(rol);
                //设置气泡
                var fishFX = machineContext.assetProvider.InstantiateGameObject("Fish");
                fishFX.transform.SetParent(featureView.transform);
                fishFX.transform.localScale = Vector3.one;
                fishFX.transform.position = targetPos;
                //生成symbol
              
                var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                var seqElement = new SequenceElement(elementConfigSet.GetElementConfig(symbolId), machineContext);
                
                var symbol = machineContext.assetProvider.InstantiateGameObject(seqElement.config.activeAssetName);
                symbol.transform.SetParent(featureView.transform);
                symbol.transform.position = targetPos;
                symbol.transform.localScale = Vector3.one;
                symbol.gameObject.SetActive(false);
                symbol.name = "S01_" + col + "_" + rol + "_" + symbolId;
                
                Constant11312.UpdateElementMaskInteraction(symbol.transform, SpriteMaskInteraction.VisibleInsideMask);
               
                var sort = symbol.AddComponent<SortingGroup>();
                sort.sortingLayerName = "SoloElement";
                sort.sortingOrder = col * 20 + rol * 2;
                if (Constant11312.ScSymbolId == symbolId)
                {
                    sort.sortingOrder += 50;
                }

                if (Constant11312.ListCoinElementIds.Contains(seqElement.config.id))
                {
                    var index = Constant11312.ListCoinElementIds.IndexOf(seqElement.config.id);
                    var winRate = Constant11312.ListCoinElementWinRates[(int) index];
                    var chips = machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                    var text = symbol.transform.Find("AnimRoot/IntegralGroup/IntegralText");
                    text.GetComponent<TextMesh>().text = "" + chips.GetAbbreviationFormat(1);
                    text.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp", 8);
                }

                //装进容器
                featureView.ScOrCoinSymbols.Add(symbol);
                featureView.FishFxs.Add(fishFX);
            }

            machineContext.WaitSeconds(1.35f, () =>
            {
                foreach (var item in featureView.ScOrCoinSymbols)
                {
                    item.gameObject.SetActive(true);
                }
            });
            
            await machineContext.WaitSeconds(2.15f);
            featureView.ClearAllFishFxs();
        }
    }
}