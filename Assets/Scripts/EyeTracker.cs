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

    private TextMeshProUGUI rightEyeTelemetryTMP;
    private TextMeshProUGUI leftEyeTelemetryTMP;

    private GameObject eyeLeft;
    private GameObject eyeRight;
    private ARFace arFace;

    Plane PlaneNearCamera;

    private GameObject screenReticle;

    void Awake()
    {
        arFace = this.GetComponent<ARFace>();
        screenReticle = GameObject.Find("Screen Reticle");
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
        if (arFace.leftEye != null && eyeLeft == null)
        {
            eyeLeft = Instantiate(eyePrefab, arFace.leftEye);
            eyeLeft.SetActive(false);
        }

        if (arFace.rightEye != null && eyeRight == null)
        {
            eyeRight = Instantiate(eyePrefab, arFace.rightEye);
            eyeRight.SetActive(false);
        }

        screenReticle.transform.position = Camera.main.WorldToScreenPoint(arFace.fixationPoint.position);

        bool arState = (arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);

        SetVisibility(arState);
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
