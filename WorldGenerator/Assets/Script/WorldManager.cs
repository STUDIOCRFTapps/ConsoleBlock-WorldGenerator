using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using OTNM;
using OTNM.Tools;
using FastNoiseLibrary;

//Manage the world by giving height (noise) and block (id) information, creating noise, giving water height information.
public class WorldManager {

	public float BiomeTransitionSmoothing = 0.00185f;
	FastNoise fn;

	public float Multiplicator = 4.0f;
	public float Divisor =  8.0f;
	public float Additive = 1.0f;

	public string NaNError = "";
	public float[] DebugNumbers = new float[3];

	public void Apply (float M, float D, float A) {
		Multiplicator = M;
		Divisor = D;
		Additive = A;
	}

	//Reference to the world creator
	public WorldCreator creator;

	//Reference to the biome creator
	public Biome[] biomes;
	public SubBiome[] subBiomes;

	//Dark things, do not mess with
	PrebakedBiomeData BiomeFinderC = new PrebakedBiomeData();

	PrebakedBiomeData ReadBakedInformation () {
		if(File.Exists(Application.persistentDataPath + "/res/bakedbiomedata.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = new FileStream(Application.persistentDataPath + "/res/bakedbiomedata.dat", FileMode.Open);

			PrebakedBiomeData res = (PrebakedBiomeData)bf.Deserialize(file);
			file.Close();

			return res;
		} else {
			return new PrebakedBiomeData();
		}
	}

	void WriteBakedInformation (PrebakedBiomeData d) {
		BinaryFormatter bf = new BinaryFormatter();
		Directory.CreateDirectory(Application.persistentDataPath + "/res/");
		FileStream file = new FileStream(Application.persistentDataPath + "/res/bakedbiomedata.dat", FileMode.Create);

		bf.Serialize(file,d);
		file.Close();
	}

	//Lists of noise and the corresponding Biome Id (aka Index)
	TerrainNoiseGroup noiseGroup = new TerrainNoiseGroup();
	int MainBiomeNoiseLength = 0;

	//Constructor
	public WorldManager(WorldCreator worldCreator, Biome[] biomesBlueprint, SubBiome[] subBiomesBlueprint) {

		//Store constructor parameters onto reference
		biomes = biomesBlueprint;
		subBiomes = subBiomesBlueprint;
		creator = worldCreator;
		fn = new FastNoise(0); //NEED A SEED

		//Directory.CreateDirectory(Application.persistentDataPath + "/res/");
		//File.WriteAllText(Application.persistentDataPath + "/res/bakedbiomedata.dat","HEya?");

		if(creator.BakeData) {
			BiomeFinderC.BiomeFinder = new BiomeInfoList[biomes.Length*8,biomes.Length*8];
			for(int x = 0; x < biomes.Length*8; x++) {
				for(int y = 0; y < biomes.Length*8; y++) {
					float CT = (float)x/(biomes.Length*8);
					float CH = (float)y/(biomes.Length*8);

					float BCurrentValue = 0f;

					float Temperature = 0f;
					float Humidity = 0f;

					float BClosestValue = Mathf.Infinity;
					int BClosestId = 0;

					List<float> ranks = new List<float>(){Mathf.Infinity,Mathf.Infinity,Mathf.Infinity};
					List<int> ranksID = new List<int>(){0,0,0};

					int AimAt = 2;
					int idv;
					float dv;

					//Find a list with the 3 closest biome
					//If a new value enter, the 4th one should be deleted, if it exist

					//Foreach biomes...
					for(int b = 0; b < biomes.Length; b++) {

						//Analize how similar the biome is to the environnement
						Temperature = Distance(biomes[b].BiomeRequiredTemperature*0.01f,CT);
						Humidity = Distance(biomes[b].BiomeRequiredHumidity*0.01f,CH);
						BCurrentValue = Mathf.Lerp(Temperature,Humidity,0.5f)*(biomes[b].BiomeRarity*0.01f);

						AimAt = -1;
						idv = 0;
						dv = 0f;

						for(int i = 2; i >= 0; i--) {
							if(ranks.Count > i) {
								if(ranks[i] >= BCurrentValue) {
									AimAt = i;
									idv = b;
									dv = BCurrentValue;
								}
							} else {
								AimAt = i;
								idv = b;
								dv = BCurrentValue;
							}
						}

						if(AimAt != -1) {
							ranks.Insert(Mathf.Clamp(AimAt,0,ranksID.Count),dv);
							ranksID.Insert(Mathf.Clamp(AimAt,0,ranksID.Count),idv);
						}

						if(ranks.Count >= 4) {
							ranks.RemoveAt(3);
						}
						if(ranksID.Count >= 4) {
							ranksID.RemoveAt(3);
						}

						/*if(BClosestValue > BCurrentValue) {
							BClosestValue = BCurrentValue;
							BClosestId = b;
						}*/
					}

					for(int v = 0; v < ranks.Count; v++) {
						if(Distance(ranks[0],ranks[v]) > BiomeTransitionSmoothing+0.05f/*0.0073f*/) {
							for(int v2 = v; v2 < 3; v2++) {
								ranks.RemoveAt(ranks.Count-1);
								ranksID.RemoveAt(ranksID.Count-1);
							}
							break;
						}
					}

					BiomeFinderC.BiomeFinder[x,y] = new BiomeInfoList();
					for(int d = 0; d < ranksID.Count; d++) {
						BiomeFinderC.BiomeFinder[x,y].infoList.Add(ranksID[d]);
					}
					//BiomeFinderC.BiomeFinder[x,y] = 
					//BiomeFinderC.BiomeFinder[x,y].infoList.Add(new BiomeInfo(BClosestId,1f));

					//NOTE: The total of the forces should be equal to 1,
					//the biome should be classed by force (0 should be the biome with the most force)


				}
			}
			WriteBakedInformation(BiomeFinderC);
			//File.WriteAllBytes(Application.dataPath + "/res/bakedbiomedata.dat", ec.ToArray());
			//creator.prebakedBData.BiomeFinder = BiomeFinderC;
		} else {
			BiomeFinderC = ReadBakedInformation();
			//BiomeFinderC = creator.prebakedBData.BiomeFinder;
		}

		//Prepare noise (from the libnoise library) and asing each noise with its correnspending Biome Id
		for(int b = 0; b < biomes.Length; b++) {
			noiseGroup.Add(new TerrainNoise(biomes[b].terrainType,worldCreator.Seed,new Accessing.NoiseParameters(biomes[b].NoiseOctaves+2,biomes[b].NoiseLacunarity,biomes[b].NoisePersistence),biomes[b].CanyonParameters,biomes[b].Frequency));
			MainBiomeNoiseLength = b+1;
		}
		for(int b = 0; b < subBiomes.Length; b++) {
			noiseGroup.Add(new TerrainNoise(subBiomes[b].terrainType,worldCreator.Seed,new Accessing.NoiseParameters(subBiomes[b].NoiseOctaves+2,subBiomes[b].NoiseLacunarity,subBiomes[b].NoisePersistence),subBiomes[b].CanyonParameters,subBiomes[b].Frequency));
		}
	}

	//Prevent Thread Issues
	private System.Object biomeLock = new System.Object();

	// Get the biome corresponding to a certain pos.
	public int GetBiomeAt (float xv, float yv, int blockSize) {
		lock(biomeLock) {
			float x = Mathf.Floor(xv);
			float y = Mathf.Floor(yv);

			int h = (int)(creator.GetHumidity(x,y) * biomes.Length*8);
			int t = (int)(creator.GetTemperature(x,y) * biomes.Length*8);

			//À fixer ^

			//Return the Id of the biome
			return BiomeFinderC.BiomeFinder[(int)Mathf.Clamp(t,0,biomes.Length*8-1),(int)Mathf.Clamp(h,0,biomes.Length*8-1)].infoList[0];
		}
	}

	//Prevent Thread Issues
	private System.Object biomesLock = new System.Object();

	// Get the biomes percentage corresponding to a certain pos.
	public BiomeInfoList GetBiomesAt (float xv, float yv, int blockSize) {
		lock(biomesLock) {
			float x = Mathf.Floor(xv);
			float y = Mathf.Floor(yv);

			int h = (int)(creator.GetHumidity(x,y) * biomes.Length*8);
			int t = (int)(creator.GetTemperature(x,y) * biomes.Length*8);

			//TODO: The biome finder list returns the list of biome to analize. Return the good proportion please :D

			return BiomeFinderC.BiomeFinder[(int)Mathf.Clamp(t,0,biomes.Length*8-1),(int)Mathf.Clamp(h,0,biomes.Length*8-1)];
		}
	}

	//Math Function
	public float Distance (float a, float b) {
		//Note: 1.1 is curve speed,  higher distance will be amplified
		return Mathf.Abs(a-b);
	}

	//Math Function
	public float QuadLerp(float a, float b, float c, float d, Vector2 middle) {
		float abu = Mathf.Lerp(a, b, middle.x);
		float cdu = Mathf.Lerp(c, d, middle.x);
		return Mathf.Lerp(abu, cdu, middle.y);
	}

	//Math Function
	public float SimpleQuadLerp(float a, float b, float c, float d) {
		float abu = Mathf.Lerp(a, b, 0.5f);
		float cdu = Mathf.Lerp(c, d, 0.5f);
		return Mathf.Lerp(abu, cdu, 0.5f);
	}

	//Math Function
	float bilinearInterpolation(float bottomLeft, float topLeft, float bottomRight, float topRight, float xMin, float xMax, float zMin, float zMax, float x, float z) {
		float width = xMax - xMin;
		float height = zMax - zMin;

		float xDistanceToMaxValue = xMax - x;
		float zDistanceToMaxValue = zMax - z;

		float xDistanceToMinValue = x - xMin;
		float zDistanceToMinValue = z - zMin;

		return 1.0f / (width * height) * (
			bottomLeft * xDistanceToMaxValue * zDistanceToMaxValue +
			bottomRight * xDistanceToMinValue * zDistanceToMaxValue +
			topLeft * xDistanceToMaxValue * zDistanceToMinValue +
			topRight * xDistanceToMinValue * zDistanceToMinValue
		);
	}

	//Declaring a lock to prevent thread issues
	private System.Object thisLock = new System.Object();
	private System.Object rawLock = new System.Object();

	int Quality = 4;

	//Get the height at a certain position 
	public float[] GetHeightMap(float x, float y) {

		//Lock some code to prevent Thread Issues.
		lock(thisLock) {

			float[] temp = new float[2];
			List<int> bil;
			if(!creator.loader.DebuggingMode) {
				bil = GetBiomesAt(x,y,1).infoList;
				NaNError = biomes[bil[0]].name;
			} else {
				//Add min height

				float DistorsionX = Mathf.Lerp(-0.0003f, 0.0003f, Mathf.PerlinNoise(x*0.0034f,y*0.0034f));
				float DistorsionY = Mathf.Lerp(-0.0003f, 0.0003f, Mathf.PerlinNoise(x*0.0034f,y*0.0034f));
				float Distorsion2X = Mathf.Lerp(-0.0001f, 0.0001f, Mathf.PerlinNoise(x*0.00212f+DistorsionX,y*0.00212f+DistorsionY));
				float Distorsion2Y = Mathf.Lerp(-0.0001f, 0.0001f, Mathf.PerlinNoise(x*0.00212f+DistorsionX,y*0.00212f+DistorsionY));
				float Frequency = Mathf.Lerp(0.004f+Distorsion2X, 0.004f+Distorsion2Y, Mathf.PerlinNoise(x*0.00167f,y*0.00167f));

				float Amplitude = Mathf.Lerp(10, 1800, Mathf.PerlinNoise(x*0.00001f,y*0.00001f));
				float Lacunarity = Mathf.Lerp(5f, 10f, Mathf.PerlinNoise(x*0.00024f,y*0.00024f));

				float AltitudeErosion = Mathf.Lerp(0.05f, 0.95f, Mathf.PerlinNoise(x*0.00029f,y*0.00029f));
				float RidgeErosion = Mathf.Lerp(0.8f, 1.2f, Mathf.PerlinNoise(x*0.00031f,y*0.00031f));
				float SlopeErosion = Mathf.Lerp(0.2f, 1.2f, Mathf.PerlinNoise(x*0.00032f,y*0.00032f));

				float Gain = Mathf.Lerp(0.063f, 0.25f, Mathf.PerlinNoise(x*0.00028f,y*0.00028f));

				float Sharpness = Mathf.Lerp(0f, 1f, Mathf.SmoothStep(0,1f,Mathf.PerlinNoise(x*0.00005f,y*0.00005f)));
				float FeatureAmplifier = Mathf.Lerp(0f, 0.09f, Mathf.PerlinNoise(x*0.0043f,y*0.0043f));

				float n = OTNM.Tools.Accessing.GetUberNoise(new XnaGeometry.Vector2(x*Frequency,y*Frequency),fn,0,7,Sharpness,FeatureAmplifier,AltitudeErosion,RidgeErosion,SlopeErosion,Lacunarity,Gain)*Amplitude;
				//XnaGeometry.Vector3 n = OTNM.Tools.Accessing.GetFractalNoiseWType(new XnaGeometry.Vector2(x*Frequency,y*Frequency),new Accessing.NoiseParameters(5,Lacunarity,Gain),fn,NoiseTypeValue)*Amplitude;


				/*float hillMinHeight = Mathf.Lerp(10, 90, Mathf.PerlinNoise(x*0.00013f,y*0.00013f));//Amplitude*0.75f;
				if(temp[0] > hillMinHeight) {
					float hillMaxHeight = 50;//Amplitude*0.75f+5f;
					float hillHeight = Mathf.Lerp(1f, 90f, Mathf.PerlinNoise(x*0.015f,y*0.015f));

					if(temp[0] > hillMinHeight+hillMaxHeight) {
						temp[0] = temp[0]+hillHeight;
					} else {
						temp[0] = Mathf.SmoothStep(hillMinHeight,hillMinHeight+hillMaxHeight+hillHeight,Mathf.InverseLerp(hillMinHeight,hillMinHeight+hillMaxHeight,temp[0]));
					}
				}*/
				temp[0] = Mathf.Lerp(-700, 400, Mathf.PerlinNoise(x*0.000093f,y*0.000093f))+n;
				temp[1] = 0;

				//temp = GetRawHeightMapBiomes(x,y,114);
				//temp[0] = (temp[0]-Mathf.Repeat(temp[0],biomes[114].CanyonStepHeight))+(biomes[114].CanyonCurve.Evaluate(Mathf.Repeat(temp[0],biomes[114].CanyonStepHeight)/biomes[114].CanyonStepHeight)*biomes[114].CanyonStepHeight);
				return temp;
			}

			float BCurrentValue = 0f;

			float Temperature = 0f;
			float Humidity = 0f;
			//NaNError = biomes[GetBiomeAt(x,y,1)].name + " at " + creator.GetTemperature(x,y) + "°W, " + creator.GetHumidity(x,y) + "%";

			List<float> res = new List<float>();
			float total = 0f;

			//Find the GOOD order of the biome
			//List<int> results = new List<int>();
			//List<float> vresults = new List<float>();

			//round this to the (biomes.Length*8) x/y center

			for(int i = 0; i < bil.Count; i++) {

				Temperature = Distance(biomes[bil[i]].BiomeRequiredTemperature*0.01f,creator.GetTemperature(x,y));
				Humidity = Distance(biomes[bil[i]].BiomeRequiredHumidity*0.01f,creator.GetHumidity(x,y));
				BCurrentValue = (Temperature+Humidity)*(biomes[bil[i]].BiomeRarity*0.01f)/2f;
				res.Add(BCurrentValue);

				/*int aimat = 0;
				for(int r = Mathf.Min(results.Count-1,0); r >= 0; r--) {
					if(res[i] < vresults[r] || results.Count-1 < 0) {
						aimat = r;
					}
				}
				results.Insert(aimat, i);
				vresults.Insert(aimat, res[i]);*/
			}

			/*List<int> bilsv = new List<int>();
			List<float> ressv = new List<float>();

			for(int i = 0; i < results.Count; i++) {
				bilsv.Add(bil[results[i]]);
				ressv.Add(res[results[i]]);
			}

			bil = bilsv;
			res = ressv;*/

			if(bil.Count == 1) {
				temp[0] = (float)noiseGroup.GetNoiseValue(bil[0],new XnaGeometry.Vector2(x,y))*biomes[bil[0]].NoiseAmplitude + (biomes[bil[0]].BiomeMinHeight+biomes[bil[0]].BiomeSecondLifter);

				float t1 = 1f;

				//NaNError = "1";
				DebugNumbers[0] = 1;
				DebugNumbers[1] = 0;
				DebugNumbers[2] = 0;

				total = temp[0]*t1;
			}
			if(bil.Count == 2) {
				temp[0] = (float)noiseGroup.GetNoiseValue(bil[0],new XnaGeometry.Vector2(x,y))*biomes[bil[0]].NoiseAmplitude + (biomes[bil[0]].BiomeMinHeight+biomes[bil[0]].BiomeSecondLifter);

				float t1 = 0f;
				float t2 = 0f;

				if(res[0] < res[1]) {
					t2 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t1 = 1f-t2;
				} else {
					t1 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t2 = 1f-t1;
				}

				//t1 /= 2;
				//t2 /= 2;

				//t1 = (t1*0.5f)+0.5f;
				//t2 = (t2*0.5f)+0.5f;

				//NaNError = t1 + " at " + bil[0] + ", " + t2 + " at " + bil[1];
				//NaNError = (Distance(res[1],res[0])*100f).ToString() + " & " + bil[0].ToString() + ";" + bil[1].ToString();
				DebugNumbers[0] = t1;
				DebugNumbers[1] = t2;
				DebugNumbers[2] = 0;

				total = temp[0]*t1;
				total += ((float)noiseGroup.GetNoiseValue(bil[1],new XnaGeometry.Vector2(x,y))*biomes[bil[1]].NoiseAmplitude+(biomes[bil[1]].BiomeMinHeight+biomes[bil[1]].BiomeSecondLifter))*t2;

				//total = temp[0] * ((1f-Mathf.InverseLerp(0f,0.0053f,Mathf.Abs(res[0]-res[1]))) * 0.5f + 0.5f);
				//total += GetRawHeightMapBiomes(x,y,bil[1],1)[0]*((Mathf.InverseLerp(0f,0.0053f,Mathf.Abs(res[0]-res[1])) * 0.5f + 0.5f));
			}
			if(bil.Count == 3) {
				temp[0] = (float)noiseGroup.GetNoiseValue(bil[0],new XnaGeometry.Vector2(x,y))*biomes[bil[0]].NoiseAmplitude+(biomes[bil[0]].BiomeMinHeight+biomes[bil[0]].BiomeSecondLifter);

				float t1 = 0f;
				float t2 = 0f;
				float t3 = 0f;

				if(res[0] < res[1] && res[1] < res[2]) {
					t2 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t3 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,BiomeTransitionSmoothing))));
					t1 = 1f-(t2+t3);
				} else if(res[0] < res[2] && res[2] < res[1]) {
					t3 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t2 = Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,0.001f/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,0.001f)));
					t1 = 1f-(t2+t3);
				} else if(res[1] < res[0] && res[0] < res[2]) {
					t1 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t3 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,BiomeTransitionSmoothing))));
					t2 = 1f-(t1+t3);
				} else if(res[1] < res[2] && res[2] < res[0]) {
					t1 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t3 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,BiomeTransitionSmoothing))));
					t2 = 1f-(t1+t3);
				} else if(res[2] < res[0] && res[0] < res[1]) {
					t1 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t2 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,BiomeTransitionSmoothing))));
					t3 = 1f-(t1+t2);
				} else if(res[2] < res[1] && res[1] < res[0]) {
					t2 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[1],res[0]),0f,BiomeTransitionSmoothing))));
					t1 = Mathf.SmoothStep(0f,1f,Mathf.Lerp(0.5f,0f,Mathf.InverseLerp(0f,BiomeTransitionSmoothing/*0.0063f*/,Mathf.Clamp(Distance(res[2],res[0]),0f,BiomeTransitionSmoothing))));
					t3 = 1f-(t1+t2);
				}

				//NaNError = t1 + " at " + bil[0] + ", " + t2 + " at " + bil[1] + ", " + t3 + " at " + bil[2];
				DebugNumbers[0] = t1;
				DebugNumbers[1] = t2;
				DebugNumbers[2] = t3;

				total = temp[0]*t1;
				total += (((float)noiseGroup.GetNoiseValue(bil[1],new XnaGeometry.Vector2(x,y))*biomes[bil[1]].NoiseAmplitude+(biomes[bil[1]].BiomeMinHeight)+biomes[bil[1]].BiomeSecondLifter))*t2;
				total += (((float)noiseGroup.GetNoiseValue(bil[2],new XnaGeometry.Vector2(x,y))*biomes[bil[2]].NoiseAmplitude+(biomes[bil[2]].BiomeMinHeight)+biomes[bil[2]].BiomeSecondLifter))*t3;

				//total = temp[0] * ((1f-Mathf.InverseLerp(0f,0.0053f,Mathf.Abs(res[0]-res[1]))) * 0.5f + 0.5f);
				//total += GetRawHeightMapBiomes(x,y,bil[1],1)[0]*((Mathf.InverseLerp(0f,0.0053f,Mathf.Abs(res[0]-res[1])) * 0.5f + 0.5f));
			}

			temp[0] = GetRawHeightMapBiomesModifier(x,y,total,bil[0])[0];//total;
			return new float[2]{temp[0],temp[1]};
		}
	}

	//Get the raw height at a certain position, gets the biome instead of searching it.
	public float[] GetRawHeightMapBiomes(float x, float y, int biome) {
		int CurrentBiome = biome;
		float Height = 0f;
		int blockId = -1;

		//BIOME MIN HEIGHT REMOVED DUE TO TESTING
		Height = ((biomes[CurrentBiome].BiomeMinHeight)+((float)noiseGroup.GetNoiseValue(CurrentBiome,new XnaGeometry.Vector2(x,y))*biomes[CurrentBiome].NoiseAmplitude));

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 0) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseStrechModule) {
			float HighLimit = (biomes[CurrentBiome].StretchMinimumHeight) + Mathf.PerlinNoise(x*0.01f,y*0.01f) * 5f;
			Height = Mathf.Lerp(Height,HighLimit+((Height-HighLimit)*AmpliCurve(Mathf.InverseLerp(HighLimit,biomes[CurrentBiome].StretchMaxHeight,Height),biomes[CurrentBiome].StretchMaxSlopeFactor)),biomes[CurrentBiome].StretchModuleInfluence);
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 1) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseCliffModule) {
			float CliffRange = biomes[CurrentBiome].CliffRange/2f;
			float Limit = biomes[CurrentBiome].CliffBottom - Mathf.PerlinNoise(x*0.01f,y*0.01f)*5f;
			if(Height >= Limit - CliffRange && Height < Limit + CliffRange) {
				float LerpPos = Mathf.InverseLerp(Limit - CliffRange, Limit + CliffRange,Height);
				Height = Mathf.SmoothStep(Height,Height+biomes[CurrentBiome].CliffHeight,LerpPos);
			} else if(Height >= Limit + CliffRange) {
				Height = Height+biomes[CurrentBiome].CliffHeight;
			}
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 2) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseCanyonModule) {
			if(Height > biomes[CurrentBiome].CanyonStartingHeight) {
				float StepHeight = (Height-biomes[CurrentBiome].CanyonStartingHeight)*biomes[CurrentBiome].CanyonStepCurveFactor;
				Height = biomes[CurrentBiome].CanyonStartingHeight + ((Mathf.Floor(StepHeight) + biomes[CurrentBiome].CanyonCurve.Evaluate(Mathf.Repeat(StepHeight,1)))*biomes[CurrentBiome].CanyonStepHeight);
			}
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 3) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		//BIOME MIN HEIGHT REMOVED DUE TO TESTING
		Height += biomes[CurrentBiome].BiomeSecondLifter;
		//Height -= biomes[CurrentBiome].BiomeMinHeight*0.75f;

		if(biomes[CurrentBiome].UseSubBiomeModule) {
			float HeightS = 0f;
			int CurrentSBiome = biomes[CurrentBiome].SubBiomeId;

			//BIOME MIN HEIGHT REMOVED DUE TO TESTING
			HeightS = ((subBiomes[CurrentSBiome].BiomeMinHeight)+((float)noiseGroup.GetNoiseValue(CurrentSBiome+MainBiomeNoiseLength,new XnaGeometry.Vector2(x,y))*subBiomes[CurrentSBiome].NoiseAmplitude));

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 0) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,Height,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseStrechModule) {
				float HighLimit = (subBiomes[CurrentSBiome].StretchMinimumHeight) + Mathf.PerlinNoise(x*0.01f,y*0.01f) * 5f;
				HeightS = Mathf.Lerp(HeightS,HighLimit+((HeightS-HighLimit)*AmpliCurve(Mathf.InverseLerp(HighLimit,subBiomes[CurrentSBiome].StretchMaxHeight,HeightS),subBiomes[CurrentSBiome].StretchMaxSlopeFactor)),subBiomes[CurrentSBiome].StretchModuleInfluence);
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 1) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseCliffModule) {
				float CliffRange = subBiomes[CurrentSBiome].CliffRange/2f;
				float Limit = subBiomes[CurrentSBiome].CliffBottom - Mathf.PerlinNoise(x*0.01f,y*0.01f)*5f;
				if(HeightS >= Limit - CliffRange && HeightS < Limit + CliffRange) {
					float LerpPos = Mathf.InverseLerp(Limit - CliffRange, Limit + CliffRange,HeightS);
					HeightS = Mathf.SmoothStep(HeightS,HeightS+subBiomes[CurrentSBiome].CliffHeight,LerpPos);
				} else if(HeightS >= Limit + CliffRange) {
					HeightS = HeightS+subBiomes[CurrentSBiome].CliffHeight;
				}
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 2) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseCanyonModule) {
				if(HeightS > subBiomes[CurrentSBiome].CanyonStartingHeight && !(HeightS < subBiomes[CurrentSBiome].CanyonMaxHeight)) {
					float StepHeight = (HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)*subBiomes[CurrentSBiome].CanyonStepCurveFactor;
					HeightS = subBiomes[CurrentSBiome].CanyonStartingHeight + ((Mathf.Floor(StepHeight) + subBiomes[CurrentSBiome].CanyonCurve.Evaluate(Mathf.Repeat(StepHeight,1)))*biomes[CurrentBiome].CanyonStepHeight);
				} else if(HeightS < subBiomes[CurrentSBiome].CanyonMaxHeight) {
					float StepHeight = (HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)*subBiomes[CurrentSBiome].CanyonStepCurveFactor;
					float MaxSH = subBiomes[CurrentSBiome].CanyonStartingHeight+Mathf.Floor((HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)/StepHeight)*StepHeight;
					HeightS = MaxSH+Height-MaxSH;
				}
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 3) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			//BIOME MIN HEIGHT REMOVED DUE TO TESTING
			HeightS += subBiomes[CurrentSBiome].BiomeSecondLifter;
			//HeightS -= subBiomes[CurrentSBiome].BiomeMinHeight*0.75f;

			if(biomes[CurrentBiome].SubBiomePassHeights) {
				if(!biomes[CurrentBiome].SubBiomeRevert) {
					if(Height < HeightS) {
						Height = HeightS;
						blockId = subBiomes[CurrentSBiome].GroundBlock;

						if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 4) {
							if(HeightS > subBiomes[CurrentSBiome].RevertHeight) {
								HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
							}
						}
					}
				} else {
					if(Height > HeightS) {
						//Height = HeightS;
						blockId = subBiomes[CurrentSBiome].GroundBlock;

						if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 4) {
							Height = RevertLastStepSubbiome(subBiomes[CurrentSBiome].revertMode,Height,HeightS,CurrentSBiome);
						} else {
							Height = HeightS;
						}
					}
				}
			} else {
				Height = Mathf.Lerp(Height, HeightS, biomes[CurrentBiome].SubBiomeModuleForce);
			}
		}

		//Height = Mathf.Round(Height)+0.08f; Felling Minecrafty? Uncomment this line!
		if(blockId == -1) {
			blockId = biomes[CurrentBiome].GroundBlock;
		}
		return new float[2]{Height,blockId};
	}

	public float[] GetRawHeightMapBiomesModifier(float x, float y, float NoiseValue, int biome) { //Need a amplified noise
		int CurrentBiome = biome;
		float Height = 0f;
		int blockId = -1;

		//BIOME MIN HEIGHT REMOVED DUE TO TESTING
		Height = NoiseValue;

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 0) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseStrechModule) {
			float HighLimit = (biomes[CurrentBiome].StretchMinimumHeight) + Mathf.PerlinNoise(x*0.01f,y*0.01f) * 5f;
			Height = Mathf.Lerp(Height,HighLimit+((Height-HighLimit)*AmpliCurve(Mathf.InverseLerp(HighLimit,biomes[CurrentBiome].StretchMaxHeight,Height),biomes[CurrentBiome].StretchMaxSlopeFactor)),biomes[CurrentBiome].StretchModuleInfluence);
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 1) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseCliffModule) {
			float CliffRange = biomes[CurrentBiome].CliffRange/2f;
			float Limit = biomes[CurrentBiome].CliffBottom - Mathf.PerlinNoise(x*0.01f,y*0.01f)*5f;
			if(Height >= Limit - CliffRange && Height < Limit + CliffRange) {
				float LerpPos = Mathf.InverseLerp(Limit - CliffRange, Limit + CliffRange,Height);
				Height = Mathf.SmoothStep(Height,Height+biomes[CurrentBiome].CliffHeight,LerpPos);
			} else if(Height >= Limit + CliffRange) {
				Height = Height+biomes[CurrentBiome].CliffHeight;
			}
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 2) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		if(biomes[CurrentBiome].UseCanyonModule) {
			if(Height > biomes[CurrentBiome].CanyonStartingHeight) {
				float StepHeight = (Height-biomes[CurrentBiome].CanyonStartingHeight)*biomes[CurrentBiome].CanyonStepCurveFactor;
				Height = biomes[CurrentBiome].CanyonStartingHeight + ((Mathf.Floor(StepHeight) + biomes[CurrentBiome].CanyonCurve.Evaluate(Mathf.Repeat(StepHeight,1)))*biomes[CurrentBiome].CanyonStepHeight);
			}
		}

		if(biomes[CurrentBiome].UseRevertModule && biomes[CurrentBiome].RevertOrder == 3) {
			if(Height >= biomes[CurrentBiome].RevertHeight) {
				Height = Revert(biomes[CurrentBiome].revertMode,Height,CurrentBiome);
			}
		}

		//BIOME MIN HEIGHT REMOVED DUE TO TESTING
		//Height += biomes[CurrentBiome].BiomeSecondLifter;
		//Height -= biomes[CurrentBiome].BiomeMinHeight*0.75f;

		if(biomes[CurrentBiome].UseSubBiomeModule) {
			float HeightS = 0f;
			int CurrentSBiome = biomes[CurrentBiome].SubBiomeId;

			//BIOME MIN HEIGHT REMOVED DUE TO TESTING
			HeightS = ((subBiomes[CurrentSBiome].BiomeMinHeight)+((float)noiseGroup.GetNoiseValue(CurrentSBiome+MainBiomeNoiseLength,new XnaGeometry.Vector2(x,y))*subBiomes[CurrentSBiome].NoiseAmplitude));

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 0) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,Height,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseStrechModule) {
				float HighLimit = (subBiomes[CurrentSBiome].StretchMinimumHeight) + Mathf.PerlinNoise(x*0.01f,y*0.01f) * 5f;
				HeightS = Mathf.Lerp(HeightS,HighLimit+((HeightS-HighLimit)*AmpliCurve(Mathf.InverseLerp(HighLimit,subBiomes[CurrentSBiome].StretchMaxHeight,HeightS),subBiomes[CurrentSBiome].StretchMaxSlopeFactor)),subBiomes[CurrentSBiome].StretchModuleInfluence);
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 1) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseCliffModule) {
				float CliffRange = subBiomes[CurrentSBiome].CliffRange/2f;
				float Limit = subBiomes[CurrentSBiome].CliffBottom - Mathf.PerlinNoise(x*0.01f,y*0.01f)*5f;
				if(HeightS >= Limit - CliffRange && HeightS < Limit + CliffRange) {
					float LerpPos = Mathf.InverseLerp(Limit - CliffRange, Limit + CliffRange,HeightS);
					HeightS = Mathf.SmoothStep(HeightS,HeightS+subBiomes[CurrentSBiome].CliffHeight,LerpPos);
				} else if(HeightS >= Limit + CliffRange) {
					HeightS = HeightS+subBiomes[CurrentSBiome].CliffHeight;
				}
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 2) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			if(subBiomes[CurrentSBiome].UseCanyonModule) {
				if(HeightS > subBiomes[CurrentSBiome].CanyonStartingHeight && !(HeightS < subBiomes[CurrentSBiome].CanyonMaxHeight)) {
					float StepHeight = (HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)*subBiomes[CurrentSBiome].CanyonStepCurveFactor;
					HeightS = subBiomes[CurrentSBiome].CanyonStartingHeight + ((Mathf.Floor(StepHeight) + subBiomes[CurrentSBiome].CanyonCurve.Evaluate(Mathf.Repeat(StepHeight,1)))*biomes[CurrentBiome].CanyonStepHeight);
				} else if(HeightS < subBiomes[CurrentSBiome].CanyonMaxHeight) {
					float StepHeight = (HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)*subBiomes[CurrentSBiome].CanyonStepCurveFactor;
					float MaxSH = subBiomes[CurrentSBiome].CanyonStartingHeight+Mathf.Floor((HeightS-subBiomes[CurrentSBiome].CanyonStartingHeight)/StepHeight)*StepHeight;
					HeightS = MaxSH+Height-MaxSH;
				}
			}

			if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 3) {
				if(HeightS >= subBiomes[CurrentSBiome].RevertHeight) {
					HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
				}
			}

			//BIOME MIN HEIGHT REMOVED DUE TO TESTING
			HeightS += subBiomes[CurrentSBiome].BiomeSecondLifter;
			//HeightS -= subBiomes[CurrentSBiome].BiomeMinHeight*0.75f;

			if(biomes[CurrentBiome].SubBiomePassHeights) {
				if(!biomes[CurrentBiome].SubBiomeRevert) {
					if(Height < HeightS) {
						Height = HeightS;
						blockId = subBiomes[CurrentSBiome].GroundBlock;

						if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 4) {
							if(HeightS > subBiomes[CurrentSBiome].RevertHeight) {
								HeightS = Revert(subBiomes[CurrentSBiome].revertMode,HeightS,CurrentBiome);
							}
						}
					}
				} else {
					if(Height > HeightS) {
						//Height = HeightS;
						blockId = subBiomes[CurrentSBiome].GroundBlock;

						if(subBiomes[CurrentSBiome].UseRevertModule && subBiomes[CurrentSBiome].RevertOrder == 4) {
							Height = RevertLastStepSubbiome(subBiomes[CurrentSBiome].revertMode,Height,HeightS,CurrentSBiome);
						} else {
							Height = HeightS;
						}
					}
				}
			} else {
				Height = Mathf.Lerp(Height, HeightS, biomes[CurrentBiome].SubBiomeModuleForce);
			}
		}

		//Height = Mathf.Round(Height)+0.08f; Felling Minecrafty? Uncomment this line!
		if(blockId == -1) {
			blockId = biomes[CurrentBiome].GroundBlock;
		}
		return new float[2]{Height,blockId};
	}


	float RevertLastStepSubbiome (Biome.RevertMode revertM, float Height, float HeightS, int BiomeId) {
		switch(revertM) {
		case Biome.RevertMode.Substract:
			return Height-Mathf.Clamp((Height-HeightS)*subBiomes[BiomeId].RevertForce,Height+-subBiomes[BiomeId].RevertLimit,Height+subBiomes[BiomeId].RevertLimit);
			break;
		case Biome.RevertMode.Add:
			return Height+Mathf.Clamp((Height-HeightS)*subBiomes[BiomeId].RevertForce,Height+-subBiomes[BiomeId].RevertLimit,Height+subBiomes[BiomeId].RevertLimit);
			break;
		}
		return HeightS;
	}

	float Revert (Biome.RevertMode revertM, float Height, int BiomeId) {
		switch(revertM) {
		case Biome.RevertMode.Substract:
			return Height-Mathf.Clamp((Height-biomes[BiomeId].RevertHeight)*biomes[BiomeId].RevertForce,biomes[BiomeId].RevertHeight+-biomes[BiomeId].RevertLimit,biomes[BiomeId].RevertHeight+biomes[BiomeId].RevertLimit);
			break;
		case Biome.RevertMode.Add:
			return Height+Mathf.Clamp((Height-biomes[BiomeId].RevertHeight)*biomes[BiomeId].RevertForce,biomes[BiomeId].RevertHeight+-biomes[BiomeId].RevertLimit,biomes[BiomeId].RevertHeight+biomes[BiomeId].RevertLimit);
			break;
		}
		return Height;
	}

	public void SaveWorldHeight (int chunkX, int chunkY, float[,] Value) {
		string FileDirectory = creator.loader.WorldDirectoryPath + Path.DirectorySeparatorChar + (chunkX.ToString() + "_" + chunkY.ToString()) + ".cbc"; //cbc: ConsoleBlock Chunk

		byte[] feed = new byte[(creator.loader.SimulatedChunkSize+1)*(creator.loader.SimulatedChunkSize+1)*4]; //4: Byte count in a float
		byte[] temp;

		for(int x = 0; x < creator.loader.SimulatedChunkSize+1; x++) {
			for(int y = 0; y < creator.loader.SimulatedChunkSize+1; y++) {
				temp = BitConverter.GetBytes(Value[x,y]);
				feed[(x+(y*(creator.loader.SimulatedChunkSize+1)))*4+0] = temp[0];
				feed[(x+(y*(creator.loader.SimulatedChunkSize+1)))*4+1] = temp[1];
				feed[(x+(y*(creator.loader.SimulatedChunkSize+1)))*4+2] = temp[2];
				feed[(x+(y*(creator.loader.SimulatedChunkSize+1)))*4+3] = temp[3];
			}
		}

		File.WriteAllBytes(FileDirectory, Compress(feed));
	}

	public float[,] GetWorldHeight (int chunkX, int chunkY, int blockInterval) {
		string FileDirectory = creator.loader.WorldDirectoryPath + Path.DirectorySeparatorChar + (chunkX.ToString() + "_" + chunkY.ToString()) + ".cbc"; //cbc: ConsoleBlock Chunk

		if(File.Exists(FileDirectory) && !creator.loader.DebuggingMode) {
			byte[] feed = Decompress(File.ReadAllBytes(FileDirectory));
			float[,] results = new float[creator.loader.SimulatedChunkSize/blockInterval+1,creator.loader.SimulatedChunkSize/blockInterval+1];

			for(int x = 0; x < creator.loader.SimulatedChunkSize+1; x+=blockInterval) {
				for(int y = 0; y < creator.loader.SimulatedChunkSize+1; y+=blockInterval) {
					results[x/blockInterval,y/blockInterval] = BitConverter.ToSingle(feed, 
						(x + (y*(creator.loader.SimulatedChunkSize+1)))*4
					);
					results[x/blockInterval,y/blockInterval] /= blockInterval;
				}
			}
			return results;
		} else {
			float[,] results = new float[creator.loader.SimulatedChunkSize+1,creator.loader.SimulatedChunkSize+1];
			float[,] results2 = new float[creator.loader.SimulatedChunkSize/blockInterval+1,creator.loader.SimulatedChunkSize/blockInterval+1];

			for(int x = 0; x < creator.loader.SimulatedChunkSize+1; x++) {
				for(int y = 0; y < creator.loader.SimulatedChunkSize+1; y++) {
					results[x,y] = GetHeightMap(chunkX*creator.loader.SimulatedChunkSize + x,chunkY*creator.loader.SimulatedChunkSize + y)[0];
				}
			}
			SaveWorldHeight(chunkX,chunkY,results);
			for(int x = 0; x < creator.loader.SimulatedChunkSize+blockInterval; x+=blockInterval) {
				for(int y = 0; y < creator.loader.SimulatedChunkSize+blockInterval; y+=blockInterval) {
					results2[x/blockInterval,y/blockInterval] = results[x,y]/blockInterval;
				}
			}
			return results2;
		}

		//If the file exist:
		//	DECOMPRESS
		//	If the file has all the parts needed
		//		Get the parts needed
		//	Else
		// 		Generate the parts needed, send them to "SaveWorldHeight" and return them
		//Else
		//	Generate the parts needed, send them to "SaveWorldHeight" and return them

		return null;
	}

	public static byte[] Compress(byte[] raw)
	{
		using (MemoryStream memory = new MemoryStream())
		{
			using (GZipStream gzip = new GZipStream(memory,
				CompressionMode.Compress, true))
			{
				gzip.Write(raw, 0, raw.Length);
			}
			return memory.ToArray();
		}
	}

	static byte[] Decompress(byte[] gzip)
	{
		using (GZipStream stream = new GZipStream(new MemoryStream(gzip),
			CompressionMode.Decompress))
		{
			const int size = 4096;
			byte[] buffer = new byte[size];
			using (MemoryStream memory = new MemoryStream())
			{
				int count = 0;
				do
				{
					count = stream.Read(buffer, 0, size);
					if (count > 0)
					{
						memory.Write(buffer, 0, count);
					}
				}
				while (count > 0);
				return memory.ToArray();
			}
		}
	}

	float CustomRound (float Value, float Step) {
		return Step*Mathf.Round(Value/Step);
	}

	float AmpliCurve (float Time, float Force) {
		return Mathf.Pow(Mathf.Clamp01(Time),Force);
	}

	//Get the height of the water at a certain pos
	public float GetWaterHeight(float x, float y) {
		return -20f;
	}

	//Declaring a lock to prevent thread issues
	private System.Object textureLock = new System.Object();
	float Height = 0f;

	//Get the block Id at a certain pos
	public int GetTextureTypeAtPixel (float x, float y, int blockSize, float blockHeight) {
		float[] temp = GetHeightMap(x,y);
		if(blockHeight == 0 || blockHeight == null) {
			Height = temp[0]*blockSize;
		}
		Height = blockHeight*blockSize;
		return (int)temp[1];
	}
}
