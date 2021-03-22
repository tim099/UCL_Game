using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCL.GameLib {
    [CreateAssetMenu(fileName = "New GameSetting", menuName = "UCL/GameSetting")]
    public class UCL_GameSetting : ScriptableObject {
        static public UCL_GameSetting Get() {
            return Resources.Load<UCL_GameSetting>("GameSetting");
        }
    }
}