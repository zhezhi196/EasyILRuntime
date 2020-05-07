using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Module;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIViewReference))]
public class UIViewRefrenceEditor : ViewReferenceEditor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Creat"))
        {
            Creat();
        }
    }
    

    public void Creat()
    {
        GameObject go = ((UIViewReference) target).gameObject;
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

        
        string outputPath = $"{Application.dataPath}/HotFix/UI/{go.name}/{ctrlName}.cs";
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

        string configcode = "";
        using (StreamReader reader = new StreamReader($"{Application.dataPath}/HotFix/UI/UIConfig.cs"))
        {
            string code = reader.ReadToEnd();
            string s = code.Replace("}", "");
            StringBuilder b = new StringBuilder(s);
            b.Append(
                $"\t\tpublic static UIType {go.name} = new UIType(typeof({ctrlName}), typeof({modulName}), typeof({viewName}),\"{go.name}\");\n");
            b.Append("\t}\n");
            b.Append("}\n");
            configcode = b.ToString();
        }
        using (StreamWriter writer = new StreamWriter($"{Application.dataPath}/HotFix/UI/UIConfig.cs"))
        {
            writer.Write(configcode);
        }
        
        //go.AddComponent<ViewReference>().targetType = viewName;
        AssetDatabase.Refresh();
    }
    
}
