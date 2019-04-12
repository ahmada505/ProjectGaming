using UnityEngine;
using System.Collections;

public class BeginScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        handleInput();
    }

    private void handleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Application.LoadLevel("BSP dungeon");
        }

    }
}
