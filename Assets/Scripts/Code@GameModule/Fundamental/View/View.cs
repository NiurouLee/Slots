// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/20/12:14
// Ver : 1.0.0
// Description : View.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tool;
using UnityEngine;

namespace GameModule
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class AssetAddressAttribute : Attribute
    {
        public string assetAddress = string.Empty;
        public string portraitAssetAddress = string.Empty;
        public string assetAddressPad = string.Empty;
        public string portraitAssetAddressPad = string.Empty;

        public AssetAddressAttribute(string address, string portraitAddress = "", string assetPad = "",  string portraitAddressPad = "")
        {
            assetAddress = address;
            portraitAssetAddress = portraitAddress;
            assetAddressPad = assetPad;
            portraitAssetAddressPad = portraitAddressPad;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeUpdateAddressAttribute : Attribute
    {
        public virtual void Update(AssetAddressAttribute addressAttribute)
        {
            
        }
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class AlbumRuntimeUpdateAddressAttribute : RuntimeUpdateAddressAttribute
    {
        public override void Update(AssetAddressAttribute addressAttribute)
        {
           var albumController = Client.Get<AlbumController>();
           var seasonId = albumController.GetSeasonId();

           if (!string.IsNullOrEmpty(addressAttribute.assetAddress) && seasonId > 0)
           {
               addressAttribute.assetAddress = addressAttribute.assetAddress.Replace("SeasonX", $"Season{seasonId}");
           }
           
           if (!string.IsNullOrEmpty(addressAttribute.portraitAssetAddress) && seasonId > 0)
           {
               addressAttribute.portraitAssetAddress = addressAttribute.portraitAssetAddress.Replace("SeasonX", $"Season{seasonId}");
           }
           
           if (!string.IsNullOrEmpty(addressAttribute.assetAddressPad) && seasonId > 0)
           {
               addressAttribute.assetAddressPad = addressAttribute.assetAddressPad.Replace("SeasonX", $"Season{seasonId}");
           }
           
           if (!string.IsNullOrEmpty(addressAttribute.portraitAssetAddressPad) && seasonId > 0)
           {
               addressAttribute.portraitAssetAddressPad = addressAttribute.portraitAssetAddressPad.Replace("SeasonX", $"Season{seasonId}");
           }
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ActivityUpdateAddressAttribute : RuntimeUpdateAddressAttribute
    {
        public string activityType;

        public ActivityUpdateAddressAttribute(string inActivityType)
        {
            activityType = inActivityType;
        }

        public override void Update(AssetAddressAttribute addressAttribute)
        {
            var activity = Client.Get<ActivityController>().GetDefaultActivity(activityType);
            if (activity != null)
            {
                activity.UpdateAssetThemeName(addressAttribute);
            }
        }
    }
    
    public class View
    {
        public Transform transform;

        public RectTransform rectTransform;

        protected Transform viewParent;

        protected GameObject gameObject;

        protected string viewAssetAddress;

        protected AssetReference assetReference;

        protected List<View> children;

        protected View parentView;
        
        public View()
        {
            viewAssetAddress = "";
        }

        public View(string assetAddress)
        {
            viewAssetAddress = assetAddress;
        }
        
        public string GetAssetAddressName()
        {
            return viewAssetAddress;
        }

        public View GetParentView()
        {
            return parentView;
        }
        
        public static string TryGetAssetAddressFromAttribute(Type type)
        {
            string address = null;
            
            var assetAddressAttributes = type.GetCustomAttributes(typeof(AssetAddressAttribute), false);

            if (assetAddressAttributes.Length > 0)
            {
                var assetAddressAttribute = assetAddressAttributes[0] as AssetAddressAttribute;
                
                if (assetAddressAttribute != null)
                {
                    var customAttributes = type.GetCustomAttributes(false);
                    
                    if (customAttributes.Length > 0)
                    {
                        for (var i = 0; i < customAttributes.Length; i++)
                        {
                            if (customAttributes[i] is RuntimeUpdateAddressAttribute)
                            {
                                ((RuntimeUpdateAddressAttribute) customAttributes[i]).Update(assetAddressAttribute);
                                break;
                            }
                        }
                    }
                    
                    address = assetAddressAttribute.assetAddress;
                        
                    if (ViewManager.Instance.IsPortrait)
                    {
                        if (!string.IsNullOrEmpty(assetAddressAttribute.portraitAssetAddress))
                            address = assetAddressAttribute.portraitAssetAddress;

                        if (!string.IsNullOrEmpty(assetAddressAttribute.portraitAssetAddressPad)
                            && ViewResolution.designSize.x > ViewResolution.referenceResolutionLandscape.x)
                        {
                            address = assetAddressAttribute.portraitAssetAddressPad;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(assetAddressAttribute.assetAddressPad)
                            && ViewResolution.designSize.x > ViewResolution.referenceResolutionLandscape.x)
                        {
                            address = assetAddressAttribute.assetAddressPad;
                        }
                    }
                }
            }

            return address;
        }

        public static async Task<TView> CreateView<TView>() where TView : View
        {
            var address = TryGetAssetAddressFromAttribute(typeof(TView));
            if (!string.IsNullOrEmpty(address))
            {
                return await CreateView<TView>(address, null);
            }

            return null;
        }
        
        public static TView CreateView<TView>(Transform transform, object extraData = null)
            where TView : View
        {
            var view = Activator.CreateInstance<TView>();

            view?.SetUpView(transform);
            view?.SetUpController(extraData);
            view?.BindingComponent();
            view?.OnViewSetUpped();
            view?.EnableView();
            return view;
        }

        public static async Task<TView> CreateView<TView>(string address, Transform parent, object extraData = null)
            where TView : View
        {
            object extraAsyncData = null;

            if (ViewAsyncDataProvider.NeedAsyncData(typeof(TView)))
            {
                extraAsyncData = await ViewAsyncDataProvider.GetAsyncData(typeof(TView));

                //如果异步数据获取失败就不创建View
                if (extraAsyncData == null)
                {
                    return null;
                }
            }
            
            var view = Activator.CreateInstance(typeof(TView), address) as TView;

            if (view != null)
            {
                await view.LoadView(parent);
                view.SetUpController(extraData, extraAsyncData);
                view.BindingComponent();
                view.OnViewSetUpped();
                view.EnableView();
            }

            return view;
        }
        
        public static async Task<View> CreateView(Type type, string address, Transform parent, object extraData = null)
        {
            try
            {
                object extraAsyncData = null;

                //检查当前View创建显示，是否需要从服务器拉最新的数据，如果拉去不到数据就直接返回NULL，不再显示View
                
                if (ViewAsyncDataProvider.NeedAsyncData(type))
                {
                    extraAsyncData = await ViewAsyncDataProvider.GetAsyncData(type);

                    //如果异步数据获取失败就不创建View
                    if (extraAsyncData == null)
                    {
                        return null;
                    }
                }
                
                var view = Activator.CreateInstance(type, address) as View;

                if (view != null)
                {
                    await view.LoadView(parent);
                    view.SetUpController(extraData, extraAsyncData);
                    view.BindingComponent();
                    view.OnViewSetUpped();
                    view.EnableView();
                }
                return view;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        protected async Task<View> LoadView(Transform inParent)
        {
            viewParent = inParent;

            var loadingTask = new TaskCompletionSource<bool>();
            
            DefaultPauseAndCancelContext.Instance.AddWaitTask(loadingTask, null);

            var startTime = Time.realtimeSinceStartup;
            XDebug.LogOnExceptionHandler($"Load {viewAssetAddress} Start:" + startTime);
            assetReference = AssetHelper.PrepareAsset<GameObject>(viewAssetAddress, inAssetReference =>
            {
                XDebug.LogOnExceptionHandler($"Load {viewAssetAddress} Finish:" + (Time.realtimeSinceStartup - startTime));
                GameObject view = inAssetReference.InstantiateAsset<GameObject>();

                SetUpView(view.transform);
                view.gameObject.SetActive(false);
                DefaultPauseAndCancelContext.Instance.RemoveTask(loadingTask);
                
                loadingTask.SetResult(true);
            });

            await loadingTask.Task;

            return this;
        }

        public void SetUpView(Transform viewTransform)
        {
            transform = viewTransform;

            rectTransform = transform as RectTransform;

            gameObject = transform.gameObject;

            if (viewParent && transform)
                transform.SetParent(viewParent, false);
        }

        protected virtual void OnViewSetUpped()
        {
            SetUpExtraView();
        }

        protected virtual void SetUpExtraView()
        {
            
        }

        protected virtual void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
        }

        protected virtual void BindingComponent()
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        /// <summary>
        /// Add Child View
        /// </summary>
        /// <param name="address">资源地址</param>
        /// <typeparam name="TView">Vie类型</typeparam>
        public async Task<TView> AddChild<TView>(string address) where TView : View
        {
            var controller = await CreateView<TView>(address, transform);
            return AddChild(controller);
        }
        
        /// <summary>
        /// Add Child View
        /// </summary>
        /// <param name="address">资源地址</param>
        /// <typeparam name="TView">Vie类型</typeparam>
        public async Task<TView> AddChild<TView>() where TView : View
        {
            var address = TryGetAssetAddressFromAttribute(typeof(TView));
            if (!string.IsNullOrEmpty(address))
            {
                var controller = await CreateView<TView>(address, transform);
                return AddChild(controller);
            }

            return null;
        }

        /// <summary>
        /// 添加Child View
        /// </summary>
        /// <param name="childTransform"></param>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public TView AddChild<TView>(Transform childTransform) where TView : View
        {
            if (children == null)
            {
                children = new List<View>();
            }

            var view = CreateView<TView>(childTransform, null);

            AddChild<TView>(view);
            return view;
        }

        /// <summary>
        /// 添加 Child View
        /// </summary>
        /// <param name="view"></param>
        /// <typeparam name="TView"></typeparam>
        /// <returns></returns>
        public TView AddChild<TView>(TView view) where TView : View
        {
            //XDebug.Log("AddChildView" + view);
            if (children == null)
            {
                children = new List<View>();
            }

            children.Add(view);

            view.SetParentView(this);

            return view;
        }

        public void SetParentView(View view)
        {
            parentView = view;
        }

        public virtual void Destroy()
        {
            if (children != null)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    children[i].Destroy();
                }
            }

            if (gameObject)
            {
                GameObject.Destroy(gameObject);
            }

            assetReference?.ReleaseOperation();
        }
        
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        public bool IsActive()
        {
            return gameObject.activeInHierarchy;
        }
        
        protected virtual void EnableView()
        {
            gameObject.SetActive(true);
            
            if (children != null && children.Count > 0)
            {
                for (var i = 0; i < children.Count; i++)
                {
                    children[i].EnableView();
                }
            }
        }

        public int GetChildViewCount()
        {
            if (children != null)
                return children.Count;
            return 0;
        }

        public void RemoveChild(View view)
        {
            if (children != null)
            {
                if(children.Contains(view))
                {
                    children.Remove(view);
                    view.Destroy();
                }
            }
        }
        public View GetChildView(int i)
        {
            if (children.Count > i)
                return children[i];
            return null;
        }

        public TView GetChildView<TView>() where TView : View
        {
            if (children != null && children.Count > 0)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (children[i] is TView)
                    {
                        return children[i] as TView;
                    }
                }
            }
            return null;
        }

        public void SetTransformActive(Transform transform, bool visible)
        {
            if (transform)
            {
                transform.gameObject.SetActive(visible);
            }
        }
    }

    public class View<T> : View where T : ViewController
    {
        public T viewController;

        public View()
        {
            
        }
        
        public View(string address)
            : base(address)
        {
        }

        protected override void SetUpController(object inExtraData, object inExtraAsyncData = null)
        {
            viewController = Activator.CreateInstance<T>();
            
            viewController.BindingView(this, inExtraData, inExtraAsyncData);
        }

        protected override void BindingComponent()
        {
            if (viewController != null && viewController.IsBindingUIComponentToController())
            {
                ComponentBinder.BindingComponent(viewController, transform);
            }
            else
            {
                ComponentBinder.BindingComponent(this, transform);
            }
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            viewController.OnViewDidLoad();
        }

        public override void Destroy()
        {
            base.Destroy();
            viewController.OnViewDestroy();
        }

        public override void Hide()
        {
            base.Hide();
            viewController?.OnViewHide();
        }

        public override void Show()
        {
            base.Show();
            viewController?.OnViewShow();
        }

       
        protected override void EnableView()
        {
            base.EnableView();
            if(transform.gameObject.activeInHierarchy)
                viewController?.OnViewEnabled();
        }
    }
}