using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Structure Group", menuName = "ConsoleBlock/StructureGroup")]
public class StructureGroup : ScriptableObject {
	new public string name = "";
	public Structure[] Structures;
}
