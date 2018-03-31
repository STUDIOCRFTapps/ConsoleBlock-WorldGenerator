using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New World Texture", menuName = "ConsoleBlock/World Texture")]
public class WorldTexture : ScriptableObject {

	new public string name = "";

	public int MainTexture;
	public TextureGroup[] TextureGroups;
}

[System.Serializable]
public class TextureGroup {
	public float MaxHeight = Mathf.Infinity;

	public int MainGroupTexture;
	public PatchParameters patchs;
}

[System.Serializable]
public class PatchParameters {
	public bool EnablePatchModule = false;
	public bool SeperatePatchs = true;
	public float PatchScale = 0.05f;
	public float PatchThreshold = 0.67f;
	public int PatchTexture;
}
