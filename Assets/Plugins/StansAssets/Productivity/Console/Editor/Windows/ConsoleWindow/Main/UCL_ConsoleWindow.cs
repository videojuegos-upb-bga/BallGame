using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using SA.Foundation.Utility;
using SA.Foundation.Editor;


namespace SA.Productivity.Console
{

    public class UCL_ConsoleWindow : EditorWindow, IHasCustomMenu
    {

		private const string SEARCH_BAR_CONTROL_NAME = "ucl_searchBat";

        private SA_SplitView m_rootView;
        private Rect m_rootViewRect = new Rect(Vector2.zero, Vector2.zero);


        private GUIStyle m_toolbarStyle;
        private GUIStyle m_toolbarSearchTextFieldStyle;
        private GUIStyle m_toolbarSearchCancelButtonStyle;

        private string m_searchString = string.Empty;
        private int m_lastLogsCount;

        private Dictionary<string, int> m_tagsMessagesCount = new Dictionary<string, int>();
       

        [SerializeField] SA_SplitViewState m_rootViewSate;
        [SerializeField] UCL_LogsView m_logsView;
        [SerializeField] UCL_StackView m_stackView;


        [SerializeField] UCL_ConsoleWindowState m_state;

        //--------------------------------------
        // Initialization
        //--------------------------------------


        private void OnEnable() {

            titleContent.text = "Console";
            if (!EditorGUIUtility.isProSkin) {
                titleContent.image = Resources.Load("console/logger_window_icon") as Texture2D;
            } else {
                titleContent.image = Resources.Load("console/logger_window_icon_pro") as Texture2D;
            }

            m_lastLogsCount = 0;
            m_toolbarStyle = null;


            m_state = UCL_Settings.ConsoleWindowState;


            if(m_rootViewSate == null) {
                m_rootViewSate = new SA_SplitViewState();
                m_logsView = new UCL_LogsView();
                m_stackView = new UCL_StackView();
            }
            
            m_rootView = new SA_SplitView(m_rootViewSate);
            m_rootView.Orientation = Orientation.Vertical;

            m_rootView.Panel1.MinSize = 150;
            m_rootView.Panel1.StartSize = 300;
            m_rootView.Panel1.SetView(m_logsView);

            m_rootView.Panel2.MinSize = 150;
            m_rootView.Panel2.SetView(m_stackView);
            m_rootView.SplitterSize = 1.0f;


            //To keep logs view up to date
            EditorApplication.update += OnEditorUpdate;
            Application.logMessageReceivedThreaded += LogReceiveHandler;


            UCL_Settings.OnSettingsUpdated.AddSafeListener(this, RebuildLogsView);
            m_logsView.OnEnable();
            m_stackView.OnEnable();
        }

       

        private void InitStyles() {

            if (m_toolbarStyle != null) { return; }

            m_toolbarStyle = GUI.skin.FindStyle("Toolbar");
            m_toolbarSearchTextFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            m_toolbarSearchCancelButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");
        }



        //--------------------------------------
        // Logs Tracking
        //--------------------------------------
       

        private void OnEditorUpdate() {

            var logsCount = UCL_LogEntries.GetCount();
            if (m_lastLogsCount == logsCount) return;
            m_lastLogsCount = logsCount;
            RebuildLogsView();
        }

        private void LogReceiveHandler(string logString, string stackTrace, LogType logType) {
            //This is the way to trigger the console logs update on a next frame
            m_lastLogsCount = -1; 
        }


        private void RebuildLogsView() {
         
            m_state = UCL_Settings.ConsoleWindowState;

            var logs = UCL_LogEntries.GetLog();
            var preFiltredLogs = new List<UCL_LogInfo>();

            foreach (var log in logs) {

                var tag = UCL_Settings.Instance.GetTag(log.TagName);
                if(m_state.IsTagEnabled(tag)) {
                    preFiltredLogs.Add(log);
                }
            }

            m_logsView.RebuildLogsView(preFiltredLogs);

            m_tagsMessagesCount = new Dictionary<string, int>();
            foreach (var log in logs) {
                IncrementMessagesCountForTag(log.TagName);
            }

        }



        //--------------------------------------
        // GUI
        //--------------------------------------

