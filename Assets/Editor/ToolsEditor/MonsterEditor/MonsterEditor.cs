using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ToolsEditor
{
    public class MonsterEditor : OdinEditorWindow
    {
        [MenuItem("Setting/MonsterEditor")]
        public static void Open()
        {
            MonsterEditor window = GetWindow<MonsterEditor>();
            window.Show();
        }
        
        public static Dictionary<string, bool> fieldBool = new Dictionary<string, bool>();
        
        
        [PropertyTooltip("分组")]
        [LabelText("分组")]
        [PropertyOrder(-2)]
        [HorizontalGroup("ConfigButton",Order = -1,Width = 0.2f,LabelWidth = 40)] 
        public string group;
        
        [HideLabel]
        [Button("加载")] [HorizontalGroup("ConfigButton",Order = -1)] 
        private void Load()
        {
            
        }
        [Button("保存")][HorizontalGroup("ConfigButton",Order = -1)] 
        private void Save()
        {
            
        }
        [Button("配置")][HorizontalGroup("ConfigButton",Order = -1)] 
        private void Config()
        {
            ConfigEditor.Open(fieldBool,typeof(MonsterEditorData));
        }
        
        [ListDrawerSettings(DraggableItems = false)][HideLabel]
        public List<MonsterEditorData> monsterData = new List<MonsterEditorData>();
        protected override void OnEnable()
        {
            base.OnEnable();
            fieldBool = ConfigEditor.InitField(typeof(MonsterEditorData));
        }

        private void OnDisable()
        {
            if (ConfigEditor.window != null)
                ConfigEditor.window.Close();
        }
    }
}