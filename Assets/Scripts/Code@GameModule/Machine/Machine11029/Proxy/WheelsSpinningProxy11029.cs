// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/18:12
// Ver : 1.0.0
// Description : WheelsSpinningProxy11001.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;
using Random = UnityEngine.Random;


namespace GameModule
{
    public class WheelsSpinningProxy11029 : WheelsSpinningProxy
    {
        ElementConfigSet elementConfigSet = null;
        private FreeSpinState freeSpinState;
        private ReSpinState reSpinState;
        public LockElementLayer11029 _layer;
        public LockElementLayer11029 _layerBase;
        public LockElementLayer11029 _layerBonusFree;
        public LockElementLayer11029 _layerMapGameRandomWild1;
        public LockElementLayer11029 _layerMapGameRandomWild2;
        public LockElementLayer11029 _layerMapGameRandomWild3;
        private ExtraState11029 extraState;
        public List<DragonRisingGameResultExtraInfo.Types.Position> lastRandomWildPosList;

        public WheelsSpinningProxy11029(MachineContext context)
            : base(context)
        {
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            lastRandomWildPosList = new List<DragonRisingGameResultExtraInfo.Types.Position>();
            extraState = machineContext.state.Get<ExtraState11029>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
            reSpinState = machineContext.state.Get<ReSpinState>();
        }

        public override void SetUp()
        {
            base.SetUp();
            
            var wheelBase = machineContext.view.Get<Wheel>(0);
            _layerBase = machineContext.view.Add<LockElementLayer11029>(wheelBase.transform);
            _layerBase.BindingWheel(wheelBase);
            _layerBase.SetSortingGroup("Element", 9999);
            
            var wheelFree = machineContext.view.Get<Wheel>(1);
            _layer = machineContext.view.Add<LockElementLayer11029>(wheelFree.transform);
            _layer.BindingWheel(wheelFree);
            _layer.SetSortingGroup("Element", 9999);

            var wheelBonus = machineContext.view.Get<Wheel>(3);
            _layerBonusFree =machineContext.view.Add<LockElementLayer11029>(wheelBonus.transform);
            _layerBonusFree.BindingWheel(wheelBonus);
            _layerBonusFree.SetSortingGroup("Element", 9999);


            var WheelBigPointGame1 = machineContext.view.Get<Wheel>(4);
            _layerMapGameRandomWild1 = machineContext.view.Add<LockElementLayer11029>(WheelBigPointGame1.transform);
            _layerMapGameRandomWild1.BindingWheel(WheelBigPointGame1);
            _layerMapGameRandomWild1.SetSortingGroup("Element", 9999);

            var WheelBigPointGame2 = machineContext.view.Get<Wheel>(5);
            _layerMapGameRandomWild2 = machineContext.view.Add<LockElementLayer11029>(WheelBigPointGame2.transform);
            _layerMapGameRandomWild2.BindingWheel(WheelBigPointGame2);
            _layerMapGameRandomWild2.SetSortingGroup("Element", 9999);

            var WheelBigPointGame3 = machineContext.view.Get<Wheel>(6);
            _layerMapGameRandomWild3 = machineContext.view.Add<LockElementLayer11029>(WheelBigPointGame3.transform);
            _layerMapGameRandomWild3.BindingWheel(WheelBigPointGame3);
            _layerMapGameRandomWild3.SetSortingGroup("Element", 9999);
        }
        
          //期待必中动画
          private async Task HandleAnticipation()
          {
              var extraState11029 = machineContext.state.Get<ExtraState11029>();

              int ranNum = Random.Range(1, 100);
              if (ranNum <= 80)
              {
                  AudioUtil.Instance.StopAudioFx("BonusGame_Character_Back");
                  machineContext.view.Get<MeiDuSha11029>().PlayShowAppear();
                  await machineContext.WaitSeconds(0.5f);
                  var wheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
                  machineContext.view.Get<MeiDuSha11029>().ShowBaseBonusLight(true);
                  machineContext.state.Get<WheelsActiveState11029>().ShowRollsAniMasks(wheel);
                  machineContext.view.Get<MeiDuSha11029>().ShowBaseBonusAni(true);
                  await machineContext.WaitSeconds(1.0f);
                  machineContext.view.Get<MeiDuSha11029>().ShowBaseBonusLight(false);
                  machineContext.view.Get<MeiDuSha11029>().ShowBaseBonusAni(false);
                  machineContext.state.Get<WheelsActiveState11029>().FadeOuAnitRollMask(wheel);
              }
          }

