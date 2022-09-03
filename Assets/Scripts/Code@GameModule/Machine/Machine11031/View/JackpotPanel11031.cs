using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class JackpotPanel11031:JackPotPanel
    {
        protected List<Transform> jackPotRemindNodes = new List<Transform>();

        public JackpotPanel11031(Transform inTransform) : base(inTransform)
        {
            for (var i = 1; i < 6; i++)
            {
                jackPotRemindNodes.Add( transform.Find($"BG/Jackpot_Remind{i}"));
            }
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        public Vector3 GetIntegralPos(int jackPotId)
        {
            return jackPotRemindNodes[jackPotId - 1].transform.position;
        }

        //在base下播放Idle
        public void PlayJackPotRemindIdle()
        {
            for (var i = 0; i < jackPotRemindNodes.Count; i++)
            {
                 jackPotRemindNodes[i].transform.gameObject.GetComponent<Animator>().Play("Idle");
            }
        }

        public void ShowJackPotRemind()
        {
            var jackPotTitleList = context.state.Get<ExtraState11031>().GetJackpotTitleList();
            for (var index = 0; index < jackPotTitleList.Count; index++)
            {
                string aniName = "";
                if (jackPotTitleList[index].Num > 0)
                {
                    if (jackPotTitleList[index].Num % 3 == 0)
                    {
                      //  jackPotTitleList[index].Num = 3;
                        aniName = "Remind0";
                    }
                    else if (jackPotTitleList[index].Num % 3 == 1)
                    {
                       // jackPotTitleList[index].Num = 1;
                        aniName = "Remind1_Idle";
                    }
                    else if (jackPotTitleList[index].Num % 3 == 2)
                    {
                      //  jackPotTitleList[index].Num = 2;
                        aniName = "Remind2_Idle";
                    }

                    XUtility.PlayAnimation(jackPotRemindNodes[index].transform.gameObject.GetComponent<Animator>(),
                        aniName);
                }
                else
                {
                    XUtility.PlayAnimation(jackPotRemindNodes[index].transform.gameObject.GetComponent<Animator>(),
                        "Remind0_Idle");
                }
            }
        }

        public async Task PlayFlyToJackPot(uint jackPotId)
        {
            var jackpotAccNum = context.state.Get<ExtraState11031>().GetJackpotAccNum(jackPotId);
            var index = (int)jackPotId - 1;
           // string aniName = "";
         //   var animator = jackPotRemindNodes[index].transform.gameObject.GetComponent<Animator>();
            if (jackpotAccNum > 0)
            {
                var animationIndex = jackpotAccNum % 3;
                
                if (animationIndex == 0)
                {
                    animationIndex = 3;
                }
                // if (jackPotTitleList[index].Num % 3 == 0)
                // {
                //     jackPotTitleList[index].Num = 3;
                //     aniName = "Remind3";
                // }
                // else if (jackPotTitleList[index].Num % 3 == 1)
                // {
                //     jackPotTitleList[index].Num = 1;
                //     aniName = "Remind1";
                // }
                // else if (jackPotTitleList[index].Num % 3 == 2)
                // {
                //     jackPotTitleList[index].Num = 2;
                //     aniName = "Remind2";
                // }
                string aniName = "Remind" + animationIndex; 
                // if (animator.GetCurrentAnimatorStateInfo(0).IsName("Remind0_Idle"))
                // {
                //     aniName = "Remind1";
                // }
                // else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Remind1_Idle"))
                // {
                //     aniName = "Remind2";
                // }
                // else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Remind2_Idle"))
                // {
                //     aniName = "Remind3";
                // }
                
                await XUtility.PlayAnimationAsync(
                    jackPotRemindNodes[index].transform.gameObject.GetComponent<Animator>(), aniName);
            }
        } 
        public void PLayOpenJackPotPanel()
        {
            AudioUtil.Instance.PlayAudioFx("Jackpot_Open");
            for (var i = 0; i < jackPotRemindNodes.Count; i++)
            {
                jackPotRemindNodes[i].transform.gameObject.GetComponent<Animator>().Play("Remind0");
            }
        }

        public void ReStartShowJackPotPanel(int jackPotId)
        {
            jackPotRemindNodes[jackPotId - 1].transform.gameObject.GetComponent<Animator>().Play("Remind0_Idle");
        }
    }
}