using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
 
namespace ULocalization{
	
	public class EditorLocalize{


		public static void GetModuelFileNames(out string[] names){
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

		public static Localize LoadModuel(string filename){
			string moduel = System.IO.Path.GetFileNameWithoutExtension(filename);
			string format = System.IO.Path.GetExtension(filename).Substring(1);
			if(string.IsNullOrEmpty(format)){
				throw new System.Exception("filename with extension need");
			}
			if(Localize.ExistModuel(moduel)){
				return Localize.GetModuel(moduel);
			}
			var file = GetPreferFilePath(filename);
			if(!File.Exists(file)){
				Debug.LogError("Unexist file: "+file);
				return null;
			}
			return Localize.Load(moduel, format, AssetDatabase.LoadAssetAtPath<TextAsset>(file));
		}

		/// <summary>
		/// use {moduelname}+{support formats} to search files.
		/// if found,return the first one.
		/// </summary>
		/// <returns>The load moduel without extension.</returns>
		/// <param name="moduelName">Moduel name.</param>
		public static Localize TryLoadModuelWithoutExtension(string moduelName){
			if(Localize.ExistModuel(moduelName)){
				return Localize.GetModuel(moduelName);
			}

			foreach(var format in Localize.supportFormats){
				string file = GetPreferFilePath(moduelName+"."+format);
				if(!File.Exists(file)){
					continue;
				}
				return Localize.Load(moduelName,format,AssetDatabase.LoadAssetAtPath<TextAsset>(file),null);
			}
			return null;
		}


		[InitializeOnLoadMethod]
		static void Initilize(){
			string[] filenames ;
			GetModuelFileNames(out filenames);
			foreach(var file in filenames){
				LoadModuel(file);
			}
		}

	}

}
