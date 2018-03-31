using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "ConsoleBlock/Structure")]
public class Structure : ScriptableObject {
	new public string name = "";
	public float MinSpawningHeight = 0f;
	public float MaxSpawningHeight = 70f;
	public float StructureFrequency = 0.1f;
	public float MinScaleModifRange = 0.9f;
	public float MaxScaleModifRange = 1.1f;

	public float HumidityValue = 0.5f;
	public float HumidityRange = 0.1f;
	public float TemperateValue = 0.5f;
	public float TemperateRange = 0.1f;

	public GameObject[] Objects;
}
