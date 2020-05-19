using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Module;
using MongoDB.Bson;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEditor;
using UnityEngine;

public struct CellInfo
{
    public string Type;
    public string Name;
    public string Desc;
}

public class ExcelMD5Info
{
    public Dictionary<string, string> fileMD5 = new Dictionary<string, string>();

    public string Get(string fileName)
    {
        string md5 = "";
        this.fileMD5.TryGetValue(fileName, out md5);
        return md5;
    }

    public void Add(string fileName, string md5)
    {
        this.fileMD5[fileName] = md5;
    }
}

public class ExcelExporterEditor: EditorWindow
{


    private const string ExcelPath = "/../Excel";

    private static ExcelMD5Info md5Info;

    private static int firstRow = 0;

    [MenuItem("Setting/导出Excel配置")]
    private static void ShowWindow()
    {
        ExportAll("/Bundles/Config/Tables");

        //ExportAllClass(@"./Assets/Model/Config/Config", "namespace ETModel\n{\n");
        ExportAllClass(@"/Hotfix/Config/", "using Module;\n\nnamespace HotFix\n{\n");

        Debug.Log($"导出客户端配置完成!");
    }
    
    private static  void ExportAllClass(string exportDir, string csHead)
    {
        firstRow = 0;
        md5Info = null;
        string[] ff = Directory.GetFiles(Application.dataPath + ExcelPath);
        foreach (string filePath in ff)
        {
            if (Path.GetExtension(filePath) != ".xlsx")
            {
                continue;
            }

            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }

            ExportClass(filePath, exportDir, csHead);
            Debug.Log($"生成{Path.GetFileName(filePath)}类");
        }

