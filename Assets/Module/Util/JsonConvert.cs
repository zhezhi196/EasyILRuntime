using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace Module
{
    public static class JsonConvert
    {
        public static JsonData ToJson(object value,Dictionary<string,bool> switchStation)
        {
            Type type = value.GetType();
            FieldInfo[] field = type.GetFields();
            JsonData data = new JsonData();
            for (int i = 0; i < field.Length; i++)
            {
                if (switchStation[field[i].Name])
                {
                    GetTypeJson(data,field[i].Name,field[i].GetValue(value));
                }
            }

            return data;
        }
        //
        // public static T ToObject<T>(JsonData data) where T: new()
        // {
        //
        //     Type type = typeof(T);
        //     T target = new T();
        //     
        //     FieldInfo[] fieldInfos = type.GetFields();
        //     for (int i = 0; i < fieldInfos.Length; i++)
        //     {
        //         string colorSpite = null;
        //         try
        //         {
        //             colorSpite= data[fieldInfos[i].Name].ToString();
        //         }
        //         catch (Exception e)
        //         {
        //             continue; 
        //         }
        //
        //         if (fieldInfos[i].FieldType == typeof(Color))
        //         {
        //             string[] spite = colorSpite.Split('_');
        //             fieldInfos[i].SetValue(target,new Color(spite[0].ToFloat(),spite[1].ToFloat(),spite[2].ToFloat(),spite[3].ToFloat()));
        //         }
        //         else if (fieldInfos[i].FieldType == typeof(Vector4))
        //         {
        //             string[] spite = colorSpite.Split('_');
        //             fieldInfos[i].SetValue(target,new Vector4(spite[0].ToFloat(),spite[1].ToFloat(),spite[2].ToFloat(),spite[3].ToFloat()));
        //         }
        //         else if (fieldInfos[i].FieldType == typeof(Vector3)||fieldInfos[i].FieldType==typeof(Vector3Int))
        //         {
        //             string[] spite = colorSpite.Split('_');
        //             fieldInfos[i].SetValue(target,new Vector3(spite[0].ToFloat(),spite[1].ToFloat(),spite[2].ToFloat()));
        //         }    
        //         else if (fieldInfos[i].FieldType == typeof(Vector2)||fieldInfos[i].FieldType==typeof(Vector2Int))
        //         {
        //             string[] spite = colorSpite.Split('_');
        //             fieldInfos[i].SetValue(target,new Vector2(spite[0].ToFloat(),spite[1].ToFloat()));
        //         }   
        //         else if (fieldInfos[i].FieldType == typeof(Transform))
        //         {
        //             string[] spite = colorSpite.Split('#');
        //             string[] posString = spite[0].Split('_');
        //             string[] eagurString = spite[1].Split('_');
        //             GameObject g = new GameObject();
        //             g.transform.position = new Vector3(posString[0].ToFloat(), posString[1].ToFloat(),
        //                 posString[2].ToFloat());
        //             g.transform.eulerAngles = new Vector3(eagurString[0].ToFloat(), eagurString[1].ToFloat(),
        //                 eagurString[2].ToFloat());
        //             fieldInfos[i].SetValue(target, g.transform);
        //         }
        //         else if (fieldInfos[i].FieldType==typeof(int))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToString().ToInt());
        //         }   
        //         else if (fieldInfos[i].FieldType==typeof(float))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToString().ToFloat());
        //         }  
        //         else if (fieldInfos[i].FieldType==typeof(double))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToString().ToDouble());
        //         } 
        //         else if (fieldInfos[i].FieldType==typeof(string))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToString());
        //         } 
        //         else if (fieldInfos[i].FieldType==typeof(long))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToLong());
        //         } 
        //         else if (fieldInfos[i].FieldType==typeof(bool))
        //         {
        //             fieldInfos[i].SetValue(target,data[fieldInfos[i].Name].ToBool());
        //         } 
        //     }
        //
        //     return target;
        //
        // }

        private static string GetJsonString(object fieldValue)
        {
            if (fieldValue == null) return null;
            if (fieldValue is Color)
            {
                Color c = (Color) fieldValue;
                return string.Join("_", c.r, c.g, c.b, c.a);
            }
            else if (fieldValue is Vector4)
            {
                Vector4 c = (Vector4) fieldValue;
                return string.Join("_", c.x, c.y, c.z, c.w);
            }
            else if (fieldValue is Vector3 || fieldValue is Vector3Int)
            {
                Vector3 c = (Vector3) fieldValue;
                return string.Join("_", c.x, c.y, c.z);
            }
            else if (fieldValue is Vector2 || fieldValue is Vector2Int)
            {
                Vector2 c = (Vector2) fieldValue;
                return string.Join("_", c.x, c.y);
            }
            else if (fieldValue is Transform)
            {
                Transform c = (Transform) fieldValue;
                return string.Join("#", string.Join("_", c.position.x, c.position.y, c.position.z),
                    string.Join("_", c.eulerAngles.x, c.eulerAngles.y, c.eulerAngles.z));
            }
            else
            {
                return fieldValue.ToString();
            }
        }

        private static void GetTypeJson(JsonData data,string fieldName,object fieldValue)
        {
            if (fieldValue == null) return;

            if (fieldValue is string|| fieldValue is Enum)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.String);
            }
            else if (fieldValue is int)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Int);
            }
            else if (fieldValue is long)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Long);
            }
            else if (fieldValue is float || fieldValue is double)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Double);
            }
            else if (fieldValue is bool)
            {
                data[fieldName] = fieldValue.ToString();
                data[fieldName].SetJsonType(JsonType.Boolean);
            }
            else if (fieldValue is Array || fieldValue is IList)
            {
                IEnumerable target = (IEnumerable) fieldValue;
                data[fieldName] = new JsonData();
                data[fieldName].SetJsonType(JsonType.Array);
                foreach (object VARIABLE in target)
                {
                    data[fieldName].Add(GetJsonString(VARIABLE));
                }
            }
            else if (fieldValue is Color)
            {
                Color c = (Color) fieldValue;
                data[fieldName] = string.Join("_", c.r, c.g, c.b, c.a);
            }
            else if (fieldValue is Vector4)
            {
                Vector4 c = (Vector4) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y, c.z, c.w);
            }
            else if (fieldValue is Vector3 || fieldValue is Vector3Int)
            {
                Vector3 c = (Vector3) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y, c.z);
            }
            else if (fieldValue is Vector2 || fieldValue is Vector2Int)
            {
                Vector2 c = (Vector2) fieldValue;
                data[fieldName] = string.Join("_", c.x, c.y);
            }
            else if (fieldValue is Transform)
            {
                Transform c = (Transform) fieldValue;
                data[fieldName] = string.Join("#", string.Join("_", c.position.x, c.position.y, c.position.z),
                    string.Join("_", c.eulerAngles.x, c.eulerAngles.y, c.eulerAngles.z));
            }
            else
            {
                Type type = fieldValue.GetType();
                FieldInfo[] fildInfo = type.GetFields();
                for (int i = 0; i < fildInfo.Length; i++)
                {
                    GetTypeJson(data[fieldName], fildInfo[i].Name, fildInfo[i].GetValue(fieldValue));
                }
            }
            
        }
    }
}