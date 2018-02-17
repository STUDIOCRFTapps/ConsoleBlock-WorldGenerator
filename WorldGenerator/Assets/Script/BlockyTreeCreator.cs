using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class BlockyTreeCreator : MonoBehaviour {
	
	[HideInInspector]
	public TreeCreatorTools.TreeShape treeShape;
	public TreeCreatorTools.LeavesType leavesType;
	public GameObject Fruit;
	public TreeCreatorTools.FruitSize fruitSize;
	public Vector3 SIZEv1;
	public Vector3 SIZEv2;
	public float LoweringFactor = 0.2f;
	public TreeCreatorTools.FruitPosition fruitPosition;
	public float MaxXRotationRange;
	public float MaxZRotationRange;
	public bool AllowRandomYRotation;

	public AnimationCurve RoundFoliage;

	public Material LogMaterial;
	public Material FluffLeavesMaterials;
	public GameObject PreparedLeaf;

	public AnimationCurve logCurve = new AnimationCurve();
	public float logCurveInfluence;
	public int LogMinHeight;
	public int LogMaxHeight;
	public float LogMinSize;
	public float LogMaxSize;

	public float LeafSize = 0.05f;

	public AnimationCurve LogCurving = new AnimationCurve();
	public float LogCurvingIntensity;
	public float HeightBeforeBranch;
	public int MinBranchCount;
	public int MaxBranchCount;
	public float FlatFluffHeight;
	public float FlatFluffWidth;
	public float MinBranchLenght;
	public float MaxBranchLenght;
	public float FoliageHeight;
	public float FoliageWidth;
	public float RoundFluffMinSize;
	public float RoundFluffMaxSize;
	public float BranchRotation;
	public float FoliageWidthInfluenceOverRotation;
	public float LeavesYRotationVariationRange;
	public float LeavesZRotationVariationRange;
	public float MinDistanceBettwenBranch;
	public float MaxDistanceBettwenBranch;
	public float BottomFolliageExtention;
	public float MiddleFolliageExtention;
	public float CrookingIntensity;

	public int Seed;
	public string TreeName;
	public GameObject FluffyTemplate;
	public GameObject ObjectTemplate;

	// Use this for initialization
	void Start () {
		RoundFoliage.AddKey(new Keyframe(0,0,2.5f,2.5f));
		RoundFoliage.AddKey(new Keyframe(0,0,2.5f,2.5f));
		RoundFoliage.AddKey(new Keyframe(0.5f,1,0,0));
		RoundFoliage.AddKey(new Keyframe(0.5f,1,0,0));
		RoundFoliage.AddKey(new Keyframe(1,0,-2.5f,-2.5f));
		RoundFoliage.AddKey(new Keyframe(1,0,-2.5f,-2.5f));

		Random.InitState(Seed);

		switch(treeShape) {
		case TreeCreatorTools.TreeShape.Pyramidal:
			int TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			float TreeSize = Random.Range(LogMinSize,LogMaxSize);
			float DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			float CurrentHeight = 0;
			float CurrentScale = 1f;

			GameObject Obj;
			AnimationCurve radiusCurve = new AnimationCurve();
			radiusCurve.AddKey(0f,BottomFolliageExtention);
			radiusCurve.AddKey(0.5f,MiddleFolliageExtention);
			radiusCurve.AddKey(1f,0f);
			radiusCurve.SmoothTangents(0,1f);
			radiusCurve.SmoothTangents(1,1f);
			radiusCurve.SmoothTangents(2,1f);
			int beforeRadius = 0;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					//Mathf.RoundToInt(DistanceBettwenBranch*3f+1f)

					for(int i = 0; i < Mathf.RoundToInt(DistanceBettwenBranch*21f+1f); i++) {
						Obj = (GameObject)Instantiate(PreparedLeaf,transform);
						float Direction = Random.Range(0f,360f);
						Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).x,CurrentHeight,TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).y);
						Obj.transform.eulerAngles = new Vector3(0f,Direction,BranchRotation-90);
						float BranchScale = (TreeSize*0.3f) * (radiusCurve.Evaluate(((float)tH-beforeRadius)/TreeHeight)*logCurveInfluence);
						Obj.transform.localScale = new Vector3(BranchScale,BranchScale,BranchScale);

						//FRUITALIZE (Prep)
						if(Random.Range(0,2) == 0 && Fruit != null) {
							GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

							float PositionZ = 0f; 
							switch(fruitPosition) {
							case TreeCreatorTools.FruitPosition.MainBranchStart:
								PositionZ = 0.7f;
								break;
							case TreeCreatorTools.FruitPosition.MainBranchEnd:
								PositionZ = 7f;
								break;
							case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
								PositionZ = Random.Range(0.7f,7f);
								break;
							}

							FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
							FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

							switch(fruitSize) {
							case TreeCreatorTools.FruitSize.NoModification:
								//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
								break;
							case TreeCreatorTools.FruitSize.Constant:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
								break;
							case TreeCreatorTools.FruitSize.Random:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
								break;
							}

							FruitObj.transform.parent = FruitObj.transform.parent.parent;

							FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
							if(AllowRandomYRotation) {
								FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
							}
							FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
						}
					}
					//Obj.name = TreeCreatorTools.CircleCreator(Direction,1f).x.ToString() + ", " + TreeCreatorTools.CircleCreator(Direction,1f).y.ToString();
				} else {
					beforeRadius++;
				}

				CurrentHeight += CurrentScale;
			}
			break;
		case TreeCreatorTools.TreeShape.Fountain:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			float TreeDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(TreeDirection,LogCurving.Evaluate(tH * (1f/TreeHeight))*LogCurvingIntensity).x,CurrentHeight,TreeCreatorTools.CircleCreator(TreeDirection,LogCurving.Evaluate(tH * (1f/TreeHeight))*LogCurvingIntensity).y);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;
				CurrentHeight += CurrentScale;
			}

			int BranchCount = Random.Range(MinBranchCount,MaxBranchCount);

			for(int bC = 0; bC < BranchCount; bC++) {
				float Direction = bC * (360f/BranchCount);

				Obj = (GameObject)Instantiate(PreparedLeaf,transform);
				Obj.transform.localScale = new Vector3(TreeHeight*0.05f,TreeHeight*LeafSize,TreeHeight*LeafSize);
				Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(TreeDirection,LogCurving.Evaluate(1f)*LogCurvingIntensity).x,CurrentHeight,TreeCreatorTools.CircleCreator(TreeDirection,LogCurving.Evaluate(1f)*LogCurvingIntensity).y);
				Obj.transform.eulerAngles = new Vector3(Random.Range(-LeavesYRotationVariationRange,LeavesYRotationVariationRange),Direction,0-BranchRotation-90+Random.Range(-LeavesZRotationVariationRange,LeavesZRotationVariationRange));

				//FRUITALIZE (Prep)
				if(Random.Range(0,2) == 0 && Fruit != null) {
					GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

					float PositionZ = 0f; 
					switch(fruitPosition) {
					case TreeCreatorTools.FruitPosition.MainBranchStart:
						PositionZ = 0.7f;
						break;
					case TreeCreatorTools.FruitPosition.MainBranchEnd:
						PositionZ = 7f;
						break;
					case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
						PositionZ = Random.Range(0.7f,7f);
						break;
					}

					FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
					FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

					switch(fruitSize) {
					case TreeCreatorTools.FruitSize.NoModification:
						//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
						break;
					case TreeCreatorTools.FruitSize.Constant:
						TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
						break;
					case TreeCreatorTools.FruitSize.Random:
						TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
						break;
					}

					FruitObj.transform.parent = FruitObj.transform.parent.parent;

					FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
					if(AllowRandomYRotation) {
						FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
					}
					FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
				}
			}

			break;
		case TreeCreatorTools.TreeShape.Round:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			float BranchDistance = DistanceBettwenBranch;
			float BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {

						if(leavesType == TreeCreatorTools.LeavesType.Prepared) {
							Obj = (GameObject)Instantiate(PreparedLeaf,transform);
							float Direction = Random.Range(0f,360f);
							Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).x,CurrentHeight,TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).y);
							Obj.transform.eulerAngles = new Vector3(0f,Direction,-BranchRotation-40);
							float BranchScale = RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*0.3f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f);
							Obj.transform.localScale = new Vector3(BranchScale,BranchScale,BranchScale);

							//FRUITALIZE (Prep)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.7f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 7f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.7f,7f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
							}

							BranchDistance = 0f;
						} else {
							//Setup a branch
							Obj = (GameObject)Instantiate(ObjectTemplate,transform);
							Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
							float THICCness = Random.Range(0.14987f,0.3756f);
							Obj.transform.localScale = new Vector3(THICCness,RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*7.8f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f),THICCness);
							Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,BranchRotation-100);
							Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

							Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
							List<Vector2> UVs = new List<Vector2>();

							for(int i = 0; i < UVEditor.uv.Length; i++) {
								if(i < 4) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 6) { //8 -> 12 
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 8) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 10) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 12) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 16) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 20) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 24) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								}
							}
							Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

							//FRUITALIZE (Branch)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.1f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 1f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.1f,1f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

								FruitObj.transform.position -= Vector3.one*LoweringFactor;
							}

							//Add the FLUFF at the end of the branch! (Also don't forget to do the prepared branch mode too)

							if(Obj.transform.localScale.y < 1.2f) {
								continue;
							}

							int FluffQ = Mathf.RoundToInt(Random.Range(RoundFluffMinSize*0.6f,RoundFluffMaxSize*0.6f));
							GameObject Fluff;

							if(FluffQ == 0 && Obj.transform.localScale.y > 6f) {
								FluffQ = 1;
							}

							for(int i = 0; i < FluffQ; i++) {
								Fluff = (GameObject)Instantiate(FluffyTemplate,Obj.transform.GetChild(0));
								Fluff.transform.localEulerAngles = Vector3.zero;
								Fluff.transform.localPosition = new Vector3(0f,Random.Range(0.1f,0.5f),0f);
								Fluff.transform.position += new Vector3(Random.Range(-RoundFluffMinSize,RoundFluffMinSize)*1f, 0f, Random.Range(RoundFluffMinSize,-RoundFluffMinSize)*1f);
								float Size = Random.Range(6f,8f);
								TreeCreatorTools.SetGlobalScale(Fluff.transform,new Vector3(Size,Size,Size));

								Fluff.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
							}

							BranchDistance = 0f;
							BranchDirection += 150+Random.Range(-16.4f,16.4f);
						}
					} else {
						BranchDistance += CurrentScale;
					}
				} else {
					beforeRadius++;
				}

				CurrentHeight += CurrentScale;
			}
			break;
		case TreeCreatorTools.TreeShape.Columnar:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {

						if(leavesType == TreeCreatorTools.LeavesType.Prepared) {
							Obj = (GameObject)Instantiate(PreparedLeaf,transform);
							float Direction = Random.Range(0f,360f);
							Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).x,CurrentHeight,TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).y);
							Obj.transform.eulerAngles = new Vector3(0f,Direction,(-BranchRotation+30)-(RoundFoliage.Evaluate((tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*FoliageWidthInfluenceOverRotation));
							float BranchScale = 1.2f;
							Obj.transform.localScale = new Vector3(BranchScale,BranchScale,BranchScale);

							//FRUITALIZE (Prep)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.7f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 7f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.7f,7f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
							}

							BranchDistance = 0f;
						} else {
							//Setup a branch
							Obj = (GameObject)Instantiate(ObjectTemplate,transform);
							Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
							float THICCness = Random.Range(0.14987f,0.3756f);
							//RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*7.8f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f)
							Obj.transform.localScale = new Vector3(THICCness,10,THICCness);
							Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,(BranchRotation-100)-(RoundFoliage.Evaluate((tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*FoliageWidthInfluenceOverRotation));
							Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

							Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
							List<Vector2> UVs = new List<Vector2>();

							for(int i = 0; i < UVEditor.uv.Length; i++) {
								if(i < 4) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 6) { //8 -> 12 
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 8) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 10) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 12) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 16) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 20) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 24) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								}
							}
							Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

							//FRUITALIZE (Branch)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.1f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 1f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.1f,1f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

								FruitObj.transform.position -= Vector3.one*LoweringFactor;
							}

							//Add the FLUFF at the end of the branch! (Also don't forget to do the prepared branch mode too)

							if(Obj.transform.localScale.y < 1.2f) {
								continue;
							}

							int FluffQ = 2;
							GameObject Fluff;

							if(FluffQ == 0 && Obj.transform.localScale.y > 6f) {
								FluffQ = 1;
							}

							for(int i = 0; i < FluffQ; i++) {
								Fluff = (GameObject)Instantiate(FluffyTemplate,Obj.transform.GetChild(0));
								Fluff.transform.localEulerAngles = Vector3.zero;
								Fluff.transform.localPosition = new Vector3(0f,Random.Range(0.2f,0.6f),0f);
								Fluff.transform.position += new Vector3(Random.Range(-RoundFluffMinSize,RoundFluffMinSize)*1f, 0f, Random.Range(RoundFluffMinSize,-RoundFluffMinSize)*1f);
								float Size = Random.Range(2f,5f);
								TreeCreatorTools.SetGlobalScale(Fluff.transform,new Vector3(Size,Size,Size));

								Fluff.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
							}

							BranchDistance = 0f;
							BranchDirection += 150+Random.Range(-16.4f,16.4f);
						}
					} else {
						BranchDistance += CurrentScale;
					}
				} else {
					beforeRadius++;
				}

				CurrentHeight += CurrentScale;
			}
			break;
		case TreeCreatorTools.TreeShape.Oval:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {
						//Setup a branch
						Obj = (GameObject)Instantiate(ObjectTemplate,transform);
						Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
						float THICCness = Random.Range(0.14987f,0.3756f);
						Obj.transform.localScale = new Vector3(THICCness,RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*7.8f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f),THICCness);
						Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,BranchRotation-100);
						Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

						Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
						List<Vector2> UVs = new List<Vector2>();

						for(int i = 0; i < UVEditor.uv.Length; i++) {
							if(i < 4) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 6) { //8 -> 12 
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 8) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 10) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 12) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 16) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 20) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 24) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							}
						}
						Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

						//FRUITALIZE (Branch)
						if(Random.Range(0,2) == 0 && Fruit != null) {
							GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

							float PositionZ = 0f; 
							switch(fruitPosition) {
							case TreeCreatorTools.FruitPosition.MainBranchStart:
								PositionZ = 0.1f;
								break;
							case TreeCreatorTools.FruitPosition.MainBranchEnd:
								PositionZ = 1f;
								break;
							case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
								PositionZ = Random.Range(0.1f,1f);
								break;
							}

							FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
							FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

							switch(fruitSize) {
							case TreeCreatorTools.FruitSize.NoModification:
								//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
								break;
							case TreeCreatorTools.FruitSize.Constant:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
								break;
							case TreeCreatorTools.FruitSize.Random:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
								break;
							}

							FruitObj.transform.parent = FruitObj.transform.parent.parent;

							FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
							if(AllowRandomYRotation) {
								FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
							}
							FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

							FruitObj.transform.position -= Vector3.one*LoweringFactor;
						}

						BranchDistance = 0f;
						BranchDirection += 150+Random.Range(-16.4f,16.4f);
					} else {
						BranchDistance += CurrentScale;
					}
				} else {
					beforeRadius++;
				}

				CurrentHeight += CurrentScale;

			}

			//Create Leaves :D

			int FluffDensity = Mathf.RoundToInt(Random.Range((FoliageHeight*FoliageWidth)/2-5,(FoliageHeight*FoliageWidth)/2+5));
			GameObject FluffObj;

			for(int f = 0; f < FluffDensity; f++) {
				//Random Height, Random Direction, Random Radius
				//Bettwen HeightBeforeBranch and HBB+FoliageHeight, Random Direction, Oval radius
				float Height = Random.Range(HeightBeforeBranch*TreeHeight,HeightBeforeBranch*TreeHeight+FoliageHeight);

				FluffObj = (GameObject)Instantiate(FluffyTemplate,transform);
				FluffObj.transform.localRotation = Random.rotationUniform;
				FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
				float Size = Random.Range(6f,8f);
				TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

				Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),RoundFoliage.Evaluate(TreeCreatorTools.Map(Height,HeightBeforeBranch*TreeHeight,HeightBeforeBranch*TreeHeight+FoliageHeight,0f,1f))*FoliageWidth);
				FluffObj.transform.localPosition = new Vector3(Position.x,Height,Position.y);

			}
			break;
		case TreeCreatorTools.TreeShape.Weeping:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {
						Obj = (GameObject)Instantiate(PreparedLeaf,transform);
						float Direction = Random.Range(0f,360f);
						Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).x,CurrentHeight,TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).y);
						Obj.transform.eulerAngles = new Vector3(180f,Direction,-BranchRotation-40);
						float BranchScale = 1.2f*CurrentScale;
						Obj.transform.localScale = new Vector3(BranchScale,BranchScale,BranchScale);

						//FRUITALIZE (Prep)
						if(Random.Range(0,2) == 0 && Fruit != null) {
							GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

							float PositionZ = 0f; 
							switch(fruitPosition) {
							case TreeCreatorTools.FruitPosition.MainBranchStart:
								PositionZ = 0.7f;
								break;
							case TreeCreatorTools.FruitPosition.MainBranchEnd:
								PositionZ = 7f;
								break;
							case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
								PositionZ = Random.Range(0.7f,7f);
								break;
							}

							FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
							FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

							switch(fruitSize) {
							case TreeCreatorTools.FruitSize.NoModification:
								//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
								break;
							case TreeCreatorTools.FruitSize.Constant:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
								break;
							case TreeCreatorTools.FruitSize.Random:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
								break;
							}

							FruitObj.transform.parent = FruitObj.transform.parent.parent;

							FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
							if(AllowRandomYRotation) {
								FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
							}
							FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
						}

						FluffObj = (GameObject)Instantiate(FluffyTemplate,transform);
						FluffObj.transform.localRotation = Random.rotationUniform;
						FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
						float Size = Random.Range(2f,3f);
						TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

						Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),CurrentScale*2f);
						FluffObj.transform.localPosition = new Vector3(Position.x,CurrentHeight,Position.y);

						BranchDistance = 0f;
						BranchDirection += 150+Random.Range(-16.4f,16.4f);
					} else {
						BranchDistance += CurrentScale;
					}
				}

				CurrentHeight += CurrentScale;
			}
			break;
		case TreeCreatorTools.TreeShape.VShaped:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {

						if(leavesType == TreeCreatorTools.LeavesType.Prepared) {
							Obj = (GameObject)Instantiate(PreparedLeaf,transform);
							float Direction = Random.Range(0f,360f);
							Obj.transform.localPosition = new Vector3(TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).x,CurrentHeight,TreeCreatorTools.CircleCreator(0-Direction,CurrentScale/2f).y);
							Obj.transform.eulerAngles = new Vector3(0f,Direction, -135f);
							float BranchScale = 1.2f*TreeCreatorTools.Map(Mathf.Pow(tH,1.02f),TreeHeight*(HeightBeforeBranch/2f),TreeHeight,0f,1f);
							Obj.transform.localScale = new Vector3(BranchScale,BranchScale,BranchScale);

							//FRUITALIZE (Prep)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.7f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 7f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.7f,7f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(-PositionZ,-LoweringFactor,0);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));
							}

							BranchDistance = 0f;
						} else {
							//Setup a branch
							Obj = (GameObject)Instantiate(ObjectTemplate,transform);
							Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
							float THICCness = Random.Range(0.14987f,0.3756f)*CurrentScale;
							//RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*7.8f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f)
							Obj.transform.localScale = new Vector3(THICCness,8.2f*TreeCreatorTools.Map(Mathf.Pow(tH,1.12f),TreeHeight*(HeightBeforeBranch/2f),TreeHeight,0f,1f),THICCness);
							Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,30f);
							Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

							Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
							List<Vector2> UVs = new List<Vector2>();

							for(int i = 0; i < UVEditor.uv.Length; i++) {
								if(i < 4) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 6) { //8 -> 12 
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 8) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 10) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 12) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 16) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
								} else if(i < 20) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								} else if(i < 24) {
									UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
								}
							}
							Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

							//FRUITALIZE (Branch)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.1f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 1f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.1f,1f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

								FruitObj.transform.position -= Vector3.one*LoweringFactor;
							}

							BranchDistance = 0f;
							BranchDirection += 150+Random.Range(-16.4f,16.4f);
						}
					} else {
						BranchDistance += CurrentScale;
					}
				}

				CurrentHeight += CurrentScale;
			}

			if(leavesType == TreeCreatorTools.LeavesType.Prepared) {
				break;
			}

			FluffDensity = Mathf.RoundToInt(Random.Range((FoliageHeight*FoliageWidth)/2-5,(FoliageHeight*FoliageWidth)/2+5));
			//GameObject FluffObj;

			for(int f = 0; f < FluffDensity; f++) {
				//Random Height, Random Direction, Random Radius
				//Bettwen HeightBeforeBranch and HBB+FoliageHeight, Random Direction, Oval radius
				float Height = Random.Range(HeightBeforeBranch*TreeHeight,HeightBeforeBranch*TreeHeight+FoliageHeight);

				FluffObj = (GameObject)Instantiate(FluffyTemplate,transform);
				FluffObj.transform.localRotation = Random.rotationUniform;
				FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
				float Size = Random.Range(3f,4f);
				TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

				Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),(TreeCreatorTools.Map(Height,HeightBeforeBranch*TreeHeight,HeightBeforeBranch*TreeHeight+FoliageHeight,0f,1f))*FoliageWidth);
				FluffObj.transform.localPosition = new Vector3(Position.x,Height,Position.y);

			}
			break;
		case TreeCreatorTools.TreeShape.Broad:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			int beforeHeight = 0;

			int branchQuantity = Random.Range(MinBranchCount,MaxBranchCount);

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight*HeightBeforeBranch; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				beforeHeight = tH;
				CurrentHeight += CurrentScale;
			}

			float CurrentHeightSave = CurrentHeight;
			float BranchVariation = Random.Range(-20f,20f);
			int TreeHeightVariation = Random.Range(-5,5);
			float RotationVariation = Random.Range(5.8f,7.3f);

			for(int i = 0; i < branchQuantity; i++) {
				CurrentHeight = CurrentHeightSave;
				for(int tH = beforeHeight+1; tH < TreeHeight + TreeHeightVariation; tH++) { //Tree Height
					Obj = (GameObject)Instantiate(ObjectTemplate,transform);
					Vector2 Position = TreeCreatorTools.CircleCreator(i*(360f/branchQuantity)+BranchVariation,(float)(tH-beforeHeight)/(TreeHeight-beforeHeight)*RotationVariation);
					Obj.transform.localPosition = new Vector3(Position.x,CurrentHeight,Position.y);
					CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
					Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
					Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

					if((float)tH/TreeHeight > HeightBeforeBranch) {
						if(BranchDistance >= DistanceBettwenBranch) {
							//Setup a branch
							Obj = (GameObject)Instantiate(ObjectTemplate,transform);
							Obj.transform.localPosition = new Vector3(Position.x,CurrentHeight,Position.y);
							float THICCness2 = Random.Range(0.14987f,0.3756f)*CurrentScale;
							Obj.transform.localScale = new Vector3(THICCness2,Random.Range(MinBranchLenght,MaxBranchLenght)*CurrentScale,THICCness2);
							Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,60);
							Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

							Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
							List<Vector2> UVs = new List<Vector2>();

							for(int c = 0; c < UVEditor.uv.Length; c++) {
								if(c < 4) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.z,UVEditor.uv[c].y*Obj.transform.localScale.y));
								} else if(c < 6) { //8 -> 12 
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.x,UVEditor.uv[c].y*Obj.transform.localScale.z));
								} else if(c < 8) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.z,UVEditor.uv[c].y*Obj.transform.localScale.y));
								} else if(c < 10) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.x,UVEditor.uv[c].y*Obj.transform.localScale.z));
								} else if(c < 12) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.z,UVEditor.uv[c].y*Obj.transform.localScale.y));
								} else if(c < 16) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.x,UVEditor.uv[c].y*Obj.transform.localScale.z));
								} else if(c < 20) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.z,UVEditor.uv[c].y*Obj.transform.localScale.y));
								} else if(c < 24) {
									UVs.Add(new Vector2(UVEditor.uv[c].x*Obj.transform.localScale.z,UVEditor.uv[c].y*Obj.transform.localScale.y));
								}
							}
							Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

							//FRUITALIZE (Branch)
							if(Random.Range(0,2) == 0 && Fruit != null) {
								GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

								float PositionZ = 0f; 
								switch(fruitPosition) {
								case TreeCreatorTools.FruitPosition.MainBranchStart:
									PositionZ = 0.1f;
									break;
								case TreeCreatorTools.FruitPosition.MainBranchEnd:
									PositionZ = 1f;
									break;
								case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
									PositionZ = Random.Range(0.1f,1f);
									break;
								}

								FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
								FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

								switch(fruitSize) {
								case TreeCreatorTools.FruitSize.NoModification:
									//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
									break;
								case TreeCreatorTools.FruitSize.Constant:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
									break;
								case TreeCreatorTools.FruitSize.Random:
									TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
									break;
								}

								FruitObj.transform.parent = FruitObj.transform.parent.parent;

								FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
								if(AllowRandomYRotation) {
									FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
								}
								FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

								FruitObj.transform.position -= Vector3.one*LoweringFactor;
							}

							BranchDistance = 0f;
							BranchDirection += 150+Random.Range(-16.4f,16.4f);
						} else {
							BranchDistance += CurrentScale;
						}
					} else {
						//beforeRadius++;
					}

					CurrentHeight += CurrentScale;
				}
				BranchVariation = Random.Range(-20f,20f);
				TreeHeightVariation = Random.Range(-5,5);
				RotationVariation = Random.Range(5.8f,7.3f);
				BranchDirection = Random.Range(0f,360f);
			}

			FluffDensity = Mathf.RoundToInt(FlatFluffWidth*4f);
			//GameObject FluffObj;

			for(int f = 0; f < FluffDensity; f++) {
				//Random Height, Random Direction, Random Radius
				//Bettwen HeightBeforeBranch and HBB+FoliageHeight, Random Direction, Oval radius
				//float Height = Random.Range((CurrentHeight+(MinBranchLenght*0.4f)+0.6f),(CurrentHeight+(MaxBranchLenght*0.4f)));
				float Height = (CurrentHeight)+Random.Range(-FlatFluffHeight,FlatFluffHeight);

				FluffObj = (GameObject)Instantiate(FluffyTemplate,transform);
				FluffObj.transform.localRotation = Random.rotationUniform;
				FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
				float Size = Random.Range(3f,4f);
				TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

				Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),Random.Range(0f,FlatFluffWidth));
				FluffObj.transform.localPosition = new Vector3(Position.x,Height,Position.y);
			}

			break;
		case TreeCreatorTools.TreeShape.Layered:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			branchQuantity = Random.Range(MinBranchCount,MaxBranchCount);

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if(tH+1 >= TreeHeight) {
					for(int b = 0; b < branchQuantity; b++) {
						//Setup a branch
						Obj = (GameObject)Instantiate(ObjectTemplate,transform);
						Obj.transform.localPosition = new Vector3(0f,CurrentHeight+Random.Range(-(Mathf.Clamp(6.4f,0f,TreeHeight)),0),0f);
						float THICCness = Random.Range(0.14987f,0.3756f);
						//RoundFoliage.Evaluate(((float)tH-beforeRadius)*(1f/(TreeHeight-beforeRadius)))*7.8f+Random.Range(MinBranchLenght*0.075f,MaxBranchLenght*0.075f)
						Obj.transform.localScale = new Vector3(THICCness,Random.Range(MinBranchLenght,MaxBranchLenght),THICCness);
						Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,40f);
						Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

						Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
						List<Vector2> UVs = new List<Vector2>();

						for(int i = 0; i < UVEditor.uv.Length; i++) {
							if(i < 4) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 6) { //8 -> 12 
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 8) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 10) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 12) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 16) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 20) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 24) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							}
						}
						Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

						//FRUITALIZE (Branch)
						if(Random.Range(0,2) == 0 && Fruit != null) {
							GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

							float PositionZ = 0f; 
							switch(fruitPosition) {
							case TreeCreatorTools.FruitPosition.MainBranchStart:
								PositionZ = 0.1f;
								break;
							case TreeCreatorTools.FruitPosition.MainBranchEnd:
								PositionZ = 1f;
								break;
							case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
								PositionZ = Random.Range(0.1f,1f);
								break;
							}

							FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
							FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

							switch(fruitSize) {
							case TreeCreatorTools.FruitSize.NoModification:
								//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
								break;
							case TreeCreatorTools.FruitSize.Constant:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
								break;
							case TreeCreatorTools.FruitSize.Random:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
								break;
							}

							FruitObj.transform.parent = FruitObj.transform.parent.parent;

							FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
							if(AllowRandomYRotation) {
								FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
							}
							FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

							FruitObj.transform.position -= Vector3.one*LoweringFactor;
						}

						BranchDistance = 0f;
						BranchDirection += 150+Random.Range(-16.4f,16.4f);
					}
				}

				CurrentHeight += CurrentScale;
			}

			FluffDensity = Mathf.RoundToInt(FlatFluffWidth*3f);
			//GameObject FluffObj;

			for(int f = 0; f < FluffDensity; f++) {
				//Random Height, Random Direction, Random Radius
				//Bettwen HeightBeforeBranch and HBB+FoliageHeight, Random Direction, Oval radius
				float Height = Random.Range((CurrentHeight+(MinBranchLenght*0.4f)+0.6f),(CurrentHeight+(MaxBranchLenght*0.4f)));

				FluffObj = (GameObject)Instantiate(FluffyTemplate,transform);
				FluffObj.transform.localRotation = Random.rotationUniform;
				FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
				float Size = Random.Range(3f,4f);
				TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

				Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),Random.Range(0f,FlatFluffWidth));
				FluffObj.transform.localPosition = new Vector3(Position.x,Height,Position.y);

			}
			break;
		case TreeCreatorTools.TreeShape.JungleTree:
			TreeHeight = Random.Range(LogMinHeight,LogMaxHeight);
			TreeSize = Random.Range(LogMinSize,LogMaxSize);
			DistanceBettwenBranch = Random.Range(MinDistanceBettwenBranch,MaxDistanceBettwenBranch);
			CurrentHeight = 0;
			CurrentScale = 1f;
			beforeRadius = 0;

			branchQuantity = Random.Range(MinBranchCount,MaxBranchCount);

			BranchDistance = DistanceBettwenBranch;
			BranchDirection = Random.Range(0f,360f);

			//GameObject Obj;

			for(int tH = 0; tH < TreeHeight; tH++) { //Tree Height
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				CurrentScale = TreeSize * (logCurve.Evaluate((float)tH/TreeHeight)*logCurveInfluence);
				Obj.transform.localScale = new Vector3(CurrentScale,CurrentScale,CurrentScale);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				if((float)tH/TreeHeight > HeightBeforeBranch) {
					if(BranchDistance >= DistanceBettwenBranch) {
						//Setup a branch
						Obj = (GameObject)Instantiate(ObjectTemplate,transform);
						Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
						float THICCness = Random.Range(0.14987f,0.3756f)*CurrentScale;
						Obj.transform.localScale = new Vector3(THICCness,Random.Range(MinBranchLenght,MaxBranchLenght)*CurrentScale,THICCness);
						Obj.transform.eulerAngles = new Vector3(0f,BranchDirection,40);
						Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

						Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
						List<Vector2> UVs = new List<Vector2>();

						for(int i = 0; i < UVEditor.uv.Length; i++) {
							if(i < 4) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 6) { //8 -> 12 
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 8) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 10) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 12) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 16) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
							} else if(i < 20) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							} else if(i < 24) {
								UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
							}
						}
						Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

						//FRUITALIZE (Branch)
						if(Random.Range(0,2) == 0 && Fruit != null) {
							GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

							float PositionZ = 0f; 
							switch(fruitPosition) {
							case TreeCreatorTools.FruitPosition.MainBranchStart:
								PositionZ = 0.1f;
								break;
							case TreeCreatorTools.FruitPosition.MainBranchEnd:
								PositionZ = 1f;
								break;
							case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
								PositionZ = Random.Range(0.1f,1f);
								break;
							}

							FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
							FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

							switch(fruitSize) {
							case TreeCreatorTools.FruitSize.NoModification:
								//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
								break;
							case TreeCreatorTools.FruitSize.Constant:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
								break;
							case TreeCreatorTools.FruitSize.Random:
								TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
								break;
							}

							FruitObj.transform.parent = FruitObj.transform.parent.parent;

							FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
							if(AllowRandomYRotation) {
								FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
							}
							FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

							FruitObj.transform.position -= Vector3.one*LoweringFactor;
						}

						for(int fl = 0; fl < 5; fl++) {
							FluffObj = (GameObject)Instantiate(FluffyTemplate,Obj.transform);
							//FluffObj.transform.localRotation = Random.rotationUniform;
							FluffObj.transform.GetChild(0).GetComponent<MeshRenderer>().material = FluffLeavesMaterials;
							float Size = Random.Range(3f,4f);
							TreeCreatorTools.SetGlobalScale(FluffObj.transform,new Vector3(Size,Size,Size));

							Vector2 Position = TreeCreatorTools.CircleCreator(Random.Range(0f,360f),Random.Range(2f,3f));
							FluffObj.transform.localPosition = new Vector3(0f,1f,0f);
							FluffObj.transform.position = new Vector3(FluffObj.transform.position.x+Position.x,FluffObj.transform.position.y,FluffObj.transform.position.z+Position.y);
						}

						BranchDistance = 0f;
						BranchDirection += 150+Random.Range(-16.4f,16.4f);
					} else {
						BranchDistance += CurrentScale;
					}
				} else {
					//beforeRadius++;
				}

				CurrentHeight += CurrentScale;
			}

			for(int bq = 0; bq < branchQuantity; bq++) {
				Obj = (GameObject)Instantiate(ObjectTemplate,transform);
				Obj.transform.localPosition = new Vector3(0f,CurrentHeight,0f);
				float THICCness = Random.Range(0.14987f,0.3756f)*CurrentScale;
				Obj.transform.localScale = new Vector3(THICCness,Random.Range(MinBranchLenght/3f,MaxBranchLenght/3f)*CurrentScale,THICCness);
				Obj.transform.eulerAngles = new Vector3(0f,bq*(360f/branchQuantity)+Random.Range(-20f,20f),40);
				Obj.transform.GetChild(0).GetComponent<MeshRenderer>().material = LogMaterial;

				Mesh UVEditor = Obj.GetComponentInChildren<MeshFilter>().mesh;
				List<Vector2> UVs = new List<Vector2>();

				for(int i = 0; i < UVEditor.uv.Length; i++) {
					if(i < 4) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
					} else if(i < 6) { //8 -> 12 
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
					} else if(i < 8) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
					} else if(i < 10) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
					} else if(i < 12) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
					} else if(i < 16) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.x,UVEditor.uv[i].y*Obj.transform.localScale.z));
					} else if(i < 20) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
					} else if(i < 24) {
						UVs.Add(new Vector2(UVEditor.uv[i].x*Obj.transform.localScale.z,UVEditor.uv[i].y*Obj.transform.localScale.y));
					}
				}
				Obj.GetComponentInChildren<MeshFilter>().mesh.uv = UVs.ToArray();

				//FRUITALIZE (Branch)
				if(Random.Range(0,2) == 0 && Fruit != null) {
					GameObject FruitObj = (GameObject)Instantiate(Fruit,Obj.transform);

					float PositionZ = 0f; 
					switch(fruitPosition) {
					case TreeCreatorTools.FruitPosition.MainBranchStart:
						PositionZ = 0.1f;
						break;
					case TreeCreatorTools.FruitPosition.MainBranchEnd:
						PositionZ = 1f;
						break;
					case TreeCreatorTools.FruitPosition.TroughtoutMainBranch:
						PositionZ = Random.Range(0.1f,1f);
						break;
					}

					FruitObj.transform.localPosition = new Vector3(0f,PositionZ,0f);
					FruitObj.transform.localEulerAngles = new Vector3(0,0,0);

					switch(fruitSize) {
					case TreeCreatorTools.FruitSize.NoModification:
						//TreeCreatorTools.SetGlobalScale(FruitObj.transform,Vector3.one);
						break;
					case TreeCreatorTools.FruitSize.Constant:
						TreeCreatorTools.SetGlobalScale(FruitObj.transform,SIZEv1);
						break;
					case TreeCreatorTools.FruitSize.Random:
						TreeCreatorTools.SetGlobalScale(FruitObj.transform,new Vector3(Random.Range(SIZEv1.x,SIZEv2.x),Random.Range(SIZEv1.y,SIZEv2.y),Random.Range(SIZEv1.z,SIZEv2.z)));
						break;
					}

					FruitObj.transform.parent = FruitObj.transform.parent.parent;

					FruitObj.transform.eulerAngles = new Vector3(0f,FruitObj.transform.eulerAngles.y,0f);
					if(AllowRandomYRotation) {
						FruitObj.transform.eulerAngles = new Vector3(FruitObj.transform.eulerAngles.x,Random.Range(0,360),FruitObj.transform.eulerAngles.z);
					}
					FruitObj.transform.eulerAngles += new Vector3(Random.Range(-MaxXRotationRange,MaxXRotationRange),0f,Random.Range(-MaxZRotationRange,MaxZRotationRange));

					FruitObj.transform.position -= Vector3.one*LoweringFactor;
				}
			}
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		
}

