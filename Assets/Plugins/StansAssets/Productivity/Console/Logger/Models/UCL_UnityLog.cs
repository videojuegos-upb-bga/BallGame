////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Productivity.Console {

	public class UCL_UnityLog {


		public const string MESSAGE_TAG_NAME = "log";
		public const string WARNING_TAG_NAME = "warning";
		public const string ERROR_TAG_NAME = "error";
		public const string EXCEPTION_TAG_NAME = "exception";



		private string _LogString;
		private string _StackTrace;

		private string _TagName = string.Empty;
		private UnityEngine.LogType _LogType;



		public UCL_UnityLog(string logString, string stackTrace, UnityEngine.LogType logType) {
			_LogString = logString;
			_StackTrace = stackTrace;
			_LogType = logType;


			//if log line has tag
			if(_LogType == LogType.Log) {
				var match = System.Text.RegularExpressions.Regex.Matches(_LogString, @"\[(.*)\]: ");
				if (match.Count > 0) {
					_TagName = match [0].Groups [1].Value;
					int subValue = _TagName.Length + 4;
					_LogString = _LogString.Substring (subValue, _LogString.Length - subValue);
				}
			}


			if(_TagName.Equals(string.Empty)) {
				switch(_LogType) {
				case LogType.Error:
					_TagName = ERROR_TAG_NAME;
					break;
				case LogType.Warning:
					_TagName = WARNING_TAG_NAME;
					break;
				case LogType.Exception:
					_TagName = EXCEPTION_TAG_NAME;
					break;
				default:
					_TagName = MESSAGE_TAG_NAME;
					break;

				}
			}



		}

		public string FullLog {
			get {
				return _LogString + "\n" + _StackTrace;
			}
		}

		public string Message {
			get {
				return _LogString;
			}
		}

		public string StackTrace {
			get {
				return _StackTrace;
			}
		}

		public string TagName {
			get {
				return _TagName;
			}
		}

		public UnityEngine.LogType LogType {
			get {
				return _LogType;
			}
		}
			
	}
}
