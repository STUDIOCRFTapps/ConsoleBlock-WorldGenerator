using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayNormals : MonoBehaviour {

	public float length = 0.2f;
	public Color color = Color.red; 

	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;

		if(mesh == null) {
			return;
		}

		for(int i = 0; i < mesh.normals.Length; i++) {
			Debug.DrawRay(transform.position+mesh.vertices[i],mesh.normals[i]*length,color,Mathf.Infinity);
		}
	}
}
