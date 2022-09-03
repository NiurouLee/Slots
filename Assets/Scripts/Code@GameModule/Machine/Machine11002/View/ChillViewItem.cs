using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class ChillViewItem
    {
        private Transform tranNumGroup;
        private TextMeshPro txtNum;
        private Animator _animatorNum;
        private Animator animRoll;
        private List<Animator> listChill = new List<Animator>();
        private GameObject _collectBoom;
        private int index;
        private MachineContext machineContext;
        private Wheel wheel;
        private Roll roll;
        private bool isFree;
        private Transform root;
        private Transform end;
        private ExtraState11002 extraState;
        private SortingGroup _sortingGroup;

        public bool IsFullWildActive()
        {
            return animRoll != null && animRoll.gameObject.activeInHierarchy;
        }

        public ChillViewItem(int index, Transform root, Animator animRoll,
            MachineContext machineContext, bool isFree)
        {
            this.machineContext = machineContext;
            this.index = index;
            this.animRoll = animRoll;
            this.isFree = isFree;
            this.root = root;

            end = root.Find("End");

            if (animRoll != null)
            {
                HideFullWild();
                _sortingGroup = animRoll.gameObject.AddComponent<SortingGroup>();
                _sortingGroup.sortingLayerName = "Element";
                _sortingGroup.sortingOrder = 100;
            }

            tranNumGroup = root.Find("ChillNumGroup");
            txtNum = root.Find("ChillNumGroup/NumText").GetComponent<TextMeshPro>();
            _animatorNum = root.Find("ChillNumGroup").GetComponent<Animator>();

            _collectBoom = root.Find("B03_CollectBoom").gameObject;
            _collectBoom.gameObject.SetActive(false);

            for (int i = 0; i < 3; i++)
            {
                listChill.Add(root.Find($"ChillCell{i}").GetComponent<Animator>());
                if (listChill[i].gameObject.activeInHierarchy)
                    listChill[i].Play("Permanent");
            }

            if (isFree)
            {
                wheel = machineContext.view.Get<Wheel>(1);
                roll = wheel.GetRoll(index);
            }
            else
            {
                wheel = machineContext.view.Get<Wheel>();
                roll = wheel.GetRoll(index);
            }

            extraState = machineContext.state.Get<ExtraState11002>();
        }

        public void PlayeAnim(Animator animator, string name)
        {
            if (animator.gameObject.activeInHierarchy) { animator.Play(name); }
        }

        private bool isStartSpin = false;
        private uint mNowChill;
        private uint mLastChill;
        private bool isChangeWild;

        public async Task RefreshUI(uint nowChill, uint lastChill, bool isActive,
            bool isStartSpin, bool isAnim, bool isReConnect, bool isChangeWild)
        {
            this.mNowChill = nowChill;
            this.mLastChill = lastChill;
            this.isChangeWild = isChangeWild;
            this.isStartSpin = isStartSpin;

            ClearCoro();
            ReszieContainer();
            if (isActive)
            {
                tranNumGroup.gameObject.SetActive(true);

                StartSetChildSprteNumCoro(nowChill, isAnim, () =>
                {
                    if (nowChill == 3 && !isStartSpin && isAnim)
                    {
                        PlayeAnim(_animatorNum, "In");
                        AudioUtil.Instance.PlayAudioFxIfNotPlaying("Number_Appear");
                    }
                    else
                    {
                        PlayeAnim(_animatorNum, "Idle");
                    }

                    txtNum.text = nowChill.ToString();

                    if (isChangeWild && animRoll != null)
                    {
                        bool isRollAnim = false;
                        //切换bet后，有小辣椒，没wild的，需要重新播动画
                        if (!animRoll.gameObject.activeInHierarchy && !isReConnect)
                        {
                            if (root.gameObject.activeInHierarchy && isStartSpin)
                            {
                                isRollAnim = true;
                                StartRollAnim();
                            }
                        }
                        if (!isRollAnim)
                        {
                            if (isStartSpin)
                            {
                                PlayFullWildFade();
                            }
                            else
                            {
                                PlayFullWildIdle();
                            }
                        }
                    }

                    if (isStartSpin && nowChill > 0)
                    {
                        if (isActive) { UpdateChillNum(nowChill - 1); }
                        txtNum.text = (nowChill - 1).ToString();
                    }
                });
            }
            else
            {
                tranNumGroup.gameObject.SetActive(false);
                PlayeAnim(_animatorNum, "Permanent");

                if (isChangeWild && animRoll != null)
                {
                    HideFullWild();
                }

                StartSetChildSprteNumCoro(nowChill, isAnim, () =>
                {
                    if (nowChill == 3)
                    {
                        //spin结束，刚进入wild
                        tranNumGroup.gameObject.SetActive(true);
                        if (!isStartSpin && isAnim)
                        {
                            PlayeAnim(_animatorNum, "In");
                            AudioUtil.Instance.PlayAudioFxIfNotPlaying("Number_Appear");
                        }
                        else
                        {
                            PlayeAnim(_animatorNum, "Idle");
                        }

                        txtNum.text = nowChill.ToString();
                    }


                    if (isStartSpin && nowChill > 0)
                    {
                        if (txtNum.gameObject.activeInHierarchy)
                        {
                            UpdateChillNum(nowChill - 1);
                        }
                        
                        txtNum.text = (nowChill - 1).ToString();
                    }
                });

                if (nowChill == 3)
                {
                    //spin结束，刚进入wild
                    if (root.gameObject.activeInHierarchy && isStartSpin)
                    {
                        StartRollAnim();
                    }
                }
            }

            if (tranNumGroup.gameObject.activeSelf)
            {
                if (isStartSpin && nowChill > 0)
                {
                    if (txtNum.gameObject.activeInHierarchy)
                    {
                        UpdateChillNum(nowChill - 1);
                    }
                    
                    txtNum.text = (nowChill - 1).ToString();
                }
            }

            if (isStartSpin && nowChill == 0)
            {
                tranNumGroup.gameObject.SetActive(false);

                if (isChangeWild && animRoll != null)
                {
                    HideFullWild();
                }
                StartSetChildSprteNumCoro(nowChill, isAnim, null);
            }
        }

        private void ClearCoro()
        {
            if (coroSetChildSpriteNum != null)
            {
                machineContext.StopCoroutine(coroSetChildSpriteNum);
            }

            if (tweenerFly != null) { tweenerFly.Kill(); }

            if (effectCollect != null) { GameObject.Destroy(effectCollect); }

            if (effectBoomEnd != null) { GameObject.Destroy(effectBoomEnd); }
        }

        private Coroutine coroSetChildSpriteNum;
        private void StartSetChildSprteNumCoro(uint nowChill, bool isAnim, Action acitonEnd)
        {
            ClearCoro();

            coroSetChildSpriteNum = machineContext.StartCoroutine(SetChildSprteNum(nowChill, isAnim, acitonEnd));
        }

        protected void ReszieContainer()
        {
            for (int i = 0; i < listTweenerRollAnim.Count; i++)
            {
                listTweenerRollAnim[i]?.Kill();
            }
            listTweenerRollAnim.Clear();

            int count = isFree ? 6 : 3;
            for (int i = 0; i < count; i++)
            {
                var container = roll.GetVisibleContainer(i);
                container.transform.localScale = Vector3.one;
            }
        }

        private List<Tweener> listTweenerRollAnim = new List<Tweener>();


        private void HideFullWild()
        {
            if (animRoll != null && animRoll.gameObject.activeInHierarchy)
            {
                animRoll.gameObject.SetActive(false);
            }
        }

        public void PlayFullWildWin()
        {
            animRoll.gameObject.SetActive(true);
            _sortingGroup.sortingOrder = 201;
            PlayeAnim(animRoll, "Win");
            
            XDebug.Log("PlayFullWildWin" + this.index);
        }

        public void PlayFullWildIdle()
        {
            animRoll.gameObject.SetActive(true);
            _sortingGroup.sortingOrder = 100;
            PlayeAnim(animRoll, "Idle");
        }

        public void PlayFullWildFade()
        {
            animRoll.gameObject.SetActive(true);
            _sortingGroup.sortingOrder = 201;
            PlayeAnim(animRoll, "Loop");
        }

        protected async Task StartRollAnim()
        {
            if (!isChangeWild) { return; }

            // SetWildLock(index, true, false);
            //TODO:roll背景出现动画
            if (animRoll != null)
            {
                animRoll.gameObject.SetActive(true);
                _sortingGroup.sortingOrder = 201;

                AudioUtil.Instance.PlayAudioFxIfNotPlaying(isFree ? "W03_Appear" : "W02_Appear");

                UpdateChillNum(2);
                XUtility.PlayAnimation(animRoll, "Appear", () =>
                {
                    if (animRoll.GetCurrentAnimatorStateInfo(0).IsName("Appear"))
                    {
                        PlayFullWildIdle();
                        _sortingGroup.sortingOrder = 201;
                    }
                }, machineContext);
            }
            // PlayFullWildIdle();
        }

        private void UpdateChillNum(uint num)
        {
            for (int i = 0; i < 3; i++)
            {
                listChill[i].gameObject.SetActive(i < num);
            }
        }


        private uint oldNum = 0;
        protected IEnumerator SetChildSprteNum(uint num, bool isAnim, Action actionEnd)
        {
            bool isHaveAnim = false;
            for (int i = 0; i < 3; i++)
            {
                if (num == 0 || i >= num)
                {
                    listChill[i].gameObject.SetActive(false);
                }
                else if (i < num)
                {
                    if (i >= oldNum)
                    {
                        //新增加的辣椒
                        if (isAnim)
                        {
                            if (!isHaveAnim)
                            {
                                yield return AnimChillEffect();
                            }

                            isHaveAnim = true;

                            listChill[i].gameObject.SetActive(true);
                            PlayeAnim(listChill[i], "In");
                            AudioUtil.Instance.PlayAudioFxIfNotPlaying("B02_Fill-02");
                        }
                        else
                        {
                            listChill[i].gameObject.SetActive(true);
                            PlayeAnim(listChill[i], "Idle");
                        }
                    }
                    else
                    {
                        //之前的辣椒
                        listChill[i].gameObject.SetActive(true);
                        if (listChill[i].gameObject.activeInHierarchy)
                        {
                            PlayeAnim(listChill[i], "Idle");
                        }
                    }
                }
            }

            oldNum = num;
            if (isHaveAnim)
            {
                yield return new WaitForSeconds(0.433f);
            }
            else
            {
                if (isAnim && mNowChill == 3 && !isStartSpin)
                {
                    //被动reset的要等主动的粒子播放完了一起变
                    yield return new WaitForSeconds(1.3f + 0.433f);
                }
            }
            actionEnd?.Invoke();
        }


        private Tweener tweenerFly;
        private GameObject effectCollect;
        private GameObject effectBoomEnd;
        protected IEnumerator AnimChillEffect()
        {
            if (!root.gameObject.activeInHierarchy || isStartSpin)
            {
                yield break;
            }

            int count = roll.rowCount;
            ElementContainer elementContainer = null;
            for (int i = 0; i < count; i++)
            {
                var container = roll.GetVisibleContainer(i);
                var config = container.sequenceElement.config;
                if (config.id == 14 ||
                    config.id == 15 ||
                    config.id == 16)
                {
                    elementContainer = container;
                    break;
                }
            }

            if (elementContainer != null)
            {
                // yield return new WaitForSeconds(0.3f);

                effectCollect = machineContext.assetProvider.InstantiateGameObject("B03_Collect");
                effectCollect.transform.position = elementContainer.transform.position;

                AudioUtil.Instance.PlayAudioFxIfNotPlaying("B02_Add");
                tweenerFly = XUtility.Fly(effectCollect.transform, elementContainer.transform.position,
                    end.position, 0, 0.6f, () =>
                      {
                          GameObject.Destroy(effectCollect);
                      });

                yield return new WaitForSeconds(0.4f);
                effectBoomEnd = machineContext.assetProvider.InstantiateGameObject("B03_CollectBoom");
                effectBoomEnd.transform.position = this.root.position;
                yield return new WaitForSeconds(0.4f);
                GameObject.Destroy(effectBoomEnd);
            }
        }
    }
}