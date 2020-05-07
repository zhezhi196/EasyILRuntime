using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Module;
using Debug = UnityEngine.Debug;

[InitializeOnLoad]
public class UnityStart
{
    private const string ScriptAssembliesDir = "Library/ScriptAssemblies";
    private const string CodeDir = "Assets/Bundles/Code/";
    private const string HotfixDll = "Hotfix.dll";
    private const string HotfixPdb = "Hotfix.pdb";
    public static List<string> hotFixType = new List<string>();

    static UnityStart()
    {
        
        var dllPath = Path.Combine(ScriptAssembliesDir, HotfixDll);
        if (File.Exists(dllPath))
        {

            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixDll), Path.Combine(CodeDir, "Hotfix.dll.bytes"), true);
        }

        dllPath = Path.Combine(ScriptAssembliesDir, HotfixPdb);
        if (File.Exists(dllPath))
        {
            File.Copy(Path.Combine(ScriptAssembliesDir, HotfixPdb), Path.Combine(CodeDir, "Hotfix.pdb.bytes"), true);
        }


        
        Debug.Log($"复制Hotfix.dll, Hotfix.pdb到{CodeDir}完成");
        AssetDatabase.Refresh();
        hotFixType.Clear();

        Type[] t = Assembly.Load("HotFix").GetTypes();

        for (int i = 0; i < t.Length; i++)
        {
            if (t[i].Namespace == "HotFix")
            {
                {
                    if (t[i].GetRoot().FullName == "Module.ViewBehaviour" && !t[i].IsAbstract)
                    {
                        hotFixType.Add(t[i].Name);
                    }
                }
            }

            hotFixType.Sort();
        }
    }
}

