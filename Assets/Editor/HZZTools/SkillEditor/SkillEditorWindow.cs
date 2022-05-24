using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class SkillEditorWindow: OdinEditorWindow
    {
        [UnityEditor.MenuItem("Tools/策划工具/关卡编辑/技能编辑 _F6")]
        private static void Open()
        {
            GetWindow<SkillEditorWindow>();
        }
        [HideInInspector]
        public int index;
        [HideInInspector]
        public Skill skillShow;

        protected override void OnEnable()
        {
            base.OnEnable();
            readDictory = $"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/Agent/Skill";
        }

        private void OnDisable()
        {
            for (int i = 0; i < readSkill.Count; i++)
            {
                for (int j = 0; j < readSkill[i].skillList.Count; j++)
                {
                    EditorUtility.SetDirty(readSkill[i].skillList[j].skill);
                }
            }
            AssetDatabase.Refresh();
        }

        [LabelText("文件夹路径")][ShowIf("@index==0")]
        public string readDictory;
        [Button("读取")][ShowIf("@index==0")]
        private void Read()
        {
            DirectoryInfo dir = new DirectoryInfo(readDictory);
            FileInfo[] fileInfo = dir.GetFiles("*.asset");
            readSkill.Clear();
            for (int i = 0; i < fileInfo.Length; i++)
            {
                string path = Pathelper.GetReleativePath(fileInfo[i].FullName);
                Skill skill = AssetDatabase.LoadAssetAtPath<Skill>(path);
                SkillDescriptAttribute att = GetDes(skill);
                if (readSkill.Contains(st => st.targetName == att.targetName))
                {
                    for (int j = 0; j < readSkill.Count; j++)
                    {
                        if (readSkill[j].targetName == att.targetName)
                        {
                            readSkill[j].AddSkill(att.skillName, skill);
                        }
                    }
                }
                else
                {
                    readSkill.Add(new SkillEditorList().Init(att.targetName, att.skillName, skill));
                }
            }

            index = 1;
        }
        
        
        
        [ShowIf("@index==1")][ListDrawerSettings(Expanded = true,HideAddButton = true,ShowItemCount = false,IsReadOnly = true,DraggableItems = false,NumberOfItemsPerPage = 7)] 
        public List<SkillEditorList> readSkill = new List<SkillEditorList>();
        
        [ShowIf("@index>0")] 
        [Button("返回")]
        public void Back()
        {
            index--;
        }
        [ShowIf("@index>0")] 
        [Button("生成脚本")]
        public void CreatName()
        {
            using (StreamWriter write=new StreamWriter($"{Application.dataPath}/GameProject/Monster/SkillName.cs"))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("public enum SkillName\n");
                builder.Append("{\n");
                for (int i = 0; i < readSkill.Count; i++)
                {
                    for (int j = 0; j < readSkill[i].skillList.Count; j++)
                    {
                        builder.Append(readSkill[i].skillList[j].skill.name);
                        builder.Append(",\n");
                    }
                }
                builder.Append("}\n");
                write.Write(builder);
                AssetDatabase.Refresh();
            }
        }

        private SkillDescriptAttribute GetDes(Skill skill)
        {
            SkillDescriptAttribute att =
                skill.GetType().GetCustomAttribute(typeof(SkillDescriptAttribute), true) as SkillDescriptAttribute;
            if (att == null) att = new SkillDescriptAttribute($"未知/{skill.GetType().Name}");
            return att;
        }
    }
}