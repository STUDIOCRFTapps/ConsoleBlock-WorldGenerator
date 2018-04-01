using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WObjectInput {
	public Data data;

	public void OnDataRecieved () {
		
	}
}

public class WObjectOutput {
	public WObjectInput[] inputSources;
	public Data data;

	public void SetData(Data dataS) {
		data.Merge(dataS);
	}

	public void OnPushRequested () {
		foreach(WObjectInput input in inputSources) {
			input.data.Merge(data);
			input.OnDataRecieved();
		}
		data.data.Clear();
	}
}

public class Data {
	public List<DataFragment> data = new List<DataFragment>();

	public void Merge (Data dataS) {
		data.AddRange(dataS.data);
	}

	public void Add (DataFragment dataF) {
		data.Add(dataF);
	}
}

public class DataFragment {
	public string Id;
	public object source;

	public DataFragment(string Id, object source) {
		this.Id = Id;
		this.source = source;
	}
}