using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullit : MonoBehaviour {
    
	void Start () {
		
	}
	
	void Update () {
        transform.Translate(transform.right);
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            other.GetComponent<Player>().Hit();
            Destroy(gameObject);
        }
    }
}
