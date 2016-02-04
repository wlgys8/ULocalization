using UnityEngine;
using System.Collections;
using ULocalization;

public class example : MonoBehaviour {

	void Awake(){
		Localize.Load("strings",delegate(Localize obj) {
			
		});
		Debug.Log(Localize.active.Get("appName"));
	}
}
