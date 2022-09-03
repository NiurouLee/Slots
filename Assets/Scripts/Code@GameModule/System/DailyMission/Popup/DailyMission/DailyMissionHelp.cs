//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 18:01
//  Ver : 1.0.0
//  Description : DailyMissionHelp.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIDailyMissionHelpH", "UIDailyMissionHelpV")]
    public class DailyMissionHelp : Popup
    {
        [ComponentBinder("Root/ToggleGroup/TogglePage1")]
        private Toggle _toggle1;
        [ComponentBinder("Root/MainGroup/Point1/CheckMark")]
        private Transform transCheckMark1;
        [ComponentBinder("Root/ToggleGroup/TogglePage2")]
        private Toggle _toggle2;
        [ComponentBinder("Root/MainGroup/Point2/CheckMark")]
        private Transform transCheckMark2;
        [ComponentBinder("Root/MainGroup/Page1")]
        private Transform transPage1;
        [ComponentBinder("Root/MainGroup/Page2")]
        private Transform transPage2;

        public DailyMissionHelp(string assetAddress)
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
            OnSelectItem(true);
            _toggle1.onValueChanged.AddListener(x => OnSelectItem(true));
            _toggle2.onValueChanged.AddListener(x => OnSelectItem(false));
        }
        private void OnSelectItem(bool visible)
        {
            transPage1.gameObject.SetActive(visible);
            transCheckMark1.gameObject.SetActive(visible);
            transPage2.gameObject.SetActive(!visible);
            transCheckMark2.gameObject.SetActive(!visible);
            _toggle1.gameObject.SetActive(!visible);
            _toggle2.gameObject.SetActive(visible);
        }
    }
}