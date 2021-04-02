/*
 * 脚本名称：GameEncryption
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-06 20:08:31
 * 脚本作用：
*/

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
namespace Module
{
    public class GameEncryption
    {

        private static Encoding _encoding
        {
            get { return Encoding.UTF8; }
        }
        #region 获取钥匙

        /// <summary>
        /// 获取钥匙
        /// </summary>
        /// <param id="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static byte[] GetAesKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "获取无效");
            }

            if (key.Length < 32)
            {
                // ????32??ȫ
                key = key.PadRight(32, '0');
            }
            if (key.Length > 32)
            {
                key = key.Substring(0, 32);
            }
            return Encoding.UTF8.GetBytes(key);
        }


        #endregion

        #region AES加密解密
        private static string AesKey = "hzzdcm522Hl30l'3-jgm5]3&89%kJHl";
        #region byte

        /// <summary>
        /// ??byte??????м???
        /// </summary>
        /// <param id="buffer"></param>
        /// <returns></returns>
        public static byte[] AESEncryptionByte(byte[] buffer)
        {
            using (Aes aesProvider = new AesManaged())
            {
                aesProvider.Key = GetAesKey(AesKey);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] results = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                    return results;
                }
            }
        }

        /// <summary>
        /// ??byte??????н???
        /// </summary>
        /// <param id="buffer"></param>
        /// <returns></returns>
        public static byte[] AESDecryptionByte(byte[] buffer)
        {
            using (Aes aesProvider = new AesManaged())
            {
                aesProvider.Key = GetAesKey(AesKey);
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] results = cryptoTransform.TransformFinalBlock(buffer, 0, buffer.Length);
                    aesProvider.Clear();
                    return results;
                }
            }
        }

        #endregion

        #region string

        /// <summary>
        /// ??string???м???
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static string AESEncryptionString(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            byte[] result = AESEncryptionByte(buffer);
            return Convert.ToBase64String(result, 0, result.Length);
        }

        /// <summary>
        /// ??string???н???
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static string AESDecryptionString(string str)
        {
            byte[] buffer = str.ToBuffer();
            byte[] result = AESDecryptionByte(buffer);
            return Encoding.UTF8.GetString(result);
        }

        #endregion


        #endregion

        private static string BsToStr(byte[] bs) => string.Join(string.Empty, bs.Select(x => x.ToString("x2"))).Replace("-", string.Empty);

        #region MD5加密

        /// <summary>
        /// 获取MD5
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = md5.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 获取md5
        /// </summary>
        /// <param id="_bs"></param>
        /// <returns></returns>
        public static string GetMD5(byte[] _bs)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var bs = md5.ComputeHash(_bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 读取文件MD5
        /// </summary>
        /// <param id="fileName"></param>
        /// <returns></returns>
        public static string GetFileMD5(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                using (var md5 = new MD5CryptoServiceProvider())
                {
                    var bs = md5.ComputeHash(file);
                    return BsToStr(bs);
                }
            }
        }

        #endregion

        /// <summary>
        /// 获取sha1
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static string GetSHA1(string str)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = sha1.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param id="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string GetSHA256(string str)
        {
            using (var Sha256 = new SHA256CryptoServiceProvider())
            {
                var bs = _encoding.GetBytes(str);
                bs = Sha256.ComputeHash(bs);
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 获取hmac md5
        /// </summary>
        /// <param id="str"></param>
        /// <param Id="password"></param>
        /// <returns></returns>
        public static string GetHMACMD5(string str, string password)
        {
            using (var hmac_md5 = new HMACMD5())
            {
                hmac_md5.Key = _encoding.GetBytes(password);
                var bs = hmac_md5.ComputeHash(_encoding.GetBytes(str));
                return BsToStr(bs);
            }
        }

        /// <summary>
        /// 已测试
        /// </summary>
        /// <param id="str"></param>
        /// <param id="key"></param>
        /// <returns></returns>
        public static string GetHMACSHA256(string str, string key)
        {
            using (var hmac_sha256 = new HMACSHA256(_encoding.GetBytes(key)))
            {
                var data = hmac_sha256.ComputeHash(_encoding.GetBytes(str));
                return BitConverter.ToString(data);
            }
        }

        /// <summary>
        /// 获取hmac sha1
        /// </summary>
        /// <param id="str"></param>
        /// <param id="password"></param>
        /// <returns></returns>
        public static string GetHMACSHA1(string str, string password)
        {
            using (var hmac_sha1 = new HMACSHA1())
            {
                hmac_sha1.Key = _encoding.GetBytes(password);
                var bs = hmac_sha1.ComputeHash(_encoding.GetBytes(str));
                return BsToStr(bs);
            }
        }

        #region DES加密解密
        private static readonly string txtKey = "PatrickpanP=";
        private static readonly string txtIV = "LiuJineagel=";

        public static string DESEncrypt(string Text, string key) => DESEncrypt(Text, key, key);
        public static string DESDecrypt(string Text, string key) => DESDecrypt(Text, key, key);

        /// <summary>
        /// 加密数据
        /// </summary>
        public static string DESEncrypt(string Text, string txtKey, string txtIV)
        {
            using (var des = new DESCryptoServiceProvider())
            {
                byte[] inputByteArray;
                inputByteArray = _encoding.GetBytes(Text);
                //des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                //des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                des.Key = Convert.FromBase64String(txtKey);
                des.IV = Convert.FromBase64String(txtIV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        StringBuilder ret = new StringBuilder();
                        foreach (byte b in ms.ToArray())
                        {
                            ret.AppendFormat("{0:X2}", b);
                        }
                        return ret.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 解密数据
        /// </summary>
        public static string DESDecrypt(string Text, string txtKey, string txtIV)
        {
            using (var des = new DESCryptoServiceProvider())
            {
                int len;
                len = Text.Length / 2;
                byte[] inputByteArray = new byte[len];
                int x, i;
                for (x = 0; x < len; x++)
                {
                    i = Convert.ToInt32(Text.Substring(x * 2, 2), 16);
                    inputByteArray[x] = (byte)i;
                }
                //des.Key = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                //des.IV = ASCIIEncoding.ASCII.GetBytes(System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(sKey, "md5").Substring(0, 8));
                des.Key = Convert.FromBase64String(txtKey);
                des.IV = Convert.FromBase64String(txtIV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        return _encoding.GetString(ms.ToArray());
                    }
                }
            }
        }

        #endregion
    }
}
