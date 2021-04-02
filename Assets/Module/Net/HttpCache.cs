/*
 * 脚本名称：HttpCache
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-03-09 13:54:33
 * 脚本作用：
*/



namespace Module
{
    public class HttpCache : Singleton<HttpCache>
	{
        /*
        /// <summary>
        /// Post 非数组
        /// </summary>
        /// <typeparam iapName="T"></typeparam>
        /// <param iapName="url"></param>
        /// <param iapName="args"></param>
        /// <param iapName="callBack"></param>
        /// <returns></returns>
        public UnityWebRequest Post<T>(string url, HttpArgs args, Action<ReturnValue<T>> callBack) where T : DataValue
        {
            if (string.IsNullOrEmpty(url))
            {
                callBack(null);
                return null;
            }

            url = string.Format("{0}:{1}{2}", DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerIP, DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerPort, url);
            Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Authorization", "Bearer " + Player.player.LocalToken);
            return NetWorkHttp.Instance.PostData(url, args, header,callBack,null,null);
        }

        /// <summary>
        /// Post 数组
        /// </summary>
        /// <typeparam iapName="T"></typeparam>
        /// <param iapName="url"></param>
        /// <param iapName="args"></param>
        /// <param iapName="callBack"></param>
        /// <returns></returns>
        public UnityWebRequest Post<T>(string url, HttpArgs args, Action<ReturnValueArray<T>> callBack) where T : DataValue
        {
            if (string.IsNullOrEmpty(url))
            {
                callBack(null);
                return null;
            }

            url = string.Format("{0}:{1}{2}", DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerIP, DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerIP, url);
            Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Authorization", "Bearer " + Player.player.LocalToken);
            return NetWorkHttp.Instance.PostData(url, args, header, callBack, null,null);
        }

        /// <summary>
        /// Get 非数组
        /// </summary>
        /// <typeparam iapName="T"></typeparam>
        /// <param iapName="url"></param>
        /// <param iapName="args"></param>
        /// <param iapName="callBack"></param>
        /// <returns></returns>
        public UnityWebRequest Get<T>(string url, HttpArgs args, Action<ReturnValue<T>> callBack) where T : DataValue
        {
            if (string.IsNullOrEmpty(url))
            {
                callBack(null);
                return null;
            }
            url = string.Format("{0}:{1}{2}", DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerIP, DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerPort, url);
            Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Authorization", "Bearer " + Player.player.LocalToken);
            int length = 0;
            string allParameter = string.Empty;
            if (args != null)
            {
                if (args.Length > 0)
                {
                    url = url + "?";
                    List<string> temp = new List<string>();

                    foreach (KeyValuePair<string, object> keyValuePair in args.Paramaters)
                    {
                        temp.Add(string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value));
                    }

                    allParameter = string.Join("&", temp);
                }
            }

            url = url + allParameter;
            return NetWorkHttp.Instance.GetData(url, header, callBack, null,null);
        }

        /// <summary>
        /// Get 数组
        /// </summary>
        /// <typeparam iapName="T"></typeparam>
        /// <param iapName="url"></param>
        /// <param iapName="args"></param>
        /// <param iapName="callBack"></param>
        /// <returns></returns>
        public UnityWebRequest Get<T>(string url, HttpArgs args, Action<ReturnValueArray<T>> callBack) where T : DataValue
        {
            if (string.IsNullOrEmpty(url))
            {
                callBack(null);
                return null;
            }

            url = string.Format("{0}:{1}{2}", DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerIP, DataMgr.Instance.GetLocalConfig<ServerInfo>()[0].ServerPort, url);
            Dictionary<string, string> header = new Dictionary<string, string>();
            //header.Add("Authorization", "Bearer " + Player.player.LocalToken);
            int length = 0;
            string allParameter = string.Empty;
            if (args != null)
            {
                if (args.Length > 0)
                {
                    url = url + "?";
                    List<string> temp = new List<string>();

                    foreach (KeyValuePair<string, object> keyValuePair in args.Paramaters)
                    {
                        temp.Add(string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value));
                    }

                    allParameter = string.Join("&", temp);
                }
            }

            url = url + allParameter;
            return NetWorkHttp.Instance.GetData(url, header, callBack);
        }
        */
    }
}