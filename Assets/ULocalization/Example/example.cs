using UnityEngine;
using System.Collections;
using ULocalization;

public class example : MonoBehaviour {

	void Awake(){
		Localize.Load("battle.json",delegate(Localize obj) {
			
		});
		Localize.Load("menu.csv",delegate(Localize obj) {
			
		});

		Debug.Log(Localize.GetModule("menu").Get("appName"));
	}
}
