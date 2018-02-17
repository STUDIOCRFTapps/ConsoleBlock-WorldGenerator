using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkParameters {
	public ChunkParameters(int ChunkLOD, Vector2 Position, bool Collider) {
		ChunkPos = Position;
		LevelOfDetails = ChunkLOD;
		ColliderMesh = Collider;
	}

	public int LevelOfDetails = 0;
	public Vector2 ChunkPos = Vector2.zero;
	public bool ColliderMesh = false;

	public override bool Equals(object other) {
		if(other.GetType() != typeof(ChunkParameters))
			return false;

		return this.LevelOfDetails == ((ChunkParameters)other).LevelOfDetails && this.ChunkPos == ((ChunkParameters)other).ChunkPos && this.ColliderMesh == ((ChunkParameters)other).ColliderMesh;
	}

	public override int GetHashCode () {
		return base.GetHashCode();
	}
}
