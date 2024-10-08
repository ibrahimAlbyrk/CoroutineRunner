```csharp
namespace Core.Runtime.Coroutines
{
    public class MonoBehaviourHook : MonoBehaviour {}
    
    public static class CoroutineRunner
    {
        public enum IDs
        {
            Global = 0
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Reset()
        {
            _hooks = new Dictionary<int, MonoBehaviourHook>();
        }
        
        private static GameObject _parent;
        
        private static Dictionary<int, MonoBehaviourHook> _hooks = new();

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

        public static void Stop(ref Coroutine coroutine, IDs categoryID)
        {
            Stop(ref coroutine, (int)categoryID);
        }
        
        public static void Stop(ref Coroutine coroutine, int categoryID = 0)
        {
            if (coroutine == null) return;

            if (_hooks.TryGetValue(categoryID, out var hook))
            {
                hook.StopCoroutine(coroutine);
                coroutine = null;
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
```
