using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shoot : MonoBehaviour {

    private float time;
    private float score;
    private float endScore;
    private GameObject Boss;
    private Text endText;

    public bool infinteAmmo = true;
    public int ammo = 15;
    public float gunRange = 100f;
    LineRenderer line;

    private Text ammoText;

	void Start () {
        endText = GameObject.Find("EndText").GetComponent<Text>();
        Boss = GameObject.Find("Boss");
        line = GetComponent<LineRenderer>();
        ammoText = GameObject.Find("AmmoText").GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButtonUp(0))
        {
            if (ammo > 0)
                shoot();
        }
        if(Boss)
            time += Time.deltaTime;
        else
        {
            endText.enabled = true;
            endScore = score * (time / 10);
            endText.text = "You Won!\nYour score is " + score;
        }
	}


    void shoot()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, gunRange))
        {
            if(!infinteAmmo)
                //ammo--;
            StartCoroutine("ShotFeedback");
            Shootable enemy = hit.collider.GetComponent<Shootable>();

            if (enemy)
            {
                if (enemy.Hit(3f))
                {
                    ammo += 5;
                    score += enemy.score;
                }
            }
            ammoText.text = ammo + "X";
        }
    }

    IEnumerator ShotFeedback()
    {
        line.startWidth = 0.2f;
        yield return new WaitForSeconds(0.15f);
        line.startWidth = 0.1f;
    }
}
