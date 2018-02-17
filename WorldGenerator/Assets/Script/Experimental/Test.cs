using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public int width;
	public int height;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomAirOverWalls;

	[Range(0,100)]
	public int randomSpecialWalls;

	[Range(0,100)]
	public int randomFillPercent;

	int[,] map;

	void Start() {
		GenerateMap();
	}

	void Update() {
		if (Input.GetMouseButtonDown(0)) {
			GenerateMap();
		}
	}

	void GenerateMap() {
		width = Mathf.FloorToInt(width/4f)*4+1;
		height = Mathf.FloorToInt(height/4f)*4+1;
		map = new int[width,height];

		int CellWidth = Mathf.FloorToInt(width/4f);
		int CellHeight = Mathf.FloorToInt(height/4f);

		if(useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		//Create Walls
		for(int x = 0; x < CellWidth; x ++) {
			for(int y = 0; y < CellHeight; y ++) {
				//Wall X
				int Wall1Type = 0;

				if(pseudoRandom.Next(0,100) < randomAirOverWalls) {
					Wall1Type = 0;
				} else {
					if(pseudoRandom.Next(0,100) > randomSpecialWalls) {
						Wall1Type = 1;
					} else {
						Wall1Type = pseudoRandom.Next(2,3);
					}
				}

				switch(Wall1Type) {
				case 0:
					//Nothin'
					break;
				case 1:
					map[x*4,y*4] = 1;
					map[x*4+1,y*4] = 1;
					map[x*4+2,y*4] = 1;
					map[x*4+3,y*4] = 1;
					map[x*4+4,y*4] = 1;
					//Everythin'
					break;
				case 2:
					map[x*4,y*4] = 1;
					map[x*4+1,y*4] = 1;
					map[x*4+2,y*4] = 1;
					//Half-a-wall 1
					break;
				case 3:
					map[x*4+2,y*4] = 1;
					map[x*4+3,y*4] = 1;
					map[x*4+4,y*4] = 1;
					//Half-a-wall 2
					break;
				}

				//Wall Y
				int Wall2Type = 0;

				if(pseudoRandom.Next(0,100) < randomAirOverWalls) {
					Wall2Type = 0;
				} else {
					if(pseudoRandom.Next(0,100) > randomSpecialWalls) {
						Wall2Type = 1;
					} else {
						Wall2Type = pseudoRandom.Next(2,3);
					}
				}

				switch(Wall2Type) {
				case 0:
					//Nothin'
					break;
				case 1:
					map[x*4,y*4] = 1;
					map[x*4,y*4+1] = 1;
					map[x*4,y*4+2] = 1;
					map[x*4,y*4+3] = 1;
					map[x*4,y*4+4] = 1;
					//Everythin'
					break;
				case 2:
					map[x*4,y*4] = 1;
					map[x*4,y*4+1] = 1;
					map[x*4,y*4+2] = 1;
					//Half-a-wall 1
					break;
				case 3:
					map[x*4,y*4+2] = 1;
					map[x*4,y*4+3] = 1;
					map[x*4,y*4+4] = 1;
					//Half-a-wall 2
					break;
				}
			}
		}

		//Flip Map
		for(int x = 0; x < width; x ++) {
			for(int y = 0; y < height; y ++) {
				map[x,y] = map[Mathf.RoundToInt(Mathf.PingPong(x,width*0.5f-0.5f)),y];
			}
		}

		//Create an outline
		for(int x = 0; x < width; x ++) {
			for(int y = 0; y < height; y ++) {
				if(x == 0 || y == 0 || x == width-1 || y == height-1) {
					map[x,y] = 1;
				}
			}
		}

		//Clean Map
		for(int x = 0; x < width; x ++) {
			for(int y = 0; y < height; y ++) {
				if(map[x,y] == 1 && GetSurroundingWallCount(x,y) == 0) {
					map[x,y] = 0;
				}
				if(map[x,y] == 0 && GetSurroundingWallCount(x,y) == 0) {
					int surrondingWallCount = 0;
					for(int x2 = -2; x2 < 3; x2++) {
						for(int y2 = -2; y2 < 3; y2++) {
							if(x2 == -2 || y2 == -2 || x2 == 2 || y2 == 2) {
								if(x+x2 >= 0 && x+x2 < width && y+y2 >= 0 && y+y2 < height) {
									if(map[x+x2,y+y2] == 1) {
										surrondingWallCount++;
									}
								}
							}
						}
					}
					if(surrondingWallCount >= 16) {
						for(int x2 = -1; x2 < 2; x2++) {
							for(int y2 = -1; y2 < 2; y2++) {
								if(x+x2 >= 0 && x+x2 < width && y+y2 >= 0 && y+y2 < height) {
									map[x+x2,y+y2] = 1;
								}
							}
						}
					}
				}
			}
		}

		/*map = new int[width,height];
		RandomFillMap();

		for (int i = 0; i < 5; i ++) {
			SmoothMap();
		}*/
	}


	void RandomFillMap() {
		if (useRandomSeed) {
			seed = Time.time.ToString();
		}

		System.Random pseudoRandom = new System.Random(seed.GetHashCode());

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = 1;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					map[x,y] = 1;
				else if (neighbourWallTiles < 4)
					map[x,y] = 0;

			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}


	void OnDrawGizmos() {
		if (map != null) {
			for (int x = 0; x < width; x ++) {
				for (int y = 0; y < height; y ++) {
					Gizmos.color = (map[x,y] == 1)?Color.black:Color.white;
					Vector3 pos = new Vector3(-width/2 + x + .5f,0, -height/2 + y+.5f);
					Gizmos.DrawCube(pos,Vector3.one);
				}
			}
		}
	}
}
