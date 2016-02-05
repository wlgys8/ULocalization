using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Linq;

namespace ULocalization{

	public class Localize{

		public static string defaultModuel{
			get;set;
		}

		public static SystemLanguage preferLanguage{
			get{
				//return SystemLanguage.Chinese;
				return Application.systemLanguage;
			}
		}

		public static Localize active{
			get;set;
		}

		private static Dictionary<string,Localize> _localizeMap = new Dictionary<string, Localize>();

		public static void UnloadAll(){
			_localizeMap.Clear();
			Localize.active = null;

		}


		public static bool ExistModuel(string name){
			return _localizeMap.ContainsKey(name);
		}

		public static Localize GetModuel(string name){
			return _localizeMap[name];
		}

		public static string[] GetNames(){
			return _localizeMap.Keys.ToArray();
		}

		private Collection _fallback;
		private Collection _current;


		private Localize(TextAsset current,TextAsset fallback){
			try{
				_current = JsonMapper.ToObject<Collection>(current.text);
				if(fallback != null){
					_fallback = JsonMapper.ToObject<Collection>(fallback.text);
				}
			}catch(System.Exception e){
				Debug.LogException(e);
				Debug.LogError("Json format is wrong!");
				return;
			}
			Localize.active = this;
			_localizeMap.Add(_current.moduelName,this);
		}

		private Localize(Collection current,Collection fallback){
			_current = current;
			_fallback = fallback;
			Localize.active = this;
			_localizeMap.Add(_current.moduelName,this);
		}

		/// <summary>
		/// Set debug true if you want to get warning if a key is missing.
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		public bool debug {
			get;set;
		}

		public string[] GetAllKeys(){
			return _current.strings.Keys.ToArray();
		}

		public string Get(string key){
			var value = _current[key];
			if(value == null){
				if(debug){
					Debug.LogWarningFormat("Can not find key = {0},Try fallback...",key);
				}
				if(_fallback != null){
					value = _fallback[key];
					if(value == null){
						Debug.LogErrorFormat("Can not find key = {0} in fallback",key);
					}
				}
			}
			if(value == null){
				value = "";
			}
			return value;
		}



		private static ILoader _defaultLoader = new DefaultLoader();


		private static void LoadCollection(string moduelName,SystemLanguage lan,System.Action<Collection> onDone,ILoader loader = null){
			var filePath = lan.ToString()+"/"+moduelName;
			if(loader == null){
				loader = _defaultLoader;
			}
			loader.StartLoad(filePath,delegate(TextAsset txt) {
				Collection col = null;
				if(txt != null){
					col = JsonMapper.ToObject<Collection>(txt.text);
				}else{
					Debug.LogError("[Localize] Load failed:"+filePath);
				}
				onDone(col);
			});
		}


		/// <summary>
		/// Load localization json files.
		/// </summary>
		public static void Load(string moduelName,System.Action<Localize> onComplete,SystemLanguage? fallbackLan = null, ILoader loader = null){
			if(ExistModuel(moduelName)){
				Debug.LogWarning("Already existed moduel : "+moduelName);
				onComplete(GetModuel(moduelName));
				return;
			}
			SystemLanguage currentLan = Localize.preferLanguage;
			LoadCollection(moduelName,currentLan,delegate(Collection current) {
				if(fallbackLan !=null && fallbackLan != currentLan){
					LoadCollection(moduelName,(SystemLanguage)fallbackLan,delegate(Collection fallback) {
						Localize loc = null;
						if(current != null){
							loc = new Localize(current,fallback);
						}
						onComplete(loc);
					},loader);
				}else{
					Localize loc = null;
					if(current != null){
						loc = new Localize(current,null);
					}
					onComplete(loc);
				}
			},loader);
		}

		public static Localize Load(TextAsset main,TextAsset fallback = null){
			return new Localize(main,fallback);
		}

		private class DefaultLoader :ILoader{
			public void StartLoad(string path,System.Action<TextAsset> onComplete){
				var asset = Resources.Load<TextAsset>(path);
				onComplete(asset);
			}
		}

//		[UnityEditor.MenuItem("Assets/test")]
//		public static void test(){
//			var c= new Collection();
//			c.moduelName ="strings";
//			c.language = Application.systemLanguage.ToString();
//			Debug.Log(JsonMapper.ToJson(c));
//		}

	}

	public class Collection{

		public string moduelName;
		public string language;
		public Dictionary<string,string> strings = new Dictionary<string, string>();
	
		public string this[string key]{
			get{
				if(strings.ContainsKey(key)){
					return strings[key];
				}
				return "";
			}
		}
	}

	public interface ILoader{

		void StartLoad(string path,System.Action<TextAsset> onComplete);

	}
}
