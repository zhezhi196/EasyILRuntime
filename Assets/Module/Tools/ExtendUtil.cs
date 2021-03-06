/*
 * 脚本名称：Util
 * 项目名称：FrameWork
 * 脚本作者：黄哲智
 * 创建时间：2017-12-21 20:36:59
 * 脚本作用：
*/

using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
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
                if (t.BaseType == null || t.BaseType == typeof(object))
                {
                    break;
                }

                t = t.BaseType;
            }

            return t;
        }

        public static bool IsChild(this Type type, Type target)
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
        
        public static Sprite ToSprite(this Object obj)
        {
            Texture2D tex = (Texture2D) obj;
            Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            return sp;
        }
        
        public static List<int> ToIntList(this IList array)
        {
            List<int> temp = new List<int>();

            for (int i = 0; i < array.Count; i++)
            {
                temp.Add(array[i].ToInt());
            }

            return temp;
        }

        public static float[] ToFloatArray(this IList array)
        {
            float[] temp = new float[array.Count];
            for (int i = 0; i < array.Count; i++)
            {
                temp[i] = array[i].ToFloat();
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

                    return defaultValue;
                }
            }

            return (T)obj;
        }

        /// <summary>
        ///     ToFloat
        /// </summary>
        /// <param id="value">value</param>
        /// <returns>retValue</returns>
        public static float ToFloat(this object value)
        {
            if (value == null) return 0;
            if (value is long || value is float || value is double || value is int)
            {
                return (float) value;
            }
            float temp = 0;
            float.TryParse(value.ToString(), out temp);
            return temp;
        }


        /// <summary>
        ///     把string类型转为int类型
        /// </summary>
        /// <param id="str"></param>
        /// <returns></returns>
        public static int ToInt(this object obj)
        {
            if (obj == null) return 0;
            if (obj is long || obj is float || obj is double || obj is int || obj is bool || obj is Enum)
            {
                return Convert.ToInt32(obj);
            }

            int tmp = 0;
            int.TryParse(obj.ToString(), out tmp);
            return tmp;
        }


        public static double ToDouble(this object obj)
        {
            if (obj == null) return 0;
            if (obj is long || obj is float || obj is double || obj is int || obj is bool || obj is Enum)
            {
                return Convert.ToDouble(obj);
            }
            double temp = 0;
            Double.TryParse(obj.ToString(), out temp);
            return temp;
        }

        public static long ToLong(this object obj)
        {
            if (obj == null) return 0;
            if (obj is long || obj is float || obj is double || obj is int || obj is bool || obj is Enum)
            {
                return Convert.ToInt64(obj);
            }
            long temp = 0;
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
            if (obj is long || obj is int)
            {
                return Convert.ToBoolean((int)obj);
            }
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

            return (T) retval;
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
                T obj = (T) iFormatter.Deserialize(ms);
                return obj;
            }
        }

        #endregion

        #region Vector3

        public static float Distance(this Vector3 v1, Vector3 target)
        {
            return Vector3.Distance(v1, target);
        }
        public static float Distance(this Vector3 v1)
        {
            return Vector3.Distance(v1, Vector3.zero);
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

        #region Transform

        public static void Sort<T>(this Transform layout, Comparison<T> compare, T[] children = null) where T: Component
        {
            if (children == null)
            {
                children = layout.transform.GetComponentsInChildren<T>(true);
            }

            children.Sort(compare);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].transform.SetSiblingIndex(i);
            }
        }


        public static void Sort<T>(this Transform layout, T[] children = null) where T : Component
        {
            if (children == null)
            {
                children = layout.transform.GetComponentsInChildren<T>(true);
            }

            for (int i = 0; i < children.Length; i++)
            {
                children[i].transform.SetSiblingIndex(i);
            }
        }
        public static bool IsForward2Target(this Transform player, Vector3 target)
        {
            Vector3 playerDir = player.transform.forward;
            Vector3 jiaoPoint = Tools.GetIntersectWithLineAndPlane(target, Vector3.down,
                player.transform.up, player.transform.position);
            return Vector3.Dot(jiaoPoint - player.transform.position, playerDir) >= 0;
        }

        public static float DistanceToTarget(this Transform player, Vector3 target)
        {
            Vector3 jiaoPoint = Tools.GetIntersectWithLineAndPlane(target, Vector3.down,
                player.up, player.position);
            return Vector3.Distance(player.position, jiaoPoint);
        }

        public static Transform[] GetNameChildren(this Transform transform, string name, bool includeUnactive)
        {
            List<Transform> result = new List<Transform>();

            Transform[] all = transform.GetComponentsInChildren<Transform>(includeUnactive);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].name.Contains(name))
                {
                    result.Add(all[i]);
                }
            }

            return result.ToArray();
        }
        
        public static Transform GetNameChild(this Transform transform, string name,bool includeUnactive=true)
        {
            
            Transform[] all = transform.GetComponentsInChildren<Transform>(includeUnactive);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].name.Contains(name))
                {
                    return all[i];
                }
            }

            return null;
        }
        public static FollowTween DoFollow(this Transform tra, Transform target, float speed)
        {
            return new FollowTween(tra, target, speed);
        }
        
        public static FollowTween DoFollow(this Transform tra, Transform target, float speed,Vector3 offset)
        {
            return new FollowTween(tra, target, speed,offset);
        }
        
        public static Tweener DOLookAt(this Transform target, Vector3 toward, Vector3 direction, float distance,
            float duation)
        {
            target.DORotate(Quaternion.LookRotation(direction).eulerAngles, duation);
            Vector3 moveTo = toward - direction.normalized * distance;
            return target.DOMove(moveTo, duation);
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
            if (parent == null || parent.gameObject == null|| tra==null|| tra.gameObject==null)
            {
                return;
            }
            tra.SetParent(parent);
            if (parent != null)
            {
                tra.eulerAngles = parent.eulerAngles;
                tra.position = parent.position;
            }
            else
            {
                tra.eulerAngles = Vector3.zero;
                tra.position = Vector3.zero;
            }
        }

        public static void SetPosZero(this Transform tra)
        {
            tra.localEulerAngles = Vector3.zero;
            tra.localPosition = Vector3.zero;
        }

        public static void SetPosAllZero(this Transform tra)
        {
            tra.localEulerAngles = Vector3.zero;
            tra.localPosition = Vector3.zero;
            tra.localScale = Vector3.one;
        }

        public static void ClearChild(this Transform tra)
        {
            if (tra == null) return;
            int length = tra.childCount;

            for (int i = 0; i < length; i++)
            {
                AssetLoad.Destroy(tra.GetChild(i).gameObject);
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

        public static Transform NewChild(this Transform tra, string name)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(tra);
            go.transform.position = tra.position;
            go.transform.eulerAngles = tra.eulerAngles;
            return go.transform;
        }
        
#endregion

        #region Component

        /// <summary>
        ///     设置某物体的层
        /// </summary>
        /// <param id="tra"></param>
        /// <param id="index"></param>
        /// <param id="includeChild"></param>
        public static void SetLayer(this Component tra, int index, bool includeChild = false)
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

        public static void SetLayer(this Component tra, string name, bool includeChild = false)
        {
            int index = LayerMask.NameToLayer(name);
            tra.SetLayer(index, includeChild);
        }

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

        #region Image

        public static void SetAlpha(this Image alpha, float value)
        {
            alpha.color = new Color(alpha.color.r, alpha.color.g, alpha.color.b, value);
        }

        #endregion

        #region GameObject

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

        public static void OnActive(this GameObject obj, bool active)
        {
            if (obj != null)
            {
                obj.SetActive(active);
            }
            else
            {
                GameDebug.LogErrorFormat("{0} is null ", obj);
            }
        }

        public static void OnActive(this Transform obj, bool active)
        {
            obj.gameObject.OnActive(active);
        }

        #endregion

        #region int

        public static string SpiteDot(this int value)
        {
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

        public static string ToPrice(this float value)
        {
            string priceStr = value.ToString();
            if (!priceStr.Contains("."))
            {
                return value.ToString("C0");
            }
            else
            {
                int dot = priceStr.IndexOf('.');
                return value.ToString("C" + (priceStr.Length - dot - 1));
            }
        } 
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

        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null || str == String.Empty) return true;
            return false;
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

        public static Vector3 ToVector(this string str)
        {
            str = str.Substring(1, str.Length - 2);
            string[] res = str.Split(',');
            return new Vector3(res[0].ToFloat(), res[1].ToFloat(), res[2].ToFloat());
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

        public static void Sort<T>(this IList<T> list,Comparison<T> compare)
        {
            if (list is List<T>)
            {
                ((List<T>) list).Sort(compare);
            }
            else
            {
                List<T> objList = new List<T>((IEnumerable<T>) list);
                objList.Sort(compare);
                for (int index = 0; index < list.Count; ++index)
                    list[index] = objList[index];
            }
        }
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

        public static void MoveLastOrAdd<T>(this List<T> array, T value)
        {
            if (array == null) return;
            if (array.Count > 0 && array.GetLast().Equals(value)) return;
            for (int i = 0; i < array.Count; i++)
            {
                if (array[i].Equals(value))
                {
                    array.RemoveAt(i);
                    break;
                }
            }

            array.Add(value);
        }

        public static bool IsNullOrEmpty(this IList lst)
        {
            if (lst == null) return true;
            if (lst.Count == 0) return true;
            return false;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                int random = UnityEngine.Random.Range(0, i);
                T t = list[i];
                list[i] = list[random];
                list[random] = t;
            }
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

        public static void ForEach<T>(this T[] obj, Action<T> action)
        {
            for (int i = 0; i < obj.Length; i++)
            {
                action(obj[i]);
            }
        }

        public static void Clear<T>(this T[] lst)
        {
            if (lst == null) return;
            for (int i = 0; i < lst.Length; i++)
            {
                lst[i] = default(T);
            }
        }

        public static int GetCount<T>(this T[] array,Predicate<T> predicate)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    count++;
                }
            }

            return count;
        }
        public static int GetCount<T>(this List<T> array,Predicate<T> predicate)
        {
            int count = 0;
            for (int i = 0; i < array.Count; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    count++;
                }
            }

            return count;
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

        public static bool Contains<T>(this T[] array,T value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Contains<T>(this T[] array, Predicate<T> predicate)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    return true;
                }
            }

            return false;
        }

        public static (List<T>, List<T>, List<T>) Comparision<T>(this List<T> resource, List<T> target)
        {
            List<T> result1 = new List<T>(resource);
            List<T> result2 = new List<T>();
            List<T> result3 = new List<T>(target);
            
            for (int i = 0; i < resource.Count; i++)
            {
                for (int j = 0; j < target.Count; j++)
                {
                    if (resource[i].Equals(target[j]))
                    {
                        result2.Add(resource[i]);
                        result1.Remove(resource[i]);
                        result3.Remove(target[j]);
                    }
                }
            }

            return (result1, result2, result3);
        }
        #endregion

        #region Dic

        public static void SetOrAdd<T, K>(this Dictionary<T, K> dic, T key, K value)
        {
            if (dic.ContainsKey(key))
            {
                dic[key] = value;
            }
            else
            {
                dic.Add(key, value);
            }
        }
        

        #endregion

        #region DateTime

        public static bool IsNewDay(this DateTime newDay, DateTime last)
        {
            if (newDay.Year == last.Year)
            {
                if (newDay.Month == last.Month)
                {
                    return newDay.Day > last.Day;
                }
                else
                {
                    return newDay.Month > last.Month;
                }
            }
            else
            {
                return newDay.Year > last.Year;
            }
        }

        #endregion

        #region Matrix4x4

        public static Quaternion GetRotation(this Matrix4x4 matrix4X4)
        {
            float qw = Mathf.Sqrt(1f + matrix4X4.m00 + matrix4X4.m11 + matrix4X4.m22)/2;
            float w = 4*qw;
            float qx = (matrix4X4.m21 - matrix4X4.m12)/w;
            float qy = (matrix4X4.m02 - matrix4X4.m20)/w;
            float qz = (matrix4X4.m10 - matrix4X4.m01)/w;
            return new Quaternion(qx, qy, qz, qw);
        }

        public static Vector3 GetPostion(this Matrix4x4 matrix4X4)
        {
            var x = matrix4X4.m03;
            var y = matrix4X4.m13;
            var z = matrix4X4.m23;
            return new Vector3(x,y,z);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            var x = Mathf.Sqrt(m.m00*m.m00 + m.m01*m.m01 + m.m02*m.m02);
            var y = Mathf.Sqrt(m.m10*m.m10 + m.m11*m.m11 + m.m12*m.m12);
            var z = Mathf.Sqrt(m.m20*m.m20 + m.m21*m.m21 + m.m22*m.m22);
            return new Vector3(x,y,z);
        }

        #endregion

    }
}