        AssetDatabase.Refresh();
    }

    private static void ExportClass(string fileName, string exportDir, string csHead)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }

        string protoName = Path.GetFileNameWithoutExtension(fileName);

        string exportPath = Path.Combine(Application.dataPath+exportDir, $"{protoName}.cs");
        using (FileStream txt = new FileStream(exportPath, FileMode.Create))
        using (StreamWriter sw = new StreamWriter(txt))
        {
            StringBuilder sb = new StringBuilder();
            ISheet sheet = xssfWorkbook.GetSheetAt(0);
            sb.Append(csHead);
            sb.Append($"\tpublic class {protoName}: IDataBase\n");
            sb.Append("\t{\n");

            int cellCount = sheet.GetRow(3).LastCellNum;

            for (int i = firstRow; i < cellCount; i++)
            {
                string fieldDesc = GetCellString(sheet, 2, i);

                if (fieldDesc.StartsWith("#"))
                {
                    continue;
                }

                string fieldName = GetCellString(sheet, 1, i);

                if (fieldName == "id" || fieldName == "_id" || fieldName == "Id")
                {
                    continue;
                }

                string fieldType = GetCellString(sheet, 2, i);
                if (fieldType == "" || fieldName == "")
                {
                    continue;
                }

                sb.Append($"\t\tpublic {fieldType} {fieldName};\n");
            }

            sb.Append("\t}\n");
            sb.Append("}\n");

            string str = sb.ToString();
            Debug.Log(str);
            sw.Write(sb.ToString());
        }
    }

    private static void ExportAll(string exportDir)
    {
        string md5File = Path.Combine(Application.dataPath+ExcelPath, "md5.txt");
        if (!File.Exists(md5File))
        {
            md5Info = new ExcelMD5Info();
        }
        else
        {
            md5Info = MongoHelper.FromJson<ExcelMD5Info>(File.ReadAllText(md5File));
        }

        string[] files = Directory.GetFiles(Application.dataPath+ExcelPath);

        foreach (string filePath in files)
        {
            if (Path.GetExtension(filePath) != ".xlsx")
            {
                continue;
            }

            if (Path.GetFileName(filePath).StartsWith("~"))
            {
                continue;
            }

            string fileName = Path.GetFileName(filePath);
            string oldMD5 = md5Info.Get(fileName);

            string md5 = EncryptionHelper.GetMD5(filePath);
            md5Info.Add(fileName, md5);


            Export(filePath, exportDir);
        }

        File.WriteAllText(md5File, md5Info.ToJson());

        Debug.Log("所有表导表完成");
        AssetDatabase.Refresh();
    }

    private static void Export(string fileName, string exportDir)
    {
        XSSFWorkbook xssfWorkbook;
        using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            xssfWorkbook = new XSSFWorkbook(file);
        }

        string protoName = Path.GetFileNameWithoutExtension(fileName);
        Debug.Log($"{protoName}导表开始");
        string exportPath = Path.Combine(Application.dataPath + exportDir, $"{protoName}.json");

        using (StreamWriter writer = new StreamWriter(exportPath))
        {
            if (xssfWorkbook.NumberOfSheets > 0)
            {
                ISheet sheet = xssfWorkbook.GetSheetAt(0);
                //sw.Write(ExportSheet(sheet));
                string content = ExportSheet(sheet);

                writer.Write(content);
            }
        }

        Debug.Log($"{protoName}导表完成");
    }

    private static string ExportSheet(ISheet sheet)
    {
        int cellCount = sheet.GetRow(3).LastCellNum;

        CellInfo[] cellInfos = new CellInfo[cellCount];

        for (int i = firstRow; i < cellCount; i++)
        {
            string fieldDesc = GetCellString(sheet, 0, i);
            string fieldName = GetCellString(sheet, 1, i);
            string fieldType = GetCellString(sheet, 2, i);
            cellInfos[i] = new CellInfo() { Name = fieldName, Type = fieldType, Desc = fieldDesc };
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 3; i <= sheet.LastRowNum; ++i)
        {
            if (GetCellString(sheet, i, 2) == "")
            {
                continue;
            }

            sb.Append("{");
            IRow row = sheet.GetRow(i);
            for (int j = firstRow; j < cellCount; ++j)
            {
                string desc = cellInfos[j].Desc.ToLower();

                if (desc.StartsWith("#"))
                {
                    continue;
                }
                
                string fieldValue = GetCellString(row, j);
                if (fieldValue == "")
                {
                    Debug.LogWarning($"sheet: {sheet.SheetName} 中有空白字段 {i}行,{j}列");
                }

                if (j > firstRow)
                {
                    sb.Append(",");
                }

                string fieldName = cellInfos[j].Name;
                Debug.Log(j + "列|" + i + "行->" + fieldName);

                if (fieldName == "Id" || fieldName == "_id")
                {
                    fieldName = "Id";
                }

                string fieldType = cellInfos[j].Type;
                string type = Convert(fieldType, fieldValue);
                if (type == null)
                {
                    return "";
                }

                sb.Append($"\"{fieldName}\":{type}");
            }

            sb.Append("}");
            if (i != sheet.LastRowNum)
            {
                sb.AppendLine();
            }
        }
        return sb.ToString();
    }

    private static string Convert(string type, string value)
    {
        switch (type)
        {
            case "int[]":
            case "int32[]":
            case "long[]":
                return $"[{value}]";
            case "string[]":
                return $"[{value}]";
            case "int":
            case "int32":
            case "int64":
            case "long":
            case "float":
            case "double":
                return value;
            case "string":
                return $"\"{value}\"";
            default:
                return null;
                throw new Exception($"不支持此类型: {type}");
        }
    }

    private static string GetCellString(ISheet sheet, int i, int j)
    {
        return sheet.GetRow(i)?.GetCell(j)?.ToString() ?? "";
    }

    private static string GetCellString(IRow row, int i)
    {
        return row?.GetCell(i)?.ToString() ?? "";
    }

    private static string GetCellString(ICell cell)
    {
        return cell?.ToString() ?? "";
    }
}