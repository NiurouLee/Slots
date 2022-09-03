using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class MapView11024:TransformHolder
    {
        private ExtraState11024 _extraState;
        public ExtraState11024 extraState
        {
            get
            {
                if (_extraState == null)
                {
                    _extraState =  context.state.Get<ExtraState11024>();
                }
                return _extraState;
            }
        }

        public bool btnEnable;
        public static float beforeJumpTime = 0.33f;
        public static float jumpTime = 0.33f;
        public Animator animator;
        // public List<int> bigPointList;
        public int nowLevel;
        public bool isOpen;
        public Transform closeBtn;
        public Transform myHead;
        public List<Transform> pointList = new List<Transform>();
        public MapView11024(Transform inTransform):base(inTransform)
        {
            // bigPointList = Constant11024.BigPointList;
            Hide();
            isOpen = false;
            btnEnable = false;
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
            pointList.Clear();
            for (var i = 0; i <= 25; i++)
            {
                pointList.Add(transform.Find("Root/MapView/Leven" + i));
            }
            myHead = transform.Find("Root/Coordinate");
            myHead.GetComponent<Animator>().keepAnimatorControllerStateOnDisable = true;
            closeBtn = transform.Find("Root/btn");
            closeBtn.gameObject.AddComponent<PointerEventCustomHandler>().BindingPointerClick(ClickCloseBtn);
        }

        public void InitAfterBindingContext()
        {
            
        }

        public void SetBtnEnable(bool inBtnEnable)
        {
            btnEnable = inBtnEnable;
        }
        public async Task OpenMap()
        {
            if (!isOpen)
            {
                isOpen = true;
                AudioUtil.Instance.PauseMusic();
                Show();
                context.view.Get<MachineSystemWidgetView>().Hide();
                context.view.Get<ControlPanel>().ShowSpinButton(false);
                context.view.Get<ControlPanel>().UpdateControlPanelState(false, true);
                SetBtnEnable(false);
                myHead.localPosition = GetHeadPosition(nowLevel);
                AudioUtil.Instance.PlayAudioFx("Map_Open");
                transform.Find("Root").gameObject.SetActive(true);
                await XUtility.PlayAnimationAsync(animator, "Open"); 
            }
        }

        public async Task CloseMap()
        {
            if (isOpen)
            {
                SetBtnEnable(false);
                await XUtility.PlayAnimationAsync(animator, "Close");
                transform.Find("Root").gameObject.SetActive(false);
                isOpen = false;
            }
        }
        
        public void InitState()
        {
            nowLevel = (int)extraState.GetMapLevel();
            var winHistory = extraState.GetMapData().TotalWinHistory;
            // var historyLength = winHistory.Count;
            for (var i = 1; i <= 25; i++)
            {
                if (!Constant11024.IsBigPoint(i))
                {
                    var winValue = winHistory[i - 1];
                    if (winValue > 0)
                    {
                        pointList[i].Find("NumText").GetComponent<TextMesh>().text = Constant11024.ChipNum2String(winHistory[i-1]);
                    }
                    else
                    {
                        pointList[i].Find("NumText").GetComponent<TextMesh>().text = "";
                    }
                }

                if (i > nowLevel)
                {
                    if (Constant11024.IsBigPoint(i))
                    {
                        pointList[i].Find("Fx_light").gameObject.SetActive(false);
                        pointList[i].Find("Fx_lightIdle").gameObject.SetActive(true);
                    }
                    else
                    {
                        pointList[i].Find("Bg_H").gameObject.SetActive(true);
                        pointList[i].Find("Bg2_D").gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (Constant11024.IsBigPoint(i))
                    {
                        pointList[i].Find("Fx_light").gameObject.SetActive(false);
                        pointList[i].Find("Fx_lightIdle").gameObject.SetActive(false);
                    }
                    else
                    {
                        pointList[i].Find("Bg_H").gameObject.SetActive(false);
                        pointList[i].Find("Bg2_D").gameObject.SetActive(true);
                    }
                }
            }
            myHead.localPosition = GetHeadPosition(nowLevel);
            XUtility.PlayAnimation(myHead.GetComponent<Animator>(),"Idle");
        }
        public async void ClickCloseBtn(PointerEventData clickPoint)
        {
            if (btnEnable)
            {
                context.DispatchInternalEvent(MachineInternalEvent.EVENT_CUSTOM_CLICK,BtnType11024.CloseMap);   
            }
        }

        public Vector3 GetHeadPosition(int targetLevel)
        {
            var pointPosition = pointList[targetLevel].localPosition;
            return pointPosition + new Vector3(0,0.2f,0);
        }
        public async Task MoveToNextLevel()
        {
            var targetLevel = (int)extraState.GetMapLevel();
            if (targetLevel > nowLevel)
            {
                nowLevel = targetLevel;
                var task = new TaskCompletionSource<bool>();
                AudioUtil.Instance.PlayAudioFx("Map_S01Move");
                XUtility.PlayAnimation(myHead.GetComponent<Animator>(),"Jump");
                await context.WaitSeconds(beforeJumpTime);
                myHead.DOKill();
                myHead.DOLocalMove(GetHeadPosition(targetLevel), jumpTime).OnComplete(async () =>
                {
                    if (Constant11024.IsBigPoint(targetLevel))
                    {
                        AudioUtil.Instance.PlayAudioFx("Map_S02MoveStop2");
                        pointList[targetLevel].Find("Fx_light").gameObject.SetActive(true);
                        pointList[targetLevel].Find("Fx_lightIdle").gameObject.SetActive(false);
                        await context.WaitSeconds(1f);
                    }
                    else
                    {
                        AudioUtil.Instance.PlayAudioFx("Map_S01MoveStop");
                        var lightCover = context.assetProvider.InstantiateGameObject("Fx_MapJump",true);
                        lightCover.SetActive(false);
                        lightCover.SetActive(true);
                        lightCover.transform.SetParent(pointList[targetLevel],false);
                        lightCover.transform.localPosition = Vector3.zero;
                        context.WaitSeconds(2f, () =>
                        {
                            context.assetProvider.RecycleGameObject("Fx_MapJump",lightCover);
                        });
                        var winHistory = extraState.GetMapData().TotalWinHistory;
                        pointList[targetLevel].Find("Bg_H").gameObject.SetActive(false);
                        pointList[targetLevel].Find("Bg2_D").gameObject.SetActive(true);
                        await context.WaitSeconds(0.2f);
                        pointList[targetLevel].Find("NumText").GetComponent<TextMesh>().text = Constant11024.ChipNum2String(winHistory[targetLevel-1]);
                        await context.WaitSeconds(0.5f);
                    }
                    task.SetResult(true);
                });
                await task.Task;
            }
            else if (targetLevel > nowLevel)
            {
                throw new Exception("地图等级倒退");
            }
            else
            {
                throw new Exception("地图等级未增长");
            }
        }
    }
}