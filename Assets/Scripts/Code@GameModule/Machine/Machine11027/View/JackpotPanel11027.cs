using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class JackpotPanel11027:JackPotPanel
    {
        private List<bool> _lockState;
        private Animator animator;
        private List<Animator> _animators;
        private float basicRandomTime = 3.0f;
        private float randomTime = 0f;
        public bool isRand = false;
        private List<int> jpIdleList;
        
        [ComponentBinder("flyNode1")] protected Transform animationNode0;

        [ComponentBinder("flyNode2")] protected Transform animationNode1;

        [ComponentBinder("flyNode3")] protected Transform animationNode2;
        
        [ComponentBinder("flyNode4")] protected Transform animationNode3;

        [ComponentBinder("flyNode5")] protected Transform animationNode4;
        
        [ComponentBinder("flyNode6")] protected Transform animationNode5;
        
        protected Transform[] animationNodes;

        public JackpotPanel11027(Transform inTransform) : base(inTransform)
        {
            _animators = new List<Animator>();
            jpIdleList = new List<int>();
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                _animators.Add(listTextJackpot[i].transform.parent.GetComponent<Animator>());
               _animators[i].keepAnimatorControllerStateOnDisable = true;
               jpIdleList.Add(i);
            }
            animationNodes = new[] {animationNode0, animationNode1, animationNode2, animationNode3, animationNode4,animationNode5};
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext); 
            jackpotInfoState = context.state.Get<JackpotInfoState>(); 
            UpdateJackpotValue();
            PlayIdle();
        }
        
        public override void Update()
        {
            base.Update();
            if (isRand)
            {
                randomTime -= Time.deltaTime;
                if (randomTime <= 0)
                {
                    isRand = false;
                    randomTime = basicRandomTime;
                    PlayIdle();
                }
            }
        }
        private void PlayIdle()
        {
            List<int> randomList = new List<int>();
            int i = 0;
            if (jpIdleList.Count < 2)
            {
                for (var e = 0; e < jpIdleList.Count; e++)
                {
                    randomList.Add(jpIdleList[e]);
                }
            }
            else
            {
                List<int> tempEggIdle = new List<int>(jpIdleList);
                for (var t = 0; t < 1; t++)
                {
                    int rand = Random.Range(0, tempEggIdle.Count);
                    int randV = tempEggIdle[rand];
                    tempEggIdle.RemoveAt(rand);
                    randomList.Add(randV);
                }
            }
            for (var r = 0; r < randomList.Count; r++)
            {
                _animators[randomList[r]].Play("Idle", 0, 0);
            }
            randomTime = basicRandomTime;
            isRand = true;
        }

        //锁定动画
        public async Task UpdateJackpotLockState(int jackpotId)
        {
             isRand = false;
             if (jpIdleList.Contains(jackpotId - 1))
             {
                 jpIdleList.Remove(jackpotId - 1);
             }
             listTextJackpot[jackpotId -1].transform.parent.Find("DisableState/BG").gameObject.SetActive(true);
             _animators[jackpotId -1].Play("Disable",0,0);
             await context.WaitSeconds(2.0f);
             isRand = true;
        }
        
        //断线重连时播放DisableIdle
        public void PlayJackpotLockStateIdle(int jackpotId)
        {
            if (jpIdleList.Contains(jackpotId - 1))
            {
                jpIdleList.Remove(jackpotId - 1);
            }

            _animators[jackpotId - 1].Play("DisableIdle", 0, 0);
        }

        public void RecoverJackpotNormalState()
        {
            for (var i = 0; i < listTextJackpot.Count; i++)
            {
                listTextJackpot[i].transform.parent.Find("DisableState/BG").gameObject.SetActive(false);
            }
        }

        public Vector3 GetJackpotPos()
        {
            return transform.position;
        }
        
        public Vector3 GetAnimationNodePos(uint jackPotId)
        {
            return listTextJackpot[(int) jackPotId - 1].transform.position;
        }
    }
}