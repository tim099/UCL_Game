using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCL.GameLib.EditorLib
{
    public class UCL_GameMenuItem
    {
        [UnityEditor.MenuItem("GameObject/UI/UCL/GraphMap")]
        private static void CreateUCL_GraphMap() {
            Object selectedObject = UnityEditor.Selection.activeObject;
            GameObject obj = selectedObject as GameObject;
            Transform p = null;
            if(obj != null) {
                p = obj.transform;
            }
            var map = UCL_GraphMap.Create(p);
            UnityEditor.Selection.activeObject = map;
        }
        [UnityEditor.MenuItem("UCL/GameLib/GameSetting")]
        static public void OpenGameSetting()
        {
            UnityEditor.Selection.activeObject = UCL_GameSetting.Get();
        }
    }
}