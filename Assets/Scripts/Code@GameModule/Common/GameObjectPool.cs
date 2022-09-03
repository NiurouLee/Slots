using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule{

    public class GameObjectPool : IDisposable
    {
        private GameObject _prefab;
        private string _name;
        private Transform _root;
        private bool _disposed;
        private List<GameObject> _actives = new List<GameObject>();
        private List<GameObject> _inactives = new List<GameObject>();
        private static readonly object _locker = new object();

        public GameObjectPool(GameObject prefab, Transform parent = null)
        {
            if (prefab == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Prefab can not be null when creating a gameobject pool.");
#endif
                _disposed = true;
                return;
            }
            _prefab = prefab;
            _name = prefab.name;

            _root = new GameObject(_name + "Pool").transform;
            _root.SetParent(parent);
            _root.localScale = Vector3.one;
            _disposed = false;
        }

        private bool CheckDisposed()
        {
            if (_disposed == true)
            {
#if UNITY_EDITOR
                Debug.LogErrorFormat("Cannot do any operation on a disposed game object pool.");
#endif
                return true;
            }
            else { return false; }
        }

        public GameObject Spawn(Transform parent = null)
        {
            if (CheckDisposed() == true) { return null; }

            lock (_locker)
            {
                GameObject result = null;
                if (_inactives.Count <= 0)
                {
                    result = GameObject.Instantiate(_prefab, parent);
                    result.name = _prefab.name;
                }
                else
                {
                    result = _inactives[0];
                    _inactives.RemoveAt(0);
                }
                if (_actives.Contains(result) == false) { _actives.Add(result); }
                result.transform.SetParent(parent);
                return result;
            }
        }

        public void Despawn(GameObject target)
        {
            if (CheckDisposed() == true) { return; }

            lock (_locker)
            {
                if (_actives.Contains(target) == true) { _actives.Remove(target); }

                if (_inactives.Contains(target) == false) { _inactives.Add(target); }

                target.SetActive(false);
                target.transform.SetParent(_root);
            }
        }

        public void DespawnAll()
        {
            if (CheckDisposed() == true) { return; }

            lock (_locker)
            {
                for (int i = _actives.Count - 1; i >= 0; i--) { Despawn(_actives[i]); }
            }
        }

        public void Clear()
        {
            if (CheckDisposed() == true) { return; }

            lock (_locker)
            {
                if (_actives != null && _actives.Count > 0)
                {
                    foreach (var item in _actives) { GameObject.Destroy(item); }
                    _actives.Clear();
                }
                if (_inactives != null && _inactives.Count > 0)
                {
                    foreach (var item in _inactives) { GameObject.Destroy(item); }
                    _inactives.Clear();
                }
            }
        }

        public void Dispose()
        {
            if (CheckDisposed() == true) { return; }
            lock (_locker)
            {
                Clear();
                _prefab = null;
                _name = null;
                _inactives = null;
                GameObject.Destroy(_root.gameObject);
                _root = null;
                _disposed = true;
            }
        }
    }
}