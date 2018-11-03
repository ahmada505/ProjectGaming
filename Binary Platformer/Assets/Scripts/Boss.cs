using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour {

    public float speed;
    private float shootSpeed;
    public int state;
    private Shootable health;

    public GameObject bullet;
	
	void Start () {
        health = GetComponent<Shootable>();
        SetState(1);
        StartCoroutine(Shoot());
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
        if (health.health <= 10 && health.health > 5)
        {
            SetState(2);
        }
        else if(health.health <= 5)
        {
            SetState(3);
        }
	}

    private IEnumerator Shoot()
    {
        yield return new WaitForSeconds(shootSpeed);
        if(state != 3)
            Instantiate(bullet, transform.position, transform.rotation);
        else
        {
            Instantiate(bullet, transform.position, new Quaternion(transform.rotation.x, transform.rotation.y - 45, transform.rotation.z, transform.rotation.w));
            Instantiate(bullet, transform.position, transform.rotation);
            Instantiate(bullet, transform.position, new Quaternion(transform.rotation.x, transform.rotation.y + 45, transform.rotation.z, transform.rotation.w));
        }
        StartCoroutine(Shoot());
    }

    private void SetState(int newstate)
    {
        state = newstate;
        switch (state)
        {
            case 1:
                {
                    speed = 6;
                    shootSpeed = 2;
                    break;
                }
            case 2:
                {
                    shootSpeed = 1;
                    break;
                }
            case 3:
                {
                    break;
                }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Cube")
        {
            speed *= -1;
        }
    }
}
