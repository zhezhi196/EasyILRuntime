using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace EditorModule
{
    public class PowerWindow: EditorWindow
    {
        public class FieldStruct
        {
            public bool anti;
            public string tag;
            public int type;
            public string name;
            public override string ToString()
            {
                return name;
            }
        }
        
        private List<string> targetList = new List<string>();
        private List<string> jichengList = new List<string>();
        private List<Type> typeList = new List<Type>();
        private List<string> typeEnum = new List<string>();
        private List<FieldStruct> _fieldStructs = new List<FieldStruct>();
        
        [MenuItem("Tools/程序工具/属性生成")]
        public static void OpenWindow()
        {
            GetWindow<PowerWindow>();
        }

        public int tarIndex;

        private void OnEnable()
        {
            targetList = new List<string>(){"GameAttribute","MonsterAttribute"};
            jichengList = new List<string>(){"IAttribute<FloatField>","IMonsterAttribute<FloatField>"};
            typeList = new List<Type>() {typeof(GameAttribute), typeof(MonsterAttribute)};
            typeEnum=new List<string>(){"FloatField"};
        }

        public void OnGUI()
        {
            tarIndex = EditorGUILayout.Popup("目标", tarIndex, targetList.ToArray());
            EditorGUILayout.Space();
            string type = targetList[tarIndex];
            for (int i = 0; i < _fieldStructs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _fieldStructs[i].anti = EditorGUILayout.Toggle("加密",_fieldStructs[i].anti);
                _fieldStructs[i].tag = EditorGUILayout.TextField(_fieldStructs[i].tag);
                //_fieldStructs[i].type = EditorGUILayout.Popup(_fieldStructs[i].type, typeEnum.ToArray());
                _fieldStructs[i].name = EditorGUILayout.TextField(_fieldStructs[i].name);
                if (GUILayout.Button("删除"))
                {
                    _fieldStructs.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("刷新"))
            {
                _fieldStructs.Clear();
                PropertyInfo[] property = typeList[tarIndex].GetProperties();
                for (int i = 0; i < property.Length; i++)
                {
                    FieldStruct temp = new FieldStruct();
                    temp = new FieldStruct();
                    LabelTextAttribute att =  (LabelTextAttribute) property[i].GetCustomAttribute(typeof(LabelTextAttribute));
                    if (att != null)
                    {
                        //temp.tag = $"[LabelText(\"{}\")][ShowInInspector]";
                        temp.tag = att.Text;
                    }
                    else
                    {
                        temp.tag = string.Empty;
                    }

                    NotAntiCrackAttribute notAnti = (NotAntiCrackAttribute) property[i].GetCustomAttribute(typeof(NotAntiCrackAttribute));
                    temp.anti = notAnti == null;
                    temp.type = 0;
                    temp.name = property[i].Name;
                    _fieldStructs.Add(temp);
                }
            }

            if (GUILayout.Button("添加"))
            {
                FieldStruct temp = new FieldStruct();
                temp.anti = true;
                _fieldStructs.Add(temp);
            }

            if (GUILayout.Button("生成脚本"))
            {
                // CsCode code = new CsCode("GameAttribute");
                // code.usingStr.Add("Sirenix.OdinInspector");
                // code.usingStr.Add("Module");
                // code.type = CsCodeType.Struct;
                // code.inherit.Add(typeof(IAttribute<FloatField>));
                // for (int i = 0; i < _fieldStructs.Count; i++)
                // {
                //     code.property.Add(new CsProperty(typeof(FloatField), _fieldStructs[i].name){isGet = true,isSet = true});
                // }
                //
                // CsConstructor constructor1 = new CsConstructor(new MethodVariable(typeof(IAttribute<float>), "target"));
                // for (int i = 0; i < _fieldStructs.Count; i++)
                // {
                //     constructor1.content.Append($"this.{_fieldStructs[i].name} = new FloatField(target.{_fieldStructs[i]})");
                // }
                //
                // code.constructor.Add(constructor1);
                //
                // CsConstructor constructor2 = new CsConstructor(new MethodVariable(typeof(float), "value"));
                // for (int i = 0; i < _fieldStructs.Count; i++)
                // {
                //     constructor2.content.Append($"this.{_fieldStructs[i].name} = new FloatField(value)");
                // }
                //
                // code.constructor.Add(constructor2);
                //
                // CsMethod methd1 = new CsMethod("", new MethodVariable(typeof(GameAttribute), "left"), new MethodVariable(typeof(GameAttribute), "right"));
                // methd1.SetOperator(true, "+");
                // methd1.content.Append("GameAttribute result = new GameAttribute()");
                // for (int i = 0; i < _fieldStructs.Count; i++)
                // {
                //     methd1.content.Append($"result.{_fieldStructs[i].name} = left.{_fieldStructs[i].name}+right.{_fieldStructs[i].name}");
                // }
                //
                // methd1.content.Append("return result");
                //
                // CsMethod methd2 = new CsMethod("", new MethodVariable(typeof(float), "left"), new MethodVariable(typeof(GameAttribute), "right"));
                // methd2.SetOperator(true, "+");
                // methd2.content.Append("return new GameAttribute(left) + right");
                //
                // CsMethod methd3 = new CsMethod("", new MethodVariable(typeof(GameAttribute), "left"), new MethodVariable(typeof(float), "right"));
                // methd3.SetOperator(true, "+");
                // methd3.content.Append("return new GameAttribute(right) + left");
                //
                // CsMethod methd4 = new CsMethod("", new MethodVariable(typeof(GameAttribute), "left"), new MethodVariable(typeof(GameAttribute), "right"));
                // methd4.SetOperator(true, "*");
                // methd4.content.Append("GameAttribute result = new GameAttribute()");
                // for (int i = 0; i < _fieldStructs.Count; i++)
                // {
                //     methd4.content.Append($"result.{_fieldStructs[i].name} = left.{_fieldStructs[i].name}*right.{_fieldStructs[i].name}");
                // }
                //
                // methd4.content.Append("return result");
                //
                // CsMethod methd5 = new CsMethod("", new MethodVariable(typeof(float), "left"), new MethodVariable(typeof(GameAttribute), "right"));
                // methd5.SetOperator(true, "*");
                // methd5.content.Append("return new GameAttribute(left) * right");
                //
                // CsMethod methd6 = new CsMethod("", new MethodVariable(typeof(GameAttribute), "left"), new MethodVariable(typeof(float), "right"));
                // methd6.SetOperator(true, "*");
                // methd6.content.Append("return new GameAttribute(right) * left");
                //
                // code.method.Add(methd1);
                // code.method.Add(methd2);
                // code.method.Add(methd3);
                // code.method.Add(methd4);
                // code.method.Add(methd5);
                // code.method.Add(methd6);
                // for (int i = 0; i < code.method.Count; i++)
                // {
                //     code.method[i].returnValue = typeof(GameAttribute);
                // }
                // code.Write($"{Application.dataPath}/GameProject/Core/Fight/Attribute/{type}.cs");

                StringBuilder builder = new StringBuilder();
                {
                    //using
                    builder.Append("using Sirenix.OdinInspector;\n");
                    builder.Append("using Module;\n");
                    //struct

                    builder.Append($"public struct {type} : {jichengList[this.tarIndex]}\n");

                    //字段
                    builder.Append("{\n");
                    for (int i = 0; i < _fieldStructs.Count; i++)
                    {
                        string langId = Language.GetID(_fieldStructs[i].tag, SystemLanguage.Chinese);
                        string header = !_fieldStructs[i].tag.IsNullOrEmpty() ? $"[LabelText(\"{_fieldStructs[i].tag}\")][ShowInInspector]" : "[ShowInInspector]";
                        header += _fieldStructs[i].anti ? string.Empty : "[NotAntiCrack]";
                        header += langId == null ? string.Empty : $"[AttributeName(\"{langId}\")]";
                        builder.Append($"    {header} public {typeEnum[_fieldStructs[i].type]} {_fieldStructs[i].name}" + " { get; set; }\n");
                    }
                    //构造函数1

                    builder.Append($"    public {type}({jichengList[tarIndex].Replace(typeEnum[tarIndex], "float")} target)\n");

                    builder.Append("    {\n");
                    for (int i = 0; i < _fieldStructs.Count; i++)
                    {
                        string temp = !_fieldStructs[i].anti ? ",FieldAesType.offset" : ",FieldAesType.Xor";
                        builder.Append($"        this.{_fieldStructs[i].name} = new FloatField(target.{_fieldStructs[i].name}{temp});\n");
                    }

                    builder.Append("    }\n");

                    //构造函数2
                    builder.Append($"    public {type}(float value)\n");
                    builder.Append("    {\n");
                    for (int i = 0; i < _fieldStructs.Count; i++)
                    {
                        string temp = !_fieldStructs[i].anti ? ",FieldAesType.offset" : ",FieldAesType.Xor";
                        builder.Append($"        this.{_fieldStructs[i].name} = new FloatField(value{temp});\n");
                    }

                    builder.Append("    }\n");

                    // +运算符
                    builder.Append($"    public static {type} operator +({type} left, {type} right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        {type} result = new {type}();\n");
                    for (int i = 0; i < _fieldStructs.Count; i++)
                    {
                        builder.Append($"        result.{_fieldStructs[i].name} = left.{_fieldStructs[i].name} + right.{_fieldStructs[i].name};\n");
                    }

                    builder.Append("        return result;\n");
                    builder.Append("    }\n");

                    // +运算符2
                    builder.Append($"    public static {type} operator +(float left, {type} right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        return new {type}(left) + right;\n");
                    builder.Append("    }\n");

                    // +运算符3
                    builder.Append($"    public static {type} operator +({type} left, float right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        return new {type}(right) + left;\n");
                    builder.Append("    }\n");

                    // *运算符
                    builder.Append($"    public static {type} operator *({type} left, {type} right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        {type} result = new {type}();\n");
                    for (int i = 0; i < _fieldStructs.Count; i++)
                    {
                        builder.Append($"        result.{_fieldStructs[i].name} = left.{_fieldStructs[i].name} * right.{_fieldStructs[i].name};\n");
                    }

                    builder.Append("        return result;\n");
                    builder.Append("    }\n");

                    // *运算符2
                    builder.Append($"    public static {type} operator *(float left, {type} right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        return new {type}(left) * right;\n");
                    builder.Append("    }\n");

                    // *运算符3
                    builder.Append($"    public static {type} operator *({type} left, float right)\n");
                    builder.Append("    {\n");
                    builder.Append($"        return new {type}(right) * left;\n");
                    builder.Append("    }\n");

                    builder.Append("}");
                    if (!Directory.Exists($"{Application.persistentDataPath}/工具"))
                    {
                        Directory.CreateDirectory($"{Application.persistentDataPath}/工具");
                    }
                    using (StreamWriter write = new StreamWriter($"{Application.persistentDataPath}/工具/{type}.txt"))
                    {
                        write.Write(builder);
                    }

                    Process.Start($"{Application.persistentDataPath}/工具/{type}.txt");
                    AssetDatabase.Refresh();
                }
            }
            if (GUILayout.Button("生成excel数据"))
            {
                if (!Directory.Exists($"{Application.persistentDataPath}/工具"))
                {
                    Directory.CreateDirectory($"{Application.persistentDataPath}/工具");
                }
                string path = $"{Application.persistentDataPath}/工具/Gameattribute.xlsx";
                Excel excel = ExcelHelper.CreateExcel(path);
                for (int i = 0; i < _fieldStructs.Count; i++)
                {
                    excel.Tables[0].SetValue(1, i + 1, _fieldStructs[i].tag);
                    excel.Tables[0].SetValue(2, i + 1, _fieldStructs[i].name);
                    excel.Tables[0].SetValue(3, i + 1, "int");
                }
                ExcelHelper.SaveExcel(excel,path);
                Process.Start(path);
            }

            if (GUILayout.Button("拷贝脚本到项目"))
            {
                if (!Directory.Exists($"{Application.persistentDataPath}/工具"))
                {
                    Directory.CreateDirectory($"{Application.persistentDataPath}/工具");
                }
                using (StreamReader rewader=new StreamReader($"{Application.persistentDataPath}/工具/{type}.txt"))
                {
                    string text = rewader.ReadToEnd();
                    using (StreamWriter wrriter=new StreamWriter($"{Application.dataPath}/GameProject/Core/Fight/Attribute/{type}.cs"))
                    {
                        wrriter.Write(text);
                    }
                }
                AssetDatabase.Refresh();
            }
        }
    }
}