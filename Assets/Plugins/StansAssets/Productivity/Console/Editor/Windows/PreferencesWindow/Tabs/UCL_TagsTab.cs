using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using Rotorz.ReorderableList;


namespace SA.Productivity.Console
{
    public class UCL_TagsTab : SA_GUILayoutElement
    {
        private const string DESCRIBTION = "Define images related to tag using this menu. If the log line without known teg will appear in console, " +
			"it will use the default tag image.";

        public override void OnGUI() {

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(DESCRIBTION, MessageType.Info);

            using (new SA_WindowBlockWithIndent(new GUIContent("Defined tags"))) {

      
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                {
                    //workaround
                    foreach(var tag in UCL_Settings.Instance.Tags) {
                        if(tag == null) {
                            UCL_Settings.Instance.Tags.Remove(tag);
                            var newTag = new UCL_ConsoleTag();
                            newTag.Name = "new_tag";
                            newTag.Icon = Resources.Load("icons/tag") as Texture2D;
                            UCL_Settings.Instance.Tags.Add(newTag);
                            break;
                        }
                    }
                    
                    ReorderableListGUI.ListField(UCL_Settings.Instance.Tags,
                        (Rect position, UCL_ConsoleTag tag) => {
                            position.x -= 12;


                            Rect iconRect = new Rect(position);
                            iconRect.width = 200;
                            iconRect.y += 88;

                            if(tag == null) {
                                return tag;
                            }

                            if(tag.Name.Equals(UCL_Settings.MESSAGE_TAG_NAME) 
                               || tag.Name.Equals(UCL_Settings.WARNING_TAG_NAME) 
                               || tag.Name.Equals(UCL_Settings.ERROR_TAG_NAME)) {
                                    GUI.enabled = false;
                            }

                            using (new SA_GuiBeginArea(iconRect)) {
                                tag.Icon = (Texture2D)EditorGUILayout.ObjectField(tag.Icon, typeof(Texture2D), false, new GUILayoutOption[] { GUILayout.Width(iconRect.width), GUILayout.Height(15) });
                            }

                            Rect labelRect = new Rect(position);
                            labelRect.x = iconRect.x + iconRect.width;
                            labelRect.width =  position.width - iconRect.width; 
                            tag.Name = EditorGUI.TextField(labelRect, tag.Name);

                            GUI.enabled = true;
                            return tag;
                        },
                        () => {

                        }
                     );
                }
                EditorGUILayout.EndVertical();

                bool restore = GUILayout.Button("Restore Defaults", GUILayout.Width(140));
                if (restore) {
                    UCL_Settings.Instance.Tags.Clear();
                }
                
            }

           

        }
    }
}