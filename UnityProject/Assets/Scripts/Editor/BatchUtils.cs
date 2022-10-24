/*
 * Custom template by Gabriele P.
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
//using Valve.VR.InteractionSystem;
using Debug = UnityEngine.Debug;

public class BatchUtils : MonoBehaviour
{
    private const int LAUNCHER_SCENE_ID = 0;

    #region Build

    [MenuItem("VR@PoliTo/Build All Scenarios Split")]
    public static void BuildGame()
    {
        var options = new BuildPlayerOptions();
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.CompressWithLz4HC;

        for (int i = 1; i < EditorBuildSettings.scenes.Length; i++)
        {
            Array.ForEach(EditorBuildSettings.scenes, scene => scene.enabled = false);
            //EditorBuildSettings.scenes.ForEach(scene => scene.enabled = false);
            EditorBuildSettings.scenes[i].enabled = true;
            
            options.scenes = new[] {EditorBuildSettings.scenes[i].path};
            options.locationPathName = Path.Combine("Builds", PlayerSettings.productName,
                Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path),
                Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path) + ".exe");
            
            // Build player.
            var summary = BuildPipeline.BuildPlayer(options).summary;


            switch (summary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log(
                        $"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path)} succeeded: {summary.totalSize / (Mathf.Pow(2, 20))} MB");
                    break;
                case BuildResult.Failed:
                case BuildResult.Unknown:
                case BuildResult.Cancelled:
                    Debug.Log(
                        $"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path)} - {summary.result}");
                    break;
            }
        }

        Debug.Log("[Build Finished]: ALL SCENARIOS!");
    }


    [MenuItem("VR@PoliTo/Build Launcher (built-in scenarios)")]
    public static void BuildLauncher()
    {
        var sceneToBuild = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettings.scenes[i].enabled = true;
            sceneToBuild.Add(EditorBuildSettings.scenes[i].path);
        }
        // Build Launcher.

        var options = new BuildPlayerOptions();
        options.scenes = sceneToBuild.ToArray();
        options.locationPathName = Path.Combine("Builds", PlayerSettings.productName, "Launcher", "Launcher.exe");
        options.target = BuildTarget.StandaloneWindows64;
        options.options = BuildOptions.CompressWithLz4HC;

        var summary = BuildPipeline.BuildPlayer(options).summary;


        switch (summary.result)
        {
            case BuildResult.Succeeded:
                Debug.Log(
                    $"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[LAUNCHER_SCENE_ID].path)} succeeded: {summary.totalSize / (Mathf.Pow(2, 20))} MB");
                break;
            case BuildResult.Failed:
            case BuildResult.Unknown:
            case BuildResult.Cancelled:
                Debug.Log(
                    $"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[LAUNCHER_SCENE_ID].path)} - {summary.result}");
                break;
        }

        Debug.Log("[Build Finished]: Launcher (with all scenarios inside)!");
    }

    #endregion
}