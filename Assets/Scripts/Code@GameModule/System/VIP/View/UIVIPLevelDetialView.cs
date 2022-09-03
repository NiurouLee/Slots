using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tool;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GameModule
{
    [AssetAddress("UIVIPLevelDetail")]
    public class UIVIPLevelDetialView : Popup
    {

        [ComponentBinder("CurrentVIPIcon")]
        private Image imgCurrentVIPIcon;

        [ComponentBinder("BackButton")]
        private Button btnBack;

        [ComponentBinder("BubbleGroup")]
        private Button btnBubbleGroup;

        [ComponentBinder("BubbleText")]
        private TextMeshProUGUI txtBubbleInfo;

        [ComponentBinder("BG","BubbleGroup")]
        private RectTransform tranBubbleBG;

        [ComponentBinder("VIPScroll")]
        private ContentScrollSnapHorizontal _contentScrollSnapHorizontal;

        [ComponentBinder("PagePointGroup")]
        private Transform tranPagePointGroup;


        #region GetBenefitsTitle

        [ComponentBinder("InformationButton","CoinPackagesCell")]
        private Button btnCoinPackages;
       
        [ComponentBinder("InformationButton","DailyBonusCell")]
        private Button btnDailyBonus;

        [ComponentBinder("InformationButton","TimerBonusCell")]
        private Button btnTimerBonus;

       

        [ComponentBinder("InformationButton","VIPPointsCell")]
        private Button btnVIPPoints;

      
        [ComponentBinder("InformationButton","WeeklyBonusCell")]
        private Button btnWeeklyBonus;
        
        [ComponentBinder("InformationButton","LevelUpBonusCell")]
        private Button btnLevelUpBonus;
        
        [ComponentBinder("InformationButton","CollectionBonusCell")]
        private Button btnCollectionBonus;
        
        [ComponentBinder("InformationButton","LottoBonusCell")]
        private Button btnLottoBonus;
        
        
        #endregion
        public UIVIPLevelDetialView(string addrass) : base(addrass)
        {
            
        }

       
        public override Vector3 CalculateScaleInfo()
        {
            if (ViewManager.Instance.IsPortrait)
            {
                return new Vector3(0.55f, 0.55f, 0.55f);
            }
            else
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    float scale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    return Vector3.one * scale;
                }
            }

            return Vector3.one;
        }

        private void OnToggleClcik(int index,bool isCheck)
        {
            if (isCheck)
            {
                _contentScrollSnapHorizontal.GoTo(index);
            }
        }

        private VipController _vipController;
        public override void OnOpen()
        {
            base.OnOpen();
            _vipController = Client.Get<VipController>();
            
            RefreshUI();
        }
     
        protected void RefreshUI()
        {
            int vipLevel = (int) _vipController.GetVipLevel();
            SetVipLevelState(vipLevel);
            if (vipLevel > 4)
            {
                _contentScrollSnapHorizontal.NextItem();
            }
            
            
            var atlas = AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
            imgCurrentVIPIcon.sprite = atlas.GetSprite($"UI_VIP_icon_{vipLevel}");
        }


        
        protected List<UIVIPLevelDetialItemTitle> listVipItemTitle = new List<UIVIPLevelDetialItemTitle>();
        protected List<UIVIPLevelDetialItem> listVipItem = new List<UIVIPLevelDetialItem>();

        //private UIVIPLevelDetialItemTitle vipItemTitle4;
        //private UIVIPLevelDetialItem vipItem4;
        
        
        private List<string> listBubbleInfo = new List<string>()
        {
            @"Get more coins from ""Buy Coins""! Any store purchased coins will be extra multiplied by your VIP level.",
            @"Daily Bonus will be extra multiplied according to your VIP level.",
            @"Timer Bonus will be extra multiplied according to your VIP level.",
            @"Earn more VIP Points for every purchase.",
            @"Get coins from INBOX every week.",
            @"Receive additional rewards when your VIP level up.",
            @"Get bigger rewards when you complete any Lucky Album sets.",
            @"Get bigger rewards when you play the lotto game."
        };

        private List<Button> listBubbleButton = new List<Button>();
        protected override void BindingComponent()
        {
            base.BindingComponent();
            btnBack.onClick.AddListener(OnBtnBackClick);
            btnBubbleGroup.onClick.AddListener(OnBtnBubbleGroupClick);
            btnBubbleGroup.gameObject.SetActive(false);
            
            listBubbleButton.Add(btnCoinPackages);
            listBubbleButton.Add(btnDailyBonus);
            listBubbleButton.Add(btnTimerBonus);
            listBubbleButton.Add(btnVIPPoints);
            
            listBubbleButton.Add(btnWeeklyBonus);
            listBubbleButton.Add(btnLevelUpBonus);
            listBubbleButton.Add(btnCollectionBonus);
            listBubbleButton.Add(btnLottoBonus);
            

            for (int i = 0; i < listBubbleButton.Count; i++)
            {
                var index = i;
                if (listBubbleButton[i] != null)
                {
                    listBubbleButton[i].onClick.AddListener(() =>
                    {
                        OnBtnBubbleClick(listBubbleButton[index], index);
                    });
                }
            }

            for (int i = 1; i <= 4; i++)
            {
                Transform tran =
                    transform.Find(
                        $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage1/TitleGroup/VIP{i}Group");
                UIVIPLevelDetialItemTitle itemTitle = new UIVIPLevelDetialItemTitle(tran);
                listVipItemTitle.Add(itemTitle);
                
                
                Transform tranItem =
                    transform.Find(
                        $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage1/Viewport/Content/VIP{i}DetailGroup");
                UIVIPLevelDetialItem item = new UIVIPLevelDetialItem(tranItem);
                listVipItem.Add(item);
            }
            
            
            Transform tran4 =
                transform.Find(
                    $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage2/TitleGroup/VIP{4}Group");
            //vipItemTitle4 = new UIVIPLevelDetialItemTitle(tran4);
            Transform tranItem4 =
                transform.Find(
                    $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage2/Viewport/Content/VIP{4}DetailGroup");
            //vipItem4 = new UIVIPLevelDetialItem(tranItem4);
           
            
            for (int i = 5; i <= 7; i++)
            {
                Transform tran =
                    transform.Find(
                        $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage2/TitleGroup/VIP{i}Group");
                UIVIPLevelDetialItemTitle itemTitle = new UIVIPLevelDetialItemTitle(tran);
                listVipItemTitle.Add(itemTitle);
                
                
                
                Transform tranItem =
                    transform.Find(
                        $"Root/MainGroup/VIPScroll/Viewprot/Content/VIPScrollViewPage2/Viewport/Content/VIP{i}DetailGroup");
                UIVIPLevelDetialItem item = new UIVIPLevelDetialItem(tranItem);
                listVipItem.Add(item);
            }
            
            
            for (int i = 1; i <= 2; i++)
            {
                int index = i - 1;
                tranPagePointGroup.Find($"Page{i}").GetComponent<Toggle>().onValueChanged.AddListener((isCheck) =>
                {
                    OnToggleClcik(index, isCheck);
                });
            }
        }

        


        protected void SetVipLevelState(int vipLevel)
        {
            //vipItemTitle4.SetNormal();
            //vipItem4.SetNormal();
            for (int i = 0; i < listVipItemTitle.Count; i++)
            {
                if (i == vipLevel - 1)
                {
                    listVipItemTitle[i].SetCurrent();
                    listVipItem[i].SetCurrent();

                    if (i == 3)
                    {
                        //vipItemTitle4.SetCurrent();
                        //vipItem4.SetCurrent();
                    }
                }
                else if (i < vipLevel - 1)
                {
                    listVipItemTitle[i].SetNext();
                    listVipItem[i].SetNext();
                    
                    if (i == 3)
                    {
                        //vipItemTitle4.SetNext();
                        //vipItem4.SetNext();
                    }
                }
                else
                {
                    listVipItemTitle[i].SetNormal();
                    listVipItem[i].SetNormal();
                }

                
            }
        }

        private void OnBtnBubbleGroupClick()
        {
            if (coroCloseBubble != null)
            {
                IEnumeratorTool.instance.StopCoroutine(coroCloseBubble);
            }
            btnBubbleGroup.gameObject.SetActive(false);
        }

        protected Coroutine coroCloseBubble;
        private WaitForSeconds _waitForSeconds = new WaitForSeconds(5);
        private void OnBtnBubbleClick(Button sender,int index)
        {
            txtBubbleInfo.text = listBubbleInfo[index];
            tranBubbleBG.position = sender.transform.position;
            btnBubbleGroup.gameObject.SetActive(true);

            if (coroCloseBubble != null)
            {
                IEnumeratorTool.instance.StopCoroutine(coroCloseBubble);
            }
            coroCloseBubble = IEnumeratorTool.instance.StartCoroutine(AutoCloseBubble());
        }


        protected IEnumerator AutoCloseBubble()
        {
            yield return _waitForSeconds;
            btnBubbleGroup.gameObject.SetActive(false);
        }


        private void OnBtnBackClick()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(UIVIPMainView), closeAction, blockLevel)));
            closeAction = null;
            this.Close();
        }
    }
}