using UnityEngine;
using System.Collections;
using UnityEditor;
using ULocalization;
using System.Linq;

[CustomEditor(typeof(LocalizeUIText))]
public class LocalizeUITextInspector : Editor {
	private float y;

	public override void OnInspectorGUI ()
	{
		LocalizeUIText text = target as LocalizeUIText;

		EditorGUILayout.LabelField("Module",text.moduleName);
		EditorGUILayout.LabelField("Label ",text.key);

		if(GUILayout.Button("Select")){
			StringSelectWindow.Show();
		}
		string selectModule;
		string selectKey;
		GUI.changed = false;
		StringSelectWindow.Select(text.moduleName,text.key,out selectModule,out selectKey);
		if(GUI.changed){
			SetUIText(selectModule,selectKey);
		}
	}

	private void SetUIText(string modulename,string key){
		var locText = target as LocalizeUIText;
		locText.Set(modulename,key);
		var txt = locText.GetComponent<UnityEngine.UI.Text>();
		EditorUtility.SetDirty(txt);
	}


	void OnEnable(){
	}

}
