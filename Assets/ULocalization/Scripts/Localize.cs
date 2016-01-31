using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System.Collections.Generic;
using LitJson;
using System.IO;

namespace ULocalization{

	public class Localize{
		public static Localize active{
			get;set;
		}

		private Collection _fallback;
		private Collection _current;

		private string _path;
		private SystemLanguage _curLan;
		private SystemLanguage _fallbackLan;
		private string _filename;

		private ILoader _loader;
		private LocalizationMono _mono;

		/// <summary>
		/// Initializes a new instance of the <see cref="ULocalization.Localize"/> class.
		/// </summary>
		/// <param name="filename">json filename.</param>
		/// <param name="fallback">if a key was missing in current language json,
		/// class will try to find it in fallback language json .</param>
		/// <param name="loader">default loader is Resources.Load, your can implement your custom Loader.</param>
		public Localize(string filename,
			SystemLanguage fallback = SystemLanguage.English,ILoader loader = null){
			_curLan = Application.systemLanguage;
			_fallbackLan = fallback;
			_filename = filename;
			_loader = loader;
			if(_loader == null){
				_loader = new DefaultLoader();
			}
			_mono = new GameObject("Localization").AddComponent<LocalizationMono>();
			Localize.active = this;
		}

		public Localize(SystemLanguage overwriteLan,string filename,
			SystemLanguage fallback = SystemLanguage.English,ILoader loader = null):this(filename,fallback,loader){
			_curLan = overwriteLan;
		}

		/// <summary>
		/// Set debug true if you want to get warning if a key is missing.
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		public bool debug {
			get;set;
		}

		/// <summary>
		/// Load localization json files.
		/// </summary>
		public Coroutine Load(){
			return _mono.StartCoroutine(_Load());
		}

		private IEnumerator _Load(){
			var co = LoadCollection(_curLan,delegate(Collection c) {
				_current = c;	
			});
			if(_current == null){
				yield return co;
			}

			if(_fallbackLan != Application.systemLanguage){
				co = LoadCollection(_fallbackLan,delegate(Collection c) {
					_fallback = c;
				});
				if(_fallback == null){
					yield return co;
				}
			}
		}

		private Coroutine LoadCollection(SystemLanguage lan,System.Action<Collection> onDone){
			return _mono.StartCoroutine(_LoadCollection(lan,onDone));
		}

		private IEnumerator _LoadCollection(SystemLanguage lan,System.Action<Collection> onDone){
			var filePath = Path.Combine(lan.ToString(),_filename);
			_loader.StartLoad(filePath);
			while(!_loader.isDone){
				yield return null;
			}
			var txt = _loader.asset;
			var col = JsonMapper.ToObject<Collection>(txt.text);
			onDone(col);
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



		private class DefaultLoader :ILoader{
			public void StartLoad(string path){
				this.asset = Resources.Load<TextAsset>(path);
				this.isDone = true;
			}

			public bool isDone{get;private set;}

			public TextAsset asset{get;private set;}
		}

	}

	internal class LocalizationMono:MonoBehaviour{

		void Awake(){
			GameObject.DontDestroyOnLoad(gameObject);
		}
	}

	public class Collection{

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

		void StartLoad(string path);

		bool isDone{get;}

		TextAsset asset{get;}
	}
}
