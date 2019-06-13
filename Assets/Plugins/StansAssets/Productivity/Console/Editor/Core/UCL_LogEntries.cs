////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @authot Alex Yaremenko (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;



namespace SA.Productivity.Console
{

	public static class UCL_LogEntries  {


        private static Type s_entriesType = null;
        private static Type s_entryType = null;
	

		//--------------------------------------
		// Public Methods
		//--------------------------------------


		public static int  GetCount() {
			return (int)Entries.GetMethod("GetCount").Invoke(null, null);
		}


		public static void Clear() {
			var clearMethod = Entries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			clearMethod.Invoke(null,null);
		}
			

		public static bool HasFlag(UCL_ConsoleFlags flags) {
			return (consoleFlags & (int)flags) != 0;
		}
			

		public static void SetFlag(UCL_ConsoleFlags flags, bool val) {
			var clearMethod = Entries.GetMethod("SetConsoleFlag", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			clearMethod.Invoke (Entries, new object[] { (int)flags, val });
		}

        #pragma warning disable 168  //, 3021
        public static int GetEntryCount(int row) {
            try {
                MethodInfo methodInfo = Entries.GetMethod("GetEntryCount");
                return (int)methodInfo.Invoke(Entries, new object[] { row });
           } catch (Exception ex) {
                return 0;
           }
			
		}
        #pragma warning restore 168


        public static List<UCL_LogInfo> GetLog(int startIndex = 0)  {

			List<UCL_LogInfo> currentOutput = new List<UCL_LogInfo> ();



            var m = Entries.GetMethod ("GetEntryInternal", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			var start = Entries.GetMethod ("StartGettingEntries", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
			start.Invoke (null, null);

			int count = GetCount ();
			for (int i = 0; i < count; i++) {

				if(i >= startIndex) {
					var e = Activator.CreateInstance (Entry);
					m.Invoke (null, new object[] { i, e });

					FieldInfo field = e.GetType().GetField("condition", BindingFlags.Instance | BindingFlags.Public);
					string condition = (string) field.GetValue(e);


					/*	field = e.GetType().GetField("errorNum", BindingFlags.Instance | BindingFlags.Public);
					int errorNum = (int) field.GetValue(e);*/


					field = e.GetType().GetField("instanceID", BindingFlags.Instance | BindingFlags.Public);
					int instanceID = (int) field.GetValue(e);


					field = e.GetType().GetField("mode", BindingFlags.Instance | BindingFlags.Public);
					int mode = (int) field.GetValue(e);



                    UCL_LogInfo log  = new UCL_LogInfo (i + 10, condition, mode, instanceID);
					log.SetLineNumber (i);
                

                    currentOutput.Add(log);
                }

			}
			var end = Entries.GetMethod ("EndGettingEntries", BindingFlags.Static | BindingFlags.Public);
			end.Invoke(null, null);



			return currentOutput;
		}
		


		//--------------------------------------
		// Get / Set
		//--------------------------------------


		public static int consoleFlags {
			get {
				PropertyInfo property = Entries.GetProperty("consoleFlags", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
				return (int) property.GetValue(null, null);
			}
		}





		private static Type Entries {
			get {
                if (s_entriesType == null) {
                    s_entriesType = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
                }
                return s_entriesType;
			}
		}


        private static Type Entry {
            get {
                if(s_entryType == null) {
                    s_entryType = System.Type.GetType("UnityEditor.LogEntry, UnityEditor.dll");
                }
                return s_entryType;
            }
        }

		//--------------------------------------
		// Private Methods
		//--------------------------------------


		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() {
			
		}
	}

}
