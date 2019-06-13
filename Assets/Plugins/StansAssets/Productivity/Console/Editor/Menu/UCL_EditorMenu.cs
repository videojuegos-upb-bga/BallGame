using System;
using UnityEditor;
using UnityEngine;
using SA.Foundation.Config;

namespace SA.Productivity.Console
{
    public static class UCL_EditorMenu
    {

        private const int MENU_INDEX = SA_Config.PRODUCTIVITY_MENU_INDEX + 10;


        [MenuItem(SA_Config.EDITOR_PRODUCTIVITY_MENU_ROOT + "Console/Show", false, MENU_INDEX)]
        public static void ShowConsole() {
            Type inspectorType = Type.GetType("UnityEditor.ConsoleWindow, UnityEditor.dll");
            var window = EditorWindow.GetWindow<UCL_ConsoleWindow>(new Type[] { inspectorType });
            window.Show();
        }

        public static void CloseConsole() {
            var window = EditorWindow.GetWindow<UCL_ConsoleWindow>();
            window.Close();
        }


        [MenuItem(SA_Config.EDITOR_PRODUCTIVITY_MENU_ROOT + "Console/Settings", false, MENU_INDEX)]
        public static void ShowSettings() {
            UCL_ConsolePreferencesWindow.ShowModal();
        }


        [MenuItem(SA_Config.EDITOR_PRODUCTIVITY_MENU_ROOT + "Console/Documentation", false, MENU_INDEX)]
        public static void Documentation() {
            Application.OpenURL(UCL_PlatfromsLogSettings.DOCUMENTATION_URL);
        }
    }
}