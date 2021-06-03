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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module
{
    public static class Tools
    {
        public static readonly MD5 md5 = MD5.Create();
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

        #region 随机一个概率是否中奖

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

        #endregion

        public static int Get2Power(int index)
        {
            return (int)Mathf.Pow(2, index);
        }

        public static float GetScreenScale()
        {
            return ((float)(1920 * Screen.height) / (1080 * Screen.width));
        }
        
        public static long MinLong(params long[] arg)
        {
            long temp = long.MaxValue;
            for (int i = 0; i < arg.Length; i++)
            {
                if (temp > arg[i])
                {
                    temp = arg[i];
                }
            }

            return temp;
        }

        public static long MaxLong(params long[] arg)
        {
            long temp = long.MinValue;
            for (int i = 0; i < arg.Length; i++)
            {
                if (temp < arg[i])
                {
                    temp = arg[i];
                }
            }

            return temp;
        }
        public static void ClearLog()
        {
            // var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
            // var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            // clearMethod.Invoke(null, null);
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
    }
}
