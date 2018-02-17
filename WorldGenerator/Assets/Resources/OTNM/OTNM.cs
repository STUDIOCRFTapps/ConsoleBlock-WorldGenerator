using System;
using System.Collections;
using System.Collections.Generic;
using XnaGeometry;
using FastNoiseLibrary;

namespace OTNM {
	public class TerrainNoiseGroup {
		List<TerrainNoise> TerrainNoises;

		public TerrainNoiseGroup() {
			TerrainNoises = new List<TerrainNoise>();
		}

		public TerrainNoiseError Add (TerrainNoise Noise) {
			if(Noise == null) {
				return new TerrainNoiseError("Couldn't add the TerrainNoise to the List, the Noise was null.", TerrainNoiseError.ErrorTypes.Warning);
			} else {
				TerrainNoises.Add(Noise);
				return null;
			}
		}

		public TerrainNoiseError Remove (int Index) {
			if(Index != MathFunc.Clamp(Index,0,TerrainNoises.Count-1)) {
				return new TerrainNoiseError("Couldn't remove the TerrainNoise at Index, the Index was out of range.", TerrainNoiseError.ErrorTypes.Warning);
			} else {
				TerrainNoises.RemoveAt(Index);
				return null;
			}
		}

		public float GetNoiseValue (int TerrainNoiseIndex, Vector2 Position, out TerrainNoiseError Error) {
			Error = null;
			if(TerrainNoiseIndex != MathFunc.Clamp(TerrainNoiseIndex,0,TerrainNoises.Count-1)) {
				Error = new TerrainNoiseError("Couldn't get the value of the TerrainNoise at Index, the Index was out of range.", TerrainNoiseError.ErrorTypes.Warning);
				return 0f;
			} else {
				return TerrainNoises[TerrainNoiseIndex].GetNoiseValue(Position, out Error);
			}
		}

		public float GetNoiseValue (int TerrainNoiseIndex, Vector2 Position) {
			if(TerrainNoiseIndex != MathFunc.Clamp(TerrainNoiseIndex,0,TerrainNoises.Count-1)) {
				return 0f;
			} else {
				return TerrainNoises[TerrainNoiseIndex].GetNoiseValue(Position);
			}
		}
	}

	public class TerrainNoise {
		TerrainType terrainType = TerrainType.SimplexNoise;
		public int seed;
		public float Frequency = 1f;
		Tools.Accessing.CanyonParameters defaultParams = new Tools.Accessing.CanyonParameters(0f,0f,new OTNM.Tools.Accessing.CanyonParameters.CanyonKey[]{new OTNM.Tools.Accessing.CanyonParameters.CanyonKey(0f,0f)});
		Tools.Accessing.CanyonParameters canyonParameters;
		Tools.Accessing.NoiseParameters noiseParameters;

		FastNoise fnoise;
		TerrainNoiseError error;
		bool BlockedByError = false;

		public TerrainNoise (TerrainType terrainType, int seed, Tools.Accessing.NoiseParameters noiseParameters, Tools.Accessing.CanyonParameters canyonParameters, float noisefrequency = 1) {
			this.terrainType = terrainType;
			this.seed = seed;
			this.noiseParameters = noiseParameters;
			this.canyonParameters = canyonParameters;
			Frequency = noisefrequency;

			fnoise = new FastNoise(seed);

			//Debugging
			if(terrainType == TerrainType.CustomCayonNoise) {
				if(canyonParameters == null) {
					BlockedByError = true;
					error = new TerrainNoiseError("The TerrainNoise cannot return any value, the canyonParameters equals null.", TerrainNoiseError.ErrorTypes.Error);
				}
				if(canyonParameters.sectionsLength.Length == 0) {
					BlockedByError = true;
					error = new TerrainNoiseError("The TerrainNoise cannot return any value, the canyonParameters has no keys.", TerrainNoiseError.ErrorTypes.Error);
				}
			}
			if(noiseParameters.octaves == 0) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noiseParameters has no octaves.", TerrainNoiseError.ErrorTypes.Error);
			}
		}

