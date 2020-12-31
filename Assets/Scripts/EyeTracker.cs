using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    public Vector3 eyeGazePosition;
    public bool arEyeTrackingState;

    [SerializeField]
    private GameObject eyePrefab;
    private GameObject eyeLeft;
    private GameObject eyeRight;

    private ARFace arFace;

    private InteractionManager interactionManager;

    private float calibrationOffsetX = 0;
    private float calibrationOffsetY = 0;

    void Awake() {

        arFace = this.GetComponent<ARFace>();
    }

    private void OnEnable() {

        ARFaceManager arFaceManager = FindObjectOfType<ARFaceManager>();

        interactionManager = FindObjectOfType<InteractionManager>();

        if (interactionManager != null) {

            interactionManager.CalibrateButton.onClick.AddListener(CalibrateReticleFixationPoint);

            interactionManager.eyeTracker = this;
        }

        //TODO: Fix this

        if(arFaceManager != null /* && arFaceManager.subsystem != null && arFaceManager.subsystem.SubsystemDescriptor.supportsEyeTracking*/) {

            arFace.updated += OnUpdated;
        }
        else {

            Debug.LogWarningFormat("Eye tracking not support on device");
        }
    }

    private void OnDisable() {

        arFace.updated -= OnUpdated;

        interactionManager.CalibrateButton.onClick.RemoveListener(CalibrateReticleFixationPoint);

        SetVisibility(false);
    }

    /// <summary>
    /// Callback detects any changes that are made to the AR Face data.
    /// </summary>
    /// <param name="eventArgs"></param>
    void OnUpdated(ARFaceUpdatedEventArgs eventArgs) {

        if (arFace.leftEye != null && eyeLeft == null) {

            eyeLeft = Instantiate(eyePrefab, arFace.leftEye);

            eyeLeft.SetActive(false);
        }

        if (arFace.rightEye != null && eyeRight == null) {

            eyeRight = Instantiate(eyePrefab, arFace.rightEye);

            eyeRight.SetActive(false);
        }

        arEyeTrackingState = (arFace.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready);

        UpdateScreenReticle();

        SetVisibility(arEyeTrackingState);
    }

    /// <summary>
    /// Sets visibility of each eye prefab so they dont show up in the scene after being instantiated.
    /// </summary>
    /// <param name="isVisible"></param>
    private void SetVisibility(bool isVisible) {

        if(eyeLeft != null && eyeRight != null) {

            eyeLeft.SetActive(isVisible);

            eyeRight.SetActive(isVisible);
        }
    }

    /// <summary>
    /// Updates the position of the reticle in screen space from the fixation point in world space.
    /// </summary>
    private void UpdateScreenReticle() {

        var fixationInViewSpace = Camera.main.WorldToViewportPoint(arFace.fixationPoint.position);

        // The camera texture is mirrored so x and y must be changed to match where the fixation point is in relation to the face.
        var mirrorFixationInView = new Vector3(1 - fixationInViewSpace.x, 1 - fixationInViewSpace.y, fixationInViewSpace.z);

        if (interactionManager.ScreenReticle != null) {

            eyeGazePosition = Camera.main.ViewportToScreenPoint(mirrorFixationInView);

            interactionManager.ScreenReticle.anchoredPosition3D = new Vector3((eyeGazePosition.x + calibrationOffsetX) * interactionManager.GazeMovementSlider.value, (eyeGazePosition.y + calibrationOffsetY) * interactionManager.GazeMovementSlider.value, eyeGazePosition.z);
        }
    }

    /// <summary>
    /// Calibrates the reticle position relitive to the center of the screen.
    /// </summary>
    public void CalibrateReticleFixationPoint() {

        calibrationOffsetX = -eyeGazePosition.x;

        calibrationOffsetY = -eyeGazePosition.y;
    }
}