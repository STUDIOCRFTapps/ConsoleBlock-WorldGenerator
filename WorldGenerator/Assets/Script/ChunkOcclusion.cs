using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkOcclusion : MonoBehaviour {

	public Transform Player;
	Camera cam;
	Renderer[] chunkRenderers;
	float angle = 90f;

	public float OverallWaterHeight;
	public float LowestChunkHeight;

	bool RenderersStatus = true;

	void Start () {
		cam = Camera.main;

		chunkRenderers = new Renderer[3];
		chunkRenderers[0] = GetComponent<MeshRenderer>();
		chunkRenderers[1] = transform.GetChild(0).GetComponent<MeshRenderer>();
		chunkRenderers[2] = transform.GetChild(1).GetComponent<MeshRenderer>();
	}

	public void UpdateWater () {
		if(chunkRenderers == null) {
			chunkRenderers = new Renderer[3];
			chunkRenderers[0] = GetComponent<MeshRenderer>();
			chunkRenderers[1] = transform.GetChild(0).GetComponent<MeshRenderer>();
			chunkRenderers[2] = transform.GetChild(1).GetComponent<MeshRenderer>();
		} else if(chunkRenderers[2] == null) {
			return;
		} else if(chunkRenderers[2].bounds == null) {
			return;
		}

		chunkRenderers[2].enabled = LowestChunkHeight <= OverallWaterHeight;
	}

	public void ChangeRenderersStatus (bool Status) {
		if(RenderersStatus == Status) {
			return;
		}
		RenderersStatus = Status;

		chunkRenderers[0].enabled = Status;
		chunkRenderers[1].enabled = Status;
	} 

	float Value = 0f;
	Vector3 FowardPlayer;
	bool ChunkVisible;

	public bool NoOcclusion = false;

	void Update () {
		/*if(!NoOcclusion) {
			ChunkVisible = IsChunkVisible();
			ChangeRenderersStatus(ChunkVisible);
		} else {
			ChangeRenderersStatus(true);
		}*/
	}

	Plane[] planes;

	private bool IsChunkVisible() {
		planes = GeometryUtility.CalculateFrustumPlanes(cam);
		if(GeometryUtility.TestPlanesAABB(planes, chunkRenderers[0].bounds))
			return true;
		else
			return false;
	}
}
