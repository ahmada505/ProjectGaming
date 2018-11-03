using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {

    Transform target;

	void Start () {
        target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.position, 5 * Time.deltaTime);
	}
}
