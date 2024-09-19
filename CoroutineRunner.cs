using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Core.Runtime.Coroutines
{
    public static class CoroutineRunner
    {
        public enum IDs
        {
            Global = 0,
            Attack = 1
        }
        
        private static GameObject _parent;
        
        private static readonly Dictionary<int, MonoBehaviourHook> _hooks = new();

        public static Coroutine Start(IEnumerator enumerator, IDs categoryID = IDs.Global)
        {
            return Start(enumerator, (int)categoryID);
        }
        
        public static Coroutine Start(IEnumerator enumerator, int categoryID = 0)
        {
            Coroutine coroutine;
            
            if (_hooks.TryGetValue(categoryID, out var hook))
            {
                coroutine = hook.StartCoroutine(enumerator);
            }
            else
            {
                hook = CreateHook(categoryID);
                coroutine = hook.StartCoroutine(enumerator);
                
                _hooks[categoryID] = hook;
            }

            return coroutine;
        }

        public static void Stop(Coroutine coroutine, IDs categoryID = IDs.Global)
        {
            Stop(coroutine, (int)categoryID);
        }
        
        public static void Stop(Coroutine coroutine, int categoryID = 0)
        {
            if (coroutine == null) return;

            if (_hooks.TryGetValue(categoryID, out var hook))
            {
                hook.StopCoroutine(coroutine);
            }
        }

        public static void StopAllCategory(IDs categoryID)
        {
            StopAllCategory((int)categoryID);
        }
        
        public static void StopAllCategory(int categoryID)
        {
            if (_hooks.TryGetValue(categoryID, out var hook))
            {
                hook.StopAllCoroutines();
            }
        }
        
        public static void StopAllCoroutines()
        {
            foreach (var hook in _hooks.Values)
            {
                hook.StopAllCoroutines();
            }
        }

        private static MonoBehaviourHook CreateHook(int categoryID)
        {
            InitParent();
            
            var hook = _parent.AddComponent<MonoBehaviourHook>();
            _hooks.Add(categoryID, hook);
            
            return hook;
        }

        private static void InitParent()
        {
            if (_parent == null)
            {
                _parent = new GameObject("Coroutine Runner");
                Object.DontDestroyOnLoad(_parent);
            }
        }
    }
}
