using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	private int HP = 20;
	private Rigidbody2D r;
	private float CurX;
	private float CurY;
	private float TargetX;
	private float TargetY;

	// Use this for initialization
	void Start () {
		r=gameObject.GetComponent<Rigidbody2D>();
		//r.velocity = new Vector2 (-0.8f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		/*if (r.velocity.SqrMagnitude () < 0.1) {
			r.velocity = new Vector2 (0f, 0f);
		} else {
			Vector2 f = new Vector2(2f, 0f);
			Debug.Log (r.velocity);
			r.AddForce(f, ForceMode2D.Force);
		}*/
		Move ();
	}

	public void beAtked(int value)
	{
		HP -= value;
		//UIEffect.instance.Shake (GameObject.Find("Background"),0.05f,0.005f);
		if (HP <= 0)
			gameObject.SetActive (false);
	}

	public void Move(){
		CurX = transform.position.x;
		CurY = transform.position.y;
		GameObject a = GameObject.Find ("Man");
		TargetX = a.transform.position.x;
		TargetY = a.transform.position.y;
		Vector2 tar = new Vector2 (TargetX, TargetY);
		Vector2 start = new Vector2 (CurX, CurY);
		Vector2 ver = tar - start;
		r.MovePosition (start + ver.normalized * 0.5f * Time.deltaTime);
	}
		
}
