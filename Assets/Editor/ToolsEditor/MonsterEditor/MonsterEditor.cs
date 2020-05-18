using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LitJson;
using Module;
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
        public static string jsonPath = $"{Application.dataPath}/Bundles/Config/Monster/";
        
        [PropertyTooltip("分组")]
        [LabelText("分组")]
        [PropertyOrder(-2)]
        [HorizontalGroup("ConfigButton",Order = -1,Width = 0.2f,LabelWidth = 40)] 
        public string group;
        
        [HideLabel]
        [Button("加载")] [HorizontalGroup("ConfigButton",Order = -1)] 
        private void Load()
        {
            // if (File.Exists(GetfullPath()))
            // {
            //     using (StreamReader reader = new StreamReader(GetfullPath()))
            //     {
            //         monsterData.Clear();
            //         string json = reader.ReadToEnd();
            //         JsonData data = JsonMapper.ToObject(json);
            //         
            //         for (int i = 0; i < data.Count; i++)
            //         {
            //             MonsterEditorData d = JsonConvert.ToObject<MonsterEditorData>(data[i]);
            //             monsterData.Add(d);    
            //         }
            //     }
            //     
            // }
        }
        [Button("保存")][HorizontalGroup("ConfigButton",Order = -1)] 
        private void Save()
        {
            Debug.Log(JsonMapper.ToJson(monsterData));
            //Debug.Log(JsonMapper.ToObject(monsterData.ToJsonData()));
            
            // JsonData data = new JsonData();
            // for (int i = 0; i < monsterData.Count; i++)
            // {
            //     data.Add(JsonConvert.ToJson(monsterData[i],fieldBool));
            // }
            // using (StreamWriter wirter = new StreamWriter(GetfullPath()))
            // {
            //     wirter.Write(data.ToJson());
            // }
            // AssetDatabase.Refresh();
        }
        [Button("配置")][HorizontalGroup("ConfigButton",Order = -1)] 
        private void Config()
        {
            ConfigEditor.Open(fieldBool,typeof(MonsterEditorData));
        }

        private string GetfullPath()
        {
            return jsonPath + @group + ".json";
        }
        
        [ListDrawerSettings(DraggableItems = false)][HideLabel]
        public List<MonsterEditorData> monsterData = new List<MonsterEditorData>();
        protected override void OnEnable()
        {
            base.OnEnable();
            fieldBool = ConfigEditor.InitField(typeof(MonsterEditorData));
        }

        private void OnDestroy()
        {
            if (ConfigEditor.window != null)
                ConfigEditor.window.Close();
        }
    }
}