using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    
    public Vector3 playerToMouse;
    private Vector3 movement;
    private Vector3 velocity;
    private float speed = 10;
    private bool isGrounded = true;

    private int maxHealth = 3;
    public int health;
    public RawImage[] hearths = new RawImage[3];

    private Rigidbody rigidbody;
    private LineRenderer laserLine;

	void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        laserLine = GetComponent<LineRenderer>();
        health = maxHealth;
	}
	
    void Update()
    {
        Turning();
    }

	void FixedUpdate ()
    {
        laserLine.SetPosition(0, transform.position);
        movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        velocity = movement.normalized * speed;
        rigidbody.velocity = velocity;

        /*if (Input.GetKeyUp(KeyCode.Space) && isGrounded)
        {
            transform.Translate(new Vector3(0, 1.2f, 0));
            isGrounded = false;
        }
        else if (!isGrounded)
        {
            GetComponent<Rigidbody>().AddForce(0, -1000f, 0);
        }*/
    }

    void Turning()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition); // cast the ray from MainCamera to mouseposition

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, 100f))
        {
            playerToMouse = floorHit.point; // store the Vector3 from hitpoint to player transform
            playerToMouse.y = transform.position.y;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse - transform.position); // store the playerToMouse as a quaternion newRotation
            rigidbody.MoveRotation(newRotation); // rotate the rigidbody to look at newRotation

            laserLine.SetPosition(1, playerToMouse);
            laserLine.SetPosition(1, new Vector3(laserLine.GetPosition(1).x, transform.position.y + 0.5f, laserLine.GetPosition(1).z));
        }
    }

    public void Hit()
    {
        health--;
        for (int i = 0; i < hearths.Length; i++)
        {
            if (i < health) { hearths[i].enabled = true; }
            else { hearths[i].enabled = false; }
        }
        if (health <= 0) { Application.LoadLevel(0); }
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.tag == "ground")
        {
            isGrounded = true;
        }

        else if(other.gameObject.tag == "enemy")
        {
            Hit();
        }

        else if(other.gameObject.name == "Ice")
        {
            speed = 9;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.name == "Ice")
        {
            speed = 7;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.name == "Pull")
        {
            rigidbody.useGravity = false;
            Vector3 direction = (other.gameObject.transform.position - transform.position);
            GetComponent<ConstantForce>().force = Vector3.Normalize(direction) * 500;
        }

        else if (other.name == "Push")
        {
            rigidbody.useGravity = false;
            Vector3 direction = (other.gameObject.transform.position - transform.position);
            GetComponent<ConstantForce>().force = -(Vector3.Normalize(direction) * 500);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Pull" || other.name == "Push")
        {
            rigidbody.useGravity = true;
            GetComponent<ConstantForce>().force = Vector3.zero;
        }
    }
}
