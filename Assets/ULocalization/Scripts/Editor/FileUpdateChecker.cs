using UnityEngine;
using System.Collections;
using UnityEditor;
namespace ULocalization{
	public class FileUpdateChecker : AssetPostprocessor {

		static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){
			bool needUpdate = false;
			foreach(string path in importedAssets){
				if(path.Contains(LocalizationEditorConfig.folder)){
					needUpdate = true;
				}
			}

			if(needUpdate){
				Localize.UnloadAll();
				var uitexts = GameObject.FindObjectsOfType<LocalizeUIText>();
				foreach(var uitext in uitexts){
					EditorLocalize.TryLoadModuleWithoutExtension(uitext.moduleName);
					uitext.Reset();
				}
			}

		}

	}

}
