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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace XnaGeometry
{
	[Serializable]
	public struct Vector3 : IEquatable<Vector3>
	{
		#region Private Fields

		private static  Vector3 zero = new Vector3(0f, 0f, 0f);
		private static  Vector3 one = new Vector3(1f, 1f, 1f);
		private static  Vector3 unitX = new Vector3(1f, 0f, 0f);
		private static  Vector3 unitY = new Vector3(0f, 1f, 0f);
		private static  Vector3 unitZ = new Vector3(0f, 0f, 1f);
		private static  Vector3 up = new Vector3(0f, 1f, 0f);
		private static  Vector3 down = new Vector3(0f, -1f, 0f);
		private static  Vector3 right = new Vector3(1f, 0f, 0f);
		private static Vector3 left = new Vector3(-1f, 0f, 0f);
		private static Vector3 forward = new Vector3(0f, 0f, -1f);
		private static Vector3 backward = new Vector3(0f, 0f, 1f);

		#endregion Private Fields


		#region Public Fields

		public float x;
		public float y;
		public float z;

		#endregion Public Fields


		#region Properties

		public static Vector3 Zero
		{
			get { return zero; }
		}

		public static Vector3 One
		{
			get { return one; }
		}

		public static Vector3 UnitX
		{
			get { return unitX; }
		}

		public static Vector3 UnitY
		{
			get { return unitY; }
		}

		public static Vector3 UnitZ
		{
			get { return unitZ; }
		}

		public static Vector3 Up
		{
			get { return up; }
		}

		public static Vector3 Down
		{
			get { return down; }
		}

		public static Vector3 Right
		{
			get { return right; }
		}

		public static Vector3 Left
		{
			get { return left; }
		}

		public static Vector3 Forward
		{
			get { return forward; }
		}

		public static Vector3 Backward
		{
			get { return backward; }
		}

		#endregion Properties


		#region Constructors

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}


		public Vector3(float value)
		{
			this.x = value;
			this.y = value;
			this.z = value;
		}


		public Vector3(Vector2 value, float z)
		{
			this.x = value.x;
			this.y = value.y;
			this.z = z;
		}


		#endregion Constructors


		#region Public Methods

		public static Vector3 Add(Vector3 value1, Vector3 value2)
		{
			value1.x += value2.x;
			value1.y += value2.y;
			value1.z += value2.z;
			return value1;
		}

		public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.x = value1.x + value2.x;
			result.y = value1.y + value2.y;
			result.z = value1.z + value2.z;
		}

		public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
		{
			return new Vector3(
				MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
				MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2),
				MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2));
		}

		public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
				MathHelper.Barycentric(value1.y, value2.y, value3.y, amount1, amount2),
				MathHelper.Barycentric(value1.z, value2.z, value3.z, amount1, amount2));
		}

		public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
		{
			return new Vector3(
				MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
				MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount),
				MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount));
		}

		public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
				MathHelper.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount),
				MathHelper.CatmullRom(value1.z, value2.z, value3.z, value4.z, amount));
		}

		public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
		{
			return new Vector3(
				MathHelper.Clamp(value1.x, min.x, max.x),
				MathHelper.Clamp(value1.y, min.y, max.y),
				MathHelper.Clamp(value1.z, min.z, max.z));
		}

		public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.Clamp(value1.x, min.x, max.x),
				MathHelper.Clamp(value1.y, min.y, max.y),
				MathHelper.Clamp(value1.z, min.z, max.z));
		}

		public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
		{
			Cross(ref vector1, ref vector2, out vector1);
			return vector1;
		}

		public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
		{
			result = new Vector3(vector1.y * vector2.z - vector2.y * vector1.z,
				-(vector1.x * vector2.z - vector2.x * vector1.z),
				vector1.x * vector2.y - vector2.x * vector1.y);
		}

		public static float Distance(Vector3 vector1, Vector3 vector2)
		{
			float result;
			DistanceSquared(ref vector1, ref vector2, out result);
			return (float)Math.Sqrt(result);
		}

		public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
		{
			DistanceSquared(ref value1, ref value2, out result);
			result = (float)Math.Sqrt(result);
		}

		public static float DistanceSquared(Vector3 value1, Vector3 value2)
		{
			float result;
			DistanceSquared(ref value1, ref value2, out result);
			return result;
		}

		public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
		{
			result = (value1.x - value2.x) * (value1.x - value2.x) +
				(value1.y - value2.y) * (value1.y - value2.y) +
				(value1.z - value2.z) * (value1.z - value2.z);
		}

		public static Vector3 Divide(Vector3 value1, Vector3 value2)
		{
			value1.x /= value2.x;
			value1.y /= value2.y;
			value1.z /= value2.z;
			return value1;
		}

		public static Vector3 Divide(Vector3 value1, float value2)
		{
			float factor = 1 / value2;
			value1.x *= factor;
			value1.y *= factor;
			value1.z *= factor;
			return value1;
		}

		public static void Divide(ref Vector3 value1, float divisor, out Vector3 result)
		{
			float factor = 1 / divisor;
			result.x = value1.x * factor;
			result.y = value1.y * factor;
			result.z = value1.z * factor;
		}

		public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.x = value1.x / value2.x;
			result.y = value1.y / value2.y;
			result.z = value1.z / value2.z;
		}

		public static float Dot(Vector3 vector1, Vector3 vector2)
		{
			return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
		}

		public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
		{
			result = vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
		}

		public override bool Equals(object obj)
		{
			return (obj is Vector3) ? this == (Vector3)obj : false;
		}

		public bool Equals(Vector3 other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return (int)(this.x + this.y + this.z);
		}

		public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
		{
			Vector3 result = new Vector3();
			Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
			return result;
		}

		public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
		{
			result.x = MathHelper.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
			result.y = MathHelper.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
			result.z = MathHelper.Hermite(value1.z, tangent1.z, value2.z, tangent2.z, amount);
		}

		public float Length()
		{
			float result;
			DistanceSquared(ref this, ref zero, out result);
			return (float)Math.Sqrt(result);
		}

		public float LengthSquared()
		{
			float result;
			DistanceSquared(ref this, ref zero, out result);
			return result;
		}

		public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
		{
			return new Vector3(
				MathHelper.Lerp(value1.x, value2.x, amount),
				MathHelper.Lerp(value1.y, value2.y, amount),
				MathHelper.Lerp(value1.z, value2.z, amount));
		}

		public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.Lerp(value1.x, value2.x, amount),
				MathHelper.Lerp(value1.y, value2.y, amount),
				MathHelper.Lerp(value1.z, value2.z, amount));
		}

		public static Vector3 Max(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				MathHelper.Max(value1.x, value2.x),
				MathHelper.Max(value1.y, value2.y),
				MathHelper.Max(value1.z, value2.z));
		}

		public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.Max(value1.x, value2.x),
				MathHelper.Max(value1.y, value2.y),
				MathHelper.Max(value1.z, value2.z));
		}

		public static Vector3 Min(Vector3 value1, Vector3 value2)
		{
			return new Vector3(
				MathHelper.Min(value1.x, value2.x),
				MathHelper.Min(value1.y, value2.y),
				MathHelper.Min(value1.z, value2.z));
		}

		public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.Min(value1.x, value2.x),
				MathHelper.Min(value1.y, value2.y),
				MathHelper.Min(value1.z, value2.z));
		}

		public static Vector3 Multiply(Vector3 value1, Vector3 value2)
		{
			value1.x *= value2.x;
			value1.y *= value2.y;
			value1.z *= value2.z;
			return value1;
		}

		public static Vector3 Multiply(Vector3 value1, float scaleFactor)
		{
			value1.x *= scaleFactor;
			value1.y *= scaleFactor;
			value1.z *= scaleFactor;
			return value1;
		}

		public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
		{
			result.x = value1.x * scaleFactor;
			result.y = value1.y * scaleFactor;
			result.z = value1.z * scaleFactor;
		}

		public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
			result.x = value1.x * value2.x;
			result.y = value1.y * value2.y;
			result.z = value1.z * value2.z;
		}

		public static Vector3 Negate(Vector3 value)
		{
			value = new Vector3(-value.x, -value.y, -value.z);
			return value;
		}

		public static void Negate(ref Vector3 value, out Vector3 result)
		{
			result = new Vector3(-value.x, -value.y, -value.z);
		}

		public void Normalize()
		{
			Normalize(ref this, out this);
		}

		public static Vector3 Normalize(Vector3 vector)
		{
			Normalize(ref vector, out vector);
			return vector;
		}

		public static void Normalize(ref Vector3 value, out Vector3 result)
		{
			float factor;
			Distance(ref value, ref zero, out factor);
			factor = 1f / factor;
			result.x = value.x * factor;
			result.y = value.y * factor;
			result.z = value.z * factor;
		}

		public static Vector3 Reflect(Vector3 vector, Vector3 normal)
		{
			// I is the original array
			// N is the normal of the incident plane
			// R = I - (2 * N * ( DotProduct[ I,N] ))
			Vector3 reflectedVector;
			// inline the dotProduct here instead of calling method
			float dotProduct = ((vector.x * normal.x) + (vector.y * normal.y)) + (vector.z * normal.z);
			reflectedVector.x = vector.x - (2.0f * normal.x) * dotProduct;
			reflectedVector.y = vector.y - (2.0f * normal.y) * dotProduct;
			reflectedVector.z = vector.z - (2.0f * normal.z) * dotProduct;

			return reflectedVector;
		}

		public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
		{
			// I is the original array
			// N is the normal of the incident plane
			// R = I - (2 * N * ( DotProduct[ I,N] ))

			// inline the dotProduct here instead of calling method
			float dotProduct = ((vector.x * normal.x) + (vector.y * normal.y)) + (vector.z * normal.z);
			result.x = vector.x - (2.0f * normal.x) * dotProduct;
			result.y = vector.y - (2.0f * normal.y) * dotProduct;
			result.z = vector.z - (2.0f * normal.z) * dotProduct;

		}

		public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
		{
			return new Vector3(
				MathHelper.SmoothStep(value1.x, value2.x, amount),
				MathHelper.SmoothStep(value1.y, value2.y, amount),
				MathHelper.SmoothStep(value1.z, value2.z, amount));
		}

		public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
		{
			result = new Vector3(
				MathHelper.SmoothStep(value1.x, value2.x, amount),
				MathHelper.SmoothStep(value1.y, value2.y, amount),
				MathHelper.SmoothStep(value1.z, value2.z, amount));
		}

		public static Vector3 Subtract(Vector3 value1, Vector3 value2)
		{
			value1.x -= value2.x;
			value1.y -= value2.y;
			value1.z -= value2.z;
			return value1;
		}

		public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
		{
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
			sb.Append("}");
			return sb.ToString();
		}

		#endregion Public methods


		#region Operators

		public static bool operator ==(Vector3 value1, Vector3 value2)
		{
			return value1.x == value2.x
				&& value1.y == value2.y
				&& value1.z == value2.z;
		}

		public static bool operator !=(Vector3 value1, Vector3 value2)
		{
			return !(value1 == value2);
		}

		public static Vector3 operator +(Vector3 value1, Vector3 value2)
		{
			value1.x += value2.x;
			value1.y += value2.y;
			value1.z += value2.z;
			return value1;
		}

		public static Vector3 operator -(Vector3 value)
		{
			value = new Vector3(-value.x, -value.y, -value.z);
			return value;
		}

		public static Vector3 operator -(Vector3 value1, Vector3 value2)
		{
			value1.x -= value2.x;
			value1.y -= value2.y;
			value1.z -= value2.z;
			return value1;
		}

		public static Vector3 operator *(Vector3 value1, Vector3 value2)
		{
			value1.x *= value2.x;
			value1.y *= value2.y;
			value1.z *= value2.z;
			return value1;
		}

		public static Vector3 operator *(Vector3 value, float scaleFactor)
		{
			value.x *= scaleFactor;
			value.y *= scaleFactor;
			value.z *= scaleFactor;
			return value;
		}

		public static Vector3 operator *(float scaleFactor, Vector3 value)
		{
			value.x *= scaleFactor;
			value.y *= scaleFactor;
			value.z *= scaleFactor;
			return value;
		}

		public static Vector3 operator /(Vector3 value1, Vector3 value2)
		{
			value1.x /= value2.x;
			value1.y /= value2.y;
			value1.z /= value2.z;
			return value1;
		}

		public static Vector3 operator /(Vector3 value, float divider)
		{
			float factor = 1 / divider;
			value.x *= factor;
			value.y *= factor;
			value.z *= factor;
			return value;
		}

		#endregion
	}
}