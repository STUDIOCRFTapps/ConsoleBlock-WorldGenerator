using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberController : MonoBehaviour {

	public Animator Grabber;
	public Transform CollisionBlock;
	public Collider[] Colliders;
	public Transform Player;
	Rigidbody PlayerRigidb;
	public int stuckLength;

	float stuckCounter = 0;
	bool S = false;
	bool Fly = false;

	// Use this for initialization
	void Start () {
		PlayerRigidb = Player.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		//Add the new value
		if(Player.position.y < CollisionBlock.position.y) {
			foreach(Collider col in Colliders) {
				col.enabled = false;
			}

			if(stuckCounter > stuckLength) {
				S = true;
			} else {
				stuckCounter += Time.deltaTime;
			}
		} else {
			if(!Colliders[0].enabled) {
				foreach(Collider col in Colliders) {
					col.enabled = true;
				}
			}

			stuckCounter = 0;
			S = false;
		}

		//Execute action
		if(S && !IsAlreadyLoading) {
			StartCoroutine(ReplacePlayer());
		}
		if(Fly) {
			PlayerRigidb.velocity = new Vector3(PlayerRigidb.velocity.x,25f,PlayerRigidb.velocity.z);
			Player.position += Vector3.up*30f*Time.deltaTime;
		}
	}

	bool IsAlreadyLoading = false;
	IEnumerator ReplacePlayer () {
		IsAlreadyLoading = true;

		Grabber.gameObject.SetActive(true);
		Grabber.SetTrigger("Spawn");

		yield return new WaitUntil(() => Grabber.GetCurrentAnimatorStateInfo(0).IsName("GrabberGrabIdle"));
		Fly = true;
		yield return new WaitUntil(() => !S);
		yield return new WaitForSeconds(0.6f);
		Fly = false;

		Grabber.SetTrigger("Release");
		yield return new WaitUntil(() => Grabber.GetCurrentAnimatorStateInfo(0).IsName("GrabberInactive"));

		Grabber.gameObject.SetActive(false);

		IsAlreadyLoading = false;
	}
}
