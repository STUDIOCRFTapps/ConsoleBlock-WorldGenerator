using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class DepthOfFieldEdit : MonoBehaviour {

	public PostProcessingProfile profile;
	RaycastHit raycastHit;
	Ray ray;

	DepthOfFieldModel.Settings settings;

	// Update is called once per frame
	void Update () {
		ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));

		if(Physics.Raycast(ray, out raycastHit)) {
			Debug.DrawLine(ray.origin, raycastHit.point, Color.red);
			settings = profile.depthOfField.settings;
			settings.focusDistance = raycastHit.distance;
			profile.depthOfField.settings = settings;
		} else {
			settings = profile.depthOfField.settings;
			settings.focusDistance = Mathf.Infinity;
			profile.depthOfField.settings = settings;
		}
	}
}
