using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;


namespace SA.Productivity.Console
{

    public class UCL_ConsolePreferencesWindow : SA_PreferencesWindow<UCL_ConsolePreferencesWindow>
    {

        protected override void OnAwake() {


            titleContent = new GUIContent("Console Preferences");

            AddSection("Console", CreateInstance<UCL_CurrentWindowTab>());
            AddSection("Platforms", CreateInstance<UCL_PlatfromsWindowTab>());
            AddSection("Ignore List", CreateInstance<UCL_IgnoreTab>());
            AddSection("Tags", CreateInstance<UCL_TagsTab>());

			AddSection("About", CreateInstance<UCL_AboutTab>());
           

        }


        protected override void BeforeGUI() {

            EditorGUI.BeginChangeCheck();
        }


        protected override void AfterGUI() {
            if(EditorGUI.EndChangeCheck()) {
                UCL_Settings.SaveSettings();
            }
        }

    }
}