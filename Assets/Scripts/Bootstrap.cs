using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    #region Private variables and const

    private int _selectedScenario = 0;

    #endregion

    #region Monobehaviour

    // Start is called before the first frame update
    void Start()
    {
        _selectedScenario = Configuration.GetInt("Scenario", 0);
        if (_selectedScenario < 0 || _selectedScenario > SceneManager.sceneCountInBuildSettings - 2)
        {
            Debug.LogError("[OutOfRange] invalid Scenario id");
            Application.Quit(100);
        }
        else{
            SceneManager.LoadScene(_selectedScenario + 1, LoadSceneMode.Single);
        }
    }

    #endregion
}