/*
 * 脚本名称：GameMemoryStream
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 22:30:15
 * 脚本作用：
*/

using System;
using System.IO;
using System.Text;

namespace Module
{
    /// <summary>
    /// 数据转换 byte short int long float decimal bool string
    /// </summary>
    public class GameMemoryStream : MemoryStream
    {
        #region 构造方法

        public GameMemoryStream() : base()
        {
        }

        public GameMemoryStream(short arg)
        {
            WriteShort(arg);
        }

        public GameMemoryStream(ushort arg)
        {
            WriteUshort(arg);
        }

        public GameMemoryStream(int arg)
        {
            WriteInt(arg);
        }

        public GameMemoryStream(uint arg)
        {
            WriteUint(arg);
        }

        public GameMemoryStream(long arg)
        {
            WriteLong(arg);
        }

        public GameMemoryStream(ulong arg)
        {
            WriteUlong(arg);
        }

        public GameMemoryStream(float arg)
        {
            WriteFloat(arg);
        }

        public GameMemoryStream(double arg)
        {
            WriteDouble(arg);
        }

        public GameMemoryStream(bool arg)
        {
            WriteBool(arg);
        }

        public GameMemoryStream(string arg)
        {
            WriteUTF8String(arg);
        }

        public GameMemoryStream(byte[] buffer) : base(buffer)
        {
        }

        #endregion

        #region short

        /// <summary>
        /// 从流中读取一个short数组
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            byte[] arr = new byte[2];
            ////base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 2);
            return BitConverter.ToInt16(arr, 0);
        }

        /// <summary>
        /// 把一个short写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteShort(short value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region ushort

        /// <summary>
        /// 从流中读取一个ushort数组
        /// </summary>
        /// <returns></returns>
        public ushort ReadUshort()
        {
            byte[] arr = new byte[2];
            ////base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 2);
            return BitConverter.ToUInt16(arr, 0);
        }

        /// <summary>
        /// 把一个ushort写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteUshort(ushort value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region int

        /// <summary>
        /// 从流中读取一个short数组
        /// </summary>
        /// <returns></returns>
        public int ReadInt()
        {
            byte[] arr = new byte[4];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 4);
            return BitConverter.ToInt32(arr, 0);
        }

        /// <summary>
        /// 把一个short写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteInt(int value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region uint

        /// <summary>
        /// 从流中读取一个ushort数组
        /// </summary>
        /// <returns></returns>
        public uint ReadUint()
        {
            byte[] arr = new byte[4];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 4);
            return BitConverter.ToUInt32(arr, 0);
        }

        /// <summary>   
        /// 把一个ushort写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteUint(uint value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region long

        /// <summary>
        /// 从流中读取一个short数组
        /// </summary>
        /// <returns></returns>
        public long ReadLong()
        {
            byte[] arr = new byte[8];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 8);
            return BitConverter.ToInt64(arr, 0);
        }

        /// <summary>
        /// 把一个short写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteLong(long value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region ulong

        /// <summary>
        /// 从流中读取一个ushort数组
        /// </summary>
        /// <returns></returns>
        public ulong ReadUlong()
        {
            byte[] arr = new byte[8];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 8);
            return BitConverter.ToUInt64(arr, 0);
        }

        /// <summary>   
        /// 把一个ushort写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteUlong(ulong value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region float

        /// <summary>
        /// 从流中读取一个short数组
        /// </summary>
        /// <returns></returns>
        public float ReadFloat()
        {
            byte[] arr = new byte[4];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 4);
            return BitConverter.ToSingle(arr, 0);
        }

        /// <summary>
        /// 把一个short写入流
        /// </summary>
        /// <param buffId="value"></param>
        public void WriteFloat(float value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region double

        /// <summary>
        /// 从流中读取一个ushort数组
        /// </summary>
        /// <returns></returns>
        public double ReadDouble()
        {
            byte[] arr = new byte[8];
            //base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, 8);
            return BitConverter.ToDouble(arr, 0);
        }

        /// <summary>   
        /// 把一个ushort写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteDouble(double value)
        {
            byte[] arr = BitConverter.GetBytes(value);
            base.Write(arr, 0, arr.Length);
        }

        #endregion

        #region bool

        /// <summary>
        /// 从流中读取一个short数组
        /// </summary>
        /// <returns></returns>
        public bool ReadBool()
        {
            base.Seek(0,SeekOrigin.Begin);
            return base.ReadByte() == 1;
        }

        /// <summary>
        /// 把一个short写入流
        /// </summary>
        /// <param id="value"></param>
        public void WriteBool(bool value)
        {
            byte t = (byte) (value == true ? 1 : 0);
            this.WriteByte(t);
        }

        #endregion

        #region String

        /// <summary>
        /// 从流中读取一个string数组 数据分两部分
        /// </summary>
        /// <returns></returns>
        public string ReadUTF8String()
        {
            ushort len = this.ReadUshort();
            byte[] arr = new byte[len];
            ////base.Seek(0, SeekOrigin.Begin);
            base.Read(arr, 0, len);
            return Encoding.UTF8.GetString(arr);
        }

        /// <summary>   
        /// 把一个string写入流 
        /// </summary>
        /// <param id="value"></param>
        public void WriteUTF8String(string value)
        {
            byte[] arr = Encoding.UTF8.GetBytes(value);
            if (arr.Length > 65535)
            {
                throw new InvalidCastException("字符串超出范围");
            }

            this.WriteUshort((ushort)arr.Length);
            base.Write(arr, 0, arr.Length);
        }

        #endregion
    }
}

