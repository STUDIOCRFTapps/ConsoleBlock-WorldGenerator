using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFlipping : MonoBehaviour {

	public Transform MainCamera;

	void Start () {
		if(MainCamera == null) {
			MainCamera = Camera.main.transform;
		}
	}

	void Update () {
		transform.eulerAngles = new Vector3(-MainCamera.eulerAngles.x,MainCamera.eulerAngles.y,-MainCamera.eulerAngles.z);
		transform.position = new Vector3(MainCamera.position.x,-(MainCamera.position.y-20),MainCamera.position.z);
	}
}
