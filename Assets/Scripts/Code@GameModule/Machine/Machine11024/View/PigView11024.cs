using System;
using System.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameModule
{
    public class PigView11024:TransformHolder
    {
        public int nowLevel = 0;
        public Animator animator;
        private int pigType;
        public PigView11024(Transform inTransform,int inPigType):base(inTransform)
        {
            animator = transform.GetComponent<Animator>();
            pigType = inPigType;
        }

        public void InitAfterBindingContext()
        {
            
        }

        public void InitState(int targetLevel)
        {
            nowLevel = targetLevel;
            XUtility.PlayAnimation(animator,"Idle" + (nowLevel+1));
            RandomPlayIdle();
        }

        public async Task CollectCoin()
        {
            await XUtility.PlayAnimationAsync(animator,"Collect" + (nowLevel+1));
        }

        public async Task Glow(int targetLevel)
        {
            if (targetLevel > nowLevel)
            {
                await XUtility.PlayAnimationAsync(animator,"Turn" + (nowLevel+1)+"_"+(targetLevel+1));
                nowLevel = targetLevel;
            }
            else if (targetLevel < nowLevel)
            {
                throw new Exception("小猪等级倒着涨");
            }
        }

        public async Task Explode()
        {
            if (nowLevel != 3)
            {
                throw new Exception("小猪等级没满就炸了");
            }
            await XUtility.PlayAnimationAsync(animator,"Bomb");
        }
        
        public void RandomPlayIdle()
        {
            float idleLength = 4f;
            int partNum = 12;
            var randomNum = Random.Range(0, partNum);
            float randomNormalizedNum = (float)randomNum/partNum;
            var skeletonAnimation = transform.Find("New SkeletonAnimation").GetComponent<SkeletonAnimation>();
            var animationState = skeletonAnimation.state;
            string animationName;
            if (pigType == 1)
            {
                animationName = "Idle";
                if (nowLevel >= 2)
                {
                    animationName = "Idle3";
                }
            }
            else
            {
                animationName = "IdleA";
                if (nowLevel >= 2)
                {
                    animationName = "IdleC";
                }
            }
            if (animationState != null)
            {
                var animation = animationState.SetAnimation(0, animationName, true);
                animation.TrackTime = idleLength * randomNormalizedNum;   
            }
            else
            {
                XDebug.Log("animationState is null");
            }
        }
    }
}