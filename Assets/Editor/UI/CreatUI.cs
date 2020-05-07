﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Module;
using UnityEditor;
using UnityEngine;

public class CreatUI : Editor
{
    [MenuItem("Tools/CreatUI")]
    public static void Creat()
    {
        GameObject go = (GameObject) Selection.objects[0];
        string baseName = go.name;
        string modulName = baseName + "Modul";
        string viewName = baseName + "View";
        string ctrlName = baseName + "Ctrl";

        StringBuilder builder = new StringBuilder();
        builder.Append("namespace HotFix\n{\n");
        builder.Append($"\tpublic class {viewName} : UIView\n");
        builder.Append("\t{\n\t}\n");
        builder.Append($"\tpublic class {modulName} : UIModul\n");
        builder.Append("\t{\n\t}\n");
        builder.Append($"\tpublic class {ctrlName} : UICtrl<{modulName}, {viewName}>\n");
        builder.Append("\t{\n\t}\n}\n");

        
        string outputPath = $"{Application.dataPath}/HotFix/UI/{go.name}/UI{ctrlName}.cs";
        if (!Directory.Exists($"{Application.dataPath}/HotFix/UI/{go.name}/"))
        {
            Directory.CreateDirectory($"{Application.dataPath}/HotFix/UI/{go.name}/");
        }
        if (File.Exists(outputPath))
        {
            Debug.LogError($"{outputPath}有文件");
            return;
        }
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            writer.Write(builder.ToString());
        }

        go.AddComponent<ViewReference>().targetType = viewName;
        AssetDatabase.Refresh();
    }
}
