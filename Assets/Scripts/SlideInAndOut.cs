using UnityEngine;
using System.Collections;

public class SlideInAndOut : MonoBehaviour {

	const int ANIM_IN = 0;
	const int ANIN_HOLD = 1;
	const int ANIM_OUT = 2;

	public Vector3 startOffset = new Vector3(-3f,0,0);
	public Vector3 endOffset = new Vector3(3f,0,0);
	public float inDuration = 2f;
	public float outDuration = 2f;
	public float maxSpeed = 1f;

	private Vector3 startPos;
	private int state = 0;
	private Vector3 vel = Vector3.zero;
	

	void Start () {
		startPos = transform.position;
		transform.position = startPos + startOffset;
	}
	
	void Update () {
		Vector3 dest;
		
		if(state==ANIM_IN){
			dest = startPos;
			transform.position = Vector3.SmoothDamp( transform.position, dest, ref vel, inDuration, maxSpeed );
			if( Vector3.Distance(transform.position,dest) < 0.01f ){
				state++;
				vel = Vector3.zero;				
			}			
		}
		
		if(state==ANIM_OUT){
			
			dest = startPos + endOffset;
			transform.position = Vector3.SmoothDamp( transform.position, dest, ref vel, outDuration, maxSpeed );
			if( Vector3.Distance(transform.position,dest) < 0.01f ){
				Debug.Log("Destroying model");
				state++;
				Destroy(gameObject);
			}
		}
	}
	
	public void TransitionOut(){
		state=ANIM_OUT;
	}
	

}
