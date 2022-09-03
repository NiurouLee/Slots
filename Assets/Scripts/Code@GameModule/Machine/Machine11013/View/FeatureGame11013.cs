using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class FeatureGame11013: TransformHolder
    {

        [ComponentBinder("StartButton")]
        protected BoxCollider2D btnStart;

        [ComponentBinder("StopButton")]
        protected BoxCollider2D btnStop;
        
        [ComponentBinder("StopButton2")]
        protected BoxCollider2D btnStopDisable;


        [ComponentBinder("IntegralText")]
        private TextMesh txtTotelGet;

        [ComponentBinder("BetText")]
        private TextMesh txtSelectMultiple;
        
        List<TextMesh> listMultiple = new List<TextMesh>();
        List<TextMesh> listMultipleLoop = new List<TextMesh>();
        
        protected Animator animator;

        private int indexSelectMult = 5;
        private int indexMoveSelectMult = 6;
        
        private List<int> listRandomMult = new List<int>();

        private int[] listRandomStaticNum = new[] {9, 9 ,8 ,8 ,7 ,7, 6 ,6, 5, 5 ,4, 3, 2, 1 }; 
        
        public FeatureGame11013(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            animator = this.transform.GetComponent<Animator>();

            Random.InitState(Time.frameCount);
            int maxCount = 7;
            for (int i = 0; i < maxCount; i++)
            {
                TextMesh txtMult = this.transform.Find($"Root/GameObject/CellBetText{i}").GetComponent<TextMesh>();

                if (i == indexMoveSelectMult)
                {
                    txtMult.text = listMultiple[indexSelectMult].text;
                }
                else
                {

                    int mult = GetRandomNumber();
                    txtMult.text = $"*{mult}";
                    listRandomMult.Add(mult);
                }

                listMultiple.Add(txtMult);
            }


            for (int i = 0; i < 100; i++)
            {
                int mult = GetRandomNumber();
                listRandomMult.Add(mult);
            }
            

            for (int i = 0; i < 3; i++)
            {
                SetMultipleItems($"Root/LoopGroup/Group1/CellBetText{i}",i);
            }
            for (int i = 3; i < 6; i++)
            {
                SetMultipleItems($"Root/LoopGroup/Group2/CellBetText{i}",i);
            }
            SetMultipleItems($"Root/LoopGroup/CellBetText{6}",6);
            SetMultipleItems($"Root/LoopGroup/CellBetText{7}",7);


            btnStart.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnStartClick);
            btnStop.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnStopClick);
            
            
            btnStop.gameObject.SetActive(false);
        }


        protected int GetRandomNumber()
        {
            int index = Random.Range(0, listRandomStaticNum.Length);
            int mult = listRandomStaticNum[index];
            return mult;
        }

        protected void SetMultipleItems(string path,int index)
        {
            TextMesh txtMult = this.transform.Find(path).GetComponent<TextMesh>();

            if (index == indexMoveSelectMult)
            {
                txtMult.text = listMultipleLoop[indexSelectMult].text;
            }
            else
            {
                if (index < listMultiple.Count)
                {
                    txtMult.text = listMultiple[index].text;
                }
                else
                {
                    int mult = Random.Range(1, 9);
                    txtMult.text = $"*{mult}";
                }

            }

            listMultipleLoop.Add(txtMult);
        }

        private SBonusProcess sBonusProcess = null;
        private async void OnBtnStopClick(PointerEventData obj)
        {
            AudioUtil.Instance.PlayAudioFx("Close");
            AudioUtil.Instance.FadeMusicTo(0.4f,1);
            btnStop.gameObject.SetActive(false);
            btnStopDisable.gameObject.SetActive(true);
            var extraState = context.state.Get<ExtraState11013>();
            sBonusProcess = await extraState.SendBonusProcess();
            
        }

        private async void OnBtnStartClick(PointerEventData obj)
        {
            AudioUtil.Instance.PlayMusic("Bg_FeatureGame_11013");
            AudioUtil.Instance.PlayAudioFx("Close");
            await XUtility.PlayAnimationAsync(animator, "Start",context,true);

            btnStop.gameObject.SetActive(true);
            StartLoop();
            
        }

        private async Task StartLoop()
        {
            try
            {
                int indexOffset = 0;
                while (sBonusProcess == null)
                {
                    await XUtility.PlayAnimationAsync(animator, "Loop", context);
                    indexOffset++;

                    if (sBonusProcess == null)
                    {

                        for (int i = 0; i < listMultiple.Count; i++)
                        {
                            int index = indexOffset + i;
                            index = index % listRandomMult.Count;
                            listMultiple[i].text = $"*{listRandomMult[index]}";
                            listMultipleLoop[i].text = $"*{listRandomMult[index]}";
                        }
                    }


                }






                await context.WaitNFrame(1);
                var extraState = context.state.Get<ExtraState11013>();
                var info = extraState.GetExtraInfo();
                string strMulti = $"*{info.MapMultiplier}";
                listMultiple[indexSelectMult].text = strMulti;
                listMultiple[indexMoveSelectMult].text = strMulti;
                listMultipleLoop[indexSelectMult].text = strMulti;
                listMultipleLoop[indexMoveSelectMult].text = strMulti;
                XUtility.PlayAnimationAsync(animator, "Stop", context);
                AudioUtil.Instance.PlayAudioFx("Hippocampus_Stop");
                await context.WaitSeconds(3.5f);
                txtSelectMultiple.text = $"*{info.MapMultiplier + 1}";
                ulong totalBet = info.MapBase * (info.MapMultiplier + 1);





                AudioUtil.Instance.PlayAudioFx("Feature_End");
                Tweener tweener = DOTween.To(() => (long) info.MapBase,
                    (num) => { txtTotelGet.text = num.GetCommaFormat(); }, (long) totalBet, 0.8f);

                context.AddTweener(tweener);
                tweener.onComplete += () =>
                {
                    context.RemoveTweener(tweener);
                    txtTotelGet.text = totalBet.GetCommaFormat();
                };



                await context.WaitSeconds(5.9f - 3.5f);
                _taskCompletionSource.SetResult(true);
                
                //AudioUtil.Instance.PlayMusic(context.machineConfig.audioConfig.GetBaseBackgroundMusicName());
                this.sBonusProcess = null;
                Close();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

        }


        private TaskCompletionSource<bool> _taskCompletionSource;
        public async Task Open()
        {
            
            PlayStartAudio();
            btnStopDisable.gameObject.SetActive(false);
            var extraState = context.state.Get<ExtraState11013>();
            txtSelectMultiple.text = "*1";
            txtTotelGet.text = extraState.GetExtraInfo().MapBase.GetCommaFormat();
            
            
            this.transform.gameObject.SetActive(true);
            await XUtility.PlayAnimationAsync(animator, "Open",context,true);
            _taskCompletionSource = new TaskCompletionSource<bool>();

            
          
            await _taskCompletionSource.Task;
        }

        private async void PlayStartAudio()
        {
            AudioUtil.Instance.StopMusic();
            await AudioUtil.Instance.PlayAudioFxAsync("FeatureGame_StartPanel");
            
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }

    }
}