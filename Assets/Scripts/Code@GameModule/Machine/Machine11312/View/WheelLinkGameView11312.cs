using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
namespace GameModule
{
    public class WheelLinkGameView11312 : TransformHolder
    {
        public TextMesh SpinMaxText;
        public TextMesh SpinRemainText;
        public GameObject FrameGroup;
        public GameObject RewardGroup;
        public GameObject RespinEffect;
        public GameObject RespinEffects;
        public GameObject Lower;
        public List<List<int>> RespinExtraRolls = new List<List<int>>();
        public List<int> respinRolls = new List<int>() { 0, 0, 0 };
        public List<GameObject> Frames = new List<GameObject>();
        public uint lastLimitCount = 0;
        public uint lastCount = 0;
        public GameObject Free_turntable;
        public WheelLinkGameView11312(Transform transform) : base(transform)
        {
            SpinMaxText = transform.Find("SpinRemainingGroup/FeatureGroupL/Upper/SpinGroup/SpinMaxText").GetComponent<TextMesh>();
            SpinRemainText = transform.Find("SpinRemainingGroup/FeatureGroupL/Upper/SpinGroup/SpinRemainText").GetComponent<TextMesh>();
            FrameGroup = transform.Find("FrameGroup").gameObject;
            RewardGroup = transform.Find("SpinRemainingGroup/FeatureGroupL/Lower/RewardGroup").gameObject;
            RespinEffect = transform.Find("SpinRemainingGroup/FeatureGroupL/Upper/RESPIN").gameObject;
            RespinEffects = transform.Find("SpinRemainingGroup/FeatureGroupL/Upper/RESPINS").gameObject;
            Lower = transform.Find("SpinRemainingGroup/FeatureGroupL/Lower").gameObject;
            Lower.gameObject.SetActive(false);
            Free_turntable = transform.Find("SpinRemainingGroup/FeatureGroupR/Free_turntable").gameObject;

            // XDebug.Log(SpinMaxText.text);
            RespinExtraRolls.Add(new List<int> { 4, 11, 18, 25, 32 });
            RespinExtraRolls.Add(new List<int> { 5, 12, 19, 26, 33 });
            RespinExtraRolls.Add(new List<int> { 6, 13, 20, 27, 34 });
        }
        /// <summary>
        /// 设置4-6行隐藏
        /// </summary>
        public void SetAllRollLockedStatus()
        {
            XDebug.Log("respin---设置4-6行隐藏");
            var wheel = context.view.Get<Wheel>(2);
            for (int i = 0; i < RespinExtraRolls.Count; i++)
            {
                for (int j = 0; j < RespinExtraRolls[i].Count; j++)
                {
                    var curRollIndex = RespinExtraRolls[i][j];
                    var curRoll = wheel.GetRoll(curRollIndex) as SoloRoll;
                    curRoll.EnableSoloRollMask(false);
                    curRoll.transform.gameObject.SetActive(false);
                    var linkWheelState = context.state.Get<LinkWheelState11312>();
                    linkWheelState.SetRollLockState(curRollIndex, true);
                }
            }
        }

        /// <summary>
        /// 设置锁定行数状态，0-2  对应  4-6 
        /// </summary>
        /// <param name="rol"></param>
        public void SetOneRollLockedStatus(int rol)
        {
            var wheel = context.view.Get<Wheel>(2);
            for (int i = 0; i < RespinExtraRolls[rol].Count; i++)
            {
                var curRollIndex = RespinExtraRolls[rol][i];
                var curRoll = wheel.GetRoll(curRollIndex) as SoloRoll;
                curRoll.transform.gameObject.SetActive(true);
                curRoll.EnableSoloRollMask(true);
                var linkWheelState = context.state.Get<LinkWheelState11312>();
                linkWheelState.SetRollLockState(curRollIndex, false);
            }
        }
        /// <summary>
        /// 设置左边金币上的值
        /// </summary>
        public void RefreshLeftGoldNums(ulong winRate)
        {
            var chips = context.state.Get<BetState>().GetPayWinChips(winRate);
            RewardGroup.transform.Find("RewardText").GetComponent<TextMesh>().text = "" + chips.GetAbbreviationFormat(1);
        }
        public void ResetReawrdValue()
        {
            RewardGroup.transform.Find("RewardText").GetComponent<TextMesh>().text = "";
        }

