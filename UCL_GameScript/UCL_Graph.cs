using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCL.GameLib
{

    public class UCL_Graph
    {
        #region Node & Path
        public interface INode
        {
            Path GetPath(Node next_node);
            Dictionary<Node, Path> GetPaths();
        }
        public enum PathDir
        {
            BothSide = 0,
            AtoB,
            BtoA,
        }
        public interface IPath
        {
            INode GetA();
            INode GetB();
            INode GetNext(INode start);
            PathDir GetDir();
        }

        public class Path : IPath
        {
            public Path(Node _A, Node _B, PathDir _Dir = PathDir.BothSide) {
                m_A = _A;
                m_B = _B;
                m_Dir = _Dir;
            }
            public INode GetA() { return m_A; }
            public INode GetB() { return m_B; }
            public INode GetNext(INode start) {
                if(start == m_A) return m_B;
                return m_A;
            }
            public PathDir GetDir() {
                return m_Dir;
            }
            public PathDir m_Dir = PathDir.BothSide;
            public Node m_A;
            public Node m_B;
        }
        public class Node : INode
        {
            public Node(UCL_Graph _Graph) {
                p_Graph = _Graph;
            }

            public void RemovePathOut(Node end_node) {
                if(m_PathOut.ContainsKey(end_node)) {
                    var path = m_PathOut[end_node];
                    if(path.m_Dir == PathDir.BothSide) {
                        if(path.m_A == this) {
                            path.m_Dir = PathDir.BtoA;
                        } else {
                            path.m_Dir = PathDir.AtoB;
                        }
                    } else {
                        p_Graph.RemovePath(path);
                    }
                    m_PathOut.Remove(end_node);
                }
            }
            public void RemovePathIn(Node start_node) {
                if(m_PathIn.ContainsKey(start_node)) {
                    var path = m_PathIn[start_node];
                    if(path.m_Dir == PathDir.BothSide) {
                        if(path.m_B == this) {
                            path.m_Dir = PathDir.BtoA;
                        } else {
                            path.m_Dir = PathDir.AtoB;
                        }
                    } else {
                        p_Graph.RemovePath(path);
                    }
                    m_PathIn.Remove(start_node);
                }
            }
            public void AddPathOut(Node end_node,Path path) {
                m_PathOut.Add(end_node, path);
            }
            public void AddPathIn(Node start_node, Path path) {
                m_PathIn.Add(start_node, path);
            }
            public void AddPath(Node node, Path path) {
                m_PathIn.Add(node, path);
                m_PathOut.Add(node, path);
            }
            public void ClearAllPath() {
                foreach(var pair in m_PathOut) {
                    pair.Key.RemovePathIn(this);
                }
                foreach(var pair in m_PathIn) {
                    pair.Key.RemovePathOut(this);
                }
                m_PathIn.Clear();
                m_PathOut.Clear();
            }
            public Path GetPath(Node next_node) {
                if(m_PathOut.ContainsKey(next_node))return m_PathOut[next_node];
                return null;
            }
            public Dictionary<Node, Path> GetPaths() {
                return m_PathOut;
            }
            /// <summary>
            /// Paths that start from this Node, and the end Node is the key
            /// </summary>
            public Dictionary<Node, Path> m_PathOut;
            /// <summary>
            /// Paths that end at this Node, and the start Node is the key
            /// </summary>
            public Dictionary<Node, Path> m_PathIn;

            protected UCL_Graph p_Graph;
        }
        #endregion


        public Node CreateNode() {
            Node node = new Node(this);
            m_Nodes.Add(node);
            return node;
        }
        public void DeleteNode(Node node) {
            node.ClearAllPath();
            m_Nodes.Remove(node);
        }
        /// <summary>
        /// Create a both side path(a to b and b to a)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void CreatePath(Node a, Node b) {
            Path path = a.GetPath(b);
            if(path == null) path = b.GetPath(a);
            if(path != null) {//Path between a and b already exist!!
                switch(path.GetDir()) {
                    case PathDir.AtoB: {
                            a.AddPathIn(b, path);
                            b.AddPathOut(a, path);
                            break;
                        }
                    case PathDir.BtoA: {
                            a.AddPathOut(b, path);
                            b.AddPathIn(a, path);
                            break;
                        }
                }
                path.m_Dir = PathDir.BothSide;
                return;//Don't need to create new path so just return
            }

            path = new Path(a, b);
            a.AddPath(b, path);
            b.AddPath(a, path);
            m_Paths.Add(path);
        }

        /// <summary>
        /// Create a one way path(a to b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public void CreateDirectionalPath(Node a, Node b) {
            if(a.GetPath(b) != null) return;//Path from a and b already exist!!
            Path path = b.GetPath(a);
            if(path != null) {//Path between a and b already exist!!
                if(path.GetDir() == PathDir.BtoA) {
                    a.AddPathOut(b, path);
                    b.AddPathIn(a, path);
                    path.m_Dir = PathDir.BothSide;
                }
                return;//Don't need to create new path so just return
            }
            path = new Path(a, b, PathDir.AtoB);
            a.AddPathOut(b, path);
            b.AddPathIn(a, path);
            m_Paths.Add(path);
        }

        internal void RemovePath(Path path) {
            m_Paths.Remove(path);
        }
        public List<Node> m_Nodes;
        public List<Path> m_Paths;
    }
}

