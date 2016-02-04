using UnityEngine;
using System.Collections;
using ULocalization;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LocalizeUIText : MonoBehaviour {

	[SerializeField]
	private string _moduelName = "default";

	[SerializeField]
	private string _key;

	void Start(){
		#if UNITY_EDITOR
		if(!Application.isPlaying){
			if(!Localize.ExistModuel(_moduelName)){
				Localize.Load(_moduelName,delegate(Localize obj) {
				});
			}
		}
		#endif
		Reset();
	}

	public void Reset(){
		Set(_moduelName,_key);
	}

	public string moduelName{
		get{
			return _moduelName;
		}
	}

	public string key{
		get{
			return _key;
		}
	}

	public void Set(string moduelName,string key){
		_moduelName = moduelName;
		_key = key;
		if(!Localize.ExistModuel(moduelName)){
			return;
		}
		var moduel = Localize.GetModuel(moduelName);
		string value = moduel.Get(key);
		GetComponent<Text>().text = value;
	}

}
