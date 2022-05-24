using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Module;
using Sirenix.Utilities.Editor;
using SqlCipher4Unity3D;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Application = UnityEngine.Application;
using Debug = UnityEngine.Debug;
using Tools = Module.Tools;

namespace EditorModule
{
    public class InsertDBTools: EditorWindow
    {
        public static string[] xiaohua =
        {
            "福建人和东北人玩成语接龙系列：心心相印→印贼作父→父相伤害→害想咋滴。。。", "挣钱是一种能力，花钱是一种技术，我能力有限，技术却很高。", "生活会让你苦上一阵子，等你适应以后，再让你苦上一辈子。",
            "现在的姑娘如果走在古代的街上，被皇上相中拉回去侍寝，晚上洗了脸，会不会判个欺君之罪啥的。", "人生不如意的事十有八九，剩下的一二是特别不如意",
            "长得好看点的人人生可能是传记，可能是小说，可能是散文。而你，只能是个段子。",
            "爱笑的人运气不会太差，可是我就想知道，一个人如果一直运气很差，不知道怎么笑得出来。",
            "谁说“念念不忘，必有回响”，喜欢的人从没搭理过我，想一夜暴富，也从没实现过。",
            "是媳妇重要还是游戏重要？当然是媳妇重要，所以我只敢打游戏，不敢打媳妇。",
            "四海八荒，我一女子，来到凡间，只为渡今生的一个劫：穷。",
            "在过去，只要感情真，年龄不是问题。而现在，只要感情真，性别不是问题。",
            "只要你非常努力，总有一天你会发现，你永远无法拉近你跟有钱人的差距。",
            "做人真不容易：18岁前问成绩，18岁后问对象，有对象后问孩子，有孩子后问孩子成绩，孩子18岁后问孩子对象，都是地球人干嘛要互相折腾。",
            "知道在学校为什么感觉很困吗？因为学校，是梦的开始的地方。",
            "建议大家尝试早睡觉，多运动，不吃夜宵，不抽烟不喝酒，早睡早起养成一个良好的习惯，久而久之，你就一个朋友都没有了。",
            "做一个独立的小仙女，不以物喜不以己悲，乐观坚强且不依靠男的，不装可爱不撒娇，直率简单不耍心机，这样坚持下来，不仅嫁不出去，而且还找不到男朋友。"
            ,"结束友情的方式有许多种，最彻底的一种是借钱不还。"
            ,"别总是单身狗单身狗的了，按年龄算你应该是单身鳖，按体型算你应该是单身猪，按智商算你应该是单身傻狍子"
            ,"好看的皮囊三千一宿，有趣的灵魂要房要车","投对了简历，可以获得一份好工作；投对了胎，你可以不工作。"
            ,"人无完人，被人羞辱是正常的，而我希望别人用这三句话来羞辱我：“你怎么瘦成这个死样”“你不就有几个臭钱吗”“有个好老公了不起啊”。"
        };

        [UnityEditor.MenuItem("Tools/策划工具/excel导入数据库")]
        public static void OpenDb()
        {
            InsertDBTools window = EditorWindow.GetWindow<InsertDBTools>();
        }

        private void OnEnable()
        {
            csFoderPath = Application.dataPath + "/GameProject/Data";
            dontCreatCs = "AudioData|InitData";
            commont = true;
            subChannel = false;
        }

        public string csFoderPath;

        public Vector2Int start = new Vector2Int(3, 1);
        public string nameSpace = "Project.Data";
        public Vector2Int ext = new Vector2Int(1, 1);
        public bool commont;
        public Vector2Int isActive = new Vector2Int(1, 2);
        public string dontCreatCs;
        private string tempPath;
        public bool subChannel;


        private void OnGUI()
        {
            subChannel = EditorGUILayout.Toggle("分渠道",subChannel);
            start = EditorGUILayout.Vector2IntField("开始", start);
            isActive = EditorGUILayout.Vector2IntField("是否激活", isActive);
            dontCreatCs = EditorGUILayout.TextField("单独不生成脚本列表", dontCreatCs);
            csFoderPath = EditorGUILayout.TextField("脚本路径", csFoderPath);
            nameSpace = EditorGUILayout.TextField("命名空间", nameSpace);
            ext = EditorGUILayout.Vector2IntField("类后缀", ext);
            commont = EditorGUILayout.Toggle("备注", commont);

            if (GUILayout.Button("导入Excel数据"))
            {
                InsertSingle();
                GameDebug.Log("所有表导入完毕!");
                EditorUtility.DisplayDialog("我一点压力都没有了,我好开心", xiaohua.Random(), "好的");
            }

            if (GUILayout.Button("生成脚本"))
            {
                CreatCs();
            }

            if (GUILayout.Button("查看数据库"))
            {
                Process.Start(SqlData.editorDbPath);
            }

            if (GUILayout.Button("打开数据库"))
            {
                string path = Application.dataPath + "/../Excel/Config.xlsx";
                Process.Start(path);
            }

            if (GUILayout.Button("清空缓存数据"))
            {
                string config = $"{Application.persistentDataPath}/{ConstKey.Player_data}";
                if (File.Exists(config))
                {
                    File.Delete(config);
                }
            }

            GUILayout.Space(50);
            GUILayout.Label("说明: \n 1:上方输入坐标对应excel中X为行,Y为列\n 2:开始=>是从目标单元格开始读取数据,需包含字段的行\n 3:是否激活=>是在目标单元格填入任意非0数据,则视为此表无效\n 4:类后缀和命名空间需要程序填写到对应的目标单元格中\n " +
                            "5:excel单元格中需要字段名和类型都需要填入对应的数据,否则此列视为无效列,不会导入到表中,也不会生成对应的脚本\n 6:表格的sheet名即为表名,请不要在表名中带有中文,否则视为无效表格\n 7: 单独不生成脚本列表=>以下表不生成脚本,不同表以|分割\n 8: 表名请不要起InitData\n 9:有任何问题请联系黄哲智");
            
            // if (GUILayout.Button("整表导入"))
            // {
            //     InsertMutiple();
            // }
            //
            // if (GUILayout.Button("合并数据"))
            // {
            //     Merge();
            // }
        }

