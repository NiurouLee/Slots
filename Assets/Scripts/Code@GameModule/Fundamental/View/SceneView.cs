// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/11:38
// Ver : 1.0.0
// Description : SceneView.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameModule
{
    public interface ISceneView
    {
        bool IsPortraitScene { get; }
        SceneType SceneType { get; }

        void DestroySceneView();

        void AttachToSceneContainer(Transform transform);

        Task PrepareAsyncAssets();
    }
    
    public class SceneView<T> : View<T>, ISceneView where T: ViewController
    {
        public virtual bool IsPortraitScene { get; } = false;
        public virtual SceneType SceneType { get; } = SceneType.TYPE_INVALID;
        
        public bool Is3DScene { get; protected set; } = false;

        public SceneType sceneType;

        public SceneView()
        {
            
        }

        public SceneView(string assetAddress, SceneType inSceneType)
            : base(assetAddress)
        {
            sceneType = inSceneType;
        }

        public void DestroySceneView()
        {
            Destroy();
        }
        
        public void AttachToSceneContainer(Transform sceneContainer)
        {
            transform.SetParent(sceneContainer, false);
        }

        public virtual async Task PrepareAsyncAssets()
        {
            await Task.CompletedTask;
        }
    }
}