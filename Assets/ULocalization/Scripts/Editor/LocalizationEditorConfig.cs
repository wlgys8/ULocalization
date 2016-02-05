using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

namespace ULocalization{
	public class LocalizationEditorConfig : ScriptableObject {

		[SerializeField]
		private string _folder;

		private static LocalizationEditorConfig _instance;

		public static LocalizationEditorConfig Instance{
			get{
				if(_instance == null){
					string[] guids = AssetDatabase.FindAssets("t:LocalizationEditorConfig");
					if(guids.Length > 1){
						throw new System.Exception("More than one LocalizationEditorConfig exist in project!Please delete one!");
					}
					if(guids.Length == 1){
						var path = AssetDatabase.GUIDToAssetPath(guids[0]);
						_instance = AssetDatabase.LoadAssetAtPath<LocalizationEditorConfig>(path);
						return _instance;
					}
					_instance =  LocalizationEditorConfig.CreateInstance<LocalizationEditorConfig>();
					AssetDatabase.CreateAsset(_instance,"Assets/ULocalization/config.asset");
				}
				return _instance;
			}
		}

		public static string folder{
			get{
				return Instance._folder;
			}set{
				Instance._folder = value;
			}
		}

		public static bool hasFolder{
			get{
				return !string.IsNullOrEmpty(folder) || System.IO.Directory.Exists(folder);
			}
		}


		[MenuItem("Window/ULocalization")]
		public static void Select(){
			Selection.activeObject = Instance;
		}


		[InitializeOnLoadMethod]
		public static void EditorInitlize(){
			//Localize.defaultFormat = Instance._defaultForamt;
		}

	}
}
