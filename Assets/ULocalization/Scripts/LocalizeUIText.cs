using UnityEngine;
using System.Collections;
using ULocalization;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Text))]
public class LocalizeUIText : MonoBehaviour {

	[SerializeField]
	private string _moduleName = "default";

	[SerializeField]
	private string _key;

	void Start(){
//		#if UNITY_EDITOR
//		if(!Application.isPlaying){
//			if(!Localize.ExistModule(_moduleName)){
//				Localize.Load(_moduleName,delegate(Localize obj) {
//				});
//			}
//		}
//		#endif
		Reset();
	}

	public void Reset(){
		Set(_moduleName,_key);
	}

	public string moduleName{
		get{
			return _moduleName;
		}
	}

	public string key{
		get{
			return _key;
		}
	}


	private string GetValue(string moduleName,string key){
		Localize module = null;
		if(!Localize.ExistModule(moduleName)){ 
			//if we do not find module by current name,try default module.
			if(Localize.ExistModule(Localize.defaultModuleName)){
				module = Localize.GetModule(Localize.defaultModuleName);
			}else{
				return null;
			}
		}else{
			module = Localize.GetModule(moduleName);
		}
		return module.Get(key);
	}

	public void Set(string moduleName,string key){
		_moduleName = moduleName;
		_key = key;
		string value = GetValue(moduleName,key);
		if(value == null){
			return;
		}
		GetComponent<Text>().text = value;
	}

	public void Set(string moduleName,string key,params object[] ps){
		_moduleName = moduleName;
		_key = key;
		string value = GetValue(moduleName,key);
		if(value == null){
			return;
		}
		GetComponent<Text>().text = string.Format(value,ps);
	}


}
