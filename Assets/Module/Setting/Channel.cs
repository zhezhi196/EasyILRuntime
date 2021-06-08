using System;
using UnityEngine;

namespace Module
{
    [Flags]
    public enum ChannelType
    {
        googlePlay = 1 << 0,
        AppStore = 1 << 1,
        AppStoreCN = 1 << 2,
        Huawei = 1 << 3,
        TapTap = 1 << 4,
        UnKnow3 = 1 << 5,
        UnKnow4 = 1 << 6,
        UnKnow5 = 1 << 7,
        UnKnow6 = 1 << 8,
    }

    public class Channel : MonoBehaviour
    {
        #region 静态

        private static Channel instance;

        private static Channel Instance
        {
            get
            {
                if (instance == null) instance = FindObjectOfType<Channel>();
                return instance;
            }
        }

        #endregion

        [SerializeField]
        private ChannelType targetChannel;

        public static ChannelType channel
        {
            get
            {
                if (Instance == null) return 0;
                return Instance.targetChannel;
            }
        }
        /// <summary>
        /// 注意channelType在打包的时候只会有一个
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasChannel(ChannelType type)
        {
            return (type & channel) != 0;
        }
    }
}