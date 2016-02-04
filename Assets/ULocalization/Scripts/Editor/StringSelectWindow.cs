using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace ULocalization{
	public class StringSelectWindow : EditorWindow {

		public List<string> stringSet = new List<string>();

		public static void Show(){
			var win = EditorWindow.GetWindow<StringSelectWindow>(true);
			win.ShowPopup();
		}

		public static void Select(string moduel,string  key,out string selectModuel,out string selectKey){
			if(gotSelection){
				selectModuel = _selectedModuel;
				selectKey = _selectedKey;
				gotSelection = false;
				GUI.changed = true;
			}else{
				selectModuel = moduel;
				selectKey = key;
			}
		}

		public static bool gotSelection = false;

		private static string _selectedModuel;
		private static string _selectedKey;

		private Localize _moduel;
		private string _moduelName;

		void OnEnable(){
			string[] moduels;
			EditorLocalize.GetModuelNames(out moduels);
			_moduelName = moduels[0];
			_moduel = EditorLocalize.LoadModuel(_moduelName);

			stringSet = _moduel.GetAllKeys().ToList();
			filter = "";
		}

		private List<StringItemDrawer> _avaliableItems = new List<StringItemDrawer>();

		private string _filter = "";

		public string filter{
			set{
				_filter = value;
				List<string> keys = stringSet.FindAll(delegate(string str) {
					return str.Contains(value);
				});
				_avaliableItems.Clear();
				foreach(string key in keys){
					_avaliableItems.Add(new StringItemDrawer(key,_moduel.Get(key)));
				}
				_scrollPos = Vector2.zero;
			}get{
				return _filter;
			}
		}

		private Vector2 _scrollPos;

		void OnGUI(){
			GUI.changed = false;
			string ret = EditorGUILayout.TextField("Search",this.filter);
			if(GUI.changed){
				this.filter = ret;
			}
			_scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
			foreach(var item in _avaliableItems){
				if(item.DrawGUI()){
					gotSelection = true;
					_selectedKey = item.key;
					_selectedModuel = _moduelName;
					this.Close();
				}
				GUILayout.Space(5);
			}
			EditorGUILayout.EndScrollView();

		}


		private class StringItemDrawer{

			private string _key;
			private string _value;

			public StringItemDrawer(string key,string value){
				_key = key;
				_value = value;
			}

			public string key{
				get{
					return _key;
				}
			}

			private bool _pressed = false;

			public bool DrawGUI(){
				bool clicked = false;
				Vector2 keySize = GUI.skin.label.CalcSize(new GUIContent(_key));
				Vector2 contentSize = GUI.skin.label.CalcSize(new GUIContent(_value));
				Rect itemRect = GUILayoutUtility.GetRect(Screen.width,keySize.y + contentSize.y);

				if(Event.current.type == EventType.MouseDown){
					if(itemRect.Contains( Event.current.mousePosition)){
						_pressed = true;
						EditorWindow.focusedWindow.Repaint();
					}
				}
				if(Event.current.type == EventType.MouseUp){
					if(_pressed && itemRect.Contains(Event.current.mousePosition)){
						//do click
						clicked = true;
					}
					_pressed = false;
					EditorWindow.focusedWindow.Repaint();
				}

				var color = GUI.color;
				if(_pressed){
					GUI.color = Color.red;
				}
				GUI.Box(itemRect,"",EditorStyles.helpBox);
				GUI.color = color;

				GUI.Label(new Rect(itemRect.xMin,itemRect.yMin,itemRect.width,keySize.y),this._key+":");

				var contentRect = new Rect(itemRect.xMin,itemRect.yMin + keySize.y,itemRect.width,contentSize.y);

				var bkColor = GUI.skin.label.normal.textColor;
				GUI.skin.label.normal.textColor = Color.blue;
				GUI.Label(contentRect,this._value);
				GUI.skin.label.normal.textColor = bkColor;

				return clicked;
			}

		}

	}
}
