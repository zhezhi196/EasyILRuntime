using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ToolsEditor
{
    public class ConfigEditor: EditorWindow
    {
        #region Static

        public static string configDataPath = $"{Application.dataPath}/Editor/ToolsEditor/ConfigEditor/ConfigData/";
        public static string classPath = $"{Application.dataPath}/HotFix/ToolsEditor/Monster/";
        public static ConfigEditor window;
        
        public static void Open(Dictionary<string,bool> target,Type type)
        {
            window = GetWindow<ConfigEditor>();
            window.target = target;
            window.type = type;
            window.tempDic = new Dictionary<string, bool>();
            foreach (KeyValuePair<string,bool> keyValuePair in target)
            {
                window.tempDic[keyValuePair.Key] = keyValuePair.Value;
            }
            window.Show();
        }
        public static Dictionary<string, bool> InitField(Type type)
        {
            FieldInfo[] field = type.GetFields();
            Dictionary<string, bool> target = new Dictionary<string, bool>();
            
            for (int i = 0; i < field.Length; i++)
            {
                target.Add(field[i].Name, true);
            }

            return target;
        }

        #endregion
        
        public Dictionary<string, bool> target;
        public Type type;
        private Dictionary<string, bool> tempDic;

        private void OnGUI()
        {
            foreach (KeyValuePair<string,bool> keyValuePair in target)
            {
                EditorGUILayout.BeginHorizontal();
                tempDic[keyValuePair.Key] = EditorGUILayout.Toggle(keyValuePair.Key, keyValuePair.Value);
                EditorGUILayout.EndHorizontal();
            }

            foreach (KeyValuePair<string,bool> keyValuePair in tempDic)
            {
                target[keyValuePair.Key] = keyValuePair.Value;
            }

            if (GUILayout.Button("保存"))
            {
                string json = JsonMapper.ToJson(target);
                using (StreamWriter writer = new StreamWriter(ConfigEditor.configDataPath + typeof(MonsterEditor).Name))
                {
                    writer.Write(json);
                }
                
                AssetDatabase.Refresh();
            }

            if (GUILayout.Button("生成数据结构"))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("using Module;\n");
                builder.Append("using System;\n\n");
                builder.Append("namespace HotFix\n{\n");
                builder.Append($"\tpublic class {type.Name}\n\t"+"{\n");

                FieldInfo[] field = type.GetFields();
                for (int i = 0; i < field.Length; i++)
                {
                    if (target[field[i].Name])
                    {
                        builder.Append($"\t\tpublic {GetFieldTypeString(field[i].FieldType)} {field[i].Name};\n");
                    }
                }

                builder.Append("\t}\n");
                builder.Append("}\n");

                using (StreamWriter writer = new StreamWriter(classPath + type.Name + ".cs"))
                {
                    writer.Write(builder);
                }
                AssetDatabase.Refresh();
            }
        }

        private string GetFieldTypeString(Type type)
        {
            if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(float))
            {
                return "double";
            }
            else if (type == typeof(double))
            {
                return "double";
            }
            else if (type == typeof(long))
            {
                return "long";
            }
            else
            {
                return type.Name;
            }
        }

        public static (bool,object) GetSerializable(Type type,object value)
        {
            if (value == null) return (true,string.Empty);
            if (type == typeof(Color))
            {
                Color color = (Color) value;
                return (true,string.Join("_", color.r, color.g, color.b, color.a));
            }
            else if (type==typeof(Transform))
            {
                Transform transform = (Transform) value;
                return (true,string.Join("#",
                    string.Join("_", transform.position.x, transform.position.y, transform.position.z),
                    string.Join("_", transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z)));
            }

            return (false, value);
        }

    }
}