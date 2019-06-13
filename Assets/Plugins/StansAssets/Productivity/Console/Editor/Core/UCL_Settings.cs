using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


using SA.Foundation.Config;
using SA.Foundation.Events;
using SA.Foundation.Patterns;



namespace SA.Productivity.Console
{
    public class UCL_Settings : SA_ScriptableSingletonEditor<UCL_Settings>
    {

        private static SA_Event m_onSettingsUpdated = new SA_Event();

        //--------------------------------------
        // Const
        //--------------------------------------

        public const string MESSAGE_TAG_NAME = "message";
        public const string WARNING_TAG_NAME = "warning";
        public const string ERROR_TAG_NAME = "error";

        private const string CONSOLE_WINDOW_STATE_SETTINGS = "CONSOLE_WINDOW_STATE_SETTINGS";


        //--------------------------------------
        // Console Window
        //--------------------------------------


        public List<string> IgnoredWrapperClasses = new List<string>();
        [SerializeField] List<UCL_ConsoleTag> m_tags = new List<UCL_ConsoleTag>();



        //--------------------------------------
        // Static
        //--------------------------------------


        public static void SaveConsoleWindowState(UCL_ConsoleWindowState state) {
            string json = JsonUtility.ToJson(state);
            EditorPrefs.SetString(CONSOLE_WINDOW_STATE_SETTINGS, json);

            m_onSettingsUpdated.Invoke();
        }

        public static UCL_ConsoleWindowState ConsoleWindowState {
            get {
                if (EditorPrefs.HasKey(CONSOLE_WINDOW_STATE_SETTINGS)) {
                    string json = EditorPrefs.GetString(CONSOLE_WINDOW_STATE_SETTINGS);
                    var state = JsonUtility.FromJson<UCL_ConsoleWindowState>(json);
                    return state;
                } else {
                    var state = new UCL_ConsoleWindowState();
                    state.SetTagDockedState(Instance.GetTag(MESSAGE_TAG_NAME), true);
                    state.SetTagDockedState(Instance.GetTag(WARNING_TAG_NAME), true);
                    state.SetTagDockedState(Instance.GetTag(ERROR_TAG_NAME),   true);

                    return state;
                }
            }
        }


        //--------------------------------------
        // Public Methods
        //--------------------------------------


        public void IgnoreScript(string script) {
            if(!IgnoredWrapperClasses.Contains(script)) {
                IgnoredWrapperClasses.Add(script);
            }

            SaveSettings();
        }

        public bool IsScriptIgnored(string script) {
            return IgnoredWrapperClasses.Contains(script);
        }

        public UCL_ConsoleTag GetTag(string tagName) {

            foreach (var tag in Tags) {
                if (tag.Name.Equals(tagName)) {
                    return tag;
                }
            }

            var newTag = new UCL_ConsoleTag();
            newTag.Name = tagName;
            newTag.Icon = Resources.Load("icons/tag") as Texture2D;
            m_tags.Add(newTag);

            SaveSettings();

            return newTag;
        }


        //--------------------------------------
        // Get /  Set
        //--------------------------------------



        public List<UCL_ConsoleTag> Tags {
            get {
                if(m_tags.Count == 0) {
                    var tag = new UCL_ConsoleTag();
                    tag.Name = ERROR_TAG_NAME;
                    tag.Icon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = WARNING_TAG_NAME;
                    tag.Icon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
                    m_tags.Add(tag);


                    tag = new UCL_ConsoleTag();
                    tag.Name = MESSAGE_TAG_NAME;
                    tag.Icon = EditorGUIUtility.FindTexture("d_console.infoicon.sml");
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "network";
                    tag.Icon = Resources.Load("icons/netwrok") as Texture2D;
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "gameplay";
                    tag.Icon = Resources.Load("icons/gameplay") as Texture2D;
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "service";
                    tag.Icon = Resources.Load("icons/service") as Texture2D;
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "cloud";
                    tag.Icon = Resources.Load("icons/cloud") as Texture2D;
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "in";
                    tag.Icon = Resources.Load("icons/down") as Texture2D;
                    m_tags.Add(tag);

                    tag = new UCL_ConsoleTag();
                    tag.Name = "out";
                    tag.Icon = Resources.Load("icons/up") as Texture2D;
                    m_tags.Add(tag);

                    SaveSettings();
                }

                return m_tags;
            }
        }

        public static SA_Event OnSettingsUpdated {
            get {
                return m_onSettingsUpdated;
            }
        }

       
         

        public static void SaveSettings() {
            Save();
            m_onSettingsUpdated.Invoke();
        }



        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------

        protected override string BasePath {
            get { return UCL_PlatfromsLogSettings.PLUGIN_FOLDER; }
        }


        public override string PluginName {
            get {
                return UCL_PlatfromsLogSettings.Instance.PluginName;
            }
        }

        public override string DocumentationURL {
            get {
                return UCL_PlatfromsLogSettings.Instance.DocumentationURL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return UCL_PlatfromsLogSettings.Instance.SettingsUIMenuItem;
            }
        }

    }
}