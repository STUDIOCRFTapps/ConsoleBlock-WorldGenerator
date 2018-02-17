using System;
using System.Collections.Generic;

[Serializable]
public class PrebakedBiomeData {
	public BiomeInfoList[,] BiomeFinder;
}

[Serializable]
public class BiomeInfoList {
	public List<int> infoList;

	public BiomeInfoList () {
		infoList = new List<int>();
	}

	public BiomeInfoList (List<int> list) {
		infoList = list;
	}
}