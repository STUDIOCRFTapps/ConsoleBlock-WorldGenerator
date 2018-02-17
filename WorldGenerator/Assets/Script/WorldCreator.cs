using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCreator : MonoBehaviour {

	public Material DisplayMaterial;
	Texture2D texture;
	public Renderer renderer;
	public Gradient gradient;

	public float WorldCreationRatio = 32;

	public int WorldReferenceSize = 1024;

	public int Seed = 0;
	public Vector2 ReferenceStartPosition = Vector2.zero;
	public float DefaultReferenceFrequency = 0.05f;
	public int ReferenceOctaves = 3;
	public float ReferenceFrequencyIncrementation = 1.2f;
	public float ReferenceAmplificationIncrementation = 0.9f;

	public bool BakeData = true;

	public WorldLoader loader;

	public AnimationCurve ReferenceFalloffX;
	public AnimationCurve ReferenceFalloffY;
	public float FalloffInfluence = 0.1f;

	float[,] TemperatureMap;
	float[,] HumidityMap;
	float[,] HeightMap;
	//float[,] InsideHole;

	System.Random prng;

	bool t = true;

	void Start () {
		
	}

	void Update () {
		/*if(Input.GetKeyDown(KeyCode.Return)) {
			Execute();
		}*/
		if(t) {
			if(DisplayMaterial != null) {
				Color[] col = new Color[loader.biomes.Length];
				for(int i = 0; i < loader.biomes.Length; i++) {
					col[i] = Color.HSVToRGB((float)i/loader.biomes.Length,1f,1f);
				}

				BiomeInfoList bif;
				for(int x = 0; x < WorldReferenceSize; x++) {
					for(int y = 0; y < WorldReferenceSize; y++) {
						bif = loader.worldManager.GetBiomesAt(x*(int)WorldCreationRatio,y*(int)WorldCreationRatio,1);
						int b = bif.infoList[0];
						if(bif.infoList.Count <= 1) {
							texture.SetPixel(x,y,Color.white);
						} else {
							texture.SetPixel(x,y,new Color(col[b].r*(1f/bif.infoList.Count),col[b].b*(1f/bif.infoList.Count),col[b].g*(1f/bif.infoList.Count)));
						}
						//texture.SetPixel(x,y,gradient.Evaluate(Mathf.Clamp(HumidityMap[x,y],0f,1f)));
					}
				}
				texture.Apply();
			}
			t = false;
		}
	}

	public void Execute () {
		if(Seed == 0) {
			Seed = Random.Range(-10000,10000);
		}
		prng = new System.Random(Seed);

		texture = new Texture2D(WorldReferenceSize,WorldReferenceSize);
		texture.filterMode = FilterMode.Point;
		if(DisplayMaterial != null) {
			DisplayMaterial.mainTexture = texture;
			renderer.material = DisplayMaterial;
		}

		TemperatureMap = new float[WorldReferenceSize,WorldReferenceSize];
		//InsideHolde = new float[WorldReferenceSize,WorldReferenceSize];

		HeightMap = GenerateNoiseMap(WorldReferenceSize,WorldReferenceSize,Seed,DefaultReferenceFrequency,ReferenceOctaves,ReferenceFrequencyIncrementation,ReferenceAmplificationIncrementation,ReferenceStartPosition);
		HumidityMap = HeightMap;

		//Debug.Log(HumidityMap[WorldReferenceSize/2,WorldReferenceSize/2]);

		/*for(int x = 0; x < WorldReferenceSize; x++) {
			for(int y = 0; y < WorldReferenceSize; y++) {
				HeightMap[x,y] *= (ReferenceFalloffX.Evaluate(1f/WorldReferenceSize*x)*ReferenceFalloffY.Evaluate(1f/WorldReferenceSize*y)*FalloffInfluence);
				HeightMap[x,y] *= 0.8f;
				TemperatureMap[x,y] = Mathf.Clamp01(Mathf.Lerp(HeightMap[x,y],ReferenceFalloffY.Evaluate(1f/WorldReferenceSize*y),1.0f));
				//HumidityMap[x,y] /= 9f;
			}
		}*/

		for(int x = 0; x < WorldReferenceSize; x++) {
			for(int y = 0; y < WorldReferenceSize; y++) {
				TemperatureMap[x,y] = Mathf.Clamp01((ReferenceFalloffX.Evaluate((float)x/WorldReferenceSize)*0.95f)+(HeightMap[x,y]*0.15f));
				HumidityMap[x,y] = Mathf.Clamp01((ReferenceFalloffY.Evaluate((float)y/WorldReferenceSize)*0.95f)+(HeightMap[x,y]*0.15f));
			}
		}
	}

	public float GetTemperature(float xv, float yv) { //Get More Precistion by using float value
		float x = xv/WorldCreationRatio;
		float y = yv/WorldCreationRatio;

		float abu = Mathf.Lerp(TemperatureMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],TemperatureMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));
		float cdu = Mathf.Lerp(TemperatureMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],TemperatureMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));

		//Value += Mathf.Lerp(TemperatureMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],TemperatureMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		//Value += Mathf.Lerp(TemperatureMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],TemperatureMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		return Mathf.Clamp01(Mathf.Lerp(abu,cdu, y-Mathf.Floor(y)));
	}

	public float GetHumidity(float xv, float yv) { //Get More Precistion by using float value
		float x = xv/WorldCreationRatio;
		float y = yv/WorldCreationRatio;

		float abu = Mathf.Lerp(HumidityMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HumidityMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));
		float cdu = Mathf.Lerp(HumidityMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],HumidityMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));

		//Value += Mathf.Lerp(HumidityMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HumidityMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		//Value += Mathf.Lerp(HumidityMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HumidityMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		return Mathf.Clamp01(Mathf.Lerp(abu,cdu, y-Mathf.Floor(y)));
		//return HumidityMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)];
	}

	public float GetHeight(float xv, float yv) { //Get More Precistion by using float value
		float x = xv/WorldCreationRatio;
		float y = yv/WorldCreationRatio;

		float abu = Mathf.Lerp(HeightMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HeightMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));
		float cdu = Mathf.Lerp(HeightMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],HeightMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],x-Mathf.Floor(x));

		//Value += Mathf.Lerp(HeightMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HeightMap[Mathf.Clamp(Mathf.FloorToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		//Value += Mathf.Lerp(HeightMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.FloorToInt(y),0,WorldReferenceSize-1)],HeightMap[Mathf.Clamp(Mathf.CeilToInt(x),0,WorldReferenceSize-1),Mathf.Clamp(Mathf.CeilToInt(y),0,WorldReferenceSize-1)],y-Mathf.Floor(y));
		return Mathf.Clamp01(Mathf.Lerp(abu,cdu, y-Mathf.Floor(y)));
	}

	//	a	 b
	//
	//	c	 d

	public float QuadLerp(float a, float b, float c, float d, Vector2 middle) {
		float abu = Mathf.Lerp(a, b, middle.x);
		float cdu = Mathf.Lerp(c, d, middle.x);
		return Mathf.Lerp(abu, cdu, middle.y);
	}

	public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		LibNoise.Unity.Generator.RiggedMultifractal Noise = new LibNoise.Unity.Generator.RiggedMultifractal(scale,lacunarity,octaves,seed,LibNoise.Unity.QualityMode.High);
		float[,] noiseMap = new float[mapWidth,mapHeight];

		System.Random prng = new System.Random(seed);

		for(int y = 0; y < mapHeight; y++) {
			for(int x = 0; x < mapWidth; x++) {
				noiseMap[x,y] = (float)Noise.GetValue((float)x,0f,(float)y);
				//noiseMap[x,y] = noiseMap[x,y];
			}
		}

		return noiseMap;
	}
		
}