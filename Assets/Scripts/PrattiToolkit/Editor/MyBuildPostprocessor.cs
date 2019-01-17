using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class MyBuildPostprocessor {

    [PostProcessBuildAttribute(1)]
    public static void CopyFilesToBuildFolder(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Copying files at @" + pathToBuiltProject);

        string fileName = String.Empty;
        string dstFile = String.Empty;
        string srcFileFolder = Path.Combine(Application.dataPath ,"BuildData");
        string dstPath = Path.Combine(Path.Combine(Path.GetDirectoryName(pathToBuiltProject),Path.GetFileNameWithoutExtension(pathToBuiltProject)+"_Data"),"BuildData");

        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            Directory.CreateDirectory(dstPath);
            string[] filesToCopy = Directory.GetFiles(srcFileFolder);
            foreach (string file in filesToCopy)
            {
                if (Path.GetExtension(file) == ".meta")
                    continue;
                fileName = Path.GetFileName(file);
                dstFile = Path.Combine(dstPath, fileName);
                File.Copy(file, dstFile, true);
            }
        }

        Debug.Log("Files copied");

    }
}
