using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

using SA.Foundation.Editor;

namespace SA.Productivity.Console
{
    [Serializable]
    public class UCL_StackView : SA_iGUIElement
    {
        private UCL_LogInfo m_log = null;
        private Rect m_treeViewRect = new Rect();
        private UCL_StackTreeView m_treeView;
        private GUIStyle m_entryDetailedView = null;


        [SerializeField] TreeViewState m_treeViewState;
        [SerializeField] int m_lastLogID = 0;
        

        public void SetLog(UCL_LogInfo log) {

            //We already had a log to show, and now we've been asked to show a new one
            //Let's check if that is the same one and if we have to drop selection
            if(log != null) {
                if (m_log != null) {
                    if (m_lastLogID != log.Id) {
                        m_treeView.SetSelection(new List<int>());
                    }
                }

                m_lastLogID = log.Id;
            }

            m_log = log;
            m_treeView.Build(m_log);
        }

        public void OnEnable() {

            m_entryDetailedView = null;
            if (m_treeViewState == null) {
                m_treeViewState = new TreeViewState();
            }
            m_treeView = new UCL_StackTreeView(m_treeViewState);
        }


        public void OnGui(Rect rect, SA_InputEvent e) {

            StylesInit();

            using (new SA_GuiBeginArea(rect)) {
                if (m_log != null) {
                    EditorGUILayout.Space();
                    EditorGUILayout.TextArea(m_log.LogString, m_entryDetailedView);
                    EditorGUILayout.Space();

                    if (Event.current.type == EventType.Repaint) {
                        var lastRect = GUILayoutUtility.GetLastRect();
                        m_treeViewRect.x = rect.x;
                        m_treeViewRect.width = rect.width;
                        m_treeViewRect.y = lastRect.y + lastRect.height;
                        m_treeViewRect.height = rect.height - m_treeViewRect.y;
                    }
                }  
            }
               
           

            m_treeView.OnGUI(m_treeViewRect);
        }


       private void StylesInit() {
            if(m_entryDetailedView == null) {
                m_entryDetailedView = new GUIStyle(EditorStyles.textArea);
                m_entryDetailedView.normal.background = null;
                m_entryDetailedView.active.background = null;
                m_entryDetailedView.onHover.background = null;
                m_entryDetailedView.hover.background = null;
                m_entryDetailedView.onFocused.background = null;
                m_entryDetailedView.focused.background = null;
                m_entryDetailedView.margin = new RectOffset(0, 0, 0, 0);
                m_entryDetailedView.padding = new RectOffset(2, 0, 0, 0);
                m_entryDetailedView.border = new RectOffset(0, 0, 0, 0);
                m_entryDetailedView.fontSize = 11;
            }
       }

    }
}