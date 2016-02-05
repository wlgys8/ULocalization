using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
 
namespace ULocalization{
	
	public class EditorLocalize{


		public static void GetModuleFileNames(out string[] names){
			if(!LocalizationEditorConfig.hasFolder){
				names = new string[]{};
				return;
			}
			var list = new List<string>();
			var folder = LocalizationEditorConfig.folder;
			var defaultLanDir = folder+"/"+ Localize.preferLanguage.ToString();

			List<string> validFiles = new List<string>();
			string[] supportFormats = Localize.supportFormats;
			foreach(var format in supportFormats){
				string[] files = Directory.GetFiles(defaultLanDir,"*."+format);
				validFiles.AddRange(files);
			}
			foreach(string filename in validFiles){
				string name = Path.GetFileName(filename);
				list.Add(name);
			}
			names = list.ToArray();
		} 

		private static string GetPreferFilePath(string filename){
			var lan = Localize.preferLanguage;
			return LocalizationEditorConfig.folder + "/" + lan.ToString()+"/"+filename;
		}

		public static Localize LoadModule(string filename){
			string module = System.IO.Path.GetFileNameWithoutExtension(filename);
			string format = System.IO.Path.GetExtension(filename).Substring(1);
			if(string.IsNullOrEmpty(format)){
				throw new System.Exception("filename with extension need");
			}
			if(Localize.ExistModule(module)){
				return Localize.GetModule(module);
			}
			var file = GetPreferFilePath(filename);
			if(!File.Exists(file)){
				Debug.LogError("Unexist file: "+file);
				return null;
			}
			return Localize.Load(module, format, AssetDatabase.LoadAssetAtPath<TextAsset>(file));
		}

		/// <summary>
		/// use {modulename}+{support formats} to search files.
		/// if found,return the first one.
		/// </summary>
		/// <returns>The load module without extension.</returns>
		/// <param name="moduleName">Module name.</param>
		public static Localize TryLoadModuleWithoutExtension(string moduleName){
			if(Localize.ExistModule(moduleName)){
				return Localize.GetModule(moduleName);
			}

			foreach(var format in Localize.supportFormats){
				string file = GetPreferFilePath(moduleName+"."+format);
				if(!File.Exists(file)){
					continue;
				}
				return Localize.Load(moduleName,format,AssetDatabase.LoadAssetAtPath<TextAsset>(file),null);
			}
			return null;
		}


		[InitializeOnLoadMethod]
		static void Initilize(){
			string[] filenames ;
			GetModuleFileNames(out filenames);
			foreach(var file in filenames){
				LoadModule(file);
			}
		}

	}

}
