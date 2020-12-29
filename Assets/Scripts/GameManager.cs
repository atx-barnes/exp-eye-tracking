using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NextMind.Calibration;
using NextMind.Devices;
using NextMind.NeuroTags;
using NextMind;
using TMPro;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private CalibrationManager CalibrationManager;

    [SerializeField]
    private GameObject Reticle;

    [SerializeField]
    private GameObject NeuroTagObject;

    [SerializeField]
    private GameObject Ball;

    [SerializeField]
    private float ForceMultiplier = 50;

    private Vector3 InitialNeuroTagScale;

    private bool CalibrationIsDone = false;

    private void Awake() {

        InitialNeuroTagScale = NeuroTagObject.transform.localScale;

        Cursor.visible = false;
    }

    private void Start() {

        StartCoroutine(StartCalibrationWhenReady());
    }

    void Update() {

        RaycastHit hitReticle;

        var rayReticle = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(rayReticle, out hitReticle)) {

            if (hitReticle.rigidbody != null) {

                Reticle.transform.position = new Vector3(hitReticle.point.x, 0.01f, hitReticle.point.z);
            }
        }
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

        Debug.Log($"Received results for {device.Name} with a grade of {grade}");

        CalibrationIsDone = true;

        ResetNeuroTag();
    }

    public void AddForce() {

        if (CalibrationIsDone) {

            RaycastHit hit;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if (hit.rigidbody != null && hit.transform.tag == "ground") {

                    Vector3 dir = (hit.point - Ball.transform.position).normalized;

                    Ball.GetComponent<Rigidbody>().AddForceAtPosition(dir * ForceMultiplier, hit.point);
                }
            }
        }
    }

    public void OnFocusForce() {

        AddForce();
    }

    public void ResetNeuroTag() {

        NeuroTagObject.transform.localScale = InitialNeuroTagScale;

        Cursor.visible = false;
    }
}
