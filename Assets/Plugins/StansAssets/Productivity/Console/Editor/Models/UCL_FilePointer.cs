////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SA.Productivity.Console
{

	[Serializable]
	public class UCL_FilePointer  {

		private string _FileName = string.Empty;
		private int _LineNumber = 0;


		//--------------------------------------
		// Initialisation
		//--------------------------------------

		public UCL_FilePointer(string name, int line) {
			_FileName = name;
			_LineNumber = line;
		}


		//--------------------------------------
		// Public Methods
		//--------------------------------------


		public void Open() {
			if (CanBeOpened) {
				var script = AssetDatabase.LoadAssetAtPath<MonoScript>(FileName);
				AssetDatabase.OpenAsset(script.GetInstanceID(), LineNumber);
			}
		}


		//--------------------------------------
		// Get / Set
		//--------------------------------------
			

		public bool CanBeOpened {
			get {
				return System.IO.File.Exists (FileName);
			}
		}

		public string FileName {
			get {
				return _FileName;
			}
		}

		public int LineNumber {
			get {
				return _LineNumber;
			}
		}

	}
}
