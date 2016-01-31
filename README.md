# ULocalization
Unity Plugin designed for text localization

# Setup

Copy all files to your Unity Project

# Usage

    using ULocalization; //namespace
    
    void Start(){
    	var loc = new Localize("strings", //json file name
    		SystemLanguage.English, //fallback language
    		null, //custom loader, if pass null,Resources.Load will be used
    	)
    	string appName = loc.Get("appName") //get string value by key
    
    }