        private void OnGUI() {


          


            SetupUI();
            DrawToolbar();

            m_rootViewRect.width = position.width;
            m_rootViewRect.height = position.height;

            //Stealing focus when mouse inside the window

            if (m_rootViewRect.Contains(Event.current.mousePosition) && focusedWindow != this && GUIUtility.hotControl == 0) {

                //No window is focused, so look like Unity Editor is in background
                //Stealing focus in this case maybe pretty harmful. And cause whole application to be expanded from background,
                //without any user action.
                if(focusedWindow == null) {
                    return;
                }
                FocusWindowIfItsOpen<UCL_ConsoleWindow>();
            }

            if (m_logsView.SelectedLogs.Count > 0) {
                m_stackView.SetLog(m_logsView.SelectedLogs[0]);
            } else {
                m_stackView.SetLog(null);
            }

            BeginWindows();
            SA_InputEvent e = new SA_InputEvent(Event.current);
            m_rootView.OnGui(m_rootViewRect, e);

            EndWindows();

            Repaint();
        }


        private void SetupUI() {

            InitStyles();

            HandleInputEvents();

            UCL_LogEntries.SetFlag(UCL_ConsoleFlags.LogLevelLog, true);
            UCL_LogEntries.SetFlag(UCL_ConsoleFlags.LogLevelWarning, true);
            UCL_LogEntries.SetFlag(UCL_ConsoleFlags.LogLevelError, true);

            if (!EditorGUIUtility.isProSkin) {
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), SA_IconManager.GetIconFromHtmlColorString("#DFDFDFFF"));
            } 
        }

        private void HandleInputEvents() {

           
            Event e = Event.current;
         
            if (GUI.GetNameOfFocusedControl().Equals(SEARCH_BAR_CONTROL_NAME)) {
                EditorGUI.FocusTextInControl(SEARCH_BAR_CONTROL_NAME);
                if (Event.current.type == EventType.KeyUp) {
                    switch (Event.current.keyCode) {
                        case KeyCode.Escape:
                            Event.current.Use();
                            m_searchString = string.Empty;
                            m_logsView.SetFocusAndEnsureSelectedItem();
                            break;

                        case KeyCode.DownArrow:
                        case KeyCode.UpArrow:
                            Event.current.Use();
                            m_logsView.SetFocusAndEnsureSelectedItem();
                            break;
                    }
                   
                } 
            } else {
                if (Event.current.type == EventType.KeyDown) {

                    switch (Event.current.keyCode) {
                        case KeyCode.UpArrow:
                        case KeyCode.DownArrow:
                            //GUI.FocusControl(null);
                            break;
                        default:
                            EditorGUI.FocusTextInControl(SEARCH_BAR_CONTROL_NAME);
                            break;
                    }
                }
            }

        }



        private void DrawToolbar() {

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {

                bool clear = GUILayout.Button("Clear", EditorStyles.toolbarButton);
                if (clear) {
                    UCL_LogEntries.Clear();
                }

                GUILayout.Space(6);
               

                bool val = false;
                if (m_state.DisplayCollapse) {
                    val = UCL_LogEntries.HasFlag(UCL_ConsoleFlags.Collapse);
                    val = GUILayout.Toggle(val, "Collapse", EditorStyles.toolbarButton);
                    UCL_LogEntries.SetFlag(UCL_ConsoleFlags.Collapse, val);
                } 

                if (m_state.DisplayClearOnPlay) {
                    val = UCL_LogEntries.HasFlag(UCL_ConsoleFlags.ClearOnPlay);
                    val = GUILayout.Toggle(val, "Clear on Play", EditorStyles.toolbarButton);
                    UCL_LogEntries.SetFlag(UCL_ConsoleFlags.ClearOnPlay, val);
                } 

                if (m_state.DisplayPauseOnError) {
                    val = UCL_LogEntries.HasFlag(UCL_ConsoleFlags.ErrorPause);
                    val = GUILayout.Toggle(val, "Error Pause", EditorStyles.toolbarButton);
                    UCL_LogEntries.SetFlag(UCL_ConsoleFlags.ErrorPause, val);
                } 


                if (UnDockedUsedTagsCount > 0) {


                    if (GUILayout.Button("Tags (" + UnDockedUsedTagsCount + ")", EditorStyles.toolbarDropDown)) {
                        GenericMenu toolsMenu = new GenericMenu();

                        foreach (var pair in m_tagsMessagesCount) {

                            var tag = UCL_Settings.Instance.GetTag(pair.Key);
                            if(m_state.IsTagDocked(tag)) {
                                continue;
                            }

                            bool enabled = m_state.IsTagEnabled(tag);
                            toolsMenu.AddItem(new GUIContent(tag.Name + " (" + pair.Value + ")"), enabled, () => {
                                enabled = !enabled;
                                m_state.SetTagEnabledState(tag, enabled);
                                UCL_Settings.SaveConsoleWindowState(m_state);
                            });
                           
                        }
                        toolsMenu.ShowAsContext();
                    }
                }


                GUILayout.FlexibleSpace();
                if (m_state.DisplaySearchBar) {
                    GUILayout.BeginHorizontal(m_toolbarStyle);
                    {
                        GUI.SetNextControlName(SEARCH_BAR_CONTROL_NAME);
                        m_searchString = EditorGUILayout.TextField(m_searchString, m_toolbarSearchTextFieldStyle, GUILayout.MinWidth(200f));

                        if (GUILayout.Button("", m_toolbarSearchCancelButtonStyle)) {
                            m_searchString = "";
                            GUI.FocusControl(null);
                        }

                        m_logsView.SerSearthcString(m_searchString);

                    }
                    GUILayout.EndHorizontal();

                }


                for (int i = UCL_Settings.Instance.Tags.Count - 1; i >= 0; i--) {
                    var tag = UCL_Settings.Instance.Tags[i];
                    //just a stupid workaround we have to make
                    // since gui list plgun an add a null balue to the tags list just for a second
                    if(tag == null) { continue;}
                    if (m_state.IsTagDocked(tag)) {
                        var content = new GUIContent(GetMessagesCountForTag(tag.Name).ToString(), tag.Icon);

                        EditorGUI.BeginChangeCheck();
                        bool enabled = GUILayout.Toggle(m_state.IsTagEnabled(tag), content, EditorStyles.toolbarButton);
                        if (EditorGUI.EndChangeCheck()) {
                            m_state.SetTagEnabledState(tag, enabled);
                            UCL_Settings.SaveConsoleWindowState(m_state);
                        }
                    }
                }
                                
            }
            GUILayout.EndHorizontal();

        }


        //--------------------------------------
        // Tags
        //--------------------------------------

        private int GetMessagesCountForTag(string tagName) {

            if (m_tagsMessagesCount.ContainsKey(tagName)) {
                return m_tagsMessagesCount[tagName];
            } else {
                return 0;
            }
        }


        private int UnDockedUsedTagsCount {
            get {
                int count = 0;
                foreach (var pair in m_tagsMessagesCount) {
                    if(!m_state.IsTagDocked(pair.Key)) {
                        count++;
                    }
                }

                return count;
            }
        }


        private void IncrementMessagesCountForTag(string tagName) {
            if (m_tagsMessagesCount.ContainsKey(tagName)) {
                m_tagsMessagesCount[tagName] += 1;
            } else {
                m_tagsMessagesCount.Add(tagName, 1);
            }
        }


        //--------------------------------------
        // IHasCustomMenu
        //--------------------------------------


        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu) {

            menu.AddItem(new GUIContent("Settings"), false, () => {
                UCL_ConsolePreferencesWindow.ShowModal();
            });

            if (Application.platform == RuntimePlatform.OSXEditor) {
                menu.AddItem(new GUIContent("Open Player Log"), false, UnityEditorInternal.InternalEditorUtility.OpenPlayerConsole);
            }

            menu.AddItem(new GUIContent("Open Editor Log"), false, UnityEditorInternal.InternalEditorUtility.OpenEditorConsole);


            menu.AddItem(new GUIContent("Copy To Clipboard"), false, () => {
  
                string fullLog = string.Empty;
                
                foreach (var log in m_logsView.Logs) {
                    fullLog += string.Format(UCL_LogInfo.TAGGED_MESSAGE_FORMAT, log.TagName, log.LogString) + "\n";
                }

                EditorGUIUtility.systemCopyBuffer = fullLog;
                ShowNotification(new GUIContent("Copied To Clipboard"));
            });

            //UnityEditorInternal.InternalEditorUtility.OpenPlayerConsole

        }

      
    }
}