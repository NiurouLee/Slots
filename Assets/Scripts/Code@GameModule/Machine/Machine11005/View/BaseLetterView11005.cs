using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class BaseLetterView11005: TransformHolder
    {
        protected List<BaseLetterViewItem11005> listItem = new List<BaseLetterViewItem11005>();


        [ComponentBinder("LetterGroup")]
        private BoxCollider2D tranLetterGroup;

        [ComponentBinder("LockButton")]
        private BoxCollider2D tranLockGroup;
        
        [ComponentBinder("LockStateNotice")]
        private Transform tranLockStateNotice;
        
        [ComponentBinder("WinIntegralNotice")]
        private Transform tranWinIntegralNotice;

        [ComponentBinder("InformationButton")]
        private Transform  btnInformation;

        [ComponentBinder("IntegralText")]
        private TextMesh txtWinIntegral;

        [ComponentBinder("StepText")]
        private TextMesh txtStep;

        [ComponentBinder("BetText")]
        private TextMesh txtNumGetLetter;

        [ComponentBinder("GetMoreButton")]
        private BoxCollider2D btnGetMore;

        [ComponentBinder("GetMoreButton")]
        private SpriteRenderer spriteRenGetMore;

        [ComponentBinder("LockButtonAnim")]
        private Animator animatorLockButtonAnim;


        [ComponentBinder("BetNoticeGroup")]
        private Animator animatorNoticeGroup;

        [ComponentBinder("LockEffetcs")]
        private Animator animatorLockEffects;

        private BaseLockInfoView baseLockInfoView;

        private BaseInformationView baseInformationView;
        
        
        public BaseLetterView11005(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);

            baseLockInfoView = new BaseLockInfoView(tranLockStateNotice);
            baseLockInfoView.isAutoClose = true;
            baseInformationView = new BaseInformationView(tranWinIntegralNotice);
            baseInformationView.isAutoClose = true;
            
            btnInformation.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnInformationClick);
            
            btnGetMore.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnGetMoreClick);
            
            tranLockGroup.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnLockGroupClick);
            
            tranLetterGroup.gameObject.AddComponent<PointerEventCustomHandler>()
                .BindingPointerClick(OnBtnLetterGroupClick);
        }

        private void OnBtnLetterGroupClick(PointerEventData obj)
        {
            baseLockInfoView.ChangeView();
        }

        private void OnBtnLockGroupClick(PointerEventData obj)
        {
            baseLockInfoView.Close();
            OnBtnGetMoreClick(null);
        }

        private void OnBtnGetMoreClick(PointerEventData obj)
        {

            if (obj != null)
            {
                AudioUtil.Instance.PlayAudioFx("GetMore_Button");
            }

            if (betState.GetMoreLetter())
            {
                UpdateSpinUiViewTotalBet(false);
            }
            ChangeBetLetter(false);
        }

        private void OnBtnInformationClick(PointerEventData obj)
        {
            var mapInfo = extraState.GetMapInfo();
            txtStep.text = (mapInfo.Step + 1).ToString();
            //XUtility.ShowTipAndAutoHide(tranWinIntegralNotice);
          
            baseInformationView.ChangeView();
            txtWinIntegral.SetText(mapInfo.JackpotWin.GetCommaFormat());

        }

        private BetState11005 betState;
        private ExtraState11005 extraState;
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            tranLockGroup.gameObject.SetActive(false);
            tranLockStateNotice.gameObject.SetActive(false);
            tranWinIntegralNotice.gameObject.SetActive(false);
            
            for (int i = 0; i < 10; i++)
            {
                var tranItem = transform.Find($"BGGroup/BottomBGGroup/LetterGroup/{i + 1}");
                BaseLetterViewItem11005 item = new BaseLetterViewItem11005(tranItem, context,i);
                listItem.Add(item);
            }

            betState = context.state.Get<BetState11005>();
            extraState = context.state.Get<ExtraState11005>();
            // GameObject go;
            // go.AddComponent<PointerEventCustomHandler>().BindingPointerClick();
        }

        public async Task RefreshUI()
        {
            List<Task> listTask = new List<Task>();
            
            if (extraState.GetThisOneGetLetterNum() > 0)
            {
                AudioUtil.Instance.PlayAudioFx("WorldFly");
            }
            for (int i = 0; i < listItem.Count; i++)
            {
               listTask.Add(listItem[i].RefreshUI());
            }

            if (listTask.Count > 0)
            {
                await Task.WhenAll(listTask);

            }

        }
        
        public async Task PlayUnlockAnim()
        {
            animatorLockEffects.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("WorldFull");
            await XUtility.PlayAnimationAsync(animatorLockEffects, "LockEffetcs",context);
            animatorLockEffects.gameObject.SetActive(false);
        }

        public void RefreshUINoAnim()
        {
            for (int i = 0; i < listItem.Count; i++)
            {
                listItem[i].GetLetterNumber();
                listItem[i].RefreshLetterNumber();
            }
        }


        public void SetBtnGetMoreEnable(bool isEnable)
        {
            

            btnGetMore.enabled = isEnable;
            tranLetterGroup.enabled = isEnable;
            tranLockGroup.enabled = isEnable;
            spriteRenGetMore.enabled = isEnable;
        }


        private int lastSelectBetLetter = -1;
        public  void ChangeBetLetter(bool isRoomSetUp)
        {
            int unlockLetter = betState.GetUnlockLetterNum(betState.totalBet);
            //int unlockLetterLast = betState.GetUnlockLetterNum(betState.lastTotalBet);
            if (unlockLetter == 0)
            {
                if (!tranLockGroup.gameObject.activeInHierarchy)
                {
                    tranLockGroup.gameObject.SetActive(true);
                    tranLetterGroup.gameObject.SetActive(false);
                    animatorLockButtonAnim.gameObject.SetActive(false);
                    AudioUtil.Instance.PlayAudioFx("LockWorld");
                }

                if (isRoomSetUp)
                {
                    baseLockInfoView.Open();
                }
            }
            else
            {
                animatorLockButtonAnim.gameObject.SetActive(false);
                if (!tranLetterGroup.gameObject.activeInHierarchy)
                {
                    
                    tranLetterGroup.gameObject.SetActive(true);
                    tranLockGroup.gameObject.SetActive(false);
                    
                    
                    var freeSpinState = context.state.Get<FreeSpinState>();
                    if (!freeSpinState.IsInFreeSpin || freeSpinState.IsOver)
                    {
                        animatorLockButtonAnim.gameObject.SetActive(true);
                        animatorLockButtonAnim.Play("LockButtonAnim");
                    }

                    AudioUtil.Instance.PlayAudioFx("UnlockWorld");
                }

                

            }

            if (unlockLetter != lastSelectBetLetter && animatorNoticeGroup.gameObject.activeInHierarchy)
            {
                int maxLetter = Constant11005.listUnlockLetterNum[Constant11005.listUnlockLetterNum.Count - 1];
                if (unlockLetter >= maxLetter && lastSelectBetLetter<maxLetter)
                {
                    animatorNoticeGroup.Play("Close");
                    SetBtnGetMoreEnable(false);
                    lastSelectBetLetter = unlockLetter;
                }
                else if(unlockLetter < maxLetter && lastSelectBetLetter >= maxLetter)
                {
                    animatorNoticeGroup.Play("Open");
                    SetBtnGetMoreEnable(true);
                    lastSelectBetLetter = unlockLetter;
                }
            }

            
            txtNumGetLetter.text = unlockLetter.ToString();
        }
        
        
        public void UpdateSpinUiViewTotalBet(bool lockBet)
        {
            context.view.Get<ControlPanel>().SetTotalBet(betState.totalBet, betState.IsMaxBet(),
                betState.IsMinBet(), betState.IsExtraBet(), lockBet);
        }
    }
    
}