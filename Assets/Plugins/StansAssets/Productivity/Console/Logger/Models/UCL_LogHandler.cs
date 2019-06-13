////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using System.Collections;


namespace SA.Productivity.Console {

	public abstract class UCL_LogHandler : ILogHandler, UCL_iLogMessageReceiver {


		//--------------------------------------
		// Abstract
		//--------------------------------------

		protected abstract void LogMessage (string logtype, string message);


		//--------------------------------------
		// Overrided
		//--------------------------------------



		public void LogFormat (LogType logType, UnityEngine.Object context, string format, params object[] args) {
			UCL_UnityLog log = new UCL_UnityLog (String.Format (format, args), "", logType);
			LogMessage (log.TagName, log.Message);
		}

		public void LogException (Exception exception, UnityEngine.Object context) {
			UCL_UnityLog log = new UCL_UnityLog (exception.Message, exception.StackTrace, LogType.Exception);
			LogMessage (log.TagName, log.FullLog);
		}


		//--------------------------------------
		// ILogMessageReceiver
		//--------------------------------------

		public void OnLogReceived(UCL_UnityLog log) {
			LogMessage (log.TagName, log.Message);
		//	LogMessage (log.LogType, log.LogString + "\n" + log.StackTrace);
		}
			
	}
}