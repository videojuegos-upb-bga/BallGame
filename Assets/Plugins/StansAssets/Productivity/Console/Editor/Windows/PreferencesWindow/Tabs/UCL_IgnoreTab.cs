using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.Productivity.Console
{
    public class UCL_IgnoreTab : SA_GUILayoutElement
    {
        private const string DESCRIBTION = "See the classes that were added to the ignore lits bellow. " +
			"If log stacktrase contains ignored class, you will be redirected to pervious class instead the ignored one. " +
			"You may remove ignored classes using this menu. \n" +
			"When you want to add class to the ignore list, right click on a coresponded log stack line inside the console window and choose the ignore option.";

        [SerializeField] Vector2 m_scrollPos;
        [SerializeField] string m_selectedWrapper = string.Empty;


        public override void OnGUI() {

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(DESCRIBTION, MessageType.Info);

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();



            GUILayout.BeginVertical(GUILayout.Width(525));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Ignored Classes",  SA_EditorStyles.OLTitle, GUILayout.ExpandWidth(true));



            Texture2D trash = SA_Skin.GetGenericIcon("trash.png");
            bool remove = GUILayout.Button(trash, SA_EditorStyles.OLTitle, GUILayout.Width(20), GUILayout.Height(20));
            if (remove) {
                UCL_Settings.Instance.IgnoredWrapperClasses.Remove(m_selectedWrapper);
            }


            GUILayout.EndHorizontal();

            m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, SA_EditorStyles.OLBox);
            foreach (var wrapper in UCL_Settings.Instance.IgnoredWrapperClasses) {
                if (GUILayout.Toggle(m_selectedWrapper == wrapper, wrapper, SA_EditorStyles.PreferencesKeysElement)) {
                    m_selectedWrapper = wrapper;
                }
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();


            GUILayout.Space(10f);
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);
            bool restoreDefaults = GUILayout.Button("Restore Defaults", GUILayout.Width(150));
            if (restoreDefaults) {
                UCL_Settings.Instance.IgnoredWrapperClasses.Clear();
            }

        }
    }
}