		public TerrainNoise (TerrainType terrainType, int seed, Tools.Accessing.NoiseParameters noiseParameters, float noiseFrequency = 1) {
			this.terrainType = terrainType;
			this.seed = seed;
			this.noiseParameters = noiseParameters;
			Frequency = noiseFrequency;

			fnoise = new FastNoise(seed);

			//Debugging
			if(terrainType == TerrainType.CustomCayonNoise) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noise requires cayonParameters.", TerrainNoiseError.ErrorTypes.Error);
			}
			if(noiseParameters.octaves == 0) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noiseParameters has no octaves.", TerrainNoiseError.ErrorTypes.Error);
			}
		}

		public float GetNoiseValue (Vector2 Position, out TerrainNoiseError Error) {
			Error = null;
			if(BlockedByError) {
				Error = error;
				return 0f;
			}

			return PrivateGetNoiseValue(Position*Frequency);
		}

		public float GetNoiseValue (Vector2 Position) {
			if(BlockedByError) {
				return 0f;
			}

			return PrivateGetNoiseValue(Position*Frequency);
		}

		float PrivateGetNoiseValue (Vector2 p) {
			float v = 0f;

			switch(terrainType) {
			case TerrainType.SimplexNoise:
				v = Tools.Accessing.GetNoise(p,fnoise,seed);
				break;
			case TerrainType.FractalSimplexNoise:
				v = Tools.Accessing.GetFractalNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.FractalPerlinErosionNoise:
				v = Tools.Accessing.GetFractalErosionNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.FractalBillowNoise:
				v = Tools.Accessing.GetFractalBillowyNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.FractalBillowErosionNoise:
				v = Tools.Accessing.GetFractalErosionBillowyNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.FractalRidgedNoise:
				v = Tools.Accessing.GetFractalRidgedNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.FractalRidgedErosionNoise:
				v = Tools.Accessing.GetFractalErosionRidgedNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.CustomAlpsNoise:
				v = Tools.Accessing.GetFractalAlpsNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.CustomCayonNoise:
				v = Tools.Accessing.GetCanyonNoise(p,noiseParameters,fnoise,canyonParameters,seed);
				break;
			case TerrainType.CustomPlainsNoise:
				v = Tools.Accessing.GetFractalPlainNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.CustomRiverPlainsNoise:
				v = Tools.Accessing.GetFractalWarpedRiversNoise(p,noiseParameters,fnoise,seed);
				break;
			case TerrainType.InvertPerlinErosionNoise:
				v = Tools.Accessing.GetFractalInverseErosionNoise(p,noiseParameters,fnoise,seed);
				break;
			}

			return v;
		}

		public TerrainNoiseError GetError () {
			return error;
		}

		public void ChangeTerrainType (TerrainType terrainType, Tools.Accessing.NoiseParameters noiseParameters, Tools.Accessing.CanyonParameters canyonParameters) {
			BlockedByError = false;
			this.terrainType = terrainType;
			this.seed = seed;
			this.noiseParameters = noiseParameters;
			this.canyonParameters = canyonParameters;

			fnoise = new FastNoise(seed);

			//Debugging
			if(terrainType == TerrainType.CustomCayonNoise) {
				if(canyonParameters == null) {
					BlockedByError = true;
					error = new TerrainNoiseError("The TerrainNoise cannot return any value, the canyonParameters equals null.", TerrainNoiseError.ErrorTypes.Error);
				}
				if(canyonParameters.sectionsLength.Length == 0) {
					BlockedByError = true;
					error = new TerrainNoiseError("The TerrainNoise cannot return any value, the canyonParameters has no keys.", TerrainNoiseError.ErrorTypes.Error);
				}
			}
			if(noiseParameters.octaves == 0) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noiseParameters has no octaves.", TerrainNoiseError.ErrorTypes.Error);
			}
		}

		public void ChangeTerrainType (TerrainType terrainType, Tools.Accessing.NoiseParameters noiseParameters) {
			BlockedByError = false;
			this.terrainType = terrainType;
			this.seed = seed;
			this.noiseParameters = noiseParameters;

			fnoise = new FastNoise(seed);

			//Debugging
			if(terrainType == TerrainType.CustomCayonNoise) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noise requires cayonParameters.", TerrainNoiseError.ErrorTypes.Error);
			}
			if(noiseParameters.octaves == 0) {
				BlockedByError = true;
				error = new TerrainNoiseError("The TerrainNoise cannot return any value, the noiseParameters has no octaves.", TerrainNoiseError.ErrorTypes.Error);
			}
		}

		public enum TerrainType {
			SimplexNoise,
			FractalSimplexNoise,
			FractalPerlinErosionNoise,
			FractalBillowNoise,
			FractalBillowErosionNoise,
			FractalRidgedNoise,
			FractalRidgedErosionNoise,
			CustomAlpsNoise,
			CustomRiverPlainsNoise,
			CustomPlainsNoise,
			CustomCayonNoise,
			InvertPerlinErosionNoise
		}
	}

	public class TerrainNoiseError {
		public string Message = "No Error";
		public ErrorTypes ErrorType;

		public TerrainNoiseError(string Message, ErrorTypes ErrorType) {
			this.Message = Message;
			this.ErrorType = ErrorType;
		}

		public enum ErrorTypes {
			Warning,
			Error,
			NoError
		}
	}
}

