using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class PopUpManager
    {
        private static PopUpManager _instance;

        public static PopUpManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PopUpManager();
                }

                return _instance;
            }
        }


        private MachineAssetProvider assetProvider;
        private Transform container;
        private GameObject grayMask;
        private List<MachinePopUp> popUpStack;
        private MachineContext machineContext;


        public void OnDestroy()
        {
            CloseAllPopUp();
            assetProvider = null;
            container = null;
            grayMask = null;
            popUpStack.Clear();
        }

        public void BindingContext(MachineContext context)
        {
            this.machineContext = context;
            assetProvider = context.assetProvider;
            container = context.MachinePopUpCanvasTransform;
            grayMask = context.MachinePopUpCanvasTransform.Find("GrayMask").gameObject;
            popUpStack = new List<MachinePopUp>();
        }

        protected GameObject GetPopUp(string popUpAddress)
        {
            return assetProvider.InstantiateGameObject(popUpAddress);
        }

        public T GetPopup<T>() where T : MachinePopUp
        {
            for (var i = 0; i < popUpStack.Count; i++)
            {
                if (popUpStack[i].GetType() == typeof(T))
                {
                    return popUpStack[i] as T;
                }
            }
            return null;
        }

        public T ShowPopUp<T>(string fullAddress = null) where T : MachinePopUp
        {
            if (string.IsNullOrEmpty(fullAddress))
            {
                Type type = typeof(T);
                fullAddress = type.Name;
            }

 
            // string fullAddress = "Dialog/Slot_" + subjectId + "_" + dialogAddress;
            var popUpObject = GetPopUp(fullAddress);

 
            //找到参数Transform的构造方法
            var constructor = typeof(T).GetConstructor(new[] { typeof(Transform) });

            if (constructor != null && popUpObject != null)
            {
                //构造Dialog对象
                T dialog = constructor.Invoke(new object[] { popUpObject.transform }) as T;
                dialog.Initialize(machineContext);
                ShowPopUp(dialog);
                dialog.OnOpen();


                return dialog;
            }



            return null;
        }

        public void ShowPopUp<T>(T popUp) where T : MachinePopUp
        {
            grayMask.transform.SetAsLastSibling();
            popUp.transform.SetParent(container.transform, false);
            popUp.transform.localPosition = Vector3.zero;
            popUp.transform.SetAsLastSibling();
            popUpStack.Add(popUp);


            //外面再套一层，因为分辨率缩放可能会跟动画缩放相互发生冲突
            var containerPopUp = new GameObject("Container");
            var rect = containerPopUp.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 0);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = Vector2.zero;
            containerPopUp.transform.localPosition = Vector3.zero;
            containerPopUp.transform.SetParent(container.transform, false);
            containerPopUp.transform.SetAsLastSibling();
            popUp.container = containerPopUp.transform;
 
            var maskImage = grayMask.GetComponent<Image>();
            var color = maskImage.color;
            color.a = popUp.GetPopUpMaskAlpha();
            maskImage.color = color;

            
            popUp.transform.SetParent(containerPopUp.transform, false);

         
            if (popUp.EnableAutoAdapt())
            {
                if (ViewManager.Instance.IsPortrait)
                {
                    var viewSize = ViewResolution.referenceResolutionPortrait;
                    if (viewSize.y < ViewResolution.designSize.x)
                    {
                        var scale = viewSize.y / ViewResolution.designSize.x;
                        containerPopUp.transform.localScale = new Vector3(scale, scale, scale);
                    }
                }
                else
                {
                    var viewSize = ViewResolution.referenceResolutionLandscape;
                    if (viewSize.x < ViewResolution.designSize.x)
                    {
                        var scale = viewSize.x / ViewResolution.designSize.x;
                        containerPopUp.transform.localScale = new Vector3(scale, scale, scale);
                    }
                }
            }


            if (grayMask.activeSelf == false)
            {
                AudioUtil.Instance.PauseMusic();
                grayMask.SetActive(true);
            }
        }

        public async void ClosePopUp<T>(T popUp) where T : MachinePopUp
        {
            if (popUpStack.Contains(popUp))
            {
                popUpStack.Remove(popUp);

                await popUp.OnClose();
                if (popUp.container != null)
                {
                    GameObject.Destroy(popUp.container.gameObject);
                }
                GameObject.Destroy(popUp.transform.gameObject);
                if (popUpStack.Count > 0)
                {
                    grayMask.transform.SetAsLastSibling();
                    popUpStack[popUpStack.Count - 1].container.SetAsLastSibling();
                    var maskImage = grayMask.GetComponent<Image>();
                    var color = maskImage.color;
                    color.a = popUpStack[popUpStack.Count - 1].GetPopUpMaskAlpha();
                    maskImage.color = color;
                    
                    if (popUp.IsCloseMustUnpauseMusic())
                    {
                        AudioUtil.Instance.UnPauseMusic();
                    }
                }
                else
                {
                    grayMask.SetActive(false);
                    AudioUtil.Instance.UnPauseMusic();
                }

                popUp.DoCloseAction();
            }
        }

        public async void CloseAllPopUp()
        {
            if (popUpStack != null && popUpStack.Count > 0)
            {
                for (var i = popUpStack.Count - 1; i >= 0; i--)
                {
                    var dialog = popUpStack[i];
                    await dialog.OnClose();
                    if (dialog.container)
                    {
                        GameObject.Destroy(dialog.container.gameObject);
                    }
                    GameObject.Destroy(dialog.transform.gameObject);
                    popUpStack.RemoveAt(i);
                }

                AudioUtil.Instance.UnPauseMusic();
            }

            grayMask?.SetActive(false);
        }

        public void SetGrayMaskState(bool isActive)
        {
            if (grayMask)
            {
                grayMask.SetActive(isActive);

            }
        }

        public int GetDialogCount()
        {
            if (popUpStack != null)
                return popUpStack.Count;
            return 0;
        }

        public void SetMaskGray(int grayValue)
        {
            var image = grayMask.GetComponent<Image>();
            if (image)
            {
                image.color = new Color(0, 0, 0, grayValue / 255f);
            }
        }
    }
}
