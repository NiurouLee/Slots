using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Storage;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    
    public class LockElementView11004: TransformHolder
    {
        protected LockPrefabLayer lockElementLayer;

        protected Animator animatorLionFeature;
        protected Animator animatorLionInit;
        protected GameObject objTitleAddingFire;
        protected GameObject objTitleMakingWild;
        
        public LockElementView11004(Transform inTransform) : base(inTransform)
        {
            animatorLionFeature = transform.Find("Wheels/CollectGroup/LionTrigger").GetComponent<Animator>();
            animatorLionInit = transform.Find("Wheels/CollectGroup/LionInitial").GetComponent<Animator>();

            objTitleAddingFire = animatorLionFeature.transform.Find("TitleAddingFire").gameObject;
            objTitleMakingWild = animatorLionFeature.transform.Find("TitleMakingWild").gameObject;

            objTitleAddingFire.gameObject.SetActive(false);
            objTitleMakingWild.gameObject.SetActive(false);
        }

        ElementConfigSet elementConfigSet = null;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            lockElementLayer = new LockPrefabLayer(context.view.Get<Wheel>(),context);
            elementConfigSet = context.state.machineConfig.elementConfigSet;
        }


        protected float timeXDragon = 0.4f;
        protected float timeYDragon = 0.3f;
        public async Task RefreshRollingLock()
        {
            //先变feature触发的框，再变panel触发的框
            var extraState = context.state.Get<ExtraState11004>();
            var lockData = extraState.GetLockData();
            var respinState = context.state.Get<ReSpinState>();
            
            var wheel = context.state.Get<WheelsActiveState11004>().GetRunningWheel()[0];

            lockElementLayer.RefreshWheel(wheel);
            
            if (lockData != null && lockData.IsTrigger && !respinState.IsInRespin)
            {
                
                context.view.Get<BaseTitleView11004>().PlayBaseAnim();

                await context.WaitSeconds(1);
                
                
                int XCount = wheel.rollCount;
                int YCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;


                var listFeatureFrame = lockData.FeatureDiffs;
                for (int i = 0; i < listFeatureFrame.Count; i++)
                {
                    animatorLionFeature.gameObject.SetActive(true);
                    objTitleAddingFire.gameObject.SetActive(true);
                    
                    TaskCompletionSource<bool> taskLionFeature = new TaskCompletionSource<bool>();
                    context.AddWaitTask(taskLionFeature,null);
                    AudioUtil.Instance.PlayAudioFx("MakingWild_Trigger");
                    AudioUtil.Instance.FadeMusicTo(0.4f,0.1f);
                    //飞龙
                    XUtility.PlayAnimation(animatorLionFeature, "LionFeature", () =>
                    {
                        context.RemoveTask(taskLionFeature);
                        if (animatorLionFeature != null)
                        {
                            animatorLionFeature.gameObject.SetActive(false);
                            objTitleAddingFire.gameObject.SetActive(false);
                        }
                        
                        AudioUtil.Instance.FadeMusicTo(1,0.1f);
                        taskLionFeature.SetResult(true);
                    },context);
                    await context.WaitSeconds(0.7f);
                    
                    
                    //出框
                    var itemFeatureFrame = listFeatureFrame[i].Items;
                    var dicItemFeatureFrame = extraState.ChangeLockData(itemFeatureFrame);
                    
                    
                   

                    
                    Action<int> YFrameAnim = async (int x) =>
                    {
                        for (int y = 0; y < YCount; y++)
                        {
                            if (dicItemFeatureFrame.TryGetValue((x, y), out var itemLock))
                            {
                                if (itemLock.Colour != 0)
                                {
                                    string objName = Constant11004.ListFrameName[(int) itemLock.Colour];
                                    var objLock = lockElementLayer.ReplaceOrAttachToElement(objName, (int) itemLock.Y, (int) itemLock.X);
                                    var animator = objLock.GetComponent<Animator>();
                                    animator.Play("Open");

                                    if ((int) itemLock.Colour == Constant11004.GreenFrameIndex)
                                    {
                                        AudioUtil.Instance.PlayAudioFx("GreenFire_Appear");
                                    }
                                    else if ((int) itemLock.Colour == Constant11004.RedFrameIndex)
                                    {
                                        AudioUtil.Instance.PlayAudioFx("RedFire_Appear");
                                    }

                                    
                                    
                                    await context.WaitSeconds(timeYDragon);
                                }
                            }
                        }
                    };
                    
                    for (int x = 0; x < XCount; x++)
                    {
                        YFrameAnim.Invoke(x);
                        await context.WaitSeconds(timeXDragon);
                    }
                    
                    
                    //出框
                    // for (int j = 0; j < itemFeatureFrame.Count; j++)
                    // {
                    //     var itemLock = itemFeatureFrame[j];
                    //     if (itemLock.Colour != 0)
                    //     {
                    //         string objName = Constant11004.ListFrameName[(int) itemLock.Colour];
                    //         var objLock = lockElementLayer.ReplaceOrAttachToElement(objName, (int) itemLock.Y, (int) itemLock.X);
                    //         var animator = objLock.GetComponent<Animator>();
                    //         animator.Play("Open");
                    //     }
                    // }

                    await taskLionFeature.Task;
                }


                animatorLionFeature.gameObject.SetActive(true);
                objTitleMakingWild.gameObject.SetActive(true);
                TaskCompletionSource<bool> taskLionFeatureOut = new TaskCompletionSource<bool>();
                context.AddWaitTask(taskLionFeatureOut,null);
                AudioUtil.Instance.PlayAudioFx("MakingWild_Trigger");
                AudioUtil.Instance.FadeMusicTo(0.4f,0.1f);
                //飞龙
                XUtility.PlayAnimation(animatorLionFeature, "LionFeature", () =>
                {
                    context.RemoveTask(taskLionFeatureOut);

                    if (animatorLionFeature != null)
                    {
                        animatorLionFeature.gameObject.SetActive(false);
                        objTitleMakingWild.gameObject.SetActive(false);
                    }
                    AudioUtil.Instance.FadeMusicTo(1f,0.1f);
                    taskLionFeatureOut.SetResult(true);
                },context);
                await context.WaitSeconds(0.766f);
                

                List<Task> listTask = new List<Task>();
                
                var dicItemTriggeringItems = extraState.ChangeLockData(lockData.TriggeringItems);

                //全变成wild
                // for (int i = 0; i < lockData.TriggeringItems.Count; i++)
                // {
                //     var itemLock = lockData.TriggeringItems[i];
                //     listTask.Add(ChangeToWild((int) itemLock.Y, (int) itemLock.X));
                // }


                //wild
                Action<int> YWildAnim = async (int x) =>
                {
                    for (int y = 0; y < YCount; y++)
                    {
                        if (dicItemTriggeringItems.TryGetValue((x, y), out var itemLock))
                        {
                            listTask.Add(ChangeToWild((int) itemLock.Y, (int) itemLock.X));
                            await context.WaitSeconds(timeYDragon);
                        }
                    }
                };
                
                //Link图标
                var dicLink = extraState.GetTriggerLinkItems();
                Action<int> YLinkAnim = async (int x) =>
                {
                    for (int y = 0; y < YCount; y++)
                    {
                        int posId = x * YCount + y;
                        if (dicLink.TryGetValue(posId, out var item))
                        {
                            int row = y;
                            int colume = x;
                            listTask.Add(ChangeToLink(item,row,colume));
                            await context.WaitSeconds(timeYDragon);
                        }
                    }
                };
                
                
                
                for (int x = 0; x < XCount; x++)
                {
                    YWildAnim.Invoke(x);
                    if (respinState.NextIsReSpin && dicLink.Count > 0)
                    {
                        YLinkAnim.Invoke(x);
                    }

                    //await context.WaitSeconds(1f/XCount);
                    await context.WaitSeconds(timeXDragon);
                }
                

                if (listTask.Count > 0)
                {
                    await Task.WhenAll(listTask);
                }
                
                context.view.Get<BaseTitleView11004>().RefreshUI();

                await taskLionFeatureOut.Task;

                //await context.WaitSeconds(0.5f);

            }
        }



        protected async Task ChangeToLink(LionGoldGameResultExtraInfo.Types.LinkData.Types.Item item,int row,int colume)
        {
            var elementConfig =
                elementConfigSet.GetElementConfig(item.SymbolId);
            string elementLinkName = elementConfig.activeAssetName;
                                    
            

            var objLink =
                lockElementLayer.ReplaceOrAttachToElement(elementLinkName, row, colume);

            var sortingGroup = objLink.GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = objLink.AddComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
            }

            TextMesh txtCoin = objLink.transform.Find("IntegralGroup/IntegralText")?.GetComponent<TextMesh>();
            if (txtCoin != null)
            {
                txtCoin.GetComponent<MeshRenderer>().material.SetFloat("_StencilComp",8);
                Constant11004.SetLinkNumber(context,txtCoin,elementConfig);
            }
            
            Animator animator = objLink.GetComponent<Animator>();
            await XUtility.PlayAnimationAsync(animator, "Appear", context);
            //animator.Play("Idle");
        }

        protected async Task ChangeToWild(int row,int colume)
        {
            GameObject objWild = lockElementLayer.ReplaceOrAttachToElement(Constant11004.WildFrameName,row, colume);
            Animator animatorWild = objWild.GetComponent<Animator>();
            AudioUtil.Instance.PlayAudioFx("ChangeToWild");
            await XUtility.PlayAnimationAsync(animatorWild, "Appear", context);
            animatorWild.Play("Idle");
        }


        public  void ClearLock()
        {
            
            var reSpinState = context.state.Get<ReSpinState>();
            if (reSpinState.ReSpinTriggered || reSpinState.NextIsReSpin)
            {
                lockElementLayer.Clear();
            }
            else
            {
                var lockData = context.state.Get<ExtraState11004>().GetLockData();
                var freeSpinState = context.state.Get<FreeSpinState>();
                if (lockData != null)
                {
                    if (lockData.IsOver)
                    {
                        lockElementLayer.Clear();
                        context.view.Get<BaseTitleView11004>().ClearUI();
                    }


                    if (freeSpinState.NextIsFreeSpin)
                    {
                        lockElementLayer.Clear();
                        context.view.Get<BaseTitleView11004>().ClearUI();

                        if (freeSpinState.IsTriggerFreeSpin)
                        {
                            var linkState = context.state.Get<ReSpinState11004>();
                            var jackpotState = context.state.Get<JackpotInfoState>();
                            if (linkState.IsInRespin || jackpotState.HasJackpotWin() || lockData.IsTrigger)
                            {
                                //当 Link / Grand Bonus / Making Wild 与Free Games同时触发时, 在进入FG之前, []在REEL区域的显示和[]的计数器都应该重置, 进入FG后,在REEL区域应不显示任何[], []计数器也应重置为0.
                                return;
                            }
                        }

                        RefreshLockNoAnim();
                    }

                }
                else
                {
                    lockElementLayer.Clear();
                    context.view.Get<BaseTitleView11004>().ClearUI();
                }
            }




        }

        public async Task StartGetLock()
        {
            var lockData = context.state.Get<ExtraState11004>().GetLockData();
            var reSpinState = context.state.Get<ReSpinState>();
            if (lockData != null && lockData.StartDiff!=null && lockData.StartDiff.Items.Count>0)
            {
               
                animatorLionInit.gameObject.SetActive(true);
                AudioUtil.Instance.PlayAudioFx("MakingWild_Reset");
                AudioUtil.Instance.FadeMusicTo(0.1f,0.1f);
                //飞龙
                await XUtility.PlayAnimationAsync(animatorLionInit, "LionInit",context);
                animatorLionInit.gameObject.SetActive(false);
                AudioUtil.Instance.FadeMusicTo(1,0.1f);

                RefreshAllLock(lockData.StartDiff.Items,true);
                
                await context.WaitSeconds(1);

                
            }
        }




        public async Task RefreshStopLock()
        {
            var extraState = context.state.Get<ExtraState11004>();
            var lockData = extraState.GetLockData();
            var respinState = context.state.Get<ReSpinState>();
            var wheel = context.state.Get<WheelsActiveState11004>().GetRunningWheel()[0];
            if (lockData != null && !respinState.IsInRespin)
            {

                
                var panelFrame = lockData.PanelDiff;

                if (panelFrame.Items.Count > 0)
                {
                    
                    //Debug.LogError($"=====GreenFrameIndex");
                    //先整体变绿
                    await RefreshStopLock(Constant11004.GreenFrameIndex, wheel, panelFrame);

                    //Debug.LogError($"=====RedFrameIndex");
                    //再整体变红
                    await RefreshStopLock(Constant11004.RedFrameIndex, wheel, panelFrame);

                    context.view.Get<BaseTitleView11004>().RefreshUI();
                    
                    //检查这把是否还有wild或者link
                    if (lockData.IsTrigger)
                    {
                        await context.WaitSeconds(0.5f);

                        var dicLink = extraState.GetLinkItems();

                        int columeCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;
                        
                        
                        List<Task> listTask = new List<Task>();
                        
                        for (int i = 0; i < panelFrame.Items.Count; i++)
                        {
                            var itemLock = panelFrame.Items[i];
                            if (respinState.NextIsReSpin && itemLock.Colour == Constant11004.GreenFrameIndex)
                            {
                                //Link
                                int positionId = (int)(itemLock.X * columeCount + itemLock.Y);
                                if (dicLink.TryGetValue(positionId, out var itemLink))
                                {
                                    listTask.Add(ChangeToLink(itemLink,(int)itemLock.Y,(int)itemLock.X));
                                }
                                
                            }
                            else
                            {
                                //wild
                                listTask.Add(ChangeToWild((int) itemLock.Y, (int) itemLock.X));
                            }

                        }

                        if (listTask.Count > 0)
                        {
                            await Task.WhenAll(listTask);
                        }

                        //await context.WaitSeconds(1);
                    }
                }
            }
            
            //把覆盖层替换到element里
            if (lockData != null && lockData.IsTrigger && !respinState.IsInRespin)
            {
                //替换wild
                for (int i = 0; i < lockData.TriggeringItems.Count; i++)
                {
                    var itemLock = lockData.TriggeringItems[i];
                    //ChangeToWild((int) itemLock.Y, (int) itemLock.X);
                    var container = wheel.GetRoll((int) itemLock.X).GetVisibleContainer((int) itemLock.Y);
                    
                    //如果下面是scatter就不替换
                    // if (container.sequenceElement.config.id != Constant11004.ScatterElementId)
                    // {
                        var wildConfig = elementConfigSet.GetElementConfig(Constant11004.WildElementId);
                        container.UpdateElement(new SequenceElement(wildConfig,context));
                        container.ShiftSortOrder(false);
                    // }
                }

                for (int i = 0; i < lockData.PanelDiff.Items.Count; i++)
                {
                    var itemLock = lockData.PanelDiff.Items[i];
                    var container = wheel.GetRoll((int) itemLock.X).GetVisibleContainer((int) itemLock.Y);
                    
                    var wildConfig = elementConfigSet.GetElementConfig(Constant11004.WildElementId);
                    container.UpdateElement(new SequenceElement(wildConfig,context));
                    container.ShiftSortOrder(false);
                }
                
                var freeSpinState = context.state.Get<FreeSpinState>();
                //替换link元宝
                if (respinState.NextIsReSpin && !freeSpinState.IsTriggerFreeSpin)
                {
                    var dicLink = extraState.GetLinkItems();
                    if (dicLink.Count > 0)
                    {
                        var listElement = wheel.GetVisibleElementInfo();
                        int columeCount = wheel.GetRoll(0).lastVisibleRowIndex - wheel.GetRoll(0).firstVisibleRowIndex;

                        for (int i = 0; i < listElement.Count; i++)
                        {
                            for (int j = 0; j < listElement[i].Count; j++)
                            {
                                if (dicLink.TryGetValue(i * columeCount + j, out var item))
                                {
                                    var container = wheel.GetRoll(i).GetVisibleContainer(j);
                                    var wildConfig = elementConfigSet.GetElementConfig(item.SymbolId);
                                    container.UpdateElement(new SequenceElement(wildConfig,context));
                                    container.ShiftSortOrder(false);
                                    //Constant11004.SetLinkNumber(context,container);
                                }
                            }
                        }


                    }

                }
                
                lockElementLayer.Clear();

                //await context.WaitSeconds(2);

            }

            if (!lockData.IsTrigger)
            {
                context.view.Get<BaseTitleView11004>().PlayLinkPreAnim();
            }
        }


        protected async Task RefreshStopLock(int color,Wheel wheel,LionGoldGameResultExtraInfo.Types.LockData.Types.ItemDiff panelFrame)
        {
            List<Task> listTask = new List<Task>();
            for (int i = 0; i < panelFrame.Items.Count; i++)
            {
                var itemLock = panelFrame.Items[i];
                if (itemLock.Colour == color)
                {
                    listTask.Add(PlayStopLockItemAnim(wheel,itemLock));
                }
            }

            if (listTask.Count>0)
            {

                if (color == Constant11004.GreenFrameIndex)
                {
                    AudioUtil.Instance.PlayAudioFx("B03_Blink");
                    AudioUtil.Instance.PlayAudioFx("GreenFire_Appear");
                }
                else if (color == Constant11004.RedFrameIndex)
                {
                    AudioUtil.Instance.PlayAudioFx("B02_Blink");
                    AudioUtil.Instance.PlayAudioFx("RedFire_Appear");
                }

                await Task.WhenAll(listTask);
                await context.WaitSeconds(0.5f);
            }

        }

        protected async Task PlayStopLockItemAnim(Wheel wheel,LionGoldGameResultExtraInfo.Types.LockData.Types.Item itemLock)
        {
            int row = (int) itemLock.Y;
            int col = (int) itemLock.X;
            string nowName = lockElementLayer.GetContainsName(row, col);
            //当前有绿色框就不播放了
            if (nowName != Constant11004.GreenFrameName)
            {
                string objName = Constant11004.ListFrameName[(int)itemLock.Colour];

                if (objName != nowName && nowName!= Constant11004.WildFrameName)
                {
                    //先播放Element动画
                    var container = wheel.GetRoll(col).GetVisibleContainer(row);
                    if (container != null)
                    {
                        container.PlayElementAnimationAsync("Trigger");
                        container.ShiftSortOrder(true);
                    }

                    //Debug.LogError($"=============Color obj:{objName}");
                    //再播放框
                    var objLock = lockElementLayer.ReplaceOrAttachToElement(objName, row, col);
                    
                    var animator = objLock.GetComponent<Animator>();
                    await XUtility.PlayAnimationAsync(animator, "Open", context);
                }
            }
        }

        public void RefreshLockNoAnim()
        {
            var lockData = context.state.Get<ExtraState11004>().GetLockData();
            if (lockData != null)
            {
                RefreshAllLock(lockData.Items,false);
            }
            else
            {
                lockElementLayer.Clear();
            }
        }


        public void RefreshAllLock(RepeatedField<LionGoldGameResultExtraInfo.Types.LockData.Types.Item> listItems,bool isAnim)
        {
            lockElementLayer.Clear();
            lockElementLayer.RefreshWheel(context.state.Get<WheelsActiveState11004>().GetRunningWheel()[0]);

            int numPlay = 0;
            for (int i = 0; i < listItems.Count; i++)
            {
                var itemLock = listItems[i];
                if (itemLock.Colour != 0)
                {
                    int row = (int) itemLock.Y;
                    int col = (int) itemLock.X;
                        
                    string nowName = lockElementLayer.GetContainsName(row,col);
                    if (nowName != Constant11004.WildFrameName)
                    {
                        string objName = Constant11004.ListFrameName[(int)itemLock.Colour];
                        var objLock = lockElementLayer.AttachToElement(objName, row, col);

                        if (isAnim)
                        {
                            var animator = objLock.GetComponent<Animator>();
                            animator.Play("Open");
                            numPlay++;
                        }
                    }
                }
            }

            if (numPlay > 0)
            {
                AudioUtil.Instance.PlayAudioFx("GreenFire_Appear");
            }

            context.view.Get<BaseTitleView11004>().RefreshUI();
        }
    }
}