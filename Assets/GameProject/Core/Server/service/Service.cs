using UnityEngine;
using System;
using System.Collections.Generic;
using LitJson;

namespace Service
{
    public class Service<T> where T : Service<T>, new()
    {
        public string ServiceName { get; protected set; }
        protected const string KEY_ERROR = "Error";
        protected const string KEY_Result = "Result";
        public const string ErrNetworkFailed = "1";
        protected Dictionary<string, float> TopicRetryTime = new Dictionary<string, float>();

        public static T Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new T();
                }
                return mInstance;
            }
        }

        protected static T mInstance;

        protected string GetTopic(string topic)
        {
            return this.ServiceName + "/" + topic;
        }

        protected void StartTopic(string topic, Action<JsonData> callBack)
        {
            StartTopic(topic, "", callBack);
        }

        protected void StartTopic(string topic, LitJson.JsonData param, Action<JsonData> callBack)
        {
            string json = "";
            if (null != param)
            {
                json = LitJson.JsonMapper.ToJson(param);
            }
            StartTopic(topic, json, callBack);
        }

        protected void StartTopic(string topic, Dictionary<string, object> param, Action<JsonData> callBack)
        {
            string json = "";
            if (null != param)
            {
                json = LitJson.JsonMapper.ToJson(param);
            }
            StartTopic(topic, json, callBack);
        }

        protected Dictionary<string, object> AddCommonParam(Dictionary<string, object> param)
        {
            if (null == param)
            {
                param = new Dictionary<string, object>();
            }
            param.Add("game", SDK.Platform.Instance.GameID);
            param.Add("channel", SDK.Platform.Instance.ChannelID);
            param.Add("version", SDK.Platform.Instance.Version);
            param.Add("user_id", SDK.Platform.Instance.UserID);
            return param;
        }

        protected JsonData AddCommonParam(JsonData param)
        {
            if (null == param)
            {
                param = new LitJson.JsonData();
            }
            param["game"] = SDK.Platform.Instance.GameID;
            param["channel"] = SDK.Platform.Instance.ChannelID;
            param["version"] = SDK.Platform.Instance.Version;
            param["user_id"] = SDK.Platform.Instance.UserID;
            return param;
        }

        private void StartTopic(string topic, string param, Action<JsonData> callBack)
        {
            if (null == param)
            {
                param = "";
            }
            Debug.Log("StartTopic:" + param.Length + ":" + param);
            topic = GetTopic(topic);
            Action<string> cb = null;
            if (null != callBack)
            {
                cb = (jsonStr) =>
                {
                    Debug.Log(string.Format("Topic [{0}] ReturnStart[{1}]", topic, jsonStr));
                    try
                    {
                        JsonData value = LitJson.JsonMapper.ToObject(jsonStr);
                        callBack(value);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(string.Format("Convert Server topic[{0}] Return Data[{1}] Error", topic, jsonStr));
                        Debug.LogException(e);
                    }
                };
            }
            Debug.Log("Start Topic:" + topic);
            if (TopicRetryTime.ContainsKey(topic))
            {
                M2MqttUnity.M2MqttUnityClient.Instance.Public(topic, param, cb, TopicRetryTime[topic]);
            } else
            {
                M2MqttUnity.M2MqttUnityClient.Instance.Public(topic, param, cb);
            }
        }
    
        protected void SubTopic(string topic, Func<JsonData, bool> cb)
        {
            Debug.Log("Service SubTopic:" + topic);
            if (null == cb)
            {
                Debug.LogError("Service SubTopic but no Cb");
                return;
            }
            topic = GetTopic(topic);
            M2MqttUnity.M2MqttUnityClient.Instance.SubscribeTopics(topic, msg =>
            {
                JsonData json = JsonMapper.ToObject(msg);
                return cb(json);
            });
        }
    }
}
