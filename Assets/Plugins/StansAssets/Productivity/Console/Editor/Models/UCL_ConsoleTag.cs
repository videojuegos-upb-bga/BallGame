using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Productivity.Console
{

    [Serializable]
    public class UCL_ConsoleTag
    {

        public string Name;
        public Texture2D Icon;

        public GUIContent DisaplyContent {

            get {
                GUIContent content = new GUIContent();
                if (Icon != null) {
                    content.image = Icon;
                }

                content.text = Name;
                return content;
            }
        }
    }
}