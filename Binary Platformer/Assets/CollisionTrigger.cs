using UnityEngine;
using System.Collections;

public class CollisionTrigger : MonoBehaviour {

    private BoxCollider2D playerCollider;

    [SerializeField]
    private BoxCollider2D platformCollider;

    [SerializeField]
    private BoxCollider2D platformTrigger;


    // Use this for initialization
    void Start () {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();

        Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.name == "player")
        {
            Physics2D.IgnoreCollision(platformCollider, playerCollider, true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "player")
        {
            Physics2D.IgnoreCollision(platformCollider, playerCollider, false);
        }
    }

	
	// Update is called once per frame
	void FixedUpdate () {
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
    }

}
