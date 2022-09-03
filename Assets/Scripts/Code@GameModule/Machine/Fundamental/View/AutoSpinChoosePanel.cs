using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;
namespace GameModule
{
    public class AutoSpinChoosePanel : TransformHolder
    {
        private Action<int> autoSpinClickAction;

        public AutoSpinChoosePanel(Transform transform, Action<int> inAutoSpinClickAction) :
            base(transform)
        {
            autoSpinClickAction = inAutoSpinClickAction;

            List<int> listSuffix = new List<int> {10, 20, 50, 75, 100};
            List<int> listAutoSpinCount = new List<int> {20, 50, 100, 200, 500};
            InitializeView(listSuffix, listAutoSpinCount);
            RegisterButtonEvent(listSuffix, listAutoSpinCount);
        }

        private void InitializeView(List<int> listSuffix, List<int> listAutoSpinCount)
        {
            for (int i = 0; i < listSuffix.Count; i++)
            {
                int suffix = listSuffix[i];
                var text = transform.Find("Image/BetTextGroup/Auto" + suffix + "Text").GetComponent<TextMeshProUGUI>();
                text.text = listAutoSpinCount[i].ToString();
            }

            var selectEventCustomHandler = transform.gameObject.AddComponent<SelectEventCustomHandler>();

            selectEventCustomHandler.BindingDeselectedAction(OnDeselected);

            transform.gameObject.SetActive(false);
        }

        private void RegisterButtonEvent(List<int> listSuffix, List<int> listAutoSpinCount)
        {
            for (int i = 0; i < listSuffix.Count; i++)
            {
                int index = i;
                int suffix = listSuffix[i];
                Button button = transform.Find("Image/Auto" + suffix + "Button").GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    HideSelectView();
                    autoSpinClickAction(listAutoSpinCount[index]);
                });
            }
        }

        public async void OnDeselected(BaseEventData eventData)
        { 
           await XUtility.WaitNFrame(1);
           if (transform == null) return;
           if ( !EventSystem.current.currentSelectedGameObject || !EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))
           {
               HideSelectView();
           }
        }

        public void CheckAndShowSelectView()
        {
            var animator = transform.GetComponent<Animator>();
            transform.gameObject.SetActive(true);
            animator.Play("Appear");
            EventSystem.current.SetSelectedGameObject(transform.gameObject);
        }

        public void HideSelectView()
        {
            transform.gameObject.SetActive(false);
        }
    }
}