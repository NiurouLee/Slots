//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-04 08:28
//  Ver : 1.0.0
//  Description : SeasonPassHelp.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISeasonPassHelpH", "UISeasonPassHelpV")]
    public class SeasonPassHelp : Popup<SeasonPassHelpViewController>
    {
        [ComponentBinder("Root/ToggleGroup/BtnLeft")]
        private Button btnLeft;
        [ComponentBinder("Root/ToggleGroup/BtnRight")]
        private Button btnRight;
        [ComponentBinder("Root/MainGroup/Point1/CheckMark")]
        private Transform transCheckMark1;
        [ComponentBinder("Root/MainGroup/Point2/CheckMark")]
        private Transform transCheckMark2;
        [ComponentBinder("Root/MainGroup/Point3/CheckMark")]
        private Transform transCheckMark3;
        [ComponentBinder("Root/MainGroup/Point4/CheckMark")]
        private Transform transCheckMark4;
        [ComponentBinder("Root/MainGroup/Page1")]
        private Transform transPage1;
        [ComponentBinder("Root/MainGroup/Page2")]
        private Transform transPage2;
        [ComponentBinder("Root/MainGroup/Page3")]
        private Transform transPage3;
        [ComponentBinder("Root/MainGroup/Page4")]
        private Transform transPage4;

        private bool canChangePage = true;
        private int nCurrentIndex = -1;
        private Transform[] arrayCheckMask;
        private Transform[] arrayPage;

        public SeasonPassHelp(string assetAddress)
            : base(assetAddress)
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            if (isPortrait)
            {
                contentDesignSize = new Vector2(768, 1365);
            }
            else
            {
                contentDesignSize = new Vector2(1365, 768);
            }
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            arrayPage = new[] { transPage1, transPage2, transPage3, transPage4 };
            arrayCheckMask = new[] { transCheckMark1, transCheckMark2, transCheckMark3, transCheckMark4 };
            OnSelectItem(1);
            btnLeft.onClick.AddListener(() => OnSelectItem(-1));
            btnRight.onClick.AddListener(() => OnSelectItem(1));
        }
        private async void OnSelectItem(int deltaIndex)
        {
            if (!canChangePage) return;
            canChangePage = false;
            if (nCurrentIndex == 0 && deltaIndex == -1)
            {
                return;
            }

            if (nCurrentIndex == 3 && deltaIndex == 1)
            {
                return;
            }
            nCurrentIndex += deltaIndex;
            for (int i = 0; i < 4; i++)
            {
                arrayPage[i].gameObject.SetActive(i == nCurrentIndex);
                arrayCheckMask[i].gameObject.SetActive(i == nCurrentIndex);
            }
            if (nCurrentIndex == 0)
            {
                btnLeft.gameObject.SetActive(false);
                btnRight.gameObject.SetActive(true);
            }
            else if (nCurrentIndex == 3)
            {
                btnLeft.gameObject.SetActive(true);
                btnRight.gameObject.SetActive(false);
            }
            else
            {
                btnLeft.gameObject.SetActive(true);
                btnRight.gameObject.SetActive(true);
            }

            await XUtility.WaitSeconds(0.1f, viewController);
            canChangePage = true;
        }
    }

    public class SeasonPassHelpViewController : ViewController<SeasonPassHelp>
    {

    }
}