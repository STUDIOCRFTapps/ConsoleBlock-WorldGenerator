using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class MenuUIManager : MonoBehaviour {

	public string ThemeFileDirectory = "Assets/Resources/TitleThemes/Main.txt";
	public string ThemeTitle = "Main";
	public Color MainUIColor;
	public Color BackgroundTopColor;
	public Color BackgroundBottomColor;
	public float BackgroundHueScale = 0.1f;

	public Color TextGradientColor;

	public Color ParticleColor;
	public float ParticlesHueScale = 0.07f;

	public Color CubeParticleColor1;
	public Color CubeParticleColor2;

	public Material SkyboxMaterial;

	public ParticleSystem[] ParticleSystems;
	public ParticleSystem[] CubeParticleSystems;
	public TextMeshProUGUI[] TextMeshes;
	Material newSkyboxMaterial;
	Skybox skybox;

	public bool UpdateColors = false;

	void Start () {
		//Get themes color
		string theme;
		StreamReader reader = new StreamReader(ThemeFileDirectory);
		theme = reader.ReadToEnd();
		reader.Close();

		string[] themeLines = theme.Split('\n');
		for(int i = 0; i < themeLines.Length; i++) {
			if(!themeLines[i].Contains(": ")) {
				continue;
			}
			string StartCode = themeLines[i].Split(':')[0];
			StartCode = StartCode.Replace("\t",string.Empty);
			string Result = themeLines[i].Split(':')[1];
			Result = Result.Remove(0,1);
			float v = 0.003921568627f;

			switch(StartCode) {
			case "Title":
				ThemeTitle = Result;
				break;
			case "Main":
				MainUIColor = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "BackgroundTop":
				BackgroundTopColor = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "BackgroundBottom":
				BackgroundBottomColor = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "BackgroundHueScale":
				BackgroundHueScale = float.Parse(Result);
				break;
			case "TextGradientColor":
				TextGradientColor = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "ParticleColor":
				ParticleColor = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "ParticleHueScale":
				BackgroundHueScale = float.Parse(Result);
				break;
			case "CubeParticleColor1":
				CubeParticleColor1 = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			case "CubeParticleColor2":
				CubeParticleColor2 = new Color(float.Parse(Result.Split(',')[0])*v,float.Parse(Result.Split(',')[1])*v,float.Parse(Result.Split(',')[2])*v,float.Parse(Result.Split(',')[3])*v);
				break;
			}
		}

		//Verify if the skybox material is there
		if(SkyboxMaterial == null) {
			Debug.LogError("SkyboxMaterial is not assigned.");
		}

		//Verify and get the skybox component
		skybox = GetComponent<Skybox>();
		if(skybox == null) {
			Debug.LogError("Add the Skybox component on this object.");
		}

		//Assing the material
		newSkyboxMaterial = new Material(SkyboxMaterial);
		skybox.material = newSkyboxMaterial;

		UpdateMenuColors();
	}

	void Update () {
		//Verify If a Color Update is needed
		if(UpdateColors) {
			//Update Colors
			UpdateMenuColors();
			UpdateColors = false;
		}
	}

	void UpdateMenuColors () {
		//Get main colors
		float MainCHue, MainCSaturation, MainCDarkness;
		Color.RGBToHSV(MainUIColor,out MainCHue,out MainCSaturation,out MainCDarkness);

		//Get particles colors
		float PCHue, PCSaturation, PCDarkness;
		Color.RGBToHSV(ParticleColor,out PCHue,out PCSaturation,out PCDarkness);

		//Get text colors
		float TCHue, TCSaturation, TCDarkness;
		Color.RGBToHSV(TextGradientColor,out TCHue,out TCSaturation,out TCDarkness);

		//Set skybox colors
		newSkyboxMaterial.SetColor("_Color1",BackgroundTopColor);
		newSkyboxMaterial.SetColor("_Color2",BackgroundBottomColor);

		//Prepare particle color
		Color maxPColor = Color.HSVToRGB(PCHue+ParticlesHueScale,PCSaturation,PCDarkness);
		maxPColor.a = ParticleSystems[0].main.startColor.colorMax.a;
		Color minPColor = Color.HSVToRGB(PCHue-ParticlesHueScale,PCSaturation,PCDarkness);
		maxPColor.a = ParticleSystems[0].main.startColor.colorMax.a;

		//Prepare text color
		Color TextGradient1 = Color.HSVToRGB(TCHue+0.07f,TCSaturation, TCDarkness);
		Color TextGradient2 = Color.HSVToRGB(TCHue,TCSaturation, TCDarkness);
		Color TextGradient3 = Color.HSVToRGB(TCHue-0.07f,TCSaturation, TCDarkness-0.07f);
		Color TextGradient4 = Color.HSVToRGB(TCHue+0.14f,TCSaturation, TCDarkness+0.07f);

		//Set text color
		foreach(TextMeshProUGUI txt in TextMeshes) {
			txt.colorGradient = new VertexGradient(TextGradient1,TextGradient2,TextGradient3,TextGradient4);
		}

		//Set particle color
		foreach(ParticleSystem ps in ParticleSystems) {
			ParticleSystem.MainModule newMain = ps.main;
			newMain.startColor = new ParticleSystem.MinMaxGradient(minPColor,maxPColor);
		}

		//Set cube particle color
		foreach(ParticleSystem ps in CubeParticleSystems) {
			ParticleSystem.MainModule newMain = ps.main;
			newMain.startColor = new ParticleSystem.MinMaxGradient(CubeParticleColor1,CubeParticleColor2);
		}
	}
}
