using System;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorModule
{
    public class CodeStatistics: OdinEditorWindow
    {
        [MenuItem("Tools/策划工具/代码统计")]
        public static void Open()
        {
            GetWindow<CodeStatistics>();
        }

        public bool allCode;
        [HideIf("allCode")]
        public string[] path;

        protected override void OnEnable()
        {
            base.OnEnable();
            path = new[] {Application.dataPath + "/Chapter2", Application.dataPath + "/Module",Application.dataPath+"/Editor/HZZTools",Application.dataPath+"/Editor/Peiyalong"};
        }

        [Button]
        public void GetCodeLine()
        {
            long totalLine = 0;
            if (!allCode)
            {
                for (int i = 0; i < path.Length; i++)
                {
                    DirectoryInfo dir = Directory.CreateDirectory(path[i]);
                    totalLine += GetLine(dir);
                }
            }
            else
            {
                DirectoryInfo dir = Directory.CreateDirectory(Application.dataPath);
                totalLine += GetLine(dir);
            }

            EditorUtility.DisplayDialog("代码统计", "代码总计: " + totalLine, "关闭");
        }

        private long GetLine(DirectoryInfo dir)
        {
            long totalLine = 0;
            FileInfo[] files = dir.GetFiles("*.cs");
            for (int i = 0; i < files.Length; i++)
            {
                totalLine += GetLine(files[i]);
            }
            
            DirectoryInfo[] child = dir.GetDirectories();
            for (int i = 0; i < child.Length; i++)
            {
                totalLine += GetLine(child[i]);
            }

            return totalLine;
        }

        private long GetLine(FileInfo file)
        {
            long res = 0;
            var reader = file.OpenText();
            while (reader.ReadLine() != null)
            {
                res++;
            }

            return res;
        }
    }
}