using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Module;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class CsCode
    {
        //[MenuItem("Tools/测试生成脚本功能")]
        public static void Test()
        {
            CsCode code = new CsCode("GameAttribute");
            code.usingStr.Add("Sirenix.OdinInspector");
            code.usingStr.Add("Module");
            code.nameSpace = "fff";
            code.type = CsCodeType.Struct;
            code.inherit.Add(typeof(IAttribute<List<int>>));
            code.inherit.Add(typeof(IDisposable));
            string[] att = new[] {"LabelText(\"枪械爆头攻击力\")", "ShowInInspector"};
            
            code.fields.Add(new CsField(typeof(List<List<List<int>>>), "hp"){attribute =att,prefix = CsFieldPrefix.Private});
            code.fields.Add(new CsField(typeof(FloatField), "gunAtt"));
            code.fields.Add(new CsField(typeof(FloatField), "moveSpeed"));
            code.property.Add(new CsProperty(typeof(FloatField), "hp1"));
            code.property.Add(new CsProperty(typeof(FloatField), "hp2"));
            code.property.Add(new CsProperty(typeof(FloatField), "hp3"));
            CsConstructor con = new CsConstructor(new MethodVariable(typeof(IAttribute<float>), "target"));
            
            code.constructor.Add(con);
            code.constructor.Add(new CsConstructor(new MethodVariable(typeof(IAttribute<int>), "target")));
            code.method.Add(new CsMethod("ffff", new MethodVariable(typeof(IAttribute<float>), "target")));
            CsMethod met = new CsMethod("ffff",new MethodVariable(typeof(GameAttribute),"left"),new MethodVariable(typeof(GameAttribute),"right"));
            met.returnValue = typeof(int);
            met.isStatic = true;
            //met.SetOperator(true, "+");
            code.method.Add(met);
            if (!Directory.Exists($"{Application.persistentDataPath}/工具"))
            {
                Directory.CreateDirectory($"{Application.persistentDataPath}/工具");
            }
            code.Write(Application.persistentDataPath + "/工具/temp.cs");
        }
        
        /// <summary>
        /// 根据type获取相应在脚本里的字符串
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTypeName(Type t)
        {
            if (t.IsGenericType)
            {
                string mainType = t.Name.Substring(0, t.Name.IndexOf('`'));
                for (int i = 0; i < t.GenericTypeArguments.Length; i++)
                {
                    mainType += ("<" + GetTypeName(t.GenericTypeArguments[i]) + ">");
                }

                return mainType;
            }
            else
            {
                return t.Name.Replace("Int32", "int").Replace("Single", "float").Replace("Int64", "long")
                    .Replace("Double", "double").Replace("String", "string").Replace("Boolean", "bool")
                    .Replace("Object", "object")
                    .Replace("Void", "void").Replace("Byte", "byte").Replace("Char", "char").Replace("UInt32", "uint")
                    .Replace("UInt64", "ulong");
            }
        }
        
        /// <summary>
        /// using cotent
        /// </summary>
        public List<string> usingStr = new List<string>();
        /// <summary>
        /// public private
        /// </summary>
        public CsFieldPrefix classPrefix;
        /// <summary>
        /// class struct enum
        /// </summary>
        public CsCodeType type;
        /// <summary>
        /// 命名空间
        /// </summary>
        public string nameSpace;
        /// <summary>
        ///  是否是抽象类
        /// </summary>
        public bool isAbstract;
        /// <summary>
        /// 是否是静态类
        /// </summary>
        public bool isStatic;
        /// <summary>
        /// 继承的对象
        /// </summary>
        public List<Type> inherit = new List<Type>();
        /// <summary>
        /// 脚本名
        /// </summary>
        public string codeName;
        /// <summary>
        /// 字段
        /// </summary>
        public List<CsField> fields = new List<CsField>();
        /// <summary>
        /// 属性
        /// </summary>
        public List<CsProperty> property = new List<CsProperty>();
        /// <summary>
        /// 构造函数
        /// </summary>
        public List<CsConstructor> constructor = new List<CsConstructor>();
        /// <summary>
        /// 普通函数
        /// </summary>
        public List<CsMethod> method = new List<CsMethod>();

        private int bracesIndex;
        /// <summary>
        /// 最终脚本
        /// </summary>
        public StringBuilder mainCode = new StringBuilder();
        
        public CsCode(string codeName)
        {
            this.codeName = codeName;
        }

        private StringBuilder GetString(StringBuilder str)
        {
            for (int i = 0; i < bracesIndex; i++)
            {
                str.Insert(0, "\t");
            }

            return str;
        }
        
        private string GetString(string str)
        {
            for (int i = 0; i < bracesIndex; i++)
            {
                str = "\t" + str;
            }

            return str;
        }
        
        /// <summary>
        /// 为脚本添加空格
        /// </summary>
        public void Black()
        {
            mainCode.Append(" ");
        }
        
        /// <summary>
        /// 另起一行,开始一个大括号
        /// </summary>
        public void BeginBrace()
        {
            mainCode.Append("\n");
            mainCode.Append(GetString("{\n"));
            bracesIndex++;
        }
        /// <summary>
        /// 另起一行,完成一个大括号
        /// </summary>
        public void EndBrace()
        {
            mainCode.Append("\n");
            bracesIndex--;
            string temp = GetString("}\n");
            mainCode.Append(temp);
        }
        
        /// <summary>
        /// 脚本写入本地
        /// </summary>
        /// <param name="path"></param>
        public void Write(string path)
        {
            GetUsing();
            if (!nameSpace.IsNullOrEmpty())
            {
                GetNameSpace();
                BeginBrace();
            }
            GetClassheader();
            BeginBrace();
            GetField();
            GetProperty();
            GetConstructor();
            GetMethod();
            EndBrace();
            if (!nameSpace.IsNullOrEmpty())
            {
                EndBrace();
            }

            using (StreamWriter wriete = new StreamWriter(path))
            {
                wriete.Write(mainCode);
            }
            AssetDatabase.Refresh();
            Process.Start(path);
        }

        #region Get

        private void GetUsing()
        {
            for (int i = 0; i < usingStr.Count; i++)
            {
                mainCode.Append("using ");
                mainCode.Append(usingStr[i]);
                mainCode.Append(";\n");
            }
        }

        private bool GetNameSpace()
        {
            if (!nameSpace.IsNullOrEmpty())
            {
                mainCode.Append($"namespace {nameSpace}");
                return true;
            }

            return false;
        }

        private void GetClassheader()
        {
            string abs = isAbstract ? " abstract" : string.Empty;
            string sta = isStatic && !isAbstract ? " static" : string.Empty;
            mainCode.Append(GetString(classPrefix.ToString().ToLower()));
            mainCode.Append(sta);
            mainCode.Append(abs);
            Black();
            mainCode.Append(type.ToString().ToLower());
            Black();
            mainCode.Append(codeName);
            if (!inherit.IsNullOrEmpty())
            {
                mainCode.Append(" : ");
                List<string> temp = new List<string>();
                for (int i = 0; i < inherit.Count; i++)
                {
                    temp.Add(GetTypeName(inherit[i]));
                }

                mainCode.Append(string.Join(",", temp));
            }
        }

        private void GetField()
        {
            if (!fields.IsNullOrEmpty())
            {
                for (int i = 0; i < fields.Count; i++)
                {
                    mainCode.Append(GetString(fields[i].GetStr()));
                    mainCode.Append(";\n");
                }
            }
        }
        
        private void GetProperty()
        {
            if (!property.IsNullOrEmpty())
            {
                for (int i = 0; i < property.Count; i++)
                {
                    mainCode.Append(GetString(property[i].GetStr()));
                }
            }
        }

        private void GetConstructor()
        {
            if (!constructor.IsNullOrEmpty())
            {
                for (int i = 0; i < constructor.Count; i++)
                {
                    mainCode.Append(GetString(constructor[i].GetStr(codeName, bracesIndex)));
                }
            }
        }
        
        private void GetMethod()
        {
            if (!method.IsNullOrEmpty())
            {
                for (int i = 0; i < method.Count; i++)
                {
                    mainCode.Append(GetString(method[i].GetStr(method[i].methodName, bracesIndex)));
                }
            }
        }

        #endregion
    }
}