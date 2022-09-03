using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIVIPMain")]
    public class UIVIPMainView : Popup
    {
        [ComponentBinder("CurrentVIPIcon")]
        private Image imgCurrentVIPTitleIcon;
        
        [ComponentBinder("CurrentTextImage")]
        private Image imgCurrentVip;

        [ComponentBinder("NextTextImage")]
        private Image imgNextVip;

        [ComponentBinder("Icon","CurrentVIPGroup")]
        private Image imgCurrentVipIcon;

        [ComponentBinder("Icon","NextVIPGroup")]
        private Image imgNextVipIcon;

        [ComponentBinder("ProgressText")]
        private TextMeshProUGUI txtProgress;

        [ComponentBinder("PercentageText")]
        private TextMeshProUGUI txtPercentage;

        [ComponentBinder("CurrentCountText")]
        private TextMeshProUGUI txtCurrentVipExp;

        [ComponentBinder("NextCountText")]
        private TextMeshProUGUI txtNextVipExp;

        [ComponentBinder("CheckButton")]
        private Button btnCheck;

        [ComponentBinder("InformationButton")]
        private Button btnInformation;

        [ComponentBinder("ProgressGroup")]
        private Transform tranProgressGroup;

        [ComponentBinder("VIPCurrentPointGroup")]
        private Transform tranVIPCurrentPointGroup;

        [ComponentBinder("VIPNextPointGroup")]
        private Transform tranVIPNextPointGroup;

        [ComponentBinder("ProgressBar")]
        private Slider slider;

        [ComponentBinder("MainGroup")]
        private Transform tranMainGroup;

        [ComponentBinder("MaxGroup")]
        private Transform tranMaxGroup;

        [ComponentBinder("CurrentCountText","MaxVIPCurrentPointGroup")]
        private TextMeshProUGUI txtMaxVipPoint;


        public UIVIPMainView(string addrass) : base(addrass)
        {
            
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            
            btnCheck.onClick.AddListener(OnBtnCheckClick);
            btnInformation.onClick.AddListener(OnBtnInformationClick);
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


        private SpriteAtlas atlas;
        private VipController _vipController;
        public override  void OnOpen()
        {
            try
            {
                base.OnOpen();
                atlas =AssetHelper.GetResidentAsset<SpriteAtlas>("CommonUIAtlas");
                _vipController = Client.Get<VipController>();
                RefreshUI();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            
        }
 
        private  void RefreshUI()
        {
            uint vipLevel = _vipController.GetVipLevel();
            imgCurrentVIPTitleIcon.sprite = atlas.GetSprite($"UI_VIP_icon_{vipLevel}");
            imgCurrentVip.sprite = atlas.GetSprite($"UI_VIP_levelText_{vipLevel}");
            imgCurrentVipIcon.sprite = atlas.GetSprite($"UI_VIP_icon_{vipLevel}");

            tranMainGroup.gameObject.SetActive(false);
            tranMaxGroup.gameObject.SetActive(false);
            
            if (vipLevel < 7)
            {
                
                tranMainGroup.gameObject.SetActive(true);
                uint nextVipLevel = vipLevel + 1;
                tranProgressGroup.gameObject.SetActive(true);
                tranVIPCurrentPointGroup.gameObject.SetActive(true);
                tranVIPNextPointGroup.gameObject.SetActive(true);
                
                imgNextVip.sprite = atlas.GetSprite($"UI_VIP_levelText_{nextVipLevel}");
                imgNextVipIcon.sprite = atlas.GetSprite($"UI_VIP_icon_{nextVipLevel}");

                //txtProgress.text = $"{_vipController.GetCurrentVipExp()}/{_vipController.GetNowLevelNeedExp()}";

                float prograss = _vipController.GetProgress();
                txtPercentage.text = $"{(int)(prograss*100)}%";
                txtProgress.text = $"{(int)(prograss*100)}%";
                slider.value = prograss;

                txtCurrentVipExp.text = _vipController.GetCurrentVipExp().ToString();
                txtNextVipExp.text = _vipController.GetNextVipExp().ToString();
                
            }
            else
            {
                tranMaxGroup.gameObject.SetActive(true);
                txtMaxVipPoint.text = _vipController.GetCurrentVipExp().ToString();
                // tranProgressGroup.gameObject.SetActive(false);
                // tranVIPCurrentPointGroup.gameObject.SetActive(false);
                // tranVIPNextPointGroup.gameObject.SetActive(false);
            }
            
            imgCurrentVip.SetNativeSize();
            imgNextVip.SetNativeSize();

        }
        
        
        private void OnBtnInformationClick()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(UIVIPInformationView))));
        }

        private void OnBtnCheckClick()
        {
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(UIVIPLevelDetialView), closeAction, blockLevel)));
            closeAction = null;
            Close();
        }
    }
}