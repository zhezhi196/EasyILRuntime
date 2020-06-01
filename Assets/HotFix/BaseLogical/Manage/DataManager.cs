using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LitJson;
using Module;
using UnityEngine;
using xasset;

namespace HotFix
{
    public class DataManager : Manager
    {
        protected override int runOrder { get; } = -5;

        protected override string processDiscription
        {
            get { return "数据初始化"; }
        }
        
        private static Dictionary<string, TableData[]> m_tableDic = new Dictionary<string, TableData[]>();
        
        protected override void BeforeInit()
        {
        }

        protected override void Init(RunTimeSequence runtime)
        {
            if (Assets.source == ResourceSource.InnerBundle)
            {
                DirectoryInfo dir = new DirectoryInfo($"{Application.dataPath}/Bundles/Config/Tables");
                FileInfo[] file = dir.GetFiles("*.json");

                for (int i = 0; i < file.Length; i++)
                {
                    using (StreamReader reader = new StreamReader(file[i].FullName))
                    {
                        string className = "HotFix." + file[i].Name.Replace(".json", string.Empty);
                        Type temp = HotFixManager.bridge.allTypes[className];
                        m_tableDic.Add(className, ReadData(reader.ReadToEnd(), temp));
                    }
                }
            }
            else if (Assets.source == ResourceSource.ServerBundle)
            {
                foreach (KeyValuePair<string, Type> keyValuePair in HotFixManager.bridge.allTypes)
                {
                    if (keyValuePair.Value.BaseType == typeof(TableData))
                    {
                        Assets.LoadAsync($"Assets/Bundles/Config/Tables/{keyValuePair.Value.Name}.json", typeof(TextAsset)).onComplete += asset =>
                        {
                            TextAsset text = (TextAsset) asset.asset;
                            string className = "HotFix." + keyValuePair.Value.Name;
                            Type temp = HotFixManager.bridge.allTypes[className];
                            m_tableDic.Add(className, ReadData(text.text, temp));
                        };
                    }
                }
            }

            runtime.NextAction();
        }
        
        private TableData[] ReadData(string content, Type type)
        {
            string[] spiteValue = content.Split('\n');
            TableData[] data = new TableData[spiteValue.Length];

            for (int i = 0; i < spiteValue.Length; i++)
            {
                data[i]= (TableData)JsonMapper.ToObject(spiteValue[i], type);
            }

            return data;
        }
        

        public static T Where<T>(Predicate<T> predicate) where T: TableData
        {
            TableData[] data = m_tableDic[typeof(T).FullName];
            for (int i = 0; i < data.Length; i++)
            {
                T t = (T) data[i];
                if (predicate.Invoke(t))
                {
                    return t;
                }
            }

            return default;
        }

        public static T[] WhereList<T>(Predicate<T> predicate) where T: TableData
        {
            TableData[] data = m_tableDic[typeof(T).FullName];
            List<T> temp = new List<T>();
            for (int i = 0; i < data.Length; i++)
            {
                T t = (T) data[i];
                if (predicate.Invoke(t))
                {
                    temp.Add(t);
                }
            }

            return temp.ToArray();
        }
    }

}
