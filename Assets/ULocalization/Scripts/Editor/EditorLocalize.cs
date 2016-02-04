using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
 
namespace ULocalization{
	
	public class EditorLocalize{


		public static void GetModuelNames(out string[] names){
			if(!LocalizationEditorConfig.hasFolder){
				names = new string[]{};
				return;
			}
			var list = new List<string>();
			var folder = LocalizationEditorConfig.folder;
			var defaultLanDir = folder+"/"+ Localize.preferLanguage.ToString();
			string[] jsonFiles = Directory.GetFiles(defaultLanDir,"*.json");
			foreach(string filename in jsonFiles){
				string name = Path.GetFileNameWithoutExtension(filename);
				list.Add(name);
			}
			names = list.ToArray();
		}

		public static Localize LoadModuel(string moduel){
			if(Localize.ExistModuel(moduel)){
				return Localize.GetModuel(moduel);
			}
			var lan = Localize.preferLanguage;
			var jsonFile = LocalizationEditorConfig.folder + "/" + lan.ToString()+"/"+moduel+".json";
			if(!File.Exists(jsonFile)){
				return null;
			}
			return Localize.Load(AssetDatabase.LoadAssetAtPath<TextAsset>(jsonFile));
		}

	}

}
