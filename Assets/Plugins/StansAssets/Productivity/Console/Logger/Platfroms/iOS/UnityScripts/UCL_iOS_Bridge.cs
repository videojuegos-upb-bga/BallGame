////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


#if UNITY_IPHONE && !UNITY_EDITOR 
using System.Runtime.InteropServices;
#endif

namespace SA.Productivity.Console.Platfroms {

    public class UCL_iOS_Bridge  {



        public const string DISABLED_MESSAGE = "Record logs output on iOS is disabled by user." +
            " If you need this feature enable it under Stan's Assets -> Console -> Settins (Platfroms tab)";


        #if UNITY_IPHONE && !UNITY_EDITOR 

        [DllImport ("__Internal")]
        private static extern  void _UL_Init();

        [DllImport ("__Internal")]
        private static extern  void _UL_ShowSharingUI();

        [DllImport ("__Internal")]
        private static extern  void _UL_ShowSessionLog();

        [DllImport ("__Internal")]
        private static extern string _UL_GetSessionLog();

        [DllImport ("__Internal")]
        private static extern  void _UL_LogMessage(string type, string message);

        #endif


        public static void Init() {
            #if UNITY_IPHONE && !UNITY_EDITOR 
            _UL_Init();
            #endif
        }

        public static void ShowSharingUI() {

            if(!UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord) {
                U.LogWarning (DISABLED_MESSAGE);
            }

            #if UNITY_IPHONE && !UNITY_EDITOR 
            _UL_ShowSharingUI();
            #endif
        }

        public static string GetSessionLog() {

            if(!UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord) {
                U.LogWarning (DISABLED_MESSAGE);
                return DISABLED_MESSAGE;
            }

            #if UNITY_IPHONE && !UNITY_EDITOR 
            return _UL_GetSessionLog();
            #else
            return "Session log will appear on supported platfroms. Feature is deisabled under the Unity Editor";
            #endif
        }


        public static void ShowSessionLog() {
            
            if(!UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord) {
                U.LogWarning (DISABLED_MESSAGE);
            }

            #if UNITY_IPHONE && !UNITY_EDITOR 
            _UL_ShowSessionLog();
            #endif
        }

        public static void LogMessage(string tag, string message) {
            #if UNITY_IPHONE && !UNITY_EDITOR 
            _UL_LogMessage(tag, message);
            #endif
        }
            
    }

}
