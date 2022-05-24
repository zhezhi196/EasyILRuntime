/*
 * 脚本名称：HzzTools
 * 项目名称：ugui
 * 脚本作者：黄哲智
 * 创建时间：2017-12-26 11:23:22
 * 脚本作用：
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Module;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace EditorModule
{
    public class HzzTools : Editor
    {
        [MenuItem("Tools/打开缓存")]
        public static void OpenCache()
        {
            Process.Start(Application.persistentDataPath);
        }
        [MenuItem("Tools/程序工具/删除(Clone)")]
        public static void CheckPrefab()
        {
            var tar = Selection.objects;
            for (int i = 0; i < tar.Length; i++)
            {
                if (tar[i] is GameObject go)
                {
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(go), go.name.Replace("(Clone)", String.Empty));
                }
            }
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/清除本地缓存")]
        public static void ClearLocalData()
        {
            File.Delete(Application.dataPath+"/Data.db");
            LocalFileMgr.RemoveAllKey();
            Debug.Log("清除完成");
        }

        
        [MenuItem("Tools/策划工具/打开Excel目录")]
        public static void OpenExcelFoder()
        {
            string path = Application.dataPath + "/../Excel";
            Process.Start(path);
        }
        
        [MenuItem("Tools/程序工具/创建UI")]
        public static void CreatUI()
        {
            Object select = Selection.activeObject;
            if(select==null) return;
            string scriptPath = $"{Application.dataPath}/GameProject/UI/{select.name}";
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append("using Module;\n");
            builder.Append("namespace ProjectUI\n");
            builder.Append("{\n");
            builder.Append($"    public class {select.name} : UIViewBase\n");
            builder.Append("    {\n");
            builder.Append("    }\n");
            builder.Append("}\n");

            using (StreamWriter streamWriter = new StreamWriter(scriptPath+"\\" + select.name + ".cs"))
            {
                streamWriter.Write(builder.ToString());
            }
            
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/程序工具/设置所有tag")]
        public static void SetTag()
        {
            GameObject ob = Selection.activeGameObject;
            ob.transform.SetTag(ob.tag, true);
        }

        [MenuItem("Tools/程序工具/行为树打不开请点这个")]
        public static void BehaviorClear()
        {
            EditorPrefs.DeleteAll();
        }

        [MenuItem("Tools/程序工具/刷新资源 _F5")]
        static void EditorCustorkKeys1()
        {
            AssetDatabase.Refresh();
        }

        #region 删除空文件夹

        [MenuItem("Tools/程序工具/删除空文件夹")]
        public static void ClearEmptyFold()
        {
            DirectoryInfo directory = Directory.CreateDirectory(Application.dataPath);
            int count = 0;
            while (true)
            {
                int temp = CheckDir(directory);
                count += temp;
                if (temp == 0)
                {
                    break;
                }
            }

            Debug.Log("核查结束,总共删除空文件夹: " + count);
            AssetDatabase.Refresh();
        }

        private static int CheckDir(DirectoryInfo info)
        {
            if (info == null) return 0;
            int count = 0;
            FileSystemInfo[] dir = info.GetFileSystemInfos();
            if (dir.Length == 0)
            {
                Debug.Log(string.Format("已删除{0}空文件夹", info.FullName));
                info.Delete();
                FileInfo meta = new FileInfo(info.FullName + ".meta");
                meta.Delete();
                count++;
            }
            else
            {
                for (int i = 0; i < dir.Length; i++)
                {
                    count += CheckDir(dir[i] as DirectoryInfo);
                }
            }

            return count;
        }
    }

    #endregion

}
