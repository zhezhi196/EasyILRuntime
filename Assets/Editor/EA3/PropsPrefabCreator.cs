using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Tools = Module.Tools;

namespace EditorModule
{
    public class PropsPrefabCreator: OdinEditorWindow
    {
        [LabelText("开始位置")]
        public int startRow;
        [LabelText("表名")]
        public string tableName;
        [LabelText("预制列")]
        public int prefabColumn;
        [LabelText("美术资源列")]
        public int artColumn;
        [LabelText("类型列")]
        public int typeColumn;
        [LabelText("程序集")]
        public string assembly;
        [LabelText("脚本位置")]
        public string csFoder;
        [LabelText("生成预制体文件夹")]
        public List<string> prefabFolder;
        [LabelText("美术资源文件夹")]
        public List<string> artFolder;

        [MenuItem("Tools/程序工具/物品生成器")]
        public static void Opp()
        {
            GetWindow<PropsPrefabCreator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            prefabColumn = 7;
            artColumn = 2;
            typeColumn = 24;
            prefabFolder = new List<string>();
            prefabFolder.Add($"Assets/Bundles/Props/Scene/");
            prefabFolder.Add($"Assets/Bundles/Props/Watch/");

            artFolder = new List<string>();
            artFolder.Add($"Assets/ArtWork/Props/Mode/");
            artFolder.Add($"Assets/ArtWork/Props/Mode_main/");
            artFolder.Add($"Assets/ArtWork/scenes_items/Artwork/Model/shi/");
            artFolder.Add($"Assets/ArtWork/scenes_items/Artwork/Model/DiaoSu/");
            artFolder.Add($"Assets/ArtWork/Player/Weapon/Bomb/");
            artFolder.Add($"Assets/ArtWork/Player/Weapon/Crossbow/Model/");
            artFolder.Add($"Assets/ArtWork/Player/Weapon/M1014/");
            artFolder.Add($"Assets/ArtWork/Player/Weapon/MP5/Model/");
            artFolder.Add($"Assets/ArtWork/Player/Weapon/pistol/");
            
            tableName = "PropData";
            startRow = 5;
            assembly=Application.dataPath + "/../Library/ScriptAssemblies/Project.dll";
            csFoder = Application.dataPath + "/GameProject/Props/";
        }
        
        [Button]
        public void CreatCs()
        {
            FolderBrowserHelper.SelectFile(st =>
            {
                Excel excel = ExcelHelper.LoadExcel(st);
                for (int i = 0; i < excel.Tables.Count; i++)
                {
                    if (excel.Tables[i].TableName == tableName)
                    {
                        CreatClass(excel.Tables[i]);
                        break;
                    }
                }
                AssetDatabase.Refresh();
            });
        }
        [Button]
        public void CreatPrefab()
        {
            FolderBrowserHelper.SelectFile(st =>
            {
                Excel excel = ExcelHelper.LoadExcel(st);
                for (int i = 0; i < excel.Tables.Count; i++)
                {
                    if (excel.Tables[i].TableName == tableName)
                    {
                        ReadTable(excel.Tables[i]);
                        break;
                    }
                }
                AssetDatabase.Refresh();
            });
        }
        
        private void CreatClass(ExcelTable excelTable)
        {
            Assembly ass = Assembly.LoadFile(assembly);

            for (int i = startRow; i <= excelTable.NumberOfRows; i++)
            {
                string className = excelTable.GetValue(i, typeColumn).ToString();
                if (!className.IsNullOrEmpty())
                {
                    Type type = ass.GetType(className);
                    if (type == null)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append($"public class {className} : PropsBase\n");
                        builder.Append("{\n");
                        builder.Append("}\n");
                        if (!Directory.Exists(csFoder))
                        {
                            Directory.CreateDirectory(csFoder);
                        }

                        using (StreamWriter writer = new StreamWriter(csFoder + className + ".cs"))
                        {
                            writer.Write(builder);
                        }
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        private void ReadTable(ExcelTable excelTable)
        {
            Assembly ass = Assembly.LoadFile(assembly);
            for (int i = startRow; i <= excelTable.NumberOfRows; i++)
            {
                string prefabName = excelTable.GetValue(i, prefabColumn).ToString();

                string artName = excelTable.GetValue(i, artColumn).ToString();
                if (prefabName.IsNullOrEmpty() || artName.IsNullOrEmpty()) continue;
                string typeName = excelTable.GetValue(i, typeColumn).ToString();
                int id = excelTable.GetValue(i, 1).ToInt();
                Type ty = ass.GetType(typeName);
                if (ty == null)
                {
                    GameDebug.LogError($"{typeName}这个类没有生成");
                    continue;
                }
                CreatPrefab(prefabName, artName.Split('|'), ty, id);
            }
        }

        private void CreatPrefab(string prefabName,string[] artPrefab,Type type,int id)
        {
            string watchPath = Pathelper.FullAssetPath(prefabFolder[1]);
            string prefabPath = Pathelper.FullAssetPath(prefabFolder[0] + prefabName + ".prefab");
            if (!File.Exists(watchPath) || !File.Exists(prefabPath))
            {
                GameObject go = new GameObject(prefabName);
                for (int i = 0; i < artPrefab.Length; i++)
                {
                    Object artPreab = null;
                    for (int j = 0; j < artFolder.Count; j++)
                    {
                        if (artPreab == null)
                        {
                            artPreab = AssetDatabase.LoadMainAssetAtPath(artFolder[j] + artPrefab[i] + ".FBX");
                        }
                    }

                    if (artPreab == null)
                    {
                        GameDebug.LogError($"找不到美术资源{artPrefab[i]}");
                        continue;
                    }

                    GameObject artFbx = PrefabUtility.InstantiatePrefab(artPreab) as  GameObject;
                    artFbx.transform.SetParent(go.transform);
                    artFbx.transform.position = go.transform.position;
                    artFbx.transform.eulerAngles = go.transform.eulerAngles;
                }
                
                if (!File.Exists(watchPath))
                {
                    PrefabUtility.SaveAsPrefabAsset(go, prefabFolder[1] + prefabName + ".prefab");
                }

                if (!File.Exists(prefabPath))
                {
                    PropsBase props = go.AddComponent(type) as PropsBase;
                    GameObject lookPoint = new GameObject("LookPoint");
                    LookPoint look = lookPoint.AddComponent<LookPoint>();

                    look.AddMeshRender();
            
                    look.transform.SetParent(props.transform);
                    look.transform.position = props.transform.position;
                    look.transform.eulerAngles = props.transform.eulerAngles;


                    go.transform.SetLayer(15);
                    GameObject prefb = PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                }
                DestroyImmediate(go);
            }
        }
    }
}