using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace SA.Productivity.Console
{

    public class UCL_StackTreeViewItem : TreeViewItem
    {

        private UCL_LogStackLine m_stackLine;
        private GUIContent m_displayContent;

        public UCL_StackTreeViewItem(int id, UCL_LogStackLine stackLine) : base (id, 0, stackLine.RawData)
        {
            m_stackLine = stackLine;
            m_displayContent = new GUIContent(stackLine.RawData, stackLine.RawData);
        }

        public UCL_LogStackLine StackLine {
            get {
                return m_stackLine;
            }
        }

        public GUIContent DisplayContent {
            get {
                return m_displayContent;
            }
        }
    }
}