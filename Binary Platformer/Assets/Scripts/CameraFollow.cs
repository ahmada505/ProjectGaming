using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    private float smoothing = 5f;

    Vector3 offset;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target != null)
            offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetCamPos = target.position + offset;
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, 20f, transform.position.z), targetCamPos, smoothing * Time.deltaTime);
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
}
