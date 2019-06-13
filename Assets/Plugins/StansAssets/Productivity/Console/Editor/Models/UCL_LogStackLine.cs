////////////////////////////////////////////////////////////////////////////////
//  
// @module Ultimate Logger
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace SA.Productivity.Console
{

	[Serializable]
	public class UCL_LogStackLine  {

		private string m_rawData = string.Empty;
		private UCL_FilePointer m_pointer = null;
		private int m_lineNumber = 0;



		//--------------------------------------
		// Initialization
		//--------------------------------------


		public UCL_LogStackLine(string unityStackFrame) {
			m_rawData = unityStackFrame;
            var match = System.Text.RegularExpressions.Regex.Matches(m_rawData, @".*\(at (.*):(\d+)");
            if (match.Count > 0) {
                string name = match[0].Groups[1].Value;
                int line = Convert.ToInt32(match[0].Groups[2].Value);
                m_pointer = new UCL_FilePointer(name, line);

                if (UCL_Settings.Instance.IsScriptIgnored(m_pointer.FileName)) {
                    m_pointer = null;
                }
            }
        }

		//--------------------------------------
		// Public Methods
		//--------------------------------------

		public void SetLineNumber(int number) {
			m_lineNumber = number;
		}


		//--------------------------------------
		// Get / Set
		//--------------------------------------
			

		public string RawData {
			get {
				return m_rawData;
			}
		}

		public int LineNumber {
			get {
				return m_lineNumber;
			}
		}			

		public UCL_FilePointer Pointer {
			get {
				return m_pointer;
			}
		}

		public bool HasValidFilePointer {
			get {
				if(Pointer == null) {
					return false;
				}

				return Pointer.CanBeOpened;
			}
		}
	}

}