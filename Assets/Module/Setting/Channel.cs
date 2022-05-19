using System;
using UnityEngine;

namespace Module
{
    public static class Channel
    {
        public static bool isIOS
        {
            get { return channel == ChannelType.AppStore || channel == ChannelType.AppStoreCN; }
        }

        public static bool isChina
        {
            get { return channel != ChannelType.AppStore && channel != ChannelType.googlePlay; }
        }


        public static ChannelType channel
        {
            get { return Config.globleConfig.channel; }
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