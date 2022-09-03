using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class FreeGameInfomationView11006: TransformHolder
    {

        [ComponentBinder("GlobalCollect/CountText")]
        private TextMesh txtAllCollect;

        [ComponentBinder("RoundCollect/CountText")]
        private TextMesh txtCollectUpgrade;

        [ComponentBinder("RoundCollect/MakeChangeTargetIcon")]
        private SpriteRenderer spriteChange;

        [ComponentBinder("GlobalCollect")]
        private Animator animatorGlobal;

        [ComponentBinder("RoundCollect")]
        private Animator animatorRound;
        
        [ComponentBinder("RoundFinish")]
        private Animator animatorRoundFinish;

        [ComponentBinder("CollectTargetIcon")]
        private Transform tranCollectTargetIcon;


        private List<GameObject> listObjLight = new List<GameObject>();
        private List<GameObject> listObjDark = new List<GameObject>();
        private List<GameObject> listObjBuffalo = new List<GameObject>();
        
        

        private SequenceElement sequenceElement;
        public FreeGameInfomationView11006(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
            animatorRoundFinish.gameObject.SetActive(false);

            for (int i = 0; i < 4; i++)
            {
                listObjLight.Add(transform.Find($"ProgressCollect/ReplaceTargetIcon{i+1}Light")?.gameObject);
                listObjDark.Add(transform.Find($"ProgressCollect/ReplaceTargetIcon{i+1}Dark")?.gameObject);
                listObjBuffalo.Add(transform.Find($"ProgressCollect/ReplaceTargetIcon{i+1}Buffalo")?.gameObject);
            }
        }


        private ExtraState11006 extraState;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            extraState = context.state.Get<ExtraState11006>();
           
        }


        //private Coroutine coroRefreshUI;
        public async void RefreshUI(Action actionEnd)
        {
            // if (coroRefreshUI != null)
            // {
            //     context.StopCoroutine(coroRefreshUI);
            // }
            //
            // coroRefreshUI = context.StartCoroutine(RefreshUICoro(actionEnd));
            await RefreshUICoro();
            actionEnd?.Invoke();
        }

        private WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        
        private async Task RefreshUICoro()
        {
            
            if (extraState.GetOldBuffaloLevel() >= Constant11006.listBuffaloLevel2Upgrade.Count)
            {
                animatorRound.gameObject.SetActive(false);
                return;
            }
            animatorRound.gameObject.SetActive(true);
            
            //先刷新老数据
            string spriteName = Constant11006.listBuffaloLevel2SpriteName[extraState.GetOldBuffaloLevel()];
            Sprite spriteElement = context.assetProvider.GetSpriteInAtlas(spriteName,"Slot1006AtlasView");
            spriteChange.sprite = spriteElement;

            txtAllCollect.text = extraState.GetOldAllCollect().ToString();
            txtCollectUpgrade.text = extraState.GetOldCollectToUpgrade().ToString();


            if (extraState.GetOldAllCollect() <
                Constant11006.listBuffaloLevel2Upgrade[Constant11006.listBuffaloLevel2Upgrade.Count - 1])
            {
                animatorRoundFinish.gameObject.SetActive(false);
                animatorRound.gameObject.SetActive(true);
                XUtility.PlayAnimation(animatorRound, "Idle", null, context);
                //animatorRound.Play("Idle");
            }
            else
            {
                animatorRoundFinish.gameObject.SetActive(true);
                animatorRound.gameObject.SetActive(false);
                XUtility.PlayAnimation(animatorRoundFinish,"Idle", null, context);
                //animatorRoundFinish.Play("Idle");
            }
            

            //然后开始动画逐个加
            var wheel = context.view.Get<Wheel>();
            var highElementContainers = wheel.GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == Constant11006.highElementId)
                {
                    return true;
                }
                
                return false;
            });

            int numCollect = extraState.GetOldAllCollect();
            int numLevel = extraState.GetOldBuffaloLevel();
            int numUpgrad = extraState.GetOldCollectToUpgrade();

            for (int i = 0; i < highElementContainers.Count; i++)
            {
                var container = highElementContainers[i];
                AudioUtil.Instance.PlayAudioFx("B02_Change");
                container.PlayElementAnimation("Change",false, () =>
                {
                    if (sequenceElement == null)
                    {
                        var elementConfigSet = context.state.machineConfig.GetElementConfigSet();
                        sequenceElement = new SequenceElement(elementConfigSet.GetElementConfig(Constant11006.normalElementId),
                            context);
                    }

                    //container.UpdateElementMaskInteraction(false);
                    container.UpdateElement(sequenceElement);
                });

                await context.WaitSeconds(0.5f);
                
                //飞粒子
                var effectFly = context.assetProvider.InstantiateGameObject("Active_Fly",true);
                effectFly.transform.position = container.transform.position;
                effectFly.transform.parent = transform;
                await XUtility.FlyAsync(effectFly.transform, container.transform.position,
                    tranCollectTargetIcon.position, 0, 0.6f,Ease.Linear,context);
                context.assetProvider.RecycleGameObject("Active_Fly",effectFly);
                
                XUtility.PlayAnimation(animatorGlobal, "Win", null, context);

                
                numCollect++;
                txtAllCollect.text = numCollect.ToString();

                int newLevel = GetBuffaloLevel(numCollect);

                
                if (newLevel == -1)
                {
                    //收集完成
                    await XUtility.PlayAnimationAsync(animatorRound, "Change1", context);
                    animatorRound.gameObject.SetActive(false);
                    animatorRoundFinish.gameObject.SetActive(true);
                    XUtility.PlayAnimation(animatorRoundFinish, "Appear", null, context);
                    //animatorRoundFinish.Play("Appear");
                    RefreshProgress(newLevel);
                }
                else if (newLevel != numLevel)
                {
                    AudioUtil.Instance.PlayAudioFx("S01Change");
                    //切换等级头像
                    await XUtility.PlayAnimationAsync(animatorRound, "Change1", context);
                    
                    spriteName = Constant11006.listBuffaloLevel2SpriteName[newLevel];
                    spriteElement = context.assetProvider.GetSpriteInAtlas(spriteName,"Slot1006AtlasView");
                    spriteChange.sprite = spriteElement;
                    
                    numLevel = newLevel;
                    numUpgrad = Constant11006.listBuffaloLevel2Upgrade[numLevel] - numCollect;
                    txtCollectUpgrade.text = numUpgrad.ToString();
                    
                    await XUtility.PlayAnimationAsync(animatorRound, "Change2", context);
                    RefreshProgress(newLevel);
                }
                else
                {
                    numUpgrad = Constant11006.listBuffaloLevel2Upgrade[numLevel] - numCollect;
                    txtCollectUpgrade.text = numUpgrad.ToString();
                    await context.WaitSeconds(1);
                }
                
                //
            }
            
        }



        protected int GetBuffaloLevel(int allCollect)
        {
            for (int i = 0; i < Constant11006.listBuffaloLevel2Upgrade.Count ; i++)
            {
                if (Constant11006.listBuffaloLevel2Upgrade[i] > allCollect)
                {
                    return i;
                }
            }

            return -1;
        }


        public void RefreshUINoAnim()
        {
            txtAllCollect.text = extraState.GetAllCollect().ToString();
            RefreshProgress(extraState.GetBuffaloLevel());
            if (extraState.GetBuffaloLevel() >= Constant11006.listBuffaloLevel2Upgrade.Count)
            {
                animatorRound.gameObject.SetActive(false);
                animatorRoundFinish.gameObject.SetActive(true);
                XUtility.PlayAnimation(animatorRoundFinish, "Appear", null, context);
                //animatorRoundFinish.Play("Appear");
                return;
            }
            animatorRound.gameObject.SetActive(true);

            string spriteName = Constant11006.listBuffaloLevel2SpriteName[extraState.GetBuffaloLevel()];
            Sprite spriteElement = context.assetProvider.GetSpriteInAtlas(spriteName,"Slot1006AtlasView");
            spriteChange.sprite = spriteElement;

            
            txtCollectUpgrade.text = extraState.GetCollectToUpgrade().ToString();
        }


        public void RefreshProgress(int buffaloLevel)
        {
            if (buffaloLevel == -1 || buffaloLevel >= Constant11006.listBuffaloLevel2Upgrade.Count)
            {
                for (int i = 0; i < 4; i++)
                {
                    listObjBuffalo[i]?.SetActive(true);
                    listObjDark[i]?.SetActive(false);
                    listObjLight[i]?.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    listObjLight[i]?.SetActive(buffaloLevel == i);

                    listObjBuffalo[i]?.SetActive(i < buffaloLevel);

                    listObjDark[i]?.SetActive(i > buffaloLevel);
                }
            }
        }


        public void Open()
        {
            this.transform.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorGlobal, "Appear", null, context);
            //animatorGlobal.Play("Appear");
            animatorRound.gameObject.SetActive(true);
            XUtility.PlayAnimation(animatorRound, "Idle", null, context);
            //animatorRound.Play("Idle");
            animatorRoundFinish.gameObject.SetActive(false);
           
        }

        public void Close()
        {
            this.transform.gameObject.SetActive(false);
        }

    }
}