        public void CreatCs()
        {
            OpenDialog(false, files =>
            {
                if (!Directory.Exists(SqlData.editorDbPath))
                {
                    Directory.CreateDirectory(SqlData.editorDbPath);
                }

                CreatCs(files[0]);

            });
        }

        public void CreatCs(string fileName)
        {
            if (!Directory.Exists(csFoderPath))
            {
                Directory.CreateDirectory(csFoderPath);
            }
            var tableName = GetAllTableName(fileName);
            for (int l = 0; l < tableName.Length; l++)
            {
                var tables = tableName[l];
                if (Tools.HasChinese(tables.TableName)) continue;
                string[] temp = dontCreatCs.Split('|');
                if (!temp.Contains(tables.TableName))
                {
                    string fileCsName = csFoderPath + "/" + tables.TableName + ".cs";
                    using (StreamWriter writer = new StreamWriter(fileCsName))
                    {
                        StringBuilder csBuilder = new StringBuilder();
                        csBuilder.Append($"using Module;\n");
                        csBuilder.Append($"\n");
                        csBuilder.Append($"namespace {nameSpace}\n");
                        csBuilder.Append("{\n");

                        csBuilder.Append($"    public class {tables.TableName} : {tables.GetValue(ext.x, ext.y)}\n");
                        csBuilder.Append("    {\n");

                        for (int i = start.y; i <= tables.NumberOfColumns; i++)
                        {
                            var fieldInfo = IsFileldVaild(tables, i);

                            if (fieldInfo.Item1)
                            {
                                string tag = tables.GetValue(start.x - 1, i).ToString();

                                if (!tag.IsNullOrEmpty() && commont)
                                {
                                    csBuilder.Append($"        /// <summary>\n");
                                    csBuilder.Append($"        /// {tag}\n");
                                    csBuilder.Append($"        /// <summary>\n");
                                }

                                csBuilder.Append($"        public {fieldInfo.Item3.ToLower()} " + fieldInfo.Item2 +
                                                 " { get; set; }\n");
                            }
                        }

                        csBuilder.Append("    }\n");
                        csBuilder.Append("}\n");

                        writer.Write(csBuilder);
                    }
                }
            }
            File.Delete(tempPath);
            AssetDatabase.Refresh();
            Debug.Log("生成脚本成功");
        }

        private void OpenDialog(bool multiselect, Action<string[]> callback)
        {
            FolderBrowserHelper.SelectFile(st =>
            {
                string[] sName = {st};
                callback?.Invoke(sName);
            });
        }
        
        private ExcelTable[] GetAllTableName(string path)
        {
            FileInfo file = new FileInfo(path);
            tempPath = path + ".temp";
            file.CopyTo(tempPath);
            Excel xls = ExcelHelper.LoadExcel(tempPath);
            return xls.Tables.ToArray();
        }

        private void ClearConfig(SQLiteConnection conn, string name)
        {
            string dataName = name.Remove(0, name.LastIndexOf('\\') + 1).Split('.')[0];
            string sql = string.Format("DROP TABLE IF EXISTS \"{0}\"", dataName);
            conn.Execute(sql);
        }

        private void InsertSingle()
        {
            OpenDialog(false, files =>
            {
                string editorPath = SqlData.editorDbPath + ConstKey.Config_data;
                if (!Directory.Exists(SqlData.editorDbPath))
                {
                    Directory.CreateDirectory(SqlData.editorDbPath);
                }
                CreateSqliteConnection(editorPath, null, files[0]);
                CreateSqliteConnection($"{Application.streamingAssetsPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.DB)}/{ConstKey.Config_data}", ConstKey.SqlPassword, files[0]);
                CreatCs(files[0]);

            });
        }


