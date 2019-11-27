
/*
 * Custom template by Gabriele P.
 */
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Debug = UnityEngine.Debug;

public class BatchUtils : MonoBehaviour
{
    #region Build

    [MenuItem("VR@PoliTo/Build All Levels Splitted")]
    public static void BuildGame()
    {
      
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            EditorBuildSettings.scenes.ForEach(scene => scene.enabled = false);
            EditorBuildSettings.scenes[i].enabled = true;
            // Build player.
            var report = BuildPipeline.BuildPlayer(new[] { EditorBuildSettings.scenes[i].path },
                Path.Combine("Builds", PlayerSettings.productName, Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path), Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path) + ".exe"),
                BuildTarget.StandaloneWindows64,
                BuildOptions.CompressWithLz4HC);
            var summary = report.summary;

            switch (summary.result)
            {
               
                case BuildResult.Succeeded:
                    Debug.Log($"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path)} succeeded: {summary.totalSize} bytes");
                    break;
                case BuildResult.Failed:
                case BuildResult.Unknown:
                case BuildResult.Cancelled:
                    Debug.Log($"Build {Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path)} - {summary.result}");
                    break;

            }
        }
        Debug.Log("[Build Finished]: ALL LEVELS!");
    }

    #endregion

    
}
