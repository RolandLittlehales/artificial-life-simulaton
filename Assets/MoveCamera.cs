using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour {
	
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.W))
            this.transform.Translate(new Vector3(0,0,1.0f),Space.World);
		
		if (Input.GetKey(KeyCode.D))
            this.transform.Translate(1.0f,0,0);
				if (Input.GetKey(KeyCode.A))
            this.transform.Translate(-1.0f,0,0);
		
		      if (Input.GetKey(KeyCode.S))
             this.transform.Translate(new Vector3(0,0,-1.0f),Space.World);

	}
}
