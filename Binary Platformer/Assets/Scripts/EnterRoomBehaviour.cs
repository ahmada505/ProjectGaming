using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoomBehaviour : MonoBehaviour {

    public GameObject enemyPrefab;
    public GameObject[] spawnPoints;
    private GameObject Boss;

    private bool doOnce = true;
    public bool activateBoss;

	void Start () {
        gameObject.SetActive(false);
        Boss = GameObject.Find("Boss");
        int random = (int)Random.Range(0f, 6f);
        if (random >= 4) gameObject.SetActive(true);
        if (gameObject.transform.parent.gameObject.name == "Last Room")
        {
            gameObject.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && doOnce)
        {
            if (gameObject.transform.parent.gameObject.name == "Last Room")
            {
                Destroy(Boss);
            }
            else
            {
                Debug.Log("TRIGGER!!!");
                doOnce = false;
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    Instantiate(enemyPrefab, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
                }
                StartCoroutine(CheckEnemies());

                other.GetComponent<Shoot>().infinteAmmo = false;
                other.GetComponent<Shoot>().ammo += 15;
                if (activateBoss)
                {
                    Boss.GetComponent<Boss>().enabled = true;
                }
            }
        }
    }

    IEnumerator CheckEnemies()
    {
        yield return new WaitForSeconds(3f);
        if(GameObject.FindGameObjectsWithTag("enemy").Length > 0)
        {
            StartCoroutine(CheckEnemies());
        }
    }
}
