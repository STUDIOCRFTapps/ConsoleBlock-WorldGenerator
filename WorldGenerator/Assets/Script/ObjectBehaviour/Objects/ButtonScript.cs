using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : WInteractable {
	WObjectOutput objectOutput;

	bool ButtonDown = false;
	bool ButtonPress = false;
	bool ButtonUp = false;

	public new void OnInteraction () {
		if(Input.GetMouseButtonDown(1)) {
			ButtonDown = true;
		}

		if(Input.GetMouseButtonUp(1)) {
			ButtonUp = true;
		}

		if(Input.GetMouseButton(1)) {
			ButtonPress = true;
		}

		objectOutput.data.Add(new DataFragment(Name + ".IsButtonDown",ButtonDown));
		objectOutput.data.Add(new DataFragment(Name + ".IsButtonUp",ButtonUp));
		objectOutput.data.Add(new DataFragment(Name + ".IsButtonPress",ButtonPress));
		objectOutput.OnPushRequested();

		ButtonDown = false;
		ButtonPress = false;
		ButtonUp = false;
	}
}
