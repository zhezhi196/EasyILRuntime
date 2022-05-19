/*
 * 脚本名称：NetWorkSocket
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-04 17:21:27
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Module
{
    public class NetWorkSocket : MonoBehaviour
    {
        //--------------发送消息---------------
        // 客户端socket
        private System.Net.Sockets.Socket m_Server;
        private Queue<byte[]> m_SendQueue = new Queue<byte[]>();
        public event Action OnSocketNetComplete;
        //--------------发送消息---------------

        //--------------接收消息---------------
        // 接收数据包的字节缓冲区
        private byte[] m_ReceiveBuffer = new byte[2048];
        //接收数据包的缓冲数据流
        private GameMemoryStream m_receiveStream = new GameMemoryStream();
        //接收消息的队列
        private Queue<byte[]> m_ReceiveQueue = new Queue<byte[]>();

        private int m_ReceiveCount = 0;
        //--------------接收消息---------------

        private const int m_CompressLen = 200;
        #region 初始化
        /// <summary>
        /// 网络初始化 最多和服务器连接5次
        /// </summary>
        /// <returns></returns>
        public AsyncLoadProcess Init(string IP,int Port)
        {
            AsyncLoadProcess process = new AsyncLoadProcess();
            //StartCoroutine(InitIEnu(process,IP,Port));
            Connect(IP, Port);
            process.SetComplete();
            return process;
        }

        //private IEnumerator InitIEnu(AsyncLoadProcess process, string IP, int Port)
        //{
        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    if (Connect(IP, Port))
        //    //    {
        //    //        process.SetComplete();

        //    //        if (OnSocketNetComplete != null)
        //    //        {
        //    //            OnSocketNetComplete();
        //    //        }
        //    //        break;
        //    //    }

        //    //    if (i == 4)
        //    //    {
        //    //        
        //    //        //UIModel win= UILoadMgr.Get(WinName.CommonPopup);
        //    //        //win.Open();
        //    //        //win.GetEntityBase<UICommonPopup>().SetString(LanguageMgr.Instance.GetString(4,"网络错误"));
        //    //        //GamePlay.Instance.UILoaing.Close();
        //    //    }

        //    //    yield return new WaitForSeconds(2);

        //    //}
        //}

        #endregion

        #region 连接服务器

        /// <summary>
        /// 连接到socketnet服务器
        /// </summary>
        /// <param id="ip"></param>
        /// <param iapName="port"></param>
        public bool Connect(string ip, int port)
        {
            //如果socke已经存在并且处于连接状态，直接返回
            if (m_Server != null && m_Server.Connected) return false;
           
            m_Server = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                m_Server.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                ReceiveMsg();
                GameDebug.Log("连接成功");
                return true;
            }
            catch (Exception ex)
            {
                GameDebug.LogError("连接失败"+ex.Message);
                return false;
            }
        }

        #endregion

        #region 发送消息
        #region 向服务器发送消息

        /// <summary>
        ///     把消息加入队列
        /// </summary>
        /// <param id="buffer"></param>
        public void SendData(byte[] buffer)
        {
            //封装后的数据包
            byte[] sendBuffer = MakeSendData(buffer);
            lock (m_SendQueue)
            {
                //把数据包加入队列
                m_SendQueue.Enqueue(sendBuffer);
                //启动委托 执行委托
                OnCheckSendQueue();
            }

        }



        #endregion

        #region 检查数据包队列的回调

        /// <summary>
        /// 检查数据包队列的回调
        /// </summary>
        private void OnCheckSendQueue()
        {
            lock (m_SendQueue)
            {
                //判断，如果队列中有数据包，则发送数据包
                if (m_SendQueue.Count > 0)
                {
                    //从队列中选取一个数据，把数据发送出去
                    Send(m_SendQueue.Dequeue());
                }
            }
        }

        #endregion

        #region 发送数据包到服务器

        /// <summary>
        /// 发送数据包到服务器
        /// </summary>
        /// <param id="buffer"></param>
        public void Send(byte[] buffer)
        {
            m_Server.BeginSend(buffer, 0, buffer.Length,SocketFlags.None, OnSend,m_Server);
        }

        private void OnSend(IAsyncResult ar)
        {
            m_Server.EndSend(ar);

            //继续检查队列
            OnCheckSendQueue();
        }

        #endregion

        #endregion

        #region 接收消息

        #region 接收

        public void Update()
        {
            while (true)
            {
                if (m_ReceiveCount <= 5)
                {
                    m_ReceiveCount++;
                    lock (m_ReceiveQueue)
                    {
                        if (m_ReceiveQueue.Count > 0)
                        {
                            //这样就得到服务器发送的byte数组了
                            byte[] buffer = m_ReceiveQueue.Dequeue();
                            ushort protoID = 0;
                            byte[] protoContent = new byte[buffer.Length - 2];
                            using (GameMemoryStream ms = new GameMemoryStream(buffer))
                            {
                                protoID = ms.ReadUshort();
                                ms.Read(protoContent, 0, protoContent.Length);
                                EventCenter.Dispatch(protoID.ToString(), protoID, MakeReceiveData(protoContent));
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    m_ReceiveCount = 0;
                    break;
                }
            }


        }
            
        private void ReceiveMsg()
        {
            //异步接收数据
            m_Server.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, OnReceiveMsgFinish,
                m_Server);
        }

        #endregion

        #region 接收数据的回调

        //接收数据的回调
        private void OnReceiveMsgFinish(IAsyncResult ar)
        {
            //这里如果客户端直接关闭进程，会报异常，相当于断开连接
            try
            {
                int length = m_Server.EndReceive(ar);

                if (length > 0)
                {
                    //把接收到的数据写入缓冲数据流的尾部
                    m_receiveStream.Position = m_receiveStream.Length;
                    // 把指定长度的字节写入数据流
                    m_receiveStream.Write(m_ReceiveBuffer, 0, length);
                    //如果缓存数据流的长度大于2，说明至少有个不完整的包过来了，那么我们需要拆包
                    //这里为什么大于2，因为我们客户端封装的时候，用的是ushort，他的长度是2
                    if (m_receiveStream.Length > 2)
                    {
                        SpitePackage();
                    }
                    //循环接收数据
                    ReceiveMsg();
                    ////假设只有一条消息，这个就是我们需要的消息
                    //byte[] buffer = m_receiveStream.ToArray();
                }
                else
                {
                    //如果传过来的数据长度是0，则客户端断开连接
                    GameDebug.LogFormat("服务器{0}断开连接", m_Server.LocalEndPoint);
                }
            }
            catch
            {
                GameDebug.LogFormat("服务器{0}断开连接", m_Server.LocalEndPoint);
            }

        }

        #endregion
 
        #region 拆包

        /// <summary>
        /// 拆包
        /// </summary>
        /// <returns></returns>
        private void SpitePackage()
        {
            //拆包
            while (true)
            {
                m_receiveStream.Position = 0;
                //currMsgLength=包体y的长度
                int currMsgLen = m_receiveStream.ReadUshort();
                //总包长度等于包头长度+包体长度
                int currFullMsgLength = 2 + currMsgLen;
                //如果数据流的长度大于等于整包的长度，那么说明我们至少收到一个完整包
                if (m_receiveStream.Length >= currFullMsgLength)
                {
                    //至少收到一个完整包

                    //定义包体的byte[]数组
                    byte[] buffer = new byte[currMsgLen];
                    //把数据流指针放在2的位置
                    m_receiveStream.Position = 2;
                    //把包体读到byte数组中
                    m_receiveStream.Read(buffer, 0, currMsgLen);
                    lock (m_ReceiveQueue)
                    {
                        m_ReceiveQueue.Enqueue(buffer);
                    }

                    //==============处理剩余的字节数组=============
                    int remainLength = (int)m_receiveStream.Length - currFullMsgLength;
                    if (remainLength > 0)
                    {
                        //有剩余字节
                        m_receiveStream.Position = currFullMsgLength;
                        //定义剩余数组
                        byte[] remainBuffer = new byte[remainLength];
                        //把剩余数组读到流中
                        m_receiveStream.Read(remainBuffer, 0, remainLength);
                        //清空数据流
                        m_receiveStream.Position = 0;
                        m_receiveStream.SetLength(0);
                        //把剩余的数组重新写入数据流
                        m_receiveStream.Write(remainBuffer, 0, remainBuffer.Length);
                    }
                    else
                    {
                        //清空数据流
                        m_receiveStream.Position = 0;
                        m_receiveStream.SetLength(0);
                        break;
                    }

                }
                else
                {
                    //还没有收到一个完整包
                    break;
                }
            }
        }

        #endregion
        #endregion

        #region 封装数据包 数据包分为包头和包体，其中包头是包体的长度

        /// <summary>
        /// 封装数据包 数据包分为包头和包体，其中包头是包体的长度
        /// </summary>
        /// <param id="data"></param>
        /// <returns></returns>
        private byte[] MakeSendData(byte[] data)
        {
            //先压缩
            bool isCompress = data.Length > m_CompressLen ? true : false;
            byte[] buffer = data;
            if (isCompress)
            {
                buffer = ZlibHelper.CompressBytes(data);
            }
            //再加密
            buffer = GameEncryption.AESEncryptionByte(buffer);
            //再CRC校验
            ushort crc = Crc16.CalculateCrc16(buffer);
            byte[] retBuffer = null;
            using (GameMemoryStream ms = new GameMemoryStream())
            {
                ms.WriteUshort((ushort)(buffer.Length + 3));
                ms.WriteBool(isCompress);
                ms.WriteUshort(crc);
                ms.Write(buffer, 0, buffer.Length);
                retBuffer = ms.ToArray();
            }

            return retBuffer;
        }

        /// 封装接收数据包
        /// </summary>
        /// <param id="data"></param>
        /// <returns></returns>
        private byte[] MakeReceiveData(byte[] data)
        {
            byte[] buffer = new byte[data.Length - 3];
            int newCrc = Crc16.CalculateCrc16(data);
            using (GameMemoryStream ms = new GameMemoryStream(data))
            {
                bool isCompress = ms.ReadBool();
                ushort crc = ms.ReadUshort();
                ms.Read(buffer, 0, buffer.Length);
                //先CRC校验
                if (newCrc == crc)
                {
                    //在解密
                    buffer = GameEncryption.AESDecryptionByte(buffer);
                    //再解压缩
                    if (isCompress)
                    {
                        buffer = ZlibHelper.DeCompressBytes(buffer);
                    }
                }
            }

            return buffer;


        }

        #endregion

        public void OnClose()
        {
            if (m_Server != null && m_Server.Connected)
            {
                m_Server.Shutdown(SocketShutdown.Both);
                m_Server.Close();
            }
        }
    }
}
