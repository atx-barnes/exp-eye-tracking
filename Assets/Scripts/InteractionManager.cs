using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using TMPro;

[RequireComponent(typeof(ARRaycastManager))]
public class InteractionManager : MonoBehaviour
{
    public GameObject WorldReticlePrefab;

    [Header("UI Elements")]

    public Slider GazeMovementSlider;

    public Button CalibrateButton;

    public RectTransform ScreenReticle;

    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

    private GameObject worldReticleObject;

    private EyeTracker m_EyeTracker;

    public EyeTracker eyeTracker {

        get => m_EyeTracker;
        set => m_EyeTracker = value;
    }

    [Header("AR Managers")]

    [SerializeField]
    private ARCameraManager m_CameraManager;

    public ARCameraManager cameraManager {

        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    private ARPlaneManager m_ARPlaneManager;

    public ARPlaneManager arPlaneManager {

        get => m_ARPlaneManager;
        set => m_ARPlaneManager = value;
    }

    [SerializeField]
    private ARRaycastManager m_ARRaycastManager;

    public ARRaycastManager arRaycastManager {

        get => m_ARRaycastManager;
        set => m_ARRaycastManager = value;
    }

    [SerializeField]
    ARSession m_Session;

    public ARSession session {

        get => m_Session;
        set => m_Session = value;
    }

    private void OnEnable() {

        if (m_CameraManager == null || m_Session == null)
            return;

        m_CameraManager.requestedFacingDirection = CameraFacingDirection.World;
    }

    private void Update() {

        if(arRaycastManager.Raycast(ScreenReticle.position, arRaycastHits, TrackableType.PlaneWithinPolygon)) {

            Pose hit = arRaycastHits[0].pose;

            if (worldReticleObject == null) {

                worldReticleObject = Instantiate(WorldReticlePrefab, hit.position, Quaternion.identity);
            }
            else {

                worldReticleObject.transform.position = hit.position;

                worldReticleObject.GetComponentInChildren<TextMeshPro>().text = hit.position.ToString("F3");
            }
        }
    }
}