        public override async void OnSpinResultReceived()
        {
            var extraState11029 = machineContext.state.Get<ExtraState11029>();
            if (freeSpinState.IsInFreeSpin && (freeSpinState.freeSpinId == 0))
            {
                //随机投放wild
                await machineContext.view.Get<MeiDuSha11029>().ShowFreeGlow();
                await _layer.ShowRandomWildElement();
                base.OnSpinResultReceived();
            }else if (freeSpinState.IsInFreeSpin && (freeSpinState.freeSpinId == 2))
            {
                AudioUtil.Instance.PlayAudioFx("Map_Classic_Spin");
                base.OnSpinResultReceived();
            }
            else if (freeSpinState.IsInFreeSpin && (freeSpinState.freeSpinId == 3))
            {
                // 知道当前可以播放单个 转盘 的最大的 wild 数量
                var listWildPos1 = extraState11029.GetRandomWildIds()[0].Items;
                var listWildPos2 = extraState11029.GetRandomWildIds()[1].Items;
                var listWildPos3 = extraState11029.GetRandomWildIds()[2].Items;
                var wheels = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel();

                if (listWildPos1.Count > 0)
                {
                    machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[0]);
                    Debug.LogError($"Create fire 1 count: {listWildPos1.count}");
                }

                if (listWildPos2.Count > 0)
                {
                    machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[1]);
                    Debug.LogError($"Create fire 2 count: {listWildPos2.count}");
                }

                if (listWildPos3.Count > 0)
                {
                    machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[2]);
                    Debug.LogError($"Create fire 3 count: {listWildPos3.count}");
                }
                if (listWildPos1.Count > 0 || listWildPos2.Count > 0 || listWildPos3.Count > 0)
                {
                    await machineContext.WaitSeconds(1.0f);
                }
                var maxWild = Mathf.Max(listWildPos1.count, listWildPos2.count, listWildPos3.count);
                for (var i = 0; i < maxWild; i++)
                {
                    AudioUtil.Instance.PlayAudioFx("FreeGame_Character_Select");
                    if (listWildPos1.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                        var position = listWildPos1[i];
                        _layerMapGameRandomWild1.ShowMapGameFireElement(wheel, position);
                    }

                    if (listWildPos2.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];
                        var position = listWildPos2[i];
                        _layerMapGameRandomWild2.ShowMapGameFireElement(wheel, position);
                    }

                    if (listWildPos3.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];
                        var position = listWildPos3[i];
                        _layerMapGameRandomWild3.ShowMapGameFireElement(wheel, position);
                    }
                    await machineContext.WaitSeconds(0.2f);

                    if (listWildPos1.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                        var position = listWildPos1[i];
                        _layerMapGameRandomWild1.ShowMapWildElement(wheel, position, 1);
                    }

                    if (listWildPos2.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[1];
                        var position = listWildPos2[i];
                        _layerMapGameRandomWild2.ShowMapWildElement(wheel, position, 2);
                    }

                    if (listWildPos3.count > i)
                    {
                        var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[2];
                        var position = listWildPos3[i];
                        _layerMapGameRandomWild3.ShowMapWildElement(wheel, position, 3);
                    }
                    await machineContext.WaitSeconds(0.3f);
                }
                machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[0]);
                machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[1]);
                machineContext.state.Get<WheelsActiveState11029>().ShowMapRollsMasks(wheels[2]);
                base.OnSpinResultReceived();
            }
            else if (freeSpinState.IsInFreeSpin && (freeSpinState.freeSpinId == 4))
            {
                //不需要随机投放wild
                bool changeWild = false;
                for (int i = 0; i < 3; i++)
                {
                    if (extraState11029.GetMovingWildIds()[i].Items.Count > 0)
                    {
                        for (int t = 0; t < extraState11029.GetMovingWildIds()[i].Items.Count;t++)
                        {
                            if (extraState11029.GetMovingWildIds()[i].Items[t].Moving == true)
                            {
                                changeWild = true;
                            }
                        }
                    }
                }
                if (changeWild)
                {
                    AudioUtil.Instance.PlayAudioFx("Map_Wild_Remove");
                    _layerMapGameRandomWild1.MovingWild1();
                    _layerMapGameRandomWild2.MovingWild2();
                    _layerMapGameRandomWild3.MovingWild3();
                    await machineContext.WaitSeconds(1.75f);
                    // _layerMapGameRandomWild1.ShowMapGame1MovingWildElement();
                    // _layerMapGameRandomWild2.ShowMapGame2MovngWildElement();
                    // _layerMapGameRandomWild3.ShowMapGame3MovingWildElement();
                    // _layerMapGameRandomWild1.RecyleMapGameM0vingOldWild1();
                    // _layerMapGameRandomWild2.RecyleMapGameMovingOldWild2();
                    // _layerMapGameRandomWild3.RecyleMapGameMovingOldWild3();
                }
                base.OnSpinResultReceived();
            }
            else if (extraState11029.GetIsDrag())
            {
                if (!freeSpinState.IsInFreeSpin)
                {
                    await HandleAnticipation();
                }

                base.OnSpinResultReceived();
              
                //Drag结束之后才能显示StopButton
                if(!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            }
            else if(extraState11029.GetIsWheel())
            {
                await HandleAnticipation();
                base.OnSpinResultReceived();
            }
            else
            {
                base.OnSpinResultReceived();
            }
        }
    }
}