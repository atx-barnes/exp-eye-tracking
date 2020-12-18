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
    public GameObject WorldTargetPrefab;

    [Header("UI Elements")]

    public Slider GazeMovementSlider;

    public Button CalibrateButton;

    public RectTransform ScreenReticle;

    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

    private Stack<GameObject> worldTargets = new Stack<GameObject>();

    private Image image;

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

        image = ScreenReticle.GetComponent<Image>();
    }

    private void Update() {

        if(arRaycastManager.Raycast(ScreenReticle.position, arRaycastHits, TrackableType.PlaneWithinPolygon)) {

            image.color = new Color(image.color.r, image.color.g, image.color.b, 0f);

            Pose hit = arRaycastHits[0].pose;

            if (worldTargets.Count == 0) {

                worldTargets.Push(Instantiate(WorldTargetPrefab, hit.position, Quaternion.identity));
            }
            else {

                worldTargets.Peek().transform.position = hit.position;
            }
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.5f);
    }

    public void PlaceTarget() {

        var target = Instantiate(WorldTargetPrefab);

        worldTargets.Push(target);
    }
}
