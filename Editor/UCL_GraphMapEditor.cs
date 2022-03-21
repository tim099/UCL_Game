using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UCL.Core;

namespace UCL.GameLib
{
    [CustomEditor(typeof(UCL_GraphMap))]
    public class UCL_GraphMapEditor : Core.EditorLib.UCL_MonobehaviorEditor
    {
        enum EditMode
        {
            Normal = 0,
            AddPath,
        }
        const int window_width = 400;
        Rect m_WindowRect = new Rect(0, 30, window_width, 60);
        Vector3[] m_Corners = new Vector3[4];
        Vector2 m_ScrollPos = Vector2.zero;
        string[] m_Options;
        UCL_GraphNode m_SelectedNode = null;
        EditMode m_EditMode = EditMode.Normal;
        UCL_GraphPath.Direction m_AddPathDirection = UCL_GraphPath.Direction.BothSide;
        void OnEnable() {


        }
        void OnSceneGUI() {
            bool map_updated = false;
            var map = target as UCL_GraphMap;
            var rect = map.GetComponent<RectTransform>();
            rect.GetWorldCorners(m_Corners);
            //EditorGUILayout.DropdownButton
            if(m_Options == null || m_Options.Length != map.m_NodeTemplates.Count) {
                m_Options = new string[map.m_NodeTemplates.Count];
            }
            for(int i=0;i< m_Options.Length; i++) {
                string node_type = "null";
                if(map.m_NodeTemplates[i] != null) {
                    node_type = map.m_NodeTemplates[i].name;
                }
                m_Options[i] = node_type;
            }
            if(map.m_SelectedNodeType >= map.m_NodeTemplates.Count) map.m_SelectedNodeType = map.m_NodeTemplates.Count - 1;
            if(map.m_SelectedNodeType < 0) map.m_SelectedNodeType = 0;

            if(rect) {
                Color but_col = new Color(1, 0, 0, 0.8f);
                Vector2 but_offset = new Vector2(0, 25);
                //int delete_node = -1;
                int delete_path = -1;
                if(map.m_Nodes == null) {
                    map.m_Nodes = new List<UCL_GraphNode>();
                }
                if(map.m_Paths == null) {
                    map.m_Paths = new List<UCL_GraphPath>();
                }
                for(int i = 0; i < map.m_Nodes.Count; i++) {
                    var node = map.m_Nodes[i];
                    int at = i;
                    if(node != null) {
                        const int w = 130, h = 120;
                        //UCL_DrawGizmos.DrawPopup(node.transform.position, new Vector2(70, 22), 0, m_Options);
                        if(m_SelectedNode == node) {
                            var pos = UCL_DrawGizmos.WorldPosToGUI(node.transform.position);
                            GUILayout.Window(123425, new Rect(pos.x-w-20, pos.y-h/2, w, h), delegate (int id) {
                                switch(m_EditMode) {
                                    case EditMode.Normal: {
                                            if(GUILayout.Button("Deselect Node")) {
                                                m_EditMode = EditMode.Normal;
                                                m_SelectedNode = null;
                                            }
                                            if(GUILayout.Button("Add Path")) {
                                                m_EditMode = EditMode.AddPath;
                                                m_AddPathDirection = UCL_GraphPath.Direction.BothSide;
                                            }
                                            if(GUILayout.Button("Add OneWay Path")) {
                                                m_EditMode = EditMode.AddPath;
                                                m_AddPathDirection = UCL_GraphPath.Direction.AtoB;
                                            }
                                            if(GUILayout.Button("Delete Node")) {
                                                //Debug.LogError("Delete Node at:" + at);
                                                map.DeleteNode(node);
                                            }
                                            break;
                                        }
                                    case EditMode.AddPath: {
                                            if(GUILayout.Button("Cancel")) {
                                                m_EditMode = EditMode.Normal;
                                            }
                                            break;
                                        }
                                }

                            }, node.name);
                            var next_pos = Handles.PositionHandle(node.transform.position, Quaternion.identity);
                            if(next_pos != node.transform.position) {
                                //Debug.LogWarning("node.transform.position:" + node.transform.position + ",next_pos:" + next_pos);
                                node.UpdatePostion(next_pos);
                                map_updated = true;
                            }
                        } else {
                            switch(m_EditMode) {
                                case EditMode.Normal: {
                                        if(UCL_DrawGizmos.DrawButtonGUI("Edit", node.transform.position, 28, new Vector2(50, 22),
                                            Color.green, Color.grey, Vector2.zero)) m_SelectedNode = node;
                                        break;
                                    }
                                case EditMode.AddPath: {
                                        if(m_SelectedNode.GetPath(node) == null) {
                                            if(UCL_DrawGizmos.DrawButtonGUI("Create Path", node.transform.position, 28, new Vector2(80, 22),
                                                Color.green, Color.grey, Vector2.zero)) {
                                                map.CreatePath(m_SelectedNode, node, m_AddPathDirection);
                                                m_EditMode = EditMode.Normal;
                                                m_SelectedNode = null;
                                            }
                                        }
                                        break;
                                    }
                            }

                        }
                    }
                }
                for(int i = 0; i < map.m_Paths.Count; i++) {
                    var path = map.m_Paths[i];
                    if(path != null) {
                        if(UCL_DrawGizmos.DrawButtonGUI("Delete", path.transform.position, 28, new Vector2(50, 22),
                            Color.red, but_col)) {
                            delete_path = i;
                        }
                    } else {
                        delete_path = i;
                    }

                }
                if(delete_path >= 0) {
                    var path = map.m_Paths[delete_path];
                    if(path != null) {
                        map.DeletePath(path);
                    } else {
                        map.m_Paths.RemoveAt(delete_path);
                    }
                    
                }


                //if(UCL_DrawGizmos.DrawButtonGUI("Delete", m_Corners[0], 28, new Vector2(50, 22), Color.red, but_col)) {

                //}
            }
            
            UnityEditor.Handles.BeginGUI();
            m_WindowRect = GUILayout.Window(122425, m_WindowRect, delegate (int id) {
                m_ScrollPos = GUILayout.BeginScrollView(m_ScrollPos, GUILayout.Width(window_width));
                GUILayout.BeginVertical();
                //GUILayout.Box("UCL_GraphMap Editor");
                GUILayout.BeginHorizontal();
                map.m_SelectedNodeType = EditorGUILayout.Popup("NodeType", map.m_SelectedNodeType, m_Options);
                if(GUILayout.Button("Create")) {
                    int aCount = map.m_Nodes.Count;
                    int x = aCount % 8;
                    int y = ((aCount - x) / 8);

                    var aNode = map.CreateNode(map.m_SelectedNodeType);

                    var aDel = m_Corners[2] - m_Corners[0];
                    aNode.transform.position = m_Corners[0] + new Vector3((aDel.x*(x+1))/9, (aDel.y * (y+1)) / 9,0);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
            },
            "UCL_GraphMap Editor");
            //GUILayout.BeginArea(new Rect(0, 0, width, 200));

            //GUILayout.EndArea();
            UnityEditor.Handles.EndGUI();

            if(map_updated) UnityEditor.EditorUtility.SetDirty(map);
        }
    }
}