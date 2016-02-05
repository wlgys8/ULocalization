using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;
using System.Collections.Generic;
using LitJson;
using System.IO;
using System.Linq;

namespace ULocalization{

	public class Localize{

		public static string defaultModuleFile{
			get;set;
		}

		public static string defaultModuleName{
			get{
				return System.IO.Path.GetFileNameWithoutExtension(defaultModuleFile);
			}
		}

		public static SystemLanguage preferLanguage{
			get{
				//return SystemLanguage.Chinese;
				return Application.systemLanguage;
			}
		}


		public static string[] supportFormats{
			get{
				return _decoderMap.Keys.ToArray();
			}
		}


		private static Dictionary<string,Localize> _localizeMap = new Dictionary<string, Localize>();
		private static Dictionary<string,Decoder> _decoderMap = new Dictionary<string, Decoder>();

		static Localize(){
			_decoderMap.Add("json",DecoderImpl.JsonDecode);
			_decoderMap.Add("csv",DecoderImpl.CSVDecode);
			defaultModuleFile = "default.json";
		}

		public static void UnloadAll(){
			_localizeMap.Clear();

		}

		public static bool ExistModule(string name){
			return _localizeMap.ContainsKey(name);
		}

		public static Localize GetModule(string name){
			return _localizeMap[name];
		}

		public static string[] GetModuleNames(){
			return _localizeMap.Keys.ToArray();
		}


		/// <summary>
		/// Set debug true if you want to get warning if a key is missing.
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		public static bool debug {
			get;set;
		}

		private Collection _fallback;
		private Collection _current;
		private string _moduleName;

		private Localize(string moduleName,Collection current,Collection fallback){
			_current = current;
			_fallback = fallback;
			_moduleName = moduleName;
		}

		public string moduleName{
			get{
				return _moduleName;
			}
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


		private static void LoadCollection(string moduleName,string format,
			SystemLanguage lan,
			System.Action<Collection> onDone,
			ILoader loader = null){

			if(!_decoderMap.ContainsKey(format)){
				throw new System.Exception("there is no decoder for this format : "+format);
			}
			var decoder = _decoderMap[format];
			var filePath = lan.ToString()+"/"+moduleName+"."+format;
			if(loader == null){
				loader = _defaultLoader;
			}
			loader.StartLoad(filePath,delegate(TextAsset txt) {
				Collection col = null;
				if(txt != null){
					col = decoder.Invoke(txt.text);
				}else{
					Debug.LogError("[Localize] Load failed:"+filePath);
				}
				onDone(col);
			});
		}


		/// <summary>
		/// Load localization json files.
		/// </summary>
		public static void Load(string filename,
			System.Action<Localize> onComplete,
			SystemLanguage? fallbackLan = null, 
			ILoader loader = null){

			string format = System.IO.Path.GetExtension(filename).Substring(1);
			string moduleName = System.IO.Path.GetFileNameWithoutExtension(filename);

			if(!_decoderMap.ContainsKey(format)){
				throw new System.Exception("no decoder for format : "+format);
			}

			if(ExistModule(moduleName)){
				Debug.LogWarning("Already existed module : "+moduleName);
				onComplete(GetModule(moduleName));
				return;
			}

			SystemLanguage currentLan = Localize.preferLanguage;
			LoadCollection(moduleName,format,currentLan,delegate(Collection main) {
				if(fallbackLan !=null && fallbackLan != currentLan){
					LoadCollection(moduleName,format,(SystemLanguage)fallbackLan,delegate(Collection fallback) {
						Localize loc = null;
						if(main != null){
							loc = new Localize(moduleName,main,fallback);
							_localizeMap.Add(moduleName,loc);
						}
						onComplete(loc);
					},loader);
				}else{
					Localize loc = null;
					if(main != null){
						loc = new Localize(moduleName, main,null);
						_localizeMap.Add(moduleName,loc);
					}
					onComplete(loc);
				}
			},loader);
		}

		public static Localize Load(string modulename,string format,TextAsset main,TextAsset fallback = null){
			if(!_decoderMap.ContainsKey(format)){
				throw new System.Exception("thers is no decoder for foramt : "+format);
			}
			var decoder = _decoderMap[format];
			Collection mainCol = null;
			Collection fallbackCol = null;

			try{
				mainCol = decoder.Invoke(main.text);
				if(fallback != null){
					fallbackCol = decoder.Invoke(fallback.text);
				}
			}catch(System.Exception e){
				Debug.LogException(e);
				Debug.LogFormat("file content is wrong! {0}.{1}",main.name,format);
				return null;
			}
			var loc = new Localize(modulename, mainCol,fallbackCol);
			_localizeMap.Add(modulename,loc);
			return loc;
		}

		private class DefaultLoader :ILoader{
			public void StartLoad(string path,System.Action<TextAsset> onComplete){
				path = Path.GetDirectoryName(path)+"/"+Path.GetFileNameWithoutExtension(path);
				var asset = Resources.Load<TextAsset>(path);
				if(asset == null){
					Debug.LogErrorFormat("Resources.Load({0}) is null",path);
				}
				onComplete(asset);
			}
		}

//		[UnityEditor.MenuItem("Assets/test")]
//		public static void test(){
//			var c= new Collection();
//			c.moduleName ="strings";
//			c.language = Application.systemLanguage.ToString();
//			Debug.Log(JsonMapper.ToJson(c));
//		}

	}

	public class Collection{

		public string moduleName;
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


	public delegate Collection Decoder(string content);

}
