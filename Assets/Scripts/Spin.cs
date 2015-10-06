using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {

	public Vector3 speed = new Vector3(0,45,0);

	void Start () {
	
	}
	
	void Update () {
		transform.Rotate( Time.deltaTime * speed );
	}
}
