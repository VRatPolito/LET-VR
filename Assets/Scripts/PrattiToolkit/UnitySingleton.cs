using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrattiToolkit
{
    /// <summary>
    /// Be aware this will not prevent a non singleton constructor
    ///   such as `T myT = new T();`
    /// To prevent that, add `protected T () {}` to your singleton class.
    /// 
    /// As a note, this is made as MonoBehaviour because we need Coroutines.
    /// </summary>
    public class UnitySingleton<T> : MonoBehaviour where T : UnitySingleton<T>
    {

        private static bool _applicationIsQuitting = false;
        private static T _instance;

        private static object _lock = new object();

        protected UnitySingleton()
        {
        }

        public static T Instance
        {
            get
            {

                if (_applicationIsQuitting)
                {
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        var objects = FindObjectsOfType<T>();
                        if (objects.Length == 0)
                        {
                            GameObject singletonContainer = new GameObject("(UnitySingleton) " + typeof(T).ToString());
                            _instance = singletonContainer.AddComponent<T>();
                            DontDestroyOnLoad(singletonContainer);
                            Debug.Log("[UnitySingleton] An instance of " + typeof(T) +
                                      " is required. '" + singletonContainer + "' was created with DontDestroyOnLoad.");
                        }
                        else if (objects.Length >= 1)
                        {
                            _instance = objects[0];
                        }
                    }

                    return _instance;
                }
            }
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
        
        protected void Quit()
        {
            Application.Quit();
        }

        //     private void Awake () {
        //         lock (_lock) {
        //             if (_instance == null) {
        //                 _instance = this as T;
        //             } else if (_instance != this) {
        //                 Destroy (this);
        //                 return;
        //             }
        //         }

        // #if UNITY_5_4_OR_NEWER
        //         SceneManager.sceneLoaded += sceneLoadedHandler;
        // #endif

        //         gameObject.hideFlags = HideFlags.HideInHierarchy;
        //         DontDestroyOnLoad (gameObject);
        //         StopAllCoroutines ();
        //     }

        // #if UNITY_5_4_OR_NEWER
        //     private void sceneLoadedHandler (Scene scene, LoadSceneMode mode) {
        //         if (gameObject != null && gameObject.activeInHierarchy) {
        //             StopAllCoroutines ();
        //         }
        //     }
        // #else
        //     private void OnLevelWasLoaded (int value) {
        //         StopAllCoroutines ();
        //         StartCoroutine (lateAwake ());
        //     }
        // #endif

    }
}