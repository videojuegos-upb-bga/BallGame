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

namespace SA.Productivity.Console
{


	[System.Serializable]
	public class UCL_LogInfo {

        [SerializeField] int m_id = 0;
        [SerializeField] string m_logString = string.Empty;
        [SerializeField] string m_stackTrace  = string.Empty;
        [SerializeField] UCL_FilePointer m_pointer = null;
        [SerializeField] List<UCL_LogStackLine> m_stack = new List<UCL_LogStackLine> ();
        [SerializeField] LogType m_logType = LogType.Log;
        [SerializeField] string m_tagName = string.Empty;
        [SerializeField] int m_mode = 0;
        [SerializeField] int m_instanceID = 0;
        [SerializeField] int m_lineNumber = 0;


        public const string TAGGED_MESSAGE_FORMAT = "[{0}]: {1}";

        //--------------------------------------
        // Initialisation
        //--------------------------------------

    

		public UCL_LogInfo(int id, string logString, int mode, int instanceId) {

            m_id = id;
			m_mode = mode;
			m_instanceID = instanceId;

			if(HasMode(UCL_LogMode.AssetImportError) || HasMode(UCL_LogMode.Error) || HasMode(UCL_LogMode.GraphCompileError) || HasMode(UCL_LogMode.ScriptCompileError) || HasMode(UCL_LogMode.ScriptingError) || HasMode(UCL_LogMode.StickyError)) {
				m_logType = LogType.Error;
			} else if(HasMode(UCL_LogMode.AssetImportWarning) || HasMode(UCL_LogMode.ScriptCompileWarning) || HasMode(UCL_LogMode.ScriptingWarning)) {
				m_logType = LogType.Warning;
			} else {
				m_logType = LogType.Log;
			}



			//Parsing stack trase and log message
			bool stackFound = false;
			string[] lines = null;
			if (Application.platform == RuntimePlatform.OSXEditor) {
				lines = System.Text.RegularExpressions.Regex.Split (logString, System.Environment.NewLine);
			} else {
				lines = System.Text.RegularExpressions.Regex.Split (logString, "\n");
			}

			if(logString.Contains("UnityEngine.Debug:")) {
				foreach(string line in lines) {
					if(line.Contains("UnityEngine.Debug:")) {
						stackFound = true;
					}

					if(!stackFound) {
						m_logString += line;
					} else {
						AddStackLine (line);
					}
				}
			} else {
				foreach (string line in lines) {
					if(!stackFound) {
						m_logString = line;
						stackFound = true;
					} else {
						AddStackLine (line);
					}
				}
			}


				

			//In case this is an error or warning
			var match = System.Text.RegularExpressions.Regex.Matches(m_logString, @"Assets\/(.*)\((\d+),(\d+)\):");
			if(match.Count > 0) {
				string name = "Assets/" + match[0].Groups[1].Value;
				int line = System.Convert.ToInt32(match[0].Groups[2].Value);
				m_pointer = new UCL_FilePointer (name, line);
			} 

			//if log line has tag
			if(m_logType == LogType.Log) {
				match = System.Text.RegularExpressions.Regex.Matches(m_logString, @"\[(.*)\]: ");
				if (match.Count > 0) {
					m_tagName = match [0].Groups [1].Value;
					int subValue = m_tagName.Length + 4;
					m_logString = m_logString.Substring (subValue, m_logString.Length - subValue);
				}
			}

			if(m_tagName.Equals(string.Empty)) {
				switch(m_logType) {
				case LogType.Error:
                case LogType.Assert:
					m_tagName = UCL_Settings.ERROR_TAG_NAME;
					break;
				case LogType.Warning:
					m_tagName = UCL_Settings.WARNING_TAG_NAME;
					break;
				default:
					m_tagName = UCL_Settings.MESSAGE_TAG_NAME;
					break;
					
				}
			}

				

		}


	




		//--------------------------------------
		// Public Methods
		//--------------------------------------

		public bool HasMode(UCL_LogMode modeToCheck) {
			return (m_mode & (int)modeToCheck) != 0;
		}

		public void SetLineNumber(int line) {
			m_lineNumber = line;
		}


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public int Id {
            get {
                return m_id;
            }
        }

        public string LogString {
			get {
				return m_logString;
			}

			set {
				m_logString = value;
			}
		}

		public string StackTrace {
			get {
				return m_stackTrace;
			}
		}

		public LogType LogType {
			get {
				return m_logType;
			}
		}

		public List<UCL_LogStackLine> Stack {
			get {
				return m_stack;
			}
		}

		public UCL_FilePointer Pointer {
			get {
				return m_pointer;
			}
		}

		public string TagName {
			get {
				return m_tagName;
			}
		}
			

		public int InstanceID {
			get {
				return m_instanceID;
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

		public int LineNumber {
			get {
				return m_lineNumber;
			}
		}


        public int EntryCount {
            get {
#if UNITY_EDITOR
                return UCL_LogEntries.GetEntryCount(LineNumber);
#else
                return 0;
#endif
            }
        }



        public Texture2D Icon {
            get {
                return UCL_Settings.Instance.GetTag(TagName).Icon;
            }
        }

        private GUIContent m_lineContent = null;
        public GUIContent LineGUIContent {

            get {
                if (m_lineContent == null) {
                    var showMessage = LogString;

                    /*
                    if (!TagName.Equals(string.Empty) && LoggerSettings.Instance.ShowTagInMessageLine) {
                        showMessage = string.Format(TAGGED_MESSAGE_FORMAT, TagName, LogString);
                    }
                    */

                    m_lineContent = new GUIContent(showMessage, UCL_Settings.Instance.GetTag(TagName).Icon);
                }

                return m_lineContent;
            }
 
        }



        //--------------------------------------
        // Private Methods
        //--------------------------------------



        private void AddStackLine(string line) {
			if(line.Length > 1) {
				var stackLine = new UCL_LogStackLine(line);
				stackLine.SetLineNumber (Stack.Count);
				Stack.Add(stackLine);
			}
		}

	}
}
