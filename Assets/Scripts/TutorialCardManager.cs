using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Events;

[Serializable]
public class StepGroup {

    public List<string> Steps = new List<string>();
}

public class TutorialCardManager : MonoBehaviour
{
    public Color EnabledButtonColor;
    public Color DisabledButtonColor;

    public TextMeshProUGUI BackButton;
    public TextMeshProUGUI NextButton;

    public GameObject StepsParent;
    public GameObject PageIndicatorPrefab;
    public Transform PageIndicatorsParent;
    public List<string> Steps;

    private List<StepGroup> StepGroups = new List<StepGroup>();
    private List<TextMeshProUGUI> StepsTMP = new List<TextMeshProUGUI>();
    private List<GameObject> PageIndicators = new List<GameObject>();

    private int index;

    private void Awake() {

        BackButton = BackButton.GetComponent<TextMeshProUGUI>();
        NextButton = NextButton.GetComponent<TextMeshProUGUI>();

        // Get references to TMPro text fields
        foreach (Transform step in StepsParent.transform) {

            StepsTMP.Add(step.GetComponent<TextMeshProUGUI>());
        }
    }

    public void InitializeTutiroialInformation(List<string> steps) {

        // Create step groups based on the amount of text fields under the parent object in the hiearchy.
        for (int i = 0, x = 0; i < steps.Count; i++) {

            if (i % StepsTMP.Count == 0) {

                StepGroup stepGroup = new StepGroup();

                for (int z = 0; z < StepsTMP.Count; z++, x++) {

                    if (x < steps.Count) {

                        // Populate each step group with tutorial content
                        stepGroup.Steps.Add(steps[x]);
                    }
                }

                // Add group to list of step groups for initializing a list of step based on a index value
                StepGroups.Add(stepGroup);
            }
        }


        // Create page display indicators per group of steps
        foreach (StepGroup group in StepGroups) {

            PageIndicators.Add(Instantiate(PageIndicatorPrefab, PageIndicatorsParent));
        }
    }

    // Populate the text fields with steps based on index number param
    private void InitializeGroupSteps(int index) {

        // Clear out previous content
        foreach (TextMeshProUGUI tmp in StepsTMP) {

            tmp.text = string.Empty;
        }

        // Populate text fields from each step group with the steps
        for (int i = 0; i < StepGroups[index].Steps.Count; i++) {

            StepsTMP[i].text = StepGroups[index].Steps[i];
        }

        // Turn off indicators
        foreach (GameObject indicator in PageIndicators) {

            indicator.transform.GetChild(1).gameObject.SetActive(false);
        }

        // Turn on indicator based on group
        PageIndicators[index].transform.GetChild(1).gameObject.SetActive(true);

        // Check if the the last steps are initialized
        if (index == StepGroups.Count - 1) {

            NextButton.color = DisabledButtonColor;

        } else {

            NextButton.color = EnabledButtonColor;
        }

        // Check if the the first steps are initialized
        if (index == 0) {

            BackButton.color = DisabledButtonColor;

        } else {

            BackButton.color = EnabledButtonColor;
        }
    }

    // Cycle to the next group of steps
    public void Next() {

        if(index < StepGroups.Count - 1) {

            index++;

            InitializeGroupSteps(index);
        }
    }

    // Cycle to the previous group of steps
    public void Back() {

        if(index > 0) {

            index--;

            InitializeGroupSteps(index);
        }
    }
}
