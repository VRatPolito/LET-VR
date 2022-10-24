using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Microsoft.Win32;
using PrattiToolkit;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace PrattiToolkit
{
    [Serializable]
    public struct ModelPathBinding
    {
        public string ControllerName;
        public string ModelName;
        public bool LeftRightSameModel;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
    }

    [CreateAssetMenu(fileName = "ControllerModelsBindings", menuName = "Prattitoolkit/OpenXR/ControllerModelsBinding",
        order = 1)]
    public class ControllersModelsLoaderOpenXR_SO : ScriptableObject
    {
        public bool _loadSteamVRPathFromConfigFile = false;

        private const string STEAM_PATH = "C:\\Program Files (x86)\\Steam";
        private const string MODELS_PATH = "steamapps\\common\\SteamVR\\resources\\rendermodels";
        public string STEAMVR_PATH => GetSteamVRPath();

        #region SPECIAL Path

        private const string VIVE_TRACKER_PATH = "steamapps\\common\\SteamVR\\drivers\\htc\\resources\\rendermodels\\vr_tracker_vive_1_0";

        #endregion
        
        public ModelPathBinding[] modelPathBindings;

        private string GetSteamVRPath()
        {
            if (_loadSteamVRPathFromConfigFile)
                return ConfigurationLookUp.Instance.GetString("STEAMVR_PATH", Path.Combine(STEAM_PATH, MODELS_PATH));
            else
            {
#if UNITY_STANDALONE_WIN || PLATFORM_STANDALONE_WIN
                return Path.Combine(Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam",
                    "InstallPath",
                    STEAM_PATH) as string ?? STEAM_PATH, MODELS_PATH);
#else
            return Path.Combine(STEAM_PATH, MODELS_PATH);
#endif
            }
        }

        public ControllersModelsItem GetModelPath(UnityEngine.XR.InputDevice device)
        {
            return GetModelPath(device.name);
        }

        public ControllersModelsItem GetModelPath(string deviceName)
        {
            var binding = modelPathBindings.FirstOrDefault(pathBinding =>
                deviceName.ToLowerInvariant().Contains(pathBinding.ControllerName.ToLowerInvariant()));
            ControllersModelsItem modelItem = default;

            if (string.IsNullOrEmpty(binding.ControllerName))
            {
                binding = modelPathBindings[0];
                Debug.LogError(
                    $"[VR@Polito|PrattiToolkit] Controllers Models Loader - Cannot find binding for '{deviceName}', using default('{binding.ControllerName}')");
            }
            
            modelItem.PositionOffset = binding.PositionOffset;
            modelItem.RotationOffset = binding.RotationOffset;
            if (binding.LeftRightSameModel)
            {
                modelItem.PathToLeft = modelItem.PathToRight =
                    Path.Combine(STEAMVR_PATH, $"{binding.ModelName}\\{binding.ModelName}.obj");
            }
            else
            {
                modelItem.PathToLeft =
                    Path.Combine(STEAMVR_PATH, $"{binding.ModelName}_left\\{binding.ModelName}_left.obj");
                modelItem.PathToRight =
                    Path.Combine(STEAMVR_PATH, $"{binding.ModelName}_right\\{binding.ModelName}_right.obj");
            }
            
            if (binding.ControllerName.Contains("HTC Vive Tracker"))
            {
                modelItem.PathToLeft = modelItem.PathToRight = Path.Combine(STEAM_PATH, $"{VIVE_TRACKER_PATH}\\{binding.ModelName}.obj");
            }

            return modelItem;
        }
    }

    public struct ControllersModelsItem
    {
        public string PathToLeft;
        public string PathToRight;
        public Vector3 PositionOffset;
        public Vector3 RotationOffset;
    }
}