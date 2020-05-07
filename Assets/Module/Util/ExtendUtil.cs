/*
 * 脚本名称：Util
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 20:36:59
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Module;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Module
{
    public static class ExtendUtil
    {
        #region Type

        public static Type GetRoot(this Type type)
        {
            Type t = type;
            while (true)
            {
                if (t.BaseType == null||t.BaseType == typeof(object))
                {
                    break;
                }
                t = t.BaseType;
            }

            return t;
        }

        public static bool IsChild(this Type type,Type target)
        {
            Type t = type;

            while (t.BaseType != null)
            {
                if (t == target)
                {
                    return true;
                }

                t = t.BaseType;
            }

            return false;
        }

        #endregion
        
        #region Object
        public static int[] ToIntArray(this object[] array)
        {
            int[] temp = new int[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                temp[i] = array[i].ToInt();
            }

            return temp;
        }

        /// <summary>
        /// string类型转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this object obj) where T : struct
        {
            T defaultValue = default(T);
            if (obj is string)
            {
                if (!string.IsNullOrEmpty(obj.ToString()))
                {
                    if (!Enum.TryParse(obj.ToString(), true, out defaultValue))
                    {
                        defaultValue = default(T);
                    }
                }
            }
            else if (obj is int)
            {
                return (T)obj;
            }


            return defaultValue;
        }

        /// <summary>
        ///     ToFloat
        /// </summary>
        /// <param id="value">value</param>
        /// <returns>retValue</returns>
        public static float ToFloat(this object value)
        {
            if (value == null) return 0;
            return Convert.ToSingle(value);
        }


        /// <summary>
        ///     把string类型转为int类型
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static int ToInt(this object str)
        {
            int tmp = 0;
            if (str == null) return 0;
            int.TryParse(str.ToString(), out tmp);
            return tmp;
        }


        public static double ToDouble(this object obj)
        {
            double temp = 0;
            if (obj == null) return 0;
            Double.TryParse(obj.ToString(), out temp);
            return temp;
        }

        public static long ToLong(this object obj)
        {
            long temp = 0;
            if (obj == null) return 0;
            Int64.TryParse(obj.ToString(), out temp);
            return temp;
        }

        public static DateTime ToDateTime(this object obj)
        {
            DateTime temp = default(DateTime);
            if (obj == null) return temp;
            DateTime.TryParse(obj.ToString(), out temp);
            return temp;
        }

        public static bool ToBool(this object obj)
        {
            bool temp;
            if (obj == null) return false;
            Boolean.TryParse(obj.ToString(), out temp);
            return temp;
        }

        public static T DeepCopy<T>(this T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }

            return (T)retval;
            //            string json = JsonMapper.ToJson(obj);
            //            return JsonMapper.ToObject<T>(json);
        }

        public static byte[] ToBuffer(this object obj)
        {
            if (obj == null) return new byte[0];
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                ms.Close();
                return ms.GetBuffer();
            }
        }

        public static T ToObject<T>(this byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                return default(T);
            }

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                IFormatter iFormatter = new BinaryFormatter();
                T obj = (T)iFormatter.Deserialize(ms);
                return obj;
            }
        }

        #endregion

        #region Quaternion

        public static string ToDatestring(this Quaternion q, char spite)
        {
            return string.Join(spite.ToString(), q.x, q.y, q.z, q.w);
        }

        #endregion

        #region Color

        public static string ToDataString(this Color v, char spite)
        {
            return string.Join(spite.ToString(), v.r, v.g, v.b, v.a);
        }

        #endregion

        #region Vector3

        public static string ToDataString(this Vector4 v, char spite)
        {
            return string.Join(spite.ToString(), v.x, v.y, v.z, v.w);
        }

        public static string ToDataString(this Vector3 v, char spite)
        {
            return string.Join(spite.ToString(), v.x, v.y, v.z);
        }

        public static string ToDataString(this Vector2 v, char spite)
        {
            return string.Join(spite.ToString(), v.x, v.y);
        }

        public static Vector3 Clamp(this Vector3 v, Vector3 from, Vector3 to)
        {
            float x = Mathf.Clamp(v.x + from.x, from.x, to.x);
            float y = Mathf.Clamp(v.y + from.y, from.y, to.y);
            float z = Mathf.Clamp(v.z + from.z, from.z, to.z);

            if (x > 180)
            {
                x -= 360;
            }

            if (y > 180)
            {
                y -= 360;
            }

            if (z > 180)
            {
                z -= 360;
            }

            return new Vector3(x, y, z);
        }

        #endregion

        #region Matrix4x4

        public static string ToDataString(this Matrix4x4 v, char spite0, char spite1)
        {
            string row0 = v.GetRow(0).ToDataString(spite1);
            string row1 = v.GetRow(1).ToDataString(spite1);
            string row2 = v.GetRow(2).ToDataString(spite1);
            return string.Join(spite0.ToString(), row0, row1, row2);
        }

        public static Vector3 ToPostion(this Matrix4x4 v)
        {
            return v.GetRow(0);
        }

        public static Vector3 ToRotation(this Matrix4x4 v)
        {
            return v.GetRow(1);
        }

        public static Vector3 ToLocalscale(this Matrix4x4 v)
        {
            return v.GetRow(2);
        }

        #endregion

        #region Transform

        public static Tweener DoCurvePath(this Transform target, Vector3 direction, AnimationCurve curve, float duation)
        {
            return null;
            //Vector3[] path=curve.Evaluate()
        }

        public static void DOLookAt(this Transform target, Vector3 toward, Vector3 direction, float distance, float duation)
        {
            target.DORotate(Quaternion.LookRotation(direction).eulerAngles, duation);
            Vector3 moveTo = toward - direction.normalized * distance;
            target.DOMove(moveTo, duation);
        }

        /// <summary>
        ///     得到物体的路径
        /// </summary>
        /// <param id="tra"></param>
        /// <returns></returns>
        public static string GetPathInScene(this Transform tra)
        {
            string path = string.Empty;
            Transform parent = tra;
            while (true)
            {
                if (parent != null)
                {
                    path = parent.name + "/" + path;
                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return path.Substring(0, path.Length - 1);
        }

        public static Transform AddEmptyChild(this Transform tra, string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(tra);
            return obj.transform;
        }

        public static void SetParentZero(this Transform tra, Transform parent)
        {
            tra.SetParent(parent);
            tra.localEulerAngles = Vector3.zero;
            tra.localPosition = Vector3.zero;
        }

        public static void SetPosZero(this Transform tra)
        {
            tra.localEulerAngles = Vector3.zero;
            tra.localPosition = Vector3.zero;
        }

        public static void ClearChild(this Transform tra)
        {
            int length = tra.childCount;
            for (int i = 0; i < length; i++)
            {
                GameObject.Destroy(tra.GetChild(i).gameObject);
            }
        }

        public static void ClearChildImmediate(this Transform tra)
        {
            for (int i = tra.childCount - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(tra.GetChild(i).gameObject);
            }
        }

        public static Transform FindOrNew(this Transform tra, string name)
        {
            Transform tr = tra.Find(name);
            if (tr == null)
            {
                tr = new GameObject(name).transform;
                tr.SetParent(tra);
            }

            return tr;
        }

        public static Matrix4x4 ToMatrix4x4(this Transform tr)
        {
            Matrix4x4 m = new Matrix4x4();
            m.SetRow(0, tr.position);
            m.SetRow(1, tr.eulerAngles);
            m.SetRow(2, tr.lossyScale);
            return m;
        }

        /// <summary>
        ///     根据类来找父物体,只能返回离子物体最近的父物体  包含自身
        /// </summary>
        /// <typeparam id="T"></typeparam>
        /// <param id="tra"></param>
        /// <returns></returns>
        public static T FindParent<T>(this Transform tra)
        {
            Transform parent = tra;
            while (true)
            {
                if (parent == null)
                {
                    return default(T);
                }

                if (parent.GetComponent<T>() == null)
                {
                    parent = parent.parent;
                }
                else
                {
                    break;
                }
            }

            return parent.GetComponent<T>();
        }



        /// <summary>
        ///     设置某物体的层
        /// </summary>
        /// <param id="tra"></param>
        /// <param id="index"></param>
        /// <param id="includeChild"></param>
        public static void SetLayer(this Transform tra, int index, bool includeChild = false)
        {
            if (!includeChild)
            {
                tra.gameObject.layer = index;
            }
            else
            {
                Transform[] children = tra.GetComponentsInChildren<Transform>(true);
                int length = children.Length;
                for (int i = 0; i < length; i++)
                {
                    children[i].gameObject.layer = index;
                }
            }
        }

        public static void SetLayer(this Transform tra, string name, bool includeChild = false)
        {
            int index = LayerMask.NameToLayer(name);
            tra.SetLayer(index, includeChild);
        }

        #endregion

        #region Component

        public static void SetTag(this Component com, string tag, bool includChild)
        {
            if (includChild)
            {
                Component[] child = com.GetComponentsInChildren<Component>();
                com.tag = tag;
                for (int i = 0; i < child.Length; i++)
                {
                    child[i].tag = tag;
                }
            }
            else
            {
                com.tag = tag;
            }
        }

        #endregion

        #region GameObject

        public static T GetScript<T>(this GameObject obj) where T : ViewBehaviour
        {
            return (T)obj.GetComponent<ViewReference>().target;
        }
        
        /// <summary>
        /// 获取或创建组建
        /// </summary>
        /// <typeparam id="T"></typeparam>
        /// <param id="obj"></param>
        /// <returns></returns>
        public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
        {
            T t = obj.GetComponent<T>();
            if (t == null)
            {
                t = obj.AddComponent<T>();
            }

            return t;
        }

        public static Component AddOrGetComponent(this GameObject obj, Type type)
        {
            Component t = obj.GetComponent(type);
            if (t == null)
            {
                t = obj.AddComponent(type);
            }

            return t;
        }

        #endregion

        #region int

        public static string SpiteDot(this int value)
        {
            // string result = "";
            // if (System.Math.Abs(value) < 1000)
            // {
            //     result = value.ToString();
            // }
            // else
            // {
            //     result = value.ToString().Substring(value.ToString().Length - 3, 3);
            // }
            // int quotient = value / 1000;
            // if (System.Math.Abs(quotient) > 0)
            // {
            //     result = SpiteDot(quotient) + "," + result;
            // }

            string result = Math.Abs(value).ToString();

            int length = result.Length;
            for (int i = length - 3, j = 3; i > 0; i = i - 3, j = j + 3)
            {
                result = result.Insert(i, ",");
            }

            if (value < 0)
            {
                result = "-" + result;
            }

            return result;
        }

        public static int Clamp(this int value, int from, int to)
        {
            if (from > to)
            {
                int temp = to;
                to = from;
                from = temp;
            }

            if (value < from)
            {
                return from;
            }
            else if (value > to)
            {
                return to;
            }
            else
            {
                return value;
            }
        }

        #endregion

        #region float

        public static float Clamp(this float value, float from, float to)
        {
            if (from > to)
            {
                float temp = to;
                to = from;
                from = temp;
            }

            if (value < from)
            {
                return from;
            }
            else if (value > to)
            {
                return to;
            }
            else
            {
                return value;
            }
        }

        public static float Spite(this float value, float from, float to)
        {
            float ToValue = to - value;
            float ValueFrom = value - from;

            if (ToValue > ValueFrom)
            {
                return from;
            }
            else
            {
                return to;
            }
        }

        #endregion

        #region string

        public static Vector4 ToVector(this string str, char spite)
        {
            if (string.IsNullOrEmpty(str)) return Vector4.zero;
            string[] sp = str.Split(spite);
            if (sp.Length == 1)
            {
                return new Vector4(sp[0].ToFloat(), 0);
            }
            else if (sp.Length == 2)
            {
                return new Vector4(sp[0].ToFloat(), sp[1].ToFloat());
            }
            else if (sp.Length == 3)
            {
                return new Vector4(sp[0].ToFloat(), sp[1].ToFloat(), sp[2].ToFloat());
            }
            else
            {
                return new Vector4(sp[0].ToFloat(), sp[1].ToFloat(), sp[2].ToFloat(), sp[3].ToFloat());
            }
        }

        public static Color ToColor(this string str, char spite)
        {
            if (string.IsNullOrEmpty(str)) return Color.white;
            string[] sp = str.Split(spite);
            return new Color(sp[0].ToFloat(), sp[1].ToFloat(), sp[2].ToFloat(), sp[3].ToFloat());
        }

        public static Quaternion ToQuaternion(this string str, char spite)
        {
            if (string.IsNullOrEmpty(str)) return Quaternion.identity;
            string[] sp = str.Split(spite);
            return new Quaternion(sp[0].ToFloat(), sp[1].ToFloat(), sp[2].ToFloat(), sp[3].ToFloat());
        }

        public static Matrix4x4 ToMatrix4x4(this string str, char spite0, char spite1)
        {
            if (string.IsNullOrEmpty(str)) return Matrix4x4.zero;
            string[] sp = str.Split(spite0);
            Matrix4x4 m = new Matrix4x4();

            for (int i = 0; i < sp.Length; i++)
            {
                Vector4 v = sp[i].ToVector(spite1);
                m.SetRow(i, v);
            }

            return m;
        }

        public static float[] ToFloatArray(this string str, char spite)
        {
            string[] tar = str.Split(spite);
            float[] res = new float[tar.Length];
            for (int i = 0; i < tar.Length; i++)
            {
                res[i] = tar[i].ToFloat();
            }

            return res;
        }

        #region 获取url的参数

        public static string GetHttpArgs(this string str, string key)
        {
            Uri uir = new Uri(str);
            string queryString = uir.Query;
            NameValueCollection col = GetQueryString(queryString);
            string searchKey = col[key];
            return searchKey;
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param id="queryString"></param>
        /// <returns></returns>
        private static NameValueCollection GetQueryString(string queryString)
        {
            return GetQueryString(queryString, null, true);
        }

        /// <summary>
        /// 将查询字符串解析转换为名值集合.
        /// </summary>
        /// <param id="queryString"></param>
        /// <param id="encoding"></param>
        /// <param id="isEncoded"></param>
        /// <returns></returns>
        private static NameValueCollection GetQueryString(string queryString, Encoding encoding, bool isEncoded)
        {
            queryString = queryString.Replace("?", "");
            NameValueCollection result = new NameValueCollection(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(queryString))
            {
                string[] args = queryString.Split('&');
                for (int i = 0; i < args.Length; i++)
                {
                    string[] arg = args[i].Split('=');

                    result.Add(arg[0], arg[1]);
                }
            }

            return result;
        }

        #endregion

        #endregion

        #region List
        public static void ClearNull<T>(this List<T> lst)
        {
            for (int i = lst.Count - 1; i >= 0; i--)
            {
                if (lst[i] == null)
                {
                    lst.RemoveAt(i);
                }
            }
        }
        public static void DefineCount<T>(this List<T> lst, int count, T defaultValue)
        {
            if (lst == null) return;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    lst.Add(defaultValue);
                }
            }
            else if (count < 0)
            {
                for (int i = 0; i < -count; i++)
                {
                    lst.RemoveAt(lst.Count - 1);
                }
            }
        }
        public static void Move<T>(this List<T> lst, T value, int index)
        {
            if (!lst.Contains(value)) return;
            int targetIndex = 0;
            for (int i = 0; i < lst.Count; i++)
            {
                if (lst[i].Equals(value))
                {
                    targetIndex = i;
                    break;
                }
            }

            if (targetIndex > index)
            {
                lst.Remove(value);
                lst.Insert(index, value);
            }
            else if (targetIndex < index)
            {
                lst.Insert(index + 1, value);
                lst.Remove(value);
            }
        }

        public static bool Contains<T>(this List<T> list, Predicate<T> target)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (target.Invoke(list[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static T GetLast<T>(this List<T> array)
        {
            return array[array.Count - 1];
        }

        public static bool IsNullOrEmpty<T>(this List<T> lst)
        {
            if (lst == null) return true;
            if (lst.Count == 0) return true;
            return false;
        }

        public static bool IsNullOrEmpty<T>(this Stack<T> lst)
        {
            if (lst == null) return true;
            if (lst.Count == 0) return true;
            return false;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for(int i = list.Count - 1; i >= 0; i--)
            {
                int random = UnityEngine.Random.Range(0, i);
                T t = list[i];
                list[i] = list[random];
                list[random] = t;
                
            }
        }

        public static T Random<T>(this List<T> array)
        {
            int index = UnityEngine.Random.Range(0, array.Count);
            return array[index];
        }

        // public static T RandomRemove<T>(this List<T> array)
        // {
        //     if (array.IsNullOrEmpty()) return default;
        //     int index = UnityEngine.Random.Range(0, array.Count);
        //     T target = array[index];
        //     array.RemoveAt(index);
        //     return target;
        // }

        // public static T RandomRemove<T>(this List<T> array, Predicate<T> predicate)
        // {
        //     if (array.IsNullOrEmpty()) return default;
        //
        //     for (int i = array.Count - 1; i >= 0; i--)
        //     {
        //         int index = UnityEngine.Random.Range(0, array.Count);
        //         T target = array[index];
        //         if (!predicate.Invoke(target))
        //         {
        //             array.RemoveAt(index);
        //         }
        //     }
        //     for (int i = 0; i < array.Count; i++)
        //     {
        //         int index = UnityEngine.Random.Range(0, array.Count);
        //         T target = array[index];
        //         if (predicate.Invoke(target))
        //         {
        //             array.RemoveAt(index);
        //         }
        //         else
        //         {
        //             return target;
        //         }
        //     }
        //
        //
        //     return default;
        // }


        public static T Random<T>(this List<T> array, Predicate<T> predicate)
        {
            for (int i = 0; i < array.Count; i++)
            {
                int index = UnityEngine.Random.Range(0, array.Count);
                if (predicate.Invoke(array[index]))
                {
                    return array[index];
                }
            }

            return default(T);
        }

        public static T GetNext<T>(this List<T> array, T value, bool loop)
        {
            int index = 0;
            for (index = 0; index < array.Count; index++)
            {
                if (array[index].Equals(value))
                {
                    break;
                }
            }

            if (loop)
            {
                return array[index + 1 % array.Count];
            }
            else
            {
                index++;
                index = Mathf.Clamp(index, index, array.Count);
                return array[index];
            }
        }

        public static T GetPrevious<T>(this List<T> array, T value, bool loop)
        {
            int index = 0;
            for (index = 0; index < array.Count; index++)
            {
                if (array[index].Equals(value))
                {
                    break;
                }
            }

            if (loop)
            {
                return array[index - 1 % array.Count];
            }
            else
            {
                index--;
                index = Mathf.Clamp(index, index, array.Count);
                return array[index];
            }
        }

        public static void RemoveBack<T>(this List<T> array, T value)
        {
            for (int i = array.Count - 1; i >= 0; i--)
            {
                if (array[i].Equals(value))
                {
                    array.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region Array

        public static List<T> Copy<T>(this List<T> obj)
        {
            List<T> list = new List<T>();
            foreach (var item in obj)
            {
                list.Add(item);
            }
            return list;
        }

        public static T ToFind<T>(this List<T> obj, Predicate<T> predicate)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                if (predicate(obj[i]))
                {
                    return obj[i];
                }
            }

            return default(T);
        }

        public static List<T> ToFindAll<T>(this List<T> obj, Predicate<T> predicate)
        {
            List<T> temp = new List<T>();
            for (int i = 0; i < obj.Count; i++)
            {
                if (predicate(obj[i]))
                {
                    temp.Add(obj[i]);
                }
            }

            return temp;
        }

        public static T Find<T>(this T[] obj, System.Predicate<T> predicate)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                if (predicate(obj[i]))
                {
                    return obj[i];
                }
            }

            return default(T);
        }

        public static T[] FindAll<T>(this T[] obj, System.Predicate<T> predicate)
        {
            List<T> temp = new List<T>();
            for (int i = 0; i < obj.Length; i++)
            {
                if (predicate(obj[i]))
                {
                    temp.Add(obj[i]);
                }
            }

            return temp.ToArray();
        }

        public static void ToForEach<T>(this List<T> obj, Action<T> action)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                action(obj[i]);
            }
        }

        public static void ForEach<T>(this T[] obj, Action<T> action)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                action(obj[i]);
            }
        }

        public static bool IsNullOrEmpty<T>(this T[] lst)
        {
            if (lst == null) return true;
            if (lst.Length == 0) return true;
            for (int i = 0; i < lst.Length; i++)
            {
                if (lst[i] != null)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsAllNull<T>(this T[] lst)
        {
            if (lst.IsNullOrEmpty()) return true;
            for (int i = 0; i < lst.Length; i++)
            {
                if (lst[i] != null) return false;
            }

            return true;
        }

        public static void Clear<T>(this T[] lst)
        {
            if (lst == null) return;
            for (int i = 0; i < lst.Length; i++)
            {
                lst[i] = default(T);
            }
        }

        // public static void ClearNull<T>(this List<T> lst)
        // {
        //     for (int i = lst.Count - 1; i >= 0; i--)
        //     {
        //         if (lst[i] == null)
        //         {
        //             lst.RemoveAt(i);
        //         }
        //     }
        // }

        public static T Random<T>(this T[] array)
        {
            int index = UnityEngine.Random.Range(0, array.Length - 1);
            return array[index];
        }

        public static T Random<T>(this T[] array, out int index)
        {
            index = UnityEngine.Random.Range(0, array.Length - 1);
            return array[index];
        }

        public static List<T> Random<T>(this List<T> array, int count, Predicate<T> predicate)
        {
            List<T> target = new List<T>();
            for (int i = 0; i < array.Count; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    target.Add(array[i]);
                }
            }

            if (target.Count <= count)
            {
                return target;
            }
            else
            {
                List<T> result = new List<T>();
                while (result.Count < count)
                {
                    int index = UnityEngine.Random.Range(0, target.Count);
                    if (!result.Contains(target[index]))
                    {
                        result.Add(target[index]);
                    }
                }

                return result;
            }
        }

        public static T[] Random<T>(this T[] array, int count, Predicate<T> predicate)
        {
            List<T> target = new List<T>(array);
            return target.Random(count, predicate).ToArray();
        }

        public static T GetNext<T>(this T[] array, T value, bool loop)
        {
            int index = 0;
            for (index = 0; index < array.Length; index++)
            {
                if (array[index].Equals(value))
                {
                    break;
                }
            }

            if (loop)
            {
                return array[index + 1 % array.Length];
            }
            else
            {
                index++;
                index = Mathf.Clamp(index, index, array.Length);
                return array[index];
            }
        }

        public static T GetPrevious<T>(this T[] array, T value, bool loop)
        {
            int index = 0;
            for (index = 0; index < array.Length; index++)
            {
                if (array[index].Equals(value))
                {
                    break;
                }
            }

            if (loop)
            {
                return array[index - 1 % array.Length];
            }
            else
            {
                index--;
                index = Mathf.Clamp(index, index, array.Length);
                return array[index];
            }
        }

        public static T GetLast<T>(this T[] array)
        {
            return array[array.Length - 1];
        }

        #endregion
        
        #region MemberInfo

        public static object GetValue(this MemberInfo member, object[] args)
        {
            switch (member)
            {
                case FieldInfo _:
                    return (member as FieldInfo).GetValue(member);
                case PropertyInfo _:
                    return (member as PropertyInfo).GetValue(member);
                case MethodBase _:
                    return (member as MethodBase).Invoke(member, args);
                default:
                    throw new ArgumentException("Can't get the value of a " + member.GetType().Name);
            }
        }

        #endregion
        
    }
}