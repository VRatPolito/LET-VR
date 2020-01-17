/*
* Custom template by F. Gabriele Pratticò {filippogabriele.prattico@polito.it}
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PrattiToolkit
{
    public class ConfigurationLookUp : UnitySingleton<ConfigurationLookUp>
    {
        protected ConfigurationLookUp()
        {
        }

        #region Events

        #endregion

        #region Editor Visible

        #endregion

        #region Private Members and Constants

        private const string NAME = "config.txt";

        private IDictionary<string, string> _config;

        private FileInfo _fileInfo;
        #endregion

        #region Properties

        #endregion

        #region MonoBehaviour

        void Awake()
        {
            _fileInfo = new FileInfo(Path.Combine(Path.Combine(Application.dataPath, "BuildData"), NAME));

            try
            {
                _config = new Dictionary<string, string>();
                Load(_fileInfo);
            }
            catch (Exception e)
            {
#if UNITY_WSA
            Debug.Log(e);
#else
                Debug.LogException(e);
#endif
            }
        }


        #endregion

        #region Public Methods

        #endregion

        #region Helper Methods

        #region PerFrame

        #endregion

        public T GetEnum<T>(string path, T deflt = default(T))
        {
            if (_config.ContainsKey(path))
                return (T)Enum.Parse(typeof(T), _config[path], true);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public string GetString(string path)
        {
            return GetString(path, string.Empty);
        }

        public string GetString(string path, string deflt)
        {
            if (_config.ContainsKey(path))
                return _config[path];
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public int GetInt(string path, int deflt = 0)
        {
            if (_config.ContainsKey(path))
                return int.Parse(_config[path], System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public float GetFloat(string path, float deflt = 0f)
        {
            if (_config.ContainsKey(path))
                return float.Parse(_config[path], System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public bool GetBool(string path, bool deflt = false)
        {
            if (_config.ContainsKey(path))
                return bool.Parse(_config[path]);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public void Put(string path, object value)
        {
            _config[path] = value.ToString();
        }

        public bool Exists(string path)
        {
            return _config.ContainsKey(path);
        }

        public IEnumerable<string> Keys
        {
            get { return _config.Keys; }
        }

        protected void Load(FileInfo file)
        {
            using (var f = new StreamReader(file.OpenRead()))
            {
                string line;
                while ((line = f.ReadLine()) != null)
                {
                    if (line.Length > 0 && !line.StartsWith("#"))
                    {
                        var split = line.Split('=');
                        _config[split[0].Trim()] = split[1].Trim();
                    }
                }
            }
        }

        #endregion

        #region Events Callbacks

        #endregion

        #region Coroutines

        #endregion
    }
}