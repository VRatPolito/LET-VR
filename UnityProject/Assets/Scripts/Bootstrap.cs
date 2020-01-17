using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PrattiToolkit;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _techniqueDropdown;
    [SerializeField] private TMP_Dropdown _scenarioDropdown;
    [SerializeField] private TMP_InputField _participantIF;
    [SerializeField] private Button _launchButton;

    private List<string> _scenes = new List<string>();
    private GameObject _loading;

    #region Monobehaviour

    void Awake()
    {
        _loading = transform.GetChildRecursive("Loading").gameObject;
        Assert.IsNotNull(_scenarioDropdown);
        Assert.IsNotNull(_techniqueDropdown);
        Assert.IsNotNull(_launchButton);
        Assert.IsNotNull(_loading);
        Assert.IsNotNull(_participantIF);

        _loading.SetActive(false);

        _participantIF.text = ConfigurationLookUp.Instance.GetString("ParticipantId","");

        _launchButton.onClick.AddListener(() =>
        {
            var it = _scenarioDropdown.options[_scenarioDropdown.value].text.Split('_');
            int selectedScenario = -1;
            int.TryParse(it[0], out selectedScenario);
            if (selectedScenario < 0 || selectedScenario > SceneManager.sceneCountInBuildSettings - 2)
            {
                Debug.LogError("[OutOfRange] invalid Scenario id");
                Application.Quit(100);
            }
            else
            {
                ConfigurationLookUp.Instance.Put("LocomotionTechnique", _techniqueDropdown.options[_techniqueDropdown.value].text);
                ConfigurationLookUp.Instance.Put("Scenario", selectedScenario + 1);
                ConfigurationLookUp.Instance.Put("ParticipantId", _participantIF.text.Trim());
                _loading.SetActive(true);
                SceneManager.LoadScene(selectedScenario + 1, LoadSceneMode.Single);
            }
        });
    }

    void Start()
    {
        var selectedScenario = ConfigurationLookUp.Instance.GetInt("Scenario", 0);
        if (selectedScenario > SceneManager.sceneCountInBuildSettings - 2)
        {
            Debug.LogError("[OutOfRange] invalid Scenario id");
            Application.Quit(100);
        }
        else if (selectedScenario < 0)
        {
            string path = "";
            string sceneName = "";
            _scenarioDropdown.ClearOptions();
            _techniqueDropdown.ClearOptions();
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                path = SceneUtility.GetScenePathByBuildIndex(i);
                sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
                _scenes.Add($"{i-1}_{sceneName}");
            }
            _scenarioDropdown.AddOptions(_scenes);
            _techniqueDropdown.AddOptions(Enum.GetNames(typeof(LocomotionTechniqueType)).ToList());
        }
        else
        {
            SceneManager.LoadScene(selectedScenario + 1, LoadSceneMode.Single);
            _loading.SetActive(true);
        }
    }

    #endregion
}