using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public Slider GazeMovementSlider;
    public Button CalibrateButton;
    public TextMeshProUGUI DebugTMPro;

    [SerializeField]
    private ARCameraManager m_CameraManager;

    public ARCameraManager cameraManager
    {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    ARSession m_Session;

    public ARSession session
    {
        get => m_Session;
        set => m_Session = value;
    }

    public RectTransform screenReticle;

    void OnEnable()
    {
        if (m_CameraManager == null || m_Session == null)
            return;

        m_CameraManager.requestedFacingDirection = CameraFacingDirection.World;
    }
}
