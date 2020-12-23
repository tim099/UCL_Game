﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCL.GameLib
{
    public class UCL_GraphNode : MonoBehaviour
    {
        public UCL_GraphMap p_Map;
        virtual public void Init(UCL_GraphMap _Map) {
            p_Map = _Map;
            gameObject.SetActive(true);
        }
        public List<UCL_GraphPath> m_Paths;
        public void AddPath(UCL_GraphPath path) {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.Undo.RecordObject(this, "Node AddPath");
                m_Paths.Add(path);
                return;
            }
#endif
            m_Paths.Add(path);
        }
        public void RemovePath(UCL_GraphPath path) {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.Undo.RecordObject(this, "Node RemovePath");
                m_Paths.Remove(path);
                return;
            }
#endif
            m_Paths.Remove(path);
        }
        public UCL_GraphPath GetPath(UCL_GraphNode node) {
            for(int i = 0; i < m_Paths.Count; i++) {
                var path = m_Paths[i];
                if(path.GetNext(this) == node) return path;
            }
            return null;
        }
        public void UpdatePostion(Vector3 pos) {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(transform, "Node UpdatePostion");
#endif
            transform.position = pos;
            for(int i = 0; i < m_Paths.Count; i++) {
                var path = m_Paths[i];
                path.UpdatePathPosition();
            }
        }
        internal void Delete() {
            var list = new List<UCL_GraphPath>();
            for(int i = 0; i < m_Paths.Count; i++) {
                list.Add(m_Paths[i]);
            }
            foreach(var path in list) {
                if(path != null) p_Map.DeletePath(path);
            }
        }
    }
}