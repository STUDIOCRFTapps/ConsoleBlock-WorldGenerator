using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk {
	WorldManager worldM;

	public Vector2 MainChunkPosition = new Vector2(0,0);
	public GameObject ChunkObject;
	public Transform ChunkTransform;

	public float[,] HeightMap;
	public float[,] HeightMapX;
	public float[,] HeightMapZ;

	float LowestY;

	MeshCollider mc;
	MeshFilter mf;
	MeshFilter mw;

	int WaterBlockSize = 2;

	WorldTexture[] worldTextures;
	int BlockSize = 1;
	int SimulatedChunkSize = 16;
	GameObject MeshTemplate;

	bool GenerateCollider = false;

	public int GetLODLevel () {
		return BlockSize;
	}

	public bool IsColliderActive () {
		return GenerateCollider;
	}

	public Chunk(Vector2 Position, int blockSize, int simulatedChunkSize, WorldTexture[] textures, WorldManager wm, GameObject mesh, bool CreateCollider) {
		worldTextures = textures;
		BlockSize = blockSize;
		WaterBlockSize = Mathf.Clamp(BlockSize*2,1,16);
		SimulatedChunkSize = simulatedChunkSize;
		GenerateCollider = CreateCollider;

		MainChunkPosition = Position;
		MeshTemplate = mesh;
		worldM = wm;

		PrepareChunkHeightMap(blockSize,new Vector2(MainChunkPosition.x*SimulatedChunkSize,MainChunkPosition.y*SimulatedChunkSize));

		LowestY = Mathf.Infinity;
	}

	public void SetNewValues (Vector2 Position, int blockSize, int simulatedChunkSize, WorldTexture[] textures, GameObject mesh, bool CreateCollider) {
		worldTextures = textures;
		BlockSize = blockSize;
		WaterBlockSize = Mathf.Clamp(BlockSize*2,1,16);
		SimulatedChunkSize = simulatedChunkSize;
		GenerateCollider = CreateCollider;

		MainChunkPosition = Position;
		MeshTemplate = mesh;

		mf = null;
		mc = null;
		mw = null;
		DestroyChunk();
		ChunkTransform = null;

		PrepareChunkHeightMap(blockSize,new Vector2(MainChunkPosition.x*SimulatedChunkSize,MainChunkPosition.y*SimulatedChunkSize));

		LowestY = Mathf.Infinity;
	}

	bool Token = false;
	Vector3 TokenSize;
	Vector3 TokenPos;

	public void ReconditionMesh (Vector2 Position, int blockSize, bool CreateCollider) {
		/*if(ChunkObject == null) {
			return;
		}*/
		BlockSize = blockSize;
		WaterBlockSize = Mathf.Clamp(BlockSize*2,1,16);
		MainChunkPosition = Position;

		Token = true;
		TokenSize = new Vector3(BlockSize,BlockSize,BlockSize);
		TokenPos = new Vector3(MainChunkPosition.x*SimulatedChunkSize,0,MainChunkPosition.y*SimulatedChunkSize);

		GenerateCollider = CreateCollider;

		PrepareChunkHeightMap(blockSize,new Vector2(TokenPos.x,TokenPos.z));

		LowestY = Mathf.Infinity;
	}

	public void GenerateTerrainModels () {
		if(ChunkObject == null) {
            Debug.Log("Error while trying to generate terrain assest: Missing Object");
			return;
		}
        if(HeightMap == null) {
            Debug.Log("Error while trying to generate terrain assest: Missing Height Map");
            return;
        }
        if(HeightMap.GetLength(0) == 0) {
            Debug.Log("Error while trying to generate terrain assest: Empty Height Map");
            return;
        }

		//Remove current assets
		var children = new List<GameObject>();
		foreach (Transform child in ChunkObject.transform.GetChild(0)) children.Add(child.gameObject);
		children.ForEach(child => GameObject.Destroy(child));

		if(worldM.creator.loader.DebuggingMode) {
			//Create new ones
			for(int y = 0; y < SimulatedChunkSize; y++) {
				for(int x = 0; x < SimulatedChunkSize; x++) {
					int cbiome = 96;//worldM.GetBiomesAt(x + (MainChunkPosition.x * SimulatedChunkSize),y + (MainChunkPosition.y * SimulatedChunkSize),1).infoList[0];
					float value = Mathf.PerlinNoise((x + (MainChunkPosition.x * SimulatedChunkSize)) * 1.128f,(y + (MainChunkPosition.y * SimulatedChunkSize)) * 1.128f);
					Random.InitState(value.GetHashCode());
					if(worldM.biomes[cbiome].Structures == null) {
						continue;
					}
					int stcId = Random.Range(0,worldM.biomes[cbiome].Structures.Structures.Length);
					if(value < worldM.biomes[cbiome].Structures.Structures[stcId].StructureFrequency) {
						if(HeightMap[x / BlockSize,y / BlockSize]*BlockSize > worldM.biomes[cbiome].Structures.Structures[stcId].MinSpawningHeight && HeightMap[x / BlockSize,y / BlockSize]*BlockSize < worldM.biomes[cbiome].Structures.Structures[stcId].MaxSpawningHeight) {
							int stcCode = Random.Range(0,worldM.biomes[cbiome].Structures.Structures[stcId].Objects.Length);
							GameObject stc = (GameObject)GameObject.Instantiate(worldM.biomes[cbiome].Structures.Structures[stcId].Objects[stcCode],ChunkObject.transform.GetChild(0));
							stc.transform.localPosition = new Vector3(x / BlockSize, HeightMap[x / BlockSize,y / BlockSize], y / BlockSize);
							stc.transform.eulerAngles = Vector3.up * (Random.Range(0,4) * 90);
							stc.transform.localScale = Vector3.one * Random.Range(worldM.biomes[cbiome].Structures.Structures[stcId].MinScaleModifRange,worldM.biomes[cbiome].Structures.Structures[stcId].MaxScaleModifRange) / BlockSize;
						}
					}
				}
			}
		} else {
			//Create new ones
			for(int y = 0; y < SimulatedChunkSize; y++) {
				for(int x = 0; x < SimulatedChunkSize; x++) {
					int cbiome = worldM.GetBiomesAt(x + (MainChunkPosition.x * SimulatedChunkSize),y + (MainChunkPosition.y * SimulatedChunkSize),1).infoList[0];
					float value = Mathf.PerlinNoise((x + (MainChunkPosition.x * SimulatedChunkSize)) * 1.128f,(y + (MainChunkPosition.y * SimulatedChunkSize)) * 1.128f);
					Random.InitState(value.GetHashCode());
					if(worldM.biomes[cbiome].Structures == null) {
						continue;
					}
					int stcId = Random.Range(0,worldM.biomes[cbiome].Structures.Structures.Length);
					if(value < worldM.biomes[cbiome].Structures.Structures[stcId].StructureFrequency) {
						if(HeightMap[x / BlockSize,y / BlockSize] * BlockSize > worldM.biomes[cbiome].Structures.Structures[stcId].MinSpawningHeight && HeightMap[x / BlockSize,y / BlockSize] < worldM.biomes[cbiome].Structures.Structures[stcId].MaxSpawningHeight) {
							int stcCode = Random.Range(0,worldM.biomes[cbiome].Structures.Structures[stcId].Objects.Length);
							GameObject stc = (GameObject)GameObject.Instantiate(worldM.biomes[cbiome].Structures.Structures[stcId].Objects[stcCode],ChunkObject.transform.GetChild(0));
							stc.transform.localPosition = new Vector3(x / BlockSize,HeightMap[x / BlockSize,y / BlockSize],y / BlockSize);
							stc.transform.eulerAngles = Vector3.up * (Random.Range(0,4) * 90);
							stc.transform.localScale = Vector3.one * Random.Range(worldM.biomes[cbiome].Structures.Structures[stcId].MinScaleModifRange,worldM.biomes[cbiome].Structures.Structures[stcId].MaxScaleModifRange) / BlockSize;
						}
					}
				}
			}
		}
	}

	public void PrepareChunkHeightMap (int blockSize, Vector2 Pos) {
		//HeightMap = new float[SimulatedChunkSize/blockSize,SimulatedChunkSize/blockSize];
		HeightMapX = new float[SimulatedChunkSize/blockSize,SimulatedChunkSize/blockSize];
		HeightMapZ = new float[SimulatedChunkSize/blockSize,SimulatedChunkSize/blockSize];

		/*for(int x = 0; x < SimulatedChunkSize/blockSize; x++) {
			for(int z = 0; z < SimulatedChunkSize/blockSize; z++) {
				HeightMap[x,z] = worldM.GetHeightMap(Pos.x+(x*blockSize),Pos.y+(z*blockSize))[0];
			}
		}*/
		HeightMap = worldM.GetWorldHeight((int)Pos.x/SimulatedChunkSize,(int)Pos.y/SimulatedChunkSize,blockSize);
		for(int x = 0; x < SimulatedChunkSize/blockSize; x++) {
			for(int z = 0; z < SimulatedChunkSize/blockSize; z++) {
				HeightMapX[x,z] = HeightMap[x+1,z];
			}
		}
		for(int x = 0; x < SimulatedChunkSize/BlockSize; x++) {
			for(int z = 0; z < SimulatedChunkSize/BlockSize; z++) {
				HeightMapZ[x,z] = HeightMap[x,z+1];
			}
		}
	}

	public void DestroyChunk () {
		UnityEngine.Object.Destroy(ChunkObject);
	}

	Vector2 VCode1 = new Vector2(0,0);
	Vector2 VCode2 = new Vector2(1,0);
	Vector2 VCode3 = new Vector2(0,1);
	Vector2 VCode4 = new Vector2(1,1);

	public bool IsPreparing = false;

	//Declaring for the prepaRatio
	Vector3[] vertices = new Vector3[0];
	int[] triangles = new int[0];
	Vector2[] UVs = new Vector2[0];

	List<Vector3> physXverts = new List<Vector3>();
	List<int> physXtris = new List<int>();

	List<Vector3> WaterVerts = new List<Vector3>();
	List<int> WaterTris = new List<int>();
	List<Vector2> WaterUVs = new List<Vector2>();
	List<Vector3> WaterNormal = new List<Vector3>();

	Mesh cMesh;
	Mesh pMesh;
	Mesh wMesh;

	public void PrepareGeneration () {
		physXtris.Clear();
		physXverts.Clear();
		WaterNormal.Clear();
		WaterTris.Clear();
		WaterUVs.Clear();
		WaterVerts.Clear();

		IsPreparing = true;

		int[,] xWallHeight = new int[SimulatedChunkSize/BlockSize,SimulatedChunkSize/BlockSize];
		int[,] yWallHeight = new int[SimulatedChunkSize/BlockSize,SimulatedChunkSize/BlockSize];

		float nY = 0f;
		float nX = 0f;
		float nZ = 0f;

		float wY = 0f;

		float xCoord = 0f;
		float yCoord = 0f;

		float xPos = 0f;
		float yPos = 0f;

		int c = 0;
		int v = 0;
		int c2 = 0;
		int neg = 0;
		int p = 0;
		int p2 = 0;

		int Dispacement = 0;
		for(int y = 0; y < SimulatedChunkSize/BlockSize; y++) {
			for(int x = 0; x < SimulatedChunkSize/BlockSize; x++) {
				//nY = GetValueAtPixel(x*BlockSize+(TestStartPos.x*SimulatedChunkSize),y*BlockSize+(TestStartPos.y*SimulatedChunkSize));
				nY = HeightMap[x,y];
				nX = HeightMapX[x,y];
				nZ = HeightMapZ[x,y];

				if(nY*BlockSize < LowestY) {
					LowestY = nY*BlockSize;
				}

				neg = 1;
				if(nX-nY < 0) {
					neg = -1;
				}

				if(neg == 1) {
					Dispacement += Mathf.Abs(Mathf.FloorToInt(nY-nX));
				} else {
					Dispacement += Mathf.Abs(Mathf.CeilToInt(nY-nX));
				}

				neg = 1;
				if(nZ-nY < 0) {
					neg = -1;
				}

				if(neg == 1) {
					Dispacement += Mathf.Abs(Mathf.FloorToInt(nY-nZ));
				} else {
					Dispacement += Mathf.Abs(Mathf.CeilToInt(nY-nZ));
				}
			}
		}

		int[] Triangles = new int[(Dispacement + (SimulatedChunkSize/BlockSize * SimulatedChunkSize/BlockSize))*6];
		int tri = 0;
		Vector3[] Vertices = new Vector3[(Dispacement + (SimulatedChunkSize/BlockSize * SimulatedChunkSize/BlockSize))*4];
		int ver = 0;
		Vector2[] VerticesUVs = new Vector2[(Dispacement + (SimulatedChunkSize/BlockSize * SimulatedChunkSize/BlockSize))*4];
		int uv = 0;
		UVs = new Vector2[(Dispacement + (SimulatedChunkSize/BlockSize * SimulatedChunkSize/BlockSize))*6];

		for(int y = 0; y < SimulatedChunkSize/WaterBlockSize; y++) {
			for(int x = 0; x < SimulatedChunkSize/WaterBlockSize; x++) {
				c = ((x+y*SimulatedChunkSize/WaterBlockSize)*4);

				wY = worldM.GetWaterHeight(x*WaterBlockSize+(MainChunkPosition.x*(SimulatedChunkSize)),y*WaterBlockSize+(MainChunkPosition.y*(SimulatedChunkSize)))/WaterBlockSize;

				WaterVerts.Add(new Vector3(x,wY,y));
				WaterUVs.Add(VCode1);
				WaterNormal.Add(Vector3.up);
				WaterVerts.Add(new Vector3(x+1,wY,y));
				WaterUVs.Add(VCode2);
				WaterNormal.Add(Vector3.up);
				WaterVerts.Add(new Vector3(x,wY,y+1));
				WaterUVs.Add(VCode3);
				WaterNormal.Add(Vector3.up);
				WaterVerts.Add(new Vector3(x+1,wY,y+1));
				WaterUVs.Add(VCode4);
				WaterNormal.Add(Vector3.up);

				WaterTris.Add(c+1);
				WaterTris.Add(c+0);
				WaterTris.Add(c+2);
				WaterTris.Add(c+1);
				WaterTris.Add(c+2);
				WaterTris.Add(c+3);
			}
		}

		for(int y = 0; y < SimulatedChunkSize/BlockSize; y++) {
			for(int x = 0; x < SimulatedChunkSize/BlockSize; x++) {
				c = ((x+y*SimulatedChunkSize/BlockSize)*4);

				nY = HeightMap[x,y];

				xCoord = x*BlockSize+(MainChunkPosition.x*(SimulatedChunkSize));
				yCoord = y*BlockSize+(MainChunkPosition.y*(SimulatedChunkSize));

				//Was GetTextureTypeAtPixel(xCoord,yCoord,nY)

				Vertices[ver++] = (new Vector3(x,nY,y));
				VerticesUVs[uv++] = ComplexTextureInfoToUV(GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY),0,VCode1);
				Vertices[ver++] = (new Vector3(x+1,nY,y));
				VerticesUVs[uv++] = ComplexTextureInfoToUV(GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY),0,VCode2);
				Vertices[ver++] = (new Vector3(x,nY,y+1));
				VerticesUVs[uv++] = ComplexTextureInfoToUV(GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY),0,VCode3);
				Vertices[ver++] = (new Vector3(x+1,nY,y+1));
				VerticesUVs[uv++] = ComplexTextureInfoToUV(GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY),0,VCode4);

				Triangles[tri] = (c+1);
				tri++;
				Triangles[tri] = (c+0);
				tri++;
				Triangles[tri] = (c+2);
				tri++;

				Triangles[tri] = (c+1);
				tri++;
				Triangles[tri] = (c+2);
				tri++;
				Triangles[tri] = (c+3);
				tri++;
			}
		}

		v = ver;
		c2 = v;
		for(int y = 0; y < SimulatedChunkSize/BlockSize; y++) {
			for(int x = 0; x < SimulatedChunkSize/BlockSize; x++) {
				c = 0;

				nX = HeightMapX[x,y];
				nY = HeightMap[x,y];

				xCoord = x*BlockSize+(MainChunkPosition.x*(SimulatedChunkSize));
				yCoord = y*BlockSize+(MainChunkPosition.y*(SimulatedChunkSize));

				xPos = x*BlockSize+(MainChunkPosition.x*SimulatedChunkSize);
				yPos = y*BlockSize+(MainChunkPosition.y*SimulatedChunkSize);

				neg = 1;
				if(nX-nY < 0) {
					neg = -1;
				}

				if(neg == 1) {
					xWallHeight[x,y] = Mathf.FloorToInt(nY-nX);
				} else {
					xWallHeight[x,y] = Mathf.CeilToInt(nY-nX);
				}

				for(int i = 0; i < Mathf.Abs(xWallHeight[x,y]); i++) {
					SubWorldTexture TextureT;
					if(nX-nY < 0) {
						TextureT = GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY);
					} else {
						TextureT = GetSubWorldTexture(Mathf.FloorToInt(xCoord)+1,Mathf.FloorToInt(yCoord),nX);
					}

					int tp = i+1;

					c = ver;

					if(neg == 1) {
						Vertices[ver++] = (new Vector3(x+1,nX-i,y));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode1);
						Vertices[ver++] = (new Vector3(x+1,nX-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode2);
						Vertices[ver++] = (new Vector3(x+1,nX-i-1,y));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode3);
						Vertices[ver++] = (new Vector3(x+1,nX-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode4);

						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+1);
						tri++;
						Triangles[tri] = (c+0);
						tri++;

						Triangles[tri] = (c+2);
						tri++;
						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+0);
						tri++;
					} else {
						Vertices[ver++] = (new Vector3(x+1,nY-i,y));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode1);
						Vertices[ver++] = (new Vector3(x+1,nY-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode2);
						Vertices[ver++] = (new Vector3(x+1,nY-i-1,y));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode3);
						Vertices[ver++] = (new Vector3(x+1,nY-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode4);

						Triangles[tri] = (c+0);
						tri++;
						Triangles[tri] = (c+1);
						tri++;
						Triangles[tri] = (c+3);
						tri++;

						Triangles[tri] = (c+0);
						tri++;
						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+2);
						tri++;
					}
				}

				c2+=yWallHeight[x,y]*4;
			}
		}

		v = ver;
		c2 = v;
		for(int y = 0; y < SimulatedChunkSize/BlockSize; y++) {
			for(int x = 0; x < SimulatedChunkSize/BlockSize; x++) {
				c = 0;

				nY = HeightMap[x,y];
				nZ = HeightMapZ[x,y];

				xCoord = x*BlockSize+(MainChunkPosition.x*(SimulatedChunkSize));
				yCoord = y*BlockSize+(MainChunkPosition.y*(SimulatedChunkSize));

				xPos = x*BlockSize+(MainChunkPosition.x*SimulatedChunkSize);
				yPos = y*BlockSize+(MainChunkPosition.y*SimulatedChunkSize);

				neg = 1;
				if(nZ-nY < 0) {
					neg = -1;
				}

				if(neg == 1) {
					yWallHeight[x,y] = Mathf.FloorToInt(nY-nZ);
				} else {
					yWallHeight[x,y] = Mathf.CeilToInt(nY-nZ);
				}

				for(int i = 0; i < Mathf.Abs(yWallHeight[x,y]); i++) {
					SubWorldTexture TextureT;
					if(nZ-nY < 0) {
						TextureT = GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord),nY);
					} else {
						TextureT = GetSubWorldTexture(Mathf.FloorToInt(xCoord),Mathf.FloorToInt(yCoord)+1,nZ);
					}

					int tp = i+1;

					c = ver;

					if(neg == 1) {
						Vertices[ver++] = (new Vector3(x,nZ-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode1);
						Vertices[ver++] = (new Vector3(x+1,nZ-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode2);
						Vertices[ver++] = (new Vector3(x,nZ-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode3);
						Vertices[ver++] = (new Vector3(x+1,nZ-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode4);

						Triangles[tri] = (c+0);
						tri++;
						Triangles[tri] = (c+1);
						tri++;
						Triangles[tri] = (c+3);
						tri++;

						Triangles[tri] = (c+0);
						tri++;
						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+2);
						tri++;
					} else {
						Vertices[ver++] = (new Vector3(x,nY-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode1);
						Vertices[ver++] = (new Vector3(x+1,nY-i,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode2);
						Vertices[ver++] = (new Vector3(x,nY-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode3);
						Vertices[ver++] = (new Vector3(x+1,nY-i-1,y+1));
						VerticesUVs[uv++] = ComplexTextureInfoToUV(TextureT,tp,VCode4);

						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+1);
						tri++;
						Triangles[tri] = (c+0);
						tri++;

						Triangles[tri] = (c+2);
						tri++;
						Triangles[tri] = (c+3);
						tri++;
						Triangles[tri] = (c+0);
						tri++;
					}

				}

				c2+=yWallHeight[x,y]*4;
			}
		}

		Vector3[] oldVerts = Vertices;
		triangles = Triangles;
		vertices = new Vector3[triangles.Length];

		for(int i = 0; i < triangles.Length; i++) {
			vertices[i] = oldVerts[triangles[i]];
			UVs[i] = VerticesUVs[triangles[i]];
			triangles[i] = i;
		}

		IsPreparing = false;
	}

	ChunkOcclusion co;

	public void GenerateWorldObject () {
		if(mf == null || mc == null || ChunkObject == null) {
			GameObject cChunk = (GameObject)GameObject.Instantiate(MeshTemplate,new Vector3(MainChunkPosition.x*SimulatedChunkSize,0,MainChunkPosition.y*SimulatedChunkSize),Quaternion.identity);

			co = cChunk.GetComponent<ChunkOcclusion>();
			//co.Player = Camera.main.transform;
			co.OverallWaterHeight = worldM.GetWaterHeight(MainChunkPosition.x*SimulatedChunkSize,MainChunkPosition.y*SimulatedChunkSize)+8;
			//co.LowestChunkHeight = LowestY;
			co.UpdateWater();
			//co.NoOcclusion = (BlockSize >= 8);

			ChunkTransform = cChunk.transform;
			ChunkTransform.localScale = new Vector3(BlockSize,BlockSize,BlockSize);

			cMesh = new Mesh();
			cMesh.vertices = vertices;
			cMesh.triangles = triangles;
			cMesh.uv = UVs;

			cMesh.RecalculateNormals();
			cMesh.RecalculateBounds();

			mf = cChunk.GetComponent<MeshFilter>();
			mf.mesh = cMesh;

			mc = cChunk.GetComponent<MeshCollider>();
			if(GenerateCollider) {
				pMesh = new Mesh();
				pMesh.SetVertices(physXverts);
				pMesh.SetTriangles(physXtris,0);

				pMesh.RecalculateBounds();

				mc.sharedMesh = pMesh;
			}

			mw = cChunk.transform.GetChild(1).GetComponent<MeshFilter>();
			cChunk.transform.GetChild(1).GetComponent<MeshRenderer>().enabled = true;
			mw.transform.localScale = new Vector3((float)WaterBlockSize/BlockSize,(float)WaterBlockSize/BlockSize,(float)WaterBlockSize/BlockSize);
			wMesh = new Mesh();
			wMesh.SetVertices(WaterVerts);
			wMesh.SetTriangles(WaterTris,0);
			wMesh.SetUVs(0,WaterUVs);
			wMesh.SetNormals(WaterNormal);
			mw.mesh = wMesh;

			ChunkObject = cChunk;
		} else {
			if(Token) {
				ChunkTransform.position = TokenPos;
				ChunkTransform.localScale = TokenSize;

				Token = false;
			}

			co.OverallWaterHeight = worldM.GetWaterHeight(MainChunkPosition.x*SimulatedChunkSize,MainChunkPosition.y*SimulatedChunkSize)+8;
			//co.NoOcclusion = (BlockSize >= 8);
			co.LowestChunkHeight = LowestY;
			co.UpdateWater();
			//co.ChangeRenderersStatus(true);

			cMesh.Clear();

			cMesh.vertices = vertices;
			cMesh.triangles = triangles;
			cMesh.uv = UVs;

			cMesh.RecalculateNormals();
			cMesh.RecalculateBounds();

			mf.mesh = cMesh;
			if(GenerateCollider) {
				pMesh.Clear();
				pMesh.SetVertices(physXverts);
				pMesh.SetTriangles(physXtris,0);

				pMesh.RecalculateBounds();

				mc.sharedMesh = pMesh;
			}

			mw.transform.localScale = new Vector3(WaterBlockSize/BlockSize,WaterBlockSize/BlockSize,WaterBlockSize/BlockSize);
			wMesh.Clear();
			wMesh.SetVertices(WaterVerts);
			wMesh.SetTriangles(WaterTris,0);
			wMesh.SetUVs(0,WaterUVs);
			wMesh.SetNormals(WaterNormal);
			mw.mesh = wMesh;
		}
		GenerateTerrainModels();
	}

	//Texture Placement Rules:
	// 0 : Top
	// 1 : UpperWall
	// 2 : LowerWall
	// 3 : UnderWall
	Vector2 Results = Vector2.zero;
	int texturePl;

	Vector2 ComplexTextureInfoToUV (SubWorldTexture swt, int texturePlacement, Vector2 verticesUV) {
		
		if(texturePlacement >= swt.Coords.Length) {
			texturePl = swt.Coords.Length-swt.RepeatRange+(int)Mathf.Repeat(texturePlacement,swt.RepeatRange);
		} else {
			texturePl = Mathf.Clamp(texturePlacement,0,swt.Coords.Length-1);
		}

		if(verticesUV.x == 0) {
			Results.x = swt.Coords[texturePl].x*16f;
		} else {
			Results.x = swt.Coords[texturePl].x*16f+15f;
		}
		if(verticesUV.y == 0) {
			Results.y = swt.Coords[texturePl].y*16f;
		} else {
			Results.y = swt.Coords[texturePl].y*16f+15f;
		}

		return V2ToUV(Results);
	}

	int GetTextureTypeAtPixel (float x, float y, float Height) {
		return worldM.GetTextureTypeAtPixel(x,y,BlockSize,Height);
	}

	public static float GetValueAtPixelParams (float x, float y, WorldManager wm) {
		return wm.GetHeightMap(x,y)[1];
	}

	float GetValueAtPixel (float x, float y) {
		return GetValueAtPixelParams(x,y,worldM);
	}

	int gr = 0;
	int WorldTexture = 0;

	SubWorldTexture GetSubWorldTexture (int x, int y, float Height) {
		return worldM.creator.loader.subWorldTextures[worldM.GetTextureTypeAtPixel(x,y,BlockSize,Height)];
		/*WorldTexture = worldM.GetTextureTypeAtPixel(x,y,BlockSize,Height);
		gr=-1;
		for(int t = worldTextures[WorldTexture].TextureGroups.Length-1; t >= 0; t--) {
			if(worldTextures[WorldTexture].TextureGroups[t].MaxHeight > Height*BlockSize) {
				gr = t;
			}
		}
		if(gr==-1) {
			return worldTextures[WorldTexture].MainTexture;
		}
		if(worldTextures[WorldTexture].TextureGroups[gr].patchs.EnablePatchModule) {
			if(worldTextures[WorldTexture].TextureGroups[gr].patchs.SeperatePatchs) {
				if(Mathf.SmoothStep(0f,1f,Mathf.PerlinNoise(x*worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchScale,y*worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchScale))>worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchThreshold) {
					return worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchTexture;
				} else {
					return worldTextures[WorldTexture].TextureGroups[gr].MainGroupTexture;
				}
			} else {
				if(Mathf.PerlinNoise(x*worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchScale,y*worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchScale)>worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchThreshold) {
					return worldTextures[WorldTexture].TextureGroups[gr].patchs.PatchTexture;
				} else {
					return worldTextures[WorldTexture].TextureGroups[gr].MainGroupTexture;
				}
			}
		} else {
			return worldTextures[WorldTexture].TextureGroups[gr].MainGroupTexture;
		}*/
	}

	Vector2 vcalcule = Vector2.zero;

	Vector2 PixelToUV (int x, int y) {
		vcalcule.x = 0;
		vcalcule.y = 0;

		vcalcule.x = (float)x*0.001953125f; //0.001953125 = 1/512, 512 = size of the texture
		vcalcule.y = 1-((float)y*0.001953125f); //0.001953125 = 1/512, 512 = size of the texture
		return vcalcule;
	}

	Vector2 V2ToUV (Vector2 v) {
		vcalcule.x = 0;
		vcalcule.y = 0;

		vcalcule.x = (float)v.x*0.001953125f; //0.001953125 = 1/512, 512 = size of the texture
		vcalcule.y = 1-((float)v.y*0.001953125f); //0.001953125 = 1/512, 512 = size of the texture
		return vcalcule;
	}

	bool IsInBound (int v, int bSize, int index) {
		return (v-(Mathf.Floor((float)v/bSize)*4))==index;
	}
}
