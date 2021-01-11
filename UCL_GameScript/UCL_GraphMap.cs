using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UCL.GameLib
{
    public class UCL_GraphMap : MonoBehaviour
    {
        [HideInInspector] public int m_SelectedNodeType = 0;
        public List<UCL_GraphNode> m_NodeTemplates;
        public List<UCL_GraphNode> m_Nodes;
        public List<UCL_GraphPath> m_Paths;
        public UCL_GraphPath m_PathTemplate;

        public RectTransform m_NodesRoot;
        public RectTransform m_PathsRoot;
        public static UCL_GraphMap Create(Transform parent) {
            var map = UCL.Core.GameObjectLib.Create<UCL_GraphMap>(parent);
            var rect = map.GetComponent<RectTransform>();
            if(rect != null) {
                rect.SetFullScreen();
                var image = UCL.Core.GameObjectLib.Create<Image>("Panel", rect);
                rect = image.GetComponent<RectTransform>();
                rect.SetFullScreen();
#if UNITY_EDITOR
                map.m_PathsRoot = UCL.Core.GameObjectLib.CreateGameObject("Paths", rect).GetComponent<RectTransform>();
                map.m_PathsRoot.SetFullScreen();
                map.m_NodesRoot = UCL.Core.GameObjectLib.CreateGameObject("Nodes", rect).GetComponent<RectTransform>();
                map.m_NodesRoot.SetFullScreen();

                map.m_NodeTemplates = new List<UCL_GraphNode>();

                image.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                image.type = Image.Type.Sliced;

                var go = UCL.Core.GameObjectLib.CreateGameObject("NodeTemplates", rect);
                rect = go.GetComponent<RectTransform>();

                var tmp = UCL.Core.GameObjectLib.Create<Image>("NodeBasic", rect);
                tmp.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                tmp.type = Image.Type.Simple;
                tmp.gameObject.SetActive(false);
                map.m_NodeTemplates.Add(tmp.gameObject.AddComponent<UCL_GraphNode>());

                tmp = UCL.Core.GameObjectLib.Create<Image>("NodeKnob", rect);
                tmp.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
                tmp.type = Image.Type.Simple;
                tmp.gameObject.SetActive(false);
                map.m_NodeTemplates.Add(tmp.gameObject.AddComponent<UCL_GraphNode>());

                tmp = UCL.Core.GameObjectLib.Create<Image>("Path", map.m_PathsRoot);
                tmp.sprite = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                tmp.type = Image.Type.Sliced;
                tmp.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 20);
                tmp.gameObject.SetActive(false);
                map.m_PathTemplate = tmp.gameObject.AddComponent<UCL_GraphPath>();
#endif
                if(rect != null) rect.SetFullScreen();
            }
            return map;
        }
        internal void RemovePath(UCL_GraphPath path) {
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.Undo.RecordObject(this, "Map RemovePath");
                m_Paths.Remove(path);
                return;
            }
#endif
            m_Paths.Remove(path);
        }
        public void DeletePath(UCL_GraphPath path) {
            path.Delete();
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying) {                
                UnityEditor.Undo.DestroyObjectImmediate(path.gameObject);
                return;
            }
#endif
            Destroy(path.gameObject);
        }
        internal void RemoveNode(UCL_GraphNode node) {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "RemoveNode");
#endif
            m_Nodes.Remove(node);
        }
        public void DeleteNode(UCL_GraphNode node) {
            node.Delete();
            RemoveNode(node);
#if UNITY_EDITOR
            if(!UnityEditor.EditorApplication.isPlaying) {
                UnityEditor.Undo.DestroyObjectImmediate(node.gameObject);
                return;
            }
#endif
            Destroy(node.gameObject);
        }
        public UCL_GraphNode CreateNode(int type) {
            if(type < 0 || type >= m_NodeTemplates.Count) {
                return null;
            }

            var tmp = m_NodeTemplates[type];
            if(tmp == null) {
                return null;
            }
            UCL_GraphNode node = null;
#if UNITY_EDITOR
            node = UnityEditor.PrefabUtility.InstantiatePrefab(tmp, m_NodesRoot) as UCL_GraphNode;
#endif
            if(node == null) {
                node = Instantiate(tmp, m_NodesRoot);
            }
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(node.gameObject, "Create object");
#endif
            node.Init(this);
            
            node.name = tmp.name + "_" + (m_Nodes.Count + 1);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "m_Nodes.Add");
#endif
            m_Nodes.Add(node);
            return node;
        }
        public UCL_GraphPath CreatePath(UCL_GraphNode a,UCL_GraphNode b, UCL_GraphPath.Direction direction = UCL_GraphPath.Direction.BothSide) {
            var path = a.GetPath(b);
            if(path != null) return path;
#if UNITY_EDITOR
            path = UnityEditor.PrefabUtility.InstantiatePrefab(m_PathTemplate, m_PathsRoot) as UCL_GraphPath;
#else
            path = Instantiate(m_PathTemplate, m_PathsRoot);
#endif

#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(path.gameObject, "Create object");
#endif
            path.Init(this, a, b, direction);
            a.AddPath(path);
            b.AddPath(path);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "m_Paths.Add");
#endif
            m_Paths.Add(path);
            return path;
        }
    }
}