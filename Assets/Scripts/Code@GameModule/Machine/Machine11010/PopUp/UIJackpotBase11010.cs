//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-29 19:54
//  Ver : 1.0.0
//  Description : UIJackpotBase11010.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIJackpotBase11010:UIJackpotBase
    {
        private long lJackpotWin;
        private bool isClicked = false;
        private TaskCompletionSource<bool> taskWait;
        private float lastTimeUpdateData = 0;
        public UIJackpotBase11010(Transform inTransform) : base(inTransform)
        {
            lastTimeUpdateData = Time.realtimeSinceStartup;
            _textJackpotWinNum = transform
                .Find("Root/AnimationGroup/jackpot/yingqiankuang/MainGroup/IntegralGroup/IntegralText")
                .GetComponent<Text>();
        }
        public override async Task AutoClose()
        {
            await Task.CompletedTask;
        }

        public void SetWaitTask(TaskCompletionSource<bool> task)
        {
            taskWait = task;
        }
        public override void SetJackpotWinNum(ulong jackpotWin)
        {
            lJackpotWin = (long)jackpotWin;
            if (_textJackpotWinNum)
            {
                _textJackpotWinNum.DOCounter(0, (long)jackpotWin, 8f).OnComplete(() =>
                {
                    _textJackpotWinNum.text = jackpotWin.GetCommaFormat();
                    taskWait.TrySetResult(true);
                });   
            }
            if (_textMeshProUGUIJackpotWinNum)
            {
                _textMeshProUGUIJackpotWinNum.DOCounter(0, (long)jackpotWin, 8f).OnComplete(() =>
                {
                    _textMeshProUGUIJackpotWinNum.text = jackpotWin.GetCommaFormat();
                    taskWait.TrySetResult(true);
                });   
            }
        }

        [ComponentBinder("StopCounter")]
        private void OnBtnStopCounterClicked()
        {
            if (Time.realtimeSinceStartup - lastTimeUpdateData < 0.8f)
                return;

            if (isClicked) return;
            isClicked = true;
            if (_textJackpotWinNum)
            {
                DOTween.Kill(_textJackpotWinNum);   
            }
            if (_textMeshProUGUIJackpotWinNum)
            {
                DOTween.Kill(_textMeshProUGUIJackpotWinNum);   
            }
            if (_textJackpotWinNum)
            {
                _textJackpotWinNum.text = lJackpotWin.GetCommaFormat();
            }
            if (_textMeshProUGUIJackpotWinNum)
            {
                _textMeshProUGUIJackpotWinNum.text = lJackpotWin.GetCommaFormat();
            }
            taskWait.TrySetResult(true);
        }
    }
}