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

		EditorGUILayout.LabelField("Moduel",text.moduelName);
		EditorGUILayout.LabelField("Label ",text.key);

		if(GUILayout.Button("Select")){
			StringSelectWindow.Show();
		}
		string selectModuel;
		string selectKey;
		GUI.changed = false;
		StringSelectWindow.Select(text.moduelName,text.key,out selectModuel,out selectKey);
		if(GUI.changed){
			SetUIText(selectModuel,selectKey);
		}
	}

	private void SetUIText(string moduelname,string key){
		var locText = target as LocalizeUIText;
		locText.Set(moduelname,key);
		var txt = locText.GetComponent<UnityEngine.UI.Text>();
		EditorUtility.SetDirty(txt);
	}


	void OnEnable(){
	}

}
