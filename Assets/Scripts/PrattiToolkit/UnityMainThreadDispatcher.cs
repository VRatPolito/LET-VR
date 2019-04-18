
/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrattiToolkit
{
    /// <summary>
    /// Thread-safe Main thread dispatcher. 
    /// Allow to process multithreaded operation from Unity main thread (e.g. network code)
    /// </summary>
    public class UnityMainThreadDispatcher : UnitySingleton<UnityMainThreadDispatcher>
    {
       
        #region UnitySingleton
 /*
        private static bool shuttingDown = false;
        private static UnityMainThreadDispatcher _instance;

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (shuttingDown)
                    return null;
                if (_instance == null)
                {
                    if (!Application.isPlaying)
                        return null;
                    var objects = FindObjectsOfType<UnityMainThreadDispatcher>();
                    if (objects.Length == 0)
                    {
                        _instance = AddDispatcherToScene();
                    }
                    else if (objects.Length >= 1)
                    {
                        _instance = objects[0];
                    }
                }

                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(this);
                return;
            }

#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += sceneLoadedHandler;
#endif

            gameObject.hideFlags = HideFlags.HideInHierarchy;
            DontDestroyOnLoad(gameObject);
            StopAllCoroutines();
        }

#if UNITY_5_4_OR_NEWER
        private void sceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            if(gameObject != null && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
            }
        }
#else
        private void OnLevelWasLoaded(int value)
        {
            StopAllCoroutines();
            StartCoroutine(lateAwake());
        }
#endif

        private void OnApplicationQuit()
        {
            shuttingDown = true;
        }

        private static UnityMainThreadDispatcher AddDispatcherToScene()
        {
            Debug.Log("UMTD initialized...");
            var dispatcherContainer = new GameObject("UnityMainThreadDispatcher");

            DontDestroyOnLoad(dispatcherContainer);

            return dispatcherContainer.AddComponent<UnityMainThreadDispatcher>();

        }
 */
        #endregion


        #region Private Members and Constants

        private readonly Queue<Action> _executionQueue = new Queue<Action>();

        #endregion

        #region MonoBehaviour

        private void Update()
        {

            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue().Invoke();
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Schedule action to be executed as soon as possible
        /// </summary>
        /// <param name="action"></param>
        public void Enqueue(Action action)
        {

            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }

        }

        #endregion

    }
}