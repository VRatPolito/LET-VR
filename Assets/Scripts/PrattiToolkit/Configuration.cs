using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

namespace PrattiToolkit
{
    public static class Configuration
    {
        private const string NAME = "config.txt";

        private static IDictionary<string, string> _config;

        private static FileInfo _fileInfo =
            new FileInfo(Path.Combine(Path.Combine(Application.dataPath, "BuildData"), NAME));

        static Configuration()
        {
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

        public static T GetEnum<T>(string path, T deflt = default(T))
        {
            if (_config.ContainsKey(path))
                return (T) Enum.Parse(typeof(T), _config[path], true);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static string GetString(string path)
        {
            return GetString(path, string.Empty);
        }

        public static string GetString(string path, string deflt)
        {
            if (_config.ContainsKey(path))
                return _config[path];
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static int GetInt(string path, int deflt = 0)
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

        public static float GetFloat(string path, float deflt = 0f)
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

        public static bool GetBool(string path, bool deflt = false)
        {
            if (_config.ContainsKey(path))
                return bool.Parse(_config[path]);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static void Put(string path, object value)
        {
            _config[path] = value.ToString();
        }

        public static bool Exists(string path)
        {
            return _config.ContainsKey(path);
        }

        public static IEnumerable<string> Keys
        {
            get { return _config.Keys; }
        }

        public static void Load(FileInfo file)
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

    }
}