        private void CreateSqliteConnection(string path, string password,string fileName)
        {
            using (SQLiteConnection conn = new SQLiteConnection(path, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite, password))
            {
                var tableName = GetAllTableName(fileName);
                for (int j = 0; j < tableName.Length; j++)
                {
                    if (!Tools.HasChinese(tableName[j].TableName))
                    {
                        ClearConfig(conn, tableName[j].TableName);
                    }
                }

                CopyAllDataToDb(conn, tableName);
                conn.Close();
            }
        }

        private string GetFieldType(string type)
        {
            if (type == "string")
            {
                return "TEXT";
            }
            else if (type == "int")
            {
                return "INTEGER";
            }
            else if (type == "float")
            {
                return "REAL";
            }

            return "TEXT";
        }

        private void CopyDataToDb(SQLiteConnection conn, ExcelTable tables)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"CREATE TABLE \"{tables.TableName}\"(");
            List<string> field = new List<string>();
            List<string> cell = new List<string>();

            for (int i = start.y; i <= tables.NumberOfColumns; i++)
            {
                var fieldInfo = IsFileldVaild(tables, i);

                if (fieldInfo.Item1)
                {
                    string f = "'" + fieldInfo.Item2 + "'";
                    field.Add(f);
                    cell.Add(f + " " + GetFieldType(fieldInfo.Item3));
                }
            }

            builder.Append(String.Join(",", cell));

            builder.Append(")");
            //Debug.Log(builder); 
            conn.Execute(builder.ToString());
            
            StringBuilder insert = new StringBuilder();
            insert.Append($"INSERT into {tables.TableName} ({string.Join(",", field)}) VALUES\n");
            List<string> cellString = new List<string>();

            for (int i = start.x + 2; i <= tables.NumberOfRows; i++)
            {
                string idKey = tables.GetValue(i, start.y).ToString();
                //获取表的渠道
                string channel = null;
                if (start.y > 1 && subChannel)
                {
                    channel = tables.GetValue(i, start.y - 1).ToString().ToLower();
                }

                Config config = AssetDatabase.LoadAssetAtPath<Config>("Assets/Config/Config.asset");
                ChannelType cn = config.channel;
                //如果没有ID列,则此行无效
                if (idKey.IsNullOrEmpty() || (!channel.IsNullOrEmpty() && channel.ToLower().Contains(cn.ToString().ToLower())))
                {
                    continue;
                }

                StringBuilder insertCell = new StringBuilder();

                insertCell.Append("(");

                List<string> content = new List<string>();
                
                for (int j = start.y; j <= tables.NumberOfColumns; j++)
                {
                    //如果字段名或者字段类型是空,则此列无效]
                    var fieldInfo = IsFileldVaild(tables, j);
                    
                    // string fieldKey = tables.GetValue(start.x, j).ToString();
                    // string fieldType = tables.GetValue(start.x + 1, j).ToString();
                    if (fieldInfo.Item1)
                    {
                        content.Add("'" + tables.GetValue(i, j) + "'");
                    }                    
                }

                insertCell.Append(string.Join(",", content));
                insertCell.Append($")\n");
                cellString.Add(insertCell.ToString());
            }

            if (!cellString.IsNullOrEmpty())
            {
                insert.Append(string.Join(",", cellString.ToArray()));
                insert.Append(";");
            
                //GameDebug.Log(insert);
                conn.Execute(insert.ToString());
            }
        }

        private (bool, string, string) IsFileldVaild(ExcelTable tables,int column)
        {
            string fieldKey = tables.GetValue(start.x, column).ToString().Trim();
            string fieldType = tables.GetValue(start.x + 1, column).ToString().Trim();
            return (!fieldKey.IsNullOrEmpty() && !fieldType.IsNullOrEmpty(), fieldKey, fieldType);
        }

        private void CopyAllDataToDb(SQLiteConnection conn, ExcelTable[] tables)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("using Module;\n");
            builder.Append("using SqlCipher4Unity3D;\n");
            builder.Append("\n");
            builder.Append("namespace Project.Data\n");
            builder.Append("{\n");
            builder.Append("    public static class InitData\n");
            builder.Append("    {\n");
            builder.Append("        public static void Init(SQLiteConnection config)\n");
            builder.Append("        {\n");
            for (int i = 0; i < tables.Length; i++)
            {
                //如果激活列填入的是非0,则视为无效
                string act = tables[i].GetValue(isActive.x, isActive.y).ToString();
                if (!((act != "0" && !act.IsNullOrEmpty()) || Tools.HasChinese(tables[i].TableName)))
                {
                    CopyDataToDb(conn, tables[i]);
                    builder.Append($"            SqlData.InitSqlData<{tables[i].TableName}>(ConstKey.Config_data, SQLiteOpenFlags.ReadOnly, config);\n");
                }
            }
            builder.Append("        }\n");
            builder.Append("    }\n");
            builder.Append("}\n");
            
            using (StreamWriter writer = new StreamWriter(csFoderPath + "/InitData.cs"))
            {
                writer.Write(builder.ToString());
            }

            File.Delete(tempPath);
            AssetDatabase.Refresh();
        }

    }
}