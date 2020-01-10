using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    #region Monobehaviour

    void Start()
    {
        var selectedScenario = Configuration.GetInt("Scenario", 0);
        if (selectedScenario < 0 || selectedScenario > SceneManager.sceneCountInBuildSettings - 2)
        {
            Debug.LogError("[OutOfRange] invalid Scenario id");
            Application.Quit(100);
        }
        else{
            SceneManager.LoadScene(selectedScenario + 1, LoadSceneMode.Single);
        }
    }

    #endregion
}