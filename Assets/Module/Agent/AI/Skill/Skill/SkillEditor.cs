using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Module
{
    [CreateAssetMenu(menuName = "HZZ/技能")]
    public class SkillEditor : SerializedScriptableObject
    {
        #region Editor

#if UNITY_EDITOR
        [SerializeField,BoxGroup]
        private Type skillType;

        [Button(ButtonStyle.Box, Name = "创建技能"),BoxGroup]
        private void CreatEditor()
        {
            if (skillType != null)
            {
                var exam = CreateInstance(skillType);
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                AssetDatabase.CreateAsset(exam, path);
                AssetDatabase.Refresh();
                skillType = null;

            }
        }
        
        private IEnumerable<Type> GetFiterType()
        {
            string assembly = Application.dataPath + "/../Library/ScriptAssemblies/Project.dll";
            Assembly ass = Assembly.LoadFile(assembly);

            var types = ass.GetTypes();
            List<Type> result = new List<Type>();
        
            foreach (Type type1 in types)
            {
                if (type1.IsChild(typeof(Skill)))
                {
                    result.Add(type1);
                }
            }
        
            return result;
        }

#endif

        #endregion
    }
}