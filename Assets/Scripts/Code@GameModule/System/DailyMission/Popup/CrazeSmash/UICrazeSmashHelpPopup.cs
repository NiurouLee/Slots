
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UICrazeSmashHelp", "UICrazeSmashHelpV")]
    public class UICrazeSmashHelpPopup : Popup<UICrazeSmashHelpPopupController>
    {
        [ComponentBinder("Root/MainGroup")]
        public Transform transformPageRoot;

        [ComponentBinder("Root/PointLayout")]
        public Transform transformPointRoot;

        [ComponentBinder("Root/ToggleGroup/ButtonLeft")]
        public Button buttonPrevious;

        [ComponentBinder("Root/ToggleGroup/ButtonRight")]
        public Button buttonNext;


        public GameObject[] pages;

        public GameObject[] points;

        public const int PageCount = 3;


        public UICrazeSmashHelpPopup(string address) : base(address)
        {
            var isPortrait = ViewManager.Instance.IsPortrait;
            if (isPortrait)
            {
                contentDesignSize = new Vector2(768, 1272);
            }
            else
            {
                contentDesignSize = new Vector2(1272, 768);
            }
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            pages = new GameObject[PageCount];
            points = new GameObject[PageCount];

            for (int i = 0; i < PageCount; i++)
            {
                pages[i] = transformPageRoot.Find($"Page{i + 1}").gameObject;
                points[i] = transformPointRoot.Find($"Point{i + 1}/CheckMark").gameObject;
            }
        }

        public void Set(int pageIndex)
        {
            pageIndex = Mathf.Clamp(pageIndex, 0, PageCount - 1);
            for (int i = 0; i < PageCount; i++)
            {
                pages[i].SetActive(i == pageIndex);
                points[i].SetActive(i == pageIndex);
            }
            buttonPrevious.gameObject.SetActive(pageIndex > 0);
            buttonNext.gameObject.SetActive(pageIndex < PageCount - 1);
        }
    }

    public class UICrazeSmashHelpPopupController : ViewController<UICrazeSmashHelpPopup>
    {
        private int _index;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            _index = 0;
            view.Set(0);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.buttonPrevious.onClick.AddListener(OnPrevious);
            view.buttonNext.onClick.AddListener(OnNext);
        }

        private void OnNext()
        {
            view.Set(++_index);
        }

        private void OnPrevious()
        {
            view.Set(--_index);
        }
    }
}