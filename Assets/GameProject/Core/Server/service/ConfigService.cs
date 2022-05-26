using LitJson;
using System;
using System.Collections.Generic;

namespace Service
{
    public enum E_ConfigType
    {
        /// <summary>
        /// 兑换码是否开启
        /// </summary>
        RedeemEnable = 1,
    }

    public class ConfigService : Service<ConfigService>
    {
        public ConfigService()
        {
            this.ServiceName = "ActivityConfigKongbu2";
        }

        public void GetConfig(E_ConfigType configId, Action<bool, LitJson.JsonData> cb, Action cbFail)
        {
            if (mConfigs.ContainsKey(configId))
            {
                if (null != cb)
                {
                    ConfigClass config = mConfigs[configId];
                    cb(config.IsOpen, config.param);
                }
                return;
            }

            var param = new LitJson.JsonData();
            param["activity"] = (int)configId;
            AddCommonParam(param);
            StartTopic("HD_GetConfig", param, (value) =>
            {
                if ((string)value[KEY_ERROR] != "") {
                    UnityEngine.Debug.Log(string.Format("ConfigService GetConfig[{0}] Error:{1}", configId, value[KEY_ERROR]));
                    if (null != cbFail)
                    {
                        cbFail();
                    }
                    return;
                }
                JsonData result = value[KEY_Result];
                bool isOpen = (bool)result[KEY_Open];
                UnityEngine.Debug.LogFormat("ConfigService GetConfig[{0}] Return, isOpen={1} param={2}"
                    , configId, isOpen, (string)result[KEY_Param]);
                JsonData configParam;
                try
                {
                    configParam = JsonMapper.ToObject((string)result[KEY_Param]);
                }
                catch (JsonException)
                {
                    configParam = new JsonData();
                }
                mConfigs.Add(configId, new ConfigClass(isOpen, configParam));
                if (null != cb)
                {
                    cb(isOpen, configParam);
                }
            });
        }
    
        internal class ConfigClass
        {
            public bool IsOpen;
            public JsonData param;

            internal ConfigClass(bool isOpen, JsonData param)
            {
                this.IsOpen = isOpen;
                this.param = param;
            }
        }


        private Dictionary<E_ConfigType, ConfigClass> mConfigs = new Dictionary<E_ConfigType, ConfigClass>();
        protected const string KEY_Open = "open";
        protected const string KEY_Param = "param";
    }
}