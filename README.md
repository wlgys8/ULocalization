# ULocalization
Unity Plugin designed for text localization

* use json to config the localization strings.
* support fallback
* support custom loader
* support powerful editor operation
* 
# Setup

1. Copy all files to your Unity Project
2. Open example scene under "Assets/ULocalization/Example" 
3. Press run

# Build your custom localization.

1. Delete Example folder first.

2. Create folder structure like below:
3. 
        Resources/
        Resources/Chinese
        Resources/Chinese/<your custom file name>.json
        ...
        Resources/
        Resources/English
        Resources/English/<your custom file name>.json

3. json file format

        {
        	"moduelName":"menu",
        	"language":"English",
    		"strings":{
    			"appName":"Star War",
    			"key1":"value1",
    			"key2":"value2"
    		}
    	}
	
        
# Code Usage

    using ULocalization; //namespace
    
    void Start(){
    	Localize.Load("moduelName", //moduel name
    	delegate(localize){ //onloadComplete callback
    	
    	},SystemLanguage.Chinese, //fallback language
    	null); //custom loader,if pass null,Resources.Load will be used
	string appName =Localize.active.Get("appName") //get string value by key
    }
    
# Editor Usage

A component named LocalizeUIText can be used for localizing `UnityEngine.UI.Text`'s content automatically .

待补充


