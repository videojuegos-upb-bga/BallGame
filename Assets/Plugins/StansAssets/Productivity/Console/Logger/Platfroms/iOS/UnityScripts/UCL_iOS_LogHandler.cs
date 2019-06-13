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
using System;

namespace SA.Productivity.Console.Platfroms {

	public class UCL_iOS_LogHandler : UCL_LogHandler {

		//--------------------------------------
		// Overrided
		//--------------------------------------

        protected override void LogMessage(string logtype, string message) {
            UCL_iOS_Bridge.LogMessage (logtype, message);
		}





	}
}