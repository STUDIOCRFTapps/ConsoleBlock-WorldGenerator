#region License
/*
MIT License
Copyright Â© 2006 The Mono.Xna Team

All rights reserved.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
#endregion License

using System;
using System.Text;

namespace XnaGeometry
{
	[Serializable]
	public struct Vector4 : IEquatable<Vector4>
	{
		#region Private Fields

		private static Vector4 zeroVector = new Vector4();
		private static Vector4 unitVector = new Vector4(1f, 1f, 1f, 1f);
		private static Vector4 unitXVector = new Vector4(1f, 0f, 0f, 0f);
		private static Vector4 unitYVector = new Vector4(0f, 1f, 0f, 0f);
		private static Vector4 unitZVector = new Vector4(0f, 0f, 1f, 0f);
		private static Vector4 unitWVector = new Vector4(0f, 0f, 0f, 1f);

		#endregion Private Fields


		#region Public Fields

		public float x;
		public float y;
		public float z;
		public float w;

		#endregion Public Fields


		#region Properties

		public static Vector4 Zero
		{
			get { return zeroVector; }
		}

		public static Vector4 One
		{
			get { return unitVector; }
		}

		public static Vector4 UnitX
		{
			get { return unitXVector; }
		}

		public static Vector4 UnitY
		{
			get { return unitYVector; }
		}

		public static Vector4 UnitZ
		{
			get { return unitZVector; }
		}

		public static Vector4 UnitW
		{
			get { return unitWVector; }
		}

		#endregion Properties


		#region Constructors

		public Vector4(float x, float y, float z, float w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4(Vector2 value, float z, float w)
		{
			this.x = value.x;
			this.y = value.y;
			this.z = z;
			this.w = w;
		}

		public Vector4(Vector3 value, float w)
		{
			this.x = value.x;
			this.y = value.y;
			this.z = value.z;
			this.w = w;
		}

		public Vector4(float value)
		{
			this.x = value;
			this.y = value;
			this.z = value;
			this.w = value;
		}

		#endregion


		#region Public Methods

		public static Vector4 Add(Vector4 value1, Vector4 value2)
		{
			value1.w += value2.w;
			value1.x += value2.x;
			value1.y += value2.y;
			value1.z += value2.z;
			return value1;
		}

		public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.w = value1.w + value2.w;
			result.x = value1.x + value2.x;
			result.y = value1.y + value2.y;
			result.z = value1.z + value2.z;
		}

		public static Vector4 Barycentric(Vector4 value1, Vector4 value2, Vector4 value3, float amount1, float amount2)
		{
			#if(USE_FARSEER)
			return new Vector4(
			SilverSpriteMathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.W, value2.W, value3.W, amount1, amount2));
			#else
			return new Vector4(
				MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
				MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2),
				MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2),
				MathHelper.Barycentric(value1.w, value2.w, value3.w, amount1, amount2));
			#endif
		}

		public static void Barycentric(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, float amount1, float amount2, out Vector4 result)
		{
			#if(USE_FARSEER)
			result = new Vector4(
			SilverSpriteMathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.Z, value2.Z, value3.Z, amount1, amount2),
			SilverSpriteMathHelper.Barycentric(value1.W, value2.W, value3.W, amount1, amount2));
			#else
			result = new Vector4(
				MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
				MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2),
				MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2),
				MathHelper.Barycentric(value1.w, value2.w, value3.w, amount1, amount2));
			#endif
		}

		public static Vector4 CatmullRom(Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4, float amount)
		{
			#if(USE_FARSEER)
			return new Vector4(
			SilverSpriteMathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
			SilverSpriteMathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
			SilverSpriteMathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount),
			SilverSpriteMathHelper.CatmullRom(value1.W, value2.W, value3.W, value4.W, amount));
			#else
			return new Vector4(
				MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
				MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount),
				MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount),
				MathHelper.CatmullRom(value1.w, value2.w, value3.w, value4.w, amount));
			#endif
		}

		public static void CatmullRom(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, ref Vector4 value4, float amount, out Vector4 result)
		{
			#if(USE_FARSEER)
			result = new Vector4(
			SilverSpriteMathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
			SilverSpriteMathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount),
			SilverSpriteMathHelper.CatmullRom(value1.Z, value2.Z, value3.Z, value4.Z, amount),
			SilverSpriteMathHelper.CatmullRom(value1.W, value2.W, value3.W, value4.W, amount));
			#else
			result = new Vector4(
				MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
				MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount),
				MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount),
				MathHelper.CatmullRom(value1.w, value2.w, value3.w, value4.w, amount));
			#endif
		}

		public static Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max)
		{
			return new Vector4(
				MathHelper.Clamp(value1.x, min.x, max.x),
				MathHelper.Clamp(value1.y, min.y, max.y),
				MathHelper.Clamp(value1.z, min.z, max.z),
				MathHelper.Clamp(value1.w, min.w, max.w));
		}

		public static void Clamp(ref Vector4 value1, ref Vector4 min, ref Vector4 max, out Vector4 result)
		{
			result = new Vector4(
				MathHelper.Clamp(value1.x, min.x, max.x),
				MathHelper.Clamp(value1.y, min.y, max.y),
				MathHelper.Clamp(value1.z, min.z, max.z),
				MathHelper.Clamp(value1.w, min.w, max.w));
		}

		public static float Distance(Vector4 value1, Vector4 value2)
		{
			return (float)Math.Sqrt(DistanceSquared(value1, value2));
		}

		public static void Distance(ref Vector4 value1, ref Vector4 value2, out float result)
		{
			result = (float)Math.Sqrt(DistanceSquared(value1, value2));
		}

		public static float DistanceSquared(Vector4 value1, Vector4 value2)
		{
			float result;
			DistanceSquared(ref value1, ref value2, out result);
			return result;
		}

		public static void DistanceSquared(ref Vector4 value1, ref Vector4 value2, out float result)
		{
			result = (value1.w - value2.w) * (value1.w - value2.w) +
				(value1.x - value2.x) * (value1.x - value2.x) +
				(value1.y - value2.y) * (value1.y - value2.y) +
				(value1.z - value2.z) * (value1.z - value2.z);
		}

		public static Vector4 Divide(Vector4 value1, Vector4 value2)
		{
			value1.w /= value2.w;
			value1.x /= value2.x;
			value1.y /= value2.y;
			value1.z /= value2.z;
			return value1;
		}

		public static Vector4 Divide(Vector4 value1, float divider)
		{
			float factor = 1f / divider;
			value1.w *= factor;
			value1.x *= factor;
			value1.y *= factor;
			value1.z *= factor;
			return value1;
		}

		public static void Divide(ref Vector4 value1, float divider, out Vector4 result)
		{
			float factor = 1f / divider;
			result.w = value1.w * factor;
			result.x = value1.x * factor;
			result.y = value1.y * factor;
			result.z = value1.z * factor;
		}

		public static void Divide(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.w = value1.w / value2.w;
			result.x = value1.x / value2.x;
			result.y = value1.y / value2.y;
			result.z = value1.z / value2.z;
		}

		public static float Dot(Vector4 vector1, Vector4 vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z + vector1.w * vector2.w;
		}

		public static void Dot(ref Vector4 vector1, ref Vector4 vector2, out float result)
		{
			result = vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z + vector1.w * vector2.w;
		}

		public override bool Equals(object obj)
		{
			return (obj is Vector4) ? this == (Vector4)obj : false;
		}

		public bool Equals(Vector4 other)
		{
			return this.w == other.w
				&& this.x == other.x
				&& this.y == other.y
				&& this.z == other.z;
		}

		public override int GetHashCode()
		{
			return (int)(this.w + this.x + this.y + this.y);
		}

		public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
		{
			Vector4 result = new Vector4();
			Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
			return result;
		}

		public static void Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
		{
			#if(USE_FARSEER)
			result.W = SilverSpriteMathHelper.Hermite(value1.W, tangent1.W, value2.W, tangent2.W, amount);
			result.X = SilverSpriteMathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
			result.Y = SilverSpriteMathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
			result.Z = SilverSpriteMathHelper.Hermite(value1.Z, tangent1.Z, value2.Z, tangent2.Z, amount);
			#else
			result.w = MathHelper.Hermite(value1.w, tangent1.w, value2.w, tangent2.w, amount);
			result.x = MathHelper.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
			result.y = MathHelper.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
			result.z = MathHelper.Hermite(value1.z, tangent1.z, value2.z, tangent2.z, amount);
			#endif
		}

		public float Length()
		{
			float result;
			DistanceSquared(ref this, ref zeroVector, out result);
			return (float)Math.Sqrt(result);
		}

		public float LengthSquared()
		{
			float result;
			DistanceSquared(ref this, ref zeroVector, out result);
			return result;
		}

		public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
		{
			return new Vector4(
				MathHelper.Lerp(value1.x, value2.x, amount),
				MathHelper.Lerp(value1.y, value2.y, amount),
				MathHelper.Lerp(value1.z, value2.z, amount),
				MathHelper.Lerp(value1.w, value2.w, amount));
		}

		public static void Lerp(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
		{
			result = new Vector4(
				MathHelper.Lerp(value1.x, value2.x, amount),
				MathHelper.Lerp(value1.y, value2.y, amount),
				MathHelper.Lerp(value1.z, value2.z, amount),
				MathHelper.Lerp(value1.w, value2.w, amount));
		}

		public static Vector4 Max(Vector4 value1, Vector4 value2)
		{
			return new Vector4(
				MathHelper.Max(value1.x, value2.x),
				MathHelper.Max(value1.y, value2.y),
				MathHelper.Max(value1.z, value2.z),
				MathHelper.Max(value1.w, value2.w));
		}

		public static void Max(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result = new Vector4(
				MathHelper.Max(value1.x, value2.x),
				MathHelper.Max(value1.y, value2.y),
				MathHelper.Max(value1.z, value2.z),
				MathHelper.Max(value1.w, value2.w));
		}

		public static Vector4 Min(Vector4 value1, Vector4 value2)
		{
			return new Vector4(
				MathHelper.Min(value1.x, value2.x),
				MathHelper.Min(value1.y, value2.y),
				MathHelper.Min(value1.z, value2.z),
				MathHelper.Min(value1.w, value2.w));
		}

		public static void Min(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result = new Vector4(
				MathHelper.Min(value1.x, value2.x),
				MathHelper.Min(value1.y, value2.y),
				MathHelper.Min(value1.z, value2.z),
				MathHelper.Min(value1.w, value2.w));
		}

		public static Vector4 Multiply(Vector4 value1, Vector4 value2)
		{
			value1.w *= value2.w;
			value1.x *= value2.x;
			value1.y *= value2.y;
			value1.z *= value2.z;
			return value1;
		}

		public static Vector4 Multiply(Vector4 value1, float scaleFactor)
		{
			value1.w *= scaleFactor;
			value1.x *= scaleFactor;
			value1.y *= scaleFactor;
			value1.z *= scaleFactor;
			return value1;
		}

		public static void Multiply(ref Vector4 value1, float scaleFactor, out Vector4 result)
		{
			result.w = value1.w * scaleFactor;
			result.x = value1.x * scaleFactor;
			result.y = value1.y * scaleFactor;
			result.z = value1.z * scaleFactor;
		}

		public static void Multiply(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.w = value1.w * value2.w;
			result.x = value1.x * value2.x;
			result.y = value1.y * value2.y;
			result.z = value1.z * value2.z;
		}

		public static Vector4 Negate(Vector4 value)
		{
			value = new Vector4(-value.x, -value.y, -value.z, -value.w);
			return value;
		}

		public static void Negate(ref Vector4 value, out Vector4 result)
		{
			result = new Vector4(-value.x, -value.y, -value.z,-value.w);
		}

		public void Normalize()
		{
			Normalize(ref this, out this);
		}

		public static Vector4 Normalize(Vector4 vector)
		{
			Normalize(ref vector, out vector);
			return vector;
		}

		public static void Normalize(ref Vector4 vector, out Vector4 result)
		{
			float factor;
			DistanceSquared(ref vector, ref zeroVector, out factor);
			factor = 1f / (float)Math.Sqrt(factor);

			result.w = vector.w * factor;
			result.x = vector.x * factor;
			result.y = vector.y * factor;
			result.z = vector.z * factor;
		}

		public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, float amount)
		{
			#if(USE_FARSEER)
			return new Vector4(
			SilverSpriteMathHelper.SmoothStep(value1.X, value2.X, amount),
			SilverSpriteMathHelper.SmoothStep(value1.Y, value2.Y, amount),
			SilverSpriteMathHelper.SmoothStep(value1.Z, value2.Z, amount),
			SilverSpriteMathHelper.SmoothStep(value1.W, value2.W, amount));
			#else
			return new Vector4(
				MathHelper.SmoothStep(value1.x, value2.x, amount),
				MathHelper.SmoothStep(value1.y, value2.y, amount),
				MathHelper.SmoothStep(value1.z, value2.z, amount),
				MathHelper.SmoothStep(value1.w, value2.w, amount));
			#endif
		}

		public static void SmoothStep(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
		{
			#if(USE_FARSEER)
			result = new Vector4(
			SilverSpriteMathHelper.SmoothStep(value1.X, value2.X, amount),
			SilverSpriteMathHelper.SmoothStep(value1.Y, value2.Y, amount),
			SilverSpriteMathHelper.SmoothStep(value1.Z, value2.Z, amount),
			SilverSpriteMathHelper.SmoothStep(value1.W, value2.W, amount));
			#else
			result = new Vector4(
				MathHelper.SmoothStep(value1.x, value2.x, amount),
				MathHelper.SmoothStep(value1.y, value2.y, amount),
				MathHelper.SmoothStep(value1.z, value2.z, amount),
				MathHelper.SmoothStep(value1.w, value2.w, amount));
			#endif
		}

		public static Vector4 Subtract(Vector4 value1, Vector4 value2)
		{
			value1.w -= value2.w;
			value1.x -= value2.x;
			value1.y -= value2.y;
			value1.z -= value2.z;
			return value1;
		}

		public static void Subtract(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
		{
			result.w = value1.w - value2.w;
			result.x = value1.x - value2.x;
			result.y = value1.y - value2.y;
			result.z = value1.z - value2.z;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(32);
			sb.Append("{X:");
			sb.Append(this.x);
			sb.Append(" Y:");
			sb.Append(this.y);
			sb.Append(" Z:");
			sb.Append(this.z);
			sb.Append(" W:");
			sb.Append(this.w);
			sb.Append("}");
			return sb.ToString();
		}

		#endregion Public Methods


		#region Operators

		public static Vector4 operator -(Vector4 value)
		{
			return new Vector4(-value.x, -value.y, -value.z, -value.w);
		}

		public static bool operator ==(Vector4 value1, Vector4 value2)
		{
			return value1.w == value2.w
				&& value1.x == value2.x
				&& value1.y == value2.y
				&& value1.z == value2.z;
		}

		public static bool operator !=(Vector4 value1, Vector4 value2)
		{
			return !(value1 == value2);
		}

		public static Vector4 operator +(Vector4 value1, Vector4 value2)
		{
			value1.w += value2.w;
			value1.x += value2.x;
			value1.y += value2.y;
			value1.z += value2.z;
			return value1;
		}

		public static Vector4 operator -(Vector4 value1, Vector4 value2)
		{
			value1.w -= value2.w;
			value1.x -= value2.x;
			value1.y -= value2.y;
			value1.z -= value2.z;
			return value1;
		}

		public static Vector4 operator *(Vector4 value1, Vector4 value2)
		{
			value1.w *= value2.w;
			value1.x *= value2.x;
			value1.y *= value2.y;
			value1.z *= value2.z;
			return value1;
		}

		public static Vector4 operator *(Vector4 value1, float scaleFactor)
		{
			value1.w *= scaleFactor;
			value1.x *= scaleFactor;
			value1.y *= scaleFactor;
			value1.z *= scaleFactor;
			return value1;
		}

		public static Vector4 operator *(float scaleFactor, Vector4 value1)
		{
			value1.w *= scaleFactor;
			value1.x *= scaleFactor;
			value1.y *= scaleFactor;
			value1.z *= scaleFactor;
			return value1;
		}

		public static Vector4 operator /(Vector4 value1, Vector4 value2)
		{
			value1.w /= value2.w;
			value1.x /= value2.x;
			value1.y /= value2.y;
			value1.z /= value2.z;
			return value1;
		}

		public static Vector4 operator /(Vector4 value1, float divider)
		{
			float factor = 1f / divider;
			value1.w *= factor;
			value1.x *= factor;
			value1.y *= factor;
			value1.z *= factor;
			return value1;
		}

		#endregion Operators
	}
}