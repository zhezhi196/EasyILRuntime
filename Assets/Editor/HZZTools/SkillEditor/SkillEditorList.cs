using System;
using System.Collections.Generic;
using System.Reflection;
using Module;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EditorModule
{
    [Serializable]
    public class SkillEditorList
    {
        [HideInInspector]
        public string targetName;

        [ListDrawerSettings(Expanded = true,HideAddButton = true,HideRemoveButton = true,DraggableItems = false), LabelText("@targetName")]
        public List<SkillEditorStuct> skillList = new List<SkillEditorStuct>();

        public SkillEditorList Init(string targetName, string skillName,Skill skill)
        {
            this.targetName = targetName;
            AddSkill(skillName,skill);
            return this;
        }

        public void AddSkill(string attSkillName, Skill skill)
        {
            SkillEditorStuct str = new SkillEditorStuct().Init(skill, attSkillName);
            skillList.Add(str);
        }
    }
}