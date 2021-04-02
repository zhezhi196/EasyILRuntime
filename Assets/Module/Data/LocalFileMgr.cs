/*
 * 脚本名称：LocalFileMgr
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2018-01-02 16:29:38
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Module
{
    public class LocalFileMgr
    {
        #region string

        public static void SetString(string key, string value)
        {
            //string temp = GameEncryption.AESEncryptionString(value);
            PlayerPrefs.SetString(key, value);
        }

        public static string GetString(string key)
        {
            if (!ContainKey(key))
            {
                return null;
            }

            string temp = PlayerPrefs.GetString(key);
            return temp;//GameEncryption.AESDecryptionString(temp);
        }

        #endregion

        #region int

        public static void SetInt(string key, int value)
        {

            //string temp = GameEncryption.AESEncryptionString(value.ToString());
            PlayerPrefs.SetInt(key, value);
        }

        public static int GetInt(string key, int defaultValue)
        {
            if (!ContainKey(key))
            {
                return defaultValue;
            }
            int temp = PlayerPrefs.GetInt(key);
            return temp;//GameEncryption.AESDecryptionString(temp).ToInt();
        }

        public static int GetInt(string key)
        {
            if (!ContainKey(key))
            {
                GameDebug.LogError(string.Format("本地不存在{0}的值", key));
                return 0;
            }
            int temp = PlayerPrefs.GetInt(key);
            return temp;//GameEncryption.AESDecryptionString(temp).ToInt();
        }

        #endregion

        #region float

        public static void SetFloat(string key, float value)
        {
            //string temp = GameEncryption.AESEncryptionString(value.ToString());
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloat(string key)
        {
            if (!ContainKey(key))
            {
                GameDebug.LogError(string.Format("本地不存在{0}的值", key));
                return 0;
            }
            float temp = PlayerPrefs.GetFloat(key);
            return temp;
            //return GameEncryption.AESDecryptionString(temp).ToFloat();
        }

        #endregion

        public static void SetDateTime(string key, DateTime value)
        {
            //string temp = GameEncryption.AESEncryptionString(value.ToString());
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static void Record(string key)
        {
            PlayerPrefs.SetString(key, string.Empty);
        }

        public static DateTime GetDateTime(string key)
        {
            string temp = PlayerPrefs.GetString(key);
            return temp.ToDateTime();
            //return GameEncryption.AESDecryptionString(temp).ToDateTime();
        }
        #region bool

        public static void SetBool(string key, bool value)
        {
            //string temp = GameEncryption.AESEncryptionString(value.ToString());
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static bool GetBool(string key)
        {
            if (!ContainKey(key))
            {
                GameDebug.LogError(string.Format("本地不存在{0}的值", key));
                return false;
            }
            string temp = PlayerPrefs.GetString(key);
            return temp.ToBool();
            //return GameEncryption.AESDecryptionString(temp).ToBool();
        }



        public static bool RemoveKey(string key)
        {
            if (ContainKey(key))
            {
                PlayerPrefs.DeleteKey(key);
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region StringArray

        public static string[] GetStringArray(string key)
        {
            if (!ContainKey(key))
            {
                GameDebug.LogError(string.Format("本地不存在{0}的值", key));
                return null;
            }

            string temp = GetString(key);
            string[] spite = temp.Split(',');
            return spite;
        }

        public static void AddStringArray(string key, string value)
        {
            string array = value;
            if (ContainKey(key))
            {
                array = GetString(key);
                array = string.Format("{0},{1}", array, value);
            }

            SetString(key, array);
        }

        public static void RemoveStringArrayAtIndex(string key, int index)
        {
            List<string> lst = new List<string>(GetStringArray(key));
            lst.RemoveAt(index);
            string temp = string.Join(",", lst);
            SetString(key, temp);
        }

        #endregion

        public static bool ContainKey(string id)
        {
            return PlayerPrefs.HasKey(id);
        }

        #region 获取本地buffer

        /// <summary>
        /// 获取本地buffer
        /// </summary>
        /// <param id="path"></param>
        /// <returns></returns>
        public static byte[] GetBuffer(string path)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        public static string PicToBase64(string url)
        {
            FileStream fs = new System.IO.FileStream(url, FileMode.Open, FileAccess.Read);
            byte[] thebytes = new byte[fs.Length];

            fs.Read(thebytes, 0, (int)fs.Length);
            //转为base64string
            string base64_texture2d = Convert.ToBase64String(thebytes);
            return base64_texture2d;
        }

        #endregion


        public static void RemoveAllKey()
        {
            PlayerPrefs.DeleteAll();
        }

        public static Vector2 GetVector2(string key,char spite)
        {
            string[] temp = GetString(key).Split(spite);
            return new Vector2(temp[0].ToFloat(), temp[1].ToFloat());
        }
        public static void SetVector2(string key, Vector2 value1, string spite0)
        {
            SetString(key, string.Join(spite0, value1.x.ToString(), value1.y.ToString()));
        }
        
        public static Vector2Int GetVector2Int(string key, char spite)
        {
            string[] temp = GetString(key).Split(spite);
            return new Vector2Int(temp[0].ToInt(), temp[1].ToInt());
        }

        public static void SetVector2Int(string key, Vector2Int value1, char spite0)
        {
            SetString(key, string.Join(spite0.ToString(), value1.x.ToString(), value1.y.ToString()));
        }
    }
}
