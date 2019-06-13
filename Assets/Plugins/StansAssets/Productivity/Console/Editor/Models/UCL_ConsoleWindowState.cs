using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Productivity.Console
{

    [Serializable]
    public class UCL_ConsoleWindowState
    {
        
        public bool DisplayCollapse     = true; 
        public bool DisplayClearOnPlay  = true; 
        public bool DisplayPauseOnError = true; 
        public bool DisplaySearchBar   = true;

        public bool RichText = true;

        [SerializeField] List<string> m_dockedTags = new List<string>();
        [SerializeField] List<string> m_enabledTags = new List<string>();



        public void SetTagDockedState(UCL_ConsoleTag tag, bool state) {
            if(state) {
                if (!m_dockedTags.Contains(tag.Name)) {
                    m_dockedTags.Add(tag.Name);
                    m_enabledTags.Add(tag.Name);
                }
            } else {
                m_dockedTags.Remove(tag.Name);
                m_enabledTags.Remove(tag.Name);
            }
        }

        public void SetTagEnabledState(UCL_ConsoleTag tag, bool state) {
            if (state) {
                if (!m_enabledTags.Contains(tag.Name)) {
                    m_enabledTags.Add(tag.Name);
                }
            } else {
                m_enabledTags.Remove(tag.Name);
            }
        }


        public bool IsTagDocked(UCL_ConsoleTag tag) {
            return IsTagDocked(tag.Name);
        }

        public bool IsTagDocked(string tagName) {
            return m_dockedTags.Contains(tagName);
        }

        public bool IsTagEnabled(UCL_ConsoleTag tag) {

            // if Tag is not enabled and not docked in same time
            // we assume such tag as enabled, since in this case we can miss
            // an important logs messages
           
            /*
            if (!IsTagDocked(tag)) {
                if (!m_enabledTags.Contains(tag.Name)) {
                    m_enabledTags.Add(tag.Name);
                    return true;
                }

            }*/

            return m_enabledTags.Contains(tag.Name);
        }



    }

}