/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.XR;


namespace PrattiToolkit
{
    public class OpenVR_GazeRaycaster : MonoBehaviour
    {
        #region Events

        public event Action<RaycastHit>
            OnRaycasthit; // This event is called every frame that the user's gaze is over a collider.

        #endregion

        #region Editor Visible

        [SerializeField] private LayerMask _checkingLayers = 0; // Layers to include in the raycast.


        //[SerializeField] private Reticle m_Reticle;                     // The reticle, if applicable.

        //[SerializeField] private VRInput m_VrInput;                     // Used to call input based events on the current VRInteractiveItem.
        [SerializeField] private bool m_ShowDebugRay; // Optionally show the debug ray.
        [SerializeField] private float m_DebugRayLength = 5f; // Debug ray length.
        [SerializeField] private float m_DebugRayDuration = 1f; // How long the Debug ray will remain visible.
        [SerializeField] private float m_RayLength = 500f; // How far into the scene the ray is cast.

        #endregion

        #region Private Members and Constants

        private Transform _hmdTrackedObject = null;
        private Transform m_Camera;

        private VRInteractiveItem m_CurrentInteractible; //The current interactive item
        private VRInteractiveItem m_LastInteractible; //The last interactive item

        #endregion

        #region Properties

        // Utility for other classes to get the current interactive item
        public VRInteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        //private void OnEnable()
        //{
        //    m_VrInput.OnClick += HandleClick;
        //    m_VrInput.OnDoubleClick += HandleDoubleClick;
        //    m_VrInput.OnUp += HandleUp;
        //    m_VrInput.OnDown += HandleDown;
        //}


        //private void OnDisable()
        //{
        //    m_VrInput.OnClick -= HandleClick;
        //    m_VrInput.OnDoubleClick -= HandleDoubleClick;
        //    m_VrInput.OnUp -= HandleUp;
        //    m_VrInput.OnDown -= HandleDown;
        //}

        #endregion

        #region MonoBehaviour

        //void Start()
        //{
        //    // dovrebbe essere per inizializzare i tracker dell'HMD valve e trovare la camera: provo a usare direttametne la main camera 
        //    /* if (_hmdTrackedObject == null)
        //     {
        //         SteamVR_TrackedObject[] trackedObjects = FindObjectsOfType<SteamVR_TrackedObject>();
        //         foreach (SteamVR_TrackedObject tracked in trackedObjects)
        //         {
        //             if (tracked.index == SteamVR_TrackedObject.EIndex.Hmd)
        //             {
        //                 _hmdTrackedObject = m_Camera = tracked.transform;
        //                 break;
        //             }
        //         }
        //     }*/
        //    _hmdTrackedObject = FindObjectOfType<Camera>().transform;
        //}
        void Start()
        {
            //_hmdTrackedObject = m_Camera = 
            var goXR = GetComponentInParent<XROrigin>();
            if (goXR != null)
            {
                _hmdTrackedObject = m_Camera = goXR.GetComponentInChildren<Camera>().transform;
            }
        }


        void Update()
        {
            EyeRaycast();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Helper Methods

        private void EyeRaycast()
        {
            // Show the debug ray if required
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
            }

            // Create a ray that points forwards from the camera.
            //Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            //RaycastHit hit;

            bool found = false;
            // Do the raycast forweards to see if we hit an interactive item
            foreach (var hit in Physics.RaycastAll(m_Camera.position, m_Camera.forward, m_RayLength, _checkingLayers))
            {
                VRInteractiveItem
                    interactible =
                        hit.collider
                            .GetComponent<VRInteractiveItem>(); //attempt to get the VRInteractiveItem on the hit object
                if (interactible != null)
                {
                    m_CurrentInteractible = interactible;

                    // If we hit an interactive item and it's not the same as the last interactive item, then call Over
                    if (interactible && interactible != m_LastInteractible)
                        interactible.Over();

                    // Deactive the last interactive item 
                    if (interactible != m_LastInteractible)
                        DeactiveLastInteractible();

                    m_LastInteractible = interactible;

                    // Something was hit, set at the hit position.
                    //if (m_Reticle)
                    //    m_Reticle.SetPosition(hit);

                    OnRaycasthit?.Invoke(hit);
                    found = true;
                }
            }

            if (!found)
            {
                // Nothing was hit, deactive the last interactive item.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

                // Position the reticle at default distance.
                //if (m_Reticle)
                //    m_Reticle.SetPosition();
            }
        }


        private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Up();
        }


        private void HandleDown()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Down();
        }


        private void HandleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.DoubleClick();
        }

        #endregion

        #region Events Callbacks

        #endregion

        #region Coroutines

        #endregion
    }
}