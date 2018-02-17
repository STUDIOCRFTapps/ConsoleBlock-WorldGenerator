using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialOffseter : MonoBehaviour {

	public Material material;
	public Vector2 Speed;

	void Update () {
		
		//Verify
		if(material == null) {
			return;
		}

		//Move
		material.mainTextureOffset += Speed * Time.deltaTime;

		//Clamp
		material.mainTextureOffset = new Vector2(Mathf.Repeat(material.mainTextureOffset.x,1f), Mathf.Repeat(material.mainTextureOffset.y,1f));
	}
}
