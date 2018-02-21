using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class WorldLoader : MonoBehaviour {

	public float BiomeTransitionSmoothing = 0.00185f;

	[HideInInspector]
	public string WorldDirectory = "World";
	public string WorldDirectoryPath;
	public string PersistentDataPath;

	public Text[] DebugingText;
	public Image[] DebugingImages;

	public Transform[] Colliders;

	public int SimulatedChunkSize;
	public GameObject MeshTemplate;

	[HideInInspector]
	public WorldManager worldManager;

	public float LOD0EndChunkDistance = 4;
	public float LOD1EndChunkDistance = 8;
	public float LOD2EndChunkDistance = 11;
	public float LOD3EndChunkDistance = 15;
	public float LOD4EndChunkDistance = 18;

	public WorldCreator worldCreator;

	public Transform Player;
	public Rigidbody PlayerBody;
	Vector2 OldChunkPos;
	Vector2 NewChunkPos;

	public WorldTexture[] worldTextures;
	public Biome[] biomes;
	public SubBiome[] subBiome;

	List<Chunk> ChunkList;

	//Lists made to match the new functions
	List<ChunkRequirementParameters> GarbadgeChunkList = new List<ChunkRequirementParameters>();
	List<ChunkRequirementParameters> RequiredChunkList = new List<ChunkRequirementParameters>();
	List<ChunkRequirementParameters> OldRequiredChunkList = new List<ChunkRequirementParameters>();
	public class ChunkRequirementParameters {
		public Vector2 Position;
		public float Force;
		public int LOD;
		public ChunkRequirementParameters(Vector2 Pos, float F, int L) {
			Position = Pos;
			Force = F;
			LOD = L;
		}
	}
	bool ListContainsChunkType (List<ChunkRequirementParameters> crpList, ChunkRequirementParameters crp, int LODModif) {
		foreach(ChunkRequirementParameters param in crpList) {
			if(param.Position == crp.Position && param.LOD+LODModif == crp.LOD) {
				return true;
			}
		}
		return false;
	}
	bool ListContainsChunkPos (List<ChunkRequirementParameters> crpList, ChunkRequirementParameters crp) {
		foreach(ChunkRequirementParameters param in crpList) {
			if(param.Position == crp.Position) {
				return true;
			}
		}
		return false;
	}

	/// End of variable declaration
	///
	///

	void Start () {
		InitializeDatapaths();

		//Initialize a bunch of stuff
		NewChunkPos = new Vector2(Mathf.Floor(Player.position.x/SimulatedChunkSize),Mathf.Floor(Player.position.z/SimulatedChunkSize));
		OldChunkPos = NewChunkPos;
		worldCreator.Execute();
		PlayerBody = Player.GetComponent<Rigidbody>();
		worldManager = new WorldManager(worldCreator,biomes,subBiome);
		worldManager.BiomeTransitionSmoothing = BiomeTransitionSmoothing;
		PlayerBlockPos = new Vector3(Mathf.Floor(Player.transform.position.x),Mathf.Floor(Player.transform.position.y),Mathf.Floor(Player.transform.position.z));

		//Preparing the world
		ChunkList = new List<Chunk>();
		for(int y = -Mathf.CeilToInt(LOD4EndChunkDistance); y <= Mathf.Ceil(LOD4EndChunkDistance); y++) {
			for(int x = -Mathf.CeilToInt(LOD4EndChunkDistance); x <= Mathf.Ceil(LOD4EndChunkDistance); x++) {
				float Distance = Vector2.Distance(Vector2.zero,new Vector2(x,y));

				if(Distance < LOD4EndChunkDistance) {
					ChunkList.Insert(0,new Chunk(new Vector2(NewChunkPos.x+x,NewChunkPos.y+y),DistanceToLOD(Distance),SimulatedChunkSize,worldTextures,worldManager,MeshTemplate,false));
					ChunkList[0].PrepareGeneration();
					ChunkList[0].GenerateWorldObject();
				}
			}
		}
	}

	int DistanceToLOD (float Distance) {
		if(Distance <= LOD0EndChunkDistance) {
			return 1;
		} else if(Distance <= LOD1EndChunkDistance) {
			return 2;
		} else if(Distance <= LOD2EndChunkDistance) {
			return 4;
		} else if(Distance <= LOD3EndChunkDistance) {
			return 8;
		} else if(Distance <= LOD4EndChunkDistance) {
			return 16;
		} else {
			return 1;
		}
	}

	void InitializeDatapaths () {
		//World data path and directories
		PersistentDataPath = Application.persistentDataPath;
		WorldDirectoryPath = PersistentDataPath + Path.DirectorySeparatorChar + "worldsaves" + Path.DirectorySeparatorChar + WorldDirectory + Path.DirectorySeparatorChar + "chuckdata";

		//Makes sure the world directory exists
		if(!Directory.Exists(WorldDirectoryPath)) {
			Directory.CreateDirectory(WorldDirectoryPath);
		}
	}
		
	bool AlreadyLoading = false;
	Vector2 ChunkToLoad = Vector2.zero;

	bool ActionRunning = false;
	bool AllowColliding = true;

	List<Action> PrepareActions = new List<Action>();
	List<Action> ThreadingActions = new List<Action>();
	//List<Action> PreThreadingActions = new List<Action>();
	Chunk[] NewChunk;
	Thread PreparingThread;
	Thread CleaningThread;
	Thread GeneratingThread;
	Thread ActionThread;

	Vector3 PlayerBlockPos;

	void Update () {
		//Debugging
		if(!IsUpdatingCollision) {
			StartCoroutine(UpdateCollision());
			//DebugingText[0].text = worldManager.NaNError;
			DebugingImages[0].fillAmount = worldManager.DebugNumbers[0];
			DebugingImages[1].fillAmount = worldManager.DebugNumbers[1];
			DebugingImages[2].fillAmount = worldManager.DebugNumbers[2];
		}

		//Water Physics Simulation (Temporary)
		if(Player.transform.position.y < -17) {
			PlayerBody.velocity += Vector3.up * 2.5f;
			PlayerBody.velocity = new Vector3(PlayerBody.velocity.x,Mathf.Clamp(PlayerBody.velocity.y,Mathf.NegativeInfinity,40f),PlayerBody.velocity.z);
		}

		PlayerBlockPos = new Vector3(Mathf.Floor(Player.transform.position.x),Mathf.Floor(Player.transform.position.y),Mathf.Floor(Player.transform.position.z));

		//Preparing collider height values
		if(HeightValues != null) {
			int index = 0;
			for(int x = -1; x < 2; x++) {
				for(int y = -1; y < 2; y++) {
					float Height = HeightValues[x+1,y+1];
					float Difference = 0;

					Difference = Height - Player.transform.position.y;

					Colliders[index].position = new Vector3(PlayerBlockPos.x+x,Height,PlayerBlockPos.z+y);
					Colliders[index].localScale = new Vector3(1,Mathf.Clamp(Mathf.Abs(Difference),1f,Mathf.Infinity)*16,1);

					index++;
				}
			}
		}

		//Makes sure to update the position if nothing is under construction
		if(!ActionRunning) {
			NewChunkPos = new Vector2(Mathf.Floor(Player.position.x/SimulatedChunkSize),Mathf.Floor(Player.position.z/SimulatedChunkSize));
		}

		if(((Mathf.Abs(OldChunkPos.x - NewChunkPos.x) + Mathf.Abs(OldChunkPos.y - NewChunkPos.y)) > 2)) {
			StartCoroutine(PrepareChunkLoading());
		}
		if(RequiredChunkList.Count > 0 && !IsPreparingChunkLoading) {
			//Load a new chunk
			StartCoroutine(LoadNextNewChunk());
		}
	}

	//Collision Managing
	bool IsUpdatingCollision = false;
	float[,] PHeightValues;
	float[,] HeightValues;
	Thread WaitForCollisionData;
	IEnumerator UpdateCollision () {
		IsUpdatingCollision = true;
		PHeightValues = new float[3,3];

		Vector3 PlayerPos = new Vector3(Mathf.Floor(Player.transform.position.x),Mathf.Floor(Player.transform.position.y),Mathf.Floor(Player.transform.position.z));

		Action WaitForValue = () => {
			for(int x = 0; x < 3; x++) {
				for(int y = 0; y < 3; y++) {
					PHeightValues[x,y] = worldManager.GetHeightMap(PlayerBlockPos.x+(x-1),PlayerBlockPos.z+(y-1))[0];
				}
			}
		};

		WaitForCollisionData = new Thread(new ThreadStart(WaitForValue));
		WaitForCollisionData.Start();
		yield return new WaitUntil(() => !WaitForCollisionData.IsAlive);

		HeightValues = PHeightValues;
		IsUpdatingCollision = false;
	}

	//Debugging
	void OnDrawGizmos () {
		if(!Application.isPlaying) {
			return;
		}
		Gizmos.color = new Color(0.1f,0.8f,0.9f,0.8f);
		for(int i = 0; i < RequiredChunkList.Count; i++) {
			//Gizmos.DrawLine(new Vector3(RequiredChunkList[i].Position.x*SimulatedChunkSize,0f,RequiredChunkList[i].Position.y*SimulatedChunkSize),Player.position);
			Gizmos.DrawCube(new Vector3(RequiredChunkList[i].Position.x*SimulatedChunkSize+8,0f,RequiredChunkList[i].Position.y*SimulatedChunkSize+8),new Vector3(16,4,16));
		}
		Gizmos.color = new Color(0.8f,0.9f,0.1f,0.8f);
		for(int i = 0; i < GarbadgeChunkList.Count; i++) {
			//Gizmos.DrawLine(new Vector3(RequiredChunkList[i].Position.x*SimulatedChunkSize,0f,RequiredChunkList[i].Position.y*SimulatedChunkSize),Player.position);
			Gizmos.DrawCube(new Vector3(GarbadgeChunkList[i].Position.x*SimulatedChunkSize+8,0f,GarbadgeChunkList[i].Position.y*SimulatedChunkSize+8),new Vector3(4,16,4));
		}
	}


	bool IsPreparingChunkLoading = false;
	IEnumerator PrepareChunkLoading () {
		if(IsPreparingChunkLoading) {
			yield break;
		} else {
			IsPreparingChunkLoading = true;
		}

		//Wait for all chunks to be loaded before messing with anything
		OldChunkPos = NewChunkPos;
		yield return new WaitWhile(() => IsLoadingAChunk);

		RequiredChunkList = new List<ChunkRequirementParameters>();
		/*for(int i = 0; i < RequiredChunkList.Count; i++) {
			float Distance = Vector2.Distance(NewChunkPos,RequiredChunkList[i].Position);
			RequiredChunkList[i].Force = Distance;
			RequiredChunkList[i].LOD = DistanceToLOD(Distance);
		}*/

		//Prepare what is require
		for(int y = -Mathf.CeilToInt(LOD4EndChunkDistance); y <= Mathf.Ceil(LOD4EndChunkDistance); y++) {
			for(int x = -Mathf.CeilToInt(LOD4EndChunkDistance); x <= Mathf.Ceil(LOD4EndChunkDistance); x++) {
				float Distance = Vector2.Distance(Vector2.zero,new Vector2(x,y));

				if(Distance < LOD4EndChunkDistance) {
					if(!ListContainsChunkType(RequiredChunkList,new ChunkRequirementParameters(new Vector2(NewChunkPos.x+x,NewChunkPos.y+y),0,DistanceToLOD(Distance)),0)) {
						RequiredChunkList.Add(new ChunkRequirementParameters(new Vector2(NewChunkPos.x+x,NewChunkPos.y+y),Distance,DistanceToLOD(Distance)));
					}
				}
			}
		}

		//Sort the list in force order
		RequiredChunkList.Sort(delegate(ChunkRequirementParameters c1, ChunkRequirementParameters c2) {
			return c1.Force.CompareTo(c2.Force);
		});

		if(RequiredChunkList.Count > ChunkList.Count) {
			RequiredChunkList.RemoveRange(Mathf.Clamp(ChunkList.Count-1,0,RequiredChunkList.Count-1),Mathf.Clamp(RequiredChunkList.Count-1-ChunkList.Count-1,0,int.MaxValue));
		}

		yield return new WaitForSeconds(0.002f);


		//Get a list of chunk to delete
		GarbadgeChunkList = new List<ChunkRequirementParameters>();
		for(int l = 0; l < ChunkList.Count; l++) {
			bool IsRequired = false;
			for(int i = 0; i < RequiredChunkList.Count; i++) {
				if(ChunkList[l].MainChunkPosition == RequiredChunkList[i].Position) {
					IsRequired = true;
				}
			}
			if(!IsRequired) {
				float Distance = Vector2.Distance(NewChunkPos,ChunkList[l].MainChunkPosition);

				ChunkList[l].ChunkObject.SetActive(false);
				GarbadgeChunkList.Add(new ChunkRequirementParameters(ChunkList[l].MainChunkPosition,Distance,0));
			}
		}

		//Find what is already generated
		int deleteIndex = 0;
		for(int i = 0; i < RequiredChunkList.Count; i++) {
			bool deleteUpgrade = false;
			for(int l = 0; l < ChunkList.Count; l++) {
				if(RequiredChunkList[deleteIndex].Position == ChunkList[l].MainChunkPosition && RequiredChunkList[deleteIndex].LOD == ChunkList[l].GetLODLevel() && ChunkList[l].ChunkObject.activeInHierarchy) {
					RequiredChunkList.RemoveAt(deleteIndex);
					deleteUpgrade = true;
					break;
				}
			}
			if(!deleteUpgrade) {
				deleteIndex++;
			}
		}

		yield return new WaitForSeconds(0.002f);

		//Sort the list in force order
		GarbadgeChunkList.Sort(delegate(ChunkRequirementParameters c1, ChunkRequirementParameters c2) {
			return c1.Force.CompareTo(c2.Force);
		});
		GarbadgeChunkList.Reverse();

		IsPreparingChunkLoading = false;
	}

	bool IsLoadingAChunk = false;
	IEnumerator LoadNextNewChunk () {
		//Quit If we're alreading loading it!
		if(IsLoadingAChunk) {
			yield break;
		} else {
			IsLoadingAChunk = true;
		}

		Thread thread;

		//Recondition & recreate
		bool UpdateOnly = false;
		int ChunkIndex = 0;
		for(int i = 0; i < ChunkList.Count; i++) {
			if(ChunkList[i].MainChunkPosition == RequiredChunkList[0].Position) {
				//Update only
				UpdateOnly = true;
				ChunkIndex = i;
				break;
			}
		}

		if(!UpdateOnly) {
			if(GarbadgeChunkList.Count < 1) {
				Debug.LogError("There's not enough chunk to recycle");
				yield break;
			}
			for(int i = 0; i < ChunkList.Count; i++) {
				if(GarbadgeChunkList[0].Position == ChunkList[i].MainChunkPosition) {
					ChunkIndex = i;
					break;
				}
			}
		}
			
		ChunkList[ChunkIndex].ReconditionMesh(RequiredChunkList[0].Position,RequiredChunkList[0].LOD,false);

		Action Generating = () => {
			ChunkList[ChunkIndex].PrepareGeneration();
		};

		thread = new Thread(new ThreadStart(Generating));
		thread.Start();
		yield return new WaitUntil(() => !thread.IsAlive);
		ChunkList[ChunkIndex].GenerateWorldObject();
		ChunkList[ChunkIndex].ChunkObject.SetActive(true);

		//Display the object, clean up arrays
		RequiredChunkList.RemoveAt(0);
		if(!UpdateOnly) {
			GarbadgeChunkList.RemoveAt(0);
		}

		IsLoadingAChunk = false;
		yield break;
	}

}
