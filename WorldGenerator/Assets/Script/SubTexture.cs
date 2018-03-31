using UnityEngine;

[CreateAssetMenu(fileName = "New Sub World Texture", menuName = "ConsoleBlock/Sub World Texture")]
public class SubWorldTexture : ScriptableObject {

	new public string name = "";

	public Vector2[] Coords;
	public int RepeatRange = 1;

	public float SlopeThreshold = 35f;
	public int SlopeSubTextureId = 0;

	//public Vector2 TopCoords;
	//public Vector2 UpperWallCoords;
	//public Vector2 LowerWallCoords;
	//public Vector2 UnderWallCoords;
}
