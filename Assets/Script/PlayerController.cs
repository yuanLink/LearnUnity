using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public int speed;//感觉没影响
    private Rigidbody2D rg2d;
    public float friction;


	// Use this for initialization
	void Start () {
        rg2d = GetComponent<Rigidbody2D>();//试下？

    }

    // Update is called once per frame

    private void FixedUpdate(){
        
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);//据说材质有friction选项
        
        // Vector2 fricMovement = new Vector2(moveHorizontal - friction, moveVertical);
        Debug.Log(movement);
        rg2d.AddForce(movement*speed*Time.deltaTime);//?
    
    }
}
