using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace SA.Productivity.Console
{
    public class UCL_TreeView : TreeView
    {
        private Rect m_lastDrawRect = new Rect();
        private List<UCL_LogInfo> m_logs =  new List<UCL_LogInfo>();

        private GUIStyle defaultLineStyle;
        private GUIStyle selecredLineStyle;
        private GUIStyle CountBadge;


        private const int ROW_HEIGHT = 20;

       
        //--------------------------------------
        // Initialization
        //--------------------------------------

        public UCL_TreeView(TreeViewState treeViewState) : base(treeViewState) {
            showAlternatingRowBackgrounds = true;
            Reload();
        }


        public void Build(List<UCL_LogInfo> logs) {
            RefreshStyles();
            bool autoScroll = IsScrolledToTheEnd; 

            m_logs = logs;
            Reload();

            if(autoScroll) {
                float bottomScrollPos = ContentHeight - m_lastDrawRect.height;
                state.scrollPos.y = bottomScrollPos;
            }
        }

        public void RefreshStyles() {
            defaultLineStyle = null;
            selecredLineStyle = null;
        }


        private void InitStylesIfNeeded() {
            if (defaultLineStyle != null) { return; }

            defaultLineStyle = new GUIStyle(DefaultStyles.label);
            defaultLineStyle.richText = UCL_Settings.ConsoleWindowState.RichText;

            selecredLineStyle = new GUIStyle(defaultLineStyle);
            selecredLineStyle.normal.textColor = Color.white;

            CountBadge = new GUIStyle() {
                margin = new RectOffset(0, 0, 0, 0),
                border = new RectOffset(5, 5, 5, 5),
                padding = new RectOffset(5, 5, 2, 2),
                fontSize = 9
            };

            if (EditorGUIUtility.isProSkin) {
                CountBadge.normal.textColor = Color.white;
                CountBadge.normal.background = Resources.Load("console/badge_bg_pro") as Texture2D;
            } else {
                CountBadge.normal.background = Resources.Load("console/badge_bg") as Texture2D;
            }
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        public List<UCL_LogInfo> Logs {
            get {
                return m_logs;
            }
        }

        public List<UCL_LogInfo> SelectedLogs {
            get {
                List<UCL_LogInfo> result = new List<UCL_LogInfo>();
               

                var rows = FindRows(GetSelection());
                foreach(var row in rows) {
                    result.Add((row as UCL_TreeViewItem).Log);
                }

                return result;
            }
        }
        

        public float ContentHeight {
            get {
                return m_logs.Count * ROW_HEIGHT;
            }
        }

        private bool IsScrolledToTheEnd {

            get {
                if (ContentHeight < m_lastDrawRect.height) {
                    return true;
                } else {
                    float bottomScrollPos = ContentHeight - m_lastDrawRect.height;
                    if (Mathf.Approximately(state.scrollPos.y, bottomScrollPos)) {
                        return true;
                    }
                }

                return false;
            }
        }


        //--------------------------------------
        // GUI
        //--------------------------------------


      
        public override void OnGUI(Rect rect) {
            m_lastDrawRect = rect;
            base.OnGUI(rect);
        }


        protected override float GetCustomRowHeight(int row, TreeViewItem item) {
            return ROW_HEIGHT;
        }

        // protected virtual bool DoesItemMatchSearch(TreeViewItem item, string search);

        protected override TreeViewItem BuildRoot() {
           
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            var allItems = new List<TreeViewItem>();
            foreach(var log in m_logs) {
                var item = new UCL_TreeViewItem(log);
                allItems.Add(item);
            }

            // Utility method that initializes the TreeViewItem.children and -parent for all items.
            SetupParentsAndChildrenFromDepths(root, allItems);

            // Return root of the tree
            return root;
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = (UCL_TreeViewItem) args.item;

            var logLine = item.Log;

            InitStylesIfNeeded();
            var style = defaultLineStyle;
            if(args.selected) {
                style = selecredLineStyle;
            }


            GUI.Label(args.rowRect, logLine.LineGUIContent, style);

            if (UCL_LogEntries.HasFlag(UCL_ConsoleFlags.Collapse)) {
               
                var badgeContent = new GUIContent(logLine.EntryCount.ToString());
                var size = CountBadge.CalcSize(badgeContent);


                Rect badgeRect = new Rect();
                badgeRect.width = size.x;
                badgeRect.height = size.y;


                badgeRect.x = args.rowRect.x + args.rowRect.width;
                badgeRect.x -= badgeRect.width;
                badgeRect.x -= 5f;
                badgeRect.y = args.rowRect.y + (args.rowRect.height - badgeRect.height) / 2f;
                GUI.Label(badgeRect, badgeContent, CountBadge);
            }
        }




        //--------------------------------------
        // Handling Input
        //--------------------------------------

        protected override void SelectionChanged(IList<int> selectedIds) {
            if(IsScrolledToTheEnd) {
                state.scrollPos.y -= 1;
            }

            if(selectedIds.Count == 1) {
                var item = (UCL_TreeViewItem)FindItem(selectedIds[0], rootItem);
                EditorGUIUtility.PingObject(item.Log.InstanceID);
            }

           
        }

        protected override void DoubleClickedItem(int id) {
            var item = (UCL_TreeViewItem) FindItem(id, rootItem);
            UCL_ConsoleUtil.JumpToSource(item.Log);
        }


        protected override void ContextClickedItem(int id) {
            var item = (UCL_TreeViewItem)FindItem(id, rootItem);
            UCL_ConsoleUtil.ShowLogLineMenu(item.Log);
        }


        protected override bool CanMultiSelect(TreeViewItem item) {
            return true;
        }
    }


}