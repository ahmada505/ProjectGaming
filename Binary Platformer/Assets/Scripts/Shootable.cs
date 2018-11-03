using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Shootable : MonoBehaviour{
    public float MaxHealth;
    public float health;
    public int score;
    private Color originalColor;

    void Start()
    {
        originalColor = GetComponent<Renderer>().material.color;
        health = MaxHealth;
    }

    public bool Hit(float dmg)
    {
        bool isKilled = false;
        StartCoroutine(HitFeedbak());
        health -= dmg;
        if(health <= 0)
        {
            Destroy(this.gameObject);
            isKilled = true;
        }
        return isKilled;
    }

    public IEnumerator HitFeedbak()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.125f);
        renderer.material.color = originalColor;
        yield return new WaitForSeconds(0.125f);
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.125f);
        renderer.material.color = originalColor;
    }
}
