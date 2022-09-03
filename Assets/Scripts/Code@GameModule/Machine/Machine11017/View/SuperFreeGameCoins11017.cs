using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class SuperFreeGameCoins11017: TransformHolder
    {

        [ComponentBinder("BaseWheelCoin")]
        protected Transform baseWheelCoin1;
        
        [ComponentBinder("BaseWheelCoin (1)")]
        protected Transform baseWheelCoin2;

        [ComponentBinder("BaseWheelCoin (2)")]
        protected Transform baseWheelCoin3;

        [ComponentBinder("BaseWheelCoin (3)")]
        protected Transform baseWheelCoin4;

        [ComponentBinder("BaseWheelCoin (4)")]
        protected Transform baseWheelCoin5;

        public SuperFreeGameCoins11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public async Task setWheelCoin()
        {
            var extraState = context.state.Get<ExtraState11017>();
            uint level = extraState.GetLevel();
            uint darktogold = level;
            uint gold = level;
            Debug.Log("level:"+level);
            if (transform)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Animator>().Play("BaseWheelCoinDark", 0, 0);
                }
                await XUtility.WaitSeconds(0.1f);
               
                for (int k = 0; k < transform.childCount; k++)
                {
                    if (darktogold <= 0) break;
                    --darktogold;
                    AudioUtil.Instance.PlayAudioFx("W01_Free_Light up");
                    transform.GetChild(k).GetComponent<Animator>().Play("DarkToGold", 0, 0);
                }
               
                if (level == 5)
                {
                     AudioUtil.Instance.PlayAudioFx("SuperFree_Flashing");
                }
                await XUtility.WaitSeconds(0.3f);
              
                for (int m = 0; m < transform.childCount; m++)
                {
                    if (gold <= 0) break;
                    --gold;
                    transform.GetChild(m).GetComponent<Animator>().Play("BaseWheelCoinGold", 0, 0);
                }
            }
        }
        
        public async Task SetWheelCoinIdle()
        {
            var extraState = context.state.Get<ExtraState11017>();
            uint level = extraState.GetLevel();
            uint gold = level;
            int dark = (int)(4 - level);
          
            if (transform)
            {
                for (int n = transform.childCount - dark; n < transform.childCount; n++)
                {
                    if (dark <= 0) return;
                    transform.GetChild(n).GetComponent<Animator>().Play("DarkIdle", 0, 0);
                } 
                
                await context.WaitSeconds(0.1f);
             
                if (gold > 0)
                { 
                    for (int m = 0; m <gold; m++) 
                    { 
                        transform.GetChild(m).GetComponent<Animator>().Play("GoldIdle", 0, 0); 
                    }
                }
                // await context.WaitSeconds(0.1f);
            }
        }
    }
}