using UnityEngine;
using System.Collections;
using ULocalization;

public class example : MonoBehaviour {

	void Awake(){
		var systemLoc = new Localize("strings");
		systemLoc.Load();
		Debug.Log(systemLoc.Get("appName"));
	}
}
