using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidLevelHelp")]
    public class TreasureRaidHelpPopup : Popup<TreasureRaidHelpPopupViewController>
    {
        [ComponentBinder("Root/MainGroup")] public RectTransform mainGroup;
        [ComponentBinder("Root/PointGroupLight")] public RectTransform pointGroup;

        [ComponentBinder("Root/ShiftPageGroup/PreviousButton")]
        public Button btnLeft;

        [ComponentBinder("Root/ShiftPageGroup/NextButton")]
        public Button btnRight;

        public TreasureRaidHelpPopup(string address)
            : base(address)
        {
            contentDesignSize = ViewResolution.designSize;
        }
    }

    public class TreasureRaidHelpPopupViewController : ViewController<TreasureRaidHelpPopup>
    {
        protected int currentIndex = 0;

        protected List<Transform> pages;
        protected List<Transform> points;

        protected override void SubscribeEvents()
        {
            view.btnLeft.onClick.AddListener(OnBtnLeftClicked);
            view.btnRight.onClick.AddListener(OnBtnRightClicked);
        }
        
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            var childCount = view.mainGroup.childCount;
            pages = new List<Transform>();
            points = new List<Transform>();

            for (var i = 0; i < childCount; i++)
            {
                var page = view.mainGroup.GetChild(i);
                pages.Add(page);
                pages[i].gameObject.SetActive(i == 0);
                var point = view.pointGroup.GetChild(i);
                points.Add(point);
                points[i].gameObject.SetActive(i == 0);
            }
            
            view.btnLeft.gameObject.SetActive(false);
            view.btnRight.gameObject.SetActive(true);
        }

        private void OnBtnRightClicked()
        {
            if (currentIndex < pages.Count - 1)
            {
                currentIndex++;
                pages[currentIndex - 1].gameObject.SetActive(false);
                pages[currentIndex].gameObject.SetActive(true);

                points[currentIndex - 1].gameObject.SetActive(false);
                points[currentIndex].gameObject.SetActive(true);
 
                view.btnRight.gameObject.SetActive(currentIndex < pages.Count - 1);
                view.btnLeft.gameObject.SetActive(true);
            }
        }

        private void OnBtnLeftClicked()
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                pages[currentIndex + 1].gameObject.SetActive(false);
                pages[currentIndex].gameObject.SetActive(true);

                points[currentIndex + 1].gameObject.SetActive(false);
                points[currentIndex].gameObject.SetActive(true);

                view.btnLeft.gameObject.SetActive(currentIndex > 0);
                view.btnRight.gameObject.SetActive(true);
            }
        }
    }
}