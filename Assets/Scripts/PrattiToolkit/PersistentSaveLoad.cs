using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;


namespace PrattiToolkit
{

    public static class PersistentSaveLoad
    {
        public enum SerializationType : byte
        {
            Binary,
            Json
        }

        public const string FILENAME = "data.pgd";

        public static void Save<T>(T data, string path, SerializationType serializationType = SerializationType.Binary)
        {
            if (!typeof(T).IsSerializable) return;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream file = File.Create(path))
            {
                switch (serializationType)
                {
                    case SerializationType.Binary:
                        BinaryFormatter bf = new BinaryFormatter();
                        bf.Serialize(file, data);
                        break;
                    case SerializationType.Json:
                        using (var sw = new StreamWriter(file))
                            sw.WriteLineAsync(JsonUtility.ToJson(data));
                        break;
                }

            }
        }

        public static T Load<T>(string path, SerializationType serializationType = SerializationType.Binary)
        {
            T tmp = default(T);
            if (!typeof(T).IsSerializable) return tmp;

            if (File.Exists(path))
            {
                using (FileStream file = File.Open(path, FileMode.Open))
                {
                    switch (serializationType)
                    {
                        case SerializationType.Binary:
                            BinaryFormatter bf = new BinaryFormatter();
                            tmp = (T)bf.Deserialize(file);
                            break;
                        case SerializationType.Json:
                            using (var sr = new StreamReader(file))
                                tmp = JsonUtility.FromJson<T>(sr.ReadToEnd());
                            break;
                    }
                }
            }

            return tmp;
        }


        public static string GetDefaultDataPath(string AppName = "unityAppPersistentData", string fileName = FILENAME)
        {
#if UNITY_STANDALONE_WIN || PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), AppName, fileName);
#else
            return Path.Combine(Application.persistentDataPath,AppName,fileName);
#endif
        }

    }

}