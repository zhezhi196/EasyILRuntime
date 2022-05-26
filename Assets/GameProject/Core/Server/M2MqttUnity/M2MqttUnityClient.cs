/*
The MIT License (MIT)

Copyright (c) 2018 Giovanni Paolo Vigano'

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using SDK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

/// <summary>
/// Adaptation for Unity of the M2MQTT library (https://github.com/eclipse/paho.mqtt.m2mqtt),
/// modified to run on UWP (also tested on Microsoft HoloLens).
/// </summary>
namespace M2MqttUnity
{
    internal class TopicInfo
    {
        internal string TopicID;
        internal string Data;
        internal Action<string> CB;
        internal bool IsSend;
        internal bool NeedConnect;
        internal float timeResend;
        internal float timeRetry;

        internal TopicInfo(string id, string data, Action<string> cb, bool needConnect, float retryTime)
        {
            this.TopicID = id;
            this.Data = data;
            this.CB = cb;
            this.IsSend = false;
            this.NeedConnect = needConnect;
            timeRetry = retryTime;
            SetTime(Time.unscaledTime);
        }

        internal void SetTime(float time)
        {
            timeResend = time + timeRetry;
        }
    }

    internal class SubTopicInfo
    {
        internal string TopicID;
        internal Func<string, bool> CB;
        internal bool NeedRemove = false;

        internal SubTopicInfo(string id, Func<string, bool> cb)
        {
            this.TopicID = id;
            this.CB = cb;
        }
    }

    /// <summary>
    /// Generic MonoBehavior wrapping a MQTT client, using a double buffer to postpone message processing in the main thread. 
    /// </summary>
    public class M2MqttUnityClient : MonoBehaviour
    {
        [Header("MQTT broker configuration")]
        [Tooltip("IP address or URL of the host running the broker")]
        //[HideInInspector] protected string brokerAddress = "192.168.0.197";
        //[HideInInspector] protected string brokerAddress = "127.0.0.1";
        //[HideInInspector] protected string brokerAddress = "192.168.0.207";
        //[HideInInspector] protected string brokerAddress = "35.163.131.168";
        [HideInInspector] protected string brokerAddress = null;
        [Tooltip("Port where the broker accepts connections")]
        public int brokerPort = 3563;
        [Tooltip("Use encrypted connection")]
        public bool isEncrypted = false;
        [Header("Connection parameters")]
        [Tooltip("Connection to the broker is delayed by the the given milliseconds")]
        public int connectionDelay = 500;
        [Tooltip("Connection timeout in milliseconds")]
        public int timeoutOnConnection = MqttSettings.MQTT_CONNECT_TIMEOUT;
        [Tooltip("Connect on startup")]
        public bool autoConnect = false;
        [Tooltip("UserName for the MQTT broker. Keep blank if no user name is required.")]
        public string mqttUserName = null;
        [Tooltip("Password for the MQTT broker. Keep blank if no password is required.")]
        public string mqttPassword = null;

        public bool IsConnected
        {
            get
            {
                return null != client && client.IsConnected && !mIsConnecting;
            }
        }
        public int MsgId
        {
            get
            {
                return msgId++;
            }
        }
        private int msgId = 0;

        /// <summary>
        /// Wrapped MQTT client
        /// </summary>
        protected MqttClient client;
        protected bool mIsConnecting;

        private List<MqttMsgPublishEventArgs> messageQueue1 = new List<MqttMsgPublishEventArgs>();
        private List<MqttMsgPublishEventArgs> messageQueue2 = new List<MqttMsgPublishEventArgs>();
        private List<MqttMsgPublishEventArgs> frontMessageQueue = null;
        private List<MqttMsgPublishEventArgs> backMessageQueue = null;
        private bool mqttClientConnectionClosed = false;
        private bool mqttClientConnected = false;


        /// <summary>
        /// Event fired when a connection is successfully established
        /// </summary>
        public event Action ConnectionSucceeded;
        /// <summary>
        /// Event fired when failing to connect
        /// </summary>
        public event Action ConnectionFailed;

        public static M2MqttUnityClient Instance { get; private set; }

        /// <summary>
        /// Connect to the broker using current settings.
        /// </summary>
        public virtual void Connect()
        {
#if !UNITY_EDITOR
            if (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
            {
                OnConnectionFailed("No Net");
                return;
            }
#endif
            if (mIsConnecting)
            {
                return;
            }
            mIsConnecting = true;
            if (client == null || !client.IsConnected)
            {
                StartCoroutine(DoConnect());
            }
        }

        /// <summary>
        /// Disconnect from the broker, if connected.
        /// </summary>
        public virtual void Disconnect()
        {
            if (client != null)
            {
                StartCoroutine(DoDisconnect());
            }
        }

        private Dictionary<string, TopicInfo> mTopics = new Dictionary<string, TopicInfo>();
        private Dictionary<string, List<SubTopicInfo>> mSubTopics = new Dictionary<string, List<SubTopicInfo>>();
        public virtual void Public(string topicId, string data, Action<string> cb, float retryTime = 10, bool needConnect = false)
        {
            Debug.Log(string.Format("{0}][retry={1}]", topicId, retryTime));
            if (mTopics.ContainsKey(topicId))
            {
                if (null != cb)
                {
                    cb(string.Format(@"error: Already Have Topic:{0}", topicId));
                    return;
                }
            }
            var topicInfo = new TopicInfo(topicId + "/" + MsgId, data, cb, needConnect, retryTime);
            this.mTopics.Add(topicInfo.TopicID, topicInfo);
            if (!IsConnected)
            {
                Connect();
                return;
            }
        }

        /// <summary>
        /// Override this method to take some actions before connection (e.g. display a message)
        /// </summary>
        protected virtual void OnConnecting()
        {
            UnityEngine.Debug.LogFormat("Connecting to broker on {0}:{1}...\n", brokerAddress, brokerPort.ToString());
        }

        /// <summary>
        /// Override this method to take some actions if the connection succeeded.
        /// </summary>
        protected virtual void OnConnected()
        {
            UnityEngine.Debug.LogWarningFormat("Connected to {0}:{1}...\n", brokerAddress, brokerPort.ToString());


            if (ConnectionSucceeded != null)
            {
                ConnectionSucceeded();
            }
            //PublicTopics();
        }

        /// <summary>
        /// Override this method to take some actions if the connection failed.
        /// </summary>
        protected virtual void OnConnectionFailed(string errorMessage)
        {
            Debug.LogWarning("Connection failed.");
            mIsConnecting = false;
            if (ConnectionFailed != null)
            {
                ConnectionFailed();
            }
            foreach (var item in mTopics)
            {
                if (null != item.Value.CB)
                {
                    item.Value.CB("{\"Error\": \"1\", \"result\":{\"msg\":\"1\"}}");
                }
            }
            mTopics.Clear();
            foreach (var item in mSubTopics)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (null != item.Value[i].CB)
                    {
                        item.Value[i].CB("{\"Error\": \"1\"}");
                    }
                }
            }
            mSubTopics.Clear();
        }

        public virtual void SubscribeTopics(string topic, Func<string, bool> cb)
        {
            List<SubTopicInfo> infos = null;
            if (mSubTopics.TryGetValue(topic, out infos))
            {
                if (null != infos)
                {
                    for (int i = 0; i < infos.Count; i++)
                    {
                        if (infos[i].CB == cb)
                        {
                            Debug.LogError(string.Format("Already Sub Topic[{0}][{1}]", topic, cb));
                            return;
                        }
                    }
                }
            }
            if (null == infos)
            {
                infos = new List<SubTopicInfo>();
            }
            SubTopicInfo info = new SubTopicInfo(topic, cb);
            infos.Add(info);
            mSubTopics[topic] = infos;
            if (!IsConnected)
            {
                Connect();
                return;
            }
        }

        /// <summary>
        /// Override this method to subscribe to MQTT topics.
        /// </summary>
        protected virtual void PublicTopics()
        {
            if (!IsConnected)
            {
                return;
            }
            foreach (var topicInfo in mTopics)
            {
                if (topicInfo.Value.IsSend == false || topicInfo.Value.timeResend < Time.unscaledTime)
                {
                    topicInfo.Value.IsSend = true;
                    topicInfo.Value.SetTime(Time.unscaledTime);
                    UnityEngine.Debug.Log("M2Mqtt Send Topic:" + topicInfo.Value.TopicID);
                    this.client.Publish(
                        topicInfo.Value.TopicID
                        , System.Text.Encoding.UTF8.GetBytes(topicInfo.Value.Data)
                        , MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE
                        , false);
                }
            }
        }

        /// <summary>
        /// Override this method to unsubscribe to MQTT topics (they should be the same you subscribed to with SubscribeTopics() ).
        /// </summary>
        protected virtual void UnsubscribeTopics()
        {
        }

        /// <summary>
        /// Disconnect before the application quits.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            CloseConnection();
        }

        /// <summary>
        /// Initialize MQTT message queue
        /// Remember to call base.Awake() if you override this method.
        /// </summary>
        protected virtual void Awake()
        {
            if (null != Instance)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            frontMessageQueue = messageQueue1;
            backMessageQueue = messageQueue2;
        }

        /// <summary>
        /// Connect on startup if autoConnect is set to true.
        /// </summary>
        protected virtual void Start()
        {
            //if (autoConnect)
            //{
            //    Connect();
            //}
            //Connect();
        }

        /// <summary>
        /// Override this method for each received message you need to process.
        /// </summary>
        protected virtual void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            //UnityEngine.Debug.LogFormat("Message received on topic: {0}:{1}", topic, msg);
            UnityEngine.Debug.LogFormat("Message DecodeMessage on topic: {0}:{1}", topic, msg);
            TopicInfo info;
            if (mTopics.TryGetValue(topic, out info))
            {
                mTopics.Remove(topic);
                if (null != info.CB)
                {
                    info.CB(msg);
                }
            }
            List<SubTopicInfo> subTopics = null;
            if (mSubTopics.TryGetValue(topic, out subTopics))
            {
                bool isDelete = false;
                for (int i = 0; null != subTopics && i < subTopics.Count; i++)
                {
                    var subInfo = subTopics[i];
                    subInfo.NeedRemove = subInfo.CB(msg);
                    isDelete = isDelete || subInfo.NeedRemove;
                }
                if (isDelete == true)
                {
                    List<SubTopicInfo> newSubInfos = new List<SubTopicInfo>();
                    for (int i = 0; i < subTopics.Count; i++)
                    {
                        if (subTopics[i].NeedRemove == false)
                        {
                            newSubInfos.Add(subTopics[i]);
                        }
                    }
                    if (newSubInfos.Count <= 0)
                    {
                        mSubTopics.Remove(topic);
                    }
                    else
                    {
                        mSubTopics[topic] = newSubInfos;
                    }
                }
            }
        }

        /// <summary>
        /// Override this method to take some actions when disconnected.
        /// </summary>
        protected virtual void OnDisconnected()
        {
            Debug.Log("Disconnected.");
            foreach (var item in mTopics)
            {
                if (null != item.Value.CB)
                {
                    item.Value.CB("{\"Error\": \"1\"}");
                }
            }
            mTopics.Clear();
            foreach (var item in mSubTopics)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (null != item.Value[i].CB)
                    {
                        item.Value[i].CB("{\"Error\": \"1\"}");
                    }
                }
            }
            mSubTopics.Clear();
        }

        /// <summary>
        /// Override this method to take some actions when the connection is closed.
        /// </summary>
        protected virtual void OnConnectionLost()
        {
            Debug.LogWarning("CONNECTION LOST!");
            foreach (var item in mTopics)
            {
                if (null != item.Value.CB)
                {
                    item.Value.CB("{\"Error\": \"1\"}");
                }
            }
            mTopics.Clear();
            foreach (var item in mSubTopics)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (null != item.Value[i].CB)
                    {
                        item.Value[i].CB("{\"Error\": \"1\"}");
                    }
                }
            }
            mSubTopics.Clear();
        }

        /// <summary>
        /// Processing of income messages and events is postponed here in the main thread.
        /// Remember to call ProcessMqttEvents() in Update() method if you override it.
        /// </summary>
        protected virtual void Update()
        {
            ProcessMqttEvents();
            PublicTopics();

            if (IsConnected && !mIsConnecting && mSubTopics.Count <= 0 && mTopics.Count <= 0)
            {
                Debug.Log("M2Mqtt No Topic");
                Disconnect();
            }
        }

        protected virtual void ProcessMqttEvents()
        {
            // process messages in the main queue
            SwapMqttMessageQueues();
            ProcessMqttMessageBackgroundQueue();
            // process messages income in the meanwhile
            SwapMqttMessageQueues();
            ProcessMqttMessageBackgroundQueue();

            if (mqttClientConnectionClosed)
            {
                mqttClientConnectionClosed = false;
                OnConnectionLost();
            }
        }

        private void ProcessMqttMessageBackgroundQueue()
        {
            foreach (MqttMsgPublishEventArgs msg in backMessageQueue)
            {
                DecodeMessage(msg.Topic, msg.Message);
            }
            backMessageQueue.Clear();
        }

        /// <summary>
        /// Swap the message queues to continue receiving message when processing a queue.
        /// </summary>
        private void SwapMqttMessageQueues()
        {
            frontMessageQueue = frontMessageQueue == messageQueue1 ? messageQueue2 : messageQueue1;
            backMessageQueue = backMessageQueue == messageQueue1 ? messageQueue2 : messageQueue1;
        }

        private void OnMqttMessageReceived(object sender, MqttMsgPublishEventArgs msg)
        {
            Debug.Log(string.Format("OnMqttMessageReceived [topic:{0}]", msg.Topic));
            frontMessageQueue.Add(msg);
        }

        private void OnMqttConnectionClosed(object sender, EventArgs e)
        {
            Debug.LogErrorFormat("OnMqttConnectionClosed:{0}={1}", mqttClientConnected, mqttClientConnectionClosed);
            if (!mqttClientConnectionClosed)
            {
                //已经在强制关闭中了，防止值被意外刷新
                // Set unexpected connection closed only if connected (avoid event handling in case of controlled disconnection)
                mqttClientConnectionClosed = mqttClientConnected;
            }
            mqttClientConnected = false;
        }

        /// <summary>
        /// Connects to the broker using the current settings.
        /// </summary>
        /// <returns>The execution is done in a coroutine.</returns>
        private IEnumerator DoConnect()
        {
            Debug.LogWarning("DoConnect");
            // wait for the given delay
            yield return new WaitForSecondsRealtime(connectionDelay / 1000f);
            // leave some time to Unity to refresh the UI
            yield return new WaitForEndOfFrame();

            if (null == brokerAddress)
            {
                brokerAddress = Platform.Instance.Server_Ip;
            }
            // create client instance 
            Debug.LogFormat("brokerAddress: " + brokerAddress + ":" + brokerPort);
            if (client == null)
            {
                try
                {
#if (!UNITY_EDITOR && UNITY_WSA_10_0 && !ENABLE_IL2CPP)
                    client = new MqttClient(brokerAddress,brokerPort,isEncrypted, isEncrypted ? MqttSslProtocols.SSLv3 : MqttSslProtocols.None);
#else

                    client = new MqttClient(brokerAddress, brokerPort, isEncrypted, null, null, isEncrypted ? MqttSslProtocols.TLSv1_0 : MqttSslProtocols.None);
                    //System.Security.Cryptography.X509Certificates.X509Certificate cert = new System.Security.Cryptography.X509Certificates.X509Certificate();
                    //client = new MqttClient(brokerAddress, brokerPort, isEncrypted, cert, null, MqttSslProtocols.TLSv1_0, MyRemoteCertificateValidationCallback);
                    client.ProtocolVersion = MqttProtocolVersion.Version_3_1;
#endif
                }
                catch (Exception e)
                {
                    client = null;
                    UnityEngine.Debug.LogErrorFormat("CONNECTION FAILED! {0}", e.ToString());
                    OnConnectionFailed(e.Message);
                    yield break;
                }
            }
            else if (client.IsConnected)
            {
                yield break;
            }
            OnConnecting();

            // leave some time to Unity to refresh the UI
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            client.Settings.TimeoutOnConnection = timeoutOnConnection;
            string clientId = Guid.NewGuid().ToString();
            Exception exception = null;
            Thread thread = new Thread(() =>
            {

                try
                {
                    client.Connect(clientId, mqttUserName, mqttPassword);
                }
                catch (Exception e)
                {
                    exception = e;
                }
            });
            thread.Start();
            while (thread.IsAlive)
            {
                yield return new WaitForEndOfFrame();
            }

            if (exception != null)
            {
                client = null;
                UnityEngine.Debug.LogErrorFormat("Failed to connect to {0}:{1}:\n{2}", brokerAddress, brokerPort, exception.ToString());
                OnConnectionFailed(exception.Message);
                yield break;
            }
            yield return new WaitForEndOfFrame();
            if (client.IsConnected)
            {
                client.ConnectionClosed -= OnMqttConnectionClosed;
                client.ConnectionClosed += OnMqttConnectionClosed;
                // register to message received 
                client.MqttMsgPublishReceived -= OnMqttMessageReceived;
                client.MqttMsgPublishReceived += OnMqttMessageReceived;
                mqttClientConnected = true;
                OnConnected();
            }
            else
            {
                OnConnectionFailed("CONNECTION FAILED!");
            }
            //连接完成后等待0.2s，等待连接准备好，因为消息发送在Update里发送，时间卡的比较紧
            yield return new WaitForSeconds(0.2f);
            mIsConnecting = false;
            yield return new WaitForEndOfFrame();
        }

        private IEnumerator DoDisconnect()
        {
            yield return new WaitForEndOfFrame();
            CloseConnection();
            OnDisconnected();
        }

        private void CloseConnection()
        {
            mqttClientConnected = false;
            if (client != null)
            {
                if (client.IsConnected)
                {
                    UnsubscribeTopics();
                    client.Disconnect();
                }
                client.MqttMsgPublishReceived -= OnMqttMessageReceived;
                client.ConnectionClosed -= OnMqttConnectionClosed;
                client = null;
            }
        }

#if ((!UNITY_EDITOR && UNITY_WSA_10_0))
        private void OnApplicationFocus(bool focus)
        {
            // On UWP 10 (HoloLens) we cannot tell whether the application actually got closed or just minimized.
            // (https://forum.unity.com/threads/onapplicationquit-and-ondestroy-are-not-called-on-uwp-10.462597/)
            if (focus)
            {
                Connect();
            }
            else
            {
                CloseConnection();
            }
        }
#endif
    }
}
