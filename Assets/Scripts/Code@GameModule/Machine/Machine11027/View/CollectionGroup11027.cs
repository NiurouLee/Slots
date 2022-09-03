using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class CollectionGroup11027 : TransformHolder
    {
        [ComponentBinder("Stage1")] protected Transform collectionGroup1;

        [ComponentBinder("Stage2")] protected Transform collectionGroup2;

        [ComponentBinder("Stage3")] protected Transform collectionGroup3;

        [ComponentBinder("Stage4")] protected Transform collectionGroup4;

        [ComponentBinder("Stage5")] protected Transform collectionGroup5;

        [ComponentBinder("animationNode0")] protected Transform animationNode0;

        [ComponentBinder("animationNode1")] protected Transform animationNode1;

        [ComponentBinder("animationNode2")] protected Transform animationNode2;
        
        [ComponentBinder("animationNode3")] protected Transform animationNode3;

        [ComponentBinder("animationNode4")] protected Transform animationNode4;
        
        [ComponentBinder("EFX")] protected Transform particle;
        
        [ComponentBinder("Active")] protected Transform activeParticle;

        protected Transform collectionGroups;
        
        protected Transform[] collectionGroupsNodes;
        
        protected Transform[] animationNodes;
        
        private float basicRandomTime = 4.0f;
        
        private float randomTime = 0f;
        
        private bool isRand = false;
        
        private uint oldLevel = 0;
        
        private int collectionGroupsIndex = 0;
        
        private ExtraState11027 _extraState11027;

        private Animator animator;

        public CollectionGroup11027(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            collectionGroupsNodes = new[] {collectionGroup1, collectionGroup2, collectionGroup3, collectionGroup4, collectionGroup5};
            animationNodes = new[] {animationNode0, animationNode1, animationNode2, animationNode3, animationNode4};
            animator = transform.GetComponent<Animator>();
            animator.keepAnimatorControllerStateOnDisable = true;
        }

        public void PlayPickTranstion()
        {
            animator.Play("Active");
        }
        
        public override void Update()
        {
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
        
        //彩蛋随机Idle动画，每次播放三个彩蛋同事播放Idle动画
        private void PlayIdle()
        {
            if (transform.gameObject.activeSelf)
            {
                _extraState11027 = context.state.Get<ExtraState11027>();
                // uint level = _extraState11027.GetPrizeLevel();
                // int index = (int)level;
                List<int> randomList = new List<int>();
                List<Animator> _animators = new List<Animator>();
                List<Transform> _activeTran = new List<Transform>();
                // collectionGroupsNodes[index].transform.gameObject.SetActive(true);
                var childCount = collectionGroupsNodes[collectionGroupsIndex].transform.GetActiveChildCount();
                for (var i = 0; i < childCount; i++)
                {
                    _animators.Add(collectionGroupsNodes[collectionGroupsIndex].GetChild(i).transform
                        .GetComponent<Animator>());
                    _animators[i].keepAnimatorControllerStateOnDisable = true;
                    _activeTran.Add(collectionGroupsNodes[collectionGroupsIndex].GetChild(i));
                }

                //每次随机三个
                for (var t = 0; t < 3; t++)
                {
                    int rand = Random.Range(0, childCount);
                    randomList.Add(rand);
                }

                for (var r = 0; r < randomList.Count; r++)
                {
                    _animators[randomList[r]].Play("EggIdle", 0, 0);
                }

                randomTime = basicRandomTime;
                isRand = true;
            }
        }

        public async Task ShowCollectionGroup(bool active = false)
        {
            isRand = false;
            var _extraState11027 = context.state.Get<ExtraState11027>();
            uint level = _extraState11027.GetPrizeLevel();
            int index = (int)level;
            collectionGroupsIndex = index;
            for (var i = 0; i < collectionGroupsNodes.Length; i++)
            {
                if (i == index)
                {
                    collectionGroupsNodes[i].transform.gameObject.SetActive(true);
                }
                else
                {
                    collectionGroupsNodes[i].transform.gameObject.SetActive(false);
                }
            }
            if(active)
            {
                PlayActive();
                ShowParticle();
            }
            isRand = true;
            PlayIdle();
        }
        public void PlayActive()
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            uint level = _extraState11027.GetPrizeLevel();
            int index = (int) level;
            List<Animator> _activeAnimators = new List<Animator>();
            collectionGroupsNodes[index].transform.gameObject.SetActive(true);
            var childCount = collectionGroupsNodes[index].transform.GetActiveChildCount();
            for (var i = 0; i < childCount; i++)
            {
                _activeAnimators.Add(collectionGroupsNodes[index].GetChild(i).transform.GetComponent<Animator>());
                _activeAnimators[i].keepAnimatorControllerStateOnDisable = true;
            }
            for (var r = 0; r < childCount; r++)
            {
                _activeAnimators[r].Play("EggActive", 0, 0);
            }
        }

        private async Task ShowParticle()
        {
            _extraState11027 = context.state.Get<ExtraState11027>();
            uint level = _extraState11027.GetPrizeLevel();
            if (oldLevel != level)
            {
                particle.transform.gameObject.SetActive(true);
                oldLevel = level;
                await context.WaitSeconds(1.0f);
                HideParticle();
                // int index = (int) level;
                // List<Animator> _activeAnimators = new List<Animator>();
                // collectionGroupsNodes[index].transform.gameObject.SetActive(true);
                // var childCount = collectionGroupsNodes[index].transform.GetActiveChildCount();
                // for (var i = 0; i < childCount; i++)
                // {
                //     _activeAnimators.Add(collectionGroupsNodes[index].GetChild(i).transform.GetComponent<Animator>());
                //     _activeAnimators[i].keepAnimatorControllerStateOnDisable = true;
                // }
                //
                // for (var r = 0; r < childCount; r++)
                // {
                //     _activeAnimators[r].Play("EggActive", 0, 0);
                // }
                //
                // await context.WaitSeconds(1.0f);
            }
            else
            {
                activeParticle.transform.gameObject.SetActive(true);
                await context.WaitSeconds(1.0f);
                HideParticle();
            }
        }

        public void ShowFeatrue()
        {
            particle.transform.gameObject.SetActive(true);
        }
        
        public void HideParticle()
        {
            particle.transform.gameObject.SetActive(false);
            activeParticle.transform.gameObject.SetActive(false);
        }
        
        public Vector3 GetIntegralAnimationNodePos()
        {
            var _extraState11027 = context.state.Get<ExtraState11027>();
            uint level = _extraState11027.GetPrizeLevel();
            int index = (int) level;
            return animationNodes[index].transform.position;
        }
    }
}