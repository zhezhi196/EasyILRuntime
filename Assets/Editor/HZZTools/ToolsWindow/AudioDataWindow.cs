using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class AudioDataWindow : OdinMenuEditorWindow
{
    public AudioDataSO data;
    [MenuItem("Tools/策划工具/音效管理")]
    private static void ShowWindow()
    {
        var window = GetWindow<AudioDataWindow>();
        window.titleContent = new GUIContent("音效管理");
        window.Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        data = AssetDatabase.LoadAssetAtPath<AudioDataSO>("Assets/Bundles/AudioData/AudioData.asset");
        tree.Add("AudioData",data);
        MenuWidth = 0;
        return tree;
    }

    protected override void OnBeginDrawEditors()
    {
        SirenixEditorGUI.BeginToolbarBox();
        if (SirenixEditorGUI.ToolbarButton("从Excel导入数据"))
        {
            ImportFromExcel();
        }
        SirenixEditorGUI.EndToolbarBox();
        
        
        base.OnBeginDrawEditors();
        
    }



    public void ImportFromExcel()
    {
        DirectoryInfo path = new DirectoryInfo(Application.dataPath);
        string p = path.Parent.FullName + "/Excel";
        string excelPath = EditorUtility.OpenFilePanel("选择excel",p,"xlsx");
        if (!excelPath.Contains("Audio"))
        {
            return;
        }

        var originData = data.units;
        
        data.units = new List<AudioDataUnit>();
        var tables = GetAllTableName(excelPath);
        var table = tables[0];
        for (int i = 0; i < table.NumberOfRows; i++)
        {
            if (i <= 1)
            {
                continue;
            }
            var find = data.units.Find((v) => v.name == table.GetCell(i, 5).Value);
            if (find == null)
            {
                if (string.IsNullOrEmpty(table.GetCell(i, 1).Value))
                {
                    continue;
                }
                var unit = new AudioDataUnit();
                var value2 = table.GetCell(i, 2);
                var value3 = table.GetCell(i, 3);
                var value4 = table.GetCell(i, 4);
                var value5 = table.GetCell(i, 5);
                var value6 = table.GetCell(i, 6);
                var value7 = table.GetCell(i, 7);

                unit.isBGM = value3.Value == "0" ? false : true;
                unit.rule = (AudioRandomRules)int.Parse(value2.Value);
                unit.mixerName = value4.Value;
                unit.name = value5.Value;
                unit.paths = new List<AudioDataUnitPath>();
                AudioDataUnitPath unitPath = new AudioDataUnitPath();
                unitPath.path = value6.Value;
                
                if (string.IsNullOrEmpty(value7.Value))
                {
                    for (int j = 0; j < originData.Count; j++)
                    {
                        for (int k = 0; k < originData[j].paths.Count; k++)
                        {
                            if (originData[j].paths[k].path == value6.Value)
                            {
                                unitPath.lowpassRange = originData[j].paths[k].lowpassRange;
                            }
                        }
                    }
                }
                else
                {
                    string[] value6List = value7.Value.Split(',');
                    unitPath.lowpassRange = new Vector2Int(int.Parse(value6List[0]), int.Parse(value6List[1]));
                }
                unit.paths.Add(unitPath);
                data.units.Add(unit);
            }
            else
            {
                var value6 = table.GetCell(i, 6);
                var value7 = table.GetCell(i, 7);
                
                AudioDataUnitPath unitPath = new AudioDataUnitPath();
                unitPath.path = value6.Value;

                if (string.IsNullOrEmpty(value7.Value))
                {
                    for (int j = 0; j < originData.Count; j++)
                    {
                        for (int k = 0; k < originData[j].paths.Count; k++)
                        {
                            if (originData[j].paths[k].path == value6.Value)
                            {
                                unitPath.lowpassRange = originData[j].paths[k].lowpassRange;
                            }
                        }
                    }
                }
                else
                {
                    string[] value6List = value7.Value.Split(',');
                    unitPath.lowpassRange = new Vector2Int(int.Parse(value6List[0]), int.Parse(value6List[1]));
                }

                find.paths.Add(unitPath);
            }
        }
        File.Delete(tempPath);
    }

    private string tempPath = "";
    private ExcelTable[] GetAllTableName(string path)
    {
        FileInfo file = new FileInfo(path);
        tempPath = path + ".temp";
        file.CopyTo(tempPath);
        Excel xls = ExcelHelper.LoadExcel(tempPath);
        return xls.Tables.ToArray();
    }
}
