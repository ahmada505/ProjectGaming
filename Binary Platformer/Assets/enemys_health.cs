using UnityEngine;
using System.Collections;

public class enemys_health : MonoBehaviour {


    public float max_health = 100;
    public float cur_health = 0f;
    public GameObject healthBar;

    private Rigidbody2D rigid2D;
    GameObject player;
    Vector3 position;
    string textPos = "";


    public Transform target;
    public float speed = 0.1f;
    private float minDistance = 20f;
    private float range;
    public player points;
    float calc_health;


    // Use this for initialization
    void Start () {
        cur_health = max_health;
        rigid2D = GetComponent<Rigidbody2D>();
        position = transform.position;
        //player = GameObject.Find("Player");
        
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //target = player.transform;
        //points.GetComponent<player>();
        points = player.GetComponent<player>();
        
        
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        player = GameObject.FindGameObjectWithTag("Player");
        target = GameObject.FindGameObjectWithTag("Player").transform;
        points = player.GetComponent<player>();

        target = player.transform;

        if (player.transform.position.x <= position.x)
        {
            textPos = "left";
            
        }
        if (player.transform.position.x >= position.x) {
            textPos = "right";
        }

        range = Vector2.Distance(transform.position, target.position);

        if (range < minDistance)
        {

            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * speed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * speed * Time.deltaTime);
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        points.SetCountText();
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "ATK")
        {
            if (textPos == "left")
            {
                decreasehealth();
                rigid2D.AddForce(transform.right * 500);
            }
            if (textPos == "right")
            {
                decreasehealth();
                rigid2D.AddForce(-transform.right * 500);
            }
        }

    }
    public void decreasehealth()
    {
        if(points.count >= 15)
        {
            cur_health -= 2.5f;
        }

        if (points.count >= 25)
        {
            cur_health -= 2.5f;
        }

        cur_health -= 10f;
        //float calc_health = cur_health / max_health;
       // SetHealthBar(calc_health);

        if (cur_health <= 0)
        {
            //gameObject.SetActive(false);
            points.count += 25;
            Destroy(this.gameObject);
            
        }
    }

    //public void SetHealthBar(float myHealth)
    //{
    //    healthBar.transform.localScale = new Vector2(myHealth, healthBar.transform.localScale.y);
    //}
}
