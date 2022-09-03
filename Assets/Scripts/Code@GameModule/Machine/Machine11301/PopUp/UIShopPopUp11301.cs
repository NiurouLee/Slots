using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DragonU3DSDK.Network.API.ILProtocol.CatInBootsGameResultExtraInfo.Types;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace GameModule{
    public class UIShopPopUp11301 : MachinePopUp
    {
        [ComponentBinder("FullMask")]
        public Button FullMask;
        [ComponentBinder("PageGroup")]
        public Transform PageGroup;
        [ComponentBinder("LeftGroup")]
        public Transform LeftGroup;
        [ComponentBinder("Root/MainGroup/ShopGroup/Scroll View/Viewport/Content")]
        public Transform Content;
        [ComponentBinder("BackButton")]
        public Button BackButton;
        [ComponentBinder("NextButton")]
        public Button NextButton;
        [ComponentBinder("CloseButton")]
        public Button CloseButton;
        [ComponentBinder("Root/MainGroup/AverageBetGroup/IntegralText")]
        public Text AvgText;
        [ComponentBinder("Root/MainGroup/AverageBetGroup/HelpButton")]
        public Button AvgHelpButton;
        [ComponentBinder("Root/MainGroup/AverageBetGroup/Tips")]
        public Transform AvgBetTips;
        [ComponentBinder("Root/MainGroup/CurrencyGroup/IntegralText")]
        public Text CurrencyText;
        [ComponentBinder("Root/MainGroup/CurrencyGroup/HelpButton")]
        public Button CurrencyHelpButton;
        [ComponentBinder("Root/MainGroup/CurrencyGroup/Tips")]
        public Transform TokenTips;

        public ExtraState11301 extraState;
        //当前解锁的页面
        private int UnlockPageIndex = 0;
        private bool isClicking=false;
        public bool boxIsMoveing=false;
        public bool IsShowBubbles=false;
        private GameObject TipsObject;
        private bool tempIsAllUnlock = false;
        public UIShopPopUp11301(Transform transform) : base(transform)
        {
            // contentDesignSize = new Vector2(1197, 768);
            // SetIsNeedAdaptive(true);
        }
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            if(CloseButton){
                CloseButton.onClick.AddListener(()=>{
                    if(IsShowBubbles) return;
                    AudioUtil.Instance.PlayAudioFx("SpecialClick1");
                    Close();
                });
            }
            BackButton.onClick.AddListener(()=>{
                ShowNextPage(-1);
            });
            NextButton.onClick.AddListener(()=>{
                ShowNextPage(1);
            });
            AvgHelpButton.onClick.AddListener(()=>{
                ShowTipsToAutoHide(AvgBetTips);
            });
            CurrencyHelpButton.onClick.AddListener(()=>{
                ShowTipsToAutoHide(TokenTips);
            });
            FullMask.onClick.AddListener(()=>{
                FullMask.gameObject.SetActive(false);
                ShowTipsToAutoHide(TokenTips);
            });
            extraState = context.state.Get<ExtraState11301>();
            
            SetTokenEventHandler();
            SetAvgbetEventHandler();
            context.view.Get<ControlPanel>().ShowSpinButton(false);
            
        }

        public void SetTokenEventHandler(){
            var CurrencyHelp_EventHandler = CurrencyHelpButton.transform.GetComponent<SelectEventCustomHandler>();
            if (CurrencyHelp_EventHandler == null)
                CurrencyHelp_EventHandler = CurrencyHelpButton.transform.gameObject.AddComponent<SelectEventCustomHandler>();
            CurrencyHelp_EventHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
            {
                XDebug.Log("按钮点击-CurrencyHelpButton");
                var anim = TokenTips.transform.GetComponent<Animator>();
                var clips = anim.GetCurrentAnimatorClipInfo(0);
                if (clips.Length > 0)
                {
                    if (clips[0].clip.name == "Tips_Close")
                    {
                        return;
                    }
                    if (clips[0].clip.name == "Tips_Open" || clips[0].clip.name == "Tips_Idle")
                    {
                        anim.Update(0);
                        anim.Play("Close");
                        return;
                    }
                }
            });
        }
        public void SetAvgbetEventHandler()
        {
            var AvgHelp_EventHandler = AvgHelpButton.transform.GetComponent<SelectEventCustomHandler>();
            if(AvgHelp_EventHandler==null)
                    AvgHelp_EventHandler = AvgHelpButton.transform.gameObject.AddComponent<SelectEventCustomHandler>();
            AvgHelp_EventHandler.BindingDeselectedAction(async (BaseEventData baseEventData)=>{
                XDebug.Log("按钮点击-AvgHelpButton");
                var anim = AvgBetTips.transform.GetComponent<Animator>();
                var clips = anim.GetCurrentAnimatorClipInfo(0);
                if(clips.Length>0){
                    if(clips[0].clip.name == "Tips_Close"){
                        return;
                    }
                    if(clips[0].clip.name == "Tips_Open" ||clips[0].clip.name == "Tips_Idle"){
                        anim.Update(0);
                        anim.Play("Close");
                        return;
                    }
                }
            });
        }
        public void InitShopInfos(bool isFull=false){
            boxIsMoveing = false;
            UpdateAvgText();
            UpdateTokenText();
            UpdatePageBoxInfo();
            if(isFull)
                UnlockPageIndex -=1;
            if(JudgeAllPageIsUnlock())
                RefreshCurUnlockPageIndex();
            UpdateDoorShow();
            boxIsMoveing = true;
            IsShowBubbles = false;
            ContentMoveEndPos(true);
        }
        public override float GetPopUpMaskAlpha()
        {
            return 0f;
        }
        /// <summary>
        /// 展示金币信息显示按钮
        /// </summary>
        /// <param name="type">类型，没钱和未解锁</param>
        public void ShowCoinTips(Transform box,int type){
            if(TipsObject == null){
                TipsObject = context.assetProvider.InstantiateGameObject("Tips");
                TipsObject.transform.SetParent(transform);
                TipsObject.transform.localScale = Vector3.one;
            }
            var anim = TipsObject.GetComponent<Animator>();
            var clips = anim.GetCurrentAnimatorClipInfo(0);
            if(clips.Length>0){
                if(clips[0].clip.name == "Tips_Open" ||clips[0].clip.name == "Tips_Idle"){
                    anim.Update(0);
                    anim.Play("Close");
                    return;
                }
            }

            TipsObject.transform.position = box.transform.position;
            for (int i = 1; i <= 6; i++)
            {
                TipsObject.transform.Find("TextGroup/Text" + i).gameObject.SetActive(false);
            }
            TipsObject.transform.Find("TextGroup").transform.localScale = new Vector3(0.9f,-0.9f,1);
            var curText = TipsObject.transform.Find("TextGroup/Text"+type);
            curText.transform.localScale = new Vector3(1,-1,1);
            curText.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("SpecialClick3");
            anim.Update(0);
            anim.Play("Open");
        }
        public void ShowTipsToAutoHide(Transform transform){
            if(IsShowBubbles) return;
            var anim = transform.GetComponent<Animator>();
            var clipArr = anim.GetCurrentAnimatorClipInfo(0);
            if(clipArr.Length>0){
                var clipName = clipArr[0].clip.name;
                if (clipName == "Tips_Open" || clipName == "Tips_Idle"){
                    anim.Update(0);
                    anim.Play("Close");
                    return;
                }
            }
            AudioUtil.Instance.PlayAudioFx("Close");
            anim.Update(0);
            anim.Play("Open");
        }
        
        public void UpdateAvgText(){
            if(AvgText!=null){
                var str = Constant11301.GetAbbreviationFormats(extraState.GetAvgBet().AvgBet,1);
                AvgText.GetComponent<Text>().text = ""+ str;
                
            }
        }
        public void UpdateTokenText(){
            if(CurrencyText!=null)
                CurrencyText.GetComponent<Text>().text = ""+extraState.GetCollectItems().GetCommaFormat();
            context.view.Get<ShopEntranceView11301>().SetBoxTokensNum(extraState.GetCollectItems());
        }
        /// <summary>
        /// 展示下一个页面
        /// </summary>
        public void ShowNextPage(int num){
            if(boxIsMoveing) return;
            if(IsShowBubbles) return;
            var res = UnlockPageIndex + num;
            if(res < 1 || res > 4) return;
            AudioUtil.Instance.PlayAudioFx("SpecialClick1");
            boxIsMoveing = true;
            LeftGroup.GetComponent<Animator>().Play("Open");
            XUtility.WaitSeconds(0.2f,new CancelableCallback(()=>{
                UnlockPageIndex = res;
                RefreshBtnStatus();
                UpdateDoorShow();
                ContentMoveEndPos();
            }));
            
        }
        public void RefreshBtnStatus(){
            if(UnlockPageIndex == 1)
                BackButton.gameObject.SetActive(false);
            else
                BackButton.gameObject.SetActive(true);
            if(UnlockPageIndex == 4)
                NextButton.gameObject.SetActive(false);
            else
                NextButton.gameObject.SetActive(true);
        }
        /// <summary>
        /// 根据当前的页面id更新对应的面板布局
        /// </summary>
        public void UpdateDoorShow(){
            InitAllPageIsActive();
            if(TipsObject!=null){
                TipsObject.GetComponent<Animator>().Play("Close");
            }
            var superFreeCount = extraState.GetRolesData()[UnlockPageIndex-1].SuperFreeSpinCount;
            LeftGroup.Find("BG/DoorGroup"+UnlockPageIndex).gameObject.SetActive(true);
            PageGroup.Find("Page"+UnlockPageIndex).gameObject.SetActive(true);
            LeftGroup.Find("IntegralText").GetComponent<Text>().text = ""+superFreeCount;
        
            RefreshBtnStatus();
        }
        public void ContentMoveEndPos(bool isInit=false){
            var rectTans = Content.transform.GetComponent<RectTransform>();
            var pagePosx = -Constant11301.pageWidth*(UnlockPageIndex-1);
            
            var endPos = new Vector3(pagePosx,0);
            if(isInit){ 
                rectTans.anchoredPosition3D = endPos;
                boxIsMoveing = false;
            }else{
                rectTans.DOAnchorPosX(pagePosx, 0.5f).OnComplete(() =>
                {
                    rectTans.anchoredPosition3D = endPos;
                });
                XUtility.WaitSeconds(0.65f,new CancelableCallback(()=>{
                    boxIsMoveing = false;
                }));
            }
            Constant11301.SaveCurrentPageIndexData(UnlockPageIndex);
        }

        /// <summary>
        /// 初始化更新page页面所有宝箱信息
        /// </summary>
        public void UpdatePageBoxInfo(bool isFull=false){
            var roleData = extraState.GetRolesData();

            for (int i = 1; i <= 4; i++)
            {   
                if(roleData[i-1].Unlocked)
                    UnlockPageIndex = i;
                var grids = roleData[i - 1].Grids;
                var curPage = Content.transform.Find("Page" + i);
                for (int j = 0; j < 9; j++)
                {
                    var box = curPage.Find("Box" + (j + 1));
                    var Price = grids[j].Price;
                    var pageIndex = i;
                    var index = j;
                    var _mouseClick = box.transform.Bind<PointerEventCustomHandler>(true);
                    _mouseClick.BindingPointerClick(async (a)=>{
                        if(IsShowBubbles) return;
                        if(grids[index].Opened)
                            return;
                        var curCollectItem = extraState.GetCollectItems();
                        int resPrice = (int)curCollectItem - (int)Price;
                        if(resPrice<0 && roleData[pageIndex-1].Unlocked){
                            XDebug.Log("没钱了==");
                            ShowCoinTips(box,6);
                            return;
                        }
                        if(!roleData[pageIndex-1].Unlocked){
                            ShowCoinTips(box,4);
                            return;
                        }
                        if(isClicking)
                            return;
                        isClicking = true;
                        IsShowBubbles = true;

                        AudioUtil.Instance.PlayAudioFx("SpecialClick2");
                        tempIsAllUnlock = JudgeAllPageIsUnlock();
                        BoxClickCallBack(box,index,pageIndex);
                    });

                    var box_EventHandler = box.transform.GetComponent<SelectEventCustomHandler>();
                    if (box_EventHandler == null)
                        box_EventHandler = box.transform.gameObject.AddComponent<SelectEventCustomHandler>();
                    box_EventHandler.BindingDeselectedAction(async (BaseEventData baseEventData) =>
                    {
                        XDebug.Log("按钮点击-boxhitCallBack==");
                        if(TipsObject==null)return;
                        var anim = TipsObject.GetComponent<Animator>();
                        var clips = anim.GetCurrentAnimatorClipInfo(0);
                        if (clips.Length > 0)
                        {
                            if (clips[0].clip.name == "Tips_Close")
                            {
                                return;
                            }
                            if (clips[0].clip.name == "Tips_Open" || clips[0].clip.name == "Tips_Idle")
                            {
                                anim.Update(0);
                                anim.Play("Close");
                                return;
                            }
                        }
                    });
                    var boxAnim = box.GetComponent<Animator>();
                    var curClips = boxAnim.GetCurrentAnimatorClipInfo(0);
                    
                    //当前宝箱是否开启
                    if (grids[j].Opened && curClips.Length>0)
                    {
                        //根据数据类型，播放对应的宝箱动画
                        if (grids[j].RewardType == RewardType.Coin)
                        {
                            XUtility.PlayAnimation(boxAnim, "CoinGroupIdle");
                            ulong goldNum = grids[j].AvgBet * grids[j].RewardValue / 100;
                            box.transform.Find("CoinGroup/IntegralText").GetComponent<Text>().text = "" + Constant11301.GetAbbreviationFormats(goldNum,0);
                        }
                        else if (grids[j].RewardType == RewardType.FreeGame)
                        {
                            XUtility.PlayAnimation(boxAnim, "FreeGameGroupIdle");
                        }
                        else if (grids[j].RewardType == RewardType.Respin)
                        {
                            XUtility.PlayAnimation(boxAnim, "RespinBonusIdle");
                        }
                    }
                    else
                    {
                        if (!roleData[i - 1].Unlocked)
                            XUtility.PlayAnimation(boxAnim,"Grey");
                        box.transform.Find("LockGroup/CurrencyGroup/IntegralText").GetComponent<Text>().text = "" + Price.GetCommaFormat();
                    }
                }
            }
        }
        
        /// <summary>
        /// 页面全部隐藏
        /// </summary>
        public void InitAllPageIsActive(){
            for (int i = 1; i <= 4; i++)
            {
                LeftGroup.Find("BG/DoorGroup" + i).gameObject.SetActive(false);
                PageGroup.Find("Page" + i).gameObject.SetActive(false);
            }
        }

        public void BoxClickCallBack(Transform box,int choose,int pageIndex){
            context.state.Get<ExtraState11301>().ChooseBoxRequest((uint)choose,(uint)pageIndex,
                async (bool ok,SBonusProcess sBonusProcess)=>{
                    if(ok){
                        XDebug.Log("点击商城宝箱请求成功");
                        var grids = extraState.GetRolesData()[pageIndex - 1].Grids;
                        XUtility.PlayAnimationAsync(box.GetComponent<Animator>(),"Collect");
                        await XUtility.WaitSeconds(0.3f);
                        PageBoxUnlockInfo(choose,pageIndex,sBonusProcess);
                        
                    }else{
                        XDebug.Log("点击商城宝箱请求异常");
                    }
                }
            );
        }
        /// <summary>
        /// 初始化更新page页面所有宝箱信息
        /// </summary>
        public async void PageBoxUnlockInfo(int choose,int pageIndex,SBonusProcess sBonusProcess){
            var roleData = extraState.GetRolesData();
            var grids = roleData[pageIndex - 1].Grids;
            var curPage = Content.transform.Find("Page" + pageIndex);
            var isClose = false;
            //当前宝箱是否开启
            if (grids[choose].Opened)
            {
                // context.view.Get<ControlPanel>().UpdateBoxPrizeProgress(0,0);
                // context.view.Get<ControlPanel>().PlayBoxAnimation("Wait");
                var box = curPage.Find("Box" + (choose + 1));
                //根据数据类型，播放对应的宝箱动画
                if (grids[choose].RewardType == RewardType.Coin)
                {
                    box.GetComponent<Animator>().Play("CoinGroup");
                    ulong goldNum = grids[choose].AvgBet * grids[choose].RewardValue / 100;
                    box.transform.Find("CoinGroup/IntegralText").GetComponent<Text>().text = "" + Constant11301.GetAbbreviationFormats(goldNum,0);
                    // 同步金币到用户左上角账户
                    EventBus.Dispatch(new EventUserProfileUpdate(sBonusProcess.UserProfile));
                    EventBus.Dispatch(new EventRefreshUserProfile());
                    //同步金币到下方赢钱框内
                    var audioName = "Symbol_SmallWin_" + context.assetProvider.AssetsId;
                    var stopAudioName = "Symbol_SmallWinEnding_" + context.assetProvider.AssetsId;
                    AudioUtil.Instance.StopAudioFx(audioName);
                    AudioUtil.Instance.PlayAudioFx(audioName);
                    XUtility.WaitSeconds(0.5f,new CancelableCallback(()=>{
                        AudioUtil.Instance.StopAudioFx(audioName);
                        AudioUtil.Instance.StopAudioFx(stopAudioName);
                        AudioUtil.Instance.PlayAudioFx(stopAudioName);
                    }));
                    context.view.Get<ControlPanel>().UpdateWinLabelChips(0);
                    context.view.Get<ControlPanel>().UpdateWinLabelChipsWithAnimation((long)goldNum,
                        0.5f, false);
                    
                    //判断当前页是否满了，是则进入superFree
                    if(JudgeGridsIsFull(grids)){
                        LeftGroup.GetComponent<Animator>().Play("Collect");
                        await XUtility.WaitSeconds(2);
                        context.view.Get<ControlPanel>().UpdateWinLabelChips(0);
                        extraState.SuperFreePattern = roleData[pageIndex - 1].SuperFreePattern;
                        context.JumpToLogicStep(LogicStepType.STEP_FREE_GAME, LogicStepType.STEP_MACHINE_SETUP);
                        if(pageIndex==4 && !tempIsAllUnlock)
                            UnlockPageIndex = 1;
                        isClose = true;
                        Close();
                    }
                    await XUtility.WaitSeconds(0.5f);
                }
                else if (grids[choose].RewardType == RewardType.FreeGame)
                {
                    box.GetComponent<Animator>().Play("FreeGameGroup");
                    await XUtility.WaitSeconds(1);
                    context.view.Get<ControlPanel>().UpdateWinLabelChips(0);
                    //进入Free
                    context.JumpToLogicStep(LogicStepType.STEP_FREE_GAME, LogicStepType.STEP_MACHINE_SETUP);
                    isClose = true;
                    Close();
                }
                else if (grids[choose].RewardType == RewardType.Respin)
                {
                    box.GetComponent<Animator>().Play("RespinBonus");
                    await XUtility.WaitSeconds(1);
                    context.view.Get<ControlPanel>().UpdateWinLabelChips(0);
                    // 踩坑---跳转前先切轮盘state
                    context.state.Get<WheelsActiveState11301>().UpdateRunningWheelState(true,false);
                    //进入Respin
                    context.JumpToLogicStep(LogicStepType.STEP_RE_SPIN, LogicStepType.STEP_MACHINE_SETUP);
                    isClose = true;
                    Close();
                }
            }
            IsShowBubbles = false;
            isClicking = false;
            if(isClose) return;
            var isFull = JudgeGridsIsFull(grids);
            //当全部解锁时，不再刷新下一页面
            if (JudgeAllPageIsUnlock())
                isFull = false;

            InitShopInfos(isFull);
        }
        /// <summary>
        /// 判断格子是否满了
        /// </summary>
        public bool JudgeGridsIsFull(RepeatedField<RoleGridData> grids)
        {
            var noOpenedNum = 0;
            for (int i = 0; i < grids.count; i++)
            {
                if(!grids[i].Opened){
                    noOpenedNum++;
                }
            }
            if(noOpenedNum>0)
                return false;
            return true;
        }
        /// <summary>
        /// 刷新当前页面的index,是否全部解锁
        /// </summary>
        public bool JudgeAllPageIsUnlock(){
            return extraState.IsAllOpened();
        }
        /// <summary>
        /// 当前全部解锁时，刷新上一次页面索引
        /// </summary>
        public void RefreshCurUnlockPageIndex(){
            var data = Constant11301.GetLastPageIndexData();
            if(data!=0)
                UnlockPageIndex = data;
            else
                UnlockPageIndex = 1;

        }
        public override void DoCloseAction()
        {
            base.DoCloseAction();
        }
        public override async Task OnClose()
        {
            if(TipsObject!=null){
                var anim = TipsObject.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0);
                if(anim.Length>0){
                    if(anim[0].clip.name != "Tips_Close"){
                        XUtility.PlayAnimation(TipsObject.GetComponent<Animator>(), "Close", () =>
                        {
                            GameObject.Destroy(TipsObject.gameObject);
                        });
                    }else{
                        GameObject.Destroy(TipsObject.gameObject);
                    }
                }
          
            }
            if (animator && animator.HasState("Close"))
                await XUtility.PlayAnimationAsync(animator, "Close");
            //每次close后，判断是否全部解锁。是则把当前商城页index存储下来
            if(JudgeAllPageIsUnlock()){
                Constant11301.SaveCurrentPageIndexData(UnlockPageIndex);
            }else{
                Constant11301.SaveCurrentPageIndexData(1);
            }
            context.view.Get<TransitionView11301>().UpdateShopMaskShow(false);
        }
    }
}

