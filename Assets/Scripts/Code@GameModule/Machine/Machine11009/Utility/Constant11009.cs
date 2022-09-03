using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using SRDebugger.UI.Other;
using UnityEngine;

namespace GameModule.Utility
{
    public class Constant11009
    {

        public static int wheelDesignHeight = 750;
        
        public static int jackpotPanelHeight = 330;

        public static readonly int IndexGreen = 0;
        public static readonly int IndexRed = 1;
        public static readonly int IndexPurple = 2;

        public static readonly uint ElementIdWild = 12;
        
        /// <summary>
        /// 绿色甲虫id
        /// </summary>
        public static readonly uint ElementIdGreen = 13;

        /// <summary>
        /// 红色甲虫id
        /// </summary>
        public static readonly uint ElementIdRed = 14;

        /// <summary>
        /// 紫色甲虫id
        /// </summary>
        public static readonly uint ElementIdPurple = 15;


        public static readonly List<uint> ListElementIdAllJackpotVaraint = new List<uint>()
        {
            20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38
        };


        public static readonly List<uint> ListElementIdJackpot4 = new List<uint>()
        {
            20,21,22,23,24
        };
        
        public static readonly List<uint> ListElementIdJackpot3 = new List<uint>()
        {
            25,26,27,28,29
        };
        
        public static readonly List<uint> ListElementIdJackpot2 = new List<uint>()
        {
            30,31,32,33,34
        };
        
        public static readonly List<uint> ListElementIdJackpot1 = new List<uint>()
        {
            35,36,37,38
        };

        public static readonly List<uint> ListElementIdSpinVaraint = new List<uint>()
        {
            39,40,41,42
        };
        
        public static readonly List<uint> ListElementIdCoinVaraint = new List<uint>()
        {
            43,44,45,46
        };
        
        
        public static readonly List<uint> ListElementIdGoldenVaraint = new List<uint>()
        {
            20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,
            40,42,44,46
        };
        
        
        public static readonly List<uint> ListElementIdPurpleVaraint = new List<uint>()
        {
            39,41,43,45
        };


        public static readonly List<uint> ListCollectElementId = new List<uint>()
        {
            ElementIdGreen,ElementIdRed,ElementIdPurple
        };


       


        public static async Task FlyElement(MachineContext context,ElementContainer container,Transform tranTarget)
        {
            bool isJackpot = ListElementIdAllJackpotVaraint.Contains(container.sequenceElement.config.id);
            bool isPurple = ListElementIdPurpleVaraint.Contains(container.sequenceElement.config.id);
            //播动画
                
            //飞
            Vector3 startPos = container.transform.position;
            string nameFly = $"Active_{container.sequenceElement.config.name}Fly";

            if (isJackpot)
            {
                nameFly = "Active_B04Fly";
            }
            else if(isPurple)
            {
                nameFly = "Active_B03Fly";
            }


            var effectFly = context.assetProvider.InstantiateGameObject(nameFly, true);
            effectFly.transform.SetParent(context.transform,false);
            //effectFly.transform.parent = tranTarget;
            effectFly.transform.position = startPos;

            

            var animtorFly = effectFly.GetComponent<Animator>();
            animtorFly.Play("Fly");
            
            if (isJackpot)
            {
                //jackpot
                Transform tranSource = container.GetElement().transform;
                //Sprite spriteBGSource = tranSource.Find("LetterGroup/BG").GetComponent<SpriteRenderer>().sprite;
                Sprite spriteLetterSource = tranSource.Find("LetterGroup/Letter").GetComponent<SpriteRenderer>().sprite;
                Sprite spriteBGSource = null;
                if (ListElementIdJackpot1.Contains(container.sequenceElement.config.id))
                {
                    spriteBGSource =
                        context.assetProvider.GetSpriteInAtlas("machine_11009_symbol_BG_Lv1_1", "Slot1009AtlasView");
                }
                else if(ListElementIdJackpot2.Contains(container.sequenceElement.config.id))
                {
                    spriteBGSource =
                        context.assetProvider.GetSpriteInAtlas("machine_11009_symbol_BG_Lv2_1", "Slot1009AtlasView");
                }
                else if(ListElementIdJackpot3.Contains(container.sequenceElement.config.id))
                {
                    spriteBGSource =
                        context.assetProvider.GetSpriteInAtlas("machine_11009_symbol_BG_Lv3_1", "Slot1009AtlasView");
                }
                else if(ListElementIdJackpot4.Contains(container.sequenceElement.config.id))
                {
                    spriteBGSource =
                        context.assetProvider.GetSpriteInAtlas("machine_11009_symbol_BG_Lv4_1", "Slot1009AtlasView");
                }


                SpriteRenderer spriteRendererBG = animtorFly.transform.Find("GameObject/LetterGroup/BG")
                    .GetComponent<SpriteRenderer>();
                SpriteRenderer spriteRendererLetter = animtorFly.transform.Find("GameObject/LetterGroup/Letter")
                    .GetComponent<SpriteRenderer>();

                spriteRendererBG.sprite = spriteBGSource;
                spriteRendererLetter.sprite = spriteLetterSource;

            }
            
            //await context.WaitSeconds(0.4f);
                
            await XUtility.FlyAsync(effectFly.transform, startPos,
                tranTarget.position, 2, 0.6f,Ease.InSine,context);

            context.assetProvider.RecycleGameObject(nameFly, effectFly);
        }


        public static async Task FlipElement(MachineContext context, ElementContainer container,LogicStepProxy stepProxy)
        {
            uint elementId = container.sequenceElement.config.id;

            AudioUtil.Instance.PlayAudioFx("CoinReverse");
            
            if (ListElementIdAllJackpotVaraint.Contains(elementId))
            {

                if (ListElementIdJackpot1.Contains(container.sequenceElement.config.id))
                {
                    await container.PlayElementAnimationAsync("JackpotMini");
                }
                else if(ListElementIdJackpot2.Contains(container.sequenceElement.config.id))
                {
                    await container.PlayElementAnimationAsync("JackpotMinor");
                }
                else if(ListElementIdJackpot3.Contains(container.sequenceElement.config.id))
                {
                    await container.PlayElementAnimationAsync("JackpotMajor");
                }
                else if(ListElementIdJackpot4.Contains(container.sequenceElement.config.id))
                {
                    await container.PlayElementAnimationAsync("JackpotGrand");
                }
                
            }
            else if(ListElementIdSpinVaraint.Contains(elementId))
            {
                await container.PlayElementAnimationAsync("Spin");
            }
            else if(ListElementIdCoinVaraint.Contains(elementId))
            {
                
                container.PlayElementAnimationAsync("Coin");
                BetState betState = context.state.Get<BetState>();
                TextMesh text = container.GetElement().transform.Find("Coins/CountText").GetComponent<TextMesh>();
                
                ulong num = betState.GetPayWinChips(container.sequenceElement.config.GetExtra<ulong>("winRate"));
                text.text = num.GetAbbreviationFormat(1);

                await context.WaitSeconds(0.666f);
                
                float addTime = 0.5f;
                stepProxy.AddWinChipsToControlPanel(num,addTime,true,false);

                //float time = context.view.Get<ControlPanel>().GetNumAnimationEndTime();
                await context.WaitSeconds(addTime);
            }
            
            

        }


    }
}