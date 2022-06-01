/*
 * 脚本名称：Common
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 18:39:07
 * 脚本作用：
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    public static class Tools
    {
        public static readonly MD5 md5 = MD5.Create();
        
        public static int GetChannelBit(string rawChannelStr)
        {
            if (rawChannelStr.IsNullOrEmpty())
            {
                return -1;
            }
            
            int ret = 0;
            var split = rawChannelStr.Split('|');
            for (int i = 0; i < split.Length; i++)
            {
                int temp = int.Parse(split[i]);
                ret = ret | (1 << temp);
            }
            // GameDebug.Log($"{rawChannelStr}:{ret}");
            return ret;
        }
        #region 对函数进行压力测试

        ///     对函数进行压力测试
        /// </summary>
        /// <param id="num"></param>
        /// <param id="e"></param>
        public static void ProfilerTest(int num, Action e)
        {
            float time = Time.realtimeSinceStartup;
            for (int i = 0; i < num; i++)
            {
                e();
            }

            GameDebug.Log(Time.realtimeSinceStartup - time);
        }

        #endregion

        #region 相机视口坐标的计算

        public static Vector3[] GetCorners(UnityEngine.Camera uiCamera, float distance)
        {
            Vector3[] corners = new Vector3[4];
            Transform tx = uiCamera.transform;
            float halfFOV = uiCamera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float aspect = uiCamera.aspect;
            float height = distance * Mathf.Tan(halfFOV);
            float width = height * aspect;

            // UpperLeft
            corners[0] = tx.position - tx.right * width;
            corners[0] += tx.up * height;
            corners[0] += tx.forward * distance;

            // UpperRight
            corners[1] = tx.position + tx.right * width;
            corners[1] += tx.up * height;
            corners[1] += tx.forward * distance;

            // LowerLeft
            corners[2] = tx.position - tx.right * width;
            corners[2] -= tx.up * height;
            corners[2] += tx.forward * distance;

            // LowerRight
            corners[3] = tx.position + tx.right * width;
            corners[3] -= tx.up * height;
            corners[3] += tx.forward * distance;
            return corners;
        }

        #endregion

        #region 对比两个数组的不同

        /// <summary>
        ///     对比两个数组的不同
        /// </summary>
        /// <typeparam id="T"></typeparam>
        /// <param id="current"></param>
        /// <param id="next"></param>
        /// <returns></returns>
        public static List<T>[] CompareList<T>(T[] current, T[] next)
        {
            List<T>[] itemLists = new List<T>[3];
            itemLists[0] = new List<T>(); //增加
            itemLists[1] = new List<T>(); //不变
            itemLists[2] = new List<T>(); //删除
            int length1 = current.Length;
            int length2 = next.Length;
            for (int i = 0; i < length1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    if (!current[i].Equals(next[j]))
                    {
                        if (j == length2 - 1)
                        {
                            itemLists[2].Add(current[i]);
                            break;
                        }

                        continue;
                    }
                    else
                    {
                        itemLists[1].Add(current[i]);
                        break;
                    }
                }
            }

            itemLists[0] = next.ToList();
            for (int i = 0; i < itemLists[1].Count; i++)
            {
                if (itemLists[0].Contains(itemLists[1][i]))
                {
                    itemLists[0].Remove(itemLists[1][i]);
                }
            }

            if (length2 == 0)
            {
                itemLists[2] = current.ToList();
            }

            return itemLists;
        }

        #endregion

        #region 对数组进行偏移

        /// <summary>
        ///     对数组进行偏移
        /// </summary>
        /// <typeparam id="T"></typeparam>
        /// <param id="array"></param>
        /// <param id="offect"></param>
        /// <returns></returns>
        public static T[] OffectArray<T>(T[] array, int offect)
        {
            int length = array.Length;
            T[] sort = new T[length];
            if (offect < 0)
            {
                offect = length + offect;
            }

            for (int i = 0; i < length; i++)
            {
                if (i >= offect)
                {
                    sort[i] = array[i - offect];
                }
                else
                {
                    sort[i] = array[length - offect + i];
                }
            }

            return sort;
        }

        #endregion

        #region 其他

        public static string ToHash(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var t in data)
                sb.Append(t.ToString("x2"));
            return sb.ToString();
        }

        public static string GetFileMD5(string path)
        {
            using (var fs = File.OpenRead(path))
            {
                var data = md5.ComputeHash(fs);
                return ToHash(data);
            }
        }
        public static string GetFileMD5(byte[] buffer)
        {
            var data = md5.ComputeHash(buffer);
            return ToHash(data);
        }

        /// <summary>
        /// 判断字符串中是否包含中文
        /// </summary>
        /// <param name="str">需要判断的字符串</param>
        /// <returns>判断结果</returns>
        public static bool HasChinese(string str)
        {
            return Regex.IsMatch(str, @"[\u4e00-\u9fa5]");
        }

        public static bool IsNumber(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            const string pattern = "^[0-9]*$";
            Regex rx = new Regex(pattern);
            return rx.IsMatch(str);
        }
        
        public static int Get2Power(int index)
        {
            return (int)Mathf.Pow(2, index);
        }

        public static Vector3 GetScreenScale()
        {
            return Mathf.Clamp01((float) (1920 * Screen.height) / (1080 * Screen.width)) * Vector3.one;
        }
        
        public static Material GenerateMaterial(Shader shader)
        {
            if (shader == null)
                return null;
            //需要判断shader是否支持
            if (shader.isSupported == false)
                return null;
            Material material = new Material(shader);
            material.hideFlags = HideFlags.DontSave;
            if (material)
                return material;
            return null;

        }

        /// <summary>
        /// 计算直线与平面的交点
        /// </summary>
        /// <param name="point">直线上某一点</param>
        /// <param name="direct">直线的方向</param>
        /// <param name="planeNormal">垂直于平面的的向量</param>
        /// <param name="planePoint">平面上的任意一点</param>
        /// <returns></returns>
        public static Vector3 GetIntersectWithLineAndPlane(Vector3 point, Vector3 direct, Vector3 planeNormal, Vector3 planePoint)
        {
            float d = Vector3.Dot(planePoint - point, planeNormal) / Vector3.Dot(direct.normalized, planeNormal);
            //print(d);
            return d * direct.normalized + point;
        }
        /// <summary>
        /// 获得一个贝塞尔曲线
        /// </summary>
        /// <param name="pos0">起点</param>
        /// <param name="pos1">中间点</param>
        /// <param name="pos3">重点</param>
        /// <param name="count">曲线段数</param>
        /// <returns></returns>
        public static List<Vector3> GetBezierPath(Vector3 pos0, Vector3 pos1, Vector3 pos3, int count)
        {
            List<Vector3> paths = new List<Vector3>();
            float t = 0f;
            Vector3 B = new Vector3(0, 0, 0);
            for (int i = 0; i < count; i++)
            {
                B = (1 - t) * (1 - t) * pos0 + 2 * (1 - t) * t * pos1 + t * t * pos3;
                paths.Add(B);
                t += (1 / (float)count);
            }
            paths.Add(pos3);
            return paths;
        }
        
        /// <summary>
        /// 写入文件到本地  适用于文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="data"></param>
        public static void WriteFile(string path, string data)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
                File.WriteAllText(path, data);
            }
            catch (Exception e)
            {
                GameDebug.LogError("写入文件异常：" + e.Message);
            }
        }

        /// <summary>
        /// 读取文件 适用于文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ReadFie(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return File.ReadAllText(path);
                }
                else
                {
                    GameDebug.LogError("文件不存在：" + path);
                    return null;
                }
            }
            catch (Exception e)
            {
                GameDebug.LogError("读取文件异常：" + e.Message);
            }

            return null;
        }

        public static void Float2Time(float t, out int d, out int h, out int m, out int s)
        {
            d = 0;
            h = 0;
            m = 0;
            if (t >= 86400) //天,
            {
                d = (int)(t / 86400);
                h = (int)((t % 86400) / 3600);
                m = (int)((t % 86400 % 3600) / 60);
                s = (int)(t % 86400 % 3600 % 60);

            }
            else if (t >= 3600)//时,
            {
                h = (int)(t / 3600);
                m = (int)((t % 3600) / 60);
                s = (int)(t % 3600 % 60);
            }
            else if (t >= 60)//分
            {
                m = (int)(t / 60);
                s = (int)(t % 60);
            }
            else
            {
                s = (int)t;
            }
        }
        public static void Float2Time(float t, out int h, out int m, out int s)
        {
            h = 0;
            m = 0;
            if (t >= 86400) //天,
            {
                //d = Convert.ToInt16(t / 86400);
                h = (int)((t % 86400) / 3600);
                m = (int)((t % 86400 % 3600) / 60);
                s = (int)(t % 86400 % 3600 % 60);

            }
            else if(t >= 3600)//时,
            {
                h = (int)(t / 3600);
                m = (int)((t % 3600) / 60);
                s = (int)(t % 3600 % 60);
            }
            else if (t >= 60)//分
            {
                m = (int)(t / 60);
                s = (int) (t % 60);
            }
            else
            {
                s =(int)t;
            }
        }
        
        public static int SpiteID(int id, int pos, int offset)
        {
            return (id / Pow(pos - offset)) % Pow(offset);
        }

        private static int Pow(int count)
        {
            int temp = 1;
            for (int i = 0; i < count; i++)
            {
                temp = temp * 10;
            }

            return temp;
        }

        public static float GetPercent(int index, params float[] weight)
        {
            if (weight.IsNullOrEmpty()) return 0;
            float sum = weight.Sum();
            return weight[index] / sum;
        }

        #endregion

        public static bool ContainRotateBounds(Bounds bounds, Transform boundsPoint, Vector3 point)
        {
            return ContainRotateBounds(bounds, boundsPoint.transform.position, boundsPoint.rotation, point);
        }

        public static bool ContainRotateBounds(Bounds bounds, Vector3 boundsPoint, Quaternion rotation, Vector3 point)
        {
            Bounds tempBounds = new Bounds(boundsPoint + bounds.center, bounds.size);
            DrawTools.DrawBounds(tempBounds, rotation, Color.green, 0.1f);
            return tempBounds.ContainPoint(point, rotation);
        }
        
        public static void RemoveClone(GameObject go)
        {
            string name = go.name;
            go.name = name.Substring(0, name.Length - 7);
        }

        public static int GetID<T>(T[] exit) where T : Identify<int>
        {
            int resuitId = 0;
            while (true)
            {
                if (!exit.Contains(fd => fd.ID == resuitId))
                {
                    return resuitId;
                    return resuitId;
                }
                else
                {
                    resuitId++;
                }
            }
        }
    }
}
