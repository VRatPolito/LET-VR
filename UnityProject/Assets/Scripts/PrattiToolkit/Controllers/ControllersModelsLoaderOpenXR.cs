using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityMeshImporter;
using System;
using System.Net.Http.Headers;
using UnityEngine.Assertions;

namespace PrattiToolkit
{
    public class ControllersModelsLoaderOpenXR : MonoBehaviour
    {
        private GameObject _left, _right;
        private string _deviceName = "";
        [SerializeField] private ControllersModelsLoaderOpenXR_SO _bindings;
        private bool _loaded = false;
        private int _retry = 5;

        // Start is called before the first frame update
        void Start()
        {
            if (!Directory.Exists(_bindings.STEAMVR_PATH))
            {
                Debug.LogWarning("[VR@Polito|PrattiToolkit] SteamVR Not found - Controller models can't be loaded");
                this.enabled = false;
                return;
            }

            //StartCoroutine(Initializer());
        }

        private IEnumerator Initializer()
        {
            var wait = new WaitForEndOfFrame();
            while (!_loaded && _retry > 0 && _deviceName == "")
            {
                CheckDevice();
                yield return new WaitForSeconds(2);
            }

            LoadModels();
        }



        void OnEnable()
        {
            if (_loaded) return;
            CheckDevice();
            if (_deviceName != "") LoadModels();
            InputDevices.deviceConnected += ctx => OnDeviceConnected(ctx);
        }



        private void OnDisable()
        {
            if (_loaded) return;
            InputDevices.deviceConnected -= ctx => OnDeviceConnected(ctx);
        }

        private void OnDeviceConnected(InputDevice device)
        {
            CheckDevice();
            if (_deviceName != "") LoadModels();
        }


        public GameObject RetrieveModel(string deviceName, InputDeviceCharacteristics leftOrRight = InputDeviceCharacteristics.Left)
        {
            var mdlpth = _bindings.GetModelPath(deviceName);
            return MeshImporter.Load(leftOrRight==InputDeviceCharacteristics.Left?mdlpth.PathToLeft:mdlpth.PathToRight);
        }
        
        void LoadModels()
        {
            if (_loaded && _deviceName.Contains("HTC Vive Tracker")) return;
            
            _left = RetrieveModel(_deviceName, InputDeviceCharacteristics.Left);
            _right = RetrieveModel(_deviceName, InputDeviceCharacteristics.Right);
            
            var vric = this.GetComponent<VRItemController>();
            var lc = vric.LeftController?.GetComponent<ActionBasedController>();
            var rc = vric.RightController?.GetComponent<ActionBasedController>();

            lc.model = _left.transform;
            rc.model = _right.transform;
            _left.transform.parent = lc.modelParent;
            _right.transform.parent = rc.modelParent;

            //TODO FIX POSROT WITH DATA FROM SO
            _left.transform.localPosition = Vector3.zero;
            _left.transform.localRotation = Quaternion.Euler(0, 180, 0);
            _right.transform.localPosition = Vector3.zero;
            _right.transform.localRotation = Quaternion.Euler(0, 180, 0);

            _loaded = true;

        }

        void CheckDevice()
        {
            var handDevices = new List<UnityEngine.XR.InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, handDevices);


            if (handDevices.Count == 0) return;
            else if (handDevices.Count >= 1)
            {
                UnityEngine.XR.InputDevice device = handDevices[0];
                Debug.Log(string.Format("[VR@Polito|PrattiToolkit]  Device name '{0}' with role '{1}'", device.name,
                    device.role.ToString()));
                _deviceName = device.name;
                return;
            }

            _deviceName = "";
            Debug.Log($"[VR@Polito|PrattiToolkit] Found controllers with name '{_deviceName}'");
        }
    }
}