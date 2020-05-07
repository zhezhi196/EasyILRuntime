using System;
using System.IO;
using UnityEngine.Networking;

namespace xasset
{
    public class Download
    {
        public Action<float> onProgress;
        public Action completed;
        public string error;

        public float progress { get; private set; }

        public bool isDone { get; private set; }

        public long maxlen { get; private set; }

        public long len { get; private set; }

        public int index { get; private set; }

        public string url { get; set; }

        public string path { get; set; }

        public string savePath { get; set; }

        public string version { get; set; }

        public State state { get; private set; }

        private UnityWebRequest request { get; set; }

        private FileStream fs { get; set; }

        private void WriteBuffer()
        {
            byte[] data = request.downloadHandler.data;
            if (data == null)
                return;
            int count = data.Length - index;
            fs.Write(data, index, count);
            index += count;
            len += count;
            progress = len / (float) maxlen;
        }

        public void Update()
        {
            if (isDone)
                return;
            switch (state)
            {
                case State.HeadRequest:
                    if (request.error != null)
                        error = request.error;
                    if (!request.isDone)
                        break;
                    maxlen = long.Parse(request.GetResponseHeader("Content-Length"));
                    request.Dispose();
                    request = null;
                    string directoryName = Path.GetDirectoryName(savePath);
                    if (!Directory.Exists(directoryName))
                        Directory.CreateDirectory(directoryName);
                    fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                    len = fs.Length;
                    bool flag1 = string.IsNullOrEmpty(version);
                    string str = Versions.Get(savePath);
                    bool flag2 = string.IsNullOrEmpty(str);
                    if (flag1 | flag2 || !str.Equals(version))
                    {
                        Versions.Set(savePath, version);
                        len = 0L;
                    }

                    if (len < maxlen)
                    {
                        fs.Seek(len, SeekOrigin.Begin);
                        request = UnityWebRequest.Get(url);
                        request.SetRequestHeader("Range", "bytes=" + len + "-");
                        request.SendWebRequest();
                        index = 0;
                        state = State.BodyRequest;
                    }
                    else
                        state = State.FinishRequest;

                    break;
                case State.BodyRequest:
                    if (request.error != null)
                        error = request.error;
                    if (!request.isDone)
                    {
                        WriteBuffer();
                        break;
                    }

                    WriteBuffer();
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }

                    request.Dispose();
                    state = State.FinishRequest;
                    break;
                case State.FinishRequest:
                    if (completed != null)
                        completed();
                    isDone = true;
                    state = State.Completed;
                    break;
            }
        }

        public void Start()
        {
            if(File.Exists(savePath)) File.Delete(savePath);
            request = UnityWebRequest.Head(url);
            request.SendWebRequest();
            progress = 0.0f;
            isDone = false;
        }

        public void Stop()
        {
            isDone = true;
        }

        public enum State
        {
            HeadRequest,
            BodyRequest,
            FinishRequest,
            Completed
        }
    }
}