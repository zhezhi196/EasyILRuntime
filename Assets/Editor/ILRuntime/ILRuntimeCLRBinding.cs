#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using ILRuntime.Runtime.Intepreter;
using Module;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.UI;
using xasset;

[Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    //[MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream dllFs = new System.IO.FileStream("Assets/Bundles/Code/Hotfix.dll.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            appdomain.LoadAssembly(dllFs);
            ILBindingHelper.Binding(appdomain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(appdomain, "Assets/Module/ILRuntime/Generated");
            AssetDatabase.Refresh();
        }
    }
}
#endif
