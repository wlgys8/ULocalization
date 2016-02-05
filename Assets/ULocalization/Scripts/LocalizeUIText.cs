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
//		#if UNITY_EDITOR
//		if(!Application.isPlaying){
//			if(!Localize.ExistModuel(_moduelName)){
//				Localize.Load(_moduelName,delegate(Localize obj) {
//				});
//			}
//		}
//		#endif
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


	private string GetValue(string moduelName,string key){
		Localize moduel = null;
		if(!Localize.ExistModuel(moduelName)){ 
			//if we do not find moduel by current name,try default module.
			if(Localize.ExistModuel(Localize.defaultModuelName)){
				moduel = Localize.GetModuel(Localize.defaultModuelName);
			}else{
				return null;
			}
		}else{
			moduel = Localize.GetModuel(moduelName);
		}
		return moduel.Get(key);
	}

	public void Set(string moduelName,string key){
		_moduelName = moduelName;
		_key = key;
		string value = GetValue(moduelName,key);
		if(value == null){
			return;
		}
		GetComponent<Text>().text = value;
	}

	public void Set(string moduelName,string key,params object[] ps){
		_moduelName = moduelName;
		_key = key;
		string value = GetValue(moduelName,key);
		if(value == null){
			return;
		}
		GetComponent<Text>().text = string.Format(value,ps);
	}


}
