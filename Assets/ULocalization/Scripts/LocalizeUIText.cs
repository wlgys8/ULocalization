using UnityEngine;
using System.Collections;
using ULocalization;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LocalizeUIText : MonoBehaviour {

	public string key;

	void Start(){
		if(Localize.active == null){
			Debug.LogError("No active localiza was found. Did you new Localize before?");
		}
		string value = Localize.active.Get(key);
		GetComponent<Text>().text = value;
	}
}
