using System;
using System.Collections;
using System.Collections.Generic;
using PrattiToolkit;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    private List<string> _scenes = new List<string>();
    private TMP_Dropdown _dropdown;
    private Button _launchButton;
    private TMP_InputField _participantIF;
    private GameObject _loading;

    #region Monobehaviour

    void Awake()
    {
        _launchButton = GetComponentInChildren<Button>();
        _dropdown = GetComponentInChildren<TMP_Dropdown>();
        _participantIF = GetComponentInChildren<TMP_InputField>();
        _loading = transform.GetChildRecursive("Loading").gameObject;
        Assert.IsNotNull(_dropdown);
        Assert.IsNotNull(_launchButton);
        Assert.IsNotNull(_loading);
        Assert.IsNotNull(_participantIF);

        _loading.SetActive(false);

        _participantIF.text = ConfigurationLookUp.Instance.GetString("ParticipantId","");

        _launchButton.onClick.AddListener(() =>
        {
            var it = _dropdown.options[_dropdown.value].text.Split('_');
            int selectedScenario = -1;
            int.TryParse(it[0], out selectedScenario);
            if (selectedScenario < 0 || selectedScenario > SceneManager.sceneCountInBuildSettings - 2)
            {
                Debug.LogError("[OutOfRange] invalid Scenario id");
                Application.Quit(100);
            }
            else
            {
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
            _dropdown.ClearOptions();
            for (int i = 1; i < SceneManager.sceneCountInBuildSettings; ++i)
            {
                path = SceneUtility.GetScenePathByBuildIndex(i);
                sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
                _scenes.Add($"{i-1}_{sceneName}");
            }
            _dropdown.AddOptions(_scenes);
        }
        else
        {
            SceneManager.LoadScene(selectedScenario + 1, LoadSceneMode.Single);
            _loading.SetActive(true);
        }
    }

    #endregion
}