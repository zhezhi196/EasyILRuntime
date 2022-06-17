using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using LitJson;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class LanguageImport: EditorWindow
    {
        [Serializable]
        public class LangTitle
        {
            [HorizontalGroup()]
            public int cloumn;
            [HorizontalGroup()]
            public SystemLanguage language;
        }
        [UnityEditor.MenuItem("Tools/策划工具/多语言导入")]
        public static void Open()
        {
            LanguageImport window = EditorWindow.GetWindow<LanguageImport>();
        }

        public bool subChannel;
        public Vector2Int fromPoint;
        public List<LangTitle> title;
        private ExcelTable table;
        private string tempPath;
        private void OnEnable()
        {
            fromPoint = new Vector2Int(2, 2);
            table = null;
            title = new List<LangTitle>();
            title.Add(new LangTitle() {cloumn = 4, language = SystemLanguage.Chinese});
            title.Add(new LangTitle() {cloumn = 5, language = SystemLanguage.English});
            subChannel = true;
        }
        private void OnGUI()
        {
            subChannel = EditorGUILayout.Toggle("分渠道", subChannel);
            fromPoint = EditorGUILayout.Vector2IntField("开始位置", fromPoint);
            if (!title.IsNullOrEmpty())
            {
                EditorGUILayout.BeginVertical();
                for (int i = 0; i < title.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    title[i].cloumn = EditorGUILayout.IntField(title[i].cloumn);
                    title[i].language = (SystemLanguage) EditorGUILayout.EnumPopup(title[i].language);
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndVertical();
            }
            
            if (GUILayout.Button("文件导入"))
            {
                string path = $"{Application.dataPath}/Language.txt";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                Import(text =>
                {
                });
            }
            if (GUILayout.Button("解密"))
            {
                using (StreamReader reader = new StreamReader($"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{Language.AssetOutPutPath}"))
                {
                    string old = reader.ReadToEnd();
                    string json = EncryptionHelper.Xor(old, Language.useKey);
                    Dictionary<string, LangInfo[]> info = JsonMapper.ToObject<Dictionary<string, LangInfo[]>>(json);
                    if (!Directory.Exists($"{Application.persistentDataPath}/工具"))
                    {
                        Directory.CreateDirectory($"{Application.persistentDataPath}/工具");
                    }

                    string path = $"{Application.persistentDataPath}/工具/Language.xlsx";
                    Excel jiemiExcel = ExcelHelper.CreateExcel(path);
                    ExcelTable table = jiemiExcel.Tables[0];
                    int row = 1;
                    foreach (KeyValuePair<string, LangInfo[]> keyValuePair in info)
                    {
                        table.SetValue(row, 1, keyValuePair.Key);
                        int column = 2;
                        for (int i = 0; i < keyValuePair.Value.Length; i++)
                        {
                            table.SetValue(row, column, keyValuePair.Value[i].c);
                            column++;
                        }
                        row++;
                    }

                    ExcelHelper.SaveExcel(jiemiExcel, path);
                    Process.Start(path);
                }
            }
        }

        private void OnDisable()
        {
            string path = $"{Application.dataPath}/Language.txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        private void Import(Action<string> callback)
        {
            OpenDialog(false, files =>
            {
                var temp = files[0];
                //File.Copy(files[0],temp , true);
                tempPath = temp + ".temp";
                FileInfo old = new FileInfo(temp);
                old.CopyTo(tempPath);
                Excel xls = ExcelHelper.LoadExcel(tempPath, true);
                table = xls.Tables[0];
                var names = Enum.GetNames(typeof(ChannelType));
                for (int k = 0; k < names.Length; k++)
                {
                    ChannelType channelT = names[k].ToEnum<ChannelType>();
                    Dictionary<string, LangInfo[]> langObj = new Dictionary<string, LangInfo[]>();
                    for (int i = fromPoint.x; i <= table.NumberOfRows; i++)
                    {
                        List<LangInfo> info = new List<LangInfo>();
                        string key = table.GetCell(i, fromPoint.y).Value;
                        int channelBit = -1;
                        if (fromPoint.y > 1 && subChannel)
                        {
                            var channel = table.GetValue(i, fromPoint.y - 1).ToString().ToLower();
                            channelBit = Module.Tools.GetChannelBit(channel);
                        }

                        if (!key.IsNullOrEmpty())
                        {
                            if (channelBit != -1 || (channelBit & (int) channelT) != 0) //channel.ToLower().Contains(Channel.channel.ToString().ToLower())))
                            {
                                for (int j = fromPoint.y; j <= table.NumberOfColumns; j++)
                                {
                                    var tar = title.Find(fd => fd.cloumn == j);
                                    if (tar != null)
                                    {
                                        info.Add(new LangInfo()
                                            {l = tar.language, c = table.GetValue(i, j).ToString()});
                                    }
                                }

                                if (!langObj.ContainsKey(key))
                                {
                                    langObj.Add(key, info.ToArray());
                                }
                                else
                                {
                                    GameDebug.LogError($"检查到{key}值重复");
                                }
                            }
                        }
                    }
                
                    string outPutFoder = $"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetChannelConfig(names[k])}";
                    if (!Directory.Exists(outPutFoder))
                    {
                        Directory.CreateDirectory(outPutFoder);
                    }
                    
                    using (StreamWriter writer = new StreamWriter(outPutFoder+"/lang.txt", false, Encoding.UTF8))
                    {
                        string txt = JsonMapper.ToJson(langObj);
                        callback?.Invoke(txt);
                        writer.Write(EncryptionHelper.Xor(txt, Language.useKey));
                    }
                }

                File.Delete(tempPath);
                AssetDatabase.Refresh();
                //File.Delete(temp);
            });
            
        }



        private void OpenDialog(bool multiselect, Action<string[]> callback)
        {
            FolderBrowserHelper.SelectFile(st =>
            {
                string[] sName = {st};
                callback?.Invoke(sName);
            });
        }
    }
}