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
            transform.position = Vector3.Lerp(new Vector3(transform.position.x, transform.position.y + 1f, -15f), targetCamPos, smoothing * Time.deltaTime);
        }
        else
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    //public GameObject player;

    //private Vector3 offset;

    //// Use this for initialization
    //void Start()
    //{
    //    offset = transform.position - player.transform.position;
    //}

    //// Update is called once per frame
    //void LateUpdate()
    //{
    //    transform.position = player.transform.position + offset;
    //}
}
