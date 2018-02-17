using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterOffset : MonoBehaviour {

	public float Direction = 45f;
	public float Speed = 5f;

	Vector2 Direction2D;

	public Material material;

	// Update is called once per frame
	void Update () {
		Direction2D += TreeCreatorTools.CircleCreator(Direction,Time.deltaTime*Speed);
		if(Direction2D.x >= 1f) {
			Direction2D.x -= 1f;
		}
		if(Direction2D.y >= 1f) {
			Direction2D.y -= 1f;
		}
		material.mainTextureOffset = new Vector2(Mathf.Repeat(Direction2D.x,1),Mathf.Repeat(Direction2D.y,1));
	}
}
