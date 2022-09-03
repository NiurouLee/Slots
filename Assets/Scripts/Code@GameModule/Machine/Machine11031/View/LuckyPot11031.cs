using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;
using SRF;
using DragonU3DSDK.Network.API.ILProtocol;
using DG.Tweening;
using Random = UnityEngine.Random;
using UnityEngine.Rendering;


namespace GameModule
{
    public class LuckyPot11031 : TransformHolder
    {
        [ComponentBinder("LuckyPot/Root/BtnClose")]
        protected Transform btnClose;

        [ComponentBinder("LuckyPot")] protected Transform luckyPot;

        [ComponentBinder("LuckyPot/Root/LuckyPotImg")]
        protected Transform luckyPotTipsIcon;

        [ComponentBinder("LuckyPot/LetterContainer")]
        protected Transform letterContainer;

        [ComponentBinder("LuckyPot/Tips/TipsBG2/TipsText")]
        protected TextMesh _textPrize;

        [ComponentBinder("LuckyPot/Tips/TipsBG1/TipsBg2")]
        protected Transform selectedCircle;

        [ComponentBinder("LuckyPot/Tips")] protected Transform tipsBottom;

        [ComponentBinder("LuckyPot/Root/Num")] protected Transform numContainer;

        [ComponentBinder("LuckyPot/MiniGameSmash/Root/Container/Letter")]
        protected TextMesh breakLetter;

        [ComponentBinder("LuckyPot/MiniGameSmash/Root/Multiple/MulBg/Num")]
        protected TextMesh breakMultiple;

        [ComponentBinder("LuckyPot/MiniGameSmash/Root/Multiple")]
        protected Transform smashMultiple;

        [ComponentBinder("LuckyPot/MiniGameSmash")]
        protected Transform breakSmash;

        [ComponentBinder("LuckyPot/MiniGameOffer")]
        protected Transform offerView;

        [ComponentBinder("LuckyPot/MiniGameOffer/MiniGameOfferimg")]
        protected Transform offerViewIcon;

        [ComponentBinder("LuckyPot/MiniGameOffer/BTN/GrabBtn")]
        protected Transform btnGrab;

        [ComponentBinder("LuckyPot/MiniGameOffer/BTN/PassBtn")]
        protected Transform btnPass;

        [ComponentBinder("LuckyPot/Round")] protected Transform roundView;

        [ComponentBinder("LuckyPot/Round/RoundText")]
        protected TextMesh roundText;

        [ComponentBinder("LuckyPot/Round/RoundText2")]
        protected TextMesh pickNumText;

        [ComponentBinder("LuckyPot/MiniGameOffer/Offer/NumText")]
        protected TextMesh finalMul;

        [ComponentBinder("LuckyPot/MiniGameOffer/Offer/NumText2")]
        protected TextMesh finalPrize;

        [ComponentBinder("LuckyPot/MiniGameSmash/Root/BreakLetterNode")]
        protected Transform breakLetterNode;

        [ComponentBinder("LuckyPot/MiniGameFOffer")]
        protected Transform finalView;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Container/KeepBtn")]
        protected Transform btnFinal1;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Container2/KeepBtn")]
        protected Transform btnFinal2;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Container/Letter")]
        protected TextMesh letterFinal1;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Container2/Letter")]
        protected TextMesh letterFinal2;

        [ComponentBinder("LuckyPot/MiniGameOffer/PrlorOffer/NumText")]
        protected TextMesh bottomMul;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Multiple")]
        protected Transform multipleR;

        [ComponentBinder("LuckyPot/MiniGameFOffer/Root/Multiple1")]
        protected Transform multipleL;

        [ComponentBinder("LuckyPot/MiniGameOffer/PrlorOffer")]
        protected Transform bottomOfferContainer;

        [ComponentBinder("HolderPos")] protected Transform holderPos;

        [ComponentBinder("BtnCloseInScale")] protected Transform btnCloseInScale;


        private List<Transform> letterContainerList;

        private List<Transform> numContainerList;

        private ExtraState11031 _extraState11031;

        public bool isResponsed = false;

        private List<bool> isClickedList;

        private List<Animator> _animators;

        private List<Animator> _numAnimators;

        private Animator letterContainerAnimator;

        private Animator finalViewAnimator;

        private Animator smashMultipleAnimator;

        private List<Transform> offerViewIconList;

        private List<int> randomIconList;

        private List<Transform> luckyPotTips;

        private List<int> numList = new List<int> {1, 2, 3, 4, 5};

        private bool _buttonResponseEnabled = true;

