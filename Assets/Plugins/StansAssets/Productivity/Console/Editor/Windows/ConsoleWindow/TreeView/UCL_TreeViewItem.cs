using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;

namespace SA.Productivity.Console
{

    public class UCL_TreeViewItem : TreeViewItem
    {

        private UCL_LogInfo m_logInfo;

        public UCL_TreeViewItem(UCL_LogInfo log) : base (log.Id, 0, log.LogString)
        {
            m_logInfo = log;
        }


        public UCL_LogInfo Log {
            get {
                return m_logInfo;
            }
        }
    }
}