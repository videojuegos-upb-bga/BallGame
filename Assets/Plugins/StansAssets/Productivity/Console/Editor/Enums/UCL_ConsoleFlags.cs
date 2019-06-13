////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @authot Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


namespace SA.Productivity.Console
{


	public enum UCL_ConsoleFlags {
		Collapse = 1,
		ClearOnPlay,
		ErrorPause = 4,
		Verbose = 8,
		StopForAssert = 16,
		StopForError = 32,
		Autoscroll = 64,
		LogLevelLog = 128,
		LogLevelWarning = 256,
		LogLevelError = 512
	}
			
}