        private bool isClick = false;

        private Vector3 beforePos;

        private List<Vector3> bowelInitPositions;
        
        private bool isClosing1 = false;

        public LuckyPot11031(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            letterContainerAnimator = luckyPot.GetComponent<Animator>();
            letterContainerAnimator.keepAnimatorControllerStateOnDisable = true;
          
            finalViewAnimator = finalView.GetComponent<Animator>();
           // finalViewAnimator.keepAnimatorControllerStateOnDisable = true;
            smashMultipleAnimator = smashMultiple.GetComponent<Animator>();
            // smashMultipleAnimator.keepAnimatorControllerStateOnDisable = true;
            randomIconList = new List<int>() {1, 2, 3, 4, 5};
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void EnableButtonResponse(bool enable)
        {
            _buttonResponseEnabled = enable;
        }

        public bool GetActiveSelf()
        {
            return transform.gameObject.activeSelf;
        }


        public Vector3 GetHoldenPos()
        {
            return holderPos.position;
            return new Vector3(1.11f, -1.966f, 0f);
        }

        public void ShowLuckyPot(bool enableBtnClose = true, bool fly = false)
        {
            btnClose.gameObject.SetActive(enableBtnClose);

            if (transform.localScale != Vector3.one)
            {
                btnClose.transform.localPosition = btnCloseInScale.localPosition;
            }

            SetPrizeText();
            UpdateLetterContainer(false, fly);
            ShowStartPotImg();
            offerView.gameObject.SetActive(false);
            finalView.gameObject.SetActive(false);
            letterContainer.gameObject.SetActive(true);
            transform.gameObject.SetActive(true);
            RecoverMultiple();
        }

        public void RecoverStartPos()
        {
            var index = _extraState11031.GetHoldenLetter();
            letterContainerList[(int) index].position = beforePos;
            for (var i = 0; i < isClickedList.Count; i++)
            {
                isClickedList[i] = true;
            }
        }

        public async void PlayIntro()
        {
            AudioUtil.Instance.PlayAudioFx("MiniGame_Board_Open");
            await XUtility.PlayAnimationAsync(letterContainerAnimator, "Intro", context);
        }

        public void ShowStartPotImg()
        {
            luckyPotTipsIcon.gameObject.SetActive(true);
            luckyPotTips[0].gameObject.SetActive(true);
            luckyPotTips[1].gameObject.SetActive(true);
            luckyPotTips[4].gameObject.SetActive(false);
            luckyPotTips[5].gameObject.SetActive(false);
        }

        public void ShowGoogLuckImg()
        {
            luckyPotTipsIcon.gameObject.SetActive(true);
            luckyPotTips[0].gameObject.SetActive(false);
            luckyPotTips[1].gameObject.SetActive(false);
            luckyPotTips[4].gameObject.SetActive(false);
            luckyPotTips[5].gameObject.SetActive(true);
        }

        public void ShowRemaingImg()
        {
            luckyPotTipsIcon.gameObject.SetActive(true);
            luckyPotTips[0].gameObject.SetActive(false);
            luckyPotTips[1].gameObject.SetActive(false);
            luckyPotTips[4].gameObject.SetActive(true);
            luckyPotTips[5].gameObject.SetActive(false);
            var remainNum = _extraState11031.GetNumOfLetterToSelect() - _extraState11031.selectedLetterList.Count;
            luckyPotTips[4].Find("LuckyPotText").GetComponent<TextMesh>().text = remainNum.ToString();
        }

        public void ShowPotImg(bool enable)
        {
            luckyPotTipsIcon.gameObject.SetActive(enable);
        }


        public async void RecoverSelect()
        {
            isResponsed = false;
            btnClose.gameObject.SetActive(false);
            transform.gameObject.SetActive(true);
            RecoverMultiple();
            UpdateLetterContainer();
            offerView.gameObject.SetActive(false);
            finalView.gameObject.SetActive(false);
            letterContainer.gameObject.SetActive(true);
            ShowPotImg(false);
            XUtility.PlayAnimation(letterContainerAnimator, "Idle", null, context);
            await ShowRoundView();
            isResponsed = true;
        }

        public async void RecoverBreakLetter()
        {
            isResponsed = false;
            btnClose.gameObject.SetActive(false);
            transform.gameObject.SetActive(true);
            RecoverMultiple();
            ShowPotImg(false);
            offerView.gameObject.SetActive(false);
            finalView.gameObject.SetActive(false);
            letterContainer.gameObject.SetActive(true);
            XUtility.PlayAnimation(letterContainerAnimator, "ScaleDownIdle", null, context);
            UpdateLetterContainer();
            await BreakLetter(false);
        }

        public Vector3 GetInternalPos()
        {
            int mapLevel = (int) _extraState11031.GetMapLevel();
            if (mapLevel > 0)
            {
                return letterContainerList[mapLevel - 1].position;
            }
            else
            {
                return letterContainerList[0].position;
            }
        }

        private async void CloseClick(PointerEventData pointerEventData)
        {
            if (!_buttonResponseEnabled || isClick)
                return;
            isClick = true;
            context.view.Get<BowlView11031>().ShowBowlInBase();
            await XUtility.PlayAnimationAsync(letterContainerAnimator, "Outro", context);
            transform.gameObject.SetActive(false);
            isClick = false;
            context.view.Get<ControlPanel>().ShowSpinButton(true);
        }

        public async Task Outro()
        {
            await XUtility.PlayAnimationAsync(letterContainerAnimator, "Outro", context);
            transform.gameObject.SetActive(false);
        }

        private async Task ShowLuckPotFinishPopup()
        {
            AudioUtil.Instance.StopMusic();
            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_End");
            TaskCompletionSource<bool> taskStartPopup = new TaskCompletionSource<bool>();
            var chips = _extraState11031.GetMapTotalWin() - _extraState11031.GetMapPreWin();
            var finalMultiple = _extraState11031.GetMapFinalMultiplier();
            var stratPrize = _extraState11031.GetMapStartPrize().GetAbbreviationFormat(1);
            var finalText = finalMultiple + "X" + stratPrize + "=" + chips.GetAbbreviationFormat(1);
            var view = PopUpManager.Instance.ShowPopUp<LuckyPotFinishPopup11031>("UIMiniGame_End");
            view.SetWinNum(chips);
            view.SetFinalText(finalText);
            view.SetPopUpCloseAction(async () => { taskStartPopup.SetResult(true); });
            await taskStartPopup.Task;
        }

        public async void ToSettle()
        {
            await ShowLuckPotFinishPopup();
            btnClose.gameObject.SetActive(false);
            transform.gameObject.SetActive(false);
            await _extraState11031.SettleBonusProgress();
            var bonusProxy11031 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11031;
            await bonusProxy11031.BonusFinish();
        }

        private  bool grabButtonInteractable = true;
        private async void GrabClick(PointerEventData pointerEventData)
        {
            if (!grabButtonInteractable)
            {
               return;
            }
            
            grabButtonInteractable = false;
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = "[" + "2121" + "]";
            await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
            await ShowLuckPotFinishPopup();
            transform.gameObject.SetActive(false);
            await _extraState11031.SettleBonusProgress();
            var bonusProxy11031 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11031;
            await bonusProxy11031.BonusFinish();
    
            grabButtonInteractable = true;
        }

        

        private async void Final1Click(PointerEventData pointerEventData)
        {
            if (!isClosing1)
            {
                isClosing1 = true;
                CBonusProcess cBonusProcess = new CBonusProcess();
                cBonusProcess.Json = "[" + "]";
                await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
                int currentLetterIndex = (int) _extraState11031.GetCurrentLetter();
                uint currentLetterMultiplier = _extraState11031.GetMapFinalMultiplier();
                multipleR.gameObject.SetActive(true);
                XUtility.PlayAnimation(finalViewAnimator, "Smash_KnockFinalR", null, context);
                XUtility.PlayAnimation(multipleR.gameObject.GetComponent<Animator>(), "Multi_Final", null, context);
                multipleR.Find("MulBg").Find("Num").gameObject.GetComponent<TextMesh>()
                    .SetText(currentLetterMultiplier + "X");
                await context.WaitSeconds(0.3f);
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Smash");
                await context.WaitSeconds(1.4f - 0.3f);
                int mulIndex = GetMultiplierIndex(currentLetterMultiplier);
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Cheers");
                ShowFinalDark(mulIndex);
                // XUtility.PlayAnimation(_numAnimators[mulIndex], "MultiDark");
                await context.WaitSeconds(4.5f - 1.4f);
                await ShowLuckPotFinishPopup();
                transform.gameObject.SetActive(false);
                await _extraState11031.SettleBonusProgress();
                var bonusProxy11031 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11031;
                await bonusProxy11031.BonusFinish();
            }
        }

        public void ShowFinalDark(int mulIndex)
        {
            for (var i = 0; i < _numAnimators.Count; i++)
            {
                if (i != mulIndex && !_numAnimators[i].GetCurrentAnimatorStateInfo(0).IsName("MultiDarkIdle"))
                {
                     XUtility.PlayAnimation(_numAnimators[i], "MultiDark");
                }
            }
        }

        private async void Final2Click(PointerEventData pointerEventData)
        {
            if (!isClosing1)
            {
                isClosing1 = true;
                CBonusProcess cBonusProcess = new CBonusProcess();
                cBonusProcess.Json = "[1]";
                await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
                int currentLetterIndex = (int) _extraState11031.GetCurrentLetter();
                uint currentLetterMultiplier = _extraState11031.GetMapFinalMultiplier();
                multipleL.gameObject.SetActive(true);
                XUtility.PlayAnimation(finalViewAnimator, "Smash_KnockFinalL");
                XUtility.PlayAnimation(multipleL.gameObject.GetComponent<Animator>(), "Multi_FinalL");
                multipleL.Find("MulBg").Find("Num").gameObject.GetComponent<TextMesh>()
                    .SetText(currentLetterMultiplier + "X");
                await context.WaitSeconds(0.3f);
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Smash");
                await context.WaitSeconds(1.4f - 0.3f);
                int mulIndex = GetMultiplierIndex(currentLetterMultiplier);
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Cheers");
                ShowFinalDark(mulIndex);
                // XUtility.PlayAnimation(_numAnimators[mulIndex], "MultiDark");
                await context.WaitSeconds(4.0f - 1.4f);
                await ShowLuckPotFinishPopup();
                transform.gameObject.SetActive(false);
                await _extraState11031.SettleBonusProgress();
                var bonusProxy11031 = context.GetLogicStepProxy(LogicStepType.STEP_BONUS) as BonusProxy11031;
                await bonusProxy11031.BonusFinish();
            }
        }

        private bool _passButtonInteractable = true;
        private async void PassClick(PointerEventData pointerEventData)
        {
            if (!_passButtonInteractable)
            {
                return;
            }
            _passButtonInteractable = false;
            
            CBonusProcess cBonusProcess = new CBonusProcess();
            cBonusProcess.Json = "[" + "]";
            await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
            offerView.gameObject.SetActive(false);
            var round = _extraState11031.GetMapRound();
            if (round == 6)
            {
                var letters = _extraState11031.GetMapLetters();
                var letterIndex1 = _extraState11031.GetHoldenLetter();
                uint letterIndex2 = 0;
                for (var i = 0; i < letters.Count; i++)
                {
                    if (letters[i].Chosen == false && letters[i].Removed == false)
                    {
                        letterIndex2 = letters[i].Index;
                    }
                }

                letterFinal1.SetText(Constant11031.ListLetter[(int) letterIndex1]);
                letterFinal2.SetText(Constant11031.ListLetter[(int) letterIndex2]);
                finalView.gameObject.SetActive(true);
                
                multipleR.gameObject.SetActive(false);
                multipleL.gameObject.SetActive(false);
                
                ShowPotImg(false);
                await ShowLuckPotLastChancePopUp();
                isClosing1 = false;
            }
            else
            {
                letterContainer.gameObject.SetActive(true);
                XUtility.PlayAnimation(letterContainerAnimator, "ScaleUp", null, context);
                UpdateLetterContainer();
                var holdenIndex = _extraState11031.GetHoldenLetter();
                letterContainerList[(int) holdenIndex].gameObject.SetActive(false);
                await context.WaitSeconds(0.73f);
                letterContainerList[(int) holdenIndex].position = GetHoldenPos();
                letterContainerList[(int) holdenIndex].gameObject.SetActive(true);
                XUtility.PlayAnimation(_animators[(int) holdenIndex], "Letter_YourBowlIdle", null, context);
                await context.WaitSeconds(1.0f - 0.73f);
                await ShowRoundView();
                isResponsed = true;
            }
            
            _passButtonInteractable = true;
        }


        private async Task ShowLuckPotLastChancePopUp()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_RoundStart");
            TaskCompletionSource<bool> taskStartPopup = new TaskCompletionSource<bool>();
            var view = PopUpManager.Instance.ShowPopUp<LuckyPotLastChangePopup11031>("UIroudLastchance");
            view.SetPopUpCloseAction(async () => { taskStartPopup.SetResult(true); });
            await taskStartPopup.Task;
        }

