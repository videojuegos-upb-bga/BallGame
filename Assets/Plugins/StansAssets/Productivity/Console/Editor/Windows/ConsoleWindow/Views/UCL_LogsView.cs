using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEditor.TreeViewExamples;

using SA.Foundation.Editor;


namespace SA.Productivity.Console
{

    [Serializable]
    public class UCL_LogsView : SA_iGUIElement
    {
        
        private UCL_TreeView m_treeView;

        [SerializeField] TreeViewState m_treeViewState;
        private const string TREE_VIEW_CONTROL_NAME = "TREE_VIEW_CONTROL_NAME";

        public void RebuildLogsView(List<UCL_LogInfo> logs) {
            m_treeView.Build(logs);
        }

        public void SerSearthcString(string searthcString) {
            m_treeView.searchString = searthcString;
        }


        public List<UCL_LogInfo> SelectedLogs {
            get {
                return m_treeView.SelectedLogs;
            }
        }

        public List<UCL_LogInfo> Logs {
            get {
                return m_treeView.Logs;
            }
        }

        public void OnEnable() {

            if(m_treeViewState == null) {
                m_treeViewState = new TreeViewState();
            }
            m_treeView = new UCL_TreeView(m_treeViewState);
        }

        public void OnGui(Rect rect, SA_InputEvent e) {
            
            Rect treeRect = new Rect(rect);
            treeRect.y += 15; 
            treeRect.height -= 15; 

            if(m_treeView.ContentHeight < rect.height) {
                treeRect.height = m_treeView.ContentHeight;
            }

            GUI.SetNextControlName(TREE_VIEW_CONTROL_NAME);
            m_treeView.OnGUI(treeRect);
          
        }


        public void SetFocusAndEnsureSelectedItem() {
            m_treeView.SetFocusAndEnsureSelectedItem();
        }


    }
}