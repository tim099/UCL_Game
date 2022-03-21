using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCL.GameLib
{
    public class UCL_GraphPath : MonoBehaviour
    {
        public enum Direction
        {
            BothSide = 0,
            AtoB,
            BtoA,
        }

        public bool IsEnable { get; set; } = true;
        public UCL_GraphNode m_A;
        public UCL_GraphNode m_B;
        public RectTransform m_Rect;
        public Direction m_Direction = Direction.BothSide;
        /// <summary>
        /// Direction Object, will set active if the path direction is AtoB or BothSide direction
        /// </summary>
        public GameObject m_AtoBDirObj;
        /// <summary>
        /// Direction Object, will set active if the path direction is BtoA or BothSide direction
        /// </summary>
        public GameObject m_BtoADirObj;
        public UCL_GraphMap p_Map;
        virtual public void Init(UCL_GraphMap _Map, UCL_GraphNode _A, UCL_GraphNode _B, Direction _Direction) {
            gameObject.SetActive(true);
            p_Map = _Map;
            m_A = _A;
            m_B = _B;
            SetDirection(_Direction);
            name = m_A.name + "=>" + m_B.name;
            m_Rect = GetComponent<RectTransform>();
            UpdatePathPosition();
        }
        public void SetDirection(Direction dir) {
            m_Direction = dir;
            switch(m_Direction) {
                case Direction.AtoB: {
                        if(m_AtoBDirObj != null) m_AtoBDirObj.SetActive(true);
                        if(m_BtoADirObj != null) m_BtoADirObj.SetActive(false);
                        break;
                    }
                case Direction.BtoA: {
                        if(m_AtoBDirObj != null) m_AtoBDirObj.SetActive(false);
                        if(m_BtoADirObj != null) m_BtoADirObj.SetActive(true);
                        break;
                    }
                case Direction.BothSide: {
                        if(m_AtoBDirObj != null) m_AtoBDirObj.SetActive(true);
                        if(m_BtoADirObj != null) m_BtoADirObj.SetActive(true);
                        break;
                    }
            }
        }
        public void UpdatePathPosition() {
            if(m_Rect) {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(m_Rect, "UpdatePathPosition");
#endif
                m_Rect.SetBetweenTwoPoint(m_A.transform.position, m_B.transform.position);
            }
        }
        internal void Delete() {
            m_A.RemovePath(this);
            m_B.RemovePath(this);
            p_Map.RemovePath(this);
        }
        public bool CanMoveTo(UCL_GraphNode node) {
            switch(m_Direction) {
                case Direction.AtoB: {
                        if(node == m_B) return true;
                        break;
                    }
                case Direction.BtoA: {
                        if(node == m_A) return true;
                        break;
                    }
                case Direction.BothSide: {
                        if(node == m_A || node == m_B) return true;
                        break;
                    }
            }
            return false;
        }
        public UCL_GraphNode GetNext(UCL_GraphNode cur) {
            if(cur == m_A) return m_B;
            return m_A;
        }
    }
}