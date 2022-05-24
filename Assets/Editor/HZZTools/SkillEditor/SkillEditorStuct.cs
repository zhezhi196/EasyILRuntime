using System;
using System.Collections.Generic;
using System.Reflection;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    [Serializable]
    public class SkillEditorStuct
    {
        [HideInInspector]
        public Skill skill;
        public SkillEditorStuct Init(Skill skill,string skillName)
        {
            this.skill = skill;
            name = skillName;
            return this;
        }

        [HideLabel, HorizontalGroup, ReadOnly] 
        public string name;
        [Button, HorizontalGroup]
        public void Edit()
        {
            Selection.activeObject = skill;
        }
    }
}