        public async Task ShowLuckPotPickYourPot()
        {
            ShowGoogLuckImg();
            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_RoundStart");
            TaskCompletionSource<bool> taskStartPopup = new TaskCompletionSource<bool>();
            var view = PopUpManager.Instance.ShowPopUp<LuckyPotLastChangePopup11031>("UIroudPlckYourPot");
            view.SetPopUpCloseAction(async () => { taskStartPopup.SetResult(true); });
            await taskStartPopup.Task;
            isResponsed = true;
        }

        public void SetBeforePos()
        {
            var holdIndex = _extraState11031.GetHoldenLetter();
            var holdPos = letterContainerList[(int) holdIndex].position;
            beforePos = holdPos;
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            var pointerEventCustomHandler = btnClose.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(CloseClick);

            var pointerEventGrabHandler = btnGrab.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventGrabHandler.BindingPointerClick(GrabClick);

            var pointerEventPassHandler = btnPass.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventPassHandler.BindingPointerClick(PassClick);

            var pointerEventFinal1Handler = btnFinal1.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventFinal1Handler.BindingPointerClick(Final1Click);

            var pointerEventFinal2Handler = btnFinal2.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventFinal2Handler.BindingPointerClick(Final2Click);

            _extraState11031 = context.state.Get<ExtraState11031>();

            isClickedList = new List<bool>();

            _animators = new List<Animator>();

            _numAnimators = new List<Animator>();

            bowelInitPositions = new List<Vector3>();

            var childCount = 27;
            letterContainerList = new List<Transform>(childCount);
            for (var i = 1; i < childCount; i++)
            {
                var letter = letterContainer.Find("Root").GetChild(i);
                letterContainerList.Add(letter);
                bowelInitPositions.Add(letter.localPosition);
            }

            for (var i = 0; i < letterContainerList.Count; i++)
            {
                var j = i;
                var pointerEventNightHandler =
                    letterContainerList[i].gameObject.GetComponentOrAdd<PointerEventCustomHandler>();
                pointerEventNightHandler.BindingPointerClick((e) => iconClick(j));
                isClickedList.Add(true);
                _animators.Add(letterContainerList[i].gameObject.GetComponent<Animator>());
                // _animators[i].keepAnimatorControllerStateOnDisable = true;
            }

            var numChildCount = 26;
            numContainerList = new List<Transform>(numChildCount);

            for (int n = 0; n < numChildCount; n++)
            {
                var num = numContainer.GetChild(n);
                numContainerList.Add(num);
            }

            for (int m = 0; m < numContainerList.Count; m++)
            {
                _numAnimators.Add(numContainerList[m].gameObject.GetComponent<Animator>());
                // _numAnimators[m].keepAnimatorControllerStateOnDisable = true;
            }

            var offerIconNum = 5;
            offerViewIconList = new List<Transform>(offerIconNum);
            for (var j = 0; j < offerIconNum; j++)
            {
                var icon = offerViewIcon.GetChild(j);
                offerViewIconList.Add(icon);
            }

            var luckyPotIconNum = 6;
            luckyPotTips = new List<Transform>(luckyPotIconNum);
            for (var j = 0; j < luckyPotIconNum; j++)
            {
                var tip = luckyPotTipsIcon.GetChild(j);
                luckyPotTips.Add(tip);
            }
        }

