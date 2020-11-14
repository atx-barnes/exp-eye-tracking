using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject eyePrefab;

    [SerializeField]
    private GameObject reticlePrefab;

    private TextMeshProUGUI rightEyeTelemetryTMP;
    private TextMeshProUGUI leftEyeTelemetryTMP;

    private GameObject eyeLeft;
    private GameObject eyeRight;
    private ARFace arFace;

    private Canvas canvas;

    Image imgReticle;

    void Awake()
    {
        arFace = this.GetComponent<ARFace>();
        canvas = FindObjectOfType<Canvas>();

        rightEyeTelemetryTMP = GameObject.FindGameObjectWithTag("Right Eye").GetComponent<TextMeshProUGUI>();
        leftEyeTelemetryTMP = GameObject.FindGameObjectWithTag("Left Eye").GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        ARFaceManager arFaceManager = FindObjectOfType<ARFaceManager>();

        if(arFaceManager != null && arFaceManager.subsystem != null && arFaceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            arFace.updated += OnUpdated;
        }
        else
        {
            Debug.LogWarningFormat("Eye tracking not support on device");
        }
    }

    private void OnDisable()
    {
        arFace.updated -= OnUpdated;
        SetVisibility(false);
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        if(arFace.leftEye != null && eyeLeft == null)
        {
            eyeLeft = Instantiate(eyePrefab, arFace.leftEye);

            // Move this
            imgReticle = Instantiate(reticlePrefab, canvas.transform).GetComponent<Image>();

            eyeLeft.SetActive(false);
        }

        if(arFace.rightEye != null && eyeRight == null)
        {
            eyeRight = Instantiate(eyePrefab, arFace.rightEye);
            eyeRight.SetActive(false);
        }

        bool arState = (arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);

        SetVisibility(arState);

        Vector3 eulerAnglesLeftEye = arFace.leftEye.localEulerAngles;
        Vector3 eulerAnglesRightEye = arFace.rightEye.localEulerAngles;

        rightEyeTelemetryTMP.text = "angles x: " + eulerAnglesRightEye.x + " y: " + eulerAnglesRightEye.y + " z: " + eulerAnglesRightEye.z;
        leftEyeTelemetryTMP.text = "angles x: " + eulerAnglesLeftEye.x + " y: " + eulerAnglesLeftEye.y + " z: " + eulerAnglesLeftEye.z;

        RaycastHit hit;

        if (Physics.Raycast(eyeRight.transform.position, eyeRight.transform.TransformDirection(Vector3.left), out hit, Mathf.Infinity))
        {
            Debug.LogFormat("Right Eye Hit: " + hit.transform.name);
        }

        imgReticle.transform.position = FindObjectOfType<ARCameraManager>().GetComponent<Camera>().WorldToScreenPoint(arFace.rightEye.position);
    }

    private void SetVisibility(bool isVisible)
    {
        if(eyeLeft != null && eyeRight != null)
        {
            eyeLeft.SetActive(isVisible);
            eyeRight.SetActive(isVisible);
        }
    }
}
