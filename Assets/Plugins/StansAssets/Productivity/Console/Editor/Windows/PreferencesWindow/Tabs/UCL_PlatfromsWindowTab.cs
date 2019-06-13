using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;



namespace SA.Productivity.Console
{
    public class UCL_PlatfromsWindowTab : SA_GUILayoutElement {
        private const string DESCRIBTION = "The Plugin will also give you an ability to collected, show and share logs per platfroms.";
       
        public override void OnGUI() {

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(DESCRIBTION, MessageType.Info);

            using (new SA_WindowBlockWithIndent(new GUIContent("iOS"))) {
                UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord = SA_EditorGUILayout.ToggleFiled("Logs Record", UCL_PlatfromsLogSettings.Instance.iOS_LogsRecord, SA_StyledToggle.ToggleType.EnabledDisabled);
                UCL_PlatfromsLogSettings.Instance.iOS_OverrideLogsOutput = SA_EditorGUILayout.ToggleFiled("Override XCode Output", UCL_PlatfromsLogSettings.Instance.iOS_OverrideLogsOutput, SA_StyledToggle.ToggleType.EnabledDisabled);
            }

            using (new SA_WindowBlockWithIndent(new GUIContent("Android"))) {
                UCL_PlatfromsLogSettings.Instance.Android_LogsRecord = SA_EditorGUILayout.ToggleFiled("Logs Record", UCL_PlatfromsLogSettings.Instance.Android_LogsRecord, SA_StyledToggle.ToggleType.EnabledDisabled);
                UCL_PlatfromsLogSettings.Instance.Android_OverrideLogsOutput = SA_EditorGUILayout.ToggleFiled("Override LogCat Output", UCL_PlatfromsLogSettings.Instance.Android_OverrideLogsOutput, SA_StyledToggle.ToggleType.EnabledDisabled);
            }

        }
    }
}