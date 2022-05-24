using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LitJson;
using Module;
using SDK;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using AnimationInfo = Module.AnimationInfo;
using Tools = Module.Tools;

namespace EditorModule
{
    [Serializable]
    public struct AnalysticPlantformInfo
    {        
        [HideLabel,HorizontalGroup()]
        public E_AnalyticsType plantform;
        [HideLabel,HorizontalGroup()]
        public int column;
    }
    public class AnalyticsWindow : OdinEditorWindow
    {
        [MenuItem("Tools/策划工具/导入打点数据")]
        public static void OpenWindow()
        {
            GetWindow<AnalyticsWindow>();
        }


        [LabelText("说明")] public string Illustrate = "说明";

        [LabelText("开始行")] public int start = 3;
        [LabelText("类型列")] public int typeClome = 8;
        [LabelText("目标ID列")] public int targetId = 3;
        [LabelText("关卡ID列")] public int missionId = 4;
        [LabelText("最大发送次数列")] public int maxSendCount = 5;

        [LabelText("渠道列")] public AnalysticPlantformInfo[] tarIndex = new[]
        {
            new AnalysticPlantformInfo() {plantform = E_AnalyticsType.GameAnalytics, column = 6},
            new AnalysticPlantformInfo() {plantform = E_AnalyticsType.Adjust, column = 7}
        };

        protected override void OnEnable()
        {
            start = 3;
            typeClome = 8;
            targetId = 3;
            missionId = 4;
            maxSendCount = 5;
            Illustrate = "说明";
            tarIndex = new AnalysticPlantformInfo[2];
            tarIndex[0] = new AnalysticPlantformInfo() {plantform = E_AnalyticsType.GameAnalytics, column = 6};
            tarIndex[1] = new AnalysticPlantformInfo() {plantform = E_AnalyticsType.Adjust, column = 7};
            base.OnEnable();
        }

        [Button("导入Excel")]
        public void SelectFile()
        {
            FolderBrowserHelper.SelectFile(st =>
            {
                string copypath = $"{Application.persistentDataPath}/工具/Analytics.temp";

                File.Copy(st, copypath, true);
                Excel exce = ExcelHelper.LoadExcel(copypath);
                for (int i = 0; i < exce.Tables.Count; i++)
                {
                    var table = exce.Tables[i];
                    if (table.TableName != Illustrate)
                    {
                        CreatText(table);
                    }
                }

                File.Delete(copypath);
                AssetDatabase.Refresh();
            });
        }

        private void CreatText(ExcelTable table)
        {
            string path = $"{Application.dataPath}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Bundle)}/{Analytics.GetOutPutPath(table.TableName.ToEnum<ChannelType>())}";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            for (int i = 0; i < tarIndex.Length; i++)
            {
                AnalyticsPlantform result = new AnalyticsPlantform();
                List<AnalyticsInfo> infoList = new List<AnalyticsInfo>();
                result.type = tarIndex[i].plantform;
                var eventId = tarIndex.Find(fd => fd.plantform == result.type).column;
                for (int j = start; j <= table.NumberOfRows; j++)
                {
                    if (!table.GetValue(j, 1).ToString().IsNullOrEmpty() && !table.GetValue(j, eventId).ToString().IsNullOrEmpty())
                    {
                        AnalyticsInfo info = new AnalyticsInfo();
                        info.eventID = table.GetValue(j, eventId).ToString();
                        info.type = table.GetValue(j, typeClome).ToInt();
                        info.targetID = table.GetValue(j, targetId).ToString();
                        info.maxSendCount = table.GetValue(j, maxSendCount).ToInt();
                        info.MissionID = table.GetValue(j, missionId).ToInt();
                        if (info.maxSendCount == 0) info.maxSendCount = -1;
                        infoList.Add(info);
                    }
                }
                result.info = infoList.ToArray();
                string json = JsonMapper.ToJson(result);
                using (StreamWriter writer = new StreamWriter($"{path}/{result.type}.json", false, Encoding.UTF8))
                {
                    writer.Write(json);
                }
            }
        }
    }
}