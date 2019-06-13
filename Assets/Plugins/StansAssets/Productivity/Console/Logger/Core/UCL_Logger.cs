////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace SA.Productivity.Console {

    public static class UCL_Logger {

        private static ILogHandler _LogHandler = null;

        private static bool AlreadyLogging = false;
        private static List<UCL_iLogMessageReceiver> Loggers = new List<UCL_iLogMessageReceiver>();


        //--------------------------------------
        // Initialisation
        //--------------------------------------

        static UCL_Logger() {
            Application.logMessageReceivedThreaded += LogReceiveHandler;
        }

        private static Dictionary<RuntimePlatform, ILogHandler> m_loggers = new Dictionary<RuntimePlatform, ILogHandler>();

        public static void SetPlatformLogger(RuntimePlatform platform, ILogHandler logger) {
            if (m_loggers.ContainsKey(platform)) {
                m_loggers[platform] = logger;
            }
            else {
                m_loggers.Add(platform, logger);
            }
        }

        public static void Init() {
            Debug.unityLogger.logHandler = LogHandler;
            if (LogHandler is UCL_iLogMessageReceiver) {
                UCL_iLogMessageReceiver receiver = (LogHandler as UCL_iLogMessageReceiver);
                AddLogReceiver(receiver);
            }
        }


        //--------------------------------------
        // Public Methods
        //--------------------------------------

        public static string GetSessionLog() {
            return Platfroms.UCL_iOS_Bridge.GetSessionLog();
        }

        public static void ShowSessionLog() {
            Platfroms.UCL_iOS_Bridge.ShowSessionLog();
        }

        public static void ShowSharingUI() {
            Platfroms.UCL_iOS_Bridge.ShowSharingUI();
        }

        public static void AddLogReceiver(UCL_iLogMessageReceiver logger) {
            lock (Loggers) {
                if (!Loggers.Contains(logger)) {
                    Loggers.Add(logger);
                }
            }
        }

        public static T GetLogReceiver<T>() where T : class {
            foreach (var logger in Loggers) {
                if (logger is T) {
                    return logger as T;
                }
            }
            return null;
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public static ILogHandler LogHandler {
            get {

                if (_LogHandler == null) {
                    if (m_loggers.ContainsKey(Application.platform) && m_loggers[Application.platform] != null) {
                        _LogHandler = m_loggers[Application.platform];
                    }
                    else {
                        switch (Application.platform) {
                            case RuntimePlatform.IPhonePlayer:

                                if (UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord) {
                                    Platfroms.UCL_iOS_Bridge.Init();
                                }

                                if (UCL_PlatfromsLogSettings.Instance.iOS_OverrideLogsOutput) {
                                    _LogHandler = new Platfroms.UCL_iOS_LogHandler();
                                }
                                else {
                                    FallbackToDefaultLogger();
                                }

                                break;
                            default:
                                FallbackToDefaultLogger();
                                break;
                        }
                    }
                }

                return _LogHandler;
            }
        }



        //--------------------------------------
        // Private Methods
        //--------------------------------------

        private static void FallbackToDefaultLogger() {
            _LogHandler = Debug.unityLogger.logHandler;
        }


        private static void LogReceiveHandler(string logString, string stackTrace, UnityEngine.LogType logType) {
            //Threads safety lock implementation
            lock (Loggers) {
                //Prevent nasty recursion problems
                if (!AlreadyLogging) {
                    try {
                        AlreadyLogging = true;
                        var logInfo = new UCL_UnityLog(logString, stackTrace, logType);

                        //Delete any dead loggers and pump them with the new log
                        Loggers.RemoveAll(l => l == null);
                        Loggers.ForEach(l => l.OnLogReceived(logInfo));
                    }
                    finally {
                        AlreadyLogging = false;
                    }
                }
            }
        }


    }


}

