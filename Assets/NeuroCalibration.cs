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
    private TextMeshProUGUI UserResultsTMP;

    [SerializeField]
    private List<GameObject> NeuroTags;

    private Vector3 InitialNeuroTagSize;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 90;

        StartCoroutine(StartCalibrationWhenReady());

        InitialNeuroTagSize = NeuroTags[0].GetComponent<RectTransform>().localScale;
    }

    private IEnumerator StartCalibrationWhenReady() {

        // Waiting for the NeuroManager to be ready
        yield return new WaitUntil(NeuroManager.Instance.IsReady);

        // Actually start the calibration process.
        CalibrationManager.StartCalibration();

        // Listen to the incoming results
        CalibrationManager.onCalibrationResultsAvailable.AddListener(OnReceivedResults);
    }


    // Calibration results callback with device and user grade information
    private void OnReceivedResults(Device device, CalibrationResults.CalibrationGrade grade) {

        UserResultsTMP.text = $"Received results for {device.Name} with a grade of {grade}";

        foreach (GameObject tag in NeuroTags) {

            tag.GetComponent<RectTransform>().localScale = InitialNeuroTagSize;
        }
    }
}
