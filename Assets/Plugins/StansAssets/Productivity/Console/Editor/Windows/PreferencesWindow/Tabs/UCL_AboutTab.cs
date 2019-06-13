using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;


namespace SA.Productivity.Console
{
    public class UCL_AboutTab : SA_PluginAboutLayout
    {
        private const string DESCRIPTION = "The Ultimate Console Plugin v. {0}\n" +
           "Plugin adds and extended Unity like console window with set of additional features. " +
           "There is also an ability to use native console view on diffret platfroms " +
           "For feature requests or bug reports please use the information below.";

        public override void OnGUI() {

            using (new SA_WindowBlockWithIndent(new GUIContent("About"))) {
                EditorGUILayout.LabelField(string.Format(DESCRIPTION, UCL_Settings.Instance.GetFormattedVersion()), SA_EditorStyles.DescribtionLabelStyle);
                EditorGUILayout.Space();
            }

            base.OnGUI();
        }
    }
}