using System.Collections.Generic;
using System.IO;
using LitJson;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ModuleEditor
{
    public class SkillEditor : OdinEditorWindow
    {
        [MenuItem("Setting/SkillEditor")]
        public static void OpenWindow()
        {
            SkillEditor window = GetWindow<SkillEditor>();
            window.Show();
            window.minSize = new Vector2(600, 800);
        }

        private static string skillConfig = $"{Application.dataPath}/Bundles/Config/Skill/Skill.json";

        [HideLabel] 
        public List<SkillEditorData> skillData = new List<SkillEditorData>();
        
        protected override void OnEnable()
        {
            base.OnEnable();
            if (File.Exists(skillConfig))
            {
                using (StreamReader reader = new StreamReader(skillConfig))
                {
                    skillData = JsonMapper.ToObject<List<SkillEditorData>>(reader.ReadToEnd());
                }
            }
        }

        [Button("生成", ButtonSizes.Medium)]
        public void CreatSkillData()
        {
            using (StreamWriter writer = new StreamWriter(skillConfig))
            {
                writer.Write(JsonMapper.ToJson(skillData));
            }
            AssetDatabase.Refresh();
        }

        protected override void OnGUI()
        {
            base.OnGUI();
            if (!skillData.IsNullOrEmpty())
            {
                for (int i = 0; i < skillData.Count; i++)
                {
                    if (!skillData[i].timeLine.IsNullOrEmpty() && skillData[i].timeLine.Count > 1)
                    {
                        skillData[i].timeLine.Sort(((node, timeNode) => node.timePercent.CompareTo(timeNode.timePercent)));
                        for (int j = 0; j < skillData[i].timeLine.Count; j++)
                        {
                            skillData[i].timeLine[j].index = j + 1;
                        }
                    }
                }
            }
        }
    }
}