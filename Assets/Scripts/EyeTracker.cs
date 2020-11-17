using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject eyePrefab;

    private GameObject eyeLeft;
    private GameObject eyeRight;

    private ARFace arFace;

    private InteractionManager interactionManager;

    private float calibrationOffsetX = 0;
    private float calibrationOffsetY = 0;

    private Vector3 mirrorFixation;

    void Awake()
    {
        arFace = this.GetComponent<ARFace>();
        interactionManager = FindObjectOfType<InteractionManager>();
    }

    private void OnEnable()
    {
        ARFaceManager arFaceManager = FindObjectOfType<ARFaceManager>();

        interactionManager.CalibrateButton.onClick.AddListener(CalibrateReticleFixationPoint);

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

        interactionManager.CalibrateButton.onClick.RemoveListener(CalibrateReticleFixationPoint);

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

        bool arState = (arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);

        UpdateScreenReticle();

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

    private void UpdateScreenReticle()
    {
        var mainCamera = Camera.main;

        var fixationInViewSpace = mainCamera.WorldToViewportPoint(arFace.fixationPoint.position);

        // The camera texture is mirrored so x and y must be changed to match where the fixation point is in relation to the face.
        var mirrorFixationInView = new Vector3(1 - fixationInViewSpace.x, 1 - fixationInViewSpace.y, fixationInViewSpace.z);

        if (interactionManager.screenReticle != null)
        {
            mirrorFixation = mainCamera.ViewportToScreenPoint(mirrorFixationInView);

            interactionManager.screenReticle.anchoredPosition3D = new Vector3((mirrorFixation.x + calibrationOffsetX) * interactionManager.GazeMovementSlider.value, (mirrorFixation.y + calibrationOffsetY) * interactionManager.GazeMovementSlider.value, mirrorFixation.z);
        }
    }

    public void CalibrateReticleFixationPoint()
    {
        calibrationOffsetX = -mirrorFixation.x;
        calibrationOffsetY = -mirrorFixation.y;
    }
}