[CustomEditor(typeof(BlockyTreeCreator))]
[CanEditMultipleObjects]
public class TreeEditor : Editor {
	BlockyTreeCreator btc;

	void OnEnable() {
		btc = (BlockyTreeCreator)target;
	}

	public override void OnInspectorGUI() {
		//btc.ac = EditorGUILayout.CurveField(btc.ac);

		EditorGUILayout.LabelField("ConsoleBlock Tree Creator - " + btc.TreeName,EditorStyles.centeredGreyMiniLabel);

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Tree Shape",EditorStyles.boldLabel);
		btc.treeShape = (TreeCreatorTools.TreeShape)EditorGUILayout.EnumPopup(btc.treeShape);
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Leaves Type",EditorStyles.boldLabel);
		bool both = false;
		if(btc.treeShape == TreeCreatorTools.TreeShape.Pyramidal || btc.treeShape == TreeCreatorTools.TreeShape.Fountain) {
			btc.leavesType = TreeCreatorTools.LeavesType.Prepared;
			EditorGUILayout.LabelField("Prepared (Locked)",EditorStyles.miniLabel);
		} else if(btc.treeShape == TreeCreatorTools.TreeShape.Oval || btc.treeShape == TreeCreatorTools.TreeShape.Layered || btc.treeShape == TreeCreatorTools.TreeShape.Broad || btc.treeShape == TreeCreatorTools.TreeShape.JungleTree) {
			btc.leavesType = TreeCreatorTools.LeavesType.Fluff;
			EditorGUILayout.LabelField("Fluff (Locked)",EditorStyles.miniLabel);
		} else if(btc.treeShape == TreeCreatorTools.TreeShape.Weeping) {
			EditorGUILayout.LabelField("Both (Locked)",EditorStyles.miniLabel);
			both = true;
		} else {
			btc.leavesType = (TreeCreatorTools.LeavesType)EditorGUILayout.EnumPopup(btc.leavesType);
		}
		if(btc.leavesType == TreeCreatorTools.LeavesType.Fluff) {
			btc.FluffLeavesMaterials = (Material)EditorGUILayout.ObjectField(btc.FluffLeavesMaterials,typeof(Material),false);
		} else if(!both) {
			btc.PreparedLeaf = (GameObject)EditorGUILayout.ObjectField(btc.PreparedLeaf,typeof(GameObject),true);
		} else {
			btc.FluffLeavesMaterials = (Material)EditorGUILayout.ObjectField(btc.FluffLeavesMaterials,typeof(Material),false);
			btc.PreparedLeaf = (GameObject)EditorGUILayout.ObjectField(btc.PreparedLeaf,typeof(GameObject),true);
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Tree Log",EditorStyles.boldLabel);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Log Size Reduction Curve");
		btc.logCurve = EditorGUILayout.CurveField(btc.logCurve);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Curve Influence Over Log");
		btc.logCurveInfluence = EditorGUILayout.FloatField(btc.logCurveInfluence);
		EditorGUILayout.EndHorizontal();
		btc.LogMaterial = (Material)EditorGUILayout.ObjectField(btc.LogMaterial,typeof(Material),false);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Log Height");
		btc.LogMinHeight = EditorGUILayout.IntField(btc.LogMinHeight,GUILayout.MaxWidth(60));
		btc.LogMaxHeight = EditorGUILayout.IntField(btc.LogMaxHeight,GUILayout.MaxWidth(60));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Log Size");
		btc.LogMinSize = EditorGUILayout.FloatField(btc.LogMinSize,GUILayout.MaxWidth(60));
		btc.LogMaxSize = EditorGUILayout.FloatField(btc.LogMaxSize,GUILayout.MaxWidth(60));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Parameters",EditorStyles.boldLabel);

		switch(btc.treeShape) {
		case TreeCreatorTools.TreeShape.Broad:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Big Branch", btc.HeightBeforeBranch,0.0f,1.0f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Big Branch Count");
			btc.MinBranchCount = EditorGUILayout.IntField(btc.MinBranchCount,GUILayout.MaxWidth(60));
			btc.MaxBranchCount = EditorGUILayout.IntField(btc.MaxBranchCount,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Lenght");
			btc.MinBranchLenght = EditorGUILayout.FloatField(btc.MinBranchLenght,GUILayout.MaxWidth(60));
			btc.MaxBranchLenght = EditorGUILayout.FloatField(btc.MaxBranchLenght,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.FlatFluffWidth = EditorGUILayout.Slider("Fluff Group Width", btc.FlatFluffWidth, 0.1f,19f);
			btc.FlatFluffHeight = EditorGUILayout.Slider("Fluff Group Height", btc.FlatFluffHeight, 0.1f,7f);
			break;
		case TreeCreatorTools.TreeShape.Columnar:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Branch", btc.HeightBeforeBranch,0.0f,1.0f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.BranchRotation = EditorGUILayout.Slider("Average Branch Rotation", btc.BranchRotation,110f,170f);
			btc.FoliageWidthInfluenceOverRotation = EditorGUILayout.FloatField("F. Influence Over Rot. ", btc.FoliageWidthInfluenceOverRotation);
			break;
		case TreeCreatorTools.TreeShape.Fountain:
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Count");
			btc.MinBranchCount = EditorGUILayout.IntField(btc.MinBranchCount,GUILayout.MaxWidth(60));
			btc.MaxBranchCount = EditorGUILayout.IntField(btc.MaxBranchCount,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.LeafSize = EditorGUILayout.Slider("Leaves Size", btc.LeafSize, 2f,0.01f);
			btc.BranchRotation = EditorGUILayout.Slider("Average Branch Rotation", btc.BranchRotation,40f,130f);
			btc.LeavesZRotationVariationRange = EditorGUILayout.Slider("Branch Rot. Variation Range", btc.LeavesZRotationVariationRange,0.0f,60.0f);
			btc.LeavesYRotationVariationRange = EditorGUILayout.Slider("Branch Y Rot. Variation Range", btc.LeavesYRotationVariationRange,0.0f,60.0f);
			btc.LogCurving = EditorGUILayout.CurveField("Log Curve",btc.LogCurving);
			btc.LogCurvingIntensity = EditorGUILayout.FloatField("Log Curve Intensity", btc.LogCurvingIntensity);
			break;
		case TreeCreatorTools.TreeShape.JungleTree:
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Lenght");
			btc.MinBranchLenght = EditorGUILayout.FloatField(btc.MinBranchLenght,GUILayout.MaxWidth(60));
			btc.MaxBranchLenght = EditorGUILayout.FloatField(btc.MaxBranchLenght,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			/*EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Top Part Branch Count");
			btc.MinBranchCount = EditorGUILayout.IntField(btc.MinBranchCount,GUILayout.MaxWidth(60));
			btc.MaxBranchCount = EditorGUILayout.IntField(btc.MaxBranchCount,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.FlatFluffWidth = EditorGUILayout.Slider("Top Fluff Group Width", btc.FlatFluffWidth,0.1f,7f);
			btc.FlatFluffHeight = EditorGUILayout.Slider("Top Fluff Group Height", btc.FlatFluffHeight,0.1f,7f);*/
			break;
		case TreeCreatorTools.TreeShape.Layered:
			btc.FlatFluffWidth = EditorGUILayout.Slider("Top Fluff Group Width", btc.FlatFluffWidth,0.1f,13f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Count");
			btc.MinBranchCount = EditorGUILayout.IntField(btc.MinBranchCount,GUILayout.MaxWidth(60));
			btc.MaxBranchCount = EditorGUILayout.IntField(btc.MaxBranchCount,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Lenght");
			btc.MinBranchLenght = EditorGUILayout.FloatField(btc.MinBranchLenght,GUILayout.MaxWidth(60));
			btc.MaxBranchLenght = EditorGUILayout.FloatField(btc.MaxBranchLenght,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			break;
		case TreeCreatorTools.TreeShape.Oval:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Foliage", btc.HeightBeforeBranch,0.0f,1.0f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Foliage Size (Width,Height)");
			btc.FoliageWidth = EditorGUILayout.FloatField(btc.FoliageWidth,GUILayout.MaxWidth(60));
			btc.FoliageHeight = EditorGUILayout.FloatField(btc.FoliageHeight,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.BranchRotation = EditorGUILayout.Slider("Average Branch Rotation", btc.BranchRotation,60f,130f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			break;
		case TreeCreatorTools.TreeShape.Pyramidal:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Foliage", btc.HeightBeforeBranch,0.0f,1.0f);
			btc.BottomFolliageExtention = EditorGUILayout.FloatField("Bottom Radius", btc.BottomFolliageExtention);
			btc.MiddleFolliageExtention = EditorGUILayout.Slider("Smart Middle Radius", btc.MiddleFolliageExtention,0.0f,btc.BottomFolliageExtention);
			btc.BranchRotation = EditorGUILayout.Slider("Average Branch Rotation", btc.BranchRotation,60f,130f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			break;
		case TreeCreatorTools.TreeShape.Round:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Foliage", btc.HeightBeforeBranch,0.0f,1.0f);
			btc.BranchRotation = EditorGUILayout.Slider("Average Branch Rotation", btc.BranchRotation,60f,130f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Distance Bettween Branch");
			btc.MinDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MinDistanceBettwenBranch,GUILayout.MaxWidth(60));
			btc.MaxDistanceBettwenBranch = EditorGUILayout.FloatField(btc.MaxDistanceBettwenBranch,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Round Fluff Size");
			btc.RoundFluffMinSize = EditorGUILayout.FloatField(btc.RoundFluffMinSize,GUILayout.MaxWidth(60));
			btc.RoundFluffMaxSize = EditorGUILayout.FloatField(btc.RoundFluffMaxSize,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Lenght");
			btc.MinBranchLenght = EditorGUILayout.FloatField(btc.MinBranchLenght,GUILayout.MaxWidth(60));
			btc.MaxBranchLenght = EditorGUILayout.FloatField(btc.MaxBranchLenght,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			break;
		case TreeCreatorTools.TreeShape.VShaped:
			btc.HeightBeforeBranch = EditorGUILayout.Slider("Height Before Foliage", btc.HeightBeforeBranch,0.0f,1.0f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Foliage Size (Width,Height)");
			btc.FoliageWidth = EditorGUILayout.FloatField(btc.FoliageWidth,GUILayout.MaxWidth(60));
			btc.FoliageHeight = EditorGUILayout.FloatField(btc.FoliageHeight,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			break;
		case TreeCreatorTools.TreeShape.Weeping:
			btc.CrookingIntensity = EditorGUILayout.Slider("Crooking Intensity",btc.CrookingIntensity,0.0f,1.0f);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Count");
			btc.MinBranchCount = EditorGUILayout.IntField(btc.MinBranchCount,GUILayout.MaxWidth(60));
			btc.MaxBranchCount = EditorGUILayout.IntField(btc.MaxBranchCount,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Branch Lenght");
			btc.MinBranchLenght = EditorGUILayout.FloatField(btc.MinBranchLenght,GUILayout.MaxWidth(60));
			btc.MaxBranchLenght = EditorGUILayout.FloatField(btc.MaxBranchLenght,GUILayout.MaxWidth(60));
			EditorGUILayout.EndHorizontal();
			btc.FlatFluffWidth = EditorGUILayout.Slider("Fluff Group Width", btc.FlatFluffWidth, 0.1f,7f);
			btc.FlatFluffHeight = EditorGUILayout.Slider("Fluff Group Height", btc.FlatFluffHeight, 0.1f,7f);
			break;
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical(EditorStyles.helpBox);
		EditorGUILayout.LabelField("Fruits",EditorStyles.boldLabel);
		btc.Fruit = (GameObject)EditorGUILayout.ObjectField(btc.Fruit,typeof(GameObject),true);
		GUILayout.Space(10);
		EditorGUILayout.LabelField("Size",EditorStyles.miniLabel);
		btc.fruitSize = (TreeCreatorTools.FruitSize)EditorGUILayout.EnumPopup(btc.fruitSize);
		switch(btc.fruitSize) {
		case TreeCreatorTools.FruitSize.Constant:
			btc.SIZEv1 = (Vector3)EditorGUILayout.Vector3Field("Size",btc.SIZEv1);
			break;
		case TreeCreatorTools.FruitSize.Random:
			btc.SIZEv1 = (Vector3)EditorGUILayout.Vector3Field("Min Size",btc.SIZEv1);
			btc.SIZEv2 = (Vector3)EditorGUILayout.Vector3Field("Max Size",btc.SIZEv2);
			break;
		}
		GUILayout.Space(5);
		EditorGUILayout.LabelField("Rotation",EditorStyles.miniLabel);
		btc.AllowRandomYRotation = (bool)EditorGUILayout.Toggle("Allow Random Y Rotation", btc.AllowRandomYRotation);
		btc.MaxXRotationRange = (float)EditorGUILayout.Slider("Max X Rotation",btc.MaxXRotationRange,0f,70f);
		btc.MaxZRotationRange = (float)EditorGUILayout.Slider("Max Z Rotation",btc.MaxZRotationRange,0f,70f);

		GUILayout.Space(5);
		EditorGUILayout.LabelField("Position",EditorStyles.miniLabel);
		btc.fruitPosition = (TreeCreatorTools.FruitPosition)EditorGUILayout.EnumPopup(btc.fruitPosition);
		btc.LoweringFactor = EditorGUILayout.FloatField("Fruit Lowering",btc.LoweringFactor);
		EditorGUILayout.EndVertical();

		btc.Seed = EditorGUILayout.IntField("Tree Seed",btc.Seed);
		btc.TreeName = EditorGUILayout.TextField("Tree Name",btc.TreeName);
		if(btc.leavesType == TreeCreatorTools.LeavesType.Fluff) {
			btc.FluffyTemplate = (GameObject)EditorGUILayout.ObjectField("Fluffy Leaves Template",btc.FluffyTemplate,typeof(GameObject));
		}
		btc.ObjectTemplate = (GameObject)EditorGUILayout.ObjectField("Cube Template",btc.ObjectTemplate,typeof(GameObject));

		//btc.RoundFoliage = EditorGUILayout.CurveField(btc.RoundFoliage);
	}
}

public static class TreeCreatorTools {
	
	public static void SetGlobalScale (this Transform transform, Vector3 globalScale) {
		transform.localScale = Vector3.one;
		transform.localScale = new Vector3 (globalScale.x/transform.lossyScale.x, globalScale.y/transform.lossyScale.y, globalScale.z/transform.lossyScale.z);
	}

	public static Vector2 CircleCreator(float Angle, float Radius) {
		return new Vector2(Mathf.Cos(Angle*Mathf.Deg2Rad)*Radius,Mathf.Sin(Angle*Mathf.Deg2Rad)*Radius);
	}

	public static float Map (this float value, float fromSource, float toSource, float fromTarget, float toTarget)
	{
		return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
	}

	public enum LeavesType {
		Prepared,
		Fluff
	}

	public enum TreeShape {
		Pyramidal,
		Round,
		Columnar,
		Fountain,
		Oval,
		Weeping,
		VShaped,
		Broad,
		Layered,
		JungleTree
	}

	public enum FruitSize {
		NoModification,
		Constant,
		Random
	}

	public enum FruitPosition {
		MainBranchStart,
		MainBranchEnd,
		TroughtoutMainBranch
	}
}