        /// <summary>
        /// 刷新link次数
        /// </summary>
        /// <param name="linkCount"></param>
        /// <param name="linkLimit"></param>
        public void RefreshLinkCountShow(uint linkCount, uint linkLimit)
        {
            if (lastLimitCount < linkLimit)
                RespinEffects.GetComponent<Animator>().Play("Open");
            lastLimitCount = linkLimit;
            if (lastCount < linkCount)
                RespinEffect.GetComponent<Animator>().Play("Open");
            lastCount = linkCount;
            SpinMaxText.text = "" + linkLimit;
            SpinRemainText.text = "" + linkCount;
        }
        // 初始化断线重连用
        public void InitRefreshLinkCount(uint linkCount, uint linkLimit)
        {
            lastLimitCount = linkLimit;
            lastCount = linkCount;
            SpinMaxText.text = "" + linkLimit;
            SpinRemainText.text = "" + linkCount;
        }
        // 设置左边帆船是否显示
        public void SetLowerIsShow(bool isShow = false)
        {
            Lower.gameObject.SetActive(isShow);
            ResetReawrdValue();
        }

        /// <summary>
        /// 展示小game特效
        /// </summary>
        public async Task ShowSmallGameFx(string animName)
        {
            // var waitTask = new TaskCompletionSource<bool>();
            // context.AddWaitTask(waitTask, null);
            bool hasPlay = false;
            // var sequence = DOTween.Sequence();

            // //TODO:
            // //context.AddTween(sequence);
            // TweenCanNotPause.AddTween(sequence);
            // sequence.AppendCallback(() =>
            // {
            //     if (!hasPlay)
            //     {
            //         var anim = Free_turntable.GetComponent<Animator>();
            //         XUtility.PlayAnimation(anim, animName);
            //     }
            //     hasPlay = true;
            // });
            // sequence.AppendInterval(3);
            // sequence.AppendCallback(() =>
            // {
            //     TweenCanNotPause.RemoveTween(sequence);
            //     //  context.RemoveTween(sequence);
            //     context.RemoveTask(waitTask);
            //     waitTask.SetResult(true);
            // });
            // sequence.Play();
            // await waitTask.Task;
            // // await XUtility.PlayAnimationAsync(anim,animName);

            ///----重写----------
            if (!hasPlay)
            {
                var anim = Free_turntable.GetComponent<Animator>();
                XUtility.PlayAnimation(anim, animName);
            }
            hasPlay = true;
            await context.WaitSeconds(3f);
            //-----------------------------
        }

        /// <summary>
        /// 展示Frames
        /// </summary>
        public void ClearAllFrames()
        {
            foreach (var item in Frames)
            {
                GameObject.Destroy(item.gameObject);
            }
            Frames = new List<GameObject>();

        }
        public void ClearAllRespinRolls()
        {
            respinRolls = new List<int>() { 0, 0, 0 };
        }
        /// <summary>
        /// 轮盘高度改变
        /// </summary>
        /// <param name="rollNum"></param>
        public void LinkWheelLengthToChange(int rollNum)
        {
            for (int i = 0; i < respinRolls.Count; i++)
            {
                if (respinRolls[i] == 0 && rollNum > 0)
                {
                    rollNum--;
                    respinRolls[i] = 1;
                    SetOneRollLockedStatus(i);
                }
            }
        }

        /// <summary>
        /// 播放wheel的动画
        /// </summary>
        /// <param name="targetIndex"></param>
        /// <returns></returns>
        public void PlayAnim(int targetIndex, bool isInit = false)
        {
            var anim = transform.GetComponent<Animator>();
            var clips = anim.GetCurrentAnimatorClipInfo(0);
            if (clips.Length > 0)
            {
                var startIndex = int.Parse(clips[0].clip.name.Split('X')[0]);
                var resIndex = startIndex + targetIndex;
                var animName = "" + startIndex + "X5to" + resIndex + "X5";
                if (isInit)
                    animName = "" + resIndex + "X5";
                XUtility.PlayAnimation(anim, animName);
            }

        }
    }
}

