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
using System.Reflection;
using System.Text;
using System.Threading;
using Module;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace EditorModule
{
    public class HzzTools : Editor
    {
        [MenuItem("Tools/策划工具/一键解锁所有物品")]
        public static void UnlockProps()
        {
            for (int i = 0; i < PropEntity.entityList.Count; i++)
            {
                PropEntity.entityList[i].Unlock();
            }
        }
        [MenuItem("Tools/策划工具/一键解锁所有成就")]
        public static void UnlockAchievement()
        {
            AchievementManager.Instance.GMUnlockAchievement();
        }
        [MenuItem("Tools/策划工具/一键解锁所有难度")]
        public static void UnlockDifficulty()
        {
            foreach (var mission in Mission.missionList)
            {
                mission.RunningMission();
            }
        }
        // [MenuItem("Tools/删除sceneLayer")]
        // public static void RemoveSceneLayer()
        // {
        //     for (int i = 0; i < Selection.objects.Length; i++)
        //     {
        //         GameObject go = Selection.objects[i] as GameObject;
        //         SceneLayer[] layer = go.transform.GetComponentsInChildren<SceneLayer>(true);
        //         for (int j = 0; j < layer.Length; j++)
        //         {
        //             GameObject.DestroyImmediate(layer[j]);
        //         }
        //
        //         EditorUtility.SetDirty(go);
        //     }
        //     AssetDatabase.Refresh();
        // }

        [MenuItem("Tools/程序工具/查找遮挡剔除")]
        public static void SearchOcc()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                PropsCreator creator = (Selection.objects[i] as GameObject).GetComponent<PropsCreator>();
                OcclusionPortal[] occ = GameObject.FindObjectsOfType<OcclusionPortal>();
                for (int j = 0; j < occ.Length; j++)
                {
                    if (occ[j].transform.position.Distance(creator.transform.position) <= 5)
                    {
                        creator.sceneObject.Add(occ[j].transform.GetPathInScene());
                        break;
                    }
                }

                EditorUtility.SetDirty(Selection.objects[i]);
            }

        }
        [MenuItem("Tools/程序工具/添加bilibili")]
        public static void Addbili()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                GameObject go = Selection.objects[i] as GameObject;
                Renderer[] render = go.transform.GetComponentsInChildren<Renderer>(true);
                for (int j = 0; j < render.Length; j++)
                {
                    for (int k = 0; k < render[j].sharedMaterials.Length; k++)
                    {
                        if (render[j].sharedMaterials[k].shader.name == "Shader Graphs/DaojuGuang"||render[j].sharedMaterials[k].shader.name =="Shader Graphs/DaojuGuang_no_n")
                        {
                            BilingBiling bi = render[j].gameObject.AddOrGetComponent<BilingBiling>();
                            bi.field = "Vector1_F4E3D5DD";
                            bi.renderTar = render[j];
                            bi.biliTime = 3;
                            bi.maxValue = 5;
                            break;
                        }
                        else
                        {
                            BilingBiling[] exitBilibili =render[j].transform.GetComponentsInChildren<BilingBiling>(true);
                            for (int l = 0; l < exitBilibili.Length; l++)
                            {
                                Debug.Log(go,exitBilibili[l].gameObject);
                            }
                        }

                    }
                }
                EditorUtility.SetDirty(go);
            }
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/程序工具/删除bilibili")]
        public static void Removebili()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                GameObject go = Selection.objects[i] as GameObject;
                BilingBiling[] BI = go.transform.GetComponentsInChildren<BilingBiling>(true);
                for (int j = 0; j < BI.Length; j++)
                {
                    DestroyImmediate(BI[j]);
                    
                }
        
                EditorUtility.SetDirty(go);
            }
            AssetDatabase.Refresh();
        }
        
        [MenuItem("Tools/程序工具/设置光照探针")]
        public static void SetLightProbe()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                GameObject go = Selection.objects[i] as GameObject;
                Renderer[] render = go.transform.GetComponentsInChildren<Renderer>(true);
                for (int j = 0; j < render.Length; j++)
                {
                    (render[i] as MeshRenderer).probeAnchor = go.transform;
                }
                EditorUtility.SetDirty(go);
            }
            AssetDatabase.Refresh();
        }
        
        // [MenuItem("Tools/程序工具/碰撞体 #T")]
        // public static void OpenConsole()
        // {
        //     // GameObject go = Selection.activeObject as GameObject;
        //     // Collider collider = go.transform.GetChild(1).GetComponent<Collider>();
        //     // ComponentUtility.CopyComponent(collider);
        //     // ComponentUtility.PasteComponentAsNew(go.transform.GetChild(0).gameObject);
        //     // var ss = go.transform.GetChild(0).gameObject.AddOrGetComponent<SceneLayer>();
        //     // ss.hurtMaterial = Hurtmaterial.Stone;
        //     // go.transform.SetLayer("Wall", true);
        //     //
        //     //EditorUtility.SetDirty(go);
        //     Collider[] collider = GameObject.FindObjectsOfType<Collider>();
        //     for (int i = 0; i < collider.Length; i++)
        //     {
        //         GameObject go = collider[i].gameObject;
        //         SceneLayer layer = go.AddOrGetComponent<SceneLayer>();
        //         layer.hurtMaterial = Hurtmaterial.Stone;
        //         // SceneLayer[] layer = collider[i].gameObject.GetComponents<SceneLayer>();
        //         // if (layer.Length > 1)
        //         // {
        //         //     for (int j = 0; j < layer.Length; j++)
        //         //     {
        //         //         DestroyImmediate(layer[j]);
        //         //     }
        //         // }
        //     
        //         EditorUtility.SetDirty(go);
        //     }
        // }
        //
        // [MenuItem("Tools/程序工具/碰撞体修改 #A")]
        // public static void AddCollider()
        // {
        //     for (int i = 0; i < Selection.objects.Length; i++)
        //     {
        //         GameObject go=Selection.objects[i] as GameObject;
        //         go.AddOrGetComponent<SceneLayer>().hurtMaterial = Hurtmaterial.Stone;
        //         go.AddOrGetComponent<MeshCollider>();
        //     }
        // }
        //
        
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
            if (EditorUtility.DisplayDialog("清除存档", "是否确认清除存档？", "确认", "我是误触"))
            {
                var allDelete = EditorUtility.DisplayDialog("删除数据库？", "是否完全删除数据库？", "全删除", "不删数据库");
                //删除存档文件
                LocalSaveFile.GmDeleteAllFile(allDelete);
                //清除PlayerPrefs
                LocalFileMgr.RemoveAllKey();
                Debug.Log("清除完成");
            }
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
            if (EditorUtility.DisplayDialog("提示", "拉取git到最新?", "确定", "取消"))
            {
                string path = Application.dataPath+"/../../gitUpdate.bat";
                string excelPatrh = Application.dataPath + "/../Excel/Config.xlsx";
                string dbPatrh = Application.dataPath + "/../DbData/Config.db";
                try
                {
                    using (var file = File.Open(excelPatrh, FileMode.Open))
                    {
                    }
                    
                    using (var file = File.Open(dbPatrh, FileMode.Open))
                    {
                    }
                    
                    Process.Start(path);
                }
                catch (Exception e)
                {
                    EditorUtility.DisplayDialog("错误", "清先关闭excel或数据库", "确定");
                }
            }
            else
            {
                AssetDatabase.Refresh();
            }
            //
        }

        [MenuItem("Tools/程序工具/清除控制台日志 _F1")]
        static void EditorClearConsoleLog()
        {
            var logEntries = typeof(EditorWindow).Assembly.GetType("UnityEditor.LogEntries");
            var clearMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }

        [MenuItem("Tools/获取所有武器皮肤")]
        static void GetAllWeaponSkin ()
        {
            if (!Application.isPlaying)
                return;
            foreach (var item in WeaponManager.allSkins)
            {
                item.Value.Acquire();
            }
            GameDebug.Log("获取所有武器皮肤");
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
