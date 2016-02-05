using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace ULocalization{
	public class DecoderImpl  {


		public static Collection JsonDecode(string content){
			return LitJson.JsonMapper.ToObject<Collection>(content);
		}

		public static Collection CSVDecode(string content){
			string[][] cells = new CSVDecoder().Decode(content);
			var collection = new Collection();
			Dictionary<string,string> configMap = new Dictionary<string, string>();
			string[] configs = cells[0];
			for(int i = 0;i<configs.Length/2;i++){
				var key = configs[2*i];
				if(string.IsNullOrEmpty(key.Trim())){
					break;
				}
				var value = configs[2*i+1];
				configMap.Add(key,value);
			}
			collection.language = configMap["language"];
			collection.strings = new Dictionary<string, string>();
			for(int i = 1;i<cells.Length;i++){
				if(string.IsNullOrEmpty(cells[i][0].Trim())){
					break;
				}
				collection.strings.Add(cells[i][0],cells[i][1]);
			}
			return collection;
		}

//		[UnityEditor.MenuItem("Assets/test")]
//		public static void test(){
//			CSVDecode(
//@"""aa,a"",1111
//""bb""""bb"",2222
//ccccc,33333");
//		}
	}


	public class CSVDecoder{

		public string[][] Decode(string content){
			
			string[] lines = content.Split(new string[]{"\r\n","\r","\n"},System.StringSplitOptions.RemoveEmptyEntries);

			string[][] ret = new string[lines.Length][];
			int idx = 0;
			foreach(var line in lines){
				_currentLine = line;
				ResetForLineDecode();
				string[] words = DecodeLine();
				ret[idx++] = words;
			}
			return ret;
		}

		private void ResetForLineDecode(){
			_charIndex = 0;
			_inQuote = false;
			_word = new StringBuilder();
			_words.Clear();
		}
		private string[] DecodeLine(){
			var end = ReadChar();
			while(!end){
				end = ReadChar();
			}
			return _words.ToArray();
		}
		private string _currentLine;
		private int _charIndex = 0;
		private bool _inQuote = false;
		private StringBuilder _word = new StringBuilder();
		private List<string> _words  = new List<string>();

		private char? NextChar(){
			if(_charIndex >= _currentLine.Length){
				return null;
			}
			char c = _currentLine[_charIndex++];
			return c;
		}

		private char? PeekNextChar(){
			if(_charIndex >= _currentLine.Length){
				return null;
			}
			return _currentLine[_charIndex];
		}

		private void CompleteWord(){
			_words.Add(_word.ToString());
			_word = new StringBuilder();
		}

		private bool ReadChar(){
			char? c = NextChar();
			if( c == null){
				CompleteWord();
				return true; //end
			}
			if(!_inQuote){ 
				if(c == '"'){
					_inQuote = true;
				}else if(c == ','){
					CompleteWord();
				}else{
					_word.Append(c);
				}
			}else{
				if(c =='"'){
					var nextChar = PeekNextChar();
					if(nextChar == '"'){
						_word.Append('"');
						NextChar(); //skip
					}else{
						_inQuote = false;
					}

				}else{
					_word.Append(c);
				}
			}
			return false;
		}
	}


}
