using System;
using System.Collections.Generic;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class EditorConsole : OdinEditorWindow
    {
        [MenuItem("Tools/程序工具/打开EditorConsole #F12")]
        public static void OpenConsole()
        {
            GetWindow<EditorConsole>();
        }

        [LabelText("UI队列"),ReadOnly] public List<string> uiPool;
        [LabelText("冻结的队列"),ReadOnly] public List<object> freezeList;
        [LabelText("loading的队列"),ReadOnly]
        public List<string> loadingKey;
        [LabelText("暂停队列")]
        public List<string> pauseKey;

        private void OnInspectorUpdate()
        {
            if (uiPool == null) uiPool = new List<string>();
            uiPool.Clear();
            for (int i = 0; i < UIController.Instance.winList.Count; i++)
            {
                uiPool.Add(UIController.Instance.winList[i].ToString());
            }

            freezeList = UICommpont.GetFreezeList();
            pauseKey = BattleController.Instance.pauseList;
        }
    }
}