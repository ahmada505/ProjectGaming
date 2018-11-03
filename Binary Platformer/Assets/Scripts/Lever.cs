using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour {

    public GameObject wall;
    private bool doOnce = true;
    public GameObject objectToRotate;

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "Player" && doOnce)
        {
            objectToRotate.transform.Rotate(new Vector3(60, 0, 0));
            wall.transform.Translate(new Vector3(0, -2.5f, 0));
            doOnce = false;
        }
    }
}
