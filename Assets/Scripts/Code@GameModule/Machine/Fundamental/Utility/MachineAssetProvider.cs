// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-25 12:40 PM
// Ver : 1.0.0
// Description : MachineAssetProvider.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine.U2D;
using UnityEngine;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;

using Object = UnityEngine.Object;

namespace GameModule
{
    public class MachineAssetProvider
    {
        private string _machineId;
        private string _assetsId;
        private Dictionary<string, Object> _nameToAssetsMap;

        private Dictionary<string, LazyInstancePool<GameObject>> _nameToPoolDict;

        private Transform _elementPoolRootTransform;
        public string MachineId => _machineId;
        public string AssetsId => _assetsId;

        protected List<string> assetLabels;
        protected List<AssetReference> assetReferenceList;
        protected int finishCount = 0;

        protected int sceneSwitchId;


        public MachineAssetProvider(string inMachineId, string inAssetId = "")
        {
            _machineId = inMachineId;
            _assetsId = string.IsNullOrEmpty(inAssetId) ? inMachineId : inAssetId;

            assetLabels = new List<string>() {"TopPanel","TopPanelV", "MachineCommon", "Machine" + _assetsId};

            _nameToPoolDict = new Dictionary<string, LazyInstancePool<GameObject>>();
        }

        public void LoadAssetByLabel(string label, Action<AssetReference> onLoadFinish)
        {
            var assetReference = AssetHelper.PrepareAssets<UnityEngine.Object>(label, assetsRef =>
            {
                if (assetsRef != null)
                {
                    var assets = assetsRef.GetReferencedAssets();

                    for (var i = 0; i < assets.Count; i++)
                    {
                        _nameToAssetsMap.Add(assets[i].name, assets[i]);
                    }
                }

                onLoadFinish?.Invoke(assetsRef);
            });

            assetReferenceList.Add(assetReference);
        }

        public void LoadMachineAsset()
        {
            _nameToAssetsMap = new Dictionary<string, Object>();
            assetReferenceList = new List<AssetReference>();
            finishCount = 0;

            sceneSwitchId = ViewManager.SwitchActionId;

            for (var i = 0; i < assetLabels.Count; i++)
            {
                LoadAssetByLabel(assetLabels[i], OnAssetDownloadFinished);
            }
        }

        protected void OnAssetDownloadFinished(AssetReference assetReference)
        {
            finishCount++;

            if (assetReference == null)
            {
                OnDownloadError();
            }
            else if (finishCount == assetLabels.Count)
            {
                EventBus.Dispatch(new EventSceneSwitchMask(SwitchMask.MASK_RESOURCE_READY, sceneSwitchId));
            }
        }

        protected void OnDownloadError()
        {
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMachineDownloadFail, ("gameId", _assetsId));
            
            var popupContentInfo = new PopupContentInfo("Warning",
                "Load assets failed, please check your internet and try again later.", "Okay");

            popupContentInfo.BindAction(() => { EventBus.Dispatch(new EventRequestGameRestart()); });

            var popUp = ViewManager.Instance.ShowHighPriorityView<HighPriorityNoticePopup>("UICommonNoticePanel",
                popupContentInfo);
        }

        public float GetCompleteProgress()
        {
            long totalBytes = 0;
            long downloadBytes = 0;

            for (var i = 0; i < assetReferenceList.Count; i++)
            {
                var handle = assetReferenceList[i].GetOperationHandle();
                if (handle.IsValid())
                {
                    totalBytes += handle.GetDownloadStatus().TotalBytes;
                    downloadBytes += handle.GetDownloadStatus().DownloadedBytes;
                }
            }

            if (totalBytes > 0)
            {
                return (float) downloadBytes / totalBytes * 0.7f;
            }

            return 0;
        }

        public T GetAsset<T>(string assetName) where T : UnityEngine.Object
        {
            if (_nameToAssetsMap.ContainsKey(assetName))
            {
                T asset = _nameToAssetsMap[assetName] as T;
                return asset;
            }

            return null;
        }

        public GameObject InstantiateGameObject(string assetName, bool usePool = false)
        {
            if (_nameToAssetsMap.ContainsKey(assetName))
            {
                if (usePool)
                {
                    if (!_nameToPoolDict.ContainsKey(assetName))
                    {
                        _nameToPoolDict[assetName] =
                            new LazyInstancePool<GameObject>(_nameToAssetsMap[assetName] as GameObject);
                    }

                    return _nameToPoolDict[assetName].Acquire();
                }

                GameObject asset = _nameToAssetsMap[assetName] as GameObject;

                if (asset)
                {
                    return GameObject.Instantiate(asset);
                }
            }

            return null;
        }

        public void RecycleGameObject(string assetName, GameObject gameObject)
        {
            if (_nameToPoolDict.ContainsKey(assetName))
            {
                _nameToPoolDict[assetName].Recycle(gameObject);
                gameObject.transform.SetParent(_elementPoolRootTransform, false);
            }
        }

        public Sprite GetSpriteInAtlas(string spriteName, string atlasName)
        {
            if (_nameToAssetsMap.ContainsKey(atlasName))
            {
                SpriteAtlas atlas = _nameToAssetsMap[atlasName] as SpriteAtlas;
                if (atlas != null)
                {
                    return atlas.GetSprite(spriteName);
                }
            }

            return null;
        }

        public void ReleaseAssets()
        {
            for (var i = 0; i < assetReferenceList.Count; i++)
            {
                assetReferenceList[i].ReleaseOperation();
            }

            assetReferenceList.Clear();

            _nameToPoolDict.Clear();

            if (_elementPoolRootTransform != null)
            {
                GameObject.Destroy(_elementPoolRootTransform.gameObject);
            }
        }

        public Transform GetPoolAttachTransform()
        {
            if (_elementPoolRootTransform == null)
            {
                var elementPoolRoot = new GameObject("ElementPoolRoot");
                _elementPoolRootTransform = elementPoolRoot.transform;
                elementPoolRoot.SetActive(false);
            }

            return _elementPoolRootTransform;
        }

        //===================added by james====================
        //填写一个前缀，自动拼接上机器的id，例如传入Machine,机器id是1001，会返回Machine1001
        //如果该前缀包含{machineId},那么将会直接将该字符串替换为机器id，并返回
        public string GetAssetNameWithPrefix(string prefix)
        {
            if (prefix.IndexOf("{machineId}", StringComparison.Ordinal) >= 0)
            {
                return prefix.Replace("{machineId}", _assetsId);
            }

            var lastDotIndex = prefix.LastIndexOf(".", StringComparison.Ordinal);
            if (lastDotIndex >= 0)
            {
                return prefix.Substring(0, lastDotIndex) + _assetsId + prefix.Substring(lastDotIndex);
            }

            return prefix + _assetsId;
        }
        //===================added by james====================
    }
}