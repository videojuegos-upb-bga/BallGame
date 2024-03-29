﻿using UnityEngine;

using SA.Foundation.Config;
using SA.Foundation.UtilitiesEditor;


namespace SA.Foundation.Patterns
{

    /// <summary>
    /// This class simplifies a singleton pattern implementation,
    /// that can be used with classes extended from a ScriptableObject
    /// Once instance is found or created gameobject will be marked as DontDestroyOnLoad
    /// </summary>
    public abstract class SA_ScriptableSingletonEditor<T> : SA_ScriptableSettings where T : ScriptableObject
    {

        private static T s_instance = null;


        /// <summary>
        /// Returns a singleton class instance
        /// If current instance is not assigned it will try to find an object of the instance type,
        /// in case instance already exists in a project. If not, new instance will be created, 
        /// and saved under a <see cref="SA_Config.STANS_ASSETS_EDITOR_SETTINGS_RESOURCES_PATH"/>  location
        /// </summary>
        public static T Instance {
            get {

                if (s_instance == null) {
                    s_instance = Resources.Load(typeof(T).Name) as T;
                    if (s_instance == null) {
                        s_instance = CreateInstance<T>();
                        SaveToAssetDatabase(s_instance);
                    }
                }
                return s_instance;
            }
        }
       
        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Saves instance to an editor database
        /// </summary>
        public static void Save() {
            /*
            UnityEditor.Undo.IncrementCurrentGroup();
            UnityEditor.Undo.RegisterCompleteObjectUndo(Instance, typeof(T).Name);
            UnityEditor.Undo.CollapseUndoOperations(UnityEditor.Undo.GetCurrentGroup());
            */
            SA_EditorUtility.SetDirty(Instance);
        }


        /// <summary>
        /// Delete's asset instance
        /// </summary>
        public static void Delete() {
            string path = SA_AssetDatabase.GetAssetPath(Instance);
            SA_AssetDatabase.DeleteAsset(path);
        }


        private static void SaveToAssetDatabase(T asset) {
            SA_AssetDatabase.CreateAsset(asset, SA_Config.STANS_ASSETS_EDITOR_SETTINGS_RESOURCES_PATH + asset.GetType().Name + ".asset");
        }
    }
}