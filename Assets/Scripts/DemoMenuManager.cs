using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

[Serializable]
public class Demo {

    public string Name;
    public string Description;
    public string SceneName;
    public Sprite IconImage;
    public Button DemoMenuButton;
    public List<string> TutorialSteps;
}

public class DemoMenuManager : MonoBehaviour {

    public TextMeshProUGUI NameTMP;
    public TextMeshProUGUI DescriptionTMP;

    public TutorialCardManager TutorialCardManager;

    public Button StartDemoSceneButton;
    public Button ContinueButton;

    public Image Icon;

    public List<Demo> Demo = new List<Demo>();

    private void OnEnable() {

        foreach (Demo demo in Demo) {

            demo.DemoMenuButton.onClick.AddListener(() => LoadDemoInformation(demo));
        }
    }

    public void LoadDemoInformation(Demo demo) {

        Debug.Log("Loading Demo Information...");

        NameTMP.text = demo.Name;
        DescriptionTMP.text = demo.Description;
        Icon.sprite = demo.IconImage;

        ContinueButton.onClick.AddListener(() => LoadTutorial(demo));
    }

    public void LoadTutorial(Demo demo) {

        Debug.Log("Loading Tutorial...");

        TutorialCardManager.InitializeTutiroialInformation(demo.TutorialSteps);

        ContinueButton.onClick.RemoveAllListeners();

        StartDemoSceneButton.onClick.AddListener(() => LoadDemoExperience(demo));
    }

    public void LoadDemoExperience(Demo demo) {

        Debug.Log("Loading Scene...");

        SceneManager.LoadScene(demo.SceneName);

        StartDemoSceneButton.onClick.RemoveAllListeners();
    }

    private void OnDisable() {

        foreach (Demo demo in Demo) {

            demo.DemoMenuButton.onClick.RemoveAllListeners();
        }

        // Remove button listeners
        ContinueButton.onClick.RemoveAllListeners();

        StartDemoSceneButton.onClick.RemoveAllListeners();
    }
}
