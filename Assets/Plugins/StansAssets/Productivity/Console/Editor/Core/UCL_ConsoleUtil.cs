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
using UnityEditor;

namespace SA.Productivity.Console {

	public  static class UCL_ConsoleUtil
    {



		//--------------------------------------
		// Right Click Actions
		//--------------------------------------


		public static void ShowLogLineMenu(UCL_LogInfo log) {
			GenericMenu toolsMenu = new GenericMenu();
			toolsMenu.AddItem (new GUIContent ("Copy"), false, () => {
				EditorGUIUtility.systemCopyBuffer = log.LogString;
				//LoggerWindow.Instance.ShowNotification( new GUIContent("Copied To Clipboard"));
			});

			toolsMenu.AddItem (new GUIContent ("Copy with Stack Trace"), false, () => {

				string fullLog = log.LogString  + "\n";

				foreach(var line in log.Stack) {
					fullLog+= line.RawData + "\n";
				}

				EditorGUIUtility.systemCopyBuffer = fullLog;
				//LoggerWindow.Instance.ShowNotification( new GUIContent("Copied To Clipboard"));

			});

			toolsMenu.ShowAsContext();
		}

		public static void ShowLogStackLineMenu(UCL_LogStackLine stackLine) {

			GenericMenu toolsMenu = new GenericMenu();

			toolsMenu.AddItem (new GUIContent ("Open"), false, () => {
				JumpToSource(stackLine);
			});

			toolsMenu.AddItem (new GUIContent ("Copy"), false, () => {
				EditorGUIUtility.systemCopyBuffer = stackLine.RawData;
				//LoggerWindow.Instance.ShowNotification( new GUIContent("Copied To Clipboard"));
			});

			toolsMenu.AddSeparator ("");

			toolsMenu.AddItem (new GUIContent ("Ignore"), false, () => {
				if(stackLine.Pointer != null) {
                    UCL_Settings.Instance.IgnoreScript(stackLine.Pointer.FileName);
                }
			});
				
			toolsMenu.ShowAsContext();
		}


		//--------------------------------------
		// Double Click Actions
		//--------------------------------------



		public static void JumpToSource(UCL_LogInfo log) {
			if(log.HasValidFilePointer) {
				JumpToSource (log.Pointer);
			}

			foreach(var stack in log.Stack) {

				if(!stack.HasValidFilePointer) {
					continue;
				}

				JumpToSource (stack.Pointer);
				break;
			}
		}

		public static void JumpToSource(UCL_LogStackLine stack) {
			if(!stack.HasValidFilePointer) {
				return;
			}

			JumpToSource (stack.Pointer);
		}


		public static void JumpToSource(UCL_FilePointer pointer) {
			pointer.Open ();
		}
	}

}