# ULocalization
Unity Plugin designed for text localization

* use json to config the localization strings.
* support fallback
* support custom loader

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
    		"strings":{
    			"appName":"Star War",
    			"key1":"value1",
    			"key2":"value2",
    		}
    	}
	
        
# Usage

    using ULocalization; //namespace
    
    void Start(){
    	var loc = new Localize("strings", //json file name
    		SystemLanguage.English, //fallback language
    		null, //custom loader, if pass null,Resources.Load will be used
    	)
    	loc.Load();
    	string appName = loc.Get("appName") //get string value by key
    
    }
