using UnityEngine;

public class FpsTarget : MonoBehaviour {

	public int FPSGoal = 30;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = FPSGoal;
	}
}
