#region Namespaces
    using UnityEngine;
#endregion

namespace Utility.Tools
{
    /// <summary> Guarantees a class only has one instance </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        #region Variables
            static T _instance; // The instance
            bool _initialized = false;  // Is the instance initialized yet?
        #endregion
        #region Getter
            public static T Instance
            {
                get
                {
                    // If instance exists, return it
                    if (_instance != null) return _instance;

                    // If not, create a new instance
                    var instance = new GameObject($"{typeof(T).Name} [Singleton]").AddComponent<T>();
                    _instance = instance.GetComponent<T>();

                    // Initialize if not done so yet
                    if (_instance._initialized) return _instance;
                    _instance.Initialize();
                    _instance._initialized = true;

                    return _instance;
                }
            }
        #endregion
        #region Methods
            protected virtual void Awake()
            {
                if (_initialized == false)
                {
                    Initialize();
                    _initialized = true;
                }
            }
            void Initialize()
            {
                if (_instance == null) _instance = this as T;
                else if (_instance != this)
                {
                    #if UNITY_EDITOR
                    Debug.LogWarning($"{GetType().Name} Singleton already exists. Destroying.");
                    #endif
                    Destroy(this);
                    return;
                }

                DontDestroyOnLoad(this);
                _initialized = true;
            }
        #endregion
    }
}

#region Credits
/// Script created by Tyler Nichols
#endregion