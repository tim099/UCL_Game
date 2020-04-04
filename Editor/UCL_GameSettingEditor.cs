using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace UCL.GameLib {
    [CustomEditor(typeof(UCL_GameSetting), true)]
    public class UCL_GameSettingEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var controller = (UCL_GameSetting)target;
        }
    }
}