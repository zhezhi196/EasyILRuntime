/*
 * 脚本名称：NetWorkHttp
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-19 12:22:38
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

namespace Module
{
    public class NetWorkHttp:Singleton<NetWorkHttp>
    {
        #region Post请求

        public UnityWebRequest Post(string url, HttpArgs args, Action<string> callBack, int timeOut = 0, Dictionary<string, string> header = null)
        {
            WWWForm form = new WWWForm();
            if (args != null)
            {
                if (args.Length > 0)
                {
                    foreach (KeyValuePair<string, object> keyValuePair in args.Paramaters)
                    {
                        string json = GetJson(keyValuePair.Value);
                        form.AddField(keyValuePair.Key, json);
                    }
                }
            }

            UnityWebRequest data = UnityWebRequest.Post(url, form);
            if (timeOut > 0)
            {
                data.timeout = timeOut;
            }

            if (header != null)
            {
                foreach (var keyValuePair in header)
                {
                    data.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                }
            }

            PostRequest(data, callBack);
            return data;
        }

        private async void PostRequest(UnityWebRequest data, Action<string> callBack)
        {
            try
            {
                await Async.WaitForAsyncOption(data.SendWebRequest());

                try
                {
                    callBack(data.downloadHandler.text);
                }
                catch (Exception e)
                {
                    GameDebug.LogError(string.Format("Post访问{0} \n回调出现错误:{1}", data.url, e.Message));
                }
            }
            finally
            {
                data.Dispose();
            }
        }

        #endregion

        #region Get请求

        public UnityWebRequest Get(string url, Action<string> callBack, int timeOut = 0, Dictionary<string, string> header = null)
        {
            UnityWebRequest data = UnityWebRequest.Get(url);
            if (timeOut > 0)
            {
                data.timeout = timeOut;
            }

            if (header != null)
            {
                foreach (var keyValuePair in header)
                {
                    data.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                }
            }

            GetRequest(data, callBack);
            return data;
        }

        private async void GetRequest(UnityWebRequest data, Action<string> callBack)
        {
            try
            {
                await Async.WaitForAsyncOption(data.SendWebRequest());

                try
                {
                    callBack(data.downloadHandler.text);
                }
                catch (Exception e)
                {
                    GameDebug.LogError(string.Format("Get: {0} \n回调出现错误:{1}", data.url, e.Message));
                }
            }
            finally
            {
                data.Dispose();
            }
        }

        #endregion

        #region 打开某网页

        /// <summary>
        ///     打开网页
        /// </summary>
        /// <param id="url"></param>
        public void OpenWeb(string url)
        {
            var www = new WWW(url);
            Application.OpenURL(www.url);
            GameDebug.LogFormat("打开{0}成功", url);
        }

        #endregion

        #region 下载图片

        public UnityWebRequest DownloadPic<T>(string url, Dictionary<string, string> header, T id,
            Action<T, Sprite> callBack)
        {
            string path = string.Format("{0}/ImageCache/", Application.persistentDataPath);
            UnityWebRequest data = null;
            if (!Directory.Exists(Application.persistentDataPath + "/ImageCache/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/ImageCache/");
            }

            if (!File.Exists(path + url.GetHashCode()))
            {
                data = UnityWebRequest.Get(url);
                DownloadImage(data, path, header, id, callBack);
            }

            else
            {
                string filePath = "file:///" + path + url.GetHashCode();
                data = UnityWebRequest.Get(filePath);
                LoadLocalImage(data, path, id, callBack);
            }

            return data;
        }

        public UnityWebRequest LoadLocalPic<T>(string path, T arg, Action<T, Sprite> callBack)
        {
            UnityWebRequest data = UnityWebRequest.Get(path);
            LoadLocalImage(data, path, arg, callBack);
            return data;
        }

        /// <summary>
        ///     从网络下载图片
        /// </summary>
        /// <param id="url"></param>
        /// <param id="path"></param>
        /// <param id="header"></param>
        /// <param id="id"></param>
        /// <param id="callBack"></param>
        /// <param id="request"></param>
        /// <returns></returns>
        private async void DownloadImage<T>(UnityWebRequest data, string path, Dictionary<string, string> header, T id,
            Action<T, Sprite> callBack)
        {
            try
            {
                if (header != null)
                {
                    foreach (var keyValuePair in header)
                    {
                        data.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);
                data.downloadHandler = downloadHandlerTexture;

                await Async.WaitForAsyncOption(data.SendWebRequest());
                Texture2D texture2D = downloadHandlerTexture.texture;
                if (texture2D == null)
                {
                    GameDebug.LogErrorFormat("Get: {0}图片为null", data.url);
                }
                else
                {
                    GameDebug.LogFormat("Get: {0}图片成功", data.url);
                }

                //将图片保存至缓存路径  
                byte[] pngData = texture2D.EncodeToPNG(); //将材质压缩成byte流  
                File.WriteAllBytes(path + data.url.GetHashCode(), pngData); //然后保存到本地  

                Sprite m_sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height),
                    Vector2.zero);

                callBack(id, m_sprite);
            }
            finally
            {
                data.Dispose();
            }
        }

        /// <summary>
        ///     从本地下载图片
        /// </summary>
        /// <param id="url"></param>
        /// <param id="path"></param>
        /// <param id="id"></param>
        /// <param id="callBack"></param>
        /// <param id="request"></param>
        /// <returns></returns>
        private async void LoadLocalImage<T>(UnityWebRequest data, string path, T id, Action<T, Sprite> callBack)
        {
            try
            {
                DownloadHandlerTexture downloadHandlerTexture = new DownloadHandlerTexture(true);
                data.downloadHandler = downloadHandlerTexture;
                await Async.WaitForAsyncOption(data.SendWebRequest());
                Texture2D texture = downloadHandlerTexture.texture;
                if (texture == null)
                {
                    GameDebug.LogErrorFormat("Get: {0}图片为null", data.url);
                }
                else
                {
                    GameDebug.LogFormat("Get: {0}图片成功", data.url);
                }

                Sprite m_sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                callBack(id, m_sprite);
            }
            finally
            {
                data.Dispose();
            }
        }

        public UnityWebRequest UploadFile<T>(string url, string[] filesPath, HttpArgs args,
            Dictionary<string, string> header, Action<ReturnValueArray<T>> callBack, bool RunInBackGround)
            where T : ISqlData
        {
            if (!RunInBackGround)
            {
                UICommpont.UiLoading.Open();
            }

            List<string> st = new List<string>(filesPath);

            for (int i = 0; i < filesPath.Length; i++)
            {
                if (!File.Exists(filesPath[i]))
                {
                    GameDebug.LogError(string.Format("无法找到{0}下的文件", url));
                    st.Remove(filesPath[i]);
                }
            }

            filesPath = st.ToArray();
            WWWForm form = new WWWForm();
            for (int i = 0; i < filesPath.Length; i++)
            {
                string fileName = filesPath[i].Substring(filesPath[i].LastIndexOf('/') + 1);
                FileStream fs = File.Open(filesPath[i], FileMode.Open);
                byte[] buffer = new byte[fs.Length];

                using (fs)
                {
                    fs.Read(buffer, 0, buffer.Length);
                    fs.Close();
                }

                form.AddBinaryData(fileName, buffer);
            }

            foreach (KeyValuePair<string, object> keyValuePair in args.Paramaters)
            {
                string json = GetJson(keyValuePair.Value);
                form.AddField(keyValuePair.Key, json);
            }

            UnityWebRequest data = UnityWebRequest.Post(url, form);
            UploadData(data, header, callBack, RunInBackGround);
            return data;
        }

        private async void UploadData<T>(UnityWebRequest data, Dictionary<string, string> header,
            Action<ReturnValueArray<T>> callBack, bool RunInBackGround) where T : ISqlData
        {
            try
            {
                if (header != null)
                {
                    foreach (var keyValuePair in header)
                    {
                        data.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                await Async.WaitForAsyncOption(data.SendWebRequest());

                ReturnValueArray<T> arg = JsonMapper.ToObject<ReturnValueArray<T>>(data.downloadHandler.text);

                if (arg == null)
                {
                    GameDebug.Log("上传文件失败");
                    return;
                }

                try
                {
                    callBack(arg);
                }
                catch (Exception e)
                {
                    GameDebug.LogErrorFormat("回调出现错误：{0}", e.Message);
                    throw;
                }

                if (!RunInBackGround)
                {
                    //UIController.Instance.Commpont.UiLoading.Close();
                }
            }
            finally
            {
                data.Dispose();
            }
        }

        private string GetJson(object args)
        {
            if (args.GetType() == typeof(string) || args.GetType() == typeof(int) || args.GetType() == typeof(long) ||
                args.GetType() == typeof(uint) || args.GetType() == typeof(ulong) || args.GetType() == typeof(short) ||
                args.GetType() == typeof(ushort) || args.GetType() == typeof(double) ||
                args.GetType() == typeof(char) || args.GetType() == typeof(decimal) ||
                args.GetType() == typeof(DateTime))
            {
                return args.ToString();
            }
            else
            {
                return JsonMapper.ToJson(args);
            }
        }

        #endregion

        #region 整合Get方法的url

        public string CombineGetUrl(string url, HttpArgs args)
        {
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

            return url + allParameter;
        }

        #endregion
    }
}