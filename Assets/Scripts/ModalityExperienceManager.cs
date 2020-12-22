using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;

[Serializable]
public class Modality {

    public string Name;
    public string Description;
    public string ModalitySceneName;
    public Button ModalityMenuButton;
    public List<string> TutorialSteps;
}

public class ModalityExperienceManager : MonoBehaviour {

    public TextMeshProUGUI NameTMP;
    public TextMeshProUGUI DescriptionTMP;

    public TutorialCardManager TutorialCardManager;

    public Button StartModalitySceneButton;
    public Button ContinueButton;

    public List<Modality> Modality = new List<Modality>();

    private void OnEnable() {

        foreach (Modality modality in Modality) {

            modality.ModalityMenuButton.onClick.AddListener(() => LoadModelityInformation(modality));
        }
    }

    public void LoadModelityInformation(Modality modality) {

        NameTMP.text = modality.Name;
        DescriptionTMP.text = modality.Description;

        ContinueButton.onClick.AddListener(() => LoadTutorial(modality));
    }

    public void LoadTutorial(Modality modality) {

        TutorialCardManager.InitializeTutiroialInformation(modality.TutorialSteps);

        ContinueButton.onClick.RemoveAllListeners();

        StartModalitySceneButton.onClick.AddListener(() => LoadModelityScene(modality));
    }

    public void LoadModelityScene(Modality modality) {

        SceneManager.LoadScene(modality.ModalitySceneName);

        StartModalitySceneButton.onClick.RemoveAllListeners();
    }

    private void OnDisable() {

        foreach (Modality modality in Modality) {

            modality.ModalityMenuButton.onClick.RemoveAllListeners();
        }
    }
}
