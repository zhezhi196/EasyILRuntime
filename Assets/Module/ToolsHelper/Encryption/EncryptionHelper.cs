﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace Module
{
    public static class EncryptionHelper
    {
        private static RijndaelManaged rm = new RijndaelManaged
        {
            Mode = CipherMode.ECB,
            Padding = PaddingMode.PKCS7
        };


        public static string Xor(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            if (key.IsNullOrEmpty()) return str;
            char[] res = new char[str.Length];
            
            for (int i = 0; i < str.Length; i++)
            {
                res[i] = (char) (key[i % key.Length] ^ str[i]);
            }

            return new string(res);
        }
        
        public static string AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
            rm.Key = Encoding.UTF8.GetBytes(key);
            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        
        public static string AesDecrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            Byte[] toEncryptArray = Convert.FromBase64String(str);
            rm.Key = Encoding.UTF8.GetBytes(key);
            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        /// <summary>
        /// 字符串MD5加密
        /// </summary>
        /// <param name="Text">要加密的字符串</param>
        /// <returns>密文</returns>
        public static string MD5Encrypt(string Text)
        {
            MD5 md5 = MD5.Create();
            //需要将字符串转成字节数组
            byte[] buffer = Encoding.Default.GetBytes(Text);
            //加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
            byte[] md5buffer = md5.ComputeHash(buffer);
            StringBuilder builder = new StringBuilder();

            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < md5buffer.Length; i++)
            {
                builder.Append(md5buffer[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}