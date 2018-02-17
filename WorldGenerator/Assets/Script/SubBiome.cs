using UnityEngine;
using OTNM;
using OTNM.Tools;

[CreateAssetMenu(fileName = "New Sub Biome", menuName = "ConsoleBlock/Sub Biome")]
public class SubBiome : ScriptableObject {
	
	public int GroundBlock = 0;
	public Color BiomeGrassColor = new Color();

	public float BiomeMinHeight = 30f;
	public float BiomeSecondLifter = 0f;

	public bool UseStrechModule;
	[Range(0.0f,1.0f)]
	public float StretchModuleInfluence = 0f;
	public float StretchMinimumHeight = -30f;
	public float StretchMaxHeight = 0f;
	public float StretchMaxSlopeFactor = 1f;
	//public AnimationCurve StretchCurve;

	public bool UseCanyonModule = false;
	public float CanyonStartingHeight = 20f;
	public float CanyonMaxHeight = 300f;
	public float CanyonStepHeight = 0.05f;
	public float CanyonStepCurveFactor = 0.05f;
	public AnimationCurve CanyonCurve;
	[Range(0.0f,1.0f)]
	public float CanyonModuleForce = 0.2f;
	Accessing.NoiseParameters nparam = new Accessing.NoiseParameters(6,2.3f,0.5f);
	public Accessing.CanyonParameters CanyonParameters = new Accessing.CanyonParameters(-0.05f,0.65f,new Accessing.CanyonParameters.CanyonKey[]{
		new Accessing.CanyonParameters.CanyonKey(0,0),
		new Accessing.CanyonParameters.CanyonKey(0.2f,0.6f),
		new Accessing.CanyonParameters.CanyonKey(0.2f,1.5f),
		new Accessing.CanyonParameters.CanyonKey(1.1f,2.2f),
		new Accessing.CanyonParameters.CanyonKey(1.1f,2.5f),
		new Accessing.CanyonParameters.CanyonKey(2.3f,3.9f),
		new Accessing.CanyonParameters.CanyonKey(2.3f,4.5f),
		new Accessing.CanyonParameters.CanyonKey(4.1f,5.3f),
		new Accessing.CanyonParameters.CanyonKey(4.1f,7.1f),
		new Accessing.CanyonParameters.CanyonKey(4.5f,7.8f),
		new Accessing.CanyonParameters.CanyonKey(4.6f,9.5f),
		new Accessing.CanyonParameters.CanyonKey(6.3f,10.9f),
		new Accessing.CanyonParameters.CanyonKey(6.3f,11.8f),
		new Accessing.CanyonParameters.CanyonKey(7.6f,15.2f)
	});
	FastNoiseLibrary.FastNoise fastn = new FastNoiseLibrary.FastNoise(true,0);


	public bool UseCliffModule = false;
	public float CliffHeight = 20f;
	public float CliffBottom = 40f;
	public float CliffRange = 10f;

	public bool UseRevertModule = false;
	public Biome.RevertMode revertMode;
	public float RevertHeight = 60f;
	public float RevertForce = 2f;
	public float RevertLimit = Mathf.Infinity;
	[Range(0,4)]
	public int RevertOrder = 1;

	public float NoiseAmplitude = 200f;
	public float Frequency = 0.0012f;
	public float NoiseLacunarity = 10f;
	public float NoisePersistence = 0.5f;
	public int NoiseOctaves = 3;
	public Biome.NoiseType noiseType = Biome.NoiseType.RiggedMultifractal;
	public OTNM.TerrainNoise.TerrainType terrainType = OTNM.TerrainNoise.TerrainType.SimplexNoise;
}
