﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    public static class RandomHelper
    {
        public static List<T> Random<T>(this IList<T> array, int count, Predicate<T> predicate)
        {
            List<T> temp = new List<T>();
            for (int i = 0; i < array.Count; i++)
            {
                if (predicate.Invoke(array[i]))
                {
                    temp.Add(array[i]);
                }
            }

            return Random(temp, count);
        }

        public static List<T> Random<T>(this IList<T> array, int count)
        {
            List<T> list = new List<T>(array);
            int currentIndex;
            T tempValue;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                currentIndex = UnityEngine.Random.Range(0, i + 1);
                tempValue = list[currentIndex];
                list[currentIndex] = list[i];
                list[i] = tempValue;
            }

            list.RemoveRange(count, array.Count - count);
            return list;
        }

        public static T Random<T>(this IList<T> array, Predicate<T> predicate)
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

        public static T Random<T>(this IList<T> array)
        {
            int index = UnityEngine.Random.Range(0, array.Count);
            return array[index];
        }
        
        public static bool RandomBool()
        {
            int temp = UnityEngine.Random.Range(0, 1);
            return temp == 0;
        }
        
        public static int RandomSymbol()
        {
            int temp = UnityEngine.Random.Range(0, 1);
            if (temp == 0) return -1;
            return temp;
        }
        
        public static int RandomWeight(params float[] rate)
        {
            float sum = 0;
            for (int i = 0; i < rate.Length; i++)
            {
                sum += rate[i];
            }

            float value = UnityEngine.Random.Range(0f, sum);
            float org = 0;
            for (int i = 0; i < rate.Length; i++)
            {
                if (value >= org && value < org + rate[i])
                {
                    return i;
                }
                else
                {
                    org += rate[i];
                }
            }

            return -1;
        }
        
        public static bool RandomValue(float value)
        {
            if (value == 1) return true;
            if (value == 0) return false;
            float v = UnityEngine.Random.Range(0f, 1f);
            return v <= value;
        }


        public static Vector3 RandomVector3(Vector3 min, Vector3 max)
        {
            float x = UnityEngine.Random.Range(min.x, max.x);
            float y = UnityEngine.Random.Range(min.y, max.y);
            float z = UnityEngine.Random.Range(min.z, max.z);
            return new Vector3(x, y, z);
        }
        
        /// <summary>
        /// 从rate中按照权重随机count个值出来
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<int> RandomSeveralWeight(IList<float> rate, int count)
        {
            List<int> value = new List<int>();
            if (count >= rate.Count)
            {
                for (int i = 0; i < rate.Count; i++)
                {
                    value.Add(i);
                }
                return value;
            }
            float[] temp = new float[rate.Count];
            for (int i = 0; i < rate.Count; i++)
            {
                temp[i] = rate[i];
            }
            for (int i = 0; i < count; i++)
            {
                int index = RandomWeight(temp);
                temp[index] = 0;
                value.Add(index);
            }
            return value;
        }
    }
}