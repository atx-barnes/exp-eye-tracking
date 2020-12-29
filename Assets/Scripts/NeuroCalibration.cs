using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NextMind.Calibration;
using NextMind.Devices;
using NextMind.NeuroTags;
using NextMind;
using TMPro;

public class NeuroCalibration : MonoBehaviour
{
    [SerializeField]
    private CalibrationManager CalibrationManager;

    [SerializeField]
    private GameObject NeuroTag;

    [SerializeField]
    private Animator Animator;

    private Vector3 InitialNeuroTagSize;

    private bool isCalibrationDone = false;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 90;

        StartCoroutine(StartCalibrationWhenReady());

        InitialNeuroTagSize = NeuroTag.transform.localScale;
    }

    private IEnumerator StartCalibrationWhenReady() {

        // Waiting for the NeuroManager to be ready
        yield return new WaitUntil(NeuroManager.Instance.IsReady);

        // Actually start the calibration process.
        CalibrationManager.StartCalibration();

        Debug.Log("Starting Calibration");

        // Listen to the incoming results
        CalibrationManager.onCalibrationResultsAvailable.AddListener(OnReceivedResults);
    }


    // Calibration results callback with device and user grade information
    private void OnReceivedResults(Device device, CalibrationResults.CalibrationGrade grade) {

        Debug.Log($"Received results for {device.Name} with a grade of {grade}");

        NeuroTag.transform.localScale = InitialNeuroTagSize;

        isCalibrationDone = true;
    }

    public void DebugNeuroTag(string text) {

        if (isCalibrationDone) {

            Debug.Log(text);
        }
    }

    public void SetAnimatorTrigger(string trigger) {

        if(isCalibrationDone) {

            foreach (AnimatorControllerParameter p in Animator.parameters) {

                if (p.type == AnimatorControllerParameterType.Trigger) {

                    Animator.ResetTrigger(p.name);
                }
            }

            Animator.SetTrigger(trigger);
        }
    }
}