namespace OTNM.Tools {
	public static class Accessing {
		public static Vector3 GetDerivativePerlinNoise(Vector2 p, FastNoise noise, int seed = 0) {
			return noise.GetPerlinDerivatives(p,seed);
		}

		public static float GetNoise(Vector2 p, FastNoise noise, int seed = 0) {
			noise.SetSeed(seed);
			return noise.GetSimplex(p.x,p.y);
		}

		public static float GetFractalNoise(Vector2 p, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			noise.SetSeed(seed);
			float sum = 0f;
			float amp = 1.0f;
			for(int i = 0; i < noiseParameters.octaves; i++) {
				noise.SetSeed(seed+i);
				float n = noise.GetPerlin(p.x,p.y);

				sum += n*amp;
				p *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalBillowyNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			for(int i = 0; i < noiseParameters.octaves; i++) {
				float n = (GetNoise(position*freq,noise,seed+i)+1)/2f;
				n = MathFunc.Abs(n)*2f-1;
				//n = 1f-MathFunc.PingPong(n,0.5f)*2f;
				sum += amp * n;
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalErosionBillowyNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq,noise,seed+i);
				n.x = MathFunc.Abs(n.x)*2f-1;
				n.y = MathFunc.Abs(n.y)*2f-1;
				n.z = MathFunc.Abs(n.z)*2f-1;
				//n.x = MathFunc.PingPong(n.x,0.5f)*2f; //to fix
				//n.y = MathFunc.PingPong(n.y,0.5f)*2f;
				//n.z = MathFunc.PingPong(n.z,0.5f)*2f;


				dsum += new Vector2(n.y,n.z);
				sum += amp * n.x / (1 + Vector2.Dot(dsum, dsum));
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalRidgedNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			for(int i = 0; i < noiseParameters.octaves; i++) {
				float n = GetNoise(position*freq,noise,seed+i);
				n = ((1f-MathFunc.Abs(n))*2f-1);
				//n = MathFunc.PingPong(n,0.5f)*2f;
				sum += amp * n;
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalErosionRidgedNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq,noise,seed+i);
				n.x = ((1f-MathFunc.Abs(n.x))*2f-1);
				n.y = ((1f-MathFunc.Abs(n.y))*2f-1);
				n.z = ((1f-MathFunc.Abs(n.z))*2f-1);
				//n.x = 1f-MathFunc.PingPong(n.x,0.5f)*2f;
				//n.y = 1f-MathFunc.PingPong(n.y,0.5f)*2f;
				//n.z = 1f-MathFunc.PingPong(n.z,0.5f)*2f;
				//n.x = MathFunc.Pow(n.x,2);

				dsum += new Vector2(n.y,n.z);
				sum += amp * n.x / (1 + Vector2.Dot(dsum, dsum));
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalErosionNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq,noise,seed+i);
				dsum += new Vector2(n.y,n.z);
				sum += amp * n.x / (1 + Vector2.Dot(dsum, dsum));
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalInverseErosionNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			return 1f-GetFractalErosionNoise(position,noiseParameters,noise,seed)-1f;
		}

		public static float GetFractalWarpedRiversNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			Vector2 sum_warp = new Vector2(0,0);
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq+sum_warp,noise,seed+i);
				dsum += new Vector2(n.y,n.z);
				sum += amp * n.x / (1 + Vector2.Dot(dsum, dsum));
				sum_warp += new Vector2(dsum.x*(1/freq),dsum.y*(0.7f/freq));
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalPlainNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			float derivative_sum = 1.0f;
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq,noise,seed+i);
				derivative_sum = (1f-((Math.Abs(n.y)+Math.Abs(n.z))*0.5f))*sum;
				dsum += new Vector2(n.y,n.z)*(derivative_sum*5f);
				sum += amp * n.x / (1 + Vector2.Dot(dsum, dsum));
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain;
			}
			return sum;
		}

		public static float GetFractalAlpsNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, int seed = 0) {
			float warp = 1.5f;
			float sum = 0f;
			float freq = 1.0f, amp = 1.0f;
			Vector2 dsum = new Vector2(0,0);
			for(int i = 0; i < noiseParameters.octaves; i++) {
				Vector3 n = GetDerivativePerlinNoise(position*freq,noise,seed+i);
				sum += amp * (1 - MathFunc.Abs(n.x));
				dsum += amp * new Vector2(n.y,n.z) * -n.x;
				freq *= noiseParameters.lacunarity;
				amp *= noiseParameters.gain * MathFunc.Clamp01(sum);
			}
			return sum;
		}

		public static float GetCanyonNoise(Vector2 position, NoiseParameters noiseParameters, FastNoise noise, CanyonParameters canyonParameters, int seed = 0) {
			//Noise Height, Value bettween CayonParameters's limits, Results
			float n = 1.0f-GetFractalErosionNoise(position,noiseParameters,noise,seed);
			float n2 = MathFunc.Clamp01(MathFunc.Lerp(0f,1f,MathFunc.InverseLerp(canyonParameters.minCanyonHeight,canyonParameters.canyonPlateauHeight,n)));
			float h = 0f;
			float t = (canyonParameters.canyonPlateauHeight-canyonParameters.minCanyonHeight);

			if(n <= canyonParameters.canyonPlateauHeight) {
				if(n >= canyonParameters.minCanyonHeight) {
					h = canyonParameters.GetValueAtHeight(n2);
					return h;
				} else {
					return canyonParameters.minCanyonHeight-n*-3f;
				}
			} else {
				//h = (n*(canyonParameters.canyonPlateauHeight-canyonParameters.minCanyonHeight)/2f)+1f;
				//h = (n-t)*0.2f+t;
				float x = 1f;
				//h = (n)*x+(t*(x*2)+0);
				h = 1+(n-canyonParameters.canyonPlateauHeight)*0.2f;
				return h;
			}
		}

		public class CanyonParameters {
			public float minCanyonHeight = 0.1f;
			public float canyonPlateauHeight = 0.95f;

			private CanyonKey[] SectionsLength;
			public CanyonKey[] sectionsLength {get{return sectionsLength;}}

			public CanyonParameters (float minCayonHeight, float cayonPlateauHeight, CanyonKey[] sections) {
				SetSectionsLenght(sections);

				this.minCanyonHeight = minCayonHeight;
				this.canyonPlateauHeight = cayonPlateauHeight;
			}

			public float GetValueAtHeight (float Time) {
				int MinIndex = 0;
				for(int i = 0; i < SectionsLength.Length-1; i++) {
					if(SectionsLength[i].KeyTime <= Time) {
						MinIndex = i;
					}
				}
				float v = MathFunc.Lerp(SectionsLength[MinIndex].KeyValue,
						  SectionsLength[MinIndex+1].KeyValue,
						  MathFunc.Lerp(0f,1f,
						  MathFunc.InverseLerp(SectionsLength[MinIndex].KeyTime,SectionsLength[MinIndex+1].KeyTime,Time))
				);

				return v;
			}

			void SetSectionsLenght (CanyonKey[] sections) {
				float ValueTotal = sections[sections.Length-1].KeyValue;
				float TimeTotal = sections[sections.Length-1].KeyTime;

				TimeTotal = 1f/TimeTotal;
				ValueTotal = 1f/ValueTotal;

				SectionsLength = sections;
				for(int i = 0; i < SectionsLength.Length; i++) {
					SectionsLength[i].KeyTime*=TimeTotal;
					SectionsLength[i].KeyValue*=ValueTotal;
				}
			}
		
			public class CanyonKey {
				public float KeyValue;
				public float KeyTime;

				public CanyonKey (float KeyTime, float KeyValue) {
					this.KeyTime = KeyTime;
					this.KeyValue = KeyValue;
				}
			}
		}

		public class NoiseParameters {
			public int octaves = 3;
			public float lacunarity = 2.0f;
			public float gain = 0.5f;

			public NoiseParameters (int octaves, float lacunarity, float gain) {
				this.octaves = octaves;
				this.lacunarity = lacunarity;
				this.gain = gain;
			}

			public NoiseParameters (int octaves) {
				this.octaves = octaves;
			}

			public NoiseParameters () {}
		}
	}
}