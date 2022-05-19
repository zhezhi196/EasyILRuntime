using System;
using System.Collections.Generic;
using LitJson;
using SDK;
using UnityEngine;

namespace Module
{
    public static class Analytics
    {
        public static List<AnalyticsPlantform> plantForms = new List<AnalyticsPlantform>();
        public static bool IsLog = true;

        public static string outPutPath
        {
            get { return GetOutPutPath(Config.globleConfig.channel); }
        }

        public static string GetOutPutPath(ChannelType channel)
        {
            return $"{ConstKey.GetChannelConfig(channel)}/{ConstKey.GetFolder(AssetLoad.AssetFolderType.Analytics)}";
        }

        public static AsyncLoadProcess Init(AsyncLoadProcess process)
        {
            process.IsDone = false;
            var analysticType=Config.globleConfig.analystic.Find(fd => fd.channel == Config.globleConfig.channel).analystic;
            if (!analysticType.IsNullOrEmpty())
            {
                Voter voter = new Voter(analysticType.Length, process.SetComplete);

                for (int i = 0; i < analysticType.Length; i++)
                {
                    var tar = analysticType[i];
                    AssetLoad.PreloadAsset<TextAsset>(outPutPath + $"{tar}.json",
                        a =>
                        {
                            var target = a.Result;
                            plantForms.Add(JsonMapper.ToObject<AnalyticsPlantform>(target.text));
                            voter.Add();
                        });
                }
            }
            else
            {
                process.SetComplete();
            }

            return process;
        }

        public static void SendEvent(int type, string targetId, int missionId)
        {
            bool isDebug = false;
            for (int i = 0; i < plantForms.Count; i++)
            {
                var plant = plantForms[i];
                for (int j = 0; j < plant.info.Length; j++)
                {
                    var matchInfo = plant.info[j];
                    if (matchInfo.MatchSuccess(type, missionId, targetId, isDebug))
                    {
                        SDKMgr.GetInstance().MyAnalyticsSDK.OnEvent(matchInfo.eventID, plant.type);
                        if (matchInfo.maxSendCount >= 0)
                        {
                            matchInfo.maxSendCount--;
                        }

                        if (IsLog)
                        {
                            GameDebug.LogFormat("打点成功: {0},平台{1}", matchInfo.eventID, plant.type);
                        }

                        break;
                    }
                }
            }
        }
    }
}