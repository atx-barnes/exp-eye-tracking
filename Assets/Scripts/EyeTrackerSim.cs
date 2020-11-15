using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackerSim : MonoBehaviour
{
    public Transform LookAtPositon;

    public GameObject RightEye;
    public GameObject LeftEye;

    public Image RightEyeScreenDot;
    public Image LeftEyeScreenDot;

    public Image ScreenReticle;

    public GameObject PointRight;
    public GameObject PointLeft;

    public GameObject SpaceReticle;

    public bool StopEyeGaze = false;

    Plane PlaneNearCamera;

    void Update()
    {
        PlaneNearCamera = new Plane(Camera.main.transform.forward, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + Camera.main.nearClipPlane));

        if (!StopEyeGaze)
        {
            // Simulate looking directly at the device
            RightEye.transform.LookAt(LookAtPositon, Vector3.up);
            LeftEye.transform.LookAt(LookAtPositon, Vector3.up);
        }

        Ray rightEyeRay = new Ray(RightEye.transform.position, RightEye.transform.forward);
        Ray leftEyeRay = new Ray(LeftEye.transform.position, LeftEye.transform.forward);

        float rightEnter = 0.0f;

        Vector3 rightHitPoint = new Vector3();

        if (PlaneNearCamera.Raycast(rightEyeRay, out rightEnter))
        {
            rightHitPoint = rightEyeRay.GetPoint(rightEnter);

            PointRight.transform.position = rightHitPoint;

            RightEyeScreenDot.transform.position = Camera.main.WorldToScreenPoint(rightHitPoint);
        }

        float leftEnter = 0.0f;

        Vector3 leftHitPoint = new Vector3();

        if (PlaneNearCamera.Raycast(leftEyeRay, out leftEnter))
        {
            leftHitPoint = leftEyeRay.GetPoint(leftEnter);

            PointLeft.transform.position = leftHitPoint;

            LeftEyeScreenDot.transform.position = Camera.main.WorldToScreenPoint(leftHitPoint);
        }

        Vector3 ScreenReticlePos = new Vector3((leftHitPoint.x + rightHitPoint.x) / 2, (leftHitPoint.y + rightHitPoint.y) / 2, Camera.main.transform.position.z - Camera.main.farClipPlane);
        Vector3 WorldReticlePos = new Vector3((leftHitPoint.x + rightHitPoint.x) / 2, (leftHitPoint.y + rightHitPoint.y) / 2, Camera.main.transform.position.z - Camera.main.farClipPlane);

        SpaceReticle.transform.position = WorldReticlePos;

        ScreenReticle.transform.position = Camera.main.WorldToScreenPoint(ScreenReticlePos);

        Debug.DrawRay(RightEye.transform.position, RightEye.transform.forward, Color.green);
        Debug.DrawRay(LeftEye.transform.position, LeftEye.transform.forward, Color.green);

        Debug.DrawRay(RightEye.transform.position, RightEye.transform.position - new Vector3(RightEye.transform.position.x, RightEye.transform.position.y, Camera.main.farClipPlane - Camera.main.transform.position.z), Color.red);
        Debug.DrawRay(LeftEye.transform.position, LeftEye.transform.position - new Vector3(LeftEye.transform.position.x, LeftEye.transform.position.y, Camera.main.farClipPlane - Camera.main.transform.localPosition.z), Color.red);
    }
}
