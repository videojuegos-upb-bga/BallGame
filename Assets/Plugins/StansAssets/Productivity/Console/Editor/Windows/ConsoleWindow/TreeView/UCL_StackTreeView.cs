using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace SA.Productivity.Console
{
    public class UCL_StackTreeView : TreeView
    {

        private UCL_LogInfo m_log;
        private const int ROW_HEIGHT = 20;


        private GUIStyle defaultLineStyle;
        private GUIStyle selecredLineStyle;
        private GUIStyle disabledLineStyle;


        //--------------------------------------
        // Initialization
        //--------------------------------------

        public UCL_StackTreeView(TreeViewState treeViewState) : base(treeViewState) {
           // showAlternatingRowBackgrounds = true;
            Reload();
        }


        public void Build(UCL_LogInfo log) {
            m_log = log;
            Reload();
        }

        protected override TreeViewItem BuildRoot() {

            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var allItems = new List<TreeViewItem>();

            if(m_log != null) {
                foreach (var stackLine in m_log.Stack) {
                    var item = new UCL_StackTreeViewItem(m_log.Stack.IndexOf(stackLine) + 1, stackLine);
                    allItems.Add(item);
                }
            }

            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);

            // Return root of the tree
            return root;
        }


        private void InitStylesIfNeeded() {
            if (defaultLineStyle != null) { return; }

            defaultLineStyle = new GUIStyle(DefaultStyles.label);
           // defaultLineStyle.wordWrap = true;

            selecredLineStyle = new GUIStyle(defaultLineStyle);
            selecredLineStyle.normal.textColor = Color.white;

            disabledLineStyle = new GUIStyle(defaultLineStyle);
            disabledLineStyle.normal.textColor = Color.gray;
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------


        //--------------------------------------
        // GUI
        //--------------------------------------

        /*
        protected override float GetCustomRowHeight(int row, TreeViewItem item) {
            InitStylesIfNeeded();
            Vector2 size = defaultLineStyle.CalcSize(new GUIContent(item.displayName));
            return 50;
        }*/


        protected override void RowGUI(RowGUIArgs args) {
            var item = (UCL_StackTreeViewItem)args.item;

            InitStylesIfNeeded();
            var style = defaultLineStyle;
            if (args.selected) {
                style = selecredLineStyle;
            }

            GUI.enabled = item.StackLine.HasValidFilePointer;
            GUI.Label(args.rowRect, item.DisplayContent, style);
            GUI.enabled = true;
        }




        protected override void SelectionChanged(IList<int> selectedIds) {
            if(selectedIds.Count == 1) {
                int id = selectedIds[0];
                var item = (UCL_StackTreeViewItem) FindItem(id, rootItem);
                if (!item.StackLine.HasValidFilePointer) {
                    state.selectedIDs.Clear();
                }
            }
        }

        //--------------------------------------
        // Handling Input
        //--------------------------------------



        protected override void DoubleClickedItem(int id) {
            var item = (UCL_StackTreeViewItem) FindItem(id, rootItem);
            UCL_ConsoleUtil.JumpToSource(item.StackLine);
        }


        protected override void ContextClickedItem(int id) {
            var item = (UCL_StackTreeViewItem)FindItem(id, rootItem);
            UCL_ConsoleUtil.ShowLogStackLineMenu(item.StackLine);
        }

    }


}