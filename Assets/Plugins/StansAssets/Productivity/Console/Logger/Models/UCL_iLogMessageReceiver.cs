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

	public interface UCL_iLogMessageReceiver  {

		void OnLogReceived (UCL_UnityLog log);
	}
}