        public void UpdateLetterContainer(bool animation = false, bool fly = false)
        {
            int mapLevel = (int) _extraState11031.GetMapLevel();
            if (fly == true)
            {
                if (mapLevel > 0)
                {
                    mapLevel = mapLevel - 1;
                }
            }

            var holdenIndex = _extraState11031.GetHoldenLetter();
            var isStarted = _extraState11031.GetMapIsStarted();
            var round = _extraState11031.GetMapRound();
            var letterDataList = _extraState11031.GetMapLetters();
            for (var i = 0; i < letterContainerList.Count; i++)
            {
                letterContainerList[i].localPosition = bowelInitPositions[i];

                if (mapLevel == 0)
                {
                    letterContainerList[i].gameObject.SetActive(false);
                }
                else
                {
                    if (i > mapLevel - 1)
                    {
                        letterContainerList[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        if ((letterDataList[i].Removed || letterDataList[i].Chosen) && isStarted)
                        {
                            if (i == holdenIndex)
                            {
                                letterContainerList[i].position = GetHoldenPos();
                                letterContainerList[i].gameObject.SetActive(true);
                                XUtility.PlayAnimation(_animators[i], "Letter_YourBowlIdle", null, context);
                            }
                            else
                            {
                                if (letterDataList[i].Chosen && letterDataList[i].Removed == false)
                                {
                                    letterContainerList[i].gameObject.SetActive(true);
                                    XUtility.PlayAnimation(_animators[i], "Letter_SelectIdle", null, context);
                                }
                                else
                                {
                                    letterContainerList[i].gameObject.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            letterContainerList[i].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        public void RecoverMultiple()
        {
            var isStart = _extraState11031.GetMapIsStarted();
            var multipleData = _extraState11031.GetMapMultipliers();
            for (var i = 0; i < multipleData.Count; i++)
            {
                if (multipleData[i].Removed && isStart == true)
                {
                    XUtility.PlayAnimation(_numAnimators[i], "MultiDarkIdle", null, context);
                }
                else
                {
                    XUtility.PlayAnimation(_numAnimators[i], "MultiIdle", null, context);
                }
            }
        }


        //点击字母
        private async void iconClick(int i)
        {
            if (isClickedList[i] == false || isResponsed == false)
            {
                return;
            }

            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Click");
            isClickedList[i] = false;
            if (_extraState11031.GetMapRound() == 0)
            {
                isResponsed = false;
                XUtility.PlayAnimation(_animators[i], "Letter_YourBowl", null, context);
                await context.WaitSeconds(0.5f);
                CBonusProcess cBonusProcess = new CBonusProcess();
                cBonusProcess.Json = "[" + i.ToString() + "]"; // $"[{i.ToString()}]");
                await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
                //如果是最开始的字母
                if (i == _extraState11031.GetHoldenLetter())
                {
                    //飞行到提示区
                    SortingGroup sortingGroup = letterContainerList[i].transform.GetComponent<SortingGroup>();
                    sortingGroup.sortingOrder = 10;
                    beforePos = letterContainerList[i].position;
                    await XUtility.FlyAsync(letterContainerList[i], letterContainerList[i].position,
                        GetHoldenPos(), 0, 0.5f);
                    sortingGroup.sortingOrder = 3;
                    await context.WaitSeconds(0.2f);
                    //显示当前是第几轮
                    await ShowRoundView();
                    isResponsed = true;
                }
            }

            if (_extraState11031.GetMapRound() >= 1 && _extraState11031.GetMapIsSelect() == true &&
                (i != _extraState11031.GetHoldenLetter()))
            {
                if (_extraState11031.selectedLetterList.Count == _extraState11031.GetNumOfLetterToSelect() - 1)
                {
                    isResponsed = false;
                }

                int j = i;
                _extraState11031.SetSelectedLetterList(j);
               
                var selectCount = _extraState11031.selectedLetterList.Count;
               
                ShowRemaingImg();
               
                await XUtility.PlayAnimationAsync(_animators[i], "Letter_Select", context);
                
                if (selectCount == _extraState11031.GetNumOfLetterToSelect())
                {
                    string finalSelect = "";
                    for (var a = 0; a < _extraState11031.selectedLetterList.Count; a++)
                    {
                        if (a < _extraState11031.selectedLetterList.Count - 1)
                        {
                            finalSelect = finalSelect + _extraState11031.selectedLetterList[a] + ",";
                        }
                        else
                        {
                            finalSelect = finalSelect + _extraState11031.selectedLetterList[a];
                        }
                    }
                    
                    CBonusProcess cBonusProcess = new CBonusProcess();
                    cBonusProcess.Json = "[" + finalSelect + "]";
                    
                    await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);
                    _extraState11031.ClearSelectedLetterList();
                    await BreakLetter();
                }
            }
        }

        //显示接下来是第几轮，可以选择多少个字母
        private async Task ShowRoundView()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_RoundStart");
            int roundNum = (int) _extraState11031.GetMapRound();

            roundText.SetText(roundNum + "");
            int selectNum = (int) _extraState11031.GetNumOfLetterToSelect();
            pickNumText.SetText(selectNum + "");
            roundView.gameObject.SetActive(true);
            await context.WaitSeconds(2.0f);
            roundView.gameObject.SetActive(false);
            ShowRemaingImg();
        }


        private bool _isBreakLetter = false;

        private async Task BreakLetter(bool playAnimation = true)
        {
            //目前不知道什么情况下，这个函数会被连续两次调用，为了避免这个情况先加一个保护和一个LOG，先避免问题出现
            if (_isBreakLetter)
            {
                XDebug.LogError("BreakLetterLogicError:[BreakLetter:Called Multiple Times]========");
                return;
            }

            _isBreakLetter = true;

            isResponsed = false;

            ShowPotImg(false);
            var holdenIndex = _extraState11031.GetHoldenLetter();
            letterContainerList[(int) holdenIndex].gameObject.SetActive(false);
            tipsBottom.gameObject.SetActive(false);
            breakSmash.gameObject.SetActive(true);
            if (playAnimation)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Board_Resize");
                await XUtility.PlayAnimationAsync(letterContainerAnimator, "ScaleDown", context);
            }

            var num = (int) _extraState11031.GetNumOfLetterToSelect();
            
            XDebug.LogOnExceptionHandler("GetNumOfLetterToSelect:" + num);
            for (var i = 0; i < num; i++)
            {
                CBonusProcess cBonusProcess = new CBonusProcess();
                cBonusProcess.Json = "[" + "]";
                await context.state.Get<ExtraState11031>().SendBonusProcess(cBonusProcess);

                int currentLetterIndex = (int) _extraState11031.GetCurrentLetter();
                uint currentLetterMultiplier = _extraState11031.GetCurrentMultiplier();
                int fakeLetterMultiplier = (int) _extraState11031.GetFakeMultiplier();
                
                XDebug.LogOnExceptionHandler("BreakIndex:" + i);
                XDebug.LogOnExceptionHandler("currentLetterIndex:" + currentLetterIndex);
                XDebug.LogOnExceptionHandler("currentLetterMultiplier:" + currentLetterMultiplier);
                XDebug.LogOnExceptionHandler("fakeLetterMultiplier:" + fakeLetterMultiplier);
                
                var startPos = letterContainerList[currentLetterIndex].position;
                var endPos = breakLetterNode.position;
                //字母飞到相应的区域
                SortingGroup sortingGroup =
                    letterContainerList[currentLetterIndex].transform.GetComponent<SortingGroup>();
                sortingGroup.sortingOrder = 10;
                AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Pot_Fly");
                XUtility.PlayAnimation(_animators[currentLetterIndex], "Letter_Fly", null, context);
                await XUtility.FlyAsync(letterContainerList[currentLetterIndex], startPos, endPos, 0, 0.5f,
                    context: context);
                sortingGroup.sortingOrder = 3;
                //开始砸罐子
                if (fakeLetterMultiplier > 0)
                {
                    XUtility.PlayAnimation(breakSmash.gameObject.GetComponent<Animator>(), "Smash_Tease", null,
                        context);
                    smashMultiple.gameObject.SetActive(true);
                    breakMultiple.SetText(fakeLetterMultiplier + "X");
                    XUtility.PlayAnimation(smashMultipleAnimator, "Multi_Tease", null, context);
                    //砸完罐子后
                    await context.WaitSeconds(0.3f);
                    AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Smash");
                    await context.WaitSeconds(0.4f - 0.3f);
                    letterContainerList[currentLetterIndex].gameObject.SetActive(false);
                    letterContainerList[currentLetterIndex].DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0f);
                    letterContainerList[currentLetterIndex].position = startPos;
                    await context.WaitSeconds(2.2f - 0.4f);
                    breakMultiple.SetText(currentLetterMultiplier + "X");
                    int mulIndex = GetMultiplierIndex(currentLetterMultiplier);
                    if (mulIndex < 13)
                    {
                        AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Cheers");
                    }
                    else
                    {
                        AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Pity");
                    }

                    XUtility.PlayAnimation(_numAnimators[mulIndex], "MultiDark", null, context);
                    await context.WaitSeconds(4.53f - 2.2f);
                    smashMultiple.gameObject.SetActive(false);
                }
                else
                {
                    XUtility.PlayAnimation(breakSmash.gameObject.GetComponent<Animator>(), "Smash_Knock", null,
                        context);
                    smashMultiple.gameObject.SetActive(true);
                    breakMultiple.SetText(currentLetterMultiplier + "X");
                    XUtility.PlayAnimation(smashMultipleAnimator, "Multi_Knock", null, context);
                    await context.WaitSeconds(0.3f);
                    AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Smash");
                    //砸完罐子后
                    await context.WaitSeconds(0.4f - 0.3f);
                    letterContainerList[currentLetterIndex].gameObject.SetActive(false);
                    letterContainerList[currentLetterIndex].DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0f);
                    letterContainerList[currentLetterIndex].position = startPos;
                    int mulIndex = GetMultiplierIndex(currentLetterMultiplier);
                    if (mulIndex < 13)
                    {
                        AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Cheers");
                    }
                    else
                    {
                        AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Pity");
                    }

                    await context.WaitSeconds(1.4f - 0.4f);
                    XUtility.PlayAnimation(_numAnimators[mulIndex], "MultiDark", null, context);
                    await context.WaitSeconds(2.86f - 1.4f);
                    smashMultiple.gameObject.SetActive(false);
                }

                if (_extraState11031.GetMapIsOffer())
                {
                    await OfferView();
                    break;
                }
            }

            _isBreakLetter = false;
        }

        public int GetMultiplierIndex(uint CurrentLetterMultiplier)
        {
            var mapMultipliers = _extraState11031.GetMapMultipliers();
            int index = 0;
            for (var i = 0; i < mapMultipliers.Count; i++)
            {
                if (mapMultipliers[i].Multiplier_ == CurrentLetterMultiplier)
                {
                    index = i;
                    return index;
                }
            }

            return 0;
        }

        public async void RecoverShowOffer()
        {
            transform.gameObject.SetActive(true);
            RecoverMultiple();
            ShowPotImg(false);
            btnClose.gameObject.SetActive(false);
            XUtility.PlayAnimation(letterContainerAnimator, "ScaleDownIdle", null, context);
            letterContainer.gameObject.SetActive(false);
            tipsBottom.gameObject.SetActive(false);
            await OfferView();
        }

        public async void RecoverFinalView()
        {
            transform.gameObject.SetActive(true);
            RecoverMultiple();
            ShowPotImg(false);
            btnClose.gameObject.SetActive(false);
            XUtility.PlayAnimation(letterContainerAnimator, "ScaleDownIdle", null, context);
            letterContainer.gameObject.SetActive(false);
            tipsBottom.gameObject.SetActive(false);
            offerView.gameObject.SetActive(false);
            var round = _extraState11031.GetMapRound();
            if (round == 6)
            {
                var letters = _extraState11031.GetMapLetters();
                var letterIndex1 = _extraState11031.GetHoldenLetter();
                uint letterIndex2 = 0;
                for (var i = 0; i < letters.Count; i++)
                {
                    if (letters[i].Chosen == false && letters[i].Removed == false)
                    {
                        letterIndex2 = letters[i].Index;
                    }
                }

                letterFinal1.SetText(Constant11031.ListLetter[(int) letterIndex1]);
                letterFinal2.SetText(Constant11031.ListLetter[(int) letterIndex2]);
                finalView.gameObject.SetActive(true);
                await ShowLuckPotLastChancePopUp();
            }
        }

        public async Task OfferView()
        {
            letterContainer.gameObject.SetActive(false);
            breakSmash.gameObject.SetActive(false);
            var offers = _extraState11031.GetAllOffers();
            var finalMultiplier = offers[offers.Count - 1];
            if (offers.Count > 1)
            {
                bottomOfferContainer.gameObject.SetActive(true);
                var bottomMultiplierString = "";
                for (var j = 0; j < offers.Count - 1; j++)
                {
                    bottomMultiplierString = bottomMultiplierString + "   " + offers[j] + "X";
                }

                bottomMul.SetText(bottomMultiplierString);
            }
            else
            {
                bottomOfferContainer.gameObject.SetActive(false);
            }

            finalMul.SetText(finalMultiplier + "x");
            var startPrize = _extraState11031.GetMapStartPrize();
            var finalChips = (startPrize * finalMultiplier).GetAbbreviationFormat(1);
            string finalPrizeText = finalMultiplier + "X" + startPrize.GetAbbreviationFormat(1) + "=" + finalChips;
            finalPrize.SetText(finalPrizeText);
            offerView.gameObject.SetActive(true);

            if (randomIconList.Count <= 0)
            {
                randomIconList = new List<int>() {1, 2, 3, 4, 5};
            }

            int ranNum = Random.Range(0, randomIconList.Count);
            int ranV = randomIconList[ranNum];
            randomIconList.RemoveAt(ranNum);

            for (var i = 0; i < offerViewIconList.Count; i++)
            {
                if (i == ranV - 1)
                {
                    offerViewIconList[i].gameObject.SetActive(true);
                }
                else
                {
                    offerViewIconList[i].gameObject.SetActive(false);
                }
            }

            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Offer_Alarm");
            XUtility.PlayAnimation(offerView.gameObject.GetComponent<Animator>(), "Intro", null, context);
            await context.WaitSeconds(2.0f);
            AudioUtil.Instance.PlayAudioFxOneShot("MiniGame_Offer_Start");
            await context.WaitSeconds(3.77f - 2.0f);
        }

        public void SetPrizeText()
        {
            var chips = _extraState11031.GetMapStartPrize();
            _textPrize.gameObject.GetComponent<TextMesh>().SetText(chips.GetAbbreviationFormat(1));
        }

        public void PLayLuckyPotMusic()
        {
            AudioUtil.Instance.PlayMusic("Bg_MiniGame_11031", true);
        }
    }
}