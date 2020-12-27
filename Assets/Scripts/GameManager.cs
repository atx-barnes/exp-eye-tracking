using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject Ball;

    public float ForceMultiplier = 50;

    void Update() {

        if (Input.GetMouseButtonDown(0)) {

            RaycastHit hit;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {

                if (hit.rigidbody != null) {

                    Vector3 dir = (hit.point - Ball.transform.position).normalized;

                    Ball.GetComponent<Rigidbody>().AddForceAtPosition(dir * ForceMultiplier, hit.point);
                }
            }
        }
    }
}
