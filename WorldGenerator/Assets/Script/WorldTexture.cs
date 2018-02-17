using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New World Texture", menuName = "ConsoleBlock/World Texture")]
public class WorldTexture : ScriptableObject {

	new public string name = "";

	public SubWorldTexture MainTexture;
	public TextureGroup[] TextureGroups;
}

[System.Serializable]
public class TextureGroup {
	public float MaxHeight = Mathf.Infinity;

	public SubWorldTexture MainGroupTexture;
	public PatchParameters patchs;
}

[System.Serializable]
public class PatchParameters {
	public bool EnablePatchModule = false;
	public bool SeperatePatchs = true;
	public float PatchScale = 0.05f;
	public float PatchThreshold = 0.67f;
	public SubWorldTexture PatchTexture;
}
