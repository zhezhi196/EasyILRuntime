using System;
using System.IO;
using UnityEngine;

namespace Module
{
    public class MatLoader: ScriptableObject
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void Check()
        {
            DirectoryInfo dir = new DirectoryInfo($"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Material)}");
            if (dir.Exists)
            {
                FileInfo[] files = dir.GetFiles("*.asset");
                for (int i = 0; i < files.Length; i++)
                {
                    string path = Pathelper.GetReleativePath(files[i].FullName);
                    MatLoader asset = UnityEditor.AssetDatabase.LoadAssetAtPath<MatLoader>(path);
                    if (asset.material == null)
                    {
                        Material copy = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(asset.matpath);
                        if (copy == null)
                        {
                            UnityEditor.AssetDatabase.DeleteAsset(path);
                        }
                        else
                        {
                            asset.material = copy;
                        }
                    }
                }
            }
        }
        [UnityEditor.MenuItem("Assets/Create/HZZ/到动态材质")]
        private static void CreatEditor()
        {
            var exam = CreateInstance<MatLoader>();
            Material mat = UnityEditor.Selection.activeObject as Material;
            if (mat != null)
            {
                string folderPath = $"Assets/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Material)}";
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                exam.material = mat;
                exam.matpath = UnityEditor.AssetDatabase.GetAssetPath(mat);
                string createPath = $"{folderPath}/{mat.name}.asset";
                if (!File.Exists(Pathelper.FullAssetPath(createPath)))
                {
                    UnityEditor.AssetDatabase.CreateAsset(exam, createPath);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                }
                else
                {
                    GameDebug.LogError("文件已存在");
                }
            }            
        }
        [SerializeField]
        private string matpath;
#endif

        public Material material;

        public static void LoadMaterial(string name, Action<Material> callback)
        {
            string path = ConstKey.GetFolder(AssetLoad.AssetFolderType.Material);
            AssetLoad.PreloadAsset<MatLoader>($"{path}/{name}.asset", hadle =>
            {
                callback?.Invoke(hadle.Result.material);
                AssetLoad.Release(hadle);
            });
        }
    }
}