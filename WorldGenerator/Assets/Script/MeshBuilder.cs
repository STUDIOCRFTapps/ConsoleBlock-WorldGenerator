using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MeshBuilder : MonoBehaviour {

	public MeshFilter meshFilter;

	public string FileName = "MeshFile.txt";

	public List<Vector3> Verts;
	public List<Vector2> UV;
	public List<Vector3> Normals;
	public List<int> Tris;

	// Use this for initialization
	void Start () {
		meshFilter = meshFilter.GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.B)) {
			
			Mesh mesh = new Mesh();

			mesh.SetVertices(Verts);
			mesh.SetUVs(0,UV);
			mesh.SetNormals(Normals);
			mesh.SetTriangles(Tris,0);

			meshFilter.mesh = mesh;
		} else if(Input.GetKeyDown(KeyCode.S)) {
			ObjExporter.MeshToFile(meshFilter,"Assets/"+FileName);
